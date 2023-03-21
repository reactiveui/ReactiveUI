// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace ReactiveUI.Fody;

/// <summary>
/// Weaver that replaces properties marked with `[DataMember]` on subclasses of `ReactiveObject` with an
/// implementation that invokes `RaisePropertyChanged` as is required for ReactiveUI.
/// </summary>
public class ReactiveUIPropertyWeaver
{
    /// <summary>
    /// Gets or sets the module definition.
    /// </summary>
    /// <value>
    /// The module definition.
    /// </value>
    public ModuleDefinition? ModuleDefinition { get; set; }

    /// <summary>
    /// Gets or sets a action that will log an MessageImportance.High message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log information.
    /// </value>
    public Action<string>? LogInfo { get; set; }

    /// <summary>
    /// Gets or sets an action that will log an error message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log error.
    /// </value>
    public Action<string>? LogError { get; set; }

    /// <summary>
    /// Executes this property weaver.
    /// </summary>
    /// <exception cref="Exception">
    /// reactiveObjectExtensions is null
    /// or
    /// raiseAndSetIfChangedMethod is null
    /// or
    /// reactiveAttribute is null
    /// or
    /// [Reactive] is decorating " + property.DeclaringType.FullName + "." + property.Name + ", but the property has no setter so there would be nothing to react to.  Consider removing the attribute.
    /// </exception>
    public void Execute()
    {
        if (ModuleDefinition is null)
        {
            LogInfo?.Invoke("The module definition has not been defined.");
            return;
        }

        var reactiveUI = ModuleDefinition.AssemblyReferences.Where(x => x.Name == "ReactiveUI").OrderByDescending(x => x.Version).FirstOrDefault();
        if (reactiveUI is null)
        {
            LogInfo?.Invoke("Could not find assembly: ReactiveUI (" + string.Join(", ", ModuleDefinition.AssemblyReferences.Select(x => x.Name)) + ")");
            return;
        }

        LogInfo?.Invoke($"{reactiveUI.Name} {reactiveUI.Version}");
        var helpers = ModuleDefinition.AssemblyReferences.Where(x => x.Name == "ReactiveUI.Fody.Helpers").OrderByDescending(x => x.Version).FirstOrDefault();
        if (helpers is null)
        {
            LogInfo?.Invoke("Could not find assembly: ReactiveUI.Fody.Helpers (" + string.Join(", ", ModuleDefinition.AssemblyReferences.Select(x => x.Name)) + ")");
            return;
        }

        LogInfo?.Invoke($"{helpers.Name} {helpers.Version}");
        var reactiveObject = new TypeReference("ReactiveUI", "IReactiveObject", ModuleDefinition, reactiveUI);
        var targetTypes = ModuleDefinition.GetAllTypes().Where(x => x.BaseType is not null && reactiveObject.IsAssignableFrom(x.BaseType)).ToArray();
        var reactiveObjectExtensions = new TypeReference("ReactiveUI", "IReactiveObjectExtensions", ModuleDefinition, reactiveUI).Resolve();
        if (reactiveObjectExtensions is null)
        {
            throw new Exception("reactiveObjectExtensions is null");
        }

        var raiseAndSetIfChangedMethod = ModuleDefinition.ImportReference(reactiveObjectExtensions.Methods.Single(x => x.Name == "RaiseAndSetIfChanged"));
        if (raiseAndSetIfChangedMethod is null)
        {
            throw new Exception("raiseAndSetIfChangedMethod is null");
        }

        var reactiveAttribute = ModuleDefinition.FindType("ReactiveUI.Fody.Helpers", "ReactiveAttribute", helpers);
        if (reactiveAttribute is null)
        {
            throw new Exception("reactiveAttribute is null");
        }

        foreach (var targetType in targetTypes)
        {
            foreach (var property in targetType.Properties.Where(x => x.IsDefined(reactiveAttribute)).ToArray())
            {
                if (property.SetMethod is null)
                {
                    LogError?.Invoke($"Property {property.DeclaringType.FullName}.{property.Name} has no setter, therefore it is not possible for the property to change, and thus should not be marked with [Reactive]");
                    continue;
                }

                // Declare a field to store the property value
                var field = new FieldDefinition("$" + property.Name, FieldAttributes.Private, property.PropertyType);
                targetType.Fields.Add(field);

                // Remove old field (the generated backing field for the auto property)
                var oldField = (FieldReference)property.GetMethod.Body.Instructions.Single(x => x.Operand is FieldReference).Operand;
                var oldFieldDefinition = oldField.Resolve();
                targetType.Fields.Remove(oldFieldDefinition);

                // See if there exists an initializer for the auto-property
                var constructors = targetType.Methods.Where(x => x.IsConstructor);
                foreach (var constructor in constructors)
                {
                    var fieldAssignment = constructor.Body.Instructions.SingleOrDefault(x => Equals(x.Operand, oldFieldDefinition) || Equals(x.Operand, oldField));
                    if (fieldAssignment is not null)
                    {
                        // Replace field assignment with a property set (the stack semantics are the same for both,
                        // so happily we don't have to manipulate the bytecode any further.)
                        var setterCall = constructor.Body.GetILProcessor().Create(property.SetMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, property.SetMethod);
                        constructor.Body.GetILProcessor().Replace(fieldAssignment, setterCall);
                    }
                }

                // Build out the getter which simply returns the value of the generated field
                property.GetMethod.Body = new MethodBody(property.GetMethod);
                property.GetMethod.Body.Emit(il =>
                {
                    il.Emit(OpCodes.Ldarg_0);                                   // this
                    il.Emit(OpCodes.Ldfld, field.BindDefinition(targetType));   // pop -> this.$PropertyName
                    il.Emit(OpCodes.Ret);                                       // Return the field value that is lying on the stack
                });

                TypeReference genericTargetType = targetType;
                if (targetType.HasGenericParameters)
                {
                    var genericDeclaration = new GenericInstanceType(targetType);
                    foreach (var parameter in targetType.GenericParameters)
                    {
                        genericDeclaration.GenericArguments.Add(parameter);
                    }

                    genericTargetType = genericDeclaration;
                }

                var methodReference = raiseAndSetIfChangedMethod.MakeGenericMethod(genericTargetType, property.PropertyType);

                // Build out the setter which fires the RaiseAndSetIfChanged method
                if (property.SetMethod is null)
                {
                    throw new Exception("[Reactive] is decorating " + property.DeclaringType.FullName + "." + property.Name + ", but the property has no setter so there would be nothing to react to.  Consider removing the attribute.");
                }

                property.SetMethod.Body = new MethodBody(property.SetMethod);
                property.SetMethod.Body.Emit(il =>
                {
                    il.Emit(OpCodes.Ldarg_0);                                   // this
                    il.Emit(OpCodes.Ldarg_0);                                   // this
                    il.Emit(OpCodes.Ldflda, field.BindDefinition(targetType));  // pop -> this.$PropertyName
                    il.Emit(OpCodes.Ldarg_1);                                   // value
                    il.Emit(OpCodes.Ldstr, property.Name);                      // "PropertyName"
                    il.Emit(OpCodes.Call, methodReference);                     // pop * 4 -> this.RaiseAndSetIfChanged(this.$PropertyName, value, "PropertyName")
                    il.Emit(OpCodes.Pop);                                       // We don't care about the result of RaiseAndSetIfChanged, so pop it off the stack (stack is now empty)
                    il.Emit(OpCodes.Ret);                                       // Return out of the function
                });
            }
        }
    }
}