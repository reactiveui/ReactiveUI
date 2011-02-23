using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Sample.Models;

namespace ReactiveUI.Sample.ViewModels
{
    public class MainWindowViewModel : ReactiveValidatedObject
    {
        public AppModel Model { get; protected set; }
    }
}
