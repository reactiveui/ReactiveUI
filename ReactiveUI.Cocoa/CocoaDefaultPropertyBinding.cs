using System;
using System.Linq;
using MonoMac.AppKit;

namespace ReactiveUI.Cocoa
{
    public class CocoaDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
            var items = new[] {
                new { Type = typeof(NSSlider), Property = "DoubleValue" },
                new { Type = typeof(NSTextView), Property = "Value" },
                new { Type = typeof(NSTextField), Property = "StringValue" },
                new { Type = typeof(NSLevelIndicator), Property = "DoubleValue" },
                new { Type = typeof(NSProgressIndicator), Property = "DoubleValue" },
                new { Type = typeof(NSButton), Property = "Title" },
                new { Type = typeof(NSImageView), Property = "Image" },
            };

            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.IsAssignableFrom(type));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }

    }
}
