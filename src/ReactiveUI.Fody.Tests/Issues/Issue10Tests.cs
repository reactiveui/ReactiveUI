// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Fody.Helpers;
using Xunit;

namespace ReactiveUI.Fody.Tests.Issues
{
    public class Issue10Tests
    {
        [Fact]
        public void UninitializedObservableAsPropertyHelperDoesntThrowAndReturnsDefaultValue()
        {
            var model = new TestModel();
            Assert.Equal(null, model.MyProperty);
            Assert.Equal(0, model.MyIntProperty);
            Assert.Equal(default(DateTime), model.MyDateTimeProperty);
        }

        private class TestModel : ReactiveObject
        {
            public TestModel() => OtherProperty = MyProperty;

            [ObservableAsProperty]
            public string? MyProperty { get; private set; }

            [ObservableAsProperty]
            public int MyIntProperty { get; private set; }

            [ObservableAsProperty]
            public DateTime MyDateTimeProperty { get; private set; }

            public string? OtherProperty { get; private set; }
        }
    }
}
