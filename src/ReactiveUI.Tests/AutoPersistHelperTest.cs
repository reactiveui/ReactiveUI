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
            var fixture = new HostTestFixture();

            bool shouldDie = true;
            try {
                fixture.AutoPersist(x => Observables.Unit);
            } catch (Exception) {
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
                fixture.AutoPersist(x => { timesSaved++; return Observables.Unit; }, manualSave, TimeSpan.FromMilliseconds(100));

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
                fixture.AutoPersist(x => { timesSaved++; return Observables.Unit; }, manualSave, TimeSpan.FromMilliseconds(100));

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

        [Fact]
        public void AutoPersistHelperDisconnects()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();
                var manualSave = new Subject<Unit>();

                int timesSaved = 0;
                var disp = fixture.AutoPersist(x => { timesSaved++; return Observables.Unit; }, manualSave, TimeSpan.FromMilliseconds(100));

                // No changes = no saving
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // Change = one save
                fixture.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Two changes after dispose = no save
                disp.Dispose();
                fixture.IsNotNullString = "Foo";
                fixture.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Trigger save after dispose = no save
                manualSave.OnNext(Unit.Default);
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);
            });
        }
    }

    public class AutoPersistCollectionTests
    {
        [Fact]
        public void AutoPersistCollectionSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var manualSave = new Subject<Unit>();

                var item = new TestFixture();
                var fixture = new ReactiveList<TestFixture> { item };

                int timesSaved = 0;
                fixture.AutoPersistCollection(x => { timesSaved++; return Observables.Unit; }, manualSave, TimeSpan.FromMilliseconds(100));

                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // By being added to collection, AutoPersist is enabled for item
                item.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Removed from collection = no save
                fixture.Clear();
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Item isn't in the collection, it doesn't get persisted anymore
                item.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Added back item gets saved
                fixture.Add(item);
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Baz";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(2, timesSaved);

                // Even if we issue a reset
                fixture.Reset();
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Bamf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);

                // Remove by hand = no save
                fixture.RemoveAt(0);
                item.IsNotNullString = "Blomf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);
            });
        }

        [Fact]
        public void AutoPersistCollectionDisconnectsOnDispose()
        {
            (new TestScheduler()).With(sched => {
                var manualSave = new Subject<Unit>();

                var item = new TestFixture();
                var fixture = new ReactiveList<TestFixture> { item };

                int timesSaved = 0;
                var disp = fixture.AutoPersistCollection(x => { timesSaved++; return Observables.Unit; }, manualSave, TimeSpan.FromMilliseconds(100));

                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // By being added to collection, AutoPersist is enabled for item
                item.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Dispose = no save
                disp.Dispose();

                // Removed from collection = no save
                fixture.Clear();
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Item isn't in the collection, it doesn't get persisted anymore
                item.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Added back item + dispose = no save
                fixture.Add(item);
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Baz";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Even if we issue a reset, no save
                fixture.Reset();
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Bamf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Remove by hand = no save
                fixture.RemoveAt(0);
                item.IsNotNullString = "Blomf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);
            });
        }
    }
}