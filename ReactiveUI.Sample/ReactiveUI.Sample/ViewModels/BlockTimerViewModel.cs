using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using ReactiveUI.Sample.Models;
using ReactiveUI.Xaml;

namespace ReactiveUI.Sample.ViewModels
{
    public enum BlockTimerViewState {
        Initialized, Started, StartedInBreak, Paused, ShouldCancel,
    }

    public class BlockTimerViewModel : ReactiveValidatedObject
    {
#if DEBUG && FALSE
        static readonly TimeSpan workDuration = TimeSpan.FromMinutes(2);
        static readonly TimeSpan breakDuration = TimeSpan.FromMinutes(1);
#else
        static readonly TimeSpan workDuration = TimeSpan.FromMinutes(25);
        static readonly TimeSpan breakDuration = TimeSpan.FromMinutes(5);
#endif

        public BlockTimerViewModel(BlockItem Model)
        { 
            this.Model = Model;

            // Complete the TimerState when the break finishes
            // When the TimerState ends (i.e. the dialog is closed), set the EndedAt time
            var timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), RxApp.DeferredScheduler)
                .TakeWhile(_ => !isFinishedWithBreak())
                .Finally(() => {
                    if (this.Model != null) {
                        this.Model.EndedAt = RxApp.DeferredScheduler.Now;
                    }
                    _TimerState.OnCompleted();
                });

            // Set up our commands based on when the user is allowed to click them
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

            // Set the Start time when we click the Start button
            Start.Subscribe(_ => this.Model.StartedAt = this.Model.StartedAt ?? RxApp.DeferredScheduler.Now);

            var timerAsState = timer.CombineLatest(_TimerState, (_, ts) => ts);
            var distinctTimerAsState = timerAsState.DistinctUntilChanged();

            // When someone hits the Start button, then Pause, then the Start 
            // button, we create a record of how long they pause.
            distinctTimerAsState
                .Timestamp(RxApp.DeferredScheduler)
                .BufferWithCount(3, 1)
                .Where(isStateSequenceAPause)
                .Subscribe(x => this.Model.AddRecordOfPause(new PauseRecord() {StartedAt = x[1].Timestamp, EndedAt = x[2].Timestamp}));

            // Move to the Break when the normal timer expires
            timerAsState
                .Where(ts => isInBreak() && ts == BlockTimerViewState.Started)
                .Subscribe(_ => _TimerState.OnNext(BlockTimerViewState.StartedInBreak));

            // Trigger UserPressedCancel when the user hits the button
            Cancel.Subscribe(_ => UserPressedCancel = true);


            //
            // Set up our output properties
            //

            // When the state is Started, move the timer display forward
            _TimeRemaining = timerAsState
                .Where(ts => ts == BlockTimerViewState.Started || ts == BlockTimerViewState.StartedInBreak)
                .Select(_ => currentTimeRemaining()).StartWith(currentTimeRemaining())
                .ToProperty(this, x => x.TimeRemaining);

            // Take the TimeSpan and convert it to the Caption (basically an 
            // IValueConverter)
            _TimeRemainingCaption = this
                .WhenAny(x => x.TimeRemaining, x => String.Format("{0:mm}:{0:ss}", x.Value))
                .ToProperty(this, x => x.TimeRemainingCaption);

            _ProgressPercentage = timerAsState
                .Where(x => x == BlockTimerViewState.Started || x == BlockTimerViewState.StartedInBreak)
                .Select(_ => progressBarPercentage())
                .ToProperty(this, x => x.ProgressPercentage);

            var colorLookupTable = new Dictionary<BlockTimerViewState, Brush> {
                {BlockTimerViewState.Initialized, null},
                {BlockTimerViewState.Paused, new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x66))},
                {BlockTimerViewState.Started, new SolidColorBrush(Color.FromRgb(0x99, 0xff, 0x66))},
                {BlockTimerViewState.StartedInBreak, new SolidColorBrush(Color.FromRgb(0x99, 0xff, 0xff))},
                {BlockTimerViewState.ShouldCancel, null},
            };

            _ProgressBackgroundBrush = distinctTimerAsState.Select(x => colorLookupTable[x])
                .ToProperty(this, x => x.ProgressBackgroundBrush);

            // Kick off everything by moving to the Initialized state
            _TimerState.OnNext(BlockTimerViewState.Initialized);
        }

        public BlockItem Model { get; protected set; }

        Subject<BlockTimerViewState> _TimerState = new Subject<BlockTimerViewState>();
        public IObservable<BlockTimerViewState> TimerState {
            get { return _TimerState; }
        }

        bool _UserPressedCancel;
        public bool UserPressedCancel {
            get { return _UserPressedCancel; }
            set { this.RaiseAndSetIfChanged(x => x.UserPressedCancel, value); }
        }

        public IReactiveCommand Start { get; protected set; }
        public IReactiveCommand Pause { get; protected set; }
        public IReactiveCommand Cancel { get; protected set; }

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


        //
        // Helper Functions
        // 

        bool isInBreak()
        {
            return unpausedTimeDuration() > workDuration;
        }

        bool isFinishedWithBreak()
        {
            return unpausedTimeDuration() > (workDuration + breakDuration);
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

            if (recentStates[0].Value != BlockTimerViewState.Started && 
                recentStates[0].Value != BlockTimerViewState.StartedInBreak) {
                return false;
            }

            if (recentStates[1].Value != BlockTimerViewState.Paused) {
                return false;
            }

            if (recentStates[0].Value != BlockTimerViewState.Started && 
                recentStates[0].Value != BlockTimerViewState.StartedInBreak) {
                return false;
            }

            return true;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :