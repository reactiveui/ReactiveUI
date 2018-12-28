using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Winforms
{
    [SuppressMessage("Design", "CA1812: Instance not created", Justification = "Created by static method in other assembly.")]
    internal class PropertyChangedEventManager : WeakEventManager<INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
    {
    }
}
