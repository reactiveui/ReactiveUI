using UIKit;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace IntegrationTests.iOS
{
    /// <summary>
    /// The main application which contains the entry point to the application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">Arguments passed from the command line to the application.</param>
        public static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
