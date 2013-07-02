using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Xaml
{
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
#if WINRT
            return Windows.UI.ViewManagement.ApplicationView.Value.ToString();
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
