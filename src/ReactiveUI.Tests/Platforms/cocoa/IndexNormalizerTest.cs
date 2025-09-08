// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ReactiveUI;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the different indexes on the cocoa platform.
    /// </summary>
    public class IndexNormalizerTest
    {
        /// <summary>
        /// Tests to make sure that the index updates are correctly normalized.
        /// </summary>
        /// <param name="inputUpdatesString">The input updates string.</param>
        /// <param name="expectedOutputUpdatesString">The expected output updates string.</param>
        [Test]
        [TestCase("", "")]
        [TestCase("D0:D0", "D0:D1")]
        [TestCase("D0:D0:D0", "D0:D1:D2")]
        [TestCase("D2:D0:D1", "D2:D0:D3")]
        [TestCase("D1:D0", "D1:D0")]
        [TestCase("D0:D1", "D0:D2")]
        [TestCase("D0:D5:D10", "D0:D6:D12")]
        [TestCase("D5:D0:D10", "D5:D0:D12")]
        [TestCase("A0:D1", "A0:D0")]
        [TestCase("D0:A0", "D0:A0")]
        [TestCase("D0:A1", "D0:A1")]
        [TestCase("A0:D0", "")]
        [TestCase("A0:A0:D0", "A0")]
        [TestCase("A1:A1:D0", "A1:A0:D0")]
        [TestCase("A1:D0:A1", "A0:D0:A1")]
        [TestCase("A1:D1", "")]
        [TestCase("A0:A1:D0", "A0")]
        [TestCase("A0:A0", "A1:A0")]
        [TestCase("A0:A0:A0", "A2:A1:A0")]
        [TestCase("A0:A1", "A0:A1")]
        [TestCase("A1:A0", "A2:A0")]
        [TestCase("A0:A10:D5:A6:D3:D6", "A0:A8:D4:A5:D2:D6")]
        [TestCase("A0:A10:D5:A6:D7:D6", "A0:A8:D4:D6")]
        [TestCase("A0:A0:A0:D0:D1", "A0")]
        [TestCase("A0:A1:A2:D0:D1", "A0")]
        [TestCase("A0:A10:D5:D7", "A0:A8:D4:D7")]
        [TestCase("A0:A0:D2:A3:D4", "A1:A0:D0:A3:D2")]
        [TestCase("D0:A0:D1", "D0:A0:D1")]
        [TestCase("A0:D1:D0", "D0")]
        [TestCase("A1:D1:D1", "D1")]
        [TestCase("D0:A0:A0:A5:A2:D3:A5:D2:A3:A1", "D0:A2:A0:A7:D1:A6:A4:A1")]
        [TestCase("A2:A5:D2", "A4")]
        [TestCase("A2:D3:A5:D2", "D2:A4")]
        [TestCase("A5:A2:D3:A5:D2", "A5:D2:A4")]
        [TestCase("A5:A2:A5:D2", "A6:A4")]
        [TestCase("A7:A2:D3:A6:A2", "A9:A3:D2:A7:A2")]
        [TestCase("D0:D0:A6:A0:D5:D0:D4:A4:A0:A6", "D0:D1:A7:D6:D7:A5:A0:A6")]
        [TestCase("D0:D0:A6:D5:D4:A4:A0:A6", "D0:D1:A7:D7:D6:A5:A0:A6")]
        [TestCase("D0:D0:A6:D5:D4", "D0:D1:A4:D7:D6")]
        public void UpdatesAreCorrectlyNormalized(string inputUpdatesString, string expectedOutputUpdatesString)
        {
            var inputUpdates = inputUpdatesString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseUpdate)
                .ToList();
            var outputUpdates = IndexNormalizer.Normalize(inputUpdates);
            Assert.That(outputUpdates, Is.Not.Null);
            var formattedOutputUpdates = FormatUpdates(outputUpdates);

            Assert.That(formattedOutputUpdates, Is.EqualTo(expectedOutputUpdatesString));
        }

        private string FormatUpdates(IEnumerable<Update> updates)
        {
            return updates.Aggregate(
                new StringBuilder(),
                (current, next) =>
                {
                    if (current.Length > 0)
                    {
                        current.Append(":");
                    }

                    return current.Append(next);
                },
                x => x.ToString());
        }

        private Update ParseUpdate(string input)
        {
            var index = int.Parse(input.Substring(1));

            switch (input[0])
            {
                case 'A':
                    return Update.CreateAdd(index);
                case 'D':
                    return Update.CreateDelete(index);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
