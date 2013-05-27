using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using ReactiveUI;

namespace ReactiveUI.NLog
{
    class NLogLogManager : ILogManager
    {
        public IFullLogger GetLogger(Type type)
        {
            return new NLogLogger(global::NLog.LogManager.GetLogger(type.Name));
        }
    }

    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            var nlogManager = new NLogLogManager();
            registerFunction(() => nlogManager, typeof(ILogManager));
        }
    }
}
