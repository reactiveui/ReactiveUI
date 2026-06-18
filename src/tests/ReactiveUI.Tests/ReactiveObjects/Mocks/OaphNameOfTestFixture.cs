// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>A fixture for the OAPH nameof override.</summary>
public class OaphNameOfTestFixture : TestFixture
{
    /// <summary>The number of leading or trailing letters to extract.</summary>
    private const int LetterCount = 3;

    /// <summary>The backing helper for the <see cref="FirstThreeLettersOfOneWord" /> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly ObservableAsPropertyHelper<string?> _firstThreeLettersOfOneWord;

    /// <summary>The backing helper for the <see cref="LastThreeLettersOfOneWord" /> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly ObservableAsPropertyHelper<string> _lastThreeLettersOfOneWord;

    /// <summary>Initializes a new instance of the <see cref="OaphNameOfTestFixture" /> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:Make sure the use of this in constructors is safe here",
        Justification = "OAPH initialization requires 'this' in the constructor; single-threaded test fixture.")]
    public OaphNameOfTestFixture()
    {
        this.WhenAnyValue(static x => x.IsOnlyOneWord)
            .Select(static x => x ?? string.Empty)
            .Select(static x => x.Length >= LetterCount ? x.Substring(0, LetterCount) : x)
            .ToProperty(
                this,
                nameof(FirstThreeLettersOfOneWord),
                out _firstThreeLettersOfOneWord);
        _lastThreeLettersOfOneWord = this.WhenAnyValue(static x => x.IsOnlyOneWord)
            .Select(static x => x ?? string.Empty)
            .Select(static x => x.Length >= LetterCount ? x.Substring(x.Length - LetterCount, LetterCount) : x)
            .ToProperty(this, nameof(LastThreeLettersOfOneWord));
    }

    /// <summary>Gets the first three letters of one word.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public string? FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;

    /// <summary>Gets the last three letters of one word.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public string LastThreeLettersOfOneWord => _lastThreeLettersOfOneWord.Value;
}
