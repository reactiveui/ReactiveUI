using System;
using System.Linq;
using ReactiveUI;
using Android.Widget;

namespace ReactiveUI.Android
{
    /// <summary>
    /// Default property bindings for common Android widgets
    /// </summary>
    public class AndroidDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
            var items = new[] {
                new { Type = typeof(TextView), Property = "Text" },
                new { Type = typeof(ProgressBar), Property = "Progress" },
                new { Type = typeof(CompoundButton), Property = "Checked" },
            };

            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.IsAssignableFrom(type));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }
    }
}