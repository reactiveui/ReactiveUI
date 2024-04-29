// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests.Activation;

/// <summary>
/// Carousel Page View.
/// </summary>
public class CarouselPageView : ReactiveCarouselView<CarouselPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarouselPageView"/> class.
    /// </summary>
    public CarouselPageView() =>
        this.WhenActivated(d =>
        {
            IsActiveCount++;
            d(Disposable.Create(() => IsActiveCount--));
        });

    /// <summary>
    /// Gets or sets the active count.
    /// </summary>
    public int IsActiveCount { get; set; }
}
