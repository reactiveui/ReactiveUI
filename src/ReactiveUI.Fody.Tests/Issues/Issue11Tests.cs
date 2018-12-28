// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            var model = new TestModel("foo");
            Assert.Equal("foo", model.MyProperty);
        }

        public class TestModel : ReactiveObject
        {
            public TestModel(string myProperty)
            {
                Observable.Return(myProperty).ToPropertyEx(this, x => x.MyProperty);
            }

            public extern string MyProperty
            {
                [ObservableAsProperty]
                get;
            }
        }
    }
}
