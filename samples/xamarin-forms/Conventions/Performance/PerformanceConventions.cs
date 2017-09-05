using Conventional;
using Xunit;

namespace Conventions.Performance
{
    public class PerformanceConventions : IClassFixture<BaseFixture>
    {
        public PerformanceConventions(BaseFixture baseFixture)
        {
            _baseFixture = baseFixture;
        }

        private readonly BaseFixture _baseFixture;

        [Fact]
        public void AsyncLibrariesMustCallConfigureAwaitFalse()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.LibraryCodeShouldCallConfigureAwaitWhenAwaitingTasks);
        }
    }
}