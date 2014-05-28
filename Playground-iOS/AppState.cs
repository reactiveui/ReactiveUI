using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.Mobile;
using System.Runtime.Serialization;

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

