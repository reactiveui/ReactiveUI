// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Used by ReactiveUI when first starting up, it will seek out classes
/// inside our own ReactiveUI projects. The implemented methods will
/// register with Splat their dependencies.
/// </summary>
internal interface IWantsToRegisterStuff
{
    /// <summary>
    /// Register platform dependencies inside Splat.
    /// </summary>
    /// <param name="registerFunction">A method the deriving class will class to register the type.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Platform registration uses ComponentModelTypeConverter and RxApp which require dynamic code generation")]
    [RequiresUnreferencedCode("Platform registration uses ComponentModelTypeConverter and RxApp which may require unreferenced code")]
#endif
    void Register(Action<Func<object>, Type> registerFunction);
}
