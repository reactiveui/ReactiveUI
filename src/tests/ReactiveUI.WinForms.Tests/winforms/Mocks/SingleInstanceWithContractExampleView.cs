// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Winforms;

[ViewContract("contract")]
[SingleInstanceView]
public class SingleInstanceWithContractExampleView : ReactiveUI.Winforms.ReactiveUserControl<SingleInstanceExampleViewModel>
{
    public SingleInstanceWithContractExampleView() => Instances++;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static int Instances { get; private set; }

    /// <summary>
    /// Reset the static counter (for test isolation only).
    /// </summary>
    internal static void ResetInstances() => Instances = 0;
}
