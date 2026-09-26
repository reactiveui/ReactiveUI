// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests.Runner;

/// <summary>One test update from the device test session.</summary>
/// <param name="Uid">The unique test node id.</param>
/// <param name="DisplayName">The test display name.</param>
/// <param name="ClassName">The fully qualified test class name, when known.</param>
/// <param name="Outcome">The test outcome.</param>
/// <param name="Message">The failure or skip message, when there is one.</param>
/// <param name="Details">The full exception text of a failure, when there is one.</param>
internal sealed record DeviceTestResult(
    string Uid,
    string DisplayName,
    string? ClassName,
    DeviceTestOutcome Outcome,
    string? Message,
    string? Details);
