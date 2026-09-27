// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using AppKit;
using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a NSWindowController that is both a NSWindowController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged), and is the view for a view model.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[DebuggerDisplay("{ViewModel}")]
public class ReactiveWindowController<TViewModel> : ReactiveWindowController, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="window">The window.</param>
    protected ReactiveWindowController(NSWindow window)
        : base(window)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="windowNibName">Name of the window nib.</param>
    protected ReactiveWindowController(string windowNibName)
        : base(windowNibName)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="windowNibName">Name of the window nib.</param>
    /// <param name="owner">The owner.</param>
    protected ReactiveWindowController(string windowNibName, NSObject owner)
        : base(windowNibName, owner)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveWindowController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="t">The t.</param>
    protected ReactiveWindowController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    /// <param name="handle">The handle.</param>
    protected ReactiveWindowController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveWindowController{TViewModel}"/> class.</summary>
    protected ReactiveWindowController()
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
