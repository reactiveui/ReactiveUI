// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests.ReactiveObjects.Mocks;

/// <summary>
///     A test fixture for exercising the various <c>WhenAny</c> observation overloads.
/// </summary>
/// <seealso cref="ReactiveObject" />
[DataContract]
public class WhenAnyTestFixture : ReactiveObject
{
    /// <summary>
    ///     The backing field for the <see cref="AccountService" /> property.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private AccountService _accountService = new();

    /// <summary>
    ///     The backing field for the <see cref="ProjectService" /> property.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    private ProjectService _projectService = new();

    /// <summary>
    ///     The backing field for the <see cref="Value1" /> property.
    /// </summary>
    private string _value1 = "1";

    /// <summary>
    ///     The backing field for the <see cref="Value10" /> property.
    /// </summary>
    private string? _value10;

    /// <summary>
    ///     The backing field for the <see cref="Value11" /> property.
    /// </summary>
    private string _value11 = "11";

    /// <summary>
    ///     The backing field for the <see cref="Value12" /> property.
    /// </summary>
    private string? _value12;

    /// <summary>
    ///     The backing field for the <see cref="Value2" /> property.
    /// </summary>
    private string? _value2;

    /// <summary>
    ///     The backing field for the <see cref="Value3" /> property.
    /// </summary>
    private string _value3 = "3";

    /// <summary>
    ///     The backing field for the <see cref="Value4" /> property.
    /// </summary>
    private string? _value4;

    /// <summary>
    ///     The backing field for the <see cref="Value5" /> property.
    /// </summary>
    private string _value5 = "5";

    /// <summary>
    ///     The backing field for the <see cref="Value6" /> property.
    /// </summary>
    private string? _value6;

    /// <summary>
    ///     The backing field for the <see cref="Value7" /> property.
    /// </summary>
    private string _value7 = "7";

    /// <summary>
    ///     The backing field for the <see cref="Value8" /> property.
    /// </summary>
    private string? _value8;

    /// <summary>
    ///     The backing field for the <see cref="Value9" /> property.
    /// </summary>
    private string _value9 = "9";

    /// <summary>
    ///     Gets the number of accounts found.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int AccountsFound => AccountsFoundHelper!.Value;

    /// <summary>
    ///     Gets or sets the account service.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public AccountService AccountService
    {
        get => _accountService;
        set => this.RaiseAndSetIfChanged(ref _accountService, value);
    }

    /// <summary>
    ///     Gets or sets the helper that projects the number of accounts found.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ObservableAsPropertyHelper<int>? AccountsFoundHelper { get; set; }

    /// <summary>
    ///     Gets or sets the project service.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public ProjectService ProjectService
    {
        get => _projectService;
        set => this.RaiseAndSetIfChanged(ref _projectService, value);
    }

    /// <summary>
    ///     Gets or sets the value1.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value1
    {
        get => _value1;
        set => this.RaiseAndSetIfChanged(ref _value1, value);
    }

    /// <summary>
    ///     Gets or sets the value10.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value10
    {
        get => _value10;
        set => this.RaiseAndSetIfChanged(ref _value10, value);
    }

    /// <summary>
    ///     Gets or sets the value11.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value11
    {
        get => _value11;
        set => this.RaiseAndSetIfChanged(ref _value11, value);
    }

    /// <summary>
    ///     Gets or sets the value12.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value12
    {
        get => _value12;
        set => this.RaiseAndSetIfChanged(ref _value12, value);
    }

    /// <summary>
    ///     Gets or sets the value2.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value2
    {
        get => _value2;
        set => this.RaiseAndSetIfChanged(ref _value2, value);
    }

    /// <summary>
    ///     Gets or sets the value3.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value3
    {
        get => _value3;
        set => this.RaiseAndSetIfChanged(ref _value3, value);
    }

    /// <summary>
    ///     Gets or sets the value4.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value4
    {
        get => _value4;
        set => this.RaiseAndSetIfChanged(ref _value4, value);
    }

    /// <summary>
    ///     Gets or sets the value5.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value5
    {
        get => _value5;
        set => this.RaiseAndSetIfChanged(ref _value5, value);
    }

    /// <summary>
    ///     Gets or sets the value6.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value6
    {
        get => _value6;
        set => this.RaiseAndSetIfChanged(ref _value6, value);
    }

    /// <summary>
    ///     Gets or sets the value7.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value7
    {
        get => _value7;
        set => this.RaiseAndSetIfChanged(ref _value7, value);
    }

    /// <summary>
    ///     Gets or sets the value8.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string? Value8
    {
        get => _value8;
        set => this.RaiseAndSetIfChanged(ref _value8, value);
    }

    /// <summary>
    ///     Gets or sets the value9.
    /// </summary>
    [DataMember]
    [JsonRequired]
    public string Value9
    {
        get => _value9;
        set => this.RaiseAndSetIfChanged(ref _value9, value);
    }
}
