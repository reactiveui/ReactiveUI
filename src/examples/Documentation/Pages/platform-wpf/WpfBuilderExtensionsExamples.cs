// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Builder;
using Splat;
using Splat.Builder;
using static ReactiveUI.Builder.WpfReactiveUIBuilderExtensions;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// Shows the WPF builder extensions the app's own startup does not call directly: the <see cref="IAppBuilder"/>
/// overload of <c>WithWpf</c>, and the pieces <c>WithWpf</c> otherwise bundles together. Each example builds its own
/// resolver so it does not touch the services the app already registered in <c>App.OnStartup</c>.
/// </summary>
public static class WpfBuilderExtensionsExamples
{
    /// <summary>
    /// <c>WithWpf</c> also has an overload on <see cref="IAppBuilder"/>, the interface every ReactiveUI builder and
    /// most Splat app builders implement; it forwards to the same configuration as the <see cref="IReactiveUIBuilder"/>
    /// overload the app's startup calls.
    /// </summary>
    public static void ShowTheAppBuilderOverload()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = new(resolver, resolver);
        IReactiveUIBuilder configured = ((IAppBuilder)builder).WithWpf();
        Console.WriteLine($"IAppBuilder.WithWpf() configured: {configured is not null}");

        // Output:
        // IAppBuilder.WithWpf() configured: True
    }

    /// <summary>
    /// <c>WithWpf</c> calls <c>WithWpfConverters</c>, <c>WithWpfScheduler</c> and
    /// <see cref="WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler"/> for you; an app that wants the WPF
    /// converters or scheduler without the rest of <c>WithWpf</c> can call them on their own.
    /// </summary>
    public static void ShowTheIndividualExtensions()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = new(resolver, resolver);
        IReactiveUIBuilder coreServices = (IReactiveUIBuilder)builder.WithCoreServices();
        IReactiveUIBuilder configured = coreServices.WithWpfConverters().WithWpfScheduler();

        Console.WriteLine($"WithWpfConverters/WithWpfScheduler configured: {configured is not null}");
        Console.WriteLine($"WPF main-thread scheduler: {WpfMainThreadScheduler.GetType().Name}");

        // Output:
        // WithWpfConverters/WithWpfScheduler configured: True
        // WPF main-thread scheduler: DispatcherSequencer
    }

    /// <summary>
    /// An app whose lists show views registered only with the service locator registers
    /// <see cref="AutoDataTemplateBindingHookUnsafe"/> next to the <see cref="AutoDataTemplateBindingHook"/> that
    /// <c>WithWpf</c> registers. It replaces the safe hook's default template, so the order does not matter.
    /// </summary>
    public static void AddTheUnsafeTemplateHook()
    {
        using ModernDependencyResolver resolver = new();
        _ = resolver.CreateReactiveUIBuilder()
            .WithWpf()
            .WithRegistration(static registrar => registrar.RegisterConstant<IPropertyBindingHook>(new AutoDataTemplateBindingHookUnsafe()));

        foreach (IPropertyBindingHook hook in resolver.GetServices<IPropertyBindingHook>())
        {
            Console.WriteLine($"Binding hook: {hook.GetType().Name}");
        }

        // Output:
        // Binding hook: AutoDataTemplateBindingHook
        // Binding hook: AutoDataTemplateBindingHookUnsafe
    }

    /// <summary>
    /// <see cref="AutoDataTemplateBindingHook.DefaultItemTemplate"/> is the template <c>WithWpf</c>'s hook assigns to
    /// an <c>ItemsControl</c> with no template of its own; an app can inspect or reuse it directly.
    /// <see cref="AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate"/> is the same idea for the Unsafe twin.
    /// </summary>
    public static void ShowTheDefaultItemTemplate()
    {
        DataTemplate template = AutoDataTemplateBindingHook.DefaultItemTemplate.Value;
        DependencyObject root = template.LoadContent();
        Console.WriteLine($"Default item template hosts each item in a: {root.GetType().Name}");

        DataTemplate unsafeTemplate = AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate.Value;
        DependencyObject unsafeRoot = unsafeTemplate.LoadContent();
        Console.WriteLine($"Unsafe default item template hosts each item in a: {unsafeRoot.GetType().Name}");

        // Output:
        // Default item template hosts each item in a: ViewModelViewHost
        // Unsafe default item template hosts each item in a: ViewModelViewHostUnsafe
    }
}
