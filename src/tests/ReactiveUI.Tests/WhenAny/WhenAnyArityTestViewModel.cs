// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// A reactive view model fixture exposing string and observable properties for the arity-based WhenAny mixin tests.
/// </summary>
internal sealed class WhenAnyArityTestViewModel : ReactiveObject
{
    /// <summary>The backing field for <see cref="Property1" />.</summary>
    private string? _property1;

    /// <summary>The backing field for <see cref="Property2" />.</summary>
    private string? _property2;

    /// <summary>The backing field for <see cref="Property3" />.</summary>
    private string? _property3;

    /// <summary>The backing field for <see cref="Property4" />.</summary>
    private string? _property4;

    /// <summary>The backing field for <see cref="Property5" />.</summary>
    private string? _property5;

    /// <summary>The backing field for <see cref="Property6" />.</summary>
    private string? _property6;

    /// <summary>The backing field for <see cref="Property7" />.</summary>
    private string? _property7;

    /// <summary>The backing field for <see cref="Property8" />.</summary>
    private string? _property8;

    /// <summary>The backing field for <see cref="Property9" />.</summary>
    private string? _property9;

    /// <summary>The backing field for <see cref="Property10" />.</summary>
    private string? _property10;

    /// <summary>The backing field for <see cref="Property11" />.</summary>
    private string? _property11;

    /// <summary>The backing field for <see cref="Property12" />.</summary>
    private string? _property12;

    /// <summary>The backing field for <see cref="ObservableProperty1" />.</summary>
    private IObservable<string>? _observableProperty1;

    /// <summary>The backing field for <see cref="ObservableProperty2" />.</summary>
    private IObservable<string>? _observableProperty2;

    /// <summary>The backing field for <see cref="ObservableProperty3" />.</summary>
    private IObservable<string>? _observableProperty3;

    /// <summary>The backing field for <see cref="ObservableProperty4" />.</summary>
    private IObservable<string>? _observableProperty4;

    /// <summary>The backing field for <see cref="ObservableProperty5" />.</summary>
    private IObservable<string>? _observableProperty5;

    /// <summary>The backing field for <see cref="ObservableProperty6" />.</summary>
    private IObservable<string>? _observableProperty6;

    /// <summary>The backing field for <see cref="ObservableProperty7" />.</summary>
    private IObservable<string>? _observableProperty7;

    /// <summary>The backing field for <see cref="ObservableProperty8" />.</summary>
    private IObservable<string>? _observableProperty8;

    /// <summary>The backing field for <see cref="ObservableProperty9" />.</summary>
    private IObservable<string>? _observableProperty9;

    /// <summary>The backing field for <see cref="ObservableProperty10" />.</summary>
    private IObservable<string>? _observableProperty10;

    /// <summary>The backing field for <see cref="ObservableProperty11" />.</summary>
    private IObservable<string>? _observableProperty11;

    /// <summary>The backing field for <see cref="ObservableProperty12" />.</summary>
    private IObservable<string>? _observableProperty12;

    /// <summary>Gets or sets the first property.</summary>
    public string? Property1
    {
        get => _property1;
        set => this.RaiseAndSetIfChanged(ref _property1, value);
    }

    /// <summary>Gets or sets the second property.</summary>
    public string? Property2
    {
        get => _property2;
        set => this.RaiseAndSetIfChanged(ref _property2, value);
    }

    /// <summary>Gets or sets the third property.</summary>
    public string? Property3
    {
        get => _property3;
        set => this.RaiseAndSetIfChanged(ref _property3, value);
    }

    /// <summary>Gets or sets the fourth property.</summary>
    public string? Property4
    {
        get => _property4;
        set => this.RaiseAndSetIfChanged(ref _property4, value);
    }

    /// <summary>Gets or sets the fifth property.</summary>
    public string? Property5
    {
        get => _property5;
        set => this.RaiseAndSetIfChanged(ref _property5, value);
    }

    /// <summary>Gets or sets the sixth property.</summary>
    public string? Property6
    {
        get => _property6;
        set => this.RaiseAndSetIfChanged(ref _property6, value);
    }

    /// <summary>Gets or sets the seventh property.</summary>
    public string? Property7
    {
        get => _property7;
        set => this.RaiseAndSetIfChanged(ref _property7, value);
    }

    /// <summary>Gets or sets the eighth property.</summary>
    public string? Property8
    {
        get => _property8;
        set => this.RaiseAndSetIfChanged(ref _property8, value);
    }

    /// <summary>Gets or sets the ninth property.</summary>
    public string? Property9
    {
        get => _property9;
        set => this.RaiseAndSetIfChanged(ref _property9, value);
    }

    /// <summary>Gets or sets the tenth property.</summary>
    public string? Property10
    {
        get => _property10;
        set => this.RaiseAndSetIfChanged(ref _property10, value);
    }

    /// <summary>Gets or sets the eleventh property.</summary>
    public string? Property11
    {
        get => _property11;
        set => this.RaiseAndSetIfChanged(ref _property11, value);
    }

    /// <summary>Gets or sets the twelfth property.</summary>
    public string? Property12
    {
        get => _property12;
        set => this.RaiseAndSetIfChanged(ref _property12, value);
    }

    /// <summary>Gets or sets the first observable property.</summary>
    public IObservable<string>? ObservableProperty1
    {
        get => _observableProperty1;
        set => this.RaiseAndSetIfChanged(ref _observableProperty1, value);
    }

    /// <summary>Gets or sets the second observable property.</summary>
    public IObservable<string>? ObservableProperty2
    {
        get => _observableProperty2;
        set => this.RaiseAndSetIfChanged(ref _observableProperty2, value);
    }

    /// <summary>Gets or sets the third observable property.</summary>
    public IObservable<string>? ObservableProperty3
    {
        get => _observableProperty3;
        set => this.RaiseAndSetIfChanged(ref _observableProperty3, value);
    }

    /// <summary>Gets or sets the fourth observable property.</summary>
    public IObservable<string>? ObservableProperty4
    {
        get => _observableProperty4;
        set => this.RaiseAndSetIfChanged(ref _observableProperty4, value);
    }

    /// <summary>Gets or sets the fifth observable property.</summary>
    public IObservable<string>? ObservableProperty5
    {
        get => _observableProperty5;
        set => this.RaiseAndSetIfChanged(ref _observableProperty5, value);
    }

    /// <summary>Gets or sets the sixth observable property.</summary>
    public IObservable<string>? ObservableProperty6
    {
        get => _observableProperty6;
        set => this.RaiseAndSetIfChanged(ref _observableProperty6, value);
    }

    /// <summary>Gets or sets the seventh observable property.</summary>
    public IObservable<string>? ObservableProperty7
    {
        get => _observableProperty7;
        set => this.RaiseAndSetIfChanged(ref _observableProperty7, value);
    }

    /// <summary>Gets or sets the eighth observable property.</summary>
    public IObservable<string>? ObservableProperty8
    {
        get => _observableProperty8;
        set => this.RaiseAndSetIfChanged(ref _observableProperty8, value);
    }

    /// <summary>Gets or sets the ninth observable property.</summary>
    public IObservable<string>? ObservableProperty9
    {
        get => _observableProperty9;
        set => this.RaiseAndSetIfChanged(ref _observableProperty9, value);
    }

    /// <summary>Gets or sets the tenth observable property.</summary>
    public IObservable<string>? ObservableProperty10
    {
        get => _observableProperty10;
        set => this.RaiseAndSetIfChanged(ref _observableProperty10, value);
    }

    /// <summary>Gets or sets the eleventh observable property.</summary>
    public IObservable<string>? ObservableProperty11
    {
        get => _observableProperty11;
        set => this.RaiseAndSetIfChanged(ref _observableProperty11, value);
    }

    /// <summary>Gets or sets the twelfth observable property.</summary>
    public IObservable<string>? ObservableProperty12
    {
        get => _observableProperty12;
        set => this.RaiseAndSetIfChanged(ref _observableProperty12, value);
    }
}
