// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace ReactiveUI.Tests.Wpf
{
    /// <summary>
    /// Always False Mode Detector.
    /// </summary>
    /// <seealso cref="Splat.IModeDetector" />
    public class AlwaysFalseModeDetector : IModeDetector
    {
        /// <summary>
        /// Gets a value indicating whether the current library or application is running through a unit test.
        /// </summary>
        /// <returns>
        /// If we are currently running in a unit test.
        /// </returns>
        public bool? InUnitTestRunner() => false;
    }
}
