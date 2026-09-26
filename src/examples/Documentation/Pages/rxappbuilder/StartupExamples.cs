// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Primitives.Advanced;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Shows the whole life cycle of starting ReactiveUI with the builder: a guard that fires before the app is built,
/// building the recipe book app end to end, then resolving what it registered. <see cref="BuildTheRecipeBookApp"/>
/// is the only method on this page that calls <c>BuildApp</c>, as an app does once at start-up. Only the first build
/// in a process applies, so every other example configures a builder of its own and stops short of building it.
/// </summary>
public static class StartupExamples
{
    /// <summary><c>EnsureInitialized</c> fails fast when nothing has called <c>BuildApp</c> yet.</summary>
    public static void EnsureInitializedThrowsBeforeBuild()
    {
        try
        {
            RxAppBuilder.EnsureInitialized();
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine(exception.GetType().Name);
        }

        // Output:
        // InvalidOperationException
    }

    /// <summary>
    /// Builds the recipe book app: its platform services, its schedulers, its exception handler, its message bus,
    /// its lifecycle settings, two plain Splat modules, its views and view models, and a print layout on its view locator.
    /// </summary>
    /// <returns>The built app, so later examples on this page can resolve what it registered.</returns>
    public static ReactiveUIBuilder BuildTheRecipeBookApp()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithRegistration(new PantryRegistrations());

        MessageBus kitchenEvents = new();
        using IDisposable subscription = kitchenEvents.Listen<string>().Subscribe(Console.WriteLine);

        _ = builder
            .WithPlatformServices()
            .WithMainThreadScheduler(Sequencer.Immediate)
            .WithTaskPoolScheduler(TaskPoolSequencer.Default, false)
            .WithExceptionHandler(Witness.Create<Exception>(static error => Console.WriteLine($"error: {error.Message}")))
            .WithMessageBus(kitchenEvents)
            .WithCacheSizes(32, 128)
            .WithSuspensionHost<RecipeBookState>()
            .UsingSplatModule(new PantryModule())
            .UsingSplatBuilder(static appBuilder => appBuilder.UsingModule(new SpiceRackModule()))
            .WithRegistrationOnBuild(static resolver => resolver.RegisterConstant<IPantryClock>(new PantryClock()))
            .RegisterView<RecipeBookView, RecipeBookViewModel>()
            .RegisterSingletonView<ShoppingListView, ShoppingListViewModel>()
            .RegisterViewModel<IngredientListViewModel>()
            .RegisterConstantViewModel<AppSettingsViewModel>()
            .RegisterSingletonViewModel<PantryViewModel>()
            .ConfigureViewLocator(static locator => locator.Map<RecipeBookViewModel, PrintableRecipeBookView>("Print"))
            .ConfigureSuspensionDriver(static driver =>
            {
                using IDisposable invalidated = driver.InvalidateState().Subscribe();
            })
            .BuildApp();

        // Views and view models resolved after BuildApp() finished, so the view locator it registered is ready.
        _ = builder
            .RegisterViews(static views => views.Map<MealPlanViewModel, MealPlanView>())
            .WithViewModule<RecipeBookViewModule>();

        RxState.DefaultExceptionHandler.OnNext(new InvalidOperationException("Burnt the toast"));
        kitchenEvents.SendMessage("Sunday roast is in the oven");

        return builder;

        // Output:
        // error: Burnt the toast
        // Sunday roast is in the oven
    }

    /// <summary>Confirms that <c>WithCacheSizes</c>, <c>WithSuspensionHost</c> and <c>BuildApp</c> initialized ReactiveUI process-wide.</summary>
    public static void ConfirmTheAppInitialized()
    {
        bool initializedWithoutError = true;
        try
        {
            RxAppBuilder.EnsureInitialized();
        }
        catch (InvalidOperationException)
        {
            initializedWithoutError = false;
        }

        Console.WriteLine(initializedWithoutError);
        Console.WriteLine(RxCacheSize.SmallCacheLimit);
        Console.WriteLine(RxCacheSize.BigCacheLimit);
        Console.WriteLine(RxSuspension.SuspensionHost.GetType().IsGenericType);

        // Output:
        // True
        // 32
        // 128
        // True
    }

    /// <summary>Resolves the suspension driver and the views and view models <see cref="BuildTheRecipeBookApp"/> registered.</summary>
    /// <param name="builder">The app <see cref="BuildTheRecipeBookApp"/> built.</param>
    public static void ResolveRecipeBookViewsAndViewModels(ReactiveUIBuilder builder)
    {
        _ = builder.WithInstance<ISuspensionDriver>(static driver => Console.WriteLine(driver?.GetType().Name));
        _ = builder.WithInstance<IViewFor<RecipeBookViewModel>>(static view => Console.WriteLine(view?.GetType().Name));
        _ = builder.WithInstance<IViewFor<ShoppingListViewModel>>(static view => Console.WriteLine(view?.GetType().Name));
        _ = builder.WithInstance<IngredientListViewModel, AppSettingsViewModel, PantryViewModel>(
            static (ingredients, settings, pantry) =>
            {
                Console.WriteLine(ingredients?.Count);
                Console.WriteLine(settings?.MetricUnits);
                Console.WriteLine(pantry?.ItemsInStock);
            });

        // Output:
        // InMemorySuspensionDriver
        // RecipeBookView
        // ShoppingListView
        // 0
        // True
        // 12
    }

    /// <summary>
    /// Resolves the views <c>ConfigureViewLocator</c>, <c>RegisterViews</c> and <c>WithViewModule</c> mapped, and the
    /// services the two plain Splat modules registered.
    /// </summary>
    /// <param name="builder">The app <see cref="BuildTheRecipeBookApp"/> built.</param>
    public static void ResolveRecipeBookModulesAndViews(ReactiveUIBuilder builder)
    {
        IViewLocator? locator = null;
        _ = builder.WithInstance<IViewLocator>(resolved => locator = resolved);
        Console.WriteLine(locator?.ResolveView(new RecipeBookViewModel(), "Print")?.GetType().Name);
        Console.WriteLine(locator?.ResolveView(new MealPlanViewModel(), null)?.GetType().Name);
        Console.WriteLine(locator?.ResolveView(new PantryViewModel(), null)?.GetType().Name);

        _ = builder.WithInstance<IIngredientCatalog, ISpiceRack, IPantryClock>(
            static (catalog, spices, clock) =>
            {
                Console.WriteLine(catalog?.Ingredients.Count);
                Console.WriteLine(spices?.Spices.Count);
                Console.WriteLine(clock is not null);
            });

        // Output:
        // PrintableRecipeBookView
        // MealPlanView
        // PantryView
        // 3
        // 2
        // True
    }
}
