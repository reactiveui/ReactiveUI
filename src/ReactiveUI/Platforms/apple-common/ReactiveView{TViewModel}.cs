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
/// <summary>This is a View that is both a NSView and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public abstract class ReactiveView<TViewModel> : ReactiveView, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveView{TViewModel}"/> class.</summary>
    protected ReactiveView()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView{TViewModel}"/> class.</summary>
    /// <param name="c">The coder.</param>
    protected ReactiveView(NSCoder c)
        : base(c)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView{TViewModel}"/> class.</summary>
    /// <param name="f">The object flag.</param>
    protected ReactiveView(NSObjectFlag f)
        : base(f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveView(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveView{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveView(CGRect frame)
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
