using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Media;
using ReactiveUI.Sample.Models;
using ReactiveUI.Xaml;

namespace ReactiveUI.Sample.ViewModels
{
    public enum BlockTimerViewState {
        Initialized, Started, StartedInBreak, Paused, ShouldCancel,
    }

    /* COOLSTUFF: A complex interaction example using Rx
     *
     * Creating a Pomodoro timer is actually a fairly complicated task - despite
     * the fact that this is not *concurrent* (i.e. at the end of the day, we're
     * still only running on one thread), it is very asynchronous, and very
     * prone to corner-cases and tricky ordering problems, since every change
     * affects everything else in the UI.
     *
     * We want to display a timer that counts down from 25 minutes
     * (workDuration), then restarts to 5 minutes (breakDuration), then
     * finishes. Seems easy enough, but here's the part that makes it tricky:
     * when coworkers come in our office, we don't want to lose "credit" for our
     * Pomodoro, so we want to be able to "pause" the timer and resume it.
     *
     * To this end, we've created an explicit state machine, represented by the
     * TimerState Subject - we can push it around via OnNext, then bind the rest 
     * of interactions to this explicit state.
     */

    public class BlockTimerViewModel : ReactiveValidatedObject
    {
        //
        // Constants
        //

        static readonly TimeSpan workDuration = TimeSpan.FromMinutes(25);
        static readonly TimeSpan breakDuration = TimeSpan.FromMinutes(5);

        //
        // Model, Fields, and Properties
        //

        public BlockItem Model { get; protected set; }

        Subject<BlockTimerViewState> _TimerState = new Subject<BlockTimerViewState>();
        public IObservable<BlockTimerViewState> TimerState {
            get { return _TimerState; }
        }


        //
        // Commands for this ViewModel
        //

        public IReactiveCommand Start { get; protected set; }
        public IReactiveCommand Pause { get; protected set; }
        public IReactiveCommand Cancel { get; protected set; }


        /* COOLSTUFF: Output Properties
         *
         * Output Properties are a new concept in ReactiveUI - they are
         * properties that WPF/Silverlight views can bind to, which are filled
         * in by an Observable. Whenever the Observable which TimeRemaining is
         * connected to provides a new value via OnNext, the _TimeRemaining
         * ObservableAsPropertyHelper saves it in its Value field, then notifies
         * WPF/SL that the property has changed. So from WPF's perspective,
         * these are "read-only properties that change :)"
         *
         * We will see these properties being created later in the code via the
         * Observable.ToProperty() helper method. They are often really useful
         * for creating simple Value converters, like the TimeRemainingCaption
         * below.
         */

        ObservableAsPropertyHelper<TimeSpan> _TimeRemaining;
        public TimeSpan TimeRemaining {
            get { return _TimeRemaining.Value; }
        }

        ObservableAsPropertyHelper<string> _TimeRemainingCaption;
        public string TimeRemainingCaption {
            get { return _TimeRemainingCaption.Value; }
        }

        ObservableAsPropertyHelper<double> _ProgressPercentage;
        public double ProgressPercentage {
            get { return _ProgressPercentage.Value; }
        }

        ObservableAsPropertyHelper<Brush> _ProgressBackgroundBrush;
        public Brush ProgressBackgroundBrush {
            get { return _ProgressBackgroundBrush.Value; }
        }

        public BlockTimerViewModel(BlockItem Model)
        { 
            this.Model = Model;

            // Complete the TimerState when the break finishes
            // When the TimerState ends (i.e. the dialog is closed), set the
            // EndedAt time.
            //
            // Note that it's really important that whenever you use any
            // time-based Rx operator like Timer or Delay, that you use an
            // RxApp-based scheduler. If you don't do this (and it's really easy
            // to forget!), your unit tests won't work properly!
            var timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), RxApp.DeferredScheduler)
                .TakeWhile(_ => !isFinishedWithBreak())
                .Finally(() => {
                    this.Model.EndedAt = RxApp.DeferredScheduler.Now;
                    _TimerState.OnCompleted();
                });

            // COOLSTUFF: Defining a Command-based State Machine
            //
            // Here, we're creating our Commands - let's look a bit further into
            // how this actually works. Looking back to our problem definition,
            // we can see that many of these buttons' CanExecute() are tied to
            // each other (i.e. Hitting Pause means I can't hit it again until I
            // hit Start).
            //
            // Here, we're defining the conditions when each of these buttons
            // are allowed to be clicked. In the next block, we convert each of
            // the Execute() Observables (each command is an Observable who
            // fires whenever it is Executed), into the state that we *should*
            // move to once the button is hit. We then subscribe to it, and move
            // to the new state.
            
            // Define when we can click each of these commands
            Start = new ReactiveCommand(
                _TimerState.Select(x => x == BlockTimerViewState.Initialized || x == BlockTimerViewState.Paused));
            Pause = new ReactiveCommand(
                _TimerState.Select(x => x == BlockTimerViewState.Started || x == BlockTimerViewState.StartedInBreak));
            Cancel = new ReactiveCommand(
                _TimerState.Select(x => x != BlockTimerViewState.Initialized));

            // Move to the next state appropriately when the Start, Pause, and Cancel buttons are clicked
            Observable.Merge(
                Start.Select(_ => isInBreak() ? BlockTimerViewState.StartedInBreak : BlockTimerViewState.Started),
                Pause.Select(_ => BlockTimerViewState.Paused),
                Cancel.Select(_ => BlockTimerViewState.ShouldCancel)
            ).Subscribe(_TimerState.OnNext);

            // Set the StartedAt time on the Model when we click the Start
            // button. It's also important here to use RxApp's Now instead of
            // DateTime.Now, or else our tests will be very confused.
            Start.Subscribe(_ => this.Model.StartedAt = this.Model.StartedAt ?? RxApp.DeferredScheduler.Now);

            // Create an Observable who yields the current State whenever the
            // timer fires - it has the Rate of the Timer but the Value of the
            // TimerState.
            var timerAsState = timer.CombineLatest(_TimerState, (_, ts) => ts);

            // Take the timer and derive a version that only notifies us when
            // the TimerState has changed
            var distinctTimerAsState = timerAsState.DistinctUntilChanged();

            // Invalidate the object when they hit Cancel
            Cancel.Subscribe(x => {
                Model.StartedAt = null; Model.EndedAt = null;
            });

            // When someone hits the Start button, then Pause, then the Start 
            // button, we create a record of how long they pause.
            // 
            // This code is a bit tricky - the key is BufferWithCount; if we
            // have a sequence [1,2,3,4,5], BufferWithCount(3,1) will return:
            // [[1,2,3], [2,3,4], [3,4,5], ...]: advancing forward one at a
            // time, returning the last three items. This is important, since
            // we're looking for the pattern, "Start, Pause, Start"
            //
            // Also, we again see here, whenever we use a Time-based operator
            // like Timestamp, that we use an RxApp scheduler. This is a very
            // common yet annoying-to-debug mistake!
            distinctTimerAsState
                .Timestamp(RxApp.DeferredScheduler)
                .Buffer(3 /*items in buffer*/, 1 /*at a time*/)
                .Where(isStateSequenceAPause)
                .Subscribe(x => this.Model.AddRecordOfPause(new PauseRecord() {
                    StartedAt = x[1].Timestamp, 
                    EndedAt = x[2].Timestamp,
                }));

            // Move to the Break when the normal timer expires
            timerAsState
                .Where(ts => isInBreak() && ts == BlockTimerViewState.Started)
                .Subscribe(_ => _TimerState.OnNext(BlockTimerViewState.StartedInBreak));


            /* COOLSTUFF: Superpowered Value Converters
             *
             * Here we're binding the application state Observables we've
             * created above into real WPF properties that we can bind to (since
             * WPF can't bind directly to Observables). However, unlike
             * IValueConverters which are very simple, we can do much more
             * interesting things here like filtering.
             */

            //
            // Set up our output properties
            //

            // When the state is Started, move the timer display forward
            _TimeRemaining = timerAsState
                .Where(isTimerCurrentlyRunning)
                .Select(_ => currentTimeRemaining()).StartWith(currentTimeRemaining())
                .ToProperty(this, x => x.TimeRemaining);

            // Take the TimeSpan and convert it to the Caption (basically an 
            // IValueConverter). 
            //
            // The WhenAny() operator allows us to easily bind one or more
            // properties to another property, while still pulling in the
            // Initial value (whereas ObservableForProperty would only update
            // the property once it had changed at least once)
            _TimeRemainingCaption = this
                .WhenAny(x => x.TimeRemaining, x => String.Format("{0:mm}:{0:ss}", x.Value))
                .ToProperty(this, x => x.TimeRemainingCaption);

            _ProgressPercentage = timerAsState
                .Where(isTimerCurrentlyRunning)
                .Select(_ => progressBarPercentage())
                .ToProperty(this, x => x.ProgressPercentage);

            // Map states to background colors
            var colorLookupTable = new Dictionary<BlockTimerViewState, Brush> {
                {BlockTimerViewState.Initialized, null},
                {BlockTimerViewState.Paused, new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x66))},
                {BlockTimerViewState.Started, new SolidColorBrush(Color.FromRgb(0x99, 0xff, 0x66))},
                {BlockTimerViewState.StartedInBreak, new SolidColorBrush(Color.FromRgb(0x99, 0xff, 0xff))},
                {BlockTimerViewState.ShouldCancel, null},
            };

            // Every time the state changes on a Timer beat, update the
            // background color.
            _ProgressBackgroundBrush = distinctTimerAsState.Select(x => colorLookupTable[x])
                .ToProperty(this, x => x.ProgressBackgroundBrush);


            //
            // Everything's set up! Kick off everything by moving to the
            // Initialized state
            // 
            
            _TimerState.OnNext(BlockTimerViewState.Initialized);
        }


        //
        // Helper Methods
        // 


        /* COOLSTUFF: Writing small helper methods
         *
         * Thes methods may seem stupid when you look at the definitions below,
         * but when writing complex interactions, methods like these make a
         * serious difference in making the high-level code readable.
         *
         * One of the disadvantages of Rx and RxUI is that it is very easy to
         * write unreadable, very dense code. Methods like this can turn
         * terse-yet-unreadable code into extremely readable code that expresses
         * your intent to both the machine and to other programmers.
         */

        bool isInBreak()
        {
            return unpausedTimeDuration() > workDuration;
        }

        bool isFinishedWithBreak()
        {
            return unpausedTimeDuration() > (workDuration + breakDuration);
        }

        bool isTimerCurrentlyRunning(BlockTimerViewState state)
        {
            return (state == BlockTimerViewState.Started || 
                    state == BlockTimerViewState.StartedInBreak);
        }

        TimeSpan unpausedTimeDuration()
        {
            if (Model.StartedAt == null)
                return TimeSpan.Zero;
            return (RxApp.DeferredScheduler.Now - Model.StartedAt.Value) - Model.DurationOfPauses();
        }

        TimeSpan currentTimeRemaining()
        {
            if (isInBreak()) {
                return breakDuration - (unpausedTimeDuration() - workDuration);
            } else {
                return workDuration - unpausedTimeDuration();
            }
        }

        double progressBarPercentage()
        {
            var totalTime = (isInBreak() ? breakDuration : workDuration);
            return 1.0 - (currentTimeRemaining().TotalMilliseconds / totalTime.TotalMilliseconds);
        }

        bool isStateSequenceAPause(IList<Timestamped<BlockTimerViewState>> recentStates)
        {
            if (recentStates.Count != 3) {
                return false;
            }

            if (!isTimerCurrentlyRunning(recentStates[0].Value)) {
                return false;
            }

            if (recentStates[1].Value != BlockTimerViewState.Paused) {
                return false;
            }

            if (!isTimerCurrentlyRunning(recentStates[2].Value)) {
                return false;
            }

            return true;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
