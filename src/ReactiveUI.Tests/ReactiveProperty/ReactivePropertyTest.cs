// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests.ReactiveProperty
{
    public class ReactivePropertyTest : ReactiveTest
    {
        [Fact]
        public void NormalCase()
        {
            var rp = new ReactiveProperty<string>();
            Assert.Null(rp.Value);
            rp.Subscribe(x => Assert.Null(x));
        }

        [Fact]
        public void InitialValue()
        {
            var rp = new ReactiveProperty<string>("Hello world");
            Assert.Equal(rp.Value, "Hello world");
            rp.Subscribe(x => Assert.Equal(x, "Hello world"));
        }
    }
}
