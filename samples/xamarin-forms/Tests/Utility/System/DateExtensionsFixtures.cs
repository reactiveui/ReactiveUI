// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;

using Xunit;

namespace UnitTests.Utility.System
{
    public class DateExtensionsFixtures
    {
        [InlineData("Tue, 26 Apr 2016 07:55:04 GMT", 1461693304000)]
        [Theory]
        public void ToEpochTimeMillisecondsShouldBeExpected(string dateString, long expected)
        {
            var dateTime = DateTime.Parse(dateString);
            var sut = dateTime.ToEpochTime(toMilliseconds: true);

            //sut.Should().Be(expected);
        }

        [InlineData("Tue, 26 Apr 2016 07:55:04 GMT", 1461693304)]
        [Theory]
        public void ToEpochTimeShouldBeExpected(string dateString, long expected)
        {
            var dateTime = DateTime.Parse(dateString);
            var sut = dateTime.ToEpochTime();

            //sut.Should().Be(expected);
        }
    }
}
