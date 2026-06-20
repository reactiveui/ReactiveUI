// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>A test fixture for exercising the various <c>WhenAny</c> observation overloads.</summary>
/// <seealso cref="ReactiveObject" />
[DataContract]
public class WhenAnyTestFixture : ReactiveObject
{
    /// <summary>Gets the number of accounts found.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int AccountsFound => AccountsFoundHelper!.Value;

    /// <summary>Gets or sets the account service.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public AccountService AccountService
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <summary>Gets or sets the helper that projects the number of accounts found.</summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ObservableAsPropertyHelper<int>? AccountsFoundHelper { get; set; }

    /// <summary>Gets or sets the project service.</summary>
    [DataMember]
    [JsonRequired]
    [field: IgnoreDataMember]
    [field: JsonIgnore]
    public ProjectService ProjectService
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <summary>Gets or sets the value1.</summary>
    [DataMember]
    [JsonRequired]
    public string Value1
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "1";

    /// <summary>Gets or sets the value10.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value10
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value11.</summary>
    [DataMember]
    [JsonRequired]
    public string Value11
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "11";

    /// <summary>Gets or sets the value12.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value12
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value2.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value3.</summary>
    [DataMember]
    [JsonRequired]
    public string Value3
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "3";

    /// <summary>Gets or sets the value4.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value4
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value5.</summary>
    [DataMember]
    [JsonRequired]
    public string Value5
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "5";

    /// <summary>Gets or sets the value6.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value6
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value7.</summary>
    [DataMember]
    [JsonRequired]
    public string Value7
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "7";

    /// <summary>Gets or sets the value8.</summary>
    [DataMember]
    [JsonRequired]
    public string? Value8
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the value9.</summary>
    [DataMember]
    [JsonRequired]
    public string Value9
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "9";
}
