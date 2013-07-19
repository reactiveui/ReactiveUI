using System;
using System.Windows;
using System.Windows.Controls;
using System.Reactive.Linq;
using System.Windows.Media;
using System.Reactive.Disposables;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using ReactiveUI;


namespace ReactiveUI.Xaml
{

    /// <summary>
    /// A generic control for wrapping up the concept of a View that has a ViewModel.
    /// Due to XAML not being friendly for generic types it is recommended to use the 
    /// following pattern
    /// 
    ///    namespace BradsAwsomeNamespace
    ///    {
    ///        public class BradViewModel : ReactiveObject {
    ///             string _Name;
    ///             public string Name 
    ///             {
    ///                 get { return _Name; }
    ///                 set { this.RaiseAndSetIfChanged(ref _Name, value); }
    ///             }
    ///             string _PhoneNumber;
    ///             public string PhoneNumber 
    ///             {
    ///                 get { return _PhoneNumber; }
    ///                 set { this.RaiseAndSetIfChanged(ref _PhoneNumber, value); }
    ///             }
    ///        }
    ///
    ///        public class BradViewBase : ReactiveUserControl<BradViewModel> { }
    ///        public partial class BradView : BradViewBase
    ///        {
    ///            public BradView()
    ///            {
    ///                InitializeComponent();
    ///            }
    ///        }
    ///    }
    ///
    /// and then in XAML do
    /// 
    ///    <l:BradViewBase x:Class="WeinCad.Controls.View.MoineauMillingView"
    ///             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
    ///             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
    ///             xmlns:l="clr-namespace:WeinCad.Controls.View"
    ///             mc:Ignorable="d" 
    ///             d:DesignHeight="300" d:DesignWidth="300">
    ///         <Grid>
    ///             <Label Content={Binding Name}/>
    ///             <Label Content={Binding PhoneNumber}/>
    ///         </Grid>
    ///    </l:BradViewBase>
    ///
    /// and use your new control like
    /// 
    ///     <DataTemplate>
    ///         <c:BradView ViewModel={Binding BradviewModel}/>
    ///     </DataTemplate>
    ///
    /// There are also helper methods to manage lifetimes of IDisposableResources. For example
    /// you may wish in your code behind to register to a timer but dispose of it when the
    /// user control is no longer used. Using ReactiveUserControl this is easy.
    /// 
    ///     _Counter = Observable.Interval(TimeSpan.Seconds(1))
    ///         .ToProperty(this, p => p.Counter);
    ///         
    ///     _Counter.DisposeWith(this);
    ///     
    /// Now the _Counter will be disposed when the control is unloaded or the
    /// dispatcher is shut down. 
    /// 
    ///
    /// Also note that the DataContext within the XAML associated with the user
    /// control is not the same as the DataContext of the user control when it
    /// is inserted into client XAML. For the purposes of sanity the DataContext
    /// inside the user control is the ViewModel instance. If a user of the
    /// user control manually sets the DataContext as in
    /// 
    ///     <c:BradView DataContext={Binding FooPath} ViewModel={Binding Model}/>
    ///     
    /// which is effectively the same thing as
    ///
    ///     <c:BradView ViewModel={Binding Path=FooPath.Model}/>
    ///     
    /// has no effect on the DataContext inside the XAML defining the BradView
    /// which will always remain the ViewModel instance.
    ///     
    /// </summary>
    /// <typeparam name="ViewModelT"></typeparam>
    [ContentProperty("AdditionalContent")]
    public class ReactiveUserControl<ViewModelT> : UserControl, IViewFor<ViewModelT>
        where ViewModelT : class
    {
        #region IViewFor<ViewModelT>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ViewModelT), typeof(ReactiveUserControl<ViewModelT>), new PropertyMetadata(null));

        public ViewModelT ViewModel
        {
            get { return (ViewModelT)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return (ViewModelT)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        #endregion

        /// <summary>
        /// Gets or sets additional content for the UserControl. The additional
        /// content will have it's DataContext set to an instance of the ViewModel
        /// </summary>
        public UIElement AdditionalContent
        {
            get { return (UIElement)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(UIElement), typeof(ReactiveUserControl<ViewModelT>),
              new PropertyMetadata(null));

        public IObservable<ViewModelT> ViewModelObservable()
        {
            return this.WhenAny(x => x.ViewModel, x => x.Value)
                .Where(v => v != null);
        }

        public UniformGrid DataContextHost { get; private set; }

        public ReactiveUserControl()
        {
            DataContextHost = new UniformGrid();
            this.Content = DataContextHost;

            // If AdditionalContent changes then we need to
            // add it to the visual tree
            this.WhenAny(p => p.AdditionalContent, p => p.Value)
                .Where(v => v != null)
                .Subscribe(v =>
                {
                    DataContextHost.Children.Clear();
                    DataContextHost.Children.Add(v);
                });

            // If the ViewModel changes we need to ensure the
            // AdditionalContent get's the correct DataContext
            this.WhenAny(x => x.ViewModel, x => x.Value)
                .BindTo(this, x => x.DataContextHost.DataContext);

            DesignUnsafeConstruct();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {

                DesignSafeConstruct();

            }
        }

        public virtual void DesignUnsafeConstruct()
        {
        }

        public virtual void DesignSafeConstruct()
        {
        }
    }

    public static class ReactiveUserControlMixins
    {
        /// <summary>
        /// Manage lifetime of a stream of disposables relative to the
        /// lifetime of the Control.  Disposes the previous disposable in the stream
        /// when the next is arrives and then dispose the last one when the UI is shut down
        /// </summary>
        /// <param name="This"></param>
        /// <param name="control"></param>
        public static void SeriallyDisposeWith(this IObservable<IDisposable> This, Control control)
        {
            var disposer = new SerialDisposable();
            This.Subscribe(d => disposer.Disposable = d);
            disposer.DisposeWith(control);
        }

        /// <summary>
        /// Manage the lifetime of disposables relative to
        /// a Control
        /// </summary>
        public class DisposableLifetimeManager
        {
            public EventHandler ShutdownStartedEventHandler = null;
            public RoutedEventHandler UnloadedEventHandler = null;
            public Control Control;
            public IDisposable Disposable;

            public static void DisposeWith( Control control, IDisposable disposer )
            {
                new DisposableLifetimeManager(control, disposer);
            }

            private DisposableLifetimeManager( Control control, IDisposable disposable )
            {
                Disposable = disposable;
                Control = control;
                ShutdownStartedEventHandler = ( s, e ) => DisposeAndDetachHandler();
                UnloadedEventHandler = ( s, e ) => DisposeAndDetachHandler();

                control.Unloaded += UnloadedEventHandler;
                control.Dispatcher.ShutdownStarted += ShutdownStartedEventHandler;
            }

            private void DisposeAndDetachHandler()
            {
                Disposable.Dispose();

                // Does CLR throw exceptions if removing an allready removed
                // event handler?
                try { Control.Unloaded -= UnloadedEventHandler; }
                finally { }

                try { Control.Dispatcher.ShutdownStarted -= ShutdownStartedEventHandler; }
                finally { }
            }

        }

        /// <summary>
        /// Manage the lifetime of a single disposable relative to the lifetime
        /// of the control. The disposable is disposed either when the control
        /// is unloaded or the dispatcher is shutdown.
        /// 
        /// Be careful because the unload event is called on items in a DataTemplate
        /// which are recycled in an ItemsControl. You may have to recreate the
        /// disposable on the load event.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="contrl"></param>
        public static void DisposeWith(this IDisposable This, Control control)
        {
            DisposableLifetimeManager.DisposeWith(control, This);
        }

    }

    public class ReactiveWindow<ViewModelT> : Window, IViewFor<ViewModelT>
        where ViewModelT : class
    {
        #region IViewFor<ViewModel>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ViewModelT), typeof(ReactiveUserControl<ViewModelT>), new PropertyMetadata(null));

        public ViewModelT ViewModel
        {
            get { return (ViewModelT)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return (ViewModelT)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        #endregion

        public ReactiveWindow()
        {
        }
    }
}
