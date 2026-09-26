// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>Shows <c>Log</c> printing each notification and <c>LoggedCatch</c> falling back after a failed load.</summary>
public static class LoggingExamples
{
    /// <summary><c>Log</c> with no message prints every OnNext and the completion, using the object's own type name.</summary>
    public static void LogEachNotification()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Info });

        BankAccountService server = new();
        List<decimal> balances = [];

        using IDisposable subscription = server.LoadBalance("checking-01").Log(server).Subscribe(balances.Add);

        Console.WriteLine(balances[0]);

        // Output:
        // BankAccountService:  OnNext: 500.00
        // BankAccountService:  OnCompleted
        // 500.00
    }

    /// <summary>A message names which stream is logging; a stringifier controls how each value is written.</summary>
    public static void LogWithAMessageAndAStringifier()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Info });

        BankAccountService server = new();

        using IDisposable checking = server.LoadBalance("checking-01")
            .Log(server, "Checking balance")
            .Subscribe(static _ => { });

        using IDisposable savings = server.LoadBalance("savings-02")
            .Log(server, "Savings balance", static balance => $"${balance:F2}")
            .Subscribe(static _ => { });

        // Output:
        // BankAccountService: Checking balance OnNext: 500.00
        // BankAccountService: Checking balance OnCompleted
        // BankAccountService: Savings balance OnNext: $1200.50
        // BankAccountService: Savings balance OnCompleted
    }

    /// <summary>With no fallback signal, <c>LoggedCatch</c> logs the failure and emits the type's default value.</summary>
    public static void FallBackToADefaultValueAfterLogging()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Error });

        BankAccountService server = new() { ServerIsDown = true };
        List<decimal> balances = [];

        using IDisposable subscription = server.LoadBalance("checking-01")
            .LoggedCatch(server)
            .Subscribe(balances.Add);

        Console.WriteLine(balances[0]);

        // Output:
        // 0
    }

    /// <summary>A fallback signal continues in place of the failure; a message names the load for the warning that <c>LoggedCatch</c> writes.</summary>
    public static void FallBackToAPreviousBalanceAfterLogging()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Error });

        BankAccountService server = new() { ServerIsDown = true };
        IObservable<decimal> lastKnownBalance = Signal.Emit(475.00M);

        List<decimal> checking = [];
        using IDisposable first = server.LoadBalance("checking-01")
            .LoggedCatch(server, lastKnownBalance)
            .Subscribe(checking.Add);

        List<decimal> savings = [];
        using IDisposable second = server.LoadBalance("savings-02")
            .LoggedCatch(server, lastKnownBalance, "Loading savings balance")
            .Subscribe(savings.Add);

        Console.WriteLine(checking[0]);
        Console.WriteLine(savings[0]);

        // Output:
        // 475.00
        // 475.00
    }

    /// <summary>The typed overloads catch only the given exception type; a message names the load for the warning.</summary>
    public static void RecoverOnlyFromAccountServiceFailures()
    {
        AppLocator.CurrentMutable.RegisterConstant<ILogger>(new ConsoleLogger { Level = LogLevel.Error });

        BankAccountService server = new() { ServerIsDown = true };
        List<string> reasons = [];

        List<decimal> checking = [];
        using IDisposable first = server.LoadBalance("checking-01")
            .LoggedCatch(
                server,
                (AccountServiceException failure) =>
                {
                    reasons.Add(failure.Message);
                    return Signal.Emit(0M);
                })
            .Subscribe(checking.Add);

        List<decimal> savings = [];
        using IDisposable second = server.LoadBalance("savings-02")
            .LoggedCatch(
                server,
                static (AccountServiceException _) => Signal.Emit(0M),
                "Loading savings balance")
            .Subscribe(savings.Add);

        Console.WriteLine(checking[0]);
        Console.WriteLine(savings[0]);
        Console.WriteLine(reasons[0]);

        // Output:
        // 0
        // 0
        // The bank server could not load account 'checking-01'.
    }
}
