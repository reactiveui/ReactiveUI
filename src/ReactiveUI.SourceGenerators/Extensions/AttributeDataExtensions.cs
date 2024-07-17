// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="AttributeData"/> type.
/// </summary>
internal static class AttributeDataExtensions
{
    /// <summary>
    /// Checks whether a given <see cref="AttributeData"/> instance contains a specified named argument.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="value">The expected value for the target named argument.</param>
    /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with the expected value.</returns>
    public static bool HasNamedArgument<T>(this AttributeData attributeData, string name, T? value)
    {
        foreach (var properties in attributeData.NamedArguments)
        {
            if (properties.Key == name)
            {
                return
                    properties.Value.Value is T argumentValue &&
                    EqualityComparer<T?>.Default.Equals(argumentValue, value);
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to get the location of the input <see cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> instance to get the location for.</param>
    /// <returns>The resulting location for <paramref name="attributeData"/>, if a syntax reference is available.</returns>
    public static Location? GetLocation(this AttributeData attributeData)
    {
        if (attributeData.ApplicationSyntaxReference is { } syntaxReference)
        {
            return syntaxReference.SyntaxTree.GetLocation(syntaxReference.Span);
        }

        return null;
    }

    /// <summary>
    /// Gets a given named argument value from an <see cref="AttributeData"/> instance, or a fallback value.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="fallback">The fallback value to use if the named argument is not present.</param>
    /// <returns>The argument named <paramref name="name"/>, or a fallback value.</returns>
    public static T? GetNamedArgument<T>(this AttributeData attributeData, string name, T? fallback = default)
    {
        if (attributeData.TryGetNamedArgument(name, out T? value))
        {
            return value;
        }

        return fallback;
    }

    /// <summary>
    /// Tries to get a given named argument value from an <see cref="AttributeData"/> instance, if present.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="value">The resulting argument value, if present.</param>
    /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
    public static bool TryGetNamedArgument<T>(this AttributeData attributeData, string name, out T? value)
    {
        foreach (var properties in attributeData.NamedArguments)
        {
            if (properties.Key == name)
            {
                value = (T?)properties.Value.Value;

                return true;
            }
        }

        value = default;

        return false;
    }

    /// <summary>
    /// Enumerates all items in a flattened sequence of constructor arguments for a given <see cref="AttributeData"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of constructor arguments to retrieve.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to get the arguments from.</param>
    /// <returns>A sequence of all constructor arguments of the specified type from <paramref name="attributeData"/>.</returns>
    public static IEnumerable<T?> GetConstructorArguments<T>(this AttributeData attributeData)
        where T : class
    {
        static IEnumerable<T?> Enumerate(IEnumerable<TypedConstant> constants)
        {
            foreach (var constant in constants)
            {
                if (constant.IsNull)
                {
                    yield return null;
                }

                if (constant.Kind == TypedConstantKind.Primitive &&
                    constant.Value is T value)
                {
                    yield return value;
                }
                else if (constant.Kind == TypedConstantKind.Array)
                {
                    foreach (var item in Enumerate(constant.Values))
                    {
                        yield return item;
                    }
                }
            }
        }

        return Enumerate(attributeData.ConstructorArguments);
    }
}
