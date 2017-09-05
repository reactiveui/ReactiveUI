using Conventional;
using Xunit;

namespace Conventions.Danger
{
    // When building systems, we often employ tools that when held the wrong way, pose danger to
    // ourselves and to other denizens of our codebase. Let's put some rubber corners on these tools
    // to ensure all contributors to our software are safe from cutting themselves on sharp edges.
    public class DangerConventions : IClassFixture<BaseFixture>
    {
        private readonly BaseFixture _baseFixture;

        public DangerConventions(BaseFixture baseFixture)
        {
            _baseFixture = baseFixture;
        }

        /// <summary>
        /// async void is bad news because you can’t wait for its completion and any unhandled
        /// exceptions will terminate your process(ouch!)
        /// </summary>
        [Fact]
        public void AsyncMethodsMustNotBeVoid()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.VoidMethodsMustNotBeAsync);
        }

        [Fact]
        public void MustNotResolveCurrentTimeViaDateTime()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.MustNotResolveCurrentTimeViaDateTime);
        }

        [Fact]
        public void MustNotUseDateTimeOffsetNow()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.MustNotUseDateTimeOffsetNow);
        }
    }
}