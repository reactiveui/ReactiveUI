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
#if WINRT80
            return Windows.Graphics.Display.DisplayProperties.CurrentOrientation.ToString();
#elif NETFX_CORE
            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString(); 
#elif SILVERLIGHT
            var app = System.Windows.Application.Current;
            if (app == null) return null;
            var frame = app.RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame;
            if (frame == null) return null;
            return frame.Orientation.ToString();
#else
            return null;
#endif
        }
    }
}
