// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A test fixture for OAPH.
    /// </summary>
    internal class OAPHIndexerTestFixture : ReactiveObject
    {
        private string? _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAPHIndexerTestFixture"/> class.
        /// </summary>
        public OAPHIndexerTestFixture()
        {
            var temp = this.WhenAnyValue(f => f.Text)
                           .ToProperty(this, f => f["Whatever"])
                           .Value;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        /// <summary>
        /// Gets the string with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The string.</returns>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by test.")]
        public string? this[string propertyName] => string.Empty;
    }
}
