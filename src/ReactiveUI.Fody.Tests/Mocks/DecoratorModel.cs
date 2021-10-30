// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests;

/// <summary>
/// A view model that is decorated with rx fody attributes.
/// </summary>
public class DecoratorModel : BaseModel
{
    private readonly BaseModel _model;

    /// <summary>
    /// Initializes a new instance of the <see cref="DecoratorModel"/> class.
    /// </summary>
    public DecoratorModel() => _model = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DecoratorModel"/> class.
    /// </summary>
    /// <param name="baseModel">The base model which to do ReactiveDependency off.</param>
    public DecoratorModel(BaseModel baseModel) => _model = baseModel;

    /// <summary>
    /// Gets or sets a property decorated with the ReactiveAttribute.
    /// </summary>
    [Reactive]
    public string? SomeCoolNewProperty { get; set; }

    /// <summary>
    /// Gets or sets a property decorated with the ReactiveDependencyAttribute.
    /// </summary>
    [ReactiveDependency(nameof(_model))]
    public override string? StringProperty { get; set; }

    /// <summary>
    /// Gets or sets a property which interacts with the base model.
    /// </summary>
    public override int IntProperty
    {
        get => _model.IntProperty * 2;
        set
        {
            _model.IntProperty = value;
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Sets the string. This is independent of the fody generation.
    /// </summary>
    /// <param name="coolNewProperty">The new value to set.</param>
    public void UpdateCoolProperty(string coolNewProperty) => SomeCoolNewProperty = coolNewProperty;
}