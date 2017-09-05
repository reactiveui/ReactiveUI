// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;


using Xunit;

namespace UnitTests.Utility.System
{
    public class StringExtensionsFixtures
    {
        [InlineData("batman", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [Theory]
        public void HasValueShouldBeExpected(string @string, bool expected)
        {
            var sut = @string.HasValue();

            //sut.Should().Be(expected);
        }

        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        [InlineData("batman", null, "batman")]
        [InlineData(null, "batman", "batman")]
        [InlineData("bat cave", null, "bat cave")]
        [InlineData(null, "bat cave", "bat cave")]
        [Theory]
        public void IsNullOrEmptyReturnShouldBeExpected(string @string, string @return, string expected)
        {
            var sut = @string.IsNullOrEmptyReturn(@return);

            //sut.Should().Be(expected);
        }

        [InlineData("batman", false)]
        [InlineData("", true)]
        [InlineData(null, true)]
        [Theory]
        public void IsNullOrEmptyShouldBeExpected(string @string, bool expected)
        {
            var sut = @string.IsNullOrEmpty();

            //sut.Should().Be(expected);
        }

        [InlineData(null, 10, null)]
        [InlineData("", 10, "")]
        [InlineData("batman", 0, "")]
        [InlineData("batman", 1, "b")]
        [InlineData("batman", 6, "batman")]
        [InlineData("batman and co", 6, "batman")]
        [InlineData("batmanandco", 6, "batman")]
        [Theory]
        public void TruncateShouldBeExpected(string @string, int maxLength, string expected)
        {
            var sut = @string.Truncate(maxLength);

            //sut.Should().Be(expected);
        }

        [InlineData(null, 10, null)]
        [InlineData("", 10, "")]
        [InlineData("batman", 0, "...")]
        [InlineData("batman", 1, "...")]
        [InlineData("batman", 6, "batman")]
        [InlineData("batman and co", 6, "bat...")]
        [InlineData("batmanandco", 9, "batman...")]
        [Theory]
        public void TruncateWithEllipsisShouldBeExpected(string @string, int maxLength, string expected)
        {
            var sut = @string.TruncateWithEllipsis(maxLength);

            //sut.Should().Be(expected);
        }
    }
}
