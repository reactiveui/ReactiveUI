// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.WhenAny.Mockups;

/// <summary>A non-observable test fixture used to verify behaviour with plain CLR objects.</summary>
public class NonObservableTestFixture
{
    /// <summary>Gets or sets the child fixture.</summary>
    public TestFixture? Child { get; set; }
}
