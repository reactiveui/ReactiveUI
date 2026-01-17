// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.AppBuilder;

/// <summary>
///     Test executor that sets up AppBuilder isolation for test duration.
///     Ensures tests run serially and AppBuilder state is reset before/after each test.
/// </summary>
/// <remarks>
/// This executor uses the default <see cref="BaseAppBuilderTestExecutor"/> behavior,
/// which registers core services only. For custom registrations, derive from
/// <see cref="BaseAppBuilderTestExecutor"/> and override <see cref="BaseAppBuilderTestExecutor.ConfigureAppBuilder"/>.
/// </remarks>
public class AppBuilderTestExecutor : BaseAppBuilderTestExecutor
{
    // No additional configuration needed - base class handles everything
}
