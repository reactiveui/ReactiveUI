using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ReactiveUI;
using ReactiveUI.Testing;
using ReactiveUI.Sample.Models;
using ReactiveUI.Sample.ViewModels;

namespace ReactiveUI.Sample.Tests
{
    /* COOLSTUFF: Time-travel Testing
     *
     * If we write all of our interactions and ViewModel behaviors in terms of
     * Observables, we unlock one of Rx's superpowers: TestScheduler.
     * TestScheduler lets us simulate the passing of time without actually
     * waiting around for it. 
     *
     * The Pomodoro app was chosen as a sample because one of the aspects of it
     * is that a traditional MVVM implementation is a nightmare to test - it's
     * all Timers and state changes; testing timers is normally slow and
     * unreliable - we don't want to have every test run take the full 30
     * minutes!
     *
     * We take advantage of the fact that we control the way that deferred
     * operations are run via RxApp.DeferredScheduler, to be able to create a
     * test "schedule" - we do some things, fast forward some time in the
     * future, then see if what we expected to happen, happened.
     */

    [TestClass()]
    public class BlockTimerViewModelTest : IEnableLogger
    {
        [TestMethod]
        public void TimerShouldFinishAfterThirtyMinutes()
        {
            // NB: This is a pattern that we'll use often - we're creating a new
            // TestScheduler object, then calling the "With" extension method,
            // which overrides RxApp.DeferredScheduler / RxApp.TaskpoolScheduler
            // with the instance of TestScheduler here, then we run the Action
            // provided (sched => {}). 
            //
            // When the Action exits, we reset RxApp to the way it was before,
            // so we don't have to worry about tests stepping on each other's
            // feet
            
            (new TestScheduler()).With(sched => {

                //
                // First, we Arrange our Objects.
                //
                
                var lastState = BlockTimerViewState.Initialized;
                bool isTimerStateDone = false;
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                /* COOLSTUFF: Setting up Subscriptions
                 *
                 * One additional step of RxUI-based tests, is setting up our
                 * Subscriptions - i.e. what we want to watch change over time,
                 * just like we do in the Constructors of our ViewModels.
                 *
                 * We'll then advance the scheduler and see what happens.
                 */

                // Watch the timer state
                fixture.TimerState.Subscribe(
                    state => lastState = state,
                    () => isTimerStateDone = true);

                // Click the Start button
                fixture.Start.Execute(null);

                // Fast forward to 25 minutes in, the timer should *not* be done
                sched.AdvanceByMs(24 * 60 * 1000);
                Assert.IsFalse(isTimerStateDone);

                // Let's go to 31 minutes
                sched.AdvanceByMs(35 * 60 * 1000);

                // Make sure our model duration took 30 minutes(ish)
                var pomodoroLength = (fixture.Model.EndedAt.Value - fixture.Model.StartedAt.Value);
                pomodoroLength.TotalMinutes.AssertWithinEpsilonOf(30.0);
                Assert.IsTrue(isTimerStateDone);
            });
        }

        [TestMethod]
        public void MakeSureWeAreInBreakMode()
        {
            (new TestScheduler()).With(sched => {
                var lastState = BlockTimerViewState.Initialized;
                bool isTimerStateDone = false;
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                // Watch the timer state
                fixture.TimerState.Subscribe(
                    state => lastState = state,
                    () => isTimerStateDone = true);

                fixture.Start.Execute(null);

                // After 26 minutes, we should be in our 5-minute break
                sched.AdvanceByMs(26 * 60 * 1000);
                fixture.TimeRemaining.TotalMinutes.AssertWithinEpsilonOf(4.0);
                Assert.AreEqual(BlockTimerViewState.StartedInBreak, lastState);
                Assert.IsFalse(isTimerStateDone);
            });
        }

        [TestMethod]
        public void TheTimerDoesntAdvanceWhenItIsPaused()
        {
            (new TestScheduler()).With(sched => {
                var lastState = BlockTimerViewState.Initialized;
                bool isTimerStateDone = false;
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                // Watch the timer state
                fixture.TimerState.Subscribe(
                    state => lastState = state,
                    () => isTimerStateDone = true);

                fixture.Start.Execute(null);

                // Five minutes in, hit the pause button
                sched.AdvanceByMs(5 * 60 * 1000);
                var timeRemaining = fixture.TimeRemaining;

                fixture.Pause.Execute(null);

                // Fast forward ten more minutes - since we're paused, we 
                // TimeRemaining shouldn'tve moved
                sched.AdvanceByMs(10 * 60 * 1000);
                Assert.AreEqual((int)timeRemaining.TotalMinutes, (int)fixture.TimeRemaining.TotalMinutes);

                fixture.Start.Execute(null);

                // Make sure the TimeRemaining has only advanced 1 minute since 
                // we resumed (i.e. we shouldn't count paused time as working)
                sched.AdvanceByMs(11 * 60 * 1000);

                // We should have one pause, and it should be 5 minutes long
                Assert.AreEqual(1, fixture.Model.PauseList.Count);

                var deltaTime = (fixture.Model.PauseList[0].EndedAt - fixture.Model.PauseList[0].StartedAt).TotalMinutes;
                this.Log().Info("Pause Time: {0} mins", deltaTime);

                deltaTime.AssertWithinEpsilonOf(5.0);

                // The timer display should have advanced only one more minute 
                // (i.e. not six minutes, since we were paused for 5 of them)
                deltaTime = (timeRemaining - fixture.TimeRemaining).TotalMinutes;
                deltaTime.AssertWithinEpsilonOf(1.0);
            });
        }

        [TestMethod]
        public void ProgressBarValueIsAccurate()
        {
            (new TestScheduler()).With(sched => {
                double lastPercentage = -1.0;
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                fixture.WhenAny(x => x.ProgressPercentage, x => x.Value).Subscribe(x => lastPercentage = x);

                fixture.Start.Execute(null);

                // At the beginning we should be zero
                sched.AdvanceByMs(10);
                lastPercentage.AssertWithinEpsilonOf(0.0);

                // Run to exactly half of the work time 25 mins / 2
                sched.AdvanceByMs((12 * 60 + 30) * 1000);
                lastPercentage.AssertWithinEpsilonOf(0.5);

                // Run to a little before the end, should be near 1.0
                sched.AdvanceByMs(25 * 60 * 1000 - 10);
                lastPercentage.AssertWithinEpsilonOf(1.0);

                // Step to the beginning of the break, we should've moved back 
                // to zero
                sched.AdvanceByMs(25 * 60 * 1000 + 1010);
                lastPercentage.AssertWithinEpsilonOf(0.0);

                // Finally run to the end of the break
                sched.AdvanceByMs(30 * 60 * 1000 - 10);
                lastPercentage.AssertWithinEpsilonOf(1.0);
            });
        }

        [TestMethod]
        public void ProgressBarShouldntMoveDuringAPause()
        {
            (new TestScheduler()).With(sched => {
                double lastPercentage = -1.0;
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                fixture.WhenAny(x => x.ProgressPercentage, x => x.Value).Subscribe(x => lastPercentage = x);

                fixture.Start.Execute(null);

                // At the beginning we should be zero
                sched.AdvanceByMs(10);
                lastPercentage.AssertWithinEpsilonOf(0.0);

                // Run to exactly half of the work time 25 mins / 2
                sched.AdvanceByMs((12 * 60 + 30) * 1000);
                lastPercentage.AssertWithinEpsilonOf(0.5);

                // Simulate hitting the Pause button
                fixture.Pause.Execute(null);

                // Run to 20 minutes; the progress bar shouldn't have moved
                // since we were paused
                sched.AdvanceByMs(20 * 60 * 1000);
                lastPercentage.AssertWithinEpsilonOf(0.5);

                fixture.Start.Execute(null);

                // Move to 25 minutes; the progress bar should've moved 5
                // minutes worth (remember, since we were paused from 12min
                // to 20min
                sched.AdvanceByMs(25 * 60 * 1000);
                lastPercentage.AssertWithinEpsilonOf(0.5 + 0.2);
            });
        }

        [TestMethod]
        public void CancelButtonShouldntEndUpSettingModel()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new BlockTimerViewModel(new BlockItem() { Description = "Test Item" });

                BlockTimerViewState lastState = BlockTimerViewState.Initialized;
                fixture.TimerState.Subscribe(x => lastState = x);

                fixture.Start.Execute(null);

                sched.AdvanceByMs(10);
                Assert.AreEqual(BlockTimerViewState.Started, lastState);

                // Run to 10 minutes in and hit Cancel
                sched.AdvanceByMs(10 * 60 * 1000);
                fixture.Cancel.Execute(null);

                // Run way past the end
                sched.AdvanceByMs(60 * 60 * 1000);
                Assert.AreEqual(BlockTimerViewState.ShouldCancel, lastState);
                Assert.IsFalse(fixture.Model.IsObjectValid());
            });
        }
    }

    static class DoubleTestMixin
    {
        public static void AssertWithinEpsilonOf(this double lhs, double rhs, double epsilon = 0.1)
        {
            Assert.IsTrue(Math.Abs(lhs - rhs) <= epsilon, String.Format("Left: {0}, Right: {1}", lhs, rhs));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
