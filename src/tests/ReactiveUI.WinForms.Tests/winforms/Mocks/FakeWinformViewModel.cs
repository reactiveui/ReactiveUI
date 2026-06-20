// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A fake view model.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeWinformViewModel"/> class.
/// </remarks>
/// <param name="screen">The screen.</param>
public class FakeWinformViewModel(IScreen? screen = null) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => "fake";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = screen ?? new TestScreen();

    /// <summary>Gets or sets some integer.</summary>
    public int SomeInteger
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets some text.</summary>
    public string? SomeText
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets some double.</summary>
    public double SomeDouble
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property1.</summary>
    public string? Property1
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property2.</summary>
    public string? Property2
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property3.</summary>
    public string? Property3
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property4.</summary>
    public string? Property4
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets a value indicating whether [boolean property].</summary>
    public bool BooleanProperty
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
