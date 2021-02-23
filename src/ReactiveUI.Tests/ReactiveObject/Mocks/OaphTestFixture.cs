// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace ReactiveUI.Tests
{
    public class OaphTestFixture : TestFixture
    {
        [IgnoreDataMember]
        private readonly ObservableAsPropertyHelper<string?> _firstThreeLettersOfOneWord;

        public OaphTestFixture() => this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, x => x.FirstThreeLettersOfOneWord, out _firstThreeLettersOfOneWord);

        [IgnoreDataMember]
        public string? FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;
    }
}
