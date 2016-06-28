using System;
using System.Drawing;
using System.Runtime.Serialization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace PlaygroundiOS
{
    [DataContract]
    public class AppState : ReactiveObject
	{
        [DataMember]
        public Guid SavedGuid { get; set; }

        public AppState()
        {
            SavedGuid = Guid.NewGuid();
        }
	}
}

