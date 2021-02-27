// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests the RxApp class.
    /// </summary>
    public class RxAppTest
    {
        /// <summary>
        /// Tests that schedulers should be current thread in test runner.
        /// </summary>
        [Fact]
        public void SchedulerShouldBeCurrentThreadInTestRunner()
        {
            Debug.WriteLine(RxApp.MainThreadScheduler.GetType().FullName);
            Assert.Equal(CurrentThreadScheduler.Instance, RxApp.MainThreadScheduler);
        }
    }
}
