// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Fody.Helpers;
using Xunit;

namespace ReactiveUI.Fody.Tests
{
    public class ReactiveDependencyTests
    {
        [Fact]
        public void IntPropertyOnWeavedFacadeReturnsBaseModelIntPropertyDefaultValueTest()
        {
            var model = new BaseModel();
            var expectedResult = model.IntProperty;

            var facade = new FacadeModel(model);

            Assert.Equal(expectedResult, facade.IntProperty);
        }

        [Fact]
        public void AnotherStringPropertyOnFacadeReturnsBaseModelStringPropertyDefaultValueTest()
        {
            var model = new BaseModel();
            var expectedResult = model.StringProperty;

            var facade = new FacadeModel(model);

            Assert.Equal(expectedResult, facade.AnotherStringProperty);
        }

        [Fact]
        public void SettingAnotherStringPropertyUpdatesTheDependencyStringProperty()
        {
            var expectedResult = "New String Value";
            var facade = new FacadeModel(new BaseModel());

            facade.AnotherStringProperty = expectedResult;

            Assert.Equal(expectedResult, facade.Dependency.StringProperty);
        }

        [Fact]
        public void SettingFacadeIntPropertyUpdatesDependencyIntProperty()
        {
            var expectedResult = 999;
            var facade = new FacadeModel(new BaseModel());

            facade.IntProperty = expectedResult;

            Assert.Equal(expectedResult, facade.Dependency.IntProperty);
        }

        [Fact]
        public void FacadeIntPropertyChangedEventFiresOnAssignmentTest()
        {
            var expectedPropertyChanged = "IntProperty";
            var resultPropertyChanged = string.Empty;

            var facade = new FacadeModel(new BaseModel());

            var obj = (INotifyPropertyChanged)facade;
            obj.PropertyChanged += (sender, args) => resultPropertyChanged = args.PropertyName;

            facade.IntProperty = 999;

            Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
        }

        [Fact]
        public void FacadeAnotherStringPropertyChangedEventFiresOnAssignmentTest()
        {
            var expectedPropertyChanged = "AnotherStringProperty";
            var resultPropertyChanged = string.Empty;

            var facade = new FacadeModel(new BaseModel());

            var obj = (INotifyPropertyChanged)facade;
            obj.PropertyChanged += (sender, args) => resultPropertyChanged = args.PropertyName;

            facade.AnotherStringProperty = "Some New Value";

            Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
        }

        [Fact]
        public void StringPropertyOnWeavedDecoratorReturnsBaseModelDefaultStringValue()
        {
            var model = new BaseModel();
            var expectedResult = model.StringProperty;

            var decorator = new DecoratorModel(model);

            Assert.Equal(expectedResult, decorator.StringProperty);
        }

        [Fact]
        public void DecoratorStringPropertyRaisesPropertyChanged()
        {
            var expectedPropertyChanged = "StringProperty";
            var resultPropertyChanged = string.Empty;

            var decorator = new DecoratorModel(new BaseModel());

            var obj = (INotifyPropertyChanged)decorator;
            obj.PropertyChanged += (sender, args) => resultPropertyChanged = args.PropertyName;

            decorator.StringProperty = "Some New Value";

            Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
        }

        [Fact]
        public void DecoratorReactiveStringPropertyRaisesPropertyChanged()
        {
            var expectedPropertyChanged = "SomeCoolNewProperty";
            var resultPropertyChanged = string.Empty;

            var decorator = new DecoratorModel(new BaseModel());

            var obj = (INotifyPropertyChanged)decorator;
            obj.PropertyChanged += (sender, args) => resultPropertyChanged = args.PropertyName;

            decorator.UpdateCoolProperty("Some Cool Property");
            Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
        }
    }
}
