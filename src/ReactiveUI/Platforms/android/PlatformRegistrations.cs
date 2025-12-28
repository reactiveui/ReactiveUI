// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI;

/// <summary>
/// Android platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [SuppressMessage("Trimming", "IL2046:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Not using reflection")]
    [SuppressMessage("AOT", "IL3051:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Not using reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction) // TODO: Create Test
    {
        ArgumentExceptionHelper.ThrowIfNull(registerFunction);

        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(static () => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new AndroidCommandBinders(), typeof(ICreatesCommandBinding));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxSchedulers.MainThreadScheduler = HandlerScheduler.MainThreadScheduler;
        }

        registerFunction(static () => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
    }
}
