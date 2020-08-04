// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests.Issues
{
    public static class Issue47Tests
    {
        /// <summary>
        /// The "test" here is simply for these to compile
        /// Tests ObservableAsPropertyWeaver.EmitDefaultValue.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Purpose is just to be compiled.")]
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
}
