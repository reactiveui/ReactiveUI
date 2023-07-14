// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xunit;

namespace ReactiveUI.Tests
{
    public class RandomTests
    {
        [Fact]
        public void StringConverterAffinityTest()
        {
            var fixture = new StringConverter();
            var result = fixture.GetAffinityForObjects(typeof(object), typeof(string));
            Assert.Equal(result, 2);
            result = fixture.GetAffinityForObjects(typeof(object), typeof(int));
            Assert.Equal(result, 0);
        }

        [Fact]
        public void StringConverterTryConvertTest()
        {
            var fixture = new StringConverter();
            var expected = fixture.GetType().FullName;
            var result = fixture.TryConvert(fixture, typeof(string), null, out var actualResult);
            Assert.True(result);
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void UnhandledErrorExceptionTest()
        {
            var fixture = new UnhandledErrorException();
            Assert.Equal(fixture.Message, "Exception of type 'ReactiveUI.UnhandledErrorException' was thrown.");
        }

        [Fact]
        public void UnhandledErrorExceptionTestWithMessage()
        {
            var fixture = new UnhandledErrorException("We are terribly sorry but a unhandled error occured.");
            Assert.Equal(fixture.Message, "We are terribly sorry but a unhandled error occured.");
        }

        [Fact]
        public void UnhandledErrorExceptionTestWithMessageAndInnerException()
        {
            var fixture = new UnhandledErrorException("We are terribly sorry but a unhandled error occured.", new Exception("Inner Exception added."));
            Assert.Equal(fixture.Message, "We are terribly sorry but a unhandled error occured.");
            Assert.Equal(fixture.InnerException?.Message, "Inner Exception added.");
        }

        [Fact]
        public void ViewLocatorNotFoundExceptionTest()
        {
            var fixture = new ViewLocatorNotFoundException();
            Assert.Equal(fixture.Message, "Exception of type 'ReactiveUI.ViewLocatorNotFoundException' was thrown.");
        }

        [Fact]
        public void ViewLocatorNotFoundExceptionTestWithMessage()
        {
            var fixture = new ViewLocatorNotFoundException("We are terribly sorry but the View Locator was Not Found.");
            Assert.Equal(fixture.Message, "We are terribly sorry but the View Locator was Not Found.");
        }

        [Fact]
        public void ViewLocatorNotFoundExceptionTestWithMessageAndInnerException()
        {
            var fixture = new ViewLocatorNotFoundException("We are terribly sorry but the View Locator was Not Found.", new Exception("Inner Exception added."));
            Assert.Equal(fixture.Message, "We are terribly sorry but the View Locator was Not Found.");
            Assert.Equal(fixture.InnerException?.Message, "Inner Exception added.");
        }

        [Fact]
        public void ViewLocatorCurrentTest()
        {
            RxApp.EnsureInitialized();
            var fixture = ViewLocator.Current;
            Assert.NotNull(fixture);
        }

        [Fact]
        public void ViewLocatorCurrentFailedTest()
        {
            Locator.CurrentMutable.UnregisterCurrent(typeof(IViewLocator));
            Assert.Throws<ViewLocatorNotFoundException>(() => ViewLocator.Current);
            Locator.CurrentMutable.Register(() => new DefaultViewLocator(), typeof(IViewLocator));
        }
    }
}
