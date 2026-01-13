// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Logging;

/// <summary>
/// Extension methods for retrieving logging-related test artifacts from a <see cref="TestContext"/>.
/// </summary>
public static class LoggingRegistrationExecutorExtensions
{
    extension(TestContext context)
    {
        /// <summary>
        /// Retrieves the current test logger instance associated with the context, if one is available.
        /// </summary>
        /// <returns>A <see cref="TestLogger"/> instance if a test logger is present in the context; otherwise, <see
        /// langword="null"/>.</returns>
        public TestLogger? GetTestLogger()
        {
            ArgumentNullException.ThrowIfNull(context);
            return (TestLogger?)context.StateBag.Items["TestLogger"];
        }
    }
}
