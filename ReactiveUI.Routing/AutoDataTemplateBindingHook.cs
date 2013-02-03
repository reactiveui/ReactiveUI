using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#endif

namespace ReactiveUI.Routing
{
    public class AutoDataTemplateBindingHook : IPropertyBindingHook
    {
        public static Lazy<DataTemplate> DefaultItemTemplate = new Lazy<DataTemplate>(() => {
#if WINRT
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:routing='using:ReactiveUI.Routing'>" +
                "<routing:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
            "</DataTemplate>";
            var assemblyName = "";
#else
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:routing='clr-namespace:ReactiveUI.Routing;assembly=__ASSEMBLYNAME__'> " +
                "<routing:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
            "</DataTemplate>";
            var assemblyName = typeof(AutoDataTemplateBindingHook).Assembly.FullName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
#endif

            #if SILVERLIGHT || WINRT
            return (DataTemplate) XamlReader.Load(
            #else
            return (DataTemplate) XamlReader.Parse(
            #endif
                template.Replace("__ASSEMBLYNAME__", assemblyName));           
        });

        public static bool Disable { get; set; }


        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            // NB: If ReactiveUI.Routing is registered but they're not actually 
            // using it, we don't want to help them out here.
            if (!RxApp.IsServiceLocationConfigured() || Disable) return true;

            var viewProperties = getCurrentViewProperties();

            var itemsControl = viewProperties.Last().Sender as ItemsControl;
            if (itemsControl == null) return true;

            if (viewProperties.Last().PropertyName != "ItemsSource") return true;

            if (itemsControl.ItemTemplate != null) return true;

#if !SILVERLIGHT
            if (itemsControl.ItemTemplateSelector != null) return true;
#endif

            itemsControl.ItemTemplate = DefaultItemTemplate.Value;
            return true;
        }
    }
}