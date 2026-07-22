// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;

#if UIKIT
using NSCoder = Foundation.NSCoder;
using NSImage = UIKit.UIImage;
using NSObjectFlag = Foundation.NSObjectFlag;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is an  ImageView that is both and ImageView and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveImageView<TViewModel> : ReactiveImageView, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    protected ReactiveImageView()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveImageView(CGRect frame)
        : base(frame)
    {
    }

#if UIKIT
    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="image">The image.</param>
    protected ReactiveImageView(NSImage image)
        : base(image)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveImageView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="image">The image.</param>
    /// <param name="highlightedImage">The highlighted image.</param>
    protected ReactiveImageView(NSImage image, NSImage highlightedImage)
        : base(image, highlightedImage)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveImageView(NSCoder coder)
        : base(coder)
    {
    }
#endif

    /// <summary>Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveImageView(in IntPtr handle)
        : base(handle)
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
