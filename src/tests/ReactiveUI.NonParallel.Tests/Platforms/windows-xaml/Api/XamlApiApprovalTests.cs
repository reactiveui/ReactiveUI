// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// API approvals for the xaml project.
/// </summary>
[ExcludeFromCodeCoverage]
public class XamlApiApprovalTests
{
    /// <summary>
    /// Generates the public API for the blend project.
    /// </summary>
    /// <returns>A task to monitor the process.</returns>
    [Test]
    public Task Blend() => typeof(Blend.FollowObservableStateBehavior).Assembly.CheckApproval(["ReactiveUI"]);
}
