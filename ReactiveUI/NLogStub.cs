using System;
using System.Dynamic;

namespace 
{

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