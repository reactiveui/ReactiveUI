// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.SourceGenerators;

namespace ReactiveUI.Tests.SourceGenerators;

/// <summary>
/// Checks that the ReactiveUI.SourceGenerators generators reach a project built on ReactiveUI, and that their
/// output works with this flavour of ReactiveUI and with ReactiveUI.Binding's generated observation.
/// </summary>
public partial class SourceGeneratorsIntegrationTests
{
    /// <summary>The first name every fixture starts with.</summary>
    private const string FirstName = "Ada";

    /// <summary>The value the load command starts from.</summary>
    private const int LoadInput = 20;

    /// <summary>How long a test waits for an asynchronous command before it fails.</summary>
    private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(10);

    /// <summary>A generated reactive property raises changing and then changed.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ReactiveProperty_RaisesChangingThenChanged()
    {
        var fixture = new PersonViewModel();
        var events = new List<string>();
        fixture.PropertyChanging += (_, e) => events.Add($"changing:{e.PropertyName}");
        fixture.PropertyChanged += (_, e) => events.Add($"changed:{e.PropertyName}");

        fixture.LastName = "Lovelace";

        await Assert.That(events).Contains($"changing:{nameof(PersonViewModel.LastName)}");
        await Assert.That(events).Contains($"changed:{nameof(PersonViewModel.LastName)}");
        await Assert.That(events.IndexOf($"changing:{nameof(PersonViewModel.LastName)}"))
            .IsLessThan(events.IndexOf($"changed:{nameof(PersonViewModel.LastName)}"));
    }

    /// <summary>A binding-generated helper follows generated reactive properties through WhenAnyValue.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ObservableAsProperty_FollowsReactiveProperties()
    {
        var fixture = new PersonViewModel { LastName = "Lovelace" };

        await Assert.That(fixture.FullName).IsEqualTo("Ada Lovelace");

        fixture.LastName = "Byron";

        await Assert.That(fixture.FullName).IsEqualTo("Ada Byron");
    }

    /// <summary>A generated command runs its method and follows its can-execute observable.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ReactiveCommand_RunsMethodAndFollowsCanExecute()
    {
        var fixture = new PersonViewModel();
        var canExecute = new List<bool>();
        using var canExecuteSubscription = fixture.ClearCommand.CanExecute.Subscribe(canExecute.Add);

        using (fixture.ClearCommand.Execute().Subscribe(static _ => { }))
        {
            await Assert.That(fixture.FirstName).IsEqualTo(string.Empty);
        }

        await Assert.That(canExecute).IsEquivalentTo([true, false]);
    }

    /// <summary>A command generated from an asynchronous method drops the Async suffix and returns the result.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ReactiveCommand_FromAsyncMethod_ReturnsResult()
    {
        var fixture = new PersonViewModel();
        var result = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        using (fixture.LoadCommand.Execute(LoadInput).Subscribe(value => result.TrySetResult(value), error => result.TrySetException(error)))
        {
            await Assert.That(await result.Task.WaitAsync(CommandTimeout)).IsEqualTo(LoadInput + 1);
        }
    }

    /// <summary>A view model whose properties and commands come from the generators.</summary>
    internal sealed partial class PersonViewModel : ReactiveObject
    {
        /// <summary>Whether a first name is set, which enables the clear command.</summary>
        private readonly IObservable<bool> _hasFirstName;

        /// <summary>Initializes a new instance of the <see cref="PersonViewModel"/> class.</summary>
        public PersonViewModel()
        {
            _fullNameHelper = this.WhenAnyValue(static x => x.FirstName, static x => x.LastName, static (first, last) => $"{first} {last}")
                .ToProperty(this, static x => x.FullName);
            _hasFirstName = this.WhenAnyValue(static x => x.FirstName).Select(static name => name.Length > 0);
        }

        /// <summary>Gets or sets the first name.</summary>
        [Reactive]
        public partial string FirstName { get; set; } = SourceGeneratorsIntegrationTests.FirstName;

        /// <summary>Gets or sets the last name.</summary>
        [Reactive]
        public partial string LastName { get; set; } = string.Empty;

        /// <summary>Gets the first and last name together.</summary>
        [ObservableAsProperty]
        public partial string FullName { get; }

        /// <summary>Loads the value after the one given.</summary>
        /// <param name="id">The value to start from.</param>
        /// <param name="cancellationToken">A token that cancels the load.</param>
        /// <returns>The value after <paramref name="id"/>.</returns>
        [ReactiveCommand]
        private static async Task<int> LoadAsync(int id, CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            return id + 1;
        }

        /// <summary>Clears the first name.</summary>
        [ReactiveCommand(CanExecute = nameof(_hasFirstName))]
        private void Clear() => FirstName = string.Empty;
    }
}
