using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using ReactiveUI.Xaml;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Routing
{
    /// <summary>
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    public class ViewModelViewHost : TransitioningContentControl
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public object ViewModel {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, somethingChanged));

        readonly Subject<Unit> updateViewModel = new Subject<Unit>();

        /// <summary>
        /// If no ViewModel is displayed, this content (i.e. a control) will be displayed.
        /// </summary>
        public object DefaultContent {
            get { return (object)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, somethingChanged));

        public ViewModelViewHost()
        {
            var latestViewModel = updateViewModel
                .Select(_ => (ViewModel ?? DataContext))
                .StartWith((object)null);

            latestViewModel.Subscribe(vm => {
                if (vm == null) {
                    Content = DefaultContent;
                    return;
                }

                var view = RxRouting.ResolveView(vm);
                view.ViewModel = vm;
                Content = view;
            });
        }

        static void somethingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ViewModelViewHost)dependencyObject).updateViewModel.OnNext(Unit.Default);
        }
    }
}