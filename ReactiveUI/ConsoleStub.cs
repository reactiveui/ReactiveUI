using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ReactiveUI
{
    internal static class Console
    {
        public static dynamic Error
        {
            get { return new Logger(); }
        }
    }
}
