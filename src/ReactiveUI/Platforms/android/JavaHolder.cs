// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Object = Java.Lang.Object;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides a container for holding a reference to a Java object instance for interop scenarios.</summary>
/// <remarks>This type is intended for internal use in interop scenarios where a managed reference to a Java
/// object must be maintained. It is not intended for general application development.</remarks>
/// <param name="instance">The Java object instance to be held. Cannot be null.</param>
internal class JavaHolder(object instance) : Object
{
    /// <summary>The held Java object instance used for interop scenarios.</summary>
    [SuppressMessage("Maintainability", "SST1401:Field should be private", Justification = "Used for interop purposes.")]
    public readonly object Instance = instance;
}
