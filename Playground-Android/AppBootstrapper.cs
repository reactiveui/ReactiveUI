using System;
using System.Runtime.Serialization;
using ReactiveUI;
using Splat;

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

