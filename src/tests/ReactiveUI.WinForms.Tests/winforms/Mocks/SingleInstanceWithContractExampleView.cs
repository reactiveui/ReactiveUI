// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A single-instance example view registered with a contract.</summary>
[ViewContract("contract")]
[SingleInstanceView]
public class SingleInstanceWithContractExampleView : ReactiveUserControl<SingleInstanceExampleViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="SingleInstanceWithContractExampleView"/> class.</summary>
    public SingleInstanceWithContractExampleView() => Instances++;

    /// <summary>Gets the number of instances created.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static int Instances { get; private set; }

    /// <summary>Reset the static counter (for test isolation only).</summary>
    internal static void ResetInstances() => Instances = 0;
}
