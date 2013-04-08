using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace ReactiveUI.NLog
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            RxApp.LoggerFactory = type => new NLogLogger(global::NLog.LogManager.GetLogger(type.Name));
        }
    }
}
