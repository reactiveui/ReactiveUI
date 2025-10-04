// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if IS_WINUI
namespace ReactiveUI.WinUI;
#endif
#if IS_MAUI
namespace ReactiveUI.Maui;
#endif

/// <summary>
/// The main registration for common classes for the Splat dependency injection.
/// We have code that runs reflection through the different ReactiveUI classes
/// searching for IWantsToRegisterStuff and will register all our required DI
/// interfaces. The registered items in this classes are common for all Platforms.
/// To get these registrations after the main ReactiveUI Initialization use the
/// DependencyResolverMixins.InitializeReactiveUI() extension method.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Register uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("Register uses methods that may require unreferenced code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentNullException.ThrowIfNull(registerFunction);

        registerFunction(static () => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(static () => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));

#if WINUI_TARGET
        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(static () => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(static () => DispatcherQueueScheduler.Current);
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
        }

        RxApp.SuppressViewCommandBindingMessage = true;
#endif
    }
}
