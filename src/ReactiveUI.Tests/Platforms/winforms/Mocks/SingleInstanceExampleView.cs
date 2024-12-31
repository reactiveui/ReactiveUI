// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// A signle instance example view.
/// </summary>
[SingleInstanceView]
public class SingleInstanceExampleView : ReactiveUI.Winforms.ReactiveUserControl<SingleInstanceExampleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleInstanceExampleView"/> class.
    /// </summary>
    public SingleInstanceExampleView() => Instances++;

    /// <summary>
    /// Gets the instances.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static int Instances { get; private set; }
}
