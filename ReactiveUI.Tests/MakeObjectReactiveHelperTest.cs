using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xunit;

namespace ReactiveUI.Tests
{
    public class NonReactiveINPCObjectMadeReactive : IReactiveNotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        TestFixture _InpcProperty;
        public TestFixture InpcProperty {
            get { return _InpcProperty; }
            set {
                if (_InpcProperty == value) {
                    return;
                }
                _InpcProperty = value;

                PropertyChanged(this, new PropertyChangedEventArgs("InpcProperty"));
            }
        }

        MakeObjectReactiveHelper _reactiveHelper;
        public NonReactiveINPCObjectMadeReactive()
        {
            _reactiveHelper = new MakeObjectReactiveHelper(this);
        }

        public IObservable<IObservedChange<object, object>> Changing {
            get { return _reactiveHelper.Changing; }
        }
        public IObservable<IObservedChange<object, object>> Changed {
            get { return _reactiveHelper.Changed; }
        }
        public IDisposable SuppressChangeNotifications() {
            return _reactiveHelper.SuppressChangeNotifications();
        }
    }

    public class MakeObjectReactiveHelperTest
    {
        [Fact]
        public void MakeObjectReactiveHelperSmokeTest()
        {
            var fixture = new NonReactiveINPCObjectMadeReactive();
            var output = fixture.Changed.CreateCollection();

            Assert.Equal(0, output.Count);

            var input = new TestFixture();
            fixture.InpcProperty = input;

            Assert.Equal(1, output.Count);
            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("InpcProperty", output[0].PropertyName);
        }
    }
}
