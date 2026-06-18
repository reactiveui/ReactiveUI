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
    /// <summary>Initializes a new instance of the <see cref="TestFixture" /> class.</summary>
    public TestFixture() => TestCollection = [];

    /// <summary>Gets or sets the is not null string.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public string? IsNotNullString
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the is only one word.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public string? IsOnlyOneWord
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the not serialized.</summary>
    public string? NotSerialized
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the nullable int.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public int? NullableInt
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the poco property.</summary>
    [field: IgnoreDataMember]
    [JsonIgnore]
    public string? PocoProperty { get; set; }

    /// <summary>Gets or sets the stack overflow trigger.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public List<string>? StackOverflowTrigger
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value?.ToList());
    }

    /// <summary>Gets or sets the test collection.</summary>
    [DataMember]
    [JsonRequired]
    public ObservableCollection<int> TestCollection { get; set; }

    /// <summary>Gets or sets the uses expr raise set.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public string? UsesExprRaiseSet
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
