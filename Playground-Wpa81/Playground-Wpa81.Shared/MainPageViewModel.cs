using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Playground_Wpa81
{
    [DataContract]
    public class MainPageViewModel : ReactiveObject
    {
        [IgnoreDataMember]
        public ReactiveCommand<Object> DoIt { get; protected set; }

        [IgnoreDataMember]
        public ReactiveCommand<int> ThreadedDoIt { get; protected set; }

        [DataMember]
        public Guid SavedGuid { get; set; }

        [IgnoreDataMember]
        ObservableAsPropertyHelper<int> threadedResult;
        public int ThreadedResult {
            get { return threadedResult.Value; }
        }

        public MainPageViewModel()
        {
            DoIt = ReactiveCommand.Create();

            ThreadedDoIt = ReactiveCommand.CreateAsyncTask(async _ => {
                await Task.Delay(5000);
                return 42;
            });

            ThreadedDoIt.ToProperty(this, x => x.ThreadedResult, out threadedResult);

            SavedGuid = Guid.NewGuid();
        }
    }
}
