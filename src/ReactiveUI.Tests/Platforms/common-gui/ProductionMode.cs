// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Splat;

namespace ReactiveUI.Tests
{
    internal class ProductionMode : IModeDetector, IPlatformModeDetector
    {
        private static readonly ProductionMode Instance = new ();

        public static IDisposable Set()
        {
            PlatformModeDetector.OverrideModeDetector(Instance);
            ModeDetector.OverrideModeDetector(Instance);
            return Disposable.Create(() =>
            {
                PlatformModeDetector.OverrideModeDetector(new DefaultPlatformModeDetector());
                ModeDetector.OverrideModeDetector(new DefaultModeDetector());
            });
        }

        public bool? InUnitTestRunner() => false;

        public bool? InDesignMode() => false;
    }
}
