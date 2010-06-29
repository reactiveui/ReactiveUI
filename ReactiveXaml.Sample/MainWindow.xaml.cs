using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;

namespace ReactiveXamlSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export("AppView", typeof(Window))]
    public partial class MainWindow : Window
    {
        /* NB: Normally, this could be a regular old auto-property since it
         * never changes. However, since MEF will fill this in later, we have to
         * make it a DP. */
        [Import(typeof(AppViewModel))]
        public AppViewModel ViewModel {
            get { return (AppViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AppViewModel), typeof(MainWindow));

        /* COOLSTUFF: What's *not* here??
         *
         * In the M-V-VM model of WPF development, one thing that you have to
         * get used to is, that you Almost Never should be accessing controls
         * directly. If you write "textBox.Foo", you're probably Doing It
         * Wrong(tm).
         *
         * The controls on the page are a Visualization of your Model (via the
         * ViewModel). It's really important to grok that. Binding to your
         * ViewModel is the way that controls will communicate with your
         * program, as well as via Commanding (i.e. invocations of
         * ICommand-derived objects).
         *
         * Another thing that you should Almost Never do is need to write an
         * Event Handler. Instead, you should be using either Commands on the
         * ViewModel combined with Expression Triggers, or Observables. The
         * former is easier and semantically more correct, and the latter lets
         * you model more complex interactions that would get ugly with
         * Triggers (in fact, there's an Expression Trigger that fires when an
         * Observable provides a value!)
         *
         * Why do we go to all this effort? I like fiddling with controls and
         * wiring up events! The reason is Testability - any code you write that
         * does stuff like that will *only* work in the context of a user
         * control, which is going to make it a pain to test. If you write in
         * an M-V-VM manner with Commands, more of your code is available as
         * plain .NET objects, which you can get under a test runner *far*
         * easier than trying to unit test WPF objects.
         */

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
