using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI.Testing;
using Xunit;
using Microsoft.Reactive.Testing;

#if !MONO
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests
{
    public class TestWhenAnyObsViewModel : ReactiveObject
    {
        public ReactiveCommand<int, int> Command1 { get; set; }
        public ReactiveCommand<int, int> Command2 { get; set; }

        ReactiveList<int> myListOfInts;
        public ReactiveList<int> MyListOfInts {
            get { return myListOfInts; }
            set { this.RaiseAndSetIfChanged(ref myListOfInts, value); }
        }

        public TestWhenAnyObsViewModel()
        {
            Command1 = ReactiveCommand.CreateFromObservable<int, int>(val => Observable.Return(val));
            Command2 = ReactiveCommand.CreateFromObservable<int, int>(val => Observable.Return(val));
        }
    }

    public class HostTestFixture : ReactiveObject
    {
        public TestFixture _Child;
        public TestFixture Child {
            get { return _Child; }
            set { this.RaiseAndSetIfChanged(ref _Child, value); }
        }

        public int _SomeOtherParam;
        public int SomeOtherParam {
            get { return _SomeOtherParam; }
            set { this.RaiseAndSetIfChanged(ref _SomeOtherParam, value); }
        }

        public NonObservableTestFixture _PocoChild;
        public NonObservableTestFixture PocoChild {
            get { return _PocoChild; }
            set { this.RaiseAndSetIfChanged(ref _PocoChild, value); }
        }
    }

    public class NonObservableTestFixture
    {
        public TestFixture Child {get; set;}
    }

    public class NonReactiveINPCObject : INotifyPropertyChanged
    {
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

    public class ObjChain1 : ReactiveObject
    {
        public ObjChain2 _Model = new ObjChain2();
        public ObjChain2 Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
        }
    }

    public class ObjChain2 : ReactiveObject
    {
        public ObjChain3 _Model = new ObjChain3();
        public ObjChain3 Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
        }
    }

    public class ObjChain3 : ReactiveObject
    {
        public HostTestFixture _Model = new HostTestFixture();
        public HostTestFixture Model {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
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
                Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
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
                Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
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
                Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
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
                Assert.True(changes.All(x => x.GetPropertyName() == "Child.IsOnlyOneWord"));
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
            obj.WhenAnyValue(x => x.SomeOtherParam)
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
                {x => x.Child.Changed, new[] {typeof(TestFixture), typeof(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>)}},
            };

            var results = data.Keys.Select(x => new { input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() }).ToArray();
            var resultTypes = dataTypes.Keys.Select(x => new {input = x, output = Reflection.Rewrite(x.Body).GetExpressionChain() }).ToArray();

            foreach(var x in results) {
                data[x.input].AssertAreEqual(x.output.Select(y => y.GetMemberInfo().Name));
            }
            foreach (var x in resultTypes) {
                dataTypes[x.input].AssertAreEqual(x.output.Select(y => y.Type));
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
            var output2 = new List<string>();
            fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output2.Add);
            var output3 = new List<IObservedChange<TestFixture, int?>>();
            fixture.WhenAny(x => x.NullableInt, x => x).Subscribe(output3.Add);

            var output4 = new List<int?>();
            fixture.WhenAnyValue(x => x.NullableInt).Subscribe(output4.Add);

            Assert.Equal(1, output.Count);
            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("PocoProperty", output[0].GetPropertyName());
            Assert.Equal("Bamf", output[0].Value);

            Assert.Equal(1, output2.Count);
            Assert.Equal("Bamf", output2[0]);

            Assert.Equal(1, output3.Count);
            Assert.Equal(fixture, output3[0].Sender);
            Assert.Equal("NullableInt", output3[0].GetPropertyName());
            Assert.Equal(null, output3[0].Value);

            Assert.Equal(1, output4.Count);
            Assert.Equal(null, output4[0]);
        }

        [Fact]
        public void WhenAnyValueSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new HostTestFixture() {Child = new TestFixture()};
                fixture.SomeOtherParam = 5;
                fixture.Child.IsNotNullString = "Foo";

                var output1 = new List<int>();
                var output2 = new List<string>();
                fixture.WhenAnyValue(x => x.SomeOtherParam, x => x.Child.IsNotNullString, (sop, nns) => new {sop, nns}).Subscribe(x => {
                    output1.Add(x.sop); output2.Add(x.nns);
                });

                sched.Start();
                Assert.Equal(1, output1.Count);
                Assert.Equal(1, output2.Count);
                Assert.Equal(5, output1[0]);
                Assert.Equal("Foo", output2[0]);

                fixture.SomeOtherParam = 10;
                sched.Start();
                Assert.Equal(2, output1.Count);
                Assert.Equal(2, output2.Count);
                Assert.Equal(10, output1[1]);
                Assert.Equal("Foo", output2[1]);

                fixture.Child.IsNotNullString = "Bar";
                sched.Start();
                Assert.Equal(3, output1.Count);
                Assert.Equal(3, output2.Count);
                Assert.Equal(10, output1[2]);
                Assert.Equal("Bar", output2[2]);
            });
        }

        [Fact]
        public void WhenAnyValueShouldWorkEvenWithNormalProperties()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

            var output1 = new List<string>();
            var output2 = new List<int>();
            fixture.WhenAnyValue(x => x.PocoProperty).Subscribe(output1.Add);
            fixture.WhenAnyValue(x => x.IsOnlyOneWord, x => x.Length).Subscribe(output2.Add);

            Assert.Equal(1, output1.Count);
            Assert.Equal("Bamf", output1[0]);
            Assert.Equal(1, output2.Count);
            Assert.Equal(3, output2[0]);
        }

        [Fact]
        public void WhenAnyShouldRunInContext()
        {
            var tid = Thread.CurrentThread.ManagedThreadId;

            (TaskPoolScheduler.Default).With(sched => {
                int whenAnyTid = 0;
                var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz", PocoProperty = "Bamf" };

                fixture.WhenAnyValue(x => x.IsNotNullString).Subscribe(x => {
                    whenAnyTid = Thread.CurrentThread.ManagedThreadId;
                });

                int timeout = 10;
                fixture.IsNotNullString = "Bar";
                while (--timeout > 0 && whenAnyTid == 0) Thread.Sleep(250);

                Assert.Equal(tid, whenAnyTid);
            });
        }

        [Fact]
        public void OFPNamedPropertyTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var changes = fixture.ObservableForProperty<TestFixture, string>(x => x.IsOnlyOneWord).CreateCollection();

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
                Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Baz" });
            });
        }

        [Fact]
        public void OFPNamedPropertyTestNoSkipInitial()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture() { IsOnlyOneWord = "Pre" };
                var changes = fixture.ObservableForProperty<TestFixture, string>(x => x.IsOnlyOneWord, skipInitial: false).CreateCollection();

                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(2, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] { "Pre", "Foo" });
            });
        }

        [Fact]
        public void OFPNamedPropertyTestBeforeChange()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture() { IsOnlyOneWord = "Pre" };
                var changes = fixture.ObservableForProperty<TestFixture, string>(x => x.IsOnlyOneWord, beforeChange: true).CreateCollection();

                sched.Start();
                Assert.Equal(0, changes.Count);

                fixture.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] { "Pre", "Foo" });
            });
        }

        [Fact]
        public void OFPNamedPropertyTestRepeats()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var changes = fixture.ObservableForProperty<TestFixture, string>(x => x.IsOnlyOneWord).CreateCollection();

                fixture.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(1, changes.Count);

                fixture.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.IsOnlyOneWord = "Bar";
                sched.Start();
                Assert.Equal(2, changes.Count);

                fixture.IsOnlyOneWord = "Foo";
                sched.Start();
                Assert.Equal(3, changes.Count);

                Assert.True(changes.All(x => x.Sender == fixture));
                Assert.True(changes.All(x => x.GetPropertyName() == "IsOnlyOneWord"));
                changes.Select(x => x.Value).AssertAreEqual(new[] { "Foo", "Bar", "Foo" });
            });
        }
    }

    public class WhenAnyObservableTests
    {
        [Fact]
        public async Task WhenAnyObservableSmokeTest()
        {
            var fixture = new TestWhenAnyObsViewModel();

            var list = new List<int>();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command2)
                   .Subscribe(list.Add);

            Assert.Equal(0, list.Count);

            await fixture.Command1.Execute(1);
            Assert.Equal(1, list.Count);

            await fixture.Command2.Execute(2);
            Assert.Equal(2, list.Count);

            await fixture.Command1.Execute(1);
            Assert.Equal(3, list.Count);

            Assert.True(
                new[] {1, 2, 1,}.Zip(list, (expected, actual) => new {expected, actual})
                                .All(x => x.expected == x.actual));
        }

        [Fact]
        public void WhenAnyWithNullObjectShouldUpdateWhenObjectIsntNullAnymore()
        {
            var fixture = new TestWhenAnyObsViewModel();
            var output = fixture.WhenAnyObservable(x => x.MyListOfInts.CountChanged).CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.MyListOfInts = new ReactiveList<int>();
            Assert.Equal(0, output.Count);

            fixture.MyListOfInts.Add(1);
            Assert.Equal(1, output.Count);

            fixture.MyListOfInts = null;
            Assert.Equal(1, output.Count);
        }

        [Fact]
        public void NullObservablesDoNotCauseExceptions()
        {
            var fixture = new TestWhenAnyObsViewModel();
            fixture.Command1 = null;

            fixture.WhenAnyObservable(x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
            fixture.WhenAnyObservable(x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1, x => x.Command1).Subscribe();
        }
    }

#if !MONO
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

    public class WhenAnyThroughDependencyObjectTests
    {
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

            fixture.WhenAnyValue(x => x.ViewModel.Child.IsNotNullString).Subscribe(output.Add);

            fixture.ViewModel = vm;
            Assert.Equal(1, output.Count);

            fixture.ViewModel.Child.IsNotNullString = "Bar";
            Assert.Equal(2, output.Count);
            new[] { "Foo", "Bar" }.AssertAreEqual(output);
        }
    }
#endif
}
