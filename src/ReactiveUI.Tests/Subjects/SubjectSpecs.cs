using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ReactiveUI.Subjects;
using Xunit;
using static ReactiveUI.Subjects.Option;

namespace ReactiveUI.Tests.Subjects
{

    public class TwoWayBindToTests
    {
        class ViewModel : ReactiveObject
        {
            int _Value0;
            public int Value0
            {
                get => _Value0;
                set => this.RaiseAndSetIfChanged(ref _Value0,value);
            }

            int _Value1;
            public int Value1
            {
                get => _Value1;
                set => this.RaiseAndSetIfChanged(ref _Value1,value);
            }

            string _StringValue;
            public string StringValue
            {
                get => _StringValue;
                set => this.RaiseAndSetIfChanged(ref _StringValue,value);
            }
        }

        [Fact]
        public async Task SubjectShouldInterfaceToProperty()
        {


            var b = new BehaviorSubject<int>(10);
            var p = new ViewModel { Value0 = 3 };

            var d = b.TwoWayBindTo(p, x=>x.Value0, scheduler: CurrentThreadScheduler.Instance);

            Assert.Equal(10, p.Value0);

            b.OnNext(20);
            Assert.Equal(20, p.Value0);


            p.Value0 = 30;

            var firstAsync = b.FirstAsync();

            Assert.Equal(30, (await firstAsync.ToTask()));

            p.Value0 = 40;

            Assert.Equal(40, (await firstAsync.ToTask()));

            // Unsubscribe
            d.Dispose();

            b.OnNext(50);

            Assert.Equal(40, p.Value0);
        }

        [Fact]
        public async Task ErrorsShouldBeForwarded()
        {
            // The source of our data ( could be async or whatever )
            var b = new BehaviorSubject<int>(10);

            // Our ViewModel with our formatted value
            var p = new ViewModel { StringValue = "3" };

            // Our Error collector. Checking 
            // catchWith.First().IsSome tells us if
            // we are in an error state.
            var catchWith = new BehaviorSubject<Option<Exception>>(None);

            // Print the errors to console. Note if there is no 
            // error the callback get Maybe.None so you can
            // clear the error state in the UI.
            // catchWith.Subscribe(e => Console.WriteLine(e));

            // Build the conversion pipeline. Validate and
            // Convert are just specializations of a bidirectional
            // LINQ(ish) pipeline for ISubject. Each method
            // will always return ISubject but converted or
            // validated.
            var d = b
                .Convert<int,string>(catchWith)
                .TwoWayBindTo(p, x=>x.StringValue, scheduler: CurrentThreadScheduler.Instance);

            // Convert schedules to the main thread so we put in a
            // delay here 
            await Task.Delay(TimeSpan.FromMilliseconds(200));

            // Check we have the initial value of the subject
            Assert.Equal("10", p.StringValue);

            // An invalid value due to parsing
            p.StringValue = "xx";
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            Assert.Equal("xx", p.StringValue);
            Assert.Equal(10, (await b.FirstAsync().ToTask()));
            Assert.True((await catchWith.FirstAsync().ToTask()).IsSome);

            // A Correct value
            p.StringValue = "5";
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            Assert.Equal(5, (await b.FirstAsync().ToTask()));
            Assert.False( (await catchWith.FirstAsync().ToTask()).IsSome );

        }

        [Fact]
        public async void ConversionShouldWork()
        {
            var b = new BehaviorSubject<int>(10);
            var p = new ViewModel { StringValue = "3" };
            var catchWith = new BehaviorSubject<Option<Exception>>(None);

            var d = b
                .Convert<int,string>(catchWith)
                .TwoWayBindTo(p, x=>x.StringValue, scheduler: CurrentThreadScheduler.Instance);

            Assert.Equal("10", p.StringValue);

            b.OnNext(20);
            Assert.Equal("20", p.StringValue);


            p.StringValue = "30";
            Assert.False((await catchWith.FirstAsync().ToTask()).IsSome);

            Assert.Equal(30, (await b.FirstAsync().ToTask()));

            p.StringValue = "40";
            Assert.False((await catchWith.FirstAsync().ToTask()).IsSome);

            Assert.Equal(40, (await b.FirstAsync().ToTask()));

            // Unsubscribe
            d.Dispose();

            b.OnNext(50);

            Assert.Equal("40", p.StringValue);


        }

        [Fact]
        public void ValidationShouldPreventErrorsFromRightHandSideMovingToLeft()
        {
            var v = new ViewModel();
            v
                .PropertySubject(p=>p.Value0)
                .TwoWayBindTo(v, p=>p.Value1, t=> v.Value1!=7, scheduler: CurrentThreadScheduler.Instance);

            v.Value0 = 1;
            Assert.Equal(1, v.Value1);
            v.Value1 = 77;
            Assert.Equal(77, v.Value0);

            // The validator prevents the value going from right to left
            v.Value1 = 7;
            Assert.Equal(77, v.Value0);

            // The validator does not prevent the value going from left to right
            v.Value1 = 12;
            v.Value0 = 7;
            v.Value1 = 7;

            
        }


        [Fact]
        public async void SubjectObservableConstraintCombinatorsShouldWork()
        {
            const double mtoinches = 39.3701;
            const double mtoyard = 1.09361;

            var errors = Observer.Create<Option<Exception>>(e => { });

            // Imagine this to be a user preference somewhere. We
            // could grab this with an IOC discovery or whatever.
            var scale = new BehaviorSubject<double>(mtoinches);

            // This is our model. We always work in units of meters 
            // of course.
            var model = new BehaviorSubject<double>(1); // in meters

            // This is our view model. It should map to the model via
            // the user preference ``scale``.
            var view = model.CombineLatest(scale, Converter.Multiply, errors);

            // Check that the view is correct initially
            Assert.Equal(mtoinches, (await view.FirstAsync().ToTask()));
            view.OnNext(mtoinches * 3.0);
            Assert.Equal(3.0, (await model.FirstAsync().ToTask()));

            // Change the user preference to yards
            model.OnNext(1.0);
            scale.OnNext(mtoyard);

            // Check the view is now in yards
            Assert.Equal(mtoyard, (await view.FirstAsync().ToTask()));
            view.OnNext(mtoyard * 5.0);
            Assert.Equal(5.0, (await model.FirstAsync().ToTask()));
        }

    }
}
