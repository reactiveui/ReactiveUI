// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using System.Windows.Media;

using DynamicData;

namespace ReactiveUI.Wpf.Binding;

/// <summary>
/// Provides validation binding functionality for WPF views and view models.
/// This class creates two-way bindings between WPF controls and view model properties
/// with support for validation and change notifications.
/// </summary>
/// <typeparam name="TView">The type of the view, which must implement IViewFor.</typeparam>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <typeparam name="TVProp">The type of the view property being bound.</typeparam>
/// <typeparam name="TVMProp">The type of the view model property being bound.</typeparam>
internal class ValidationBindingWpf<TView, TViewModel, TVProp, TVMProp> : IReactiveBinding<TView, TVMProp>
    where TView : class, IViewFor
    where TViewModel : class
{
    private readonly FrameworkElement _control;
    private readonly DependencyProperty _dpPropertyName;
    private readonly TViewModel _viewModel;
    private readonly string _vmPropertyName;
    private IDisposable? _inner;

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ValidationBindingWpf uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ValidationBindingWpf uses methods that may require unreferenced code")]
#endif
    public ValidationBindingWpf(
        TView view,
        TViewModel viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty)
    {
        // Get the ViewModel details
        _viewModel = viewModel;
        ViewModelExpression = Reflection.Rewrite(vmProperty.Body);
        _vmPropertyName = ExtractPropertyPath(ViewModelExpression);

        // Get the View details
        View = view;
        ViewExpression = Reflection.Rewrite(viewProperty.Body);
        var viewExpressionChain = ViewExpression.GetExpressionChain().ToArray();

        var controlName = ExtractControlName(viewExpressionChain, typeof(TView));
        _control = FindControlByName(view as DependencyObject, controlName)
            ?? throw new ArgumentException($"Control '{controlName}' not found in view {typeof(TView).Name}", nameof(viewProperty));

        var propertyName = viewExpressionChain.LastOrDefault()?.GetMemberInfo()?.Name;
        _dpPropertyName = GetDependencyProperty(_control, propertyName)
            ?? throw new ArgumentException($"Dependency property '{propertyName}' not found on {typeof(TVProp).Name}", nameof(viewProperty));

        Changed = Reflection.ViewModelWhenAnyValue(viewModel, view, ViewModelExpression)
            .Select(static tvm => (TVMProp?)tvm)
            .Merge(view.WhenAnyDynamic(ViewExpression, static x => (TVProp?)x.Value)
                .Select(static p => default(TVMProp)));
        Direction = BindingDirection.TwoWay;
        Bind();
    }

    /// <summary>
    /// Gets the expression representing the view model property being bound.
    /// </summary>
    public System.Linq.Expressions.Expression ViewModelExpression { get; }

    /// <summary>
    /// Gets the view instance that owns this binding.
    /// </summary>
    public TView View { get; }

    /// <summary>
    /// Gets the expression representing the view property being bound.
    /// </summary>
    public System.Linq.Expressions.Expression ViewExpression { get; }

    /// <summary>
    /// Gets an observable that emits values when either the view model or view property changes.
    /// </summary>
    public IObservable<TVMProp?> Changed { get; }

    /// <summary>
    /// Gets the direction of the binding (always TwoWay for validation bindings).
    /// </summary>
    public BindingDirection Direction { get; }

    /// <summary>
    /// Establishes the two-way data binding between the view control and view model property.
    /// </summary>
    /// <returns>A disposable that can be used to remove the binding.</returns>
    public IDisposable Bind()
    {
        _control.SetBinding(_dpPropertyName, new System.Windows.Data.Binding
        {
            Source = _viewModel,
            Path = new(_vmPropertyName),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        _inner = Disposable.Create(() => BindingOperations.ClearBinding(_control, _dpPropertyName));

        return _inner;
    }

    /// <summary>
    /// Disposes the binding and releases all associated resources.
    /// </summary>
    public void Dispose()
    {
        _inner?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Extracts the full property path from a view model expression chain.
    /// Converts an expression like "x => x.User.Name" to "User.Name".
    /// </summary>
    /// <param name="expression">The expression to extract the path from.</param>
    /// <returns>The dot-separated property path.</returns>
    internal static string ExtractPropertyPath(System.Linq.Expressions.Expression expression)
    {
        var chain = expression.GetExpressionChain();
        var pathParts = chain
            .Select(static x => x.GetMemberInfo()?.Name)
            .Where(static name => !string.IsNullOrEmpty(name));

        return string.Join(".", pathParts);
    }

    /// <summary>
    /// Extracts the control name from a view expression chain.
    /// For an expression like "view => view.MyTextBox.Text", returns "MyTextBox".
    /// </summary>
    /// <param name="expressionChain">The expression chain from the view property.</param>
    /// <param name="viewType">The type of the view for error messages.</param>
    /// <returns>The name of the control.</returns>
    /// <exception cref="ArgumentException">Thrown when the control name cannot be determined.</exception>
    internal static string ExtractControlName(System.Linq.Expressions.Expression[] expressionChain, Type viewType)
    {
        if (expressionChain.Length < 2)
        {
            throw new ArgumentException($"Expression chain too short to contain a control reference on {viewType.Name}", nameof(expressionChain));
        }

        var lastIndex = expressionChain.Length - 1;
        var controlExpression = expressionChain[lastIndex - 1];
        var controlName = controlExpression.GetMemberInfo()?.Name;

        return controlName ?? throw new ArgumentException($"Control name not found on {viewType.Name}", nameof(expressionChain));
    }

    /// <summary>
    /// Enumerates all dependency properties on a WPF element using reflection.
    /// </summary>
    /// <param name="element">The element to enumerate properties for.</param>
    /// <returns>An enumerable of dependency properties.</returns>
    internal static IEnumerable<DependencyProperty> EnumerateDependencyProperties(object? element)
    {
        if (element is null)
        {
            yield break;
        }

        var markupObject = MarkupWriter.GetMarkupObjectFor(element);
        if (markupObject is null)
        {
            yield break;
        }

        foreach (var property in markupObject.Properties)
        {
            if (property.DependencyProperty is not null)
            {
                yield return property.DependencyProperty;
            }
        }
    }

    /// <summary>
    /// Enumerates all attached properties on a WPF element using reflection.
    /// </summary>
    /// <param name="element">The element to enumerate attached properties for.</param>
    /// <returns>An enumerable of attached dependency properties.</returns>
    internal static IEnumerable<DependencyProperty> EnumerateAttachedProperties(object? element)
    {
        if (element is null)
        {
            yield break;
        }

        var markupObject = MarkupWriter.GetMarkupObjectFor(element);
        if (markupObject is null)
        {
            yield break;
        }

        foreach (var property in markupObject.Properties)
        {
            if (property.IsAttached)
            {
                yield return property.DependencyProperty;
            }
        }
    }

    /// <summary>
    /// Gets a dependency property by name from an element, searching both
    /// regular and attached properties.
    /// </summary>
    /// <param name="element">The element to search for the property.</param>
    /// <param name="name">The name of the property to find.</param>
    /// <returns>The dependency property if found; otherwise, null.</returns>
    internal static DependencyProperty? GetDependencyProperty(object? element, string? name)
    {
        if (element is null || string.IsNullOrEmpty(name))
        {
            return null;
        }

        return EnumerateDependencyProperties(element)
            .Concat(EnumerateAttachedProperties(element))
            .FirstOrDefault(x => x.Name == name);
    }

    /// <summary>
    /// Finds the first control with the specified name in the visual tree.
    /// </summary>
    /// <param name="parent">The root element to start searching from.</param>
    /// <param name="name">The name of the control to find.</param>
    /// <returns>The first matching FrameworkElement, or null if not found.</returns>
    internal static FrameworkElement? FindControlByName(DependencyObject? parent, string? name)
    {
        if (parent is null)
        {
            return null;
        }

        if (name is null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return FindControlsByNameIterator(parent, name).FirstOrDefault();
    }

    /// <summary>
    /// Recursively searches the visual tree for controls matching the specified name.
    /// </summary>
    /// <param name="parent">The parent element to search within.</param>
    /// <param name="name">The name to match.</param>
    /// <returns>An enumerable of all matching FrameworkElements.</returns>
    private static IEnumerable<FrameworkElement> FindControlsByNameIterator(DependencyObject parent, string name)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is FrameworkElement { Name: var elementName } element && elementName == name)
            {
                yield return element;
            }

            foreach (var descendant in FindControlsByNameIterator(child, name))
            {
                yield return descendant;
            }
        }
    }
}
