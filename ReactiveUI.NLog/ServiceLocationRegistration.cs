using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace ReactiveUI.NLog
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
            RxApp.LoggerFactory = type => new NLogLogger(global::NLog.LogManager.GetLogger(type.Name));
        }
    }
}
