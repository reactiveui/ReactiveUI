// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A book's cover. This app has no bundled artwork, so a blank image stands in and the accessibility label
/// carries the book's title instead.</summary>
[System.Diagnostics.DebuggerDisplay("BookCoverImageView")]
public sealed class BookCoverImageView : ReactiveImageView<Book>
{
    /// <summary>Initializes a new instance of the <see cref="BookCoverImageView"/> class.</summary>
    public BookCoverImageView()
    {
        Image ??= new UIImage();

        _ = this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.AccessibilityLabel));
            d(ThrownExceptions.Subscribe(static error => Console.WriteLine($"BookCoverImageView threw: {error.Message}")));
        });
    }
}
