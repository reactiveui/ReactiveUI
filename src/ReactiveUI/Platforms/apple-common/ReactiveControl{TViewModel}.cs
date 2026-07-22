// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UIControl that is both and UIControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveControl<TViewModel> : ReactiveControl, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.</summary>
    protected ReactiveControl()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.</summary>
    /// <param name="c">The coder.</param>
    protected ReactiveControl(NSCoder c)
        : base(c)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.</summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveControl(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer handle.</param>
    protected ReactiveControl(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveControl{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveControl(CGRect frame)
        : base(frame)
    {
    }

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value!;
    }
}
