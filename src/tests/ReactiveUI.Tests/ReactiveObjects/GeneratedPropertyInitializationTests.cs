// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>Generated getters expose synchronous helper values before their backing field is assigned.</summary>
public partial class GeneratedPropertyInitializationTests
{
    /// <summary>An existing listener reads the emitted value during activation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Activation_SynchronousSource_ListenerReadsCurrentValue()
    {
        using var fixture = new ActivatedViewModel();
        var observed = new List<bool>();
        fixture.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(ActivatedViewModel.Shown))
            {
                observed.Add(fixture.Shown);
            }
        };

        using (fixture.Activator.Activate())
        {
            await Assert.That(observed[^1]).IsTrue();
            await Assert.That(fixture.Shown).IsTrue();
        }
    }

    /// <summary>A dependent property follows a helper initialized later in the same constructor.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Constructor_DependentHelperInitializedFirst_ReceivesSourceValue()
    {
        using var fixture = new DependentViewModel();

        await Assert.That(fixture.Ready).IsTrue();
        await Assert.That(fixture.Waiting).IsFalse();
    }

    /// <summary>Assigns a generated property's helper whenever the view model activates.</summary>
    public sealed partial class ActivatedViewModel : ReactiveObject, IActivatableViewModel, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="ActivatedViewModel"/> class.</summary>
        public ActivatedViewModel() =>
            this.WhenActivated(disposables =>
                _shownHelper = Signal.Return(true).ToProperty(this, nameof(Shown)).DisposeWith(disposables));

        /// <inheritdoc/>
        public ViewModelActivator Activator { get; } = new();

        /// <summary>Gets the value emitted as activation subscribes.</summary>
        [ObservableAsProperty]
        public partial bool Shown { get; }

        /// <inheritdoc/>
        public void Dispose() => Activator.Dispose();
    }

    /// <summary>Initializes the dependent property before its synchronous source.</summary>
    public sealed partial class DependentViewModel : ReactiveObject, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="DependentViewModel"/> class.</summary>
        public DependentViewModel()
        {
            _waitingHelper = this.WhenAnyValue(static x => x.Ready, static ready => !ready)
                .ToProperty(this, static x => x.Waiting);
            _readyHelper = Signal.Return(true).ToProperty(this, static x => x.Ready);
        }

        /// <summary>Gets the source value assigned after the dependent helper.</summary>
        [ObservableAsProperty]
        public partial bool Ready { get; }

        /// <summary>Gets whether the source is not ready.</summary>
        [ObservableAsProperty]
        public partial bool Waiting { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _waitingHelper?.Dispose();
            _readyHelper?.Dispose();
        }
    }
}
