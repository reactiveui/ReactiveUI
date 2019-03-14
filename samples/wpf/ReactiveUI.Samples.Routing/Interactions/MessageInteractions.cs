using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Samples.Routing.Interactions
{
    public static class MessageInteractions
    {
        public static Interaction<string, Unit> ShowMessage { get; } = new Interaction<string, Unit>();
    }
}
