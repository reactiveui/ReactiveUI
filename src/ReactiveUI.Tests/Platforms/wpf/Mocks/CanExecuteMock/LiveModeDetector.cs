// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests.Wpf
{
    public static class LiveModeDetector
    {
        private static AlwaysFalseModeDetector liveModeDetector = new();
        private static DefaultModeDetector defaultModeDetector = new();

        public static void UseRuntimeThreads() =>
            ModeDetector.OverrideModeDetector(liveModeDetector);

        public static void UseDefaultModeDetector() =>
            ModeDetector.OverrideModeDetector(defaultModeDetector);

        public static bool? InUnitTestRunner() => ModeDetector.InUnitTestRunner();
    }
}
