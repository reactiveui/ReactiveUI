// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using Xunit;

namespace ReactiveUI.Fody.Tests.Issues
{
    public class Issue11Tests
    {
        [Fact]
        public void AllowObservableAsPropertyAttributeOnAccessor()
        {
            var model = new Issue11TestModel("foo");
            Assert.Equal("foo", model.MyProperty);
        }
    }
}
