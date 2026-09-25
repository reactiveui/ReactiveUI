// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Verifies that an application delegate forwards the lifecycle methods a suspension helper depends on.</summary>
internal static class MethodOverrideGuard
{
    /// <summary>Checks that each named method is declared on the target type.</summary>
    /// <param name="callingTypeName">The name of the helper the methods must call.</param>
    /// <param name="targetType">The type to check. Must preserve public and non-public methods under trimming.</param>
    /// <param name="methodsToCheck">The method names to check.</param>
    /// <exception cref="InvalidOperationException">A method is not declared on the target type.</exception>
    internal static void ThrowIfMethodsNotOverloaded(
        string callingTypeName,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        Type targetType,
        params string[] methodsToCheck)
    {
        var methods = targetType.GetTypeInfo().DeclaredMethods;

        for (var i = 0; i < methodsToCheck.Length; i++)
        {
            var name = methodsToCheck[i];
            if (!Declares(methods, name))
            {
                throw new InvalidOperationException($"Your class must implement {name} and call {callingTypeName}.{name}");
            }
        }
    }

    /// <summary>Determines whether a method of the given name is among the declared methods.</summary>
    /// <param name="methods">The declared methods.</param>
    /// <param name="name">The method name.</param>
    /// <returns><see langword="true"/> when a method of that name is declared.</returns>
    private static bool Declares(IEnumerable<MethodInfo> methods, string name)
    {
        foreach (var method in methods)
        {
            if (string.Equals(method.Name, name, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
