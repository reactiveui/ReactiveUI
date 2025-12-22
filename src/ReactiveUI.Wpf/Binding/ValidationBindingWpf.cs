// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using System.Windows.Media;

using DynamicData;

namespace ReactiveUI.Wpf.Binding;

internal class ValidationBindingWpf<TView, TViewModel, TVProp, TVMProp> : IReactiveBinding<TView, TVMProp>
    where TView : class, IViewFor
    where TViewModel : class
{
    private const string DotValue = ".";
    private readonly FrameworkElement _control;
    private readonly DependencyProperty? _dpPropertyName;
    private readonly TViewModel _viewModel;
    private readonly string? _vmPropertyName;
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
        var vmet = ViewModelExpression.GetExpressionChain();
        var vmFullName = vmet.Select(static x => x.GetMemberInfo()?.Name).Aggregate(new StringBuilder(), static (sb, x) => sb.Append(x).Append('.')).ToString();
        if (vmFullName.EndsWith(DotValue))
        {
            vmFullName = vmFullName.Substring(0, vmFullName.Length - 1);
        }

        _vmPropertyName = vmFullName;

        // Get the View details
        View = view;
        ViewExpression = Reflection.Rewrite(viewProperty.Body);
        var vet = ViewExpression.GetExpressionChain().ToArray();
        var controlName = string.Empty;
        var index = vet.IndexOf(vet.Last()!);
        if (vet != null && index > 0)
        {
            controlName = vet[vet.IndexOf(vet.Last()!) - 1]!.GetMemberInfo()?.Name
                ?? throw new ArgumentException($"Control name not found on {typeof(TView).Name}");
        }

        _control = FindControlsByName(view as DependencyObject, controlName).FirstOrDefault()!;
        var controlDpPropertyName = vet?.Last().GetMemberInfo()?.Name;
        _dpPropertyName = GetDependencyProperty(_control, controlDpPropertyName) ?? throw new ArgumentException($"Dependency property not found on {typeof(TVProp).Name}");

        var somethingChanged = Reflection.ViewModelWhenAnyValue(viewModel, view, ViewModelExpression).Select(static tvm => (TVMProp?)tvm).Merge(
              view.WhenAnyDynamic(ViewExpression, static x => (TVProp?)x.Value).Select(static p => default(TVMProp)));
        Changed = somethingChanged;
        Direction = BindingDirection.TwoWay;
        Bind();
    }

    public System.Linq.Expressions.Expression ViewModelExpression { get; }

    public TView View { get; }

    public System.Linq.Expressions.Expression ViewExpression { get; }

    public IObservable<TVMProp?> Changed { get; }

    public BindingDirection Direction { get; }

    public IDisposable Bind()
    {
        _control.SetBinding(_dpPropertyName, new System.Windows.Data.Binding()
        {
            Source = _viewModel,
            Path = new(_vmPropertyName),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        _inner = Disposable.Create(() => BindingOperations.ClearBinding(_control, _dpPropertyName));

        return _inner;
    }

    public void Dispose()
    {
        _inner?.Dispose();
        GC.SuppressFinalize(this);
    }

    private static IEnumerable<DependencyProperty> EnumerateDependencyProperties(object element)
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

    private static IEnumerable<DependencyProperty> EnumerateAttachedProperties(object element)
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

    private static DependencyProperty? GetDependencyProperty(object element, string? name) =>
        EnumerateDependencyProperties(element).Concat(EnumerateAttachedProperties(element)).FirstOrDefault(x => x.Name == name);

    private static IEnumerable<FrameworkElement> FindControlsByName(DependencyObject? parent, string? name)
    {
        if (parent == null)
        {
            yield break;
        }

        if (name == null)
        {
            yield break;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is FrameworkElement element && element.Name == name)
            {
                yield return element;
            }

            foreach (var descendant in FindControlsByName(child, name))
            {
                yield return descendant;
            }
        }
    }
}
