// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Builder.Tests.Executors;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>
/// Tests for the <see cref="BuilderMixins"/> extension methods.
/// </summary>
[NotInParallel]
[TestExecutor<ResetOnlyExecutor>]
public class BuilderMixinsTests
{
    /// <summary>
    /// The set of mixin invocations that should each throw when given a null builder.
    /// </summary>
    private static readonly Action[] NullBuilderCases;

    /// <summary>
    /// Initializes static members of the <see cref="BuilderMixinsTests"/> class.
    /// </summary>
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
            () => BuilderMixins.WithMessageBus(null!, _ => { }),
            () => BuilderMixins.ConfigureViewLocator(null!, _ => { }),
            () => BuilderMixins.ConfigureSuspensionDriver(null!, _ => { }),
            () => BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!),
            () => BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(null!)
        ];
    }

    /// <summary>
    /// Verifies that every builder extension method throws <see cref="ArgumentNullException"/> when given a null builder.
    /// </summary>
    [Test]
    public void Builder_extension_methods_throw_when_builder_null()
    {
        foreach (var action in NullBuilderCases)
        {
            ArgumentNullException.ThrowIfNull(action);
            Assert.Throws<ArgumentNullException>(action);
        }
    }

    /// <summary>
    /// Verifies that setting the task pool scheduler updates both the builder and <see cref="RxSchedulers"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithTaskPoolScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
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

    /// <summary>
    /// Verifies that setting the main thread scheduler updates both the builder and <see cref="RxSchedulers"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithMainThreadScheduler_sets_scheduler_and_rx_schedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
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

    /// <summary>
    /// Verifies that a registration deferred to build time registers its service when the builder is built.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithRegistrationOnBuild_registers_service_when_building()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistrationOnBuild(builder, r => r.RegisterConstant("mixins", typeof(string)));
        builder.WithCoreServices().Build();

        await Assert.That(Locator.Current.GetService<string>()).IsEqualTo("mixins");
    }

    /// <summary>
    /// Verifies that an immediate registration registers its service right away.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithRegistration_registers_service_immediately()
    {
        const int ExpectedRegisteredValue = 42;
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.WithRegistration(builder, r => r.RegisterConstant(ExpectedRegisteredValue, typeof(int)));

        await Assert.That(Locator.Current.GetService<int>()).IsEqualTo(ExpectedRegisteredValue);
    }

    /// <summary>
    /// Verifies that views discovered in an assembly are registered in the resolver.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithViewsFromAssembly_registers_views_in_resolver()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.WithViewsFromAssembly(builder, typeof(BuilderMixinsTestView).Assembly);
        builder.WithCoreServices().Build();

        var view = Locator.Current.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        await Assert.That(view).IsTypeOf<BuilderMixinsTestView>();
    }

    /// <summary>
    /// Verifies that a platform module registers its types.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithPlatformModule_registers_module_types()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.WithPlatformModule<TestRegistrationModule>(builder);
        builder.WithCoreServices().Build();

        await Assert.That(Locator.Current.GetService<PlatformRegistrationMarker>()).IsNotNull();
    }

    /// <summary>
    /// Verifies that using a Splat module invokes its registration.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UsingSplatModule_invokes_module_registration()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var module = new TestSplatModule();

        BuilderMixins.UsingSplatModule(builder, module);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(Locator.Current.GetService<SplatModuleMarker>()).IsNotNull();
            await Assert.That(module.Registered).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that using the Splat builder executes the supplied callback.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UsingSplatBuilder_Executes_Callback()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var invoked = false;

        BuilderMixins.UsingSplatBuilder(builder, _ => invoked = true);

        await Assert.That(invoked).IsTrue();
    }

    /// <summary>
    /// Verifies that using the Splat builder with a null callback returns the same builder.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UsingSplatBuilder_Handles_Null_Callback()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = BuilderMixins.UsingSplatBuilder(builder, null);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that configuring a custom platform sets the scheduler and applies platform registrations.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ForCustomPlatform_sets_scheduler_and_platform_registrations()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var scheduler = ImmediateScheduler.Instance;

        BuilderMixins.ForCustomPlatform(
            builder,
            scheduler,
            r => r.RegisterConstant(new PlatformRegistrationMarker(), typeof(PlatformRegistrationMarker)));
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(scheduler);
            await Assert.That(Locator.Current.GetService<PlatformRegistrationMarker>()).IsNotNull();
        }
    }

    /// <summary>
    /// Verifies that configuring multiple platforms invokes all the supplied actions.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ForPlatforms_invokes_all_actions()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var executed = new List<string>();

        BuilderMixins.ForPlatforms(
            builder,
            b =>
            {
                _ = b;
                executed.Add("first");
            },
            b =>
            {
                _ = b;
                executed.Add("second");
            });

        await Assert.That(executed).IsEquivalentTo(["first", "second"]);
    }

    /// <summary>
    /// Verifies that configuring the message bus registers the configured instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConfigureMessageBus_registers_configured_instance()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.WithMessageBus(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(Locator.Current.GetService<IMessageBus>()).IsAssignableTo<MessageBus>();
            await Assert.That(configured).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that configuring the view locator registers the configured locator.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConfigureViewLocator_registers_configured_locator()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var configured = false;

        BuilderMixins.ConfigureViewLocator(builder, _ => configured = true);
        builder.WithCoreServices().Build();

        using (Assert.Multiple())
        {
            await Assert.That(Locator.Current.GetService<IViewLocator>()).IsAssignableTo<DefaultViewLocator>();
            await Assert.That(configured).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that configuring the suspension driver invokes the action with the registered driver.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConfigureSuspensionDriver_invokes_action_when_driver_registered()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var driver = new TestSuspensionDriver();
        Locator.CurrentMutable.RegisterConstant(driver, typeof(ISuspensionDriver));
        ISuspensionDriver? observed = null;

        BuilderMixins.ConfigureSuspensionDriver(builder, d => observed = d);
        builder.WithCoreServices().Build();

        await Assert.That(observed).IsSameReferenceAs(driver);
    }

    /// <summary>
    /// Verifies that registering a view model registers it as transient.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterViewModel_registers_transient_view_model()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.RegisterViewModel<BuilderMixinsTestViewModel>(builder);

        var first = Locator.Current.GetService<BuilderMixinsTestViewModel>();
        var second = Locator.Current.GetService<BuilderMixinsTestViewModel>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(second).IsNotNull();
            await Assert.That(first).IsNotSameReferenceAs(second);
        }
    }

    /// <summary>
    /// Verifies that registering a singleton view model returns the same instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterSingletonViewModel_registers_singleton_instance()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonViewModel<BuilderMixinsTestViewModel>(builder);

        var first = Locator.Current.GetService<BuilderMixinsTestViewModel>();
        var second = Locator.Current.GetService<BuilderMixinsTestViewModel>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(first).IsSameReferenceAs(second);
        }
    }

    /// <summary>
    /// Verifies that registering a view registers it as transient.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterView_registers_transient_view()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.RegisterView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = Locator.Current.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = Locator.Current.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(second).IsNotNull();
            await Assert.That(first).IsNotSameReferenceAs(second);
        }
    }

    /// <summary>
    /// Verifies that registering a singleton view returns the same instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterSingletonView_registers_singleton_view()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        BuilderMixins.RegisterSingletonView<BuilderMixinsTestView, BuilderMixinsTestViewModel>(builder);

        var first = Locator.Current.GetService<IViewFor<BuilderMixinsTestViewModel>>();
        var second = Locator.Current.GetService<IViewFor<BuilderMixinsTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotNull();
            await Assert.That(first).IsSameReferenceAs(second);
        }
    }

    // Additional coverage tests for WithInstance overloads

    /// <summary>
    /// Verifies that the single-parameter WithInstance invokes the action with the resolved instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_SingleParameter_InvokesActionWithResolvedInstance()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        Locator.CurrentMutable.RegisterConstant("test-value", typeof(string));

        string? captured = null;
        builder.WithInstance<string>(value => captured = value);
        builder.WithCoreServices().Build();

        await Assert.That(captured).IsEqualTo("test-value");
    }

    /// <summary>
    /// Verifies that the two-parameter WithInstance invokes the action with both resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_TwoParameters_InvokesActionWithBothInstances()
    {
        const int ExpectedIntInstance = 42;
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        Locator.CurrentMutable.RegisterConstant("string-value", typeof(string));
        Locator.CurrentMutable.RegisterConstant(ExpectedIntInstance, typeof(int));

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
            await Assert.That(capturedInt).IsEqualTo(ExpectedIntInstance);
        }
    }

    /// <summary>
    /// Verifies that WithInstance throws <see cref="ArgumentNullException"/> when the builder is null.
    /// </summary>
    [Test]
    public void WithInstance_WithNullBuilder_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() =>
            BuilderMixins.WithInstance<string>(null!, _ => { }));

    /// <summary>
    /// Verifies that RegisterViews throws <see cref="ArgumentNullException"/> when the builder is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterViews_WithNullBuilder_ThrowsArgumentNullException() =>
        await Assert.That(() => BuilderMixins.RegisterViews(null!, _ => { }))
            .Throws<ArgumentNullException>();

    /// <summary>
    /// Verifies that RegisterViews throws <see cref="ArgumentNullException"/> when the configure action is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterViews_WithNullConfigure_ThrowsArgumentNullException()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        await Assert.That(() => builder.RegisterViews(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that RegisterViews returns the same builder when a view locator is registered.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisterViews_WithViewLocator_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        // WithCoreServices registers the DefaultViewLocator
        builder.WithCoreServices().Build();

        var result = builder.RegisterViews(views =>
            views.Map<BuilderMixinsTestViewModel, BuilderMixinsTestView>());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that WithViewModule throws <see cref="ArgumentNullException"/> when the builder is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithViewModule_WithNullBuilder_ThrowsArgumentNullException() =>
        await Assert.That(() => BuilderMixins.WithViewModule<TestViewModule>(null!))
            .Throws<ArgumentNullException>();

    /// <summary>
    /// Verifies that WithViewModule returns the same builder for chaining.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithViewModule_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        // WithCoreServices registers the DefaultViewLocator
        builder.WithCoreServices().Build();

        var result = builder.WithViewModule<TestViewModule>();

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that BuildApp throws <see cref="ArgumentNullException"/> when the builder is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildApp_WithNullBuilder_ThrowsArgumentNullException() =>
        await Assert.That(() => BuilderMixins.BuildApp(null!))
            .Throws<ArgumentNullException>();

    /// <summary>
    /// Verifies that BuildApp throws <see cref="InvalidOperationException"/> when the builder is not a ReactiveUI builder.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildApp_WithNonReactiveUIBuilder_ThrowsInvalidOperationException()
    {
        var nonReactiveBuilder = new NonReactiveUIAppBuilder();

        var ex = await Assert.That(() => nonReactiveBuilder.BuildApp())
            .Throws<InvalidOperationException>();

        await Assert.That(ex).IsNotNull();
        await Assert.That(ex.Message).Contains("not an IReactiveUIBuilder");
    }

    /// <summary>
    /// Verifies that BuildApp builds successfully for a ReactiveUI builder and returns it.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildApp_WithReactiveUIBuilder_BuildsSuccessfully()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithCoreServices();

        var result = BuilderMixins.BuildApp(builder);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that WithMessageBus throws <see cref="ArgumentNullException"/> when the builder is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithMessageBus_WithNullBuilder_ThrowsArgumentNullException()
    {
        var messageBus = new MessageBus();

        await Assert.That(() => BuilderMixins.WithMessageBus(null!, messageBus))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that WithMessageBus registers the supplied custom message bus.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithMessageBus_RegistersCustomMessageBus()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var customMessageBus = new MessageBus();

        BuilderMixins.WithMessageBus(builder, customMessageBus);
        builder.WithCoreServices().Build();

        var registered = Locator.Current.GetService<IMessageBus>();
        await Assert.That(registered).IsSameReferenceAs(customMessageBus);
    }

    /// <summary>
    /// Verifies that setting the task pool scheduler with setRxApp false leaves <see cref="RxSchedulers"/> unchanged.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithTaskPoolScheduler_WithSetRxAppFalse_DoesNotSetRxSchedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            // Set a known baseline scheduler
            var baselineScheduler = CurrentThreadScheduler.Instance;
            RxSchedulers.TaskpoolScheduler = baselineScheduler;

            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithTaskPoolScheduler(scheduler, false);
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

    /// <summary>
    /// Verifies that setting the main thread scheduler with setRxApp false leaves <see cref="RxSchedulers"/> unchanged.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithMainThreadScheduler_WithSetRxAppFalse_DoesNotSetRxSchedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            // Set a known baseline scheduler
            var baselineScheduler = CurrentThreadScheduler.Instance;
            RxSchedulers.MainThreadScheduler = baselineScheduler;

            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithMainThreadScheduler(scheduler, false);
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

    /// <summary>
    /// Executor that only resets state, leaving builder configuration to each test.
    /// </summary>
    internal sealed class ResetOnlyExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            // Tests in this class configure and build the builder themselves.
        }
    }

    /// <summary>
    /// Test view module that maps a test view model to a test view.
    /// </summary>
    private sealed class TestViewModule : IViewModule
    {
        /// <inheritdoc/>
        public void RegisterViews(DefaultViewLocator locator) =>
            locator.Map<BuilderMixinsTestViewModel, BuilderMixinsTestView>(() => new());
    }

    /// <summary>
    /// App builder that does not implement <see cref="IReactiveUIBuilder"/>, used to verify error handling.
    /// </summary>
    private sealed class NonReactiveUIAppBuilder : IAppBuilder
    {
        /// <inheritdoc/>
        public IAppInstance Build() => new TestAppInstance();

        /// <inheritdoc/>
        public IAppBuilder UseCurrentSplatLocator() => this;

        /// <inheritdoc/>
        public IAppBuilder UsingModule<T>(T module)
            where T : IModule
        {
            _ = module;
            return this;
        }

        /// <inheritdoc/>
        public IAppBuilder WithCoreServices() => this;

        /// <inheritdoc/>
        public IAppBuilder WithCustomRegistration(Action<IMutableDependencyResolver> action)
        {
            _ = action;
            return this;
        }
    }

    /// <summary>
    /// Minimal app instance returned by <see cref="NonReactiveUIAppBuilder"/>.
    /// </summary>
    private sealed class TestAppInstance : IAppInstance, IDisposable
    {
        /// <inheritdoc/>
        public IReadonlyDependencyResolver Current => throw new NotSupportedException();

        /// <inheritdoc/>
        public IMutableDependencyResolver CurrentMutable => throw new NotSupportedException();

        /// <inheritdoc/>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required by interface")]
        public void Dispose()
        {
            // No-op: test stub holds no resources to release.
        }
    }

    /// <summary>
    /// Test view model used in builder registration tests.
    /// </summary>
    [SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class BuilderMixinsTestViewModel : ReactiveObject;

    /// <summary>
    /// Test view bound to <see cref="BuilderMixinsTestViewModel"/>.
    /// </summary>
    private sealed class BuilderMixinsTestView : IViewFor<BuilderMixinsTestViewModel>
    {
        /// <summary>
        /// Gets or sets the strongly typed view model bound to this view.
        /// </summary>
        public BuilderMixinsTestViewModel? ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the weakly typed view model bound to this view.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (BuilderMixinsTestViewModel?)value;
        }
    }

    /// <summary>
    /// Marker service registered by platform registration tests.
    /// </summary>
    [SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class PlatformRegistrationMarker;

    /// <summary>
    /// Marker service registered by Splat module tests.
    /// </summary>
    [SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class SplatModuleMarker;

    /// <summary>
    /// Test registration module that registers a <see cref="PlatformRegistrationMarker"/>.
    /// </summary>
    private sealed class TestRegistrationModule : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(IRegistrar registrar) =>
            registrar.RegisterConstant<PlatformRegistrationMarker>(() => new());
    }

    /// <summary>
    /// Test Splat module that registers a <see cref="SplatModuleMarker"/> and records that it ran.
    /// </summary>
    private sealed class TestSplatModule : IModule
    {
        /// <summary>
        /// Gets a value indicating whether the module has been registered.
        /// </summary>
        public bool Registered { get; private set; }

        /// <inheritdoc/>
        public void Configure(IMutableDependencyResolver services)
        {
            services.RegisterConstant(new SplatModuleMarker(), typeof(SplatModuleMarker));
            Registered = true;
        }

        /// <summary>
        /// Registers the module's services into the supplied dependency resolver.
        /// </summary>
        /// <param name="services">The mutable dependency resolver to register services into.</param>
        /// <param name="resolver">The readonly dependency resolver used to resolve existing services.</param>
        public void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver? resolver)
        {
            _ = resolver;
            Configure(services);
        }
    }

    /// <summary>
    /// Test suspension driver that returns empty state observables.
    /// </summary>
    private sealed class TestSuspensionDriver : ISuspensionDriver
    {
        /// <inheritdoc/>
        public IObservable<object?> LoadState() => Observable.Return<object?>(null);

        /// <inheritdoc/>
        public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
        {
            _ = typeInfo;
            return Observable.Return<T?>(default);
        }

        /// <inheritdoc/>
        public IObservable<Unit> SaveState<T>(T state)
        {
            _ = state;
            return Observable.Return(Unit.Default);
        }

        /// <inheritdoc/>
        public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
        {
            _ = state;
            _ = typeInfo;
            return Observable.Return(Unit.Default);
        }

        /// <inheritdoc/>
        public IObservable<Unit> InvalidateState() => Observable.Return(Unit.Default);
    }
}
