// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;

namespace ReactiveUI.Tests.Suspension
{
    public class SuspensionHostExtensionsTests
    {
        [Fact]
        public void GetAppStateReturns()
        {
            var fixture = new SuspensionHost
            {
                AppState = new DummyAppState()
            };

            var result = fixture.GetAppState<DummyAppState>();

            result.Should().Be(fixture.AppState);
        }

        [Fact]
        public void NullSuspensionHostThrowsException()
        {
            Action result = () => ((SuspensionHost)null!).SetupDefaultSuspendResume();

            result.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullAppStateDoesNotThrowException()
        {
            var fixture = new SuspensionHost();

            var result = Record.Exception(() => fixture.SetupDefaultSuspendResume());

            result.Should().BeNull();
        }

        [Fact]
        public void ObserveAppStateDoesNotThrowException()
        {
            var fixture = new SuspensionHost();

            var result = Record.Exception(() => fixture.ObserveAppState<DummyAppState>().Subscribe());

            result.Should().BeNull();
        }

        [Fact]
        public void ObserveAppStateDoesNotThrowInvalidCastException()
        {
            var fixture = new SuspensionHost();

            Action result = () => fixture.ObserveAppState<DummyAppState>().Subscribe();

            result.Should().NotThrow<InvalidCastException>();
        }
    }
}
