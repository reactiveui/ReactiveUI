// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Represents a module that registers view-to-viewmodel mappings for AOT-compatible view resolution.</summary>
/// <remarks>
/// <para>
/// View modules provide a way to organize view registrations by feature or module.
/// Implement this interface to create reusable, testable view registration logic.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class AuthenticationViewModule : IViewModule
/// {
///     public void RegisterViews(DefaultViewLocator locator)
///     {
///         locator.CreateMappingBuilder()
///             .Map<LoginViewModel, LoginView>()
///             .Map<RegisterViewModel, RegisterView>()
///             .Map<ForgotPasswordViewModel, ForgotPasswordView>();
///     }
/// }
/// ]]>
/// </code>
/// </example>
public interface IViewModule
{
    /// <summary>Registers view-to-viewmodel mappings with the provided view locator.</summary>
    /// <param name="locator">The view locator to register mappings with.</param>
    void RegisterViews(DefaultViewLocator locator);
}
