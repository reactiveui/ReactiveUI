// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using ReactiveUI.Fody.Helpers;

using Xunit;

namespace ReactiveUI.Fody.Tests.API;

/// <summary>
/// Tests for checking if the Fody public API is not changing.
/// We have a file checked into the repository with the approved public API.
/// If it is changing you'll need to override to make it obvious the API has changed
/// for version changing reasons.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApiApprovalTests : ApiApprovalBase
{
    /// <summary>
    /// Checks the version API.
    /// </summary>
    [Fact]
    public void ReactiveUIFody() => CheckApproval(typeof(ReactiveAttribute).Assembly);
}