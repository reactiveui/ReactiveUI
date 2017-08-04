﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class TestUtilsTest
    {
        [Fact]
        public async Task WithAsyncScheduler()
        {
            await new TestScheduler().WithAsync(_ => Task.Run(() => { }));
        }
    }
}
