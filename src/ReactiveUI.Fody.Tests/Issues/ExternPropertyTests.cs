// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using Xunit;

namespace ReactiveUI.Fody.Tests.Issues
{
    /// <summary>
    /// Checks to make sure that extern properties work.
    /// </summary>
    public class ExternPropertyTests
    {
        /// <summary>
        /// Checks that observables passed as a parameter are supported.
        /// </summary>
        [Fact]
        public void AllowObservableAsPropertyAttributeOnAccessor()
        {
            var model = new TestModel("foo");
            Assert.Equal("foo", model.MyProperty);
        }

        private class TestModel : ReactiveObject
        {
            public TestModel(string myProperty) => Observable.Return(myProperty).ToPropertyEx(this, x => x.MyProperty);

            public extern string MyProperty
            {
                [ObservableAsProperty]
                get;
            }
        }
    }
}
