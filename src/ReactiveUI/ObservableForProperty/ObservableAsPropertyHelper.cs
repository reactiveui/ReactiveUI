// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// ObservableAsPropertyHelper is a class to help ViewModels implement
/// "output properties", that is, a property that is backed by an
/// Observable. The property will be read-only, but will still fire change
/// notifications. This class can be created directly, but is more often created
/// via the <see cref="OAPHCreationHelperMixin" /> extension methods.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
public sealed class ObservableAsPropertyHelper<T> : IHandleObservableErrors, IDisposable, IEnableLogger
{
    private readonly Lazy<ISubject<Exception>> _thrownExceptions;
    private readonly ISubject<T?> _subject;
    private readonly Func<T?> _getInitialValue;
    private T? _lastValue;
    private CompositeDisposable _disposable = new();
    private int _activated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class.
    /// </summary>
    /// <param name="observable">
    /// The Observable to base the property on.
    /// </param>
    /// <param name="onChanged">
    /// The action to take when the property changes, typically this will call the
    /// ViewModel's RaisePropertyChanged method.
    /// </param>
    /// <param name="initialValue">
    /// The initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="observable"/> source
    /// until the first call to <see cref="Value"/>, or if it should immediately
    /// subscribe to the <paramref name="observable"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on -
    /// this should normally be a Dispatcher-based scheduler.
    /// </param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        T? initialValue = default,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
        : this(observable, onChanged, null, initialValue, deferSubscription, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class.
    /// </summary>
    /// <param name="observable">
    /// The Observable to base the property on.
    /// </param>
    /// <param name="onChanged">
    /// The action to take when the property changes, typically this will call
    /// the ViewModel's RaisePropertyChanged method.
    /// </param>
    /// <param name="onChanging">
    /// The action to take when the property changes, typically this will call
    /// the ViewModel's RaisePropertyChanging method.
    /// </param>
    /// <param name="initialValue">
    /// The initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="observable"/> source
    /// until the first call to <see cref="Value"/>, or if it should immediately
    /// subscribe to the <paramref name="observable"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will provided on - this
    /// should normally be a Dispatcher-based scheduler.
    /// </param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging = null,
        T? initialValue = default,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        : this(observable, onChanged, onChanging, () => initialValue, deferSubscription, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class.
    /// </summary>
    /// <param name="observable">
    /// The Observable to base the property on.
    /// </param>
    /// <param name="onChanged">
    /// The action to take when the property changes, typically this will call
    /// the ViewModel's RaisePropertyChanged method.
    /// </param>
    /// <param name="onChanging">
    /// The action to take when the property changes, typically this will call
    /// the ViewModel's RaisePropertyChanging method.
    /// </param>
    /// <param name="getInitialValue">
    /// The function used to retrieve the initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="observable"/> source
    /// until the first call to <see cref="Value"/>, or if it should immediately
    /// subscribe to the <paramref name="observable"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will provided on - this
    /// should normally be a Dispatcher-based scheduler.
    /// </param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging = null,
        Func<T?>? getInitialValue = null,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
    {
        if (observable is null)
        {
            throw new ArgumentNullException(nameof(observable));
        }

        if (onChanged is null)
        {
            throw new ArgumentNullException(nameof(onChanged));
        }

        scheduler ??= CurrentThreadScheduler.Instance;
        onChanging ??= _ => { };

        _thrownExceptions = new Lazy<ISubject<Exception>>(() => new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler));

        _subject = new ScheduledSubject<T?>(scheduler);
        _subject.Subscribe(
                           x =>
                           {
                               onChanging(x);
                               _lastValue = x;
                               onChanged!(x);
                           },
                           ex => _thrownExceptions.Value.OnNext(ex))
                .DisposeWith(_disposable);

        _getInitialValue = getInitialValue!;

        if (deferSubscription)
        {
            _lastValue = default;
            Source = observable.DistinctUntilChanged();
        }
        else
        {
            _lastValue = _getInitialValue();
            Source = observable.StartWith(_lastValue).DistinctUntilChanged();
            Source.Subscribe(_subject).DisposeWith(_disposable);
            _activated = 1;
        }
    }

    /// <summary>
    /// Gets the last provided value from the Observable.
    /// </summary>
    public T Value
    {
        get
        {
            if (Interlocked.CompareExchange(ref _activated, 1, 0) == 0)
            {
                // Do not subscribe if disposed
                var localReferenceInCaseDisposeIsCalled = _disposable;
                if (localReferenceInCaseDisposeIsCalled is not null)
                {
                    _lastValue = _getInitialValue();
                    Source.StartWith(_lastValue).Subscribe(_subject).DisposeWith(localReferenceInCaseDisposeIsCalled);
                }
            }

            return _lastValue!;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the ObservableAsPropertyHelper
    /// has subscribed to the source Observable.
    /// Useful for scenarios where you use deferred subscription and want to know if
    /// the ObservableAsPropertyHelper Value has been accessed yet.
    /// </summary>
    public bool IsSubscribed => _activated > 0;

    /// <summary>
    /// Gets an observable which signals whenever an exception would normally terminate ReactiveUI
    /// internal state.
    /// </summary>
    public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

    internal /* for testing purposes */ IObservable<T?> Source { get; }

    /// <summary>
    /// Constructs a "default" ObservableAsPropertyHelper object. This is
    /// useful for when you will initialize the OAPH later, but don't want
    /// bindings to access a null OAPH at startup.
    /// </summary>
    /// <param name="initialValue">
    /// The initial (and only) value of the property.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>A default property helper.</returns>
    public static ObservableAsPropertyHelper<T> Default(T? initialValue = default, IScheduler? scheduler = null) => // TODO: Create Test
        new(Observable<T>.Never, _ => { }, initialValue!, false, scheduler);

    /// <summary>
    /// Disposes this ObservableAsPropertyHelper.
    /// </summary>
    public void Dispose() // TODO: Create Test
    {
        _disposable?.Dispose();
        _disposable = null!;
    }
}