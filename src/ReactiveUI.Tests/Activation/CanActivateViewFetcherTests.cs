﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;

using Xunit;

namespace ReactiveUI.Tests
{
    public class CanActivateViewFetcherTests
    {
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

        #pragma warning disable CA1812 // Class is not instantiated
        private class CanActivateStub : ICanActivate
        {
            public IObservable<Unit> Activated { get; } = Observable.Empty<Unit>();

            public IObservable<Unit> Deactivated { get; } = Observable.Empty<Unit>();
        }
    }
}
