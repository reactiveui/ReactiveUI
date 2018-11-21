namespace IntegrationTests.XamarinForms.UWP
{
    /// <summary>
    /// The main page for the application.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new XamarinForms.App());
        }
    }
}
