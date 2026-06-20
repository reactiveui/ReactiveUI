// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A mock routable view model used by routing tests.</summary>
public class TestViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Gets or sets a sample property.</summary>
    public string? SomeProp
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the URL path segment.</summary>
    public string UrlPathSegment => "Test";

    /// <summary>Gets or sets the host screen.</summary>
    public IScreen HostScreen { get; set; } = new TestScreen();
}
