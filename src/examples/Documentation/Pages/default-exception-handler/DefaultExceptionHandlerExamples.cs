// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Primitives.Advanced;

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>Shows what happens to a command error nobody watches, and how an app replaces the handler it reaches.</summary>
public static class DefaultExceptionHandlerExamples
{
    /// <summary>A command error nobody subscribes to through <c>ThrownExceptions</c> reaches the installed handler.</summary>
    /// <returns>A task that completes once the refused load has been reported.</returns>
    public static async Task ReplaceTheDefaultExceptionHandler()
    {
        List<string> failures = [];
        IObserver<Exception> handler = Witness.Create<Exception>(error => failures.Add(error.Message));
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithExceptionHandler(handler).BuildApp();

        Console.WriteLine(ReferenceEquals(RxState.DefaultExceptionHandler, handler));

        BankAccountService server = new() { ServerIsDown = true };
        using ReactiveCommand<string, decimal> loadBalance = ReactiveCommand.CreateFromObservable<string, decimal>(server.LoadBalance);

        try
        {
            _ = await loadBalance.Execute("checking-01");
        }
        catch (AccountServiceException)
        {
            // The caller sees the error too; nobody subscribed to loadBalance.ThrownExceptions, so it also reached the handler above.
        }

        Console.WriteLine(failures[0]);

        // Output:
        // True
        // The bank server could not load account 'checking-01'.
    }

    /// <summary>Left unreplaced, the default handler breaks into the debugger (if attached) and throws this
    /// exception; application code can construct the same type for its own unrecoverable failures.</summary>
    public static void ConstructAnUnhandledErrorException()
    {
        UnhandledErrorException bare = new();
        UnhandledErrorException withMessage = new("The bank server never replied.");
        AccountServiceException connectionReset = new("Connection reset.");
        UnhandledErrorException withInner = new("Could not load the account balance.", connectionReset);

        Console.WriteLine(bare.Message);
        Console.WriteLine(withMessage.Message);
        Console.WriteLine(withInner.Message);
        Console.WriteLine(withInner.InnerException?.Message);

        // Output:
        // Exception of type 'ReactiveUI.UnhandledErrorException' was thrown.
        // The bank server never replied.
        // Could not load the account balance.
        // Connection reset.
    }
}
