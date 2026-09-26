// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests.Runner;

/// <summary>Receives results from a <see cref="DeviceTestSession"/> for a platform head to report.</summary>
internal interface IDeviceTestReporter
{
    /// <summary>Called when a test starts or finishes.</summary>
    /// <param name="result">The test update.</param>
    void OnResult(DeviceTestResult result);

    /// <summary>Called when the TRX report is written.</summary>
    /// <param name="path">The report's full path on the device.</param>
    void OnReport(string path);
}
