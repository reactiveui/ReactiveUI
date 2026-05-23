// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A fake view model.
/// </summary>
public class FakeWinformsView : Control, IViewFor<FakeWinformViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeWinformsView"/> class.
    /// </summary>
    public FakeWinformsView()
    {
        Property1 = new();
        Property2 = new();
        Property3 = new();
        Property4 = new();
        BooleanProperty = new();
        SomeDouble = new();
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (FakeWinformViewModel?)value;
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FakeWinformViewModel? ViewModel { get; set; }

    /// <summary>
    /// Gets the property1.
    /// </summary>
    public Button Property1 { get; }

    /// <summary>
    /// Gets the property2.
    /// </summary>
    public Label Property2 { get; }

    /// <summary>
    /// Gets the property3.
    /// </summary>
    public TextBox Property3 { get; }

    /// <summary>
    /// Gets the property4.
    /// </summary>
    public RichTextBox Property4 { get; }

    /// <summary>
    /// Gets the boolean property.
    /// </summary>
    public CheckBox BooleanProperty { get; }

    /// <summary>
    /// Gets some double.
    /// </summary>
    public TextBox SomeDouble { get; }
}
