using System;
using System.Linq;

#if UIKIT
using MonoTouch.UIKit;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// Provides default property bindings for a number of common Cocoa controls.
    /// </summary>
    public class CocoaDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
#if UIKIT
            var items = new[] {
                new { Type = typeof(UISlider), Property = "Value" },
                new { Type = typeof(UITextView), Property = "Text" },
                new { Type = typeof(UITextField), Property = "Text" },
                new { Type = typeof(UIButton), Property = "Title" },
                new { Type = typeof(UIImageView), Property = "Image" },
            };
#else
            var items = new[] {
                new { Type = typeof(NSSlider), Property = "DoubleValue" },
                new { Type = typeof(NSTextView), Property = "Value" },
                new { Type = typeof(NSTextField), Property = "StringValue" },
                new { Type = typeof(NSLevelIndicator), Property = "DoubleValue" },
                new { Type = typeof(NSProgressIndicator), Property = "DoubleValue" },
                new { Type = typeof(NSButton), Property = "Title" },
                new { Type = typeof(NSMenuItem), Property = "Title" },
                new { Type = typeof(NSImageView), Property = "Image" },
            };
#endif

            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.IsAssignableFrom(type));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }
    }
}
