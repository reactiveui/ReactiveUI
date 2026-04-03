// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace ReactiveUI.Samples.Winforms;

/// <summary>
/// A view model that handles user login with reactive validation and async execution.
/// </summary>
public class LoginViewModel : ReactiveObject, IDisposable
{
    /// <summary>
    /// Signal used to cancel an in-flight login operation via TakeUntil.
    /// </summary>
    private readonly Subject<Unit> _cancelSignal = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="scheduler">The scheduler to use for command execution.</param>
    public LoginViewModel(IScheduler scheduler)
    {
        var canLogin = this.WhenAnyValue(
            vm => vm.UserName,
            vm => vm.Password,
            (user, pass) => !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass));

        Login = ReactiveCommand.CreateFromObservable(
            () => Observable
                .Return(Password is "secret")
                .Delay(TimeSpan.FromSeconds(1), scheduler)
                .TakeUntil(_cancelSignal),
            canLogin,
            scheduler);

        Cancel = ReactiveCommand.Create(
            () => _cancelSignal.OnNext(Unit.Default),
            Login.IsExecuting,
            scheduler);
    }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string? UserName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string? Password
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets the login command. Returns true on success, false on failure.
    /// </summary>
    public ReactiveCommand<Unit, bool> Login { get; }

    /// <summary>
    /// Gets the cancel command. Only available while login is executing.
    /// </summary>
    public ReactiveCommand<Unit, Unit> Cancel { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed resources.
    /// </summary>
    /// <param name="disposing">Whether to release managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancelSignal.Dispose();
            Login.Dispose();
            Cancel.Dispose();
        }
    }
}
