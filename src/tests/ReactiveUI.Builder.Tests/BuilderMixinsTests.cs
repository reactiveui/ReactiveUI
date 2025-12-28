// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

[NotInParallel]
public class BuilderMixinsTests
{
    private static readonly Action[] NullBuilderCases;

    static BuilderMixinsTests()
    {
        var scheduler = ImmediateScheduler.Instance;
        NullBuilderCases =
        [
            () => BuilderMixins.WithTaskPoolScheduler(null!, scheduler),
            () => BuilderMixins.WithMainThreadScheduler(null!, scheduler),
            () => BuilderMixins.WithRegistrationOnBuild(null!, _ => { }),
            () => BuilderMixins.WithRegistration(null!, _ => { }),
            () => BuilderMixins.WithViewsFromAssembly(null!, typeof(BuilderMixinsTests).Assembly),
            () => BuilderMixins.WithPlatformModule<TestRegistrationModule>(null!),
            () => BuilderMixins.UsingSplatModule(null!, new TestSplatModule()),
            () => BuilderMixins.UsingSplatBuilder(null!, _ => { }),
            () => BuilderMixins.ForCustomPlatform(null!, scheduler, _ => { }),
            () => BuilderMixins.ForPlatforms(null!, _ => { }),
            () => BuilderMixins.ConfigureMessageBus(null!, _ => { }),
            () => BuilderMixins.ConfigureViewLocator(null!, _ => { }),
            () => BuilderMixins.ConfigureSuspensionDriver(null!, _ => { }),
            () => BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!),
        ];
    }

    [Before(HookType.Test)]
    public void SetUp() => AppBuilder.ResetBuilderStateForTests();

    [Test]
    public void Builder_extension_methods_throw_when_builder_null()
    {
        foreach (var action in NullBuilderCases)
        {
            ArgumentNullException.ThrowIfNull(action);
            Assert.Throws<ArgumentNullException>(action);
        }
    }

    [Test]
    public async Task WithTaskPoolScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithTaskPoolScheduler(scheduler);
            builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.TaskpoolScheduler).IsSameReferenceAs(scheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsSameReferenceAs(scheduler);
            }
        }
        finally
        {
            RxSchedulers.TaskpoolScheduler = original;
        }
    }

    [Test]
    public async Task WithMainThreadScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithMainThreadScheduler(scheduler);
            builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(scheduler);
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler);
            }
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = original;
        }
    }

    [Test]
    public async Task WithRegistrationOnBuild_registers_service_when_building()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistrationOnBuild(builder, r => r.RegisterConstant("mixins", typeof(string)));
        builder.WithCoreServices().Build();

        await Assert.That(resolver.GetService<string>()).IsEqualTo("mixins");
    }

    [Test]
    public async Task WithRegistration_registers_service_immediately()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistration(builder, r => r.RegisterConstant(42, typeof(int)));

        await Assert.That(resolver.GetService<int>()).IsEqualTo(42);
    }

    [Test]
    public async Task WithViewsFromAssembly_registers_views_in_resolver()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithViewsFromAssembly(builder, typeof(BuilderMixinsTestView).Assembly);
        builder.WithCoreServices().Build();

        var view = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        await Assert.That(view).IsTypeOf<BuilderMixinsTestView>();
    }

    [Test]
    public async Task WithPlatformModule_registers_module_types()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.WithPlatformModule<TestRegistrationModule>(builder);
        builder.WithCoreServices().Build();

        await Assert.That(resolver.GetService<PlatformRegistrationMarker>()).IsNotNull();
    }

    [Test]
    public async Task UsingSplatModule_invokes_module_registration()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var module = new TestSplatModule();

        BuilderMixins.UsingSplatModule(builder, module);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(resolver.GetService<SplatModuleMarker>()).IsNotNull();
            await Assert.That(module.Registered).IsTrue();
        }
    }

    [Test]
    public async Task UsingSplatBuilder_executes_callback()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var invoked = false;

        BuilderMixins.UsingSplatBuilder(builder, _ => invoked = true);

        await Assert.That(invoked).IsTrue();
    }

    [Test]
    public async Task ForCustomPlatform_sets_scheduler_and_platform_registrations()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var scheduler = ImmediateScheduler.Instance;

        BuilderMixins.ForCustomPlatform(builder, scheduler, r => r.RegisterConstant(new PlatformRegistrationMarker(), typeof(PlatformRegistrationMarker)));
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(scheduler);
            await Assert.That(resolver.GetService<PlatformRegistrationMarker>()).IsNotNull();
        }
    }

    [Test]
    public async Task ForPlatforms_invokes_all_actions()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var executed = new List<string>();

        BuilderMixins.ForPlatforms(
            builder,
            b => executed.Add("first"),
            b => executed.Add("second"));

        await Assert.That(executed).IsEquivalentTo(["first", "second"]);
    }

    [Test]
    public async Task ConfigureMessageBus_registers_configured_instance()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.ConfigureMessageBus(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(resolver.GetService<IMessageBus>()).IsAssignableTo<MessageBus>();
            await Assert.That(configured).IsTrue();
        }
    }

    [Test]
    public async Task ConfigureViewLocator_registers_configured_locator()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.ConfigureViewLocator(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(resolver.GetService<IViewLocator>()).IsAssignableTo<DefaultViewLocator>();
            await Assert.That(configured).IsTrue();
        }
    }

    [Test]
    public async Task ConfigureSuspensionDriver_invokes_action_when_driver_registered()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var driver = new TestSuspensionDriver();
        resolver.RegisterConstant(driver, typeof(ISuspensionDriver));
        ISuspensionDriver? observed = null;

        BuilderMixins.ConfigureSuspensionDriver(builder, d => observed = d);
        builder.WithCoreServices().Build();

        await Assert.That(observed).IsSameReferenceAs(driver);
    }

    [Test]
    public async Task RegisterViewModel_registers_transient_view_model()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<BuilderMixinsTestViewModel>();
        var second = resolver.GetService<BuilderMixinsTestViewModel>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(second).IsNotNull();
            await Assert.That(first).IsNotSameReferenceAs(second);
        }
    }

    [Test]
    public async Task RegisterSingletonViewModel_registers_singleton_instance()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<BuilderMixinsTestViewModel>();
        var second = resolver.GetService<BuilderMixinsTestViewModel>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(first).IsSameReferenceAs(second);
        }
    }

    [Test]
    public async Task RegisterView_registers_transient_view()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(second).IsNotNull();
            await Assert.That(first).IsNotSameReferenceAs(second);
        }
    }

    [Test]
    public async Task RegisterSingletonView_registers_singleton_view()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = resolver.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(first).IsSameReferenceAs(second);
        }
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
