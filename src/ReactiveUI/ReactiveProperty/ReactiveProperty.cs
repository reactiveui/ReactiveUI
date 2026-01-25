// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Represents a reactive property that notifies subscribers of value changes and supports asynchronous validation and
/// error notification.
/// </summary>
/// <remarks>ReactiveProperty{T} provides observable value semantics, allowing consumers to subscribe to value
/// changes and validation error updates. It supports INotifyPropertyChanged and INotifyDataErrorInfo for integration
/// with data binding scenarios. Validation logic can be attached to perform asynchronous or synchronous checks, and
/// error notifications are propagated to observers. Thread safety and scheduling of notifications are managed via the
/// provided scheduler. Disposing the instance releases all resources and completes all observable streams.</remarks>
/// <typeparam name="T">The type of the value stored by the reactive property.</typeparam>
[DataContract]
public class ReactiveProperty<T> : ReactiveObject, IReactiveProperty<T>
{
    /// <summary>
    /// The scheduler used to marshal validation and error notifications.
    /// </summary>
    private readonly IScheduler _scheduler;

    /// <summary>
    /// Holds disposables associated with the instance lifetime.
    /// </summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// The equality comparer used to compare incoming values to the current value.
    /// </summary>
    private readonly EqualityComparer<T?> _checkIf = EqualityComparer<T?>.Default;

    /// <summary>
    /// Publishes value changes to the validation pipeline.
    /// </summary>
    private readonly Subject<T?> _checkValidation = new();

    /// <summary>
    /// Publishes "refresh" signals for the current value (used to force emission even when the value is unchanged).
    /// </summary>
    private readonly Subject<T?> _valueRefereshed = new();

    /// <summary>
    /// Publishes <see cref="Value"/> changes without relying on reflection-based property observation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This subject replaces the prior <c>WhenAnyValue(nameof(Value))</c> path and provides the source stream used by
    /// <see cref="GetSubscription"/>.
    /// </para>
    /// <para>
    /// The subject is disposed with the instance; emission sites guard against disposal using <see cref="IsDisposed"/>.
    /// </para>
    /// </remarks>
    private readonly BehaviorSubject<T?> _valueChanged;

    /// <summary>
    /// Holds the active validation subscription, if any.
    /// </summary>
    private readonly SerialDisposable _validationDisposable = new();

    /// <summary>
    /// Lazily created subject that publishes the current error sequence.
    /// </summary>
    private readonly Lazy<BehaviorSubject<IEnumerable?>> _errorChanged;

    /// <summary>
    /// Stores validators registered via <see cref="AddValidationError(Func{IObservable{T}, IObservable{IEnumerable}}, bool)"/>.
    /// </summary>
    private readonly Lazy<List<Func<IObservable<T?>, IObservable<IEnumerable?>>>> _validatorStore = new(static () => []);

    /// <summary>
    /// The number of initial values to skip for subscriptions created by <see cref="GetSubscription"/>.
    /// </summary>
    private readonly int _skipCurrentValue;

    /// <summary>
    /// Indicates whether <see cref="GetSubscription"/> applies <c>DistinctUntilChanged</c>.
    /// </summary>
    private readonly bool _isDistinctUntilChanged;

    /// <summary>
    /// The shared observable backing <see cref="Subscribe(IObserver{T})"/>.
    /// </summary>
    private IObservable<T?>? _observable;

    /// <summary>
    /// The current value backing <see cref="Value"/>.
    /// </summary>
    private T? _value;

    /// <summary>
    /// The current validation errors, if any.
    /// </summary>
    private IEnumerable? _currentErrors;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the default value, using the task pool
    /// scheduler and default notification and distinctness settings.
    /// </summary>
    /// <remarks>This constructor is a convenient way to create a <see cref="ReactiveProperty{T}"/> with commonly used
    /// defaults. The property will use the default value for its type, schedule notifications on the task pool
    /// scheduler, and will not suppress notifications or enforce distinctness by default.</remarks>
    public ReactiveProperty()
        : this(default, RxSchedulers.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the specified initial value.
    /// </summary>
    /// <param name="initialValue">The initial value to assign to the property. Can be null for reference types and nullable value types.</param>
    public ReactiveProperty(T? initialValue)
        : this(initialValue, RxSchedulers.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the specified initial value, subscription
    /// behavior, and duplicate value handling, using the default task pool scheduler.
    /// </summary>
    /// <param name="initialValue">The initial value to assign to the property. Can be null if the type parameter T is a reference type or a
    /// nullable value type.</param>
    /// <param name="skipCurrentValueOnSubscribe">true to prevent subscribers from immediately receiving the current value upon subscription; otherwise, false.</param>
    /// <param name="allowDuplicateValues">true to allow consecutive duplicate values to be published; otherwise, false.</param>
    public ReactiveProperty(T? initialValue, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
        : this(initialValue, RxSchedulers.TaskpoolScheduler, skipCurrentValueOnSubscribe, allowDuplicateValues)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the specified initial value, scheduler, and
    /// configuration options.
    /// </summary>
    /// <remarks>Use this constructor to customize the behavior of value emission and notification scheduling
    /// for the ReactiveProperty. The configuration options allow control over whether subscribers receive the current
    /// value immediately and whether duplicate values are propagated.</remarks>
    /// <param name="initialValue">The initial value to assign to the property. This value is immediately available to subscribers upon
    /// subscription unless skipped by configuration.</param>
    /// <param name="scheduler">The scheduler used to notify observers of value changes. If null, a default task pool scheduler is used.</param>
    /// <param name="skipCurrentValueOnSubscribe">true to prevent the current value from being emitted to new subscribers upon subscription; otherwise, false.</param>
    /// <param name="allowDuplicateValues">true to allow consecutive duplicate values to be published to subscribers; otherwise, false to suppress
    /// duplicate notifications.</param>
    public ReactiveProperty(T? initialValue, IScheduler? scheduler, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
    {
        _skipCurrentValue = skipCurrentValueOnSubscribe ? 1 : 0;
        _isDistinctUntilChanged = !allowDuplicateValues;
        _value = initialValue;
        _scheduler = scheduler ?? RxSchedulers.TaskpoolScheduler;

        _valueChanged = new BehaviorSubject<T?>(initialValue);
        _errorChanged = new Lazy<BehaviorSubject<IEnumerable?>>(() => new BehaviorSubject<IEnumerable?>(GetErrors(null)));

        GetSubscription();
    }

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets a value indicating whether gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _disposables.IsDisposed;

    /// <summary>
    /// Gets or sets the current value of the container.
    /// </summary>
    /// <remarks>Assigning a new value triggers change notifications if the value differs from the previous
    /// one. If duplicate assignments are allowed, setting the same value may also trigger a refresh
    /// notification.</remarks>
    [DataMember]
    [JsonInclude]
    public T? Value
    {
        get => _value;
        set
        {
            if (_checkIf.Equals(_value, value))
            {
                if (!_isDistinctUntilChanged)
                {
                    // Preserve existing semantics: identical assignment produces a "refresh" emission when duplicates are allowed.
                    _valueRefereshed.OnNext(_value);
                }

                return;
            }

            SetValue(value);
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets a value indicating whether any errors are currently present.
    /// </summary>
    public bool HasErrors => _currentErrors != null;

    /// <summary>
    /// Gets an observable sequence that signals when the collection of validation errors changes.
    /// </summary>
    /// <remarks>Subscribers receive notifications whenever the set of errors is updated. The sequence emits
    /// the current collection of errors after each change. The observable completes when the owning object is disposed,
    /// if applicable.</remarks>
    public IObservable<IEnumerable?> ObserveErrorChanged => _errorChanged.Value.AsObservable();

    /// <summary>
    /// Gets an observable sequence that signals whether the object currently has validation errors.
    /// </summary>
    /// <remarks>The returned observable emits a new value each time the error state changes. Subscribers
    /// receive the current error state immediately upon subscription, followed by updates as errors are added or
    /// cleared.</remarks>
    public IObservable<bool> ObserveHasErrors => ObserveErrorChanged.Select(_ => HasErrors);

    /// <summary>
    /// Creates a new instance of ReactiveProperty without requiring RequiresUnreferencedCode attributes.
    /// Uses RxSchedulers.TaskpoolScheduler as the default scheduler.
    /// </summary>
    /// <returns>A new ReactiveProperty instance.</returns>
    public static ReactiveProperty<T> Create()
        => new(default, RxSchedulers.TaskpoolScheduler, false, false);

    /// <summary>
    /// Creates a new instance of ReactiveProperty with an initial value without requiring RequiresUnreferencedCode attributes.
    /// Uses RxSchedulers.TaskpoolScheduler as the default scheduler.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <returns>A new ReactiveProperty instance.</returns>
    public static ReactiveProperty<T> Create(T? initialValue)
        => new(initialValue, RxSchedulers.TaskpoolScheduler, false, false);

    /// <summary>
    /// Creates a new instance of ReactiveProperty with configuration options without requiring RequiresUnreferencedCode attributes.
    /// Uses RxSchedulers.TaskpoolScheduler as the default scheduler.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    /// <returns>A new ReactiveProperty instance.</returns>
    public static ReactiveProperty<T> Create(T? initialValue, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
        => new(initialValue, RxSchedulers.TaskpoolScheduler, skipCurrentValueOnSubscribe, allowDuplicateValues);

    /// <summary>
    /// Creates a new instance of ReactiveProperty with a custom scheduler without requiring RequiresUnreferencedCode attributes.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    /// <returns>A new ReactiveProperty instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> is <see langword="null"/>.</exception>
    public static ReactiveProperty<T> Create(T? initialValue, IScheduler scheduler, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
        => new(initialValue, scheduler, skipCurrentValueOnSubscribe, allowDuplicateValues);

    /// <summary>
    /// Adds a validation rule to the current ReactiveProperty instance using the specified validator function.
    /// </summary>
    /// <remarks>Multiple validation rules can be added by calling this method multiple times. Validation
    /// errors from all registered validators are combined. The ErrorsChanged event is raised whenever the set of
    /// validation errors changes.</remarks>
    /// <param name="validator">A function that takes an observable sequence of property values and returns an observable sequence of validation
    /// errors. The returned sequence should emit validation results whenever the property value changes.</param>
    /// <param name="ignoreInitialError">true to ignore validation for the initial value of the property; otherwise, false. If true, validation will only
    /// occur on subsequent value changes.</param>
    /// <returns>The current ReactiveProperty instance with the added validation rule.</returns>
    public ReactiveProperty<T> AddValidationError(Func<IObservable<T?>, IObservable<IEnumerable?>> validator, bool ignoreInitialError = false)
    {
        _validatorStore.Value.Add(validator);
        var validators = _validatorStore.Value
            .Select(x => x(ignoreInitialError ? _checkValidation : _checkValidation.StartWith(_value)))
            .ToArray();

        _validationDisposable.Disposable = Observable
            .CombineLatest(validators)
            .ObserveOn(_scheduler)
            .Select(xs =>
            {
                if (xs.Count == 0 || xs.All(x => x == null))
                {
                    return null;
                }

                var strings = xs
                    .Where(x => x != null)
                    .OfType<string>();
                var others = xs
                    .Where(x => x is not null and not string)
                    .SelectMany(x => x!.OfType<object?>());

                return strings.Concat(others);
            })
            .Subscribe(x =>
            {
                var lastHasErrors = HasErrors;
                _currentErrors = x;
                var currentHasErrors = HasErrors;
                var handler = ErrorsChanged;
                if (handler != null)
                {
                    _scheduler.Schedule(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                }

                if (lastHasErrors != currentHasErrors)
                {
                    _scheduler.Schedule(() => this.RaisePropertyChanged(SingletonPropertyChangedEventArgs.HasErrors.PropertyName));
                }

                _errorChanged.Value.OnNext(x);
            }).DisposeWith(_disposables);
        return this;
    }

    /// <summary>
    /// Adds a validation rule to the property using the specified validator function.
    /// </summary>
    /// <remarks>Multiple validation rules can be added by calling this method multiple times. Validation
    /// errors are aggregated and exposed by the property. The validator function should be stateless and
    /// thread-safe.</remarks>
    /// <param name="validator">A function that receives an observable sequence of property values and returns an observable sequence of
    /// validation error messages. The returned string is interpreted as the error message; a null value indicates no
    /// error.</param>
    /// <param name="ignoreInitialError">true to suppress the initial validation error until the property value changes; otherwise, false.</param>
    /// <returns>The current <see cref="ReactiveProperty{T}"/> instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<IObservable<T?>, IObservable<string?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => validator(xs).Select(x => (IEnumerable?)x), ignoreInitialError);

    /// <summary>
    /// Adds asynchronous validation logic to the reactive property using the specified validator function.
    /// </summary>
    /// <remarks>This method enables chaining of multiple validation rules on a <see cref="ReactiveProperty{T}"/>.
    /// Validation is triggered whenever the property's value changes. The validator function can perform asynchronous
    /// operations, such as remote checks or complex computations.</remarks>
    /// <param name="validator">A function that asynchronously validates the current value and returns a collection of validation errors. The
    /// function receives the current value as input and returns a task that produces an enumerable of validation error
    /// objects. If the collection is empty or null, the value is considered valid.</param>
    /// <param name="ignoreInitialError">true to suppress validation errors for the initial value; otherwise, false.</param>
    /// <returns>The current <see cref="ReactiveProperty{T}"/> instance with the specified validation logic applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<IEnumerable?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.SelectMany(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Adds an asynchronous validation rule to the property using the specified validator function.
    /// </summary>
    /// <remarks>The validator function is invoked whenever the property's value changes. If multiple
    /// validation rules are added, all are evaluated and their error messages are aggregated.</remarks>
    /// <param name="validator">A function that asynchronously validates the property's value and returns an error message if validation fails,
    /// or null if the value is valid.</param>
    /// <param name="ignoreInitialError">true to suppress the initial validation error until the value changes; otherwise, false.</param>
    /// <returns>A <see cref="ReactiveProperty{T}"/> instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<string?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.SelectMany(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Adds a validation rule to the reactive property using the specified validator function.
    /// </summary>
    /// <remarks>If multiple validation rules are added, all validators are evaluated and their errors are
    /// aggregated. The validator function is invoked whenever the property's value changes.</remarks>
    /// <param name="validator">A function that takes the current value and returns a collection of validation errors. Returns null or an empty
    /// collection if the value is valid.</param>
    /// <param name="ignoreInitialError">true to ignore validation errors for the initial value; otherwise, false.</param>
    /// <returns>The current <see cref="ReactiveProperty{T}"/> instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, IEnumerable?> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.Select(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Adds a validation rule to the property using the specified validator function.
    /// </summary>
    /// <remarks>If multiple validation rules are added, all validators are evaluated and their error messages
    /// are aggregated. The property is considered valid only if all validators return null or an empty
    /// string.</remarks>
    /// <param name="validator">A function that takes the current value of the property and returns a validation error message if the value is
    /// invalid; otherwise, returns null or an empty string if the value is valid.</param>
    /// <param name="ignoreInitialError">true to suppress validation errors for the initial value of the property; otherwise, false.</param>
    /// <returns>The current <see cref="ReactiveProperty{T}"/> instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, string?> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.Select(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Triggers the validation check for the current value.
    /// </summary>
    /// <remarks>This method notifies any observers that a validation check should be performed using the
    /// current value. Typically used to initiate validation logic in response to user actions or programmatic
    /// changes.</remarks>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void CheckValidation() => _checkValidation.OnNext(_value);

    /// <summary>
    /// Refreshes the current value and notifies subscribers of any changes.
    /// </summary>
    /// <remarks>Call this method to force the value to be re-evaluated and to raise change notifications,
    /// even if the value has not changed. This is useful when the underlying data source may have changed independently
    /// of property setters.</remarks>
    public void Refresh()
    {
        SetValue(_value);
        _valueRefereshed.OnNext(_value);
        this.RaisePropertyChanged(nameof(Value));
    }

    /// <summary>
    /// Gets the validation errors for the specified property or for the entire object.
    /// </summary>
    /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or empty to retrieve errors for the entire
    /// object.</param>
    /// <returns>An enumerable collection of validation errors for the specified property, or for the entire object if <paramref
    /// name="propertyName"/> is null or empty. Returns null if there are no errors.</returns>
    public IEnumerable? GetErrors(string? propertyName) => _currentErrors;

    /// <summary>
    /// Returns the validation errors for the specified property or for the entire object.
    /// </summary>
    /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or empty to retrieve errors for the entire
    /// object.</param>
    /// <returns>An enumerable collection of validation errors. Returns an empty collection if there are no errors for the
    /// specified property or object.</returns>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _currentErrors ?? Enumerable.Empty<object>();

    /// <summary>
    /// Subscribes the specified observer to receive notifications from this observable sequence.
    /// </summary>
    /// <remarks>If the observable has already been disposed, the observer's OnCompleted method is called
    /// immediately and a no-op disposable is returned.</remarks>
    /// <param name="observer">The observer that will receive notifications. Cannot be null.</param>
    /// <returns>A disposable object that can be used to unsubscribe the observer from the observable sequence.</returns>
    public IDisposable Subscribe(IObserver<T?> observer)
    {
        if (observer == null)
        {
            return Disposable.Empty;
        }

        if (IsDisposed)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }

        return _observable!.Subscribe(observer).DisposeWith(_disposables);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposables?.IsDisposed == false && disposing)
        {
            _disposables?.Dispose();

            _checkValidation.Dispose();
            _valueRefereshed.Dispose();
            _validationDisposable.Dispose();

            _valueChanged.OnCompleted();
            _valueChanged.Dispose();

            if (_errorChanged.IsValueCreated)
            {
                _errorChanged.Value.OnCompleted();
                _errorChanged.Value.Dispose();
            }
        }
    }

    /// <summary>
    /// Sets the current value and notifies subscribers of the change.
    /// </summary>
    /// <param name="value">The new value to assign. May be null if the type parameter allows null values.</param>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void SetValue(T? value)
    {
        _value = value;

        if (!IsDisposed)
        {
            _checkValidation.OnNext(value);
            _valueChanged.OnNext(value);
        }
    }

    /// <summary>
    /// Initializes the shared subscription backing <see cref="Subscribe(IObserver{T})"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The stream is built from a value source that does not require reflection-based property observation.
    /// </para>
    /// <para>
    /// Duplicate suppression (when enabled) applies only to value-change emissions, while explicit refresh emissions
    /// always flow through to subscribers.
    /// </para>
    /// </remarks>
    private void GetSubscription()
    {
        IObservable<T?> source = _valueChanged;

        source = source.Skip(_skipCurrentValue);

        if (_isDistinctUntilChanged)
        {
            source = source.DistinctUntilChanged();
        }

        _observable = source
            .Merge(_valueRefereshed)
            .Replay(1)
            .RefCount()
            .ObserveOn(_scheduler);
    }
}
