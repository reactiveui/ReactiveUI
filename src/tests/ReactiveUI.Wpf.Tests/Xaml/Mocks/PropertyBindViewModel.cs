// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData.Binding;
using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// A property bind view model.
/// </summary>
/// <seealso cref="ReactiveObject" />
public sealed class PropertyBindViewModel : ReactiveUI.ReactiveObject
{
    /// <summary>
    /// The default value used to seed the model's <c>AThing</c> property.
    /// </summary>
    private const int DefaultAThing = 42;

    /// <summary>
    /// The default values used to seed <see cref="SomeCollectionOfStrings"/>.
    /// </summary>
    private static readonly string[] _defaultStrings = ["Foo", "Bar"];

    /// <summary>
    /// Backing field for the <see cref="JustABoolean"/> property.
    /// </summary>
    private bool _justABoolean;

    /// <summary>
    /// Backing field for the <see cref="JustAByte"/> property.
    /// </summary>
    private byte _justAByte;

    /// <summary>
    /// Backing field for the <see cref="JustADecimal"/> property.
    /// </summary>
    private decimal _justADecimal;

    /// <summary>
    /// Backing field for the <see cref="JustADouble"/> property.
    /// </summary>
    private double _justADouble;

    /// <summary>
    /// Backing field for the <see cref="JustAInt16"/> property.
    /// </summary>
    private short _justAInt16;

    /// <summary>
    /// Backing field for the <see cref="JustAInt32"/> property.
    /// </summary>
    private int _justAInt32;

    /// <summary>
    /// Backing field for the <see cref="JustAInt64"/> property.
    /// </summary>
    private long _justAInt64;

    /// <summary>
    /// Backing field for the <see cref="JustANullByte"/> property.
    /// </summary>
    private byte? _justANullByte;

    /// <summary>
    /// Backing field for the <see cref="JustANullDecimal"/> property.
    /// </summary>
    private decimal? _justANullDecimal;

    /// <summary>
    /// Backing field for the <see cref="JustANullDouble"/> property.
    /// </summary>
    private double? _justANullDouble;

    /// <summary>
    /// Backing field for the <see cref="JustANullInt16"/> property.
    /// </summary>
    private short? _justANullInt16;

    /// <summary>
    /// Backing field for the <see cref="JustANullInt32"/> property.
    /// </summary>
    private int? _justANullInt32;

    /// <summary>
    /// Backing field for the <see cref="JustANullSingle"/> property.
    /// </summary>
    private float? _justANullSingle;

    /// <summary>
    /// Backing field for the <see cref="JustASingle"/> property.
    /// </summary>
    private float _justASingle;

    /// <summary>
    /// Backing field for the <see cref="JustAVisibility"/> property.
    /// </summary>
    private Visibility _justAVisibility;

    /// <summary>
    /// Backing field for the <see cref="Model"/> property.
    /// </summary>
    private PropertyBindModel? _model;

    /// <summary>
    /// Backing field for the <see cref="NullableDouble"/> property.
    /// </summary>
    private double? _nullableDouble;

    /// <summary>
    /// Backing field for the <see cref="Property1"/> property.
    /// </summary>
    private string? _property1;

    /// <summary>
    /// Backing field for the <see cref="Property2"/> property.
    /// </summary>
    private int _property2;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBindViewModel"/> class.
    /// </summary>
    public PropertyBindViewModel()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBindViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public PropertyBindViewModel(PropertyBindModel? model)
    {
        Model = model ?? new PropertyBindModel { AThing = DefaultAThing, AnotherThing = "Baz" };
        SomeCollectionOfStrings = new ObservableCollectionExtended<string>(_defaultStrings);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [just a boolean].
    /// </summary>
    public bool JustABoolean
    {
        get => _justABoolean;
        set => this.RaiseAndSetIfChanged(ref _justABoolean, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public byte JustAByte
    {
        get => _justAByte;
        set => this.RaiseAndSetIfChanged(ref _justAByte, value);
    }

    /// <summary>
    /// Gets or sets the just a decimal.
    /// </summary>
    public decimal JustADecimal
    {
        get => _justADecimal;
        set => this.RaiseAndSetIfChanged(ref _justADecimal, value);
    }

    /// <summary>
    /// Gets or sets the just a double.
    /// </summary>
    public double JustADouble
    {
        get => _justADouble;
        set => this.RaiseAndSetIfChanged(ref _justADouble, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public short JustAInt16
    {
        get => _justAInt16;
        set => this.RaiseAndSetIfChanged(ref _justAInt16, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public int JustAInt32
    {
        get => _justAInt32;
        set => this.RaiseAndSetIfChanged(ref _justAInt32, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public long JustAInt64
    {
        get => _justAInt64;
        set => this.RaiseAndSetIfChanged(ref _justAInt64, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public byte? JustANullByte
    {
        get => _justANullByte;
        set => this.RaiseAndSetIfChanged(ref _justANullByte, value);
    }

    /// <summary>
    /// Gets or sets the just a decimal.
    /// </summary>
    public decimal? JustANullDecimal
    {
        get => _justANullDecimal;
        set => this.RaiseAndSetIfChanged(ref _justANullDecimal, value);
    }

    /// <summary>
    /// Gets or sets the just a double.
    /// </summary>
    public double? JustANullDouble
    {
        get => _justANullDouble;
        set => this.RaiseAndSetIfChanged(ref _justANullDouble, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public short? JustANullInt16
    {
        get => _justANullInt16;
        set => this.RaiseAndSetIfChanged(ref _justANullInt16, value);
    }

    /// <summary>
    /// Gets or sets the just a int32.
    /// </summary>
    public int? JustANullInt32
    {
        get => _justANullInt32;
        set => this.RaiseAndSetIfChanged(ref _justANullInt32, value);
    }

    /// <summary>
    /// Gets or sets the just a single.
    /// </summary>
    public float? JustANullSingle
    {
        get => _justANullSingle;
        set => this.RaiseAndSetIfChanged(ref _justANullSingle, value);
    }

    /// <summary>
    /// Gets or sets the just a single.
    /// </summary>
    public float JustASingle
    {
        get => _justASingle;
        set => this.RaiseAndSetIfChanged(ref _justASingle, value);
    }

    /// <summary>
    /// Gets or sets the just a visibility.
    /// </summary>
    public Visibility JustAVisibility
    {
        get => _justAVisibility;
        set => this.RaiseAndSetIfChanged(ref _justAVisibility, value);
    }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    public PropertyBindModel? Model
    {
        get => _model;
        set => this.RaiseAndSetIfChanged(ref _model, value);
    }

    /// <summary>
    /// Gets or sets the nullable double.
    /// </summary>
    public double? NullableDouble
    {
        get => _nullableDouble;
        set => this.RaiseAndSetIfChanged(ref _nullableDouble, value);
    }

    /// <summary>
    /// Gets or sets the property1.
    /// </summary>
    public string? Property1
    {
        get => _property1;
        set => this.RaiseAndSetIfChanged(ref _property1, value);
    }

    /// <summary>
    /// Gets or sets the property2.
    /// </summary>
    public int Property2
    {
        get => _property2;
        set => this.RaiseAndSetIfChanged(ref _property2, value);
    }

    /// <summary>
    /// Gets some collection of strings.
    /// </summary>
    public ObservableCollectionExtended<string> SomeCollectionOfStrings { get; }
}
