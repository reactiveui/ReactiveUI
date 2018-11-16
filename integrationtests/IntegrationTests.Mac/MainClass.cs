using AppKit;

namespace IntegrationTests.Mac
{
    /// <summary>
    /// The class which hosts the main entry point for the application.
    /// </summary>
    public static class MainClass
    {
        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="args">Arguments passed to the appliation on the command line.</param>
        public static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
