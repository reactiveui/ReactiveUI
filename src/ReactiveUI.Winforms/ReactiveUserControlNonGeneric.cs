// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Winforms;

/// <summary>
/// This is an  UserControl that is both and UserControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <seealso cref="System.Windows.Forms.UserControl" />
/// <seealso cref="ReactiveUI.IViewFor" />
public partial class ReactiveUserControlNonGeneric : UserControl, IViewFor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUserControlNonGeneric"/> class.
    /// </summary>
    public ReactiveUserControlNonGeneric() => InitializeComponent();

    /// <inheritdoc/>
    object? IViewFor.ViewModel { get; set; }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
