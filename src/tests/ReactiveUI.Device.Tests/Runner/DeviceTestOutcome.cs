// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests.Runner;

/// <summary>The state of one device test.</summary>
internal enum DeviceTestOutcome
{
    /// <summary>The test started and has not finished.</summary>
    Running = 0,

    /// <summary>The test passed.</summary>
    Passed = 1,

    /// <summary>The test failed, errored or timed out.</summary>
    Failed = 2,

    /// <summary>The test was skipped.</summary>
    Skipped = 3,
}
