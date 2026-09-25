// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.CommandBinding;

/// <summary>Exercises generated Binding command calls with ReactiveUI commands and a notifying view.</summary>
public class CommandBindingIntegrationTests
{
    /// <summary>The event used by the control under test.</summary>
    private const string ClickEvent = "Click";

    /// <summary>The supported command-parameter sources.</summary>
    public enum ParameterSource
    {
        /// <summary>No command parameter.</summary>
        None = 0,

        /// <summary>An observable command parameter.</summary>
        Observable = 1,

        /// <summary>A view-model property command parameter.</summary>
        Expression = 2,
    }

    /// <summary>Verifies generated command binding executes for each parameter source.</summary>
    /// <param name="parameterSource">The parameter source to bind.</param>
    /// <returns>A task representing the asynchronous test operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The parameter source is unknown.</exception>
    [Test]
    [Arguments(ParameterSource.None)]
    [Arguments(ParameterSource.Observable)]
    [Arguments(ParameterSource.Expression)]
    public async Task BindCommand_ParameterSource_Executes(ParameterSource parameterSource)
    {
        using var viewModel = new FakeViewModel();
        var view = new FakeView { ViewModel = viewModel };
        var invocationCount = 0;
        using var execution = viewModel.Command.Subscribe(_ => invocationCount++);

        using var binding = parameterSource switch
        {
            ParameterSource.None => view.BindCommand(viewModel, static model => model.Command, static target => target.Control, ClickEvent),
            ParameterSource.Observable => view.BindCommand(
                viewModel,
                static model => model.Command,
                static target => target.Control,
                Signal.Emit<RxVoid?>(default),
                ClickEvent),
            ParameterSource.Expression => view.BindCommand(
                viewModel,
                static model => model.Command,
                static target => target.Control,
                static model => model.Parameter,
                ClickEvent),
            _ => throw new ArgumentOutOfRangeException(nameof(parameterSource), parameterSource, null),
        };

        view.Control.RaiseClick();

        await Assert.That(invocationCount).IsEqualTo(1);
    }

    /// <summary>A control exposing a bindable event.</summary>
    public sealed class FakeControl
    {
        /// <summary>Occurs when the control is clicked.</summary>
        public event EventHandler? Click;

        /// <summary>Raises <see cref="Click"/>.</summary>
        public void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>A notifying view that owns the control.</summary>
    public sealed class FakeView : ReactiveObject, IViewFor<FakeViewModel>
    {
        /// <summary>Gets the bound control.</summary>
        public FakeControl Control { get; } = new();

        /// <summary>Gets or sets the view model.</summary>
        public FakeViewModel? ViewModel
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeViewModel?)value;
        }
    }

    /// <summary>A notifying view model that owns the command.</summary>
    public sealed class FakeViewModel : ReactiveObject, IDisposable
    {
        /// <summary>Gets the command under test.</summary>
        public ReactiveCommand<RxVoid, RxVoid> Command { get; } = ReactiveCommand.Create(static () => { }, outputScheduler: Sequencer.Immediate);

        /// <summary>Gets the expression-backed parameter.</summary>
        public RxVoid Parameter => default;

        /// <inheritdoc/>
        public void Dispose() => Command.Dispose();
    }
}
