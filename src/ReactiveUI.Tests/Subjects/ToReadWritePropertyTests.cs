using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using ReactiveUI.Subjects;
using Xunit;

namespace ReactiveUI.Tests.Subjects
{
    public class ToReadWritePropertyTests
    {
        public class Model : ReactiveObject
        {
            /// <summary>
            /// The model length will be in units meters
            /// </summary>
            private double _LengthInMeters;
            public double LengthInMeters { get => _LengthInMeters; set => this.RaiseAndSetIfChanged( ref _LengthInMeters, value ); }

        }

        public class ViewModel : ReactiveObject
        {
            public enum UnitsEnum
            {
                Meters
              , Milliters
            };

            private readonly SubjectAsPropertyHelper<double> _Length;

            private UnitsEnum _Units;
            public  UnitsEnum Units { get => _Units; set => this.RaiseAndSetIfChanged( ref _Units, value ); }


            public double LengthInCurrentUnits { get => _Length.Value; set => _Length.Value = value; }

            public Model Model { get; }

            public ViewModel( Model model, IScheduler scheduler = null )
            {
                Model = model;

                var subject = model
                             .PropertySubject( p => p.LengthInMeters ) // Generate an ISubject<double>
                             .CombineLatest
                                  ( this.WhenAnyValue( p => p.Units ) // current value of units
                                  , UnitsToScaleConstraintFactory     // Function for generating the converter
                                  , Observer.Create<Option<Exception>>
                                        ( e =>
                                        {
                                        } ) // Error handler
                                  )
                    ;

                _Length = subject.ToReadWriteProperty( this, p => p.LengthInCurrentUnits, scheduler: scheduler );
            }

            /// <summary>
            /// Converts the units enum to a constraint that converts between
            /// the base unit meters and the selected unit.
            /// </summary>
            /// <param name="u"></param>
            /// <returns></returns>
            private static TwoWayConstraint<double, double> UnitsToScaleConstraintFactory( UnitsEnum u )
            {
                switch (u)
                {
                case UnitsEnum.Meters:
                    return Constraint.Multiply( 1.0 );
                case UnitsEnum.Milliters:
                    return Constraint.Multiply( 1000 );
                default:
                    throw new ArgumentOutOfRangeException( nameof(u), u, null );
                }
            }
        }

        [StaFact]
        public async Task ShouldWork()
        {
            var m  = new Model();
            var vm = new ViewModel(m);

            m.LengthInMeters = 10;
            Assert.Equal( 10, m.LengthInMeters );

            vm.Units = ViewModel.UnitsEnum.Meters;
            Assert.Equal( 10, vm.LengthInCurrentUnits );

            Assert.Equal( 10, vm.LengthInCurrentUnits );
            vm.LengthInCurrentUnits = 20;

            Assert.Equal( 20, m.LengthInMeters );

            vm.Units = ViewModel.UnitsEnum.Milliters;

            Assert.Equal( 20000, vm.LengthInCurrentUnits );
            Assert.Equal( 20,    m.LengthInMeters );

            m.LengthInMeters = 5;
            Assert.Equal( 5000, vm.LengthInCurrentUnits );

            vm.LengthInCurrentUnits = 6000;
            Assert.Equal( 6, m.LengthInMeters );
        }
    }
}
