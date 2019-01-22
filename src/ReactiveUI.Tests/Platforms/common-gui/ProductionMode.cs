// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Splat;

namespace ReactiveUI.Tests
{
    internal class ProductionMode : IModeDetector
    {
        public static IDisposable Set()
        {
            ModeDetector.OverrideModeDetector(new ProductionMode());
            return Disposable.Create(() => ModeDetector.OverrideModeDetector(new PlatformModeDetector()));
        }

        public bool? InUnitTestRunner()
        {
            return false;
        }

        public bool? InDesignMode()
        {
            return false;
        }
    }
}
