using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ReactiveUI;

namespace Playground_Wpa81
{
    using System.Reactive;

    [DataContract]
    public class MainPageViewModel : ReactiveObject
    {
        [IgnoreDataMember]
        public ReactiveCommand<Unit, Unit> DoIt { get; private set; }

        [IgnoreDataMember]
        public ReactiveCommand<Unit, int> ThreadedDoIt { get; private set; }

        [DataMember]
        public Guid SavedGuid { get; private set; }

        [IgnoreDataMember]
        readonly ObservableAsPropertyHelper<int> threadedResult;
        public int ThreadedResult {
            get { return threadedResult.Value; }
        }

        public MainPageViewModel()
        {
            DoIt = ReactiveCommand.Create(() => Unit.Default);

            ThreadedDoIt = ReactiveCommand.CreateFromTask<Unit, int>(async _ => {
                await Task.Delay(5000);
                return 42;
            });

            ThreadedDoIt.ToProperty(this, x => x.ThreadedResult, out threadedResult);

            SavedGuid = Guid.NewGuid();
        }
    }
}
