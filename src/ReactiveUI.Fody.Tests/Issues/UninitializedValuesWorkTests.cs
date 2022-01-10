// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Fody.Helpers;
using Xunit;

namespace ReactiveUI.Fody.Tests.Issues;

/// <summary>
/// Makes sure that uninitialized values, which don't have ToPropertyEx called work.
/// </summary>
public class UninitializedValuesWorkTests
{
    /// <summary>
    /// Test to make sure that properties without PropertyHelper return the correct value.
    /// </summary>
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

        public string? OtherProperty { get; }
    }
}
