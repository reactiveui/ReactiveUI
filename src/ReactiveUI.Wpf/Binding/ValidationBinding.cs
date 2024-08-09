// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;

namespace ReactiveUI.Wpf.Binding;

internal class ValidationBinding<TView, TViewModel, TVProp, TType> : IReactiveBinding<TView, TType>
    where TView : IViewFor
    where TViewModel : class
    where TVProp : FrameworkElement
{
    private readonly TVProp _control;
    private readonly DependencyProperty? _propertyName;
    private readonly IReactiveProperty<TType> _property;
    private IDisposable? _inner;

    public ValidationBinding(TView view, TViewModel viewModel, Func<TViewModel, IReactiveProperty<TType>> viewModelPropertySelector, Func<TView, TVProp> frameworkElementSelector, Func<TVProp, string> propertySelector)
    {
        View = view;
        _control = frameworkElementSelector(view);
        var dps = GetDependencyProperty(_control, propertySelector(_control)) ?? throw new ArgumentException($"Dependency property not found on {typeof(TVProp).Name}, use nameof(prop.Property)");
        _propertyName = dps;
        _property = viewModelPropertySelector(viewModel)!;
        Changed = _property;
        Direction = BindingDirection.TwoWay;
        ViewModelExpression = default!;
        ViewExpression = default!;
        Bind();
    }

    public System.Linq.Expressions.Expression ViewModelExpression { get; }

    public TView View { get; }

    public System.Linq.Expressions.Expression ViewExpression { get; }

    public IObservable<TType?> Changed { get; }

    public BindingDirection Direction { get; }

    public IDisposable Bind()
    {
        _control.SetBinding(_propertyName, new System.Windows.Data.Binding()
        {
            Source = _property,
            Path = new(nameof(_property.Value)),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        _inner = Disposable.Create(() => BindingOperations.ClearBinding(_control, _propertyName));

        return _inner;
    }

    public void Dispose()
    {
        _inner?.Dispose();
        GC.SuppressFinalize(this);
    }

    internal static IEnumerable<DependencyProperty> EnumerateDependencyProperties(object element)
    {
        if (element != null)
        {
            var markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (var mp in markupObject.Properties)
                {
                    if (mp.DependencyProperty != null)
                    {
                        yield return mp.DependencyProperty;
                    }
                }
            }
        }
    }

    internal static IEnumerable<DependencyProperty> EnumerateAttachedProperties(object element)
    {
        if (element != null)
        {
            var markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (var mp in markupObject.Properties)
                {
                    if (mp.IsAttached)
                    {
                        yield return mp.DependencyProperty;
                    }
                }
            }
        }
    }

    internal static DependencyProperty? GetDependencyProperty(object element, string name) =>
        EnumerateDependencyProperties(element).Concat(EnumerateAttachedProperties(element)).FirstOrDefault(x => x.Name == name);
}
