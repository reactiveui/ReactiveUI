using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
#if NETFX_CORE
            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
#else
            return null;
#endif
        }
    }
}
