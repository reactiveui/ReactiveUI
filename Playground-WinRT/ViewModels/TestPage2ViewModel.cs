﻿using System;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Xaml;
using Splat;

namespace MobileSample_WinRT.ViewModels
{
    [DataContract]
    public class TestPage2ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test2"; } }
        public IScreen HostScreen { get; private set; }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        public TestPage2ViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            RandomGuid = Guid.NewGuid();
        }
    }
}