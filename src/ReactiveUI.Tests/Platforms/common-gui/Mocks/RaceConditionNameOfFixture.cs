// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace ReactiveUI.Tests
{
    public class RaceConditionNameOfFixture : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _A;

        public RaceConditionNameOfFixture() =>

            // We need to generate a value on subscription
            // which is different than the default value.
            // This triggers the property change firing
            // upon subscription in the ObservableAsPropertyHelper
            // constructor.
            Observables
                .True
                .Do(_ => Count++)
                .ToProperty(this, nameof(A), out _A);

        public int Count { get; set; }

        public bool A => _A.Value;
    }
}
