namespace ReactiveUI.Benchmarks
{
    public class MockHostScreen : IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
    }
}