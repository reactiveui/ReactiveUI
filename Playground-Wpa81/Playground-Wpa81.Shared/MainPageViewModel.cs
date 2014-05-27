using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Playground_Wpa81
{
    public class MainPageViewModel : ReactiveObject
    {
        public ReactiveCommand<Object> DoIt { get; protected set; }

        public MainPageViewModel()
        {
            DoIt = ReactiveCommand.Create();
        }
    }
}
