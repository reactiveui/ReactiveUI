using System;
using System.Dynamic;

namespace NLog
{
    public class Logger : DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this;
            return true;
        }
    }

    public static class LogManager
    {
        public static dynamic GetLogger(string dontcare)
        {
            return new Logger();
        }
    }
}

namespace NLog.Config
{

}

namespace NLog.Targets
{
}