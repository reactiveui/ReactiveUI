// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>A binding hook that gives an <c>ItemsControl</c> a default item template hosting each item in a <see cref="ViewModelViewHostUnsafe"/>.</summary>
/// <remarks>
/// <para>
/// <see cref="AutoDataTemplateBindingHook"/> is registered by <c>WithWpf</c> and hosts each item in a
/// <see cref="ViewModelViewHost"/>, which asks the generated view lookup and the view locator's <c>Map</c>
/// registrations. Register this hook as well when a list shows view models whose view is registered only with the
/// service locator. It replaces the default template <see cref="AutoDataTemplateBindingHook"/> assigned, and leaves
/// every template the app set itself alone.
/// </para>
/// <para>
/// <see cref="ViewModelViewHostUnsafe"/> closes a generic type over the view model's run-time type, so this hook is
/// not safe to compile ahead of time.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithWpf()
///     .WithRegistration(static r => r.RegisterConstant<IPropertyBindingHook>(new AutoDataTemplateBindingHookUnsafe()))
///     .BuildApp();
/// ]]></code>
/// </example>
[RequiresDynamicCode(ViewResolutionMessages.UnsafeTemplateHook)]
[DebuggerDisplay("AutoDataTemplateBindingHookUnsafe")]
public class AutoDataTemplateBindingHookUnsafe : IPropertyBindingHook
{
    /// <summary>Gets the default item template, which hosts each item in a <see cref="ViewModelViewHostUnsafe"/>.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } =
        new(static () => CreateItemTemplate(nameof(ViewModelViewHostUnsafe)));

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
        return true;
    }

    /// <summary>Builds an item template that hosts each item in the named host type from this assembly.</summary>
    /// <param name="hostTypeName">The name of the host type, which lives in this type's namespace and assembly.</param>
    /// <returns>The item template.</returns>
    private static DataTemplate CreateItemTemplate(string hostTypeName)
    {
#if REACTIVE_SHIM
        const string XamlClrNamespace = "clr-namespace:ReactiveUI.Reactive";
#else
        const string XamlClrNamespace = "clr-namespace:ReactiveUI";
#endif
        var assemblyName = typeof(ViewModelViewHostUnsafe).Assembly.FullName;
        assemblyName = assemblyName?.Substring(0, assemblyName.IndexOf(','));

        var template =
            $"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:xaml='{XamlClrNamespace};assembly={assemblyName}'>"
            + $" <xaml:{hostTypeName} ViewModel=\"{{Binding Mode=OneWay}}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" /></DataTemplate>";

        return (DataTemplate)XamlReader.Parse(template);
    }
}
