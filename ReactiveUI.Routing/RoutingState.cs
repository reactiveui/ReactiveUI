using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    [DataContract]
    public class RoutingState : ReactiveObject
    {
        [IgnoreDataMember] ReactiveCollection<IRoutableViewModel> _NavigationStack;

        [DataMember]
        public ReactiveCollection<IRoutableViewModel> NavigationStack {
            get { return _NavigationStack; }
            protected set { _NavigationStack = value; }
        }

        [IgnoreDataMember]
        public ReactiveCommand NavigateBack { get; protected set; }

        [IgnoreDataMember]
        public ReactiveCommand NavigateForward { get; protected set; }

        [IgnoreDataMember]
        public ReactiveCommand Navigate { get; protected set; }

        [IgnoreDataMember]
        public IObservable<IRoutableViewModel> CurrentViewModel { get; protected set; }

        [DataMember]
        string _AutoSaveContract;

        public RoutingState() : this(null)
        {
        }

        public RoutingState(string autoSaveContract)
        {
            _NavigationStack = new ReactiveCollection<IRoutableViewModel>();
            _AutoSaveContract = autoSaveContract;
            setupRx();
        }

        [OnDeserialized]
        void setupRx(StreamingContext sc) { setupRx();  }
        void setupRx()
        {
            NavigateBack = new ReactiveCommand(
                NavigationStack.CollectionCountChanged.StartWith(_NavigationStack.Count).Select(x => x > 0));
            NavigateBack.Subscribe(_ =>
                NavigationStack.RemoveAt(NavigationStack.Count - 1));

            NavigateForward = new ReactiveCommand();
            NavigateForward.Subscribe(x => 
                NavigationStack.Insert(NavigationStack.Count, (IRoutableViewModel)x));

            Navigate = new ReactiveCommand();
            Navigate.Subscribe(x => {
                NavigationStack.Clear();
                NavigationStack.Add((IRoutableViewModel) x);
            });

            /*
            if (_AutoSaveContract != null) {
                Observable.Merge(Navigate, NavigateForward, NavigateBack)
                    .Subscribe(_ => RxStorage.Engine.CreateSyncPoint(this, _AutoSaveContract));
            }
            */

            CurrentViewModel = NavigationStack.CollectionCountChanged
                .Select(_ => NavigationStack.LastOrDefault())
                .Multicast(new ReplaySubject<IRoutableViewModel>(1)).PermaRef();
        }

        public T FindViewModelInStack<T>()
            where T : IRoutableViewModel
        {
            return NavigationStack.Reverse().OfType<T>().FirstOrDefault();
        }
    }
}