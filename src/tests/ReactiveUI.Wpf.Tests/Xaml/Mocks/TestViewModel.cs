// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

namespace ReactiveUI.Tests.Xaml.Mocks;

public class TestViewModel : ReactiveUI.ReactiveObject, IRoutableViewModel
{
    private string? _someProp;

    public string? SomeProp
    {
        get => _someProp;
        set => this.RaiseAndSetIfChanged(ref _someProp, value);
    }

    /// <summary>
    /// Gets the URL path segment.
    /// </summary>
    public string UrlPathSegment => "Test";

    /// <summary>
    /// Gets or sets the host screen.
    /// </summary>
    public IScreen HostScreen { get; set; } = new TestScreen();
}
