// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>A fixture for testing the OAPH expression-based override.</summary>
/// <seealso cref="TestFixture" />
public class OaphTestFixture : TestFixture
{
    /// <summary>The number of leading characters exposed by <see cref="FirstThreeLettersOfOneWord" />.</summary>
    private const int PrefixLength = 3;

    /// <summary>The backing helper for the <see cref="FirstThreeLettersOfOneWord" /> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private readonly ObservableAsPropertyHelper<string?> _firstThreeLettersOfOneWord;

    /// <summary>Initializes a new instance of the <see cref="OaphTestFixture" /> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:Make sure the use of this in constructors is safe here",
        Justification = "OAPH initialization requires 'this' in the constructor; single-threaded test fixture.")]
    public OaphTestFixture() => this.WhenAnyValue(static x => x.IsOnlyOneWord)
        .Select(static x => x ?? string.Empty)
        .Select(static x => x.Length >= PrefixLength ? x.Substring(0, PrefixLength) : x)
        .ToProperty(
            this,
            static x => x.FirstThreeLettersOfOneWord,
            out _firstThreeLettersOfOneWord);

    /// <summary>Gets the first three letters of one word.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public string? FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;
}
