using System;
using ReactiveUI;
using ReactiveUI.Mobile;
using Splat;
using System.Runtime.Serialization;

namespace MobileSample_Android
{
    [DataContract]
    public class AppBootstrapper
    {
        [DataMember]
        public Guid SavedGuid { get; set; }

        public AppBootstrapper()
        {
            SavedGuid = Guid.NewGuid();
        }
    }
}

