// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>
/// This is a generic <see cref="NavigationPage"/> that serves as a router with compile-time type safety.
/// This version is fully AOT-compatible and does not use reflection-based view resolution.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor.</typeparam>
/// <seealso cref="NavigationPage" />
/// <seealso cref="IActivatableView" />
public class RoutedViewHost<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> :
    RoutedViewHost
    where TViewModel : class, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost{TViewModel}"/> class.</summary>
    [RequiresUnreferencedCode(
        "This class uses reflection to determine view model types at runtime through ViewLocator, which may be incompatible with trimming.")]
    [RequiresDynamicCode("ViewLocator.ResolveView uses reflection which is incompatible with AOT compilation.")]
    public RoutedViewHost()
    {
    }

    /// <summary>Pages for view model.</summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    protected override IObservable<Page> PagesForViewModel(IRoutableViewModel? vm)
    {
        if (vm is null)
        {
            return Signal.None<Page>();
        }

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var ret = ViewLocator.Current.ResolveView<TViewModel>();
        if (ret is null)
        {
            const string msg =
                $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{nameof(TViewModel)}>";

            return Signal.Fail<Page>(new InvalidOperationException(msg));
        }

        ret.ViewModel = vm as TViewModel;

        var pg = (Page)ret;
        if (SetTitleOnNavigate)
        {
            pg.Title = vm.UrlPathSegment;
        }

        return Signal.Emit(pg);
    }

    /// <summary>Page for view model.</summary>
    /// <param name="vm">The vm.</param>
    /// <returns>A page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    protected override Page PageForViewModel(IRoutableViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm);

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var ret = ViewLocator.Current.ResolveView<TViewModel>();
        if (ret is null)
        {
            const string msg =
                $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{nameof(TViewModel)}>";

            throw new InvalidOperationException(msg);
        }

        ret.ViewModel = vm as TViewModel;

        var pg = (Page)ret;

        if (SetTitleOnNavigate)
        {
            _ = RxSchedulers.MainThreadScheduler.Schedule((page: pg, vm), static (_, state) =>
            {
                state.page.Title = state.vm.UrlPathSegment;
                return EmptyDisposable.Instance;
            });
        }

        return pg;
    }
}
