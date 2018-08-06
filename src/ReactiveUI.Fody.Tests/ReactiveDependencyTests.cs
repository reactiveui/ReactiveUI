// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        public void FacadeIntPropertyChangedEventFiresOnAssignementTest()
        {
            var expectedPropertyChanged = "IntProperty";
            var resultPropertyChanged = string.Empty;

            var facade = new FacadeModel(new BaseModel());

            var obj = (INotifyPropertyChanged) facade;
            obj.PropertyChanged += (sender, args) => resultPropertyChanged = args.PropertyName;

            facade.IntProperty = 999;

            Assert.Equal(expectedPropertyChanged, resultPropertyChanged);
        }

        [Fact]
        public void FacadeAnotherStringPropertyChangedEventFiresOnAssignementTest()
        {
            var expectedPropertyChanged = "AnotherStringProperty";
            var resultPropertyChanged = string.Empty;

            var facade = new FacadeModel(new BaseModel());

            var obj = (INotifyPropertyChanged) facade;
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

            var obj = (INotifyPropertyChanged) decorator;
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

    public class BaseModel : ReactiveObject
    {
        public virtual int IntProperty { get; set; } = 5;
        public virtual string StringProperty { get; set; } = "Initial Value";
    }

    public class FacadeModel : ReactiveObject
    {
        private BaseModel _dependency;

        public FacadeModel()
        {
            _dependency = new BaseModel();
        }

        public FacadeModel(BaseModel dependency)
        {
            _dependency = dependency;
        }

        public BaseModel Dependency
        {
            get { return _dependency; }
            private set { _dependency = value; }
        }

        // Property with the same name, will look for a like for like name on the named dependency
        [ReactiveDependency(nameof(Dependency))]
        public int IntProperty { get; set; }

        // Property named differently to that on the dependency but still pass through value
        [ReactiveDependency(nameof(Dependency), TargetProperty = "StringProperty")]
        public string AnotherStringProperty { get; set; }
    }

    public class DecoratorModel : BaseModel
    {
        private readonly BaseModel _model;

        // Testing ctor
        public DecoratorModel()
        {
            _model = new BaseModel();
        }

        public DecoratorModel(BaseModel baseModel)
        {
            _model = baseModel;
        }

        [Reactive]
        public string SomeCoolNewProperty { get; set; }

        // Works with private fields
        [ReactiveDependency(nameof(_model))]
        public override string StringProperty { get; set; }

        // Can't be attributed as has additional functionality in the decorated get
        public override int IntProperty
        {
            get { return _model.IntProperty * 2; }
            set
            {
                _model.IntProperty = value;
                this.RaisePropertyChanged();
            }
        }

        public void UpdateCoolProperty(string coolNewProperty)
        {
            SomeCoolNewProperty = coolNewProperty;
        }
    }
}
