namespace ReactiveUI.Tests;

public partial class DefaultViewLocatorTests
{
    /// <summary>
    /// Tests that whether this instance [can resolve view from view model with IRoutableViewModel].
    /// </summary>
    [Test]
    public void CanResolveViewFromViewModelWithIRoutableViewModelType()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            Assert.That(result, Is.TypeOf<RoutableFooView>());
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can override name resolution function].
    /// </summary>
    [Test]
    public void CanOverrideNameResolutionFunc()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new RoutableFooCustomView());

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc = x => x.Replace("ViewModel", "CustomView")
            };
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            Assert.That(result, Is.TypeOf<RoutableFooCustomView>());
        }
    }
}
