// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows registering an <see cref="ILogger"/> so framework messages, sent to a null logger by default, are written somewhere the app can see.</summary>
public static class EnableFrameworkLoggingExamples
{
    /// <summary>Registering a logger at the composition root lets any <see cref="IEnableLogger"/> write through it with <c>Log</c>.</summary>
    public static void PreferRegisteringALogger()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Info });

        SchoolLog log = new();
        using IDisposable subscription = Signal.Emit("Robotics Club")
            .Log(log, "Roster loaded")
            .Subscribe(static _ => { });

        // Output:
        // SchoolLog: Roster loaded OnNext: Robotics Club
        // SchoolLog: Roster loaded OnCompleted
    }

    /// <summary>A marker type that does nothing but let its stream write through the registered logger.</summary>
    private sealed class SchoolLog : IEnableLogger;
}
