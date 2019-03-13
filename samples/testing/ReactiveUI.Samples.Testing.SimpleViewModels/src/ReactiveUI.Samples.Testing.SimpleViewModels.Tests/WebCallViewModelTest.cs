using Microsoft.Reactive.Testing;
using ReactiveUI.Samples.Testing.SimpleViewModels;
using ReactiveUI.Testing;
using System.Reactive.Linq;
using Xunit;

namespace ReactiveUI.Samples.Testing.SimpleViewModelsUnitTests
{
    /// <summary>
    /// The web call ViewModel is time dependent. There is the webservice time and there
    /// is the time that one waits for the user to stop typing. We could wait 800 ms, and test
    /// that way. Or we can time travel with some nifty tools from the System.Reactive.Testing
    /// namespace.
    /// </summary>
    public class WebCallViewModelTest
    {
        /// <summary>
        /// Make sure no webservice call is send off until 800 ms have passed.
        /// </summary>
        [Fact]
        public void TestNothingTill800ms()
        {
            // Run a test scheduler to put time under our control.
            new TestScheduler().With(s =>
            {
                var fixture = new WebCallViewModel(new immediateWebService());
                fixture.InputText = "hi";

                // Run the clock forward to 800 ms. At that point, nothing should have happened.
                s.AdvanceToMs(799);
                Assert.Equal("", fixture.ResultText);

                // Run the clock 1 tick past and the result should show up.
                s.AdvanceToMs(801);
                Assert.Equal("result hi", fixture.ResultText);
            });
        }

        /// <summary>
        /// User types something, pauses, then types something again.
        /// </summary>
        [Fact]
        public void TestDelayAfterUpdate()
        {
            // Run a test scheduler to put time under our control.
            new TestScheduler().With(s =>
            {
                var fixture = new WebCallViewModel(new immediateWebService());
                fixture.InputText = "hi";

                // Run the clock forward 300 ms, where they type again.
                s.AdvanceToMs(300);
                fixture.InputText = "there";

                // Now, at 800, there should be nothing!
                s.AdvanceToMs(799);
                Assert.Equal("", fixture.ResultText);

                // But, at 800+300+1, our result should appear!
                s.AdvanceToMs(800 + 300 + 1);
                Assert.Equal("result there", fixture.ResultText);
            });
        }

        /// <summary>
        /// This dummy webservice takes zero time so we can isolate the timing tests for
        /// the typing above.
        /// </summary>
        class immediateWebService : IWebCaller
        {
            public System.IObservable<string> GetResult(string searchItems)
            {
                return Observable.Return("result " + searchItems);
            }
        }

    }
}
