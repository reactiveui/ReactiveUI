// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities;
using TUnit.Core;
using TUnit.Core.Enums;

namespace ReactiveUI.Testing.Tests.API;

/// <summary>
/// Checks to make sure that the API is consistent with previous releases, and new API changes are highlighted.
/// </summary>
[ExcludeFromCodeCoverage]
[RunOn(OS.Windows)]
public class ApiApprovalTests
{
    /// <summary>
    /// Generates public API for the ReactiveUI.Testing API.
    /// </summary>
    /// <returns>A task to monitor the process.</returns>
    [Test]
    public Task Testing() => typeof(Testing.SchedulerExtensions).Assembly.CheckApproval(["ReactiveUI"]);
}
