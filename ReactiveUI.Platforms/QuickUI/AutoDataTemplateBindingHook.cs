using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.QuickUI;
using Xamarin.QuickUI.Xaml;

namespace ReactiveUI.QuickUI
{
    /// <summary>
    /// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
    /// that don't have DataTemplates, and assigns a default DataTemplate that
    /// loads the View associated with each ViewModel.
    /// </summary>
    public class AutoDataTemplateBindingHook : IPropertyBindingHook
    {
        public static Lazy<DataTemplate> DefaultItemTemplate = new Lazy<DataTemplate>(() => {
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:xaml='clr-namespace:ReactiveUI.Xaml;assembly=__ASSEMBLYNAME__'> " +
                "<xaml:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
            "</DataTemplate>";

            var assemblyName = typeof(AutoDataTemplateBindingHook).AssemblyQualifiedName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));

            // XXX: Fix Me
            return null;
            //return (DataTemplate) XamlReader.Parse(template.Replace("__ASSEMBLYNAME__", assemblyName));
        });

        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            var viewProperties = getCurrentViewProperties();
            var lastViewProperty = viewProperties.LastOrDefault();
            if (lastViewProperty == null) return true;

            var itemsControl = lastViewProperty.Sender as ItemsView;
            if (itemsControl == null) return true;

            if (viewProperties.Last().PropertyName != "ItemsSource") return true;

            if (itemsControl.ItemTemplate != null) return true;

            itemsControl.ItemTemplate = DefaultItemTemplate.Value;
            return true;
        }
    }
}
