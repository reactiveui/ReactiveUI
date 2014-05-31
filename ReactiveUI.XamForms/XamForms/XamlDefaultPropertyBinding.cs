using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public class XamlDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
            var items = new[] {
                new { Type = typeof(Slider), Property = "Value" },
                new { Type = typeof(InputView), Property = "Text" },
                new { Type = typeof(TextCell), Property = "Text" },
                new { Type = typeof(Label), Property = "Text" },
                new { Type = typeof(ProgressBar), Property = "Value" },
                new { Type = typeof(Image), Property = "Source" },
            };

            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }
    }
}