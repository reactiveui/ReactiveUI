using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Testing;
using ReactiveUI.Xaml;
using Xunit;

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests
{
    public class TestWhenAnyObsViewModel : ReactiveObject
    {
        public ReactiveCommand Command1 { get; protected set; }
        public ReactiveCommand Command2 { get; protected set; }

        public TestWhenAnyObsViewModel()
        {
            Command1 = new ReactiveCommand();
            Command2 = new ReactiveCommand();
        }
    }

    public class HostTestFixture : ReactiveObject
    {
        public TestFixture _Child;
        public TestFixture Child {
            get { return _Child; }
            set { this.RaiseAndSetIfChanged(x => x.Child, value); }
        }

        public int _SomeOtherParam;
        public int SomeOtherParam {
            get { return _SomeOtherParam; }
            set { this.RaiseAndSetIfChanged(x => x.SomeOtherParam, value); }
        }

        public NonObservableTestFixture _PocoChild;
        public NonObservableTestFixture PocoChild {
            get { return _PocoChild; }
            set { this.RaiseAndSetIfChanged(x => x.PocoChild, value); }
        }
    }

    public class NonObservableTestFixture
    {
        public TestFixture Child {get; set;}
    }

    public class NonReactiveINPCObject : INotifyPropertyChanged
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

                if (PropertyChanged == null) return;
                PropertyChanged(this, new PropertyChangedEventArgs("InpcProperty"));
            }
        }
    }

    public class HostTestView : Control, IViewFor<HostTestFixture>
    {
        public HostTestFixture ViewModel {
            get { return (HostTestFixture)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(HostTestFixture), typeof(HostTestView), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (HostTestFixture) value; }
        }
    }

    public class ObjChain1 : ReactiveObject
    {
        public ObjChain2 _Model = new ObjChain2();
        public ObjChain2 Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(x => x.Model, value); }
        }
    }

    public class ObjChain2 : ReactiveObject
    {
        public ObjChain3 _Model = new ObjChain3();
        public ObjChain3 Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(x => x.Model, value); }
        }
    }

    public class ObjChain3 : ReactiveObject
    {
        public HostTestFixture _Model = new HostTestFixture();
        public HostTestFixture Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(x => x.Model, value); }
        }
    }

    public class ReactiveNotifyPropertyChangedMixinTest
    {
        [Fact]
        public void OFPSimplePropertyTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).CreateCollection();

                fixture.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(3, changes.Count);

                fixture.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(3, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.PropertyName == "IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] {"Foo", "Bar", "Baz"});
            });
        }

        [Fact]
        public void OFPSimpleChildPropertyTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection();

                fixture.Child.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.Child.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.Child.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(3, changes.Count);

                fixture.Child.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(3, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.PropertyName == "Child.IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] {"Foo", "Bar", "Baz"});
            });
        }

        [Fact]
        public void OFPReplacingTheHostShouldResubscribeTheObservable()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection();

                fixture.Child.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.Child.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                // Tricky! This is a change too, because from the perspective 
                // of the binding, we've went from "Bar" to null
                fixture.Child = new TestFixture();
                sched.Start();
                Assert.Equal(3, changes.Count);

                // Here we've set the value but it shouldn't change
                fixture.Child.IsOnlyOneWord = null;
                sched.Start();
                Assert.Equal(3, changes.Count);

                fixture.Child.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(4, changes.Count);

                fixture.Child.IsOnlyOneWord = "Baz";
                sched.Start();
                Assert.Equal(4, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.PropertyName == "Child.IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] {"Foo", "Bar", null, "Baz"});
            });           
        }


        [Fact]
        public void OFPReplacingTheHostWithNullThenSettingItBackShouldResubscribeTheObservable()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection();

                fixture.Child.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.Child.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                // Oops, now the child is Null, we may now blow up
                fixture.Child = null;
                sched.Start();
                Assert.Equal(2, changes.Count);

                // Tricky! This is a change too, because from the perspective 
                // of the binding, we've went from "Bar" to null
                fixture.Child = new TestFixture();
                sched.Start();
                Assert.Equal(3, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.PropertyName == "Child.IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] {"Foo", "Bar", null});
            });
        }

        [Fact]
        public void OFPChangingTheHostPropertyShouldFireAChildChangeNotificationOnlyIfThePreviousChildIsDifferent()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                var changes = fixture.ObservableForProperty(x => x.Child.IsOnlyOneWord).CreateCollection();

                fixture.Child.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.Child.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.Child = new TestFixture() {IsOnlyOneWord = "Bar"};
                sched.Start();
                Assert.Equal(2, changes.Count);
            });
        }

        [Fact]
        public void OFPShouldWorkWithINPCObjectsToo()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new NonReactiveINPCObject() { InpcProperty = null };

                var changes = fixture.ObservableForProperty(x => x.InpcProperty.IsOnlyOneWord).CreateCollection();

                fixture.InpcProperty = new TestFixture(); 
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.InpcProperty.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.InpcProperty.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(3, changes.Count);
            });
        }

        [Fact]
        public void AnyChangeInExpressionListTriggersUpdate() 
        {
            var obj = new ObjChain1();
            bool obsUpdated;

            obj.ObservableForProperty(x => x.Model.Model.Model.SomeOtherParam).Subscribe(_ => obsUpdated = true);
           
            obsUpdated = false;
            obj.Model.Model.Model.SomeOtherParam = 42;
            Assert.True(obsUpdated);
 
            obsUpdated = false;
            obj.Model.Model.Model = new HostTestFixture();
            Assert.True(obsUpdated);
 
            obsUpdated = false;
            obj.Model.Model = new ObjChain3() {Model = new HostTestFixture() {SomeOtherParam = 10 } } ;
            Assert.True(obsUpdated);
 
            obsUpdated = false;
            obj.Model = new ObjChain2();
            Assert.True(obsUpdated);
        }

        [Fact]
        public void SubscriptionToWhenAnyShouldReturnCurrentValue()
        {
            var obj = new HostTestFixture();
            int observedValue = 1;
            obj.WhenAny(x => x.SomeOtherParam, x => x.Value)
               .Subscribe(x => observedValue = x);

            obj.SomeOtherParam = 42;
            
            Assert.True(observedValue == obj.SomeOtherParam);
        }

        [Fact]
        public void MultiPropertyExpressionsShouldBeProperlyResolved()
        {
            var data = new Dictionary<Expression<Func<HostTestFixture, object>>, string[]>() {
                {x => x.Child.IsOnlyOneWord.Length, new[] {"Child", "IsOnlyOneWord", "Length"}},
                {x => x.SomeOtherParam, new[] {"SomeOtherParam"}},
                {x => x.Child.IsNotNullString, new[] {"Child", "IsNotNullString"}},
                {x => x.Child.Changed, new[] {"Child", "Changed"}},
            };

            var dataTypes = new Dictionary<Expression<Func<HostTestFixture, object>>, Type[]>() {
                {x => x.Child.IsOnlyOneWord.Length, new[] {typeof(TestFixture), typeof(string), typeof(int) }},
                {x => x.SomeOtherParam, new[] { typeof(int) }},
                {x => x.Child.IsNotNullString, new[] {typeof(TestFixture), typeof(string)}},
                {x => x.Child.Changed, new[] {typeof(TestFixture), typeof(IObservable<IObservedChange<object, object>>)}},
            };

            var results = data.Keys.Select(x => new {input = x, output = Reflection.ExpressionToPropertyNames(x)}).ToArray();
            var resultTypes = dataTypes.Keys.Select(x => new {input = x, output = Reflection.ExpressionToPropertyTypes(x)}).ToArray();

            foreach(var x in results) {
                data[x.input].AssertAreEqual(x.output);
            }
            foreach (var x in resultTypes) {
                dataTypes[x.input].AssertAreEqual(x.output);
            }
        }

        [Fact]
        public void WhenAnySmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                fixture.SomeOtherParam = 5;
                fixture.Child.IsNotNullString = "Foo";

                var output1 = new List<IObservedChange<HostTestFixture, int>>();
                var output2 = new List<IObservedChange<HostTestFixture, string>>();
                fixture.WhenAny(x => x.SomeOtherParam, x => x.Child.IsNotNullString, (sop, nns) => new {sop, nns}).Subscribe(x => {
                    output1.Add(x.sop); output2.Add(x.nns);
                });

                sched.Start();
                Assert.Equal(1, output1.Count);
                Assert.Equal(1, output2.Count);
                Assert.Equal(fixture, output1[0].Sender);
                Assert.Equal(fixture, output2[0].Sender);
                Assert.Equal(5, output1[0].Value);
                Assert.Equal("Foo", output2[0].Value);

                fixture.SomeOtherParam = 10;
                sched.Start();
                Assert.Equal(2, output1.Count);
                Assert.Equal(2, output2.Count);
                Assert.Equal(fixture, output1[1].Sender);
                Assert.Equal(fixture, output2[1].Sender);
                Assert.Equal(10, output1[1].Value);
                Assert.Equal("Foo", output2[1].Value);

                fixture.Child.IsNotNullString = "Bar";
                sched.Start();
                Assert.Equal(3, output1.Count);
                Assert.Equal(3, output2.Count);
                Assert.Equal(fixture, output1[2].Sender);
                Assert.Equal(fixture, output2[2].Sender);
                Assert.Equal(10, output1[2].Value);
                Assert.Equal("Bar", output2[2].Value);
            });
        }

        [Fact]
        public void WhenAnyShouldWorkEvenWithNormalProperties()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

            var output = new List<IObservedChange<TestFixture, string>>();
            fixture.WhenAny(x => x.PocoProperty, x => x).Subscribe(output.Add);

            Assert.Equal(1, output.Count);
            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("PocoProperty", output[0].PropertyName);
            Assert.Equal("Bamf", output[0].Value);
        }

        [Fact]
        public void WhenAnyShouldRunInContext()
        {
            var tid = Thread.CurrentThread.ManagedThreadId;

            (Scheduler.TaskPool).With(sched => {
                int whenAnyTid = 0;
                var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

                fixture.WhenAny(x => x.IsNotNullString, x => x.Value).Subscribe(x => {
                    whenAnyTid = Thread.CurrentThread.ManagedThreadId;
                });

                int timeout = 10;
                fixture.IsNotNullString = "Bar";
                while (--timeout > 0 && whenAnyTid == 0) Thread.Sleep(250);

                Assert.Equal(tid, whenAnyTid);
            });
        }

        [Fact]
        public void WhenAnyThroughAViewShouldntGiveNullValues()
        {
            var vm = new HostTestFixture() {
                Child = new TestFixture() {IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf"},
            };

            var fixture = new HostTestView();

            var output = new List<string>();

            Assert.Equal(0, output.Count);
            Assert.Null(fixture.ViewModel);

            fixture.WhenAny(x => x.ViewModel.Child.IsNotNullString, x => x.Value).Subscribe(output.Add);

            fixture.ViewModel = vm;
            Assert.Equal(1, output.Count);

            fixture.ViewModel.Child.IsNotNullString = "Bar";
            Assert.Equal(2, output.Count);
            new[] { "Foo", "Bar" }.AssertAreEqual(output);
        }
    }

    public class WhenAnyObservableTests
    {
        [Fact]
        public void WhenAnyObservableSmokeTest()
        {
            var fixture = new TestWhenAnyObsViewModel();

            var list = new List<int>();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command2)
                   .Subscribe(x => list.Add((int)x));

            Assert.Equal(0, list.Count);

            fixture.Command1.Execute(1);
            Assert.Equal(1, list.Count);

            fixture.Command2.Execute(2);
            Assert.Equal(2, list.Count);

            fixture.Command1.Execute(1);
            Assert.Equal(3, list.Count);

            Assert.True(
                new[] {1, 2, 1,}.Zip(list, (expected, actual) => new {expected, actual})
                                .All(x => x.expected == x.actual));
        }
    }

}