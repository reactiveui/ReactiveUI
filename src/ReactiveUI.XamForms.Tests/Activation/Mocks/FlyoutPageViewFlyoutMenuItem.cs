// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.XamForms.Tests.Activation.Mocks;

/// <summary>
/// FlyoutPageViewFlyoutMenuItem.
/// </summary>
public class FlyoutPageViewFlyoutMenuItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutPageViewFlyoutMenuItem"/> class.
    /// </summary>
    public FlyoutPageViewFlyoutMenuItem()
    {
        TargetType = typeof(FlyoutPageViewFlyoutMenuItem);
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    /// <value>
    /// The type of the target.
    /// </value>
    public Type TargetType { get; set; }
}
