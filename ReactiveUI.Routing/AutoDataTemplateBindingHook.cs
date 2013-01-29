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
        public static DataTemplate DefaultItemTemplate = (DataTemplate)
            #if SILVERLIGHT || WINRT
            XamlReader.Load(
            #else
            XamlReader.Parse(
            #endif
                #if WINRT
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:routing='using:ReactiveUI.Routing'>" +
                    "<routing:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
                "</DataTemplate>"
                #else
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                        "xmlns:routing='clr-namespace:ReactiveUI.Routing;assembly=ReactiveUI.Routing'> " +
                    "<routing:ViewModelViewHost ViewModel=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
                "</DataTemplate>"
                #endif
            );

        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            var viewProperties = getCurrentViewProperties();

            var itemsControl = viewProperties.Last().Sender as ItemsControl;
            if (itemsControl == null) return true;

            if (viewProperties.Last().PropertyName != "ItemsSource") return true;

            if (itemsControl.ItemTemplate != null) return true;

#if !SILVERLIGHT
            if (itemsControl.ItemTemplateSelector != null) return true;
#endif

            itemsControl.ItemTemplate = DefaultItemTemplate;
            return true;
        }
    }
}