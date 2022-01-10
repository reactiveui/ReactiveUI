// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

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
        public OAPHIndexerTestFixture(int test)
        {
            switch (test)
            {
                case 0:
                    var temp = this.WhenAnyValue(f => f.Text)
                                               .ToProperty(this, f => f["Whatever"])
                                               .Value;
                    break;

                case 1:
                    var temp1 = this.WhenAnyValue(f => f.Text)
                                               .ToProperty(new ReactiveObject(), f => f.ToString())
                                               .Value;
                    break;

                case 2:
                    var temp2 = Observable.Return("happy")
                                                .ToProperty(this, string.Empty)
                                                .Value;
                    break;
            }
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
#pragma warning disable RCS1163 // Unused parameter.
        public string? this[string propertyName] => string.Empty;
#pragma warning restore RCS1163 // Unused parameter.
    }
}
