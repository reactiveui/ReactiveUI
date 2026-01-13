// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Enums;

namespace ReactiveUI.Tests.API;

/// <summary>
/// Checks to make sure that the API is consistent with previous releases, and new API changes are highlighted.
/// </summary>
[ExcludeFromCodeCoverage]
[RunOn(OS.Windows)]
public class ApiApprovalTests
{
    /// <summary>
    /// Generates public API for the ReactiveUI.Winforms API.
    /// </summary>
    /// <returns>A task to monitor the process.</returns>
    [Test]
    public Task Winforms()
    {
#if WINDOWS
        return typeof(ReactiveUI.Winforms.RoutedControlHost).Assembly.CheckApproval(["ReactiveUI"]);
#else
        return Task.CompletedTask;
#endif
    }
}
