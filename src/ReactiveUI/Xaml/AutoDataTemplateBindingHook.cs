using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
    /// that don't have DataTemplates, and assigns a default DataTemplate that
    /// loads the View associated with each ViewModel.
    /// </summary>
    public class AutoDataTemplateBindingHook : IPropertyBindingHook
    {
        public static Lazy<DataTemplate> DefaultItemTemplate = new Lazy<DataTemplate>(() => {
#if NETFX_CORE
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:xaml='using:ReactiveUI'>" +
                "<xaml:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
            "</DataTemplate>";
            var assemblyName = "";
#else
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:xaml='clr-namespace:ReactiveUI;assembly=__ASSEMBLYNAME__'> " +
                "<xaml:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
            "</DataTemplate>";
            var assemblyName = typeof(AutoDataTemplateBindingHook).Assembly.FullName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
#endif

#if SILVERLIGHT || NETFX_CORE
            return (DataTemplate) XamlReader.Load(
            #else
            return (DataTemplate) XamlReader.Parse(
            #endif

            template.Replace("__ASSEMBLYNAME__", assemblyName));
        });

        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            var viewProperties = getCurrentViewProperties();
            var lastViewProperty = viewProperties.LastOrDefault();
            if (lastViewProperty == null) return true;

            var itemsControl = lastViewProperty.Sender as ItemsControl;
            if (itemsControl == null) return true;

            if (!String.IsNullOrEmpty(itemsControl.DisplayMemberPath)) return true;

            if (viewProperties.Last().GetPropertyName() != "ItemsSource") return true;

            if (itemsControl.ItemTemplate != null) return true;

#if !SILVERLIGHT
            if (itemsControl.ItemTemplateSelector != null) return true;
#endif

            itemsControl.ItemTemplate = DefaultItemTemplate.Value;
            return true;
        }
    }
}
