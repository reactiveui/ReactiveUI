using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using ReactiveXaml;
using System.Concurrency;

namespace ReactiveXamlSample
{
	public partial class AddPersonDialog : Window
	{
		protected AddPersonDialog()
		{
			this.InitializeComponent();

            // NB: Since this is a Dependency Property, we set it *after* the
            // InitializeComponent.
            ViewModel = new AddPersonViewModel();

            // HACK: This is a trick to let us close the window once the OK
            // button gets pressed. We don't need this for the Cancel button
            // because we can get away with the CloseWindowAction behavior.
            ViewModel.OkCommand.Subscribe(_ => {
                DialogResult = true;
                Close();
            });
		}

        /* COOLSTUFF: Implementing model-based dialogs
         *
         * Many of the dialogs you write will be centered around editing a
         * single object - instead of having the main window have to deal with
         * creating the Window object and managing showing it, write a method
         * like this so that it just gets back the model object that it's
         * looking for, or null if the user gave up
         *
         * Why the inner class though? Since our main class derives from Window,
         * we can't really import it directly in the test runner, nor could we
         * reuse the object since once you close a Window, the object is
         * essentially trashed. In the test runner, we'll provide an
         * implementation that just returns either a dummy object, or null.
         */

        [Export(typeof(IPromptForModelDialog<PersonEntry>))]
        public class wpfAddPersonDialogPrompt : IPromptForModelDialog<PersonEntry>
        {
            public PersonEntry Prompt(object sender, object parameter)
            {
                var dlg = new AddPersonDialog();
                bool? cancelled = dlg.ShowDialog();
                if (cancelled == null || cancelled.Value == false)
                    return null;

                return dlg.ViewModel.Person;
            }
        }


        /*
         * Our ViewModel
         */

        public AddPersonViewModel ViewModel {
            get { return (AddPersonViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AddPersonViewModel), typeof(AddPersonDialog));
    }

    /* COOLSTUFF: A Simpler ViewModel
     *
     * Here's a cleaner example of a prototypical ViewModel - our ViewModel is a
     * class that solves the "impedance mismatch" between our Model (which is
     * UI-independent and easily testable), and our View (our controls and our
     * window).
     *
     * What do I mean by "impedance mismatch?" For example, in our Model we
     * might store a file as a full path - however, when we display a file in
     * the UI, we probably just want to display the name, and maybe an icon. We
     * wouldn't want to put code in our Model to do this, that'd pollute our
     * Model with UI details. Instead, we'd make properties on the ViewModel
     * that changes the data to the format we want. You can almost think of a
     * ViewModel as just a bunch of IValueConverters put together.
     *
     * So, three things go in our ViewModel:
     *
     * 1. The Model (PersonEntry)
     * 2. Commands that our View will want to execute (like SetImageViaFlickr)
     * 3. Properties that transform model data into more convenient formats
     *    (like SpinnerVisibility)
     */

    public class AddPersonViewModel : ReactiveValidatedObject
    {
        public AddPersonViewModel()
        {
            setupCommands();
        }

        /*
         * Our Model
         */

        PersonEntry _Person = new PersonEntry();
        public PersonEntry Person {
            get { return _Person; }
            set {
                if (_Person == value)
                    return;
                _Person = value;
                RaisePropertyChanged("Person");
            }
        }


        /*
         * Command Declarations
         */

        public ReactiveAsyncCommand SetImageViaFlickr { get; protected set; }
        public ReactiveCommand OkCommand { get; protected set; }


        /*
         * Properties that help our view out
         */

        ObservableAsPropertyHelper<Visibility> _SpinnerVisibility;
        public Visibility SpinnerVisibility {
            get {
                return _SpinnerVisibility.Value;
            }
        }

        protected void setupCommands()
        {

            /* COOLSTUFF: How to do stuff in the background
             *
             * ReactiveAsyncCommand is similar to ReactiveCommand, except that
             * its Executed function queues an item to run in the background that
             * we set up with the RegisterAsyncFunction method.
             *
             * It also guarantees that it will only provide results on the UI
             * thread. So, the calculation bit goes in the Executed part, then
             * we Subscribe to get the value that eventually gets generated.
             * 
             * This is important because WPF controls have thread affinity, that
             * is, you can only access them from the thread that created them.
             * Even though our VM objects aren't WPF controls, the change
             * notification fired by them will eventually result in touching WPF
             * controls, so we have to guarantee that the notifications only fire
             * on the UI thread.
             */

            SetImageViaFlickr = new ReactiveAsyncCommand(null, 1, Scheduler.Dispatcher);

            // NB: The regular RNG isn't thread-safe, and will be very un-random
            // if we use it from more than one thread - however, RAC guarantees 
            // that only one will be running at a time.
            var rng = new Random();

            var results = SetImageViaFlickr.RegisterAsyncFunction(_ => {
                var images = findRandomFlickrImages("Face");
                if (images == null || images.Length == 0)
                    return null;
                if (images.Length == 1)
                    return images[0];

                // Return a random URL
                return images[rng.Next(0, images.Length - 1)];
            });
                
            // This will be on the UI thread, so we're safe to set WPF properties
            // here
            results.Subscribe(s => Person.Image = new BitmapImage(new Uri(s)),
                ex => this.Log().Error("Oh no!", ex));

            /* COOLSTUFF: The Reactive Extensions
             *
             * Reactive Extensions (Rx) allow us to take existing IObservables
             * and Events, and combine them to into something we're interested
             * in looking for.
             *
             * For the OK button, it really should only be able to be
             * clicked when the model is valid, *and* we're not trying to fetch
             * a picture from Flickr. We'll use the Flickr CanExecute observable
             * and combine it with the Model's observable, who fires every time
             * that we change a property since it's a ReactiveObservableObject.
             */

            _SpinnerVisibility = new ObservableAsPropertyHelper<Visibility>(
                SetImageViaFlickr.CanExecuteObservable.Select(x => x ? Visibility.Collapsed : Visibility.Visible),
                _ => RaisePropertyChanged("SpinnerVisibility"), Visibility.Collapsed);

            OkCommand = new ReactiveCommand(
                SetImageViaFlickr.CanExecuteObservable.CombineLatest(Person.Select(x => Person.IsValid()),
                    (flickr_isnt_running, is_valid) => flickr_isnt_running && is_valid),
                null, Scheduler.Dispatcher);
        }

        /* COOLSTUFF: Always write the sync version first!
         *
         * When you're writing an asynchronous function, it's always handy to
         * write the core routine synchronously, so that you can test it in
         * isolation. Later, you can add in the async bits, preferably in a
         * general way
         */

        static string[] findRandomFlickrImages(string SearchTerm)
        {
            var doc = XDocument.Load(String.Format(CultureInfo.InvariantCulture,
                "http://api.flickr.com/services/feeds/photos_public.gne?tags={0}&format=rss_200",
                HttpUtility.UrlEncode(SearchTerm)));

            if (doc.Root == null)
                return null;

            var node_name = "{http://search.yahoo.com/mrss/}thumbnail";
            return doc.Root.Descendants(node_name)
                .Select(x => x.Attributes("url").First().Value)
                .ToArray();
        }
    }
}
