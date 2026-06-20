// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>This is an Activity that is both an Activity and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveAppCompatActivity<TViewModel> : ReactiveAppCompatActivity, IViewFor<TViewModel>, ICanActivate
    where TViewModel : class
{
    /// <summary>The backing field for the view model.</summary>
    private TViewModel? _viewModel;

    /// <summary>Initializes a new instance of the <see cref="ReactiveAppCompatActivity{TViewModel}"/> class.</summary>
    protected ReactiveAppCompatActivity()
    {
    }

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => _viewModel;
        set => _viewModel = (TViewModel?)value;
    }
}
