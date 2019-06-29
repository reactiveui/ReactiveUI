// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Testing.Tests
{
    /// <summary>
    /// Test fixture.
    /// </summary>
    public class TestFixture
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        public IEnumerable<string> Tests { get; set; }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        [SuppressMessage("Design", "CA2227: Read only dictionary", Justification = "Used in mock.")]
        public Dictionary<string, string> Variables { get; set; }
    }
}
