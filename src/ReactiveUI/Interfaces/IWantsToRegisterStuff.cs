// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Represents a class that can register services with ReactiveUI's dependency resolver.
/// </summary>
/// <remarks>
/// <para>
/// This interface is used with the ReactiveUI builder pattern to provide custom registrations.
/// The registration methods use generic types to avoid runtime reflection, making them compatible
/// with AOT compilation and trimming.
/// </para>
/// <para>
/// Usage with builder:
/// <code>
/// public class MyCustomRegistrations : IWantsToRegisterStuff
/// {
///     public void Register(IRegistrar registrar)
///     {
///         registrar.RegisterConstant&lt;IMyService&gt;(() => new MyService());
///         registrar.RegisterLazySingleton&lt;IMyViewModel&gt;(() => new MyViewModel());
///     }
/// }
///
/// // In your app initialization:
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithCoreServices()
///     .WithPlatformServices()
///     .WithRegistration(new MyCustomRegistrations())
///     .BuildApp();
/// </code>
/// </para>
/// </remarks>
public interface IWantsToRegisterStuff
{
    /// <summary>
    /// Register platform dependencies using the provided registrar.
    /// This method uses generic registration to avoid runtime Type reflection,
    /// making it compatible with AOT compilation and trimming.
    /// </summary>
    /// <param name="registrar">The AOT-friendly registrar to use for registering services.</param>
    void Register(IRegistrar registrar);
}
