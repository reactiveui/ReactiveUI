// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Xunit;

namespace ReactiveUI.Fody.Tests;

/// <summary>
/// Tests for the ReactiveDependencyAttribute.
/// </summary>
public class ReactiveDependencyTests
{
    /// <summary>
    /// Tests to make sure that the facade returns the same valid as the dependency for the int property.
    /// </summary>
    [Fact]
    public void IntPropertyOnWeavedFacadeReturnsBaseModelIntPropertyDefaultValueTest()
    {
        var model = new BaseModel();
        var expectedResult = model.IntProperty;

        var facade = new FacadeModel(model);

        Assert.Equal(expectedResult, facade.IntProperty);
    }

    /// <summary>
    /// Tests to make sure that the facade returns the same valid as the dependency for the string property.
    /// </summary>
    [Fact]
    public void AnotherStringPropertyOnFacadeReturnsBaseModelStringPropertyDefaultValueTest()
    {
        var model = new BaseModel();
        var expectedResult = model.StringProperty;

        var facade = new FacadeModel(model);

        Assert.Equal(expectedResult, facade.AnotherStringProperty);
    }

    /// <summary>
    /// Tests to make sure that the facade returns the same valid as the dependency for the string property after being updated.
    /// </summary>
    [Fact]
    public void SettingAnotherStringPropertyUpdatesTheDependencyStringProperty()
    {
        const string? expectedResult = "New String Value";
        var facade = new FacadeModel(new()) { AnotherStringProperty = expectedResult };

        Assert.Equal(expectedResult, facade.Dependency.StringProperty);
    }

    /// <summary>
    /// Tests to make sure that the facade returns the same valid as the dependency for the int property after being updated.
    /// </summary>
    [Fact]
    public void SettingFacadeIntPropertyUpdatesDependencyIntProperty()
    {
        const int expectedResult = 999;
        var facade = new FacadeModel(new()) { IntProperty = expectedResult };

        Assert.Equal(expectedResult, facade.Dependency.IntProperty);
    }

    /// <summary>
    /// Checks to make sure that the property changed event is fired after first assignment.
    /// </summary>
    [Fact]
    public void FacadeIntPropertyChangedEventFiresOnAssignmentTest()
    {
        const string? expectedPropertyChanged = "IntProperty";
        var resultPropertyChanged = string.Empty;

        var facade = new FacadeModel(new());

        var obj = (INotifyPropertyChanged)facade;
        obj.PropertyChanged += (_, args) => resultPropertyChanged = args.PropertyName;

        facade.IntProperty = 999;

        Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
    }

    /// <summary>
    /// Checks to make sure that the property changed event is fired after first assignment.
    /// </summary>
    [Fact]
    public void FacadeAnotherStringPropertyChangedEventFiresOnAssignmentTest()
    {
        const string? expectedPropertyChanged = "AnotherStringProperty";
        var resultPropertyChanged = string.Empty;

        var facade = new FacadeModel(new());

        var obj = (INotifyPropertyChanged)facade;
        obj.PropertyChanged += (_, args) => resultPropertyChanged = args.PropertyName;

        facade.AnotherStringProperty = "Some New Value";

        Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
    }

    /// <summary>
    /// Checks to make sure that the facade and the decorate return the same value.
    /// </summary>
    [Fact]
    public void StringPropertyOnWeavedDecoratorReturnsBaseModelDefaultStringValue()
    {
        var model = new BaseModel();
        var expectedResult = model.StringProperty;

        var decorator = new DecoratorModel(model);

        Assert.Equal(expectedResult, decorator.StringProperty);
    }

    /// <summary>
    /// Checks to make sure that the decorator property changed is fired.
    /// </summary>
    [Fact]
    public void DecoratorStringPropertyRaisesPropertyChanged()
    {
        const string? expectedPropertyChanged = "StringProperty";
        var resultPropertyChanged = string.Empty;

        var decorator = new DecoratorModel(new());

        var obj = (INotifyPropertyChanged)decorator;
        obj.PropertyChanged += (_, args) => resultPropertyChanged = args.PropertyName;

        decorator.StringProperty = "Some New Value";

        Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
    }

    /// <summary>
    /// Checks to make sure that the decorator property changed is fired.
    /// </summary>
    [Fact]
    public void DecoratorReactiveStringPropertyRaisesPropertyChanged()
    {
        const string? expectedPropertyChanged = "SomeCoolNewProperty";
        var resultPropertyChanged = string.Empty;

        var decorator = new DecoratorModel(new());

        var obj = (INotifyPropertyChanged)decorator;
        obj.PropertyChanged += (_, args) => resultPropertyChanged = args.PropertyName;

        decorator.UpdateCoolProperty("Some Cool Property");
        Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
    }
}
