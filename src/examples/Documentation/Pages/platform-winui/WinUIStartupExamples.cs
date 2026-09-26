// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;
using Splat.Builder;
using static ReactiveUI.Builder.WinUIReactiveUIBuilderExtensions;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows the WinUI builder extensions the app's own startup does not call directly: the <see cref="IAppBuilder"/>
/// overload of <c>WithWinUI</c> is not present for WinUI (unlike WPF), so this shows the pieces <c>WithWinUI</c>
/// bundles together instead, plus the startup choices for views outside the generated view lookup. Each example builds
/// its own resolver or view locator so it does not touch the services the app already registered at startup.
/// </summary>
public static class WinUIStartupExamples
{
    /// <summary>
    /// <c>WithWinUI</c> calls <c>WithWinUIConverters</c>, <c>WithWinUIScheduler</c> and registers
    /// <see cref="WinUI.Registrations"/> for you; an app that wants only the WinUI converters or scheduler
    /// can call them on their own.
    /// </summary>
    public static void ShowTheIndividualExtensions()
    {
        using ModernDependencyResolver resolver = new();
        IReactiveUIBuilder builder = (IReactiveUIBuilder)new ReactiveUIBuilder(resolver, resolver).WithCoreServices();
        IReactiveUIBuilder configured = builder.WithWinUIConverters().WithWinUIScheduler().WithPlatformModule<WinUI.Registrations>();

        Console.WriteLine($"WithWinUIConverters/WithWinUIScheduler/Registrations configured: {configured is not null}");
        Console.WriteLine($"WinUI main-thread scheduler: {WinUIMainThreadScheduler.GetType().Name}");
    }

    /// <summary>
    /// An app whose lists show views registered only with the service locator registers
    /// <see cref="AutoDataTemplateBindingHookUnsafe"/> next to the <see cref="AutoDataTemplateBindingHook"/> that
    /// <c>WithWinUI</c> registers. It replaces the safe hook's default template, so the order does not matter.
    /// </summary>
    public static void AddTheUnsafeTemplateHook()
    {
        using ModernDependencyResolver resolver = new ModernDependencyResolver();
        _ = resolver.CreateReactiveUIBuilder()
            .WithWinUI()
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
    /// <c>MapFromServiceLocator</c> adds a view the service locator builds to the view locator's <c>Map</c> entries,
    /// so the default <see cref="ViewModelViewHost"/> finds it without an Unsafe twin.
    /// </summary>
    public static void MapAViewFromTheServiceLocator()
    {
        DefaultViewLocator locator = new DefaultViewLocator();
        _ = locator.CreateMappingBuilder().MapFromServiceLocator<RadarImageViewModel, IViewFor<RadarImageViewModel>>();

        ViewModelViewHost host = new ViewModelViewHost { ViewLocator = locator };
        host.ViewModel = new RadarImageViewModel(new WeatherShell(), "Harbor");

        Console.WriteLine($"ViewModelViewHost shows: {host.Content?.GetType().Name ?? "(nothing)"}");

        // Output:
        // ViewModelViewHost shows: RadarImageView
    }
}
