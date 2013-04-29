using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;
using System.Reactive.Subjects;

namespace ReactiveUI.Tests
{
    public class AutoPersistHelperTest
    {
        [Fact]
        public void AutoPersistDoesntWorkOnNonDataContractClasses()
        {
            var fixture = new FakeViewModel();

            bool shouldDie = true;
            try {
                fixture.AutoPersist(x => Observable.Return(Unit.Default));
            } catch (Exception ex) {
                shouldDie = false;
            }

            Assert.False(shouldDie);
        }

        [Fact]
        public void AutoPersistHelperShouldntTriggerOnNonPersistableProperties()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var manualSave = new Subject<Unit>();

                int timesSaved = 0;
                fixture.AutoPersist(x => { timesSaved++; return Observable.Return(Unit.Default); }, manualSave, TimeSpan.FromMilliseconds(100));

                // No changes = no saving
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // Change to not serialized = no saving
                fixture.NotSerialized = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);
            });
        }

        [Fact]
        public void AutoPersistHelperSavesOnInterval()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var manualSave = new Subject<Unit>();

                int timesSaved = 0;
                fixture.AutoPersist(x => { timesSaved++; return Observable.Return(Unit.Default); }, manualSave, TimeSpan.FromMilliseconds(100));

                // No changes = no saving
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // Change = one save
                fixture.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Two fast changes = one save
                fixture.IsNotNullString = "Foo";
                fixture.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(2, timesSaved);

                // Trigger save twice = one save
                manualSave.OnNext(Unit.Default);
                manualSave.OnNext(Unit.Default);
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);
            });
        }
    }
}