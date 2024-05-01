// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests;

[DataContract]
public class WhenAnyTestFixture : ReactiveObject
{
    [IgnoreDataMember]
    [JsonIgnore]
#pragma warning disable SA1401 // Fields should be private
    internal ObservableAsPropertyHelper<int?>? _accountsFound;

#pragma warning restore SA1401 // Fields should be private

    [IgnoreDataMember]
    [JsonIgnore]
    private AccountService _accountService = new();

    [IgnoreDataMember]
    [JsonIgnore]
    private ProjectService _projectService = new();

    private string _value1 = "1";
    private string? _value2;
    private string _value3 = "3";
    private string? _value4;
    private string _value5 = "5";
    private string? _value6;
    private string _value7 = "7";
    private string? _value8;
    private string _value9 = "9";
    private string? _value10;
    private string _value11 = "11";
    private string? _value12;

    /// <summary>
    /// Gets or sets the account service.
    /// </summary>
    /// <value>
    /// The account service.
    /// </value>
    [DataMember]
    [JsonRequired]
    public AccountService AccountService
    {
        get => _accountService;
        set => this.RaiseAndSetIfChanged(ref _accountService, value);
    }

    /// <summary>
    /// Gets or sets the project service.
    /// </summary>
    /// <value>
    /// The project service.
    /// </value>
    [DataMember]
    [JsonRequired]
    public ProjectService ProjectService
    {
        get => _projectService;
        set => this.RaiseAndSetIfChanged(ref _projectService, value);
    }

    /// <summary>
    /// Gets the first three letters of one word.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int? AccountsFound => _accountsFound!.Value;

    /// <summary>
    /// Gets or sets the value1.
    /// </summary>
    /// <value>
    /// The value1.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value1
    {
        get => _value1;
        set => this.RaiseAndSetIfChanged(ref _value1, value);
    }

    /// <summary>
    /// Gets or sets the value2.
    /// </summary>
    /// <value>
    /// The value2.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value2
    {
        get => _value2;
        set => this.RaiseAndSetIfChanged(ref _value2, value);
    }

    /// <summary>
    /// Gets or sets the value3.
    /// </summary>
    /// <value>
    /// The value3.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value3
    {
        get => _value3;
        set => this.RaiseAndSetIfChanged(ref _value3, value);
    }

    /// <summary>
    /// Gets or sets the value4.
    /// </summary>
    /// <value>
    /// The value4.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value4
    {
        get => _value4;
        set => this.RaiseAndSetIfChanged(ref _value4, value);
    }

    /// <summary>
    /// Gets or sets the value5.
    /// </summary>
    /// <value>
    /// The value5.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value5
    {
        get => _value5;
        set => this.RaiseAndSetIfChanged(ref _value5, value);
    }

    /// <summary>
    /// Gets or sets the value6.
    /// </summary>
    /// <value>
    /// The value6.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value6
    {
        get => _value6;
        set => this.RaiseAndSetIfChanged(ref _value6, value);
    }

    /// <summary>
    /// Gets or sets the value7.
    /// </summary>
    /// <value>
    /// The value7.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value7
    {
        get => _value7;
        set => this.RaiseAndSetIfChanged(ref _value7, value);
    }

    /// <summary>
    /// Gets or sets the value8.
    /// </summary>
    /// <value>
    /// The value8.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value8
    {
        get => _value8;
        set => this.RaiseAndSetIfChanged(ref _value8, value);
    }

    /// <summary>
    /// Gets or sets the value9.
    /// </summary>
    /// <value>
    /// The value9.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value9
    {
        get => _value9;
        set => this.RaiseAndSetIfChanged(ref _value9, value);
    }

    /// <summary>
    /// Gets or sets the value10.
    /// </summary>
    /// <value>
    /// The value10.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value10
    {
        get => _value10;
        set => this.RaiseAndSetIfChanged(ref _value10, value);
    }

    /// <summary>
    /// Gets or sets the value11.
    /// </summary>
    /// <value>
    /// The value11.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string Value11
    {
        get => _value11;
        set => this.RaiseAndSetIfChanged(ref _value11, value);
    }

    /// <summary>
    /// Gets or sets the value12.
    /// </summary>
    /// <value>
    /// The value12.
    /// </value>
    [DataMember]
    [JsonRequired]
    public string? Value12
    {
        get => _value12;
        set => this.RaiseAndSetIfChanged(ref _value12, value);
    }
}
