// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveUI.Tests;

/// <summary>
/// Demonstrates using ReactiveUI schedulers without RequiresUnreferencedCode attributes.
/// </summary>
public class SchedulerConsumptionTest
{
    [Test]
    public void ViewModelCanUseSchedulersWithoutAttributes()
    {
        var testScheduler = new TestScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        try
        {
            // Use test scheduler for predictable behavior
            RxSchedulers.MainThreadScheduler = testScheduler;

            var viewModel = new ExampleViewModel();
            viewModel.Name = "ReactiveUI";

            // Advance the test scheduler to process the observable
            testScheduler.AdvanceBy(1);

            // For this test, we're primarily verifying that the code compiles and runs
            // without requiring RequiresUnreferencedCode attributes on the test method
            Assert.That(viewModel.Name, Is.EqualTo("ReactiveUI"));
        }
        finally
        {
            // Restore original scheduler
            RxSchedulers.MainThreadScheduler = originalScheduler;
        }
    }

    [Test]
    public void RepositoryCanUseSchedulersWithoutAttributes()
    {
        var testScheduler = new TestScheduler();
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
            testScheduler.AdvanceBy(1);

            Assert.That(result, Is.EqualTo("Processed: test"));
        }
        finally
        {
            // Restore original scheduler
            RxSchedulers.TaskpoolScheduler = originalScheduler;
        }
    }

    [Test]
    public void ReactivePropertyFactoryMethodsWork()
    {
        // These factory methods use RxSchedulers internally, so no RequiresUnreferencedCode needed
        var prop1 = ReactiveProperty<string>.Create();
        var prop2 = ReactiveProperty<string>.Create("initial");
        var prop3 = ReactiveProperty<int>.Create(42, false, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(prop1, Is.Not.Null);
            Assert.That(prop2, Is.Not.Null);
            Assert.That(prop3, Is.Not.Null);

            Assert.That(prop1.Value, Is.Null);
            Assert.That(prop2.Value, Is.EqualTo("initial"));
            Assert.That(prop3.Value, Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Example repository class that uses RxSchedulers without requiring attributes.
    /// </summary>
    private sealed class ExampleRepository : IDisposable
    {
        private readonly Subject<string> _dataSubject = new();

        public IObservable<string> GetData()
        {
            // Using RxSchedulers instead of RxApp schedulers avoids RequiresUnreferencedCode
            return _dataSubject
                .ObserveOn(RxSchedulers.TaskpoolScheduler) // No RequiresUnreferencedCode needed!
                .Select(data => $"Processed: {data}");
        }

        public void PublishData(string data)
        {
            _dataSubject.OnNext(data);
        }

        public void Dispose()
        {
            _dataSubject?.Dispose();
        }
    }

    /// <summary>
    /// Example ViewModel that uses RxSchedulers without requiring attributes.
    /// This would previously require RequiresUnreferencedCode when using RxApp schedulers.
    /// </summary>
    private class ExampleViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _greeting;
        private string? _name;

        public ExampleViewModel()
        {
            // Using RxSchedulers instead of RxApp schedulers avoids RequiresUnreferencedCode
            _greeting = this.WhenAnyValue(x => x.Name)
                .Select(name => $"Hello, {name ?? "World"}!")
                .ObserveOn(RxSchedulers.MainThreadScheduler) // No RequiresUnreferencedCode needed!
                .ToProperty(this, nameof(Greeting), scheduler: RxSchedulers.MainThreadScheduler);
        }

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Greeting => _greeting.Value;
    }
}
