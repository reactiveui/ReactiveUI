// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;

using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests to make sure the can activate view fetcher works correctly.
    /// </summary>
    public class CanActivateViewFetcherTests
    {
        /// <summary>
        /// Tests return positive for ICanActivate.
        /// </summary>
        [Fact]
        public void ReturnPositiveForICanActivate()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(ICanActivate));
            Assert.True(affinity > 0);
        }

        /// <summary>
        /// Tests return positive for ICanActivate derivatives.
        /// </summary>
        [Fact]
        public void ReturnPositiveForICanActivateDerivatives()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateStub));
            Assert.True(affinity > 0);
        }

        /// <summary>
        /// Tests return zero for non ICanActivate derivatives.
        /// </summary>
        [Fact]
        public void ReturnZeroForNonICanActivateDerivatives()
        {
            var canActivateViewFetcher = new CanActivateViewFetcher();
            var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateViewFetcherTests));
            Assert.Equal(0, affinity);
        }

        #pragma warning disable CA1812 // Class is not instantiated
        private class CanActivateStub : ICanActivate
        {
            public IObservable<Unit> Activated { get; } = Observable.Empty<Unit>();

            public IObservable<Unit> Deactivated { get; } = Observable.Empty<Unit>();
        }
    }
}
