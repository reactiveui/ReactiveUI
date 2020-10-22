// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests
{
    internal class OAPHIndexerTestFixture : ReactiveObject
    {
        private string? _text;

        public OAPHIndexerTestFixture()
        {
            var temp = this.WhenAnyValue(f => f.Text)
                           .ToProperty(this, f => f["Whatever"])
                           .Value;
        }

        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by test.")]
        public string? this[string propertyName] => string.Empty;
    }
}
