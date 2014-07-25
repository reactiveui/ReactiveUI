using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xunit.Runners.UI;
using System.Reflection;
using Xunit;

namespace ReactiveUI.Tests
{
    [Activity(Label = "xUnit Android Runner", MainLauncher = true)]
    public class MainActivity : RunnerActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            Add(Assembly.GetExecutingAssembly());
            AddExecutionAssembly(typeof(ExceptionUtility).Assembly);
            // or in any reference assemblies           
            //  Add(typeof(m4a.tests.RunnerTest).Assembly);
            // or in any assembly that you load (since JIT is available)

#if false
            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
            Runner.Writer = new TcpTextWriter ("10.0.1.2", 16384);
            // start running the test suites as soon as the application is loaded
            Runner.AutoStart = true;
            // crash the application (to ensure it's ended) and return to springboard
            Runner.TerminateAfterExecution = true;
#endif
            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
