// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests;

/// <summary>
/// A fixture for the OAPH nameof override.
/// </summary>
public class OaphNameOfTestFixture : TestFixture
{
        [IgnoreDataMember]
        [JsonIgnore]
        private readonly ObservableAsPropertyHelper<string?> _firstThreeLettersOfOneWord;

        [IgnoreDataMember]
        [JsonIgnore]
        private readonly ObservableAsPropertyHelper<string> _lastThreeLettersOfOneWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="OaphNameOfTestFixture"/> class.
        /// </summary>
        public OaphNameOfTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, nameof(FirstThreeLettersOfOneWord), out _firstThreeLettersOfOneWord);
            _lastThreeLettersOfOneWord = this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(x.Length - 3, 3) : x).ToProperty(this, nameof(LastThreeLettersOfOneWord));
        }

        /// <summary>
        /// Gets the first three letters of one word.
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public string? FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;

        /// <summary>
        /// Gets the last three letters of one word.
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public string LastThreeLettersOfOneWord => _lastThreeLettersOfOneWord.Value;
}
