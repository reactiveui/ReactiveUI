// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Android.Preferences;
using Android.Runtime;

namespace ReactiveUI;

/// <summary>
/// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactivePreferenceFragment<TViewModel> inherits from ReactiveObject which uses extension methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactivePreferenceFragment<TViewModel> inherits from ReactiveObject which uses extension methods that may require unreferenced code")]
#endif
public class ReactivePreferenceFragment<TViewModel> : ReactivePreferenceFragment, IViewFor<TViewModel>, ICanActivate
    where TViewModel : class
{
    private TViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
    /// </summary>
    protected ReactivePreferenceFragment()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    [ObsoletedOSPlatform("android28.0")]
    protected ReactivePreferenceFragment(in IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
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
        set => _viewModel = (TViewModel?)value!;
    }
}
