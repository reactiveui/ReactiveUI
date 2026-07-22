// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Maui;

/// <summary>A view model that handles user login with reactive validation and async execution.</summary>
public class LoginViewModel : ReactiveObject, IDisposable
{
    /// <summary>Cancellation source for the in-flight login operation, signalled by the <see cref="Cancel"/> command.</summary>
    private CancellationTokenSource? _loginCancellation;

    /// <summary>Initializes a new instance of the <see cref="LoginViewModel"/> class.</summary>
    /// <param name="scheduler">The scheduler to use for command execution.</param>
    public LoginViewModel(ISequencer scheduler)
    {
        var canLogin = this.WhenAnyValue(
            vm => vm.UserName,
            vm => vm.Password,
            static (user, pass) => !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass));

        Login = ReactiveCommand.CreateFromTask(
            async () =>
            {
                using var cancellation = new CancellationTokenSource();
                _loginCancellation = cancellation;
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellation.Token);
                    return Password is "secret";
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                finally
                {
                    _loginCancellation = null;
                }
            },
            canLogin,
            scheduler);

        Cancel = ReactiveCommand.Create(
            () => _loginCancellation?.Cancel(),
            Login.IsExecuting,
            scheduler);
    }

    /// <summary>Gets or sets the user name.</summary>
    public string? UserName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the password.</summary>
    public string? Password
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the login command. Returns true on success, false on failure.</summary>
    public ReactiveCommand<RxVoid, bool> Login { get; }

    /// <summary>Gets the cancel command. Only available while login is executing.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Cancel { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases managed resources.</summary>
    /// <param name="disposing">Whether to release managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Login.Dispose();
        Cancel.Dispose();
    }
}
