// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A property bind view model.</summary>
/// <seealso cref="ReactiveObject" />
public sealed class PropertyBindViewModel : ReactiveObject
{
    /// <summary>The default value used to seed the model's <c>AThing</c> property.</summary>
    private const int DefaultAThing = 42;

    /// <summary>The default values used to seed <see cref="SomeCollectionOfStrings"/>.</summary>
    private static readonly string[] _defaultStrings = ["Foo", "Bar"];

    /// <summary>Initializes a new instance of the <see cref="PropertyBindViewModel"/> class.</summary>
    public PropertyBindViewModel()
        : this(null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PropertyBindViewModel"/> class.</summary>
    /// <param name="model">The model.</param>
    public PropertyBindViewModel(PropertyBindModel? model)
    {
        Model = model ?? new PropertyBindModel { AThing = DefaultAThing, AnotherThing = "Baz" };
        SomeCollectionOfStrings = new ObservableCollection<string>(_defaultStrings);
    }

    /// <summary>Gets or sets a value indicating whether [just a boolean].</summary>
    public bool JustABoolean
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public byte JustAByte
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a decimal.</summary>
    public decimal JustADecimal
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a double.</summary>
    public double JustADouble
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public short JustAInt16
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public int JustAInt32
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public long JustAInt64
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public byte? JustANullByte
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a decimal.</summary>
    public decimal? JustANullDecimal
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a double.</summary>
    public double? JustANullDouble
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public short? JustANullInt16
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a int32.</summary>
    public int? JustANullInt32
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a single.</summary>
    public float? JustANullSingle
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a single.</summary>
    public float JustASingle
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the just a visibility.</summary>
    public Visibility JustAVisibility
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the model.</summary>
    public PropertyBindModel? Model
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the nullable double.</summary>
    public double? NullableDouble
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property1.</summary>
    public string? Property1
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the property2.</summary>
    public int Property2
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets some collection of strings.</summary>
    public ObservableCollection<string> SomeCollectionOfStrings { get; }
}
