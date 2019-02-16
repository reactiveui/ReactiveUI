// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace EventBuilder.Console
{
    /// <summary>
    ///     The exit/return code (aka %ERRORLEVEL%) on application exit.
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Error
        /// </summary>
        Error = 1,
    }
}
