// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests the awaiters.
    /// </summary>
    public class AwaiterTest
    {
        /// <summary>
        /// A smoke test for Awaiters.
        /// </summary>
        [Fact]
        public void AwaiterSmokeTest()
        {
            var fixture = AwaitAnObservable();
            fixture.Wait();

            Assert.Equal(42, fixture.Result);
        }

        private static async Task<int> AwaitAnObservable()
        {
            var o = Observable.Start(
                () =>
                {
                    Thread.Sleep(1000);
                    return 42;
                },
                RxApp.TaskpoolScheduler);

            var ret = await o;
            return ret;
        }
    }
}
