// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// UIKit platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
[Preserve(AllMembers = true)]
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    [SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Marked as Preserve")]
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentNullException.ThrowIfNull(registerFunction);

        registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new UIKitObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(() => new UIKitCommandBinders(), typeof(ICreatesCommandBinding));
        registerFunction(() => new DateTimeNSDateConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));

        if (!ModeDetector.InUnitTestRunner())
        {
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
        }

        registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
    }
}
