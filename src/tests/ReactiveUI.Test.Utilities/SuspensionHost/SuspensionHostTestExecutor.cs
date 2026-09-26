// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Utilities.SuspensionHost;

/// <summary>Test executor for SuspensionHostExtensions tests.</summary>
/// <remarks>
/// SuspensionHostExtensions keeps its suspend/resume state per host, so each test isolates itself by creating its own
/// host. This executor only adds the AppBuilder isolation that the tests need for the service locator and logging.
/// </remarks>
public class SuspensionHostTestExecutor : AppBuilderTestExecutor;
