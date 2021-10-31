// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
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
/// Weaver that converts observables as property helper.
/// </summary>
public class ObservableAsPropertyWeaver
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
    /// Gets a function that will find a type from referenced assemblies by name.
    /// </summary>
    public Func<string, TypeDefinition>? FindType { get; internal set; }

    /// <summary>
    /// Executes this property weaver.
    /// </summary>
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

        var exceptionName = typeof(Exception).FullName;

        if (exceptionName is null)
        {
            LogInfo?.Invoke("Could not find the full name for System.Exception");
            return;
        }

        var reactiveObject = ModuleDefinition.FindType("ReactiveUI", "ReactiveObject", reactiveUI);

        // The types we will scan are subclasses of ReactiveObject
        var targetTypes = ModuleDefinition.GetAllTypes().Where(x => x.BaseType is not null && reactiveObject.IsAssignableFrom(x.BaseType));

        var observableAsPropertyHelper = ModuleDefinition.FindType("ReactiveUI", "ObservableAsPropertyHelper`1", reactiveUI, "T");
        var observableAsPropertyAttribute = ModuleDefinition.FindType("ReactiveUI.Fody.Helpers", "ObservableAsPropertyAttribute", helpers);
        var observableAsPropertyHelperGetValue = ModuleDefinition.ImportReference(observableAsPropertyHelper.Resolve().Properties.Single(x => x.Name == "Value").GetMethod);
        var exceptionDefinition = FindType?.Invoke(exceptionName);
        var constructorDefinition = exceptionDefinition.GetConstructors().Single(x => x.Parameters.Count == 1);
        var exceptionConstructor = ModuleDefinition.ImportReference(constructorDefinition);

        foreach (var targetType in targetTypes)
        {
            foreach (var property in targetType.Properties.Where(x => x.IsDefined(observableAsPropertyAttribute) || (x.GetMethod?.IsDefined(observableAsPropertyAttribute) ?? false)).ToArray())
            {
                var genericObservableAsPropertyHelper = observableAsPropertyHelper.MakeGenericInstanceType(property.PropertyType);
                var genericObservableAsPropertyHelperGetValue = observableAsPropertyHelperGetValue.Bind(genericObservableAsPropertyHelper);
                ModuleDefinition.ImportReference(genericObservableAsPropertyHelperGetValue);

                // Declare a field to store the property value
                var field = new FieldDefinition("$" + property.Name, FieldAttributes.Private, genericObservableAsPropertyHelper);
                targetType.Fields.Add(field);

                // It's an auto-property, so remove the generated field
                if (property.SetMethod is not null && property.SetMethod.HasBody)
                {
                    // Remove old field (the generated backing field for the auto property)
                    var oldField = (FieldReference)property.GetMethod.Body.Instructions.Single(x => x.Operand is FieldReference).Operand;
                    var oldFieldDefinition = oldField.Resolve();
                    targetType.Fields.Remove(oldFieldDefinition);

                    // Re-implement setter to throw an exception
                    property.SetMethod.Body = new MethodBody(property.SetMethod);
                    property.SetMethod.Body.Emit(il =>
                    {
                        il.Emit(OpCodes.Ldstr, "Never call the setter of an ObservableAsPropertyHelper property.");
                        il.Emit(OpCodes.Newobj, exceptionConstructor);
                        il.Emit(OpCodes.Throw);
                        il.Emit(OpCodes.Ret);
                    });
                }

                property.GetMethod.Body = new MethodBody(property.GetMethod);
                property.GetMethod.Body.Emit(il =>
                {
                    var isValid = il.Create(OpCodes.Nop);
                    il.Emit(OpCodes.Ldarg_0);                                               // this
                    il.Emit(OpCodes.Ldfld, field.BindDefinition(targetType));               // pop -> this.$PropertyName
                    il.Emit(OpCodes.Dup);                                                   // Put an extra copy of this.$PropertyName onto the stack
                    il.Emit(OpCodes.Brtrue, isValid);                                       // If the helper is null, return the default value for the property
                    il.Emit(OpCodes.Pop);                                                   // Drop this.$PropertyName
                    EmitDefaultValue(property.GetMethod.Body, il, property.PropertyType);   // Put the default value onto the stack
                    il.Emit(OpCodes.Ret);                                                   // Return that default value
                    il.Append(isValid);                                                     // Add a marker for if the helper is not null
                    il.Emit(OpCodes.Callvirt, genericObservableAsPropertyHelperGetValue);   // pop -> this.$PropertyName.Value
                    il.Emit(OpCodes.Ret);                                                   // Return the value that is on the stack
                });
            }
        }
    }

    /// <summary>
    /// Emits the default value.
    /// </summary>
    /// <param name="methodBody">The method body.</param>
    /// <param name="il">The il.</param>
    /// <param name="type">The type.</param>
    public void EmitDefaultValue(MethodBody methodBody, ILProcessor il, TypeReference type)
    {
        if (methodBody is null)
        {
            throw new ArgumentNullException(nameof(methodBody));
        }

        if (il is null)
        {
            throw new ArgumentNullException(nameof(il));
        }

        if (ModuleDefinition is not null)
        {
            if (type.CompareTo(ModuleDefinition.TypeSystem.Boolean) || type.CompareTo(ModuleDefinition.TypeSystem.Byte) ||
                type.CompareTo(ModuleDefinition.TypeSystem.Int16) || type.CompareTo(ModuleDefinition.TypeSystem.Int32))
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else if (type.CompareTo(ModuleDefinition.TypeSystem.Single))
            {
                il.Emit(OpCodes.Ldc_R4, 0F);
            }
            else if (type.CompareTo(ModuleDefinition.TypeSystem.Int64))
            {
                il.Emit(OpCodes.Ldc_I8, 0L);
            }
            else if (type.CompareTo(ModuleDefinition.TypeSystem.Double))
            {
                il.Emit(OpCodes.Ldc_R8, 0D);
            }
            else if (type.IsGenericParameter || type.IsValueType)
            {
                methodBody.InitLocals = true;
                var local = new VariableDefinition(type);
                il.Body.Variables.Add(local);
                il.Emit(OpCodes.Ldloca_S, local);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, local);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
        }
    }
}