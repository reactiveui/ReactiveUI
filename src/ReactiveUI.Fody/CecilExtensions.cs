// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ReactiveUI.Fody;

/// <summary>
/// Mono.Cecil extension methods.
/// </summary>
public static class CecilExtensions
{
    /// <summary>
    /// Emits the specified il.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="il">The il.</param>
    public static void Emit(this MethodBody body, Action<ILProcessor> il)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(il);
#else
        if (body is null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        if (il is null)
        {
            throw new ArgumentNullException(nameof(il));
        }
#endif

        il(body.GetILProcessor());
    }

    /// <summary>
    /// Makes the method generic.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="genericArguments">The generic arguments.</param>
    /// <returns>A generic method with generic typed arguments.</returns>
    public static GenericInstanceMethod MakeGenericMethod(this MethodReference method, params TypeReference[] genericArguments)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(genericArguments);
#else
        if (genericArguments is null)
        {
            throw new ArgumentNullException(nameof(genericArguments));
        }
#endif

        var result = new GenericInstanceMethod(method);
        foreach (var argument in genericArguments)
        {
            result.GenericArguments.Add(argument);
        }

        return result;
    }

    /// <summary>
    /// Determines whether [is assignable from] [the specified type].
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    /// <param name="type">The type.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>
    ///   <c>true</c> if [is assignable from] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAssignableFrom(this TypeReference baseType, TypeReference type, Action<string>? logger = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(baseType);
        ArgumentNullException.ThrowIfNull(type);
#else
        if (baseType is null)
        {
            throw new ArgumentNullException(nameof(baseType));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
#endif

        return baseType.Resolve().IsAssignableFrom(type.Resolve(), logger);
    }

    /// <summary>
    /// Determines whether [is assignable from] [the specified type].
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    /// <param name="type">The type.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>
    ///   <c>true</c> if [is assignable from] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAssignableFrom(this TypeDefinition baseType, TypeDefinition type, Action<string>? logger = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(baseType);
#else
        if (baseType is null)
        {
            throw new ArgumentNullException(nameof(baseType));
        }
#endif

        logger ??= _ => { };

        var queue = new Queue<TypeDefinition>();
        queue.Enqueue(type);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            logger(current.FullName);

            if (baseType.FullName == current.FullName)
            {
                return true;
            }

            if (current.BaseType is not null)
            {
                queue.Enqueue(current.BaseType.Resolve());
            }

            foreach (var @interface in current.Interfaces)
            {
                queue.Enqueue(@interface.InterfaceType.Resolve());
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified attribute type is defined.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="attributeType">Type of the attribute.</param>
    /// <returns>
    ///   <c>true</c> if the specified attribute type is defined; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDefined(this IMemberDefinition member, TypeReference attributeType)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(member);
#else
        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }
#endif

        return member.HasCustomAttributes && member.CustomAttributes.Any(x => x.AttributeType.FullName == attributeType.FullName);
    }

    /// <summary>
    /// Binds the method to the specified generic type.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="genericType">Type of the generic.</param>
    /// <returns>The method bound to the generic type.</returns>
    public static MethodReference Bind(this MethodReference method, GenericInstanceType genericType)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(method);
#else
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }
#endif

        var reference = new MethodReference(method.Name, method.ReturnType, genericType)
        {
            HasThis = method.HasThis,
            ExplicitThis = method.ExplicitThis,
            CallingConvention = method.CallingConvention
        };

        foreach (var parameter in method.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

    /// <summary>
    /// Binds the generic type definition to a field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <returns>The field bound to the generic type.</returns>
    public static FieldReference BindDefinition(this FieldReference field, TypeReference genericTypeDefinition)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(genericTypeDefinition);
#else
        if (field is null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (genericTypeDefinition is null)
        {
            throw new ArgumentNullException(nameof(genericTypeDefinition));
        }
#endif

        if (!genericTypeDefinition.HasGenericParameters)
        {
            return field;
        }

        var genericDeclaration = new GenericInstanceType(genericTypeDefinition);
        foreach (var parameter in genericTypeDefinition.GenericParameters)
        {
            genericDeclaration.GenericArguments.Add(parameter);
        }

        return new(field.Name, field.FieldType, genericDeclaration);
    }

    /// <summary>
    /// Finds an assembly in a module.
    /// </summary>
    /// <param name="currentModule">The current module.</param>
    /// <param name="assemblyName">Name of the assembly.</param>
    /// <returns>The assembly if found, null if not.</returns>
    public static AssemblyNameReference? FindAssembly(this ModuleDefinition currentModule, string assemblyName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(currentModule);
#else
        if (currentModule is null)
        {
            throw new ArgumentNullException(nameof(currentModule));
        }
#endif

        var assemblyReferences = currentModule.AssemblyReferences;

        return assemblyReferences.SingleOrDefault(x => x.Name == assemblyName);
    }

    /// <summary>
    /// Finds a type reference in the module.
    /// </summary>
    /// <param name="currentModule">The current module.</param>
    /// <param name="namespace">The namespace.</param>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="typeParameters">The type parameters.</param>
    /// <returns>The type reference.</returns>
    public static TypeReference FindType(this ModuleDefinition currentModule, string @namespace, string typeName, IMetadataScope? scope = null, params string[] typeParameters)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(typeParameters);
#else
        if (typeParameters is null)
        {
            throw new ArgumentNullException(nameof(typeParameters));
        }
#endif

        var result = new TypeReference(@namespace, typeName, currentModule, scope);
        foreach (var typeParameter in typeParameters)
        {
            result.GenericParameters.Add(new GenericParameter(typeParameter, result));
        }

        return result;
    }

    /// <summary>
    /// Compares two type references for equality.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="compareTo">The compare to.</param>
    /// <returns>A value indicating the result of the comparison.</returns>
    public static bool CompareTo(this TypeReference type, TypeReference compareTo)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(compareTo);
#else
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (compareTo is null)
        {
            throw new ArgumentNullException(nameof(compareTo));
        }
#endif

        return type.FullName == compareTo.FullName;
    }
}
