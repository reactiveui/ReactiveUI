// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>A test fixture.</summary>
/// <seealso cref="ReactiveObject" />
[DataContract]
public class TestFixture : ReactiveObject
{
    /// <summary>Backing field for the <see cref="IsNotNullString"/> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private string? _isNotNullString;

    /// <summary>Backing field for the <see cref="IsOnlyOneWord"/> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private string? _isOnlyOneWord;

    /// <summary>Backing field for the <see cref="NullableInt"/> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private int? _nullableInt;

    /// <summary>Backing field for the <see cref="StackOverflowTrigger"/> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private List<string>? _stackOverflowTrigger;

    /// <summary>Backing field for the <see cref="UsesExprRaiseSet"/> property.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private string? _usesExprRaiseSet;

    /// <summary>Initializes a new instance of the <see cref="TestFixture" /> class.</summary>
    public TestFixture() => TestCollection = [];

    /// <summary>Gets or sets the is not null string.</summary>
    [DataMember]
    [JsonRequired]
    public string? IsNotNullString
    {
        get => _isNotNullString;
        set => this.RaiseAndSetIfChanged(ref _isNotNullString, value);
    }

    /// <summary>Gets or sets the is only one word.</summary>
    [DataMember]
    [JsonRequired]
    public string? IsOnlyOneWord
    {
        get => _isOnlyOneWord;
        set => this.RaiseAndSetIfChanged(ref _isOnlyOneWord, value);
    }

    /// <summary>Gets or sets the not serialized.</summary>
    public string? NotSerialized
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the nullable int.</summary>
    [DataMember]
    [JsonRequired]
    public int? NullableInt
    {
        get => _nullableInt;
        set => this.RaiseAndSetIfChanged(ref _nullableInt, value);
    }

    /// <summary>Gets or sets the poco property.</summary>
    [field: IgnoreDataMember]
    [JsonIgnore]
    public string? PocoProperty { get; set; }

    /// <summary>Gets or sets the stack overflow trigger.</summary>
    [DataMember]
    [JsonRequired]
    public List<string>? StackOverflowTrigger
    {
        get => _stackOverflowTrigger;
        set => this.RaiseAndSetIfChanged(ref _stackOverflowTrigger, value?.ToList());
    }

    /// <summary>Gets or sets the test collection.</summary>
    [DataMember]
    [JsonRequired]
    public ObservableCollection<int> TestCollection { get; set; }

    /// <summary>Gets or sets the uses expr raise set.</summary>
    [DataMember]
    [JsonRequired]
    public string? UsesExprRaiseSet
    {
        get => _usesExprRaiseSet;
        set => this.RaiseAndSetIfChanged(ref _usesExprRaiseSet, value);
    }
}
