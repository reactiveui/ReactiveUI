// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>
///     Initializes a new instance of the <see cref="OaphTestFixture" /> class.
/// </summary>
/// <seealso cref="TestFixture" />
public class OaphTestFixture : TestFixture
{
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly ObservableAsPropertyHelper<string?> _firstThreeLettersOfOneWord;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OaphTestFixture" /> class.
    /// </summary>
    public OaphTestFixture() => this.WhenAnyValue(static x => x.IsOnlyOneWord).Select(static x => x ?? string.Empty)
        .Select(static x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(
            this,
            static x => x.FirstThreeLettersOfOneWord,
            out _firstThreeLettersOfOneWord);

    /// <summary>
    ///     Gets the first three letters of one word.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public string? FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;
}
