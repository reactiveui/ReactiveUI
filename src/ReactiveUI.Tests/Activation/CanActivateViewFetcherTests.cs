// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using Xunit;

namespace ReactiveUI.Tests
{
    public class CanActivateViewFetcherTests
    {
        private class CanActivateStub : ICanActivate
        {
            public IObservable<Unit> Activated { get; }

            public IObservable<Unit> Deactivated { get; }
        }

        [Fact]
        public void ReturnsPositiveForICanActivate()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(ICanActivate));
            Assert.True(affinity > 0);
        }

        [Fact]
        public void ReturnsPositiveForICanActivateDerivatives()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateStub));
            Assert.True(affinity > 0);
        }

        [Fact]
        public void ReturnsZeroForNonICanActivateDerivatives()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateViewFetcherTests));
            Assert.Equal(0, affinity);
        }
    }
}
