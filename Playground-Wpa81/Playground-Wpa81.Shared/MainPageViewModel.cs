using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ReactiveUI;

namespace Playground_Wpa81
{
    [DataContract]
    public class MainPageViewModel : ReactiveObject
    {
        [IgnoreDataMember]
        public ReactiveCommand<Object> DoIt { get; protected set; }

        [DataMember]
        public Guid SavedGuid { get; set; }

        public MainPageViewModel()
        {
            DoIt = ReactiveCommand.Create();
            SavedGuid = Guid.NewGuid();
        }
    }
}
