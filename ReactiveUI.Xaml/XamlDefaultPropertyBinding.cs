using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace ReactiveUI.Xaml
{
    public class XamlDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
            var items = new[] {
                new { Type = typeof(RichTextBox), Property = "Document" },
                new { Type = typeof(Slider), Property = "Value" },
                new { Type = typeof(Expander), Property = "IsExpanded" },
                new { Type = typeof(ToggleButton), Property = "IsChecked" },
                new { Type = typeof(TextBox), Property = "Text" },
                new { Type = typeof(TextBlock), Property = "Text" },
                new { Type = typeof(ProgressBar), Property = "Value" },
                new { Type = typeof(ItemsControl), Property = "ItemsSource" },
                new { Type = typeof(Image), Property = "Source" },
                new { Type = typeof(FrameworkContentElement), Property = "Content" },
                new { Type = typeof(FrameworkElement), Property = "Visibility" },
            };

            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.IsAssignableFrom(type));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }
    }
}