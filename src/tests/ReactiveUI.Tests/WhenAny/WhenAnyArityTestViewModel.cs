// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>A reactive view model fixture exposing string and observable properties for the arity-based WhenAny mixin tests.</summary>
internal sealed class WhenAnyArityTestViewModel : ReactiveObject
{
    /// <summary>Gets or sets the first property.</summary>
    public string? Property1
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the second property.</summary>
    public string? Property2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the third property.</summary>
    public string? Property3
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the fourth property.</summary>
    public string? Property4
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the fifth property.</summary>
    public string? Property5
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the sixth property.</summary>
    public string? Property6
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the seventh property.</summary>
    public string? Property7
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the eighth property.</summary>
    public string? Property8
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the ninth property.</summary>
    public string? Property9
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the tenth property.</summary>
    public string? Property10
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the eleventh property.</summary>
    public string? Property11
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the twelfth property.</summary>
    public string? Property12
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the first observable property.</summary>
    public IObservable<string>? ObservableProperty1
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the second observable property.</summary>
    public IObservable<string>? ObservableProperty2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the third observable property.</summary>
    public IObservable<string>? ObservableProperty3
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the fourth observable property.</summary>
    public IObservable<string>? ObservableProperty4
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the fifth observable property.</summary>
    public IObservable<string>? ObservableProperty5
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the sixth observable property.</summary>
    public IObservable<string>? ObservableProperty6
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the seventh observable property.</summary>
    public IObservable<string>? ObservableProperty7
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the eighth observable property.</summary>
    public IObservable<string>? ObservableProperty8
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the ninth observable property.</summary>
    public IObservable<string>? ObservableProperty9
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the tenth observable property.</summary>
    public IObservable<string>? ObservableProperty10
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the eleventh observable property.</summary>
    public IObservable<string>? ObservableProperty11
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the twelfth observable property.</summary>
    public IObservable<string>? ObservableProperty12
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
