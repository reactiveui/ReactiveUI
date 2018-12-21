namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// A mock for the screen in ReactiveUI. This will only contain the routing state.
    /// </summary>
    public class MockHostScreen : IScreen
    {
        /// <summary>
        /// Gets the routing state for our mock.
        /// </summary>
        public RoutingState Router { get; } = new RoutingState();
    }
}
