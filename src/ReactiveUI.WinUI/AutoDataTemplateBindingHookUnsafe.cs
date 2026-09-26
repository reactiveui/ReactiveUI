// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>A binding hook that gives an <c>ItemsControl</c> a default item template hosting each item in a <see cref="ViewModelViewHostUnsafe"/>.</summary>
/// <remarks>
/// <para>
/// <see cref="AutoDataTemplateBindingHook"/> is registered by <c>WithWinUI</c> and hosts each item in a
/// <see cref="ViewModelViewHost"/>, which asks the generated view lookup and the view locator's <c>Map</c>
/// registrations. Register this hook as well when a list shows view models whose view is registered only with the
/// service locator. It replaces the default template <see cref="AutoDataTemplateBindingHook"/> assigned, and leaves every
/// template the app set itself alone.
/// </para>
/// <para>
/// <see cref="ViewModelViewHostUnsafe"/> closes a generic type over the view model's run-time type, so this hook is
/// not safe to compile ahead of time.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithWinUI()
///     .WithRegistration(static r => r.RegisterConstant<IPropertyBindingHook>(new AutoDataTemplateBindingHookUnsafe()))
///     .BuildApp();
/// ]]></code>
/// </example>
[RequiresDynamicCode(ViewResolutionMessages.UnsafeTemplateHook)]
[DebuggerDisplay("AutoDataTemplateBindingHookUnsafe")]
public class AutoDataTemplateBindingHookUnsafe : IPropertyBindingHook
{
    /// <summary>The converter that hosts each item in a <see cref="ViewModelViewHostUnsafe"/>.</summary>
    private static readonly ViewModelViewHostConverter Converter =
        new($"{typeof(ViewModelViewHostUnsafe).FullName}Converter", static () => new ViewModelViewHostUnsafe());

    /// <summary>Gets the default item template, which hosts each item in a <see cref="ViewModelViewHostUnsafe"/>.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(static () => Converter.CreateItemTemplate());

    /// <inheritdoc/>
    public bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        var safeTemplate = AutoDataTemplateBindingHook.DefaultItemTemplate;
        var replaceable = safeTemplate.IsValueCreated ? safeTemplate.Value : null;
        if (ItemsControlTemplateBinding.FindDefaultTemplateTarget(getCurrentViewProperties, replaceable) is not { } itemsControl)
        {
            return true;
        }

        itemsControl.ItemTemplate = DefaultItemTemplate.Value;
        Converter.EnsureRegistered();
        return true;
    }
}
