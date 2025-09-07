// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;

namespace ReactiveUI.Tests;

/// <summary>
/// Demonstrates using ReactiveUI schedulers without RequiresUnreferencedCode attributes.
/// </summary>
public class SchedulerConsumptionTest
{
    [Fact]
    public void ViewModelCanUseSchedulersWithoutAttributes()
    {
        var viewModel = new ExampleViewModel();

        viewModel.Name = "ReactiveUI";

        Assert.Equal("Hello, ReactiveUI!", viewModel.Greeting);
    }

    [Fact]
    public void RepositoryCanUseSchedulersWithoutAttributes()
    {
        var repository = new ExampleRepository();
        string? result = null;

        repository.GetData().Subscribe(data => result = data);
        repository.PublishData("test");

        Assert.Equal("Processed: test", result);
    }

    [Fact]
    public void ReactivePropertyFactoryMethodsWork()
    {
        // These factory methods use RxSchedulers internally, so no RequiresUnreferencedCode needed
        var prop1 = ReactiveProperty<string>.Create();
        var prop2 = ReactiveProperty<string>.Create("initial");
        var prop3 = ReactiveProperty<int>.Create(42, false, true);

        Assert.NotNull(prop1);
        Assert.NotNull(prop2);
        Assert.NotNull(prop3);

        Assert.Null(prop1.Value);
        Assert.Equal("initial", prop2.Value);
        Assert.Equal(42, prop3.Value);
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
