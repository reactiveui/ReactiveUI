// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Drawing;

/// <summary>
/// Splat Drawing platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Register uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("Register uses methods that may require unreferenced code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        if (registerFunction is null)
        {
            throw new ArgumentNullException(nameof(registerFunction));
        }

#if NETFRAMEWORK || (NET5_0_OR_GREATER && WINDOWS)
        registerFunction(static () => new PlatformBitmapLoader(), typeof(IBitmapLoader));
#endif
    }
}
