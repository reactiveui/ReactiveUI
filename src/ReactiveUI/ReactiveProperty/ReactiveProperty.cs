// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// ReactiveProperty - a two way bindable declarative observable property with imperative get set.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
/// <seealso cref="ReactiveObject" />
/// <seealso cref="IReactiveProperty&lt;T&gt;" />
[DataContract]
public class ReactiveProperty<T> : ReactiveObject, IReactiveProperty<T>
{
    private readonly IScheduler _scheduler;
    private readonly CompositeDisposable _disposables = [];
    private T? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// </summary>
    public ReactiveProperty() => _scheduler = RxApp.TaskpoolScheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public ReactiveProperty(T? initialValue)
    {
        Value = initialValue;
        _scheduler = RxApp.TaskpoolScheduler;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="scheduler">The scheduler.</param>
    public ReactiveProperty(T? initialValue, IScheduler? scheduler)
    {
        Value = initialValue;
        _scheduler = scheduler ?? RxApp.TaskpoolScheduler;
    }

    /// <summary>
    /// Gets a value indicating whether gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _disposables.IsDisposed;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    [DataMember]
    [JsonInclude]
    public T? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>
    /// A reference to an interface that allows observers to stop receiving notifications before
    /// the provider has finished sending them.
    /// </returns>
    public IDisposable Subscribe(IObserver<T?> observer) =>
        this.WhenAnyValue(vm => vm.Value)
        .ObserveOn(_scheduler)
        .Subscribe(observer)
        .DisposeWith(_disposables);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposables?.IsDisposed == false && disposing)
        {
            _disposables?.Dispose();
        }
    }
}
