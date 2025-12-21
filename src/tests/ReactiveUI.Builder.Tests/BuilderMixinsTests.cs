// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests covering the builder extension methods defined in <see cref="BuilderMixins"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BuilderMixinsTests
{
    [SetUp]
    public void SetUp() => AppBuilder.ResetBuilderStateForTests();

    [TestCaseSource(nameof(NullBuilderCases))]
    public void Builder_extension_methods_throw_when_builder_null(TestDelegate action) =>
        Assert.Throws<ArgumentNullException>(action);

    [Test]
    public void WithTaskPoolScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithTaskPoolScheduler(scheduler);
            builder.WithCoreServices().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(builder.TaskpoolScheduler, Is.SameAs(scheduler));
                Assert.That(RxSchedulers.TaskpoolScheduler, Is.SameAs(scheduler));
            }
        }
        finally
        {
            RxSchedulers.TaskpoolScheduler = original;
        }
    }

    [Test]
    public void WithMainThreadScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithMainThreadScheduler(scheduler);
            builder.WithCoreServices().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(builder.MainThreadScheduler, Is.SameAs(scheduler));
                Assert.That(RxSchedulers.MainThreadScheduler, Is.SameAs(scheduler));
            }
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = original;
        }
    }

    [Test]
    public void WithRegistrationOnBuild_registers_service_when_building()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistrationOnBuild(builder, r => r.RegisterConstant("mixins", typeof(string)));
        builder.WithCoreServices().Build();

        Assert.That(resolver.GetService<string>(), Is.EqualTo("mixins"));
    }

    [Test]
    public void WithRegistration_registers_service_immediately()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistration(builder, r => r.RegisterConstant(42, typeof(int)));

        Assert.That(resolver.GetService<int>(), Is.EqualTo(42));
    }

    [Test]
    public void WithViewsFromAssembly_registers_views_in_resolver()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithViewsFromAssembly(builder, typeof(BuilderMixinsTestView).Assembly);
        builder.WithCoreServices().Build();

        var view = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        Assert.That(view, Is.TypeOf<BuilderMixinsTestView>());
    }

    [Test]
    public void WithPlatformModule_registers_module_types()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithPlatformModule<TestRegistrationModule>(builder);
        builder.WithCoreServices().Build();

        Assert.That(resolver.GetService<PlatformRegistrationMarker>(), Is.Not.Null);
    }

    [Test]
    public void UsingSplatModule_invokes_module_registration()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var module = new TestSplatModule();

        BuilderMixins.UsingSplatModule(builder, module);
        builder.WithCoreServices().Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(resolver.GetService<SplatModuleMarker>(), Is.Not.Null);
            Assert.That(module.Registered, Is.True);
        }
    }

    [Test]
    public void UsingSplatBuilder_executes_callback()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var invoked = false;

        BuilderMixins.UsingSplatBuilder(builder, _ => invoked = true);

        Assert.That(invoked, Is.True);
    }

    [Test]
    public void ForCustomPlatform_sets_scheduler_and_platform_registrations()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var scheduler = ImmediateScheduler.Instance;

        BuilderMixins.ForCustomPlatform(builder, scheduler, r => r.RegisterConstant(new PlatformRegistrationMarker(), typeof(PlatformRegistrationMarker)));
        builder.WithCoreServices().Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.MainThreadScheduler, Is.SameAs(scheduler));
            Assert.That(resolver.GetService<PlatformRegistrationMarker>(), Is.Not.Null);
        }
    }

    [Test]
    public void ForPlatforms_invokes_all_actions()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var executed = new List<string>();

        BuilderMixins.ForPlatforms(
            builder,
            b => executed.Add("first"),
            b => executed.Add("second"));

        Assert.That(executed, Is.EquivalentTo(new[] { "first", "second" }));
    }

    [Test]
    public void ConfigureMessageBus_registers_configured_instance()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.ConfigureMessageBus(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(resolver.GetService<IMessageBus>(), Is.InstanceOf<MessageBus>());
            Assert.That(configured, Is.True);
        }
    }

    [Test]
    public void ConfigureViewLocator_registers_configured_locator()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.ConfigureViewLocator(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(resolver.GetService<IViewLocator>(), Is.InstanceOf<DefaultViewLocator>());
            Assert.That(configured, Is.True);
        }
    }

    [Test]
    public void ConfigureSuspensionDriver_invokes_action_when_driver_registered()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var driver = new TestSuspensionDriver();
        resolver.RegisterConstant(driver, typeof(ISuspensionDriver));
        ISuspensionDriver? observed = null;

        BuilderMixins.ConfigureSuspensionDriver(builder, d => observed = d);
        builder.WithCoreServices().Build();

        Assert.That(observed, Is.SameAs(driver));
    }

    [Test]
    public void RegisterViewModel_registers_transient_view_model()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<BuilderMixinsTestViewModel>();
        var second = resolver.GetService<BuilderMixinsTestViewModel>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(first, Is.Not.SameAs(second));
        }
    }

    [Test]
    public void RegisterSingletonViewModel_registers_singleton_instance()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<BuilderMixinsTestViewModel>();
        var second = resolver.GetService<BuilderMixinsTestViewModel>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.Not.Null);
            Assert.That(first, Is.SameAs(second));
        }
    }

    [Test]
    public void RegisterView_registers_transient_view()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(first, Is.Not.SameAs(second));
        }
    }

    [Test]
    public void RegisterSingletonView_registers_singleton_view()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.Not.Null);
            Assert.That(first, Is.SameAs(second));
        }
    }

    private static IEnumerable<TestCaseData> NullBuilderCases()
    {
        var scheduler = ImmediateScheduler.Instance;
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithTaskPoolScheduler(null!, scheduler)))
            .SetName("WithTaskPoolScheduler");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithMainThreadScheduler(null!, scheduler)))
            .SetName("WithMainThreadScheduler");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithRegistrationOnBuild(null!, _ => { })))
            .SetName("WithRegistrationOnBuild");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithRegistration(null!, _ => { })))
            .SetName("WithRegistration");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithViewsFromAssembly(null!, typeof(BuilderMixinsTests).Assembly)))
            .SetName("WithViewsFromAssembly");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.WithPlatformModule<TestRegistrationModule>(null!)))
            .SetName("WithPlatformModule");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.UsingSplatModule(null!, new TestSplatModule())))
            .SetName("UsingSplatModule");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.UsingSplatBuilder(null!, _ => { })))
            .SetName("UsingSplatBuilder");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.ForCustomPlatform(null!, scheduler, _ => { })))
            .SetName("ForCustomPlatform");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.ForPlatforms(null!, _ => { })))
            .SetName("ForPlatforms");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.ConfigureMessageBus(null!, _ => { })))
            .SetName("ConfigureMessageBus");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.ConfigureViewLocator(null!, _ => { })))
            .SetName("ConfigureViewLocator");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.ConfigureSuspensionDriver(null!, _ => { })))
            .SetName("ConfigureSuspensionDriver");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(null!)))
            .SetName("RegisterViewModel");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(null!)))
            .SetName("RegisterSingletonViewModel");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!)))
            .SetName("RegisterView");
        yield return new TestCaseData((TestDelegate)(() => BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!)))
            .SetName("RegisterSingletonView");
    }

    private sealed class BuilderMixinsTestViewModel : ReactiveObject
    {
    }

    private sealed class BuilderMixinsTestView : IViewFor<BuilderMixinsTestViewModel>
    {
        public BuilderMixinsTestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (BuilderMixinsTestViewModel?)value;
        }
    }

    private sealed class PlatformRegistrationMarker
    {
    }

    private sealed class SplatModuleMarker
    {
    }

    private sealed class TestRegistrationModule : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformRegistrationMarker(), typeof(PlatformRegistrationMarker));
        }
    }

    private sealed class TestSplatModule : IModule
    {
        public bool Registered { get; private set; }

        public void Configure(IMutableDependencyResolver services)
        {
            services.RegisterConstant(new SplatModuleMarker(), typeof(SplatModuleMarker));
            Registered = true;
        }

        public void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver? resolver) =>
            Configure(services);
    }

    private sealed class TestSuspensionDriver : ISuspensionDriver
    {
        public IObservable<object?> LoadState() => Observable.Return<object?>(null);

        public IObservable<Unit> SaveState(object state) => Observable.Return(Unit.Default);

        public IObservable<Unit> InvalidateState() => Observable.Return(Unit.Default);
    }
}
