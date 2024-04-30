﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Checks the WinForms API to make sure there aren't any unexpected public API changes.
/// </summary>
[ExcludeFromCodeCoverage]
public class WinformsApiApprovalTests
{
    /// <summary>
    /// Checks the approved vs the received API.
    /// </summary>
    /// <returns>A task to monitor the process.</returns>
    [Fact]
    public Task Winforms() => typeof(ReactiveUI.Winforms.WinformsCreatesObservableForProperty).Assembly.CheckApproval(["ReactiveUI"]);
}
