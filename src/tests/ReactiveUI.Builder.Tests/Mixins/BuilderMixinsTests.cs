// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text.Json.Serialization.Metadata;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Mixins;

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
            () => BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!)];
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
    public async Task UsingSplatBuilder_Executes_Callback()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var invoked = false;

        BuilderMixins.UsingSplatBuilder(builder, _ => invoked = true);

        await Assert.That(invoked).IsTrue();
    }

    [Test]
    public async Task UsingSplatBuilder_Handles_Null_Callback()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        var result = BuilderMixins.UsingSplatBuilder(builder, null);

        await Assert.That(result).IsSameReferenceAs(builder);
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

    // Additional coverage tests for WithInstance overloads
    [Test]
    public async Task WithInstance_SingleParameter_InvokesActionWithResolvedInstance()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        resolver.RegisterConstant("test-value", typeof(string));

        string? captured = null;
        builder.WithInstance<string>(value => captured = value);
        builder.WithCoreServices().Build();

        await Assert.That(captured).IsEqualTo("test-value");
    }

    [Test]
    public async Task WithInstance_TwoParameters_InvokesActionWithBothInstances()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        resolver.RegisterConstant("string-value", typeof(string));
        resolver.RegisterConstant(42, typeof(int));

        string? capturedString = null;
        int? capturedInt = null;
        builder.WithInstance<string, int>((s, i) =>
        {
            capturedString = s;
            capturedInt = i;
        });
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(capturedString).IsEqualTo("string-value");
            await Assert.That(capturedInt).IsEqualTo(42);
        }
    }

    [Test]
    public void WithInstance_WithNullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            BuilderMixins.WithInstance<string>(null!, _ => { }));
    }

    [Test]
    public async Task RegisterViews_WithNullBuilder_ThrowsArgumentNullException()
    {
        await Assert.That(() => BuilderMixins.RegisterViews(null!, _ => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task RegisterViews_WithNullConfigure_ThrowsArgumentNullException()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        await Assert.That(() => BuilderMixins.RegisterViews(builder, null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task RegisterViews_WithViewLocator_ReturnsBuilder()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        // WithCoreServices registers the DefaultViewLocator
        builder.WithCoreServices().Build();

        var result = BuilderMixins.RegisterViews(builder, views =>
            views.Map<BuilderMixinsTestViewModel, BuilderMixinsTestView>());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithViewModule_WithNullBuilder_ThrowsArgumentNullException()
    {
        await Assert.That(() => BuilderMixins.WithViewModule<TestViewModule>(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithViewModule_ReturnsBuilder()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();

        // WithCoreServices registers the DefaultViewLocator
        builder.WithCoreServices().Build();

        var result = BuilderMixins.WithViewModule<TestViewModule>(builder);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task BuildApp_WithNullBuilder_ThrowsArgumentNullException()
    {
        await Assert.That(() => BuilderMixins.BuildApp(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task BuildApp_WithNonReactiveUIBuilder_ThrowsInvalidOperationException()
    {
        var nonReactiveBuilder = new NonReactiveUIAppBuilder();

        var ex = await Assert.That(() => BuilderMixins.BuildApp(nonReactiveBuilder))
            .Throws<InvalidOperationException>();

        await Assert.That(ex).IsNotNull();
        await Assert.That(ex.Message).Contains("not an IReactiveUIBuilder");
    }

    [Test]
    public async Task BuildApp_WithReactiveUIBuilder_BuildsSuccessfully()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        builder.WithCoreServices();

        var result = BuilderMixins.BuildApp(builder);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithMessageBus_WithNullBuilder_ThrowsArgumentNullException()
    {
        var messageBus = new MessageBus();

        await Assert.That(() => BuilderMixins.WithMessageBus(null!, messageBus))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithMessageBus_RegistersCustomMessageBus()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var customMessageBus = new MessageBus();

        BuilderMixins.WithMessageBus(builder, customMessageBus);
        builder.WithCoreServices().Build();

        var registered = resolver.GetService<IMessageBus>();
        await Assert.That(registered).IsSameReferenceAs(customMessageBus);
    }

    [Test]
    public async Task WithTaskPoolScheduler_WithSetRxAppFalse_DoesNotSetRxSchedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            // Set a known baseline scheduler
            var baselineScheduler = CurrentThreadScheduler.Instance;
            RxSchedulers.TaskpoolScheduler = baselineScheduler;

            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithTaskPoolScheduler(scheduler, setRxApp: false);
            builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.TaskpoolScheduler).IsSameReferenceAs(scheduler);

                // RxSchedulers should remain unchanged (still the baseline)
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsSameReferenceAs(baselineScheduler);
            }
        }
        finally
        {
            RxSchedulers.TaskpoolScheduler = original;
        }
    }

    [Test]
    public async Task WithMainThreadScheduler_WithSetRxAppFalse_DoesNotSetRxSchedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            // Set a known baseline scheduler
            var baselineScheduler = CurrentThreadScheduler.Instance;
            RxSchedulers.MainThreadScheduler = baselineScheduler;

            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithMainThreadScheduler(scheduler, setRxApp: false);
            builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(scheduler);

                // RxSchedulers should remain unchanged (still the baseline)
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(baselineScheduler);
            }
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = original;
        }
    }

    private sealed class TestViewModule : IViewModule
    {
        public void RegisterViews(DefaultViewLocator locator)
        {
            locator.Map<BuilderMixinsTestViewModel, BuilderMixinsTestView>(() => new BuilderMixinsTestView());
        }
    }

    private sealed class NonReactiveUIAppBuilder : IAppBuilder
    {
        public IAppInstance Build() => new TestAppInstance();

        public IAppBuilder UseCurrentSplatLocator() => this;

        public IAppBuilder UsingModule<T>(T module)
            where T : IModule => this;

        public IAppBuilder WithCoreServices() => this;

        public IAppBuilder WithCustomRegistration(Action<IMutableDependencyResolver> action) => this;
    }

    private sealed class TestAppInstance : IAppInstance
    {
        public IReadonlyDependencyResolver Current => throw new NotImplementedException();

        public IMutableDependencyResolver CurrentMutable => throw new NotImplementedException();

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required by interface")]
        public void Dispose()
        {
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
        public void Register(IRegistrar registrar)
        {
            registrar.RegisterConstant<PlatformRegistrationMarker>(() => new PlatformRegistrationMarker());
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

        public IObservable<Unit> SaveState<T>(T state) => Observable.Return(Unit.Default);

        public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => Observable.Return<T?>(default);

        public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => Observable.Return(Unit.Default);

        public IObservable<Unit> InvalidateState() => Observable.Return(Unit.Default);
    }
}
