// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>Demonstrates using ReactiveUI schedulers without RequiresUnreferencedCode attributes.</summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public class SchedulerConsumptionTest
{
    /// <summary>Verifies that ReactiveProperty factory methods work using RxSchedulers internally.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactivePropertyFactoryMethodsWork()
    {
        const int ExpectedValue = 42;

        // These factory methods use RxSchedulers internally, so no RequiresUnreferencedCode needed
        var prop1 = ReactiveProperty<string>.Create();
        var prop2 = ReactiveProperty<string>.Create("initial");
        var prop3 = ReactiveProperty<int>.Create(ExpectedValue, false, true);

        using (Assert.Multiple())
        {
            await Assert.That(prop1).IsNotNull();
            await Assert.That(prop2).IsNotNull();
            await Assert.That(prop3).IsNotNull();

            await Assert.That(prop1.Value).IsNull();
            await Assert.That(prop2.Value).IsEqualTo("initial");
            await Assert.That(prop3.Value).IsEqualTo(ExpectedValue);
        }
    }

    /// <summary>Verifies that a repository can use RxSchedulers without requiring attributes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task RepositoryCanUseSchedulersWithoutAttributes()
    {
        var testScheduler = TestContext.Current.GetVirtualTimeScheduler();
        var originalScheduler = RxSchedulers.TaskpoolScheduler;

        try
        {
            // Use test scheduler for predictable behavior
            RxSchedulers.TaskpoolScheduler = testScheduler;

            var repository = new ExampleRepository();
            string? result = null;

            using var subscription = repository.GetData().Subscribe(data => result = data);
            repository.PublishData("test");

            // Advance the test scheduler to process the observable
            testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            await Assert.That(result).IsEqualTo("Processed: test");
        }
        finally
        {
            // Restore original scheduler
            RxSchedulers.TaskpoolScheduler = originalScheduler;
        }
    }

    /// <summary>Verifies that a view model can use RxSchedulers without requiring attributes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ViewModelCanUseSchedulersWithoutAttributes()
    {
        var testScheduler = TestContext.Current.GetVirtualTimeScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        try
        {
            // Use test scheduler for predictable behavior
            RxSchedulers.MainThreadScheduler = testScheduler;

            var viewModel = new ExampleViewModel { Name = "ReactiveUI" };

            // Advance the test scheduler to process the observable
            testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

            // For this test, we're primarily verifying that the code compiles and runs
            // without requiring RequiresUnreferencedCode attributes on the test method
            await Assert.That(viewModel.Name).IsEqualTo("ReactiveUI");
        }
        finally
        {
            // Restore original scheduler
            RxSchedulers.MainThreadScheduler = originalScheduler;
        }
    }

    /// <summary>Example repository class that uses RxSchedulers without requiring attributes.</summary>
    private sealed class ExampleRepository : IDisposable
    {
        /// <summary>The subject used to publish data.</summary>
        private readonly Signal<string> _dataSubject = new();

        /// <inheritdoc/>
        public void Dispose() => _dataSubject?.Dispose();

        /// <summary>Gets an observable stream of processed data.</summary>
        /// <returns>An observable sequence of processed data strings.</returns>
        public IObservable<string> GetData() => _dataSubject.ObserveOn((ISequencer)RxSchedulers.TaskpoolScheduler).Select(data => $"Processed: {data}");

        /// <summary>Publishes a data value to the repository.</summary>
        /// <param name="data">The data to publish.</param>
        public void PublishData(string data) => _dataSubject.OnNext(data);
    }

    /// <summary>
    ///     Example ViewModel that uses RxSchedulers without requiring attributes.
    ///     This would previously require RequiresUnreferencedCode when using RxApp schedulers.
    /// </summary>
    private sealed class ExampleViewModel : ReactiveObject
    {
        /// <summary>The output property helper backing the <see cref="Greeting" /> property.</summary>
        private readonly ObservableAsPropertyHelper<string> _greeting;

        /// <summary>Initializes a new instance of the <see cref="ExampleViewModel" /> class.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S3366:Make sure the use of this in constructors is safe here",
            Justification = "OAPH initialization requires 'this' in the constructor; single-threaded test fixture.")]
        public ExampleViewModel() => _greeting = this.WhenAnyValue(x => x.Name)
            .Select(name => $"Hello, {name ?? "World"}!")
            .ObserveOn((ISequencer)RxSchedulers.MainThreadScheduler)
            .ToProperty(this, nameof(Greeting), scheduler: RxSchedulers.MainThreadScheduler);

        /// <summary>Gets the greeting derived from the name.</summary>
        public string Greeting => _greeting.Value;

        /// <summary>Gets or sets the name.</summary>
        public string? Name
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
