// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A view that is never used.
/// </summary>
[SingleInstanceView]
public class NeverUsedView : ReactiveUI.Winforms.ReactiveUserControl<NeverUsedViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NeverUsedView"/> class.
    /// </summary>
    public NeverUsedView() => Instances++;

    /// <summary>
    /// Gets the instances.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static int Instances { get; private set; }

    /// <summary>
    /// Reset the static counter (for test isolation only).
    /// </summary>
    internal static void ResetInstances() => Instances = 0;
}
