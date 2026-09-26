// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Shows configuration that scopes itself to a resolver of its own, rather than to the app-wide one <see
/// cref="RxAppBuilder"/> uses. A plugin host or a test fixture builds this way, so its registrations never leak.
/// </summary>
public static class IsolatedBuilderExamples
{
    /// <summary>
    /// <c>CreateReactiveUIBuilder</c> on an <see cref="IMutableDependencyResolver"/> scopes the whole builder to that
    /// resolver, so what it registers can be read back from the same resolver without ever calling <c>BuildApp</c>.
    /// </summary>
    public static void ConfigureAnIsolatedBuilder()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = resolver.CreateReactiveUIBuilder();

        _ = builder
            .WithRegistration(static mutable => mutable.RegisterConstant<IPantryClock>(new PantryClock()))
            .WithPlatformModule<PantryRegistrations>()
            .WithSuspensionHost();

        // ForCustomPlatform's platform-services action, like WithRegistrationOnBuild, only runs once BuildApp() does,
        // so this example checks the scheduler it sets immediately instead of the registration it defers.
        IReactiveUIBuilder customPlatform = builder.ForCustomPlatform(
            Sequencer.Immediate,
            static mutable => mutable.RegisterConstant<PlatformName>(new("Custom Console")));

        _ = customPlatform.ForPlatforms(
            static platform => platform.WithRegistration(static mutable => mutable.RegisterConstant("north", typeof(string), "compass")),
            static platform => platform.WithRegistration(static mutable => mutable.RegisterConstant("south", typeof(string), "compass")));

        _ = customPlatform.WithInstance<IPantryClock, ISuspensionDriver>(static (clock, driver) =>
        {
            Console.WriteLine(clock is not null);
            Console.WriteLine(driver?.GetType().Name);
        });
        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, Sequencer.Immediate));
        Console.WriteLine(resolver.GetServices<string>("compass").Count());

        // Output:
        // True
        // InMemorySuspensionDriver
        // True
        // 2
    }
}
