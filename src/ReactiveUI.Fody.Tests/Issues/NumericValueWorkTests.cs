// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;

using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests.Issues;

/// <summary>
/// A set of tests to make sure that they produce valid numeric values for different types.
/// </summary>
public static class NumericValueWorkTests
{
    /// <summary>
    /// A test to make sure that all the default values are kept after generation.
    /// </summary>
    public static void KeepsDefaultValuesTest()
    {
        var testModel = new TestModel();

        testModel.DoubleProperty.Should().Be(default(double));
        testModel.IntProperty.Should().Be(default(int));
        testModel.FloatProperty.Should().Be(default(float));
        testModel.LongProperty.Should().Be(default(long));
    }

    /// <summary>
    /// The "test" here is simply for these to compile
    /// Tests ObservableAsPropertyWeaver.EmitDefaultValue.
    /// </summary>
    private class TestModel : ReactiveObject
    {
        [ObservableAsProperty]
        public int IntProperty { get; }

        [ObservableAsProperty]
        public double DoubleProperty { get; }

        [ObservableAsProperty]
        public float FloatProperty { get; }

        [ObservableAsProperty]
        public long LongProperty { get; }
    }
}
