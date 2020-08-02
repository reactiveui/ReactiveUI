using System;
using Shouldly;
using Xunit;

namespace ReactiveUI.Tests.Suspension
{
    public class SuspensionHostExtensionsTests
    {
        [Fact]
        public void GetAppStateReturns()
        {
            var fixture = new SuspensionHost();
            fixture.AppState = new DummyAppState();

            var result = fixture.GetAppState<DummyAppState>();

            result.ShouldBe(fixture.AppState);
        }

        [Fact]
        public void NullSuspensionHostThrowsException()
        {
            var result = Record.Exception(() => ((SuspensionHost)null!).SetupDefaultSuspendResume());

            result.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NullAppStateDoesNotThrowException()
        {
            var fixture = new SuspensionHost();

            var result = Record.Exception(() => fixture.SetupDefaultSuspendResume());

            result.ShouldBeNull();
        }

        [Fact]
        public void ObserveAppStateDoesNotThrowException()
        {
            var fixture = new SuspensionHost();

            var result = Record.Exception(() => fixture.ObserveAppState<DummyAppState>().Subscribe());

            result.ShouldBeNull();
        }

        [Fact]
        public void ObserveAppStateDoesNotThrowInvalidCastException()
        {
            var fixture = new SuspensionHost();

            var result = Record.Exception(() => fixture.ObserveAppState<DummyAppState>().Subscribe());

            result.ShouldNotBeOfType<InvalidCastException>();
        }
    }
}
