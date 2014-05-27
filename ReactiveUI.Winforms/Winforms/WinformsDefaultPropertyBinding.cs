namespace ReactiveUI.Winforms
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    public class WinformsDefaultPropertyBinding : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            // NB: These are intentionally arranged in priority order from most
            // specific to least specific.
            var items = new[] {

                new { Type = typeof(RichTextBox), Property = "Text" },
                new { Type = typeof(Label), Property = "Text" },
                new { Type = typeof(Button), Property = "Text" },
                new { Type = typeof(CheckBox), Property = "Checked" },
                new { Type = typeof(TextBox), Property = "Text" },
                new { Type = typeof(ProgressBar), Property = "Value" }
            };
           
            var type = control.GetType();
            var kvp = items.FirstOrDefault(x => x.Type.IsAssignableFrom(type));

            return kvp != null ? Tuple.Create(kvp.Property, 5) : null;
        }
    }
}