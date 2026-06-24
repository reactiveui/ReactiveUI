// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
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
    /// <summary>The scheduler used to marshal validation and error notifications.</summary>
    private readonly ISequencer _scheduler;

    /// <summary>Holds disposables associated with the instance lifetime.</summary>
    private readonly MultipleDisposable _disposables = [];

    /// <summary>The equality comparer used to compare incoming values to the current value.</summary>
    private readonly EqualityComparer<T?> _checkIf = EqualityComparer<T?>.Default;

    /// <summary>Publishes value changes to the validation pipeline.</summary>
    private readonly Signal<T?> _checkValidation = new();

    /// <summary>Publishes "refresh" signals for the current value (used to force emission even when the value is unchanged).</summary>
    private readonly Signal<T?> _valueRefereshed = new();

    /// <summary>Publishes <see cref="Value"/> changes without relying on reflection-based property observation.</summary>
    /// <remarks>
    /// <para>
    /// This subject replaces the prior <c>WhenAnyValue(nameof(Value))</c> path and provides the source stream used by
    /// <see cref="GetSubscription"/>.
    /// </para>
    /// <para>
    /// The subject is disposed with the instance; emission sites guard against disposal using <see cref="IsDisposed"/>.
    /// </para>
    /// </remarks>
    private readonly BehaviorSignal<T?> _valueChanged;

    /// <summary>Holds the active validation subscription, if any.</summary>
    private readonly SwapDisposable _validationDisposable = new();

    /// <summary>Lazily created subject that publishes the current error sequence.</summary>
    private readonly Lazy<BehaviorSignal<IEnumerable?>> _errorChanged;

    /// <summary>Stores validators registered via <see cref="AddValidationError(Func{IObservable{T}, IObservable{IEnumerable}}, bool)"/>.</summary>
    private readonly Lazy<List<Func<IObservable<T?>, IObservable<IEnumerable?>>>>
        _validatorStore = new(static () => []);

    /// <summary>The number of initial values to skip for subscriptions created by <see cref="GetSubscription"/>.</summary>
    private readonly int _skipCurrentValue;

    /// <summary>Indicates whether <see cref="GetSubscription"/> applies <c>DistinctUntilChanged</c>.</summary>
    private readonly bool _isDistinctUntilChanged;

    /// <summary>The shared observable backing <see cref="Subscribe(IObserver{T})"/>.</summary>
    private ReplaySignal<T?>? _observable;

    /// <summary>The current value backing <see cref="Value"/>.</summary>
    private T? _value;

    /// <summary>The current validation errors, if any.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the specified initial value.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class with the specified initial value, scheduler, and configuration options.</summary>
    /// <remarks>Use this constructor to customize the behavior of value emission and notification scheduling
    /// for the ReactiveProperty. The configuration options allow control over whether subscribers receive the current
    /// value immediately and whether duplicate values are propagated.</remarks>
    /// <param name="initialValue">The initial value to assign to the property. This value is immediately available to subscribers upon
    /// subscription unless skipped by configuration.</param>
    /// <param name="scheduler">The scheduler used to notify observers of value changes. If null, a default task pool scheduler is used.</param>
    /// <param name="skipCurrentValueOnSubscribe">true to prevent the current value from being emitted to new subscribers upon subscription; otherwise, false.</param>
    /// <param name="allowDuplicateValues">true to allow consecutive duplicate values to be published to subscribers; otherwise, false to suppress
    /// duplicate notifications.</param>
    public ReactiveProperty(
        T? initialValue,
        ISequencer? scheduler,
        bool skipCurrentValueOnSubscribe,
        bool allowDuplicateValues)
    {
        _skipCurrentValue = skipCurrentValueOnSubscribe ? 1 : 0;
        _isDistinctUntilChanged = !allowDuplicateValues;
        _value = initialValue;
        _scheduler = scheduler ?? RxSchedulers.TaskpoolScheduler;

        _valueChanged = new(initialValue);
        _errorChanged = new(() => new(GetErrors(null)));

        GetSubscription();
    }

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>Gets a value indicating whether gets a value that indicates whether the object is disposed.</summary>
    public bool IsDisposed => _disposables.IsDisposed;

    /// <summary>Gets or sets the current value of the container.</summary>
    /// <remarks>Assigning a new value triggers change notifications if the value differs from the previous
    /// one. If duplicate assignments are allowed, setting the same value may also trigger a refresh
    /// notification.</remarks>
    [DataMember]
    [JsonInclude]
    [SuppressMessage(
        "Critical Bug",
        "S4275:Getters and setters should access the expected fields",
        Justification = "Setter writes _value via SetValue().")]
    public T? Value
    {
        get => _value;
        set
        {
            if (_checkIf.Equals(_value, value))
            {
                if (!_isDistinctUntilChanged)
                {
                    _valueRefereshed.OnNext(_value);
                }

                return;
            }

            SetValue(value);
            this.RaisePropertyChanged();
        }
    }

    /// <summary>Gets a value indicating whether any errors are currently present.</summary>
    public bool HasErrors => _currentErrors is not null;

    /// <summary>Gets an observable sequence that signals when the collection of validation errors changes.</summary>
    /// <remarks>Subscribers receive notifications whenever the set of errors is updated. The sequence emits
    /// the current collection of errors after each change. The observable completes when the owning object is disposed,
    /// if applicable.</remarks>
    public IObservable<IEnumerable?> ObserveErrorChanged => _errorChanged.Value;

    /// <summary>Gets an observable sequence that signals whether the object currently has validation errors.</summary>
    /// <remarks>The returned observable emits a new value each time the error state changes. Subscribers
    /// receive the current error state immediately upon subscription, followed by updates as errors are added or
    /// cleared.</remarks>
    public IObservable<bool> ObserveHasErrors => new MapSignal<IEnumerable?, bool>(ObserveErrorChanged, _ => HasErrors);

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
    public static ReactiveProperty<T> Create(
        T? initialValue,
        bool skipCurrentValueOnSubscribe,
        bool allowDuplicateValues) => new(initialValue, RxSchedulers.TaskpoolScheduler, skipCurrentValueOnSubscribe, allowDuplicateValues);

    /// <summary>
    /// Creates a new instance of ReactiveProperty with a custom scheduler without requiring RequiresUnreferencedCode attributes.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    /// <returns>A new ReactiveProperty instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> is <see langword="null"/>.</exception>
    public static ReactiveProperty<T> Create(
        T? initialValue,
        ISequencer scheduler,
        bool skipCurrentValueOnSubscribe,
        bool allowDuplicateValues) => new(initialValue, scheduler, skipCurrentValueOnSubscribe, allowDuplicateValues);

    /// <summary>Adds a validation rule to the current ReactiveProperty instance using the specified validator function.</summary>
    /// <param name="validator">A function that takes an observable sequence of property values and returns an observable sequence of validation errors.</param>
    /// <returns>The current ReactiveProperty instance with the added validation rule.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<IObservable<T?>, IObservable<IEnumerable?>> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds a validation rule to the current ReactiveProperty instance using the specified validator function.</summary>
    /// <remarks>Multiple validation rules can be added by calling this method multiple times. Validation
    /// errors from all registered validators are combined. The ErrorsChanged event is raised whenever the set of
    /// validation errors changes.</remarks>
    /// <param name="validator">A function that takes an observable sequence of property values and returns an observable sequence of validation
    /// errors. The returned sequence should emit validation results whenever the property value changes.</param>
    /// <param name="ignoreInitialError">true to ignore validation for the initial value of the property; otherwise, false. If true, validation will only
    /// occur on subsequent value changes.</param>
    /// <returns>The current ReactiveProperty instance with the added validation rule.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<IObservable<T?>, IObservable<IEnumerable?>> validator,
        bool ignoreInitialError)
    {
        _validatorStore.Value.Add(validator);
        var validatorFuncs = _validatorStore.Value;
        IObservable<T?> validationSource = ignoreInitialError ? _checkValidation : new PrependObservable(_checkValidation, _value);
        var validators = new IObservable<IEnumerable?>[validatorFuncs.Count];
        for (var i = 0; i < validatorFuncs.Count; i++)
        {
            validators[i] = validatorFuncs[i](validationSource);
        }

        _validationDisposable.Disposable = new ValidationStream(validators, _scheduler)
            .Subscribe(new DelegateObserver<IEnumerable?>(x =>
            {
                var lastHasErrors = HasErrors;
                _currentErrors = x;
                var currentHasErrors = HasErrors;
                var handler = ErrorsChanged;
                if (handler is not null)
                {
                    _ = _scheduler.ScheduleOrInline(() => handler(this, SingletonDataErrorsChangedEventArgs.Value));
                }

                if (lastHasErrors != currentHasErrors)
                {
                    _ = _scheduler.ScheduleOrInline(() =>
                        this.RaisePropertyChanged(SingletonPropertyChangedEventArgs.HasErrors.PropertyName));
                }

                _errorChanged.Value.OnNext(x);
            }));
        return this;
    }

    /// <summary>Adds a validation rule to the property using the specified validator function.</summary>
    /// <param name="validator">A function that receives an observable sequence of property values and returns an observable sequence of validation error messages.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<IObservable<T?>, IObservable<string?>> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds a validation rule to the property using the specified validator function.</summary>
    /// <remarks>Multiple validation rules can be added by calling this method multiple times. Validation
    /// errors are aggregated and exposed by the property. The validator function should be stateless and
    /// thread-safe.</remarks>
    /// <param name="validator">A function that receives an observable sequence of property values and returns an observable sequence of
    /// validation error messages. The returned string is interpreted as the error message; a null value indicates no
    /// error.</param>
    /// <param name="ignoreInitialError">true to suppress the initial validation error until the property value changes; otherwise, false.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<IObservable<T?>, IObservable<string?>> validator,
        bool ignoreInitialError) =>
        AddValidationError(xs => new MapSignal<string?, IEnumerable?>(validator(xs), static x => (IEnumerable?)x), ignoreInitialError);

    /// <summary>Adds asynchronous validation logic to the reactive property using the specified validator function.</summary>
    /// <param name="validator">A function that asynchronously validates the current value and returns a collection of validation errors.</param>
    /// <returns>The current ReactiveProperty instance with the specified validation logic applied.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<T?, Task<IEnumerable?>> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds asynchronous validation logic to the reactive property using the specified validator function.</summary>
    /// <remarks>This method enables chaining of multiple validation rules on a ReactiveProperty.
    /// Validation is triggered whenever the property's value changes. The validator function can perform asynchronous
    /// operations, such as remote checks or complex computations.</remarks>
    /// <param name="validator">A function that asynchronously validates the current value and returns a collection of validation errors. The
    /// function receives the current value as input and returns a task that produces an enumerable of validation error
    /// objects. If the collection is empty or null, the value is considered valid.</param>
    /// <param name="ignoreInitialError">true to suppress validation errors for the initial value; otherwise, false.</param>
    /// <returns>The current ReactiveProperty instance with the specified validation logic applied.</returns>
    public ReactiveProperty<T> AddValidationError(
        Func<T?, Task<IEnumerable?>> validator,
        bool ignoreInitialError) =>
        AddValidationError(xs => new AsyncProjectObservable<T?, IEnumerable?>(xs, x => validator(x)), ignoreInitialError);

    /// <summary>Adds an asynchronous validation rule to the property using the specified validator function.</summary>
    /// <param name="validator">A function that asynchronously validates the property's value and returns an error message or null.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<string?>> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds an asynchronous validation rule to the property using the specified validator function.</summary>
    /// <remarks>The validator function is invoked whenever the property's value changes. If multiple
    /// validation rules are added, all are evaluated and their error messages are aggregated.</remarks>
    /// <param name="validator">A function that asynchronously validates the property's value and returns an error message if validation fails,
    /// or null if the value is valid.</param>
    /// <param name="ignoreInitialError">true to suppress the initial validation error until the value changes; otherwise, false.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<string?>> validator, bool ignoreInitialError) =>
        AddValidationError(xs => new AsyncProjectObservable<T?, string?>(xs, x => validator(x)), ignoreInitialError);

    /// <summary>Adds a validation rule to the reactive property using the specified validator function.</summary>
    /// <param name="validator">A function that takes the current value and returns a collection of validation errors.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, IEnumerable?> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds a validation rule to the reactive property using the specified validator function.</summary>
    /// <remarks>If multiple validation rules are added, all validators are evaluated and their errors are
    /// aggregated. The validator function is invoked whenever the property's value changes.</remarks>
    /// <param name="validator">A function that takes the current value and returns a collection of validation errors. Returns null or an empty
    /// collection if the value is valid.</param>
    /// <param name="ignoreInitialError">true to ignore validation errors for the initial value; otherwise, false.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, IEnumerable?> validator, bool ignoreInitialError) =>
        AddValidationError(xs => new MapSignal<T?, IEnumerable?>(xs, x => validator(x)), ignoreInitialError);

    /// <summary>Adds a validation rule to the property using the specified validator function.</summary>
    /// <param name="validator">A function that returns a validation error message or null if the value is valid.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, string?> validator) =>
        AddValidationError(validator, false);

    /// <summary>Adds a validation rule to the property using the specified validator function.</summary>
    /// <remarks>If multiple validation rules are added, all validators are evaluated and their error messages
    /// are aggregated. The property is considered valid only if all validators return null or an empty
    /// string.</remarks>
    /// <param name="validator">A function that takes the current value of the property and returns a validation error message if the value is
    /// invalid; otherwise, returns null or an empty string if the value is valid.</param>
    /// <param name="ignoreInitialError">true to suppress validation errors for the initial value of the property; otherwise, false.</param>
    /// <returns>The current ReactiveProperty instance with the validation rule applied.</returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, string?> validator, bool ignoreInitialError) =>
        AddValidationError(xs => new MapSignal<T?, string?>(xs, x => validator(x)), ignoreInitialError);

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
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

    /// <summary>Triggers the validation check for the current value.</summary>
    /// <remarks>This method notifies any observers that a validation check should be performed using the
    /// current value. Typically used to initiate validation logic in response to user actions or programmatic
    /// changes.</remarks>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void CheckValidation() => _checkValidation.OnNext(_value);

    /// <summary>Refreshes the current value and notifies subscribers of any changes.</summary>
    /// <remarks>Call this method to force the value to be re-evaluated and to raise change notifications,
    /// even if the value has not changed. This is useful when the underlying data source may have changed independently
    /// of property setters.</remarks>
    public void Refresh()
    {
        SetValue(_value);
        _valueRefereshed.OnNext(_value);
        this.RaisePropertyChanged(nameof(Value));
    }

    /// <summary>Gets the validation errors for the specified property or for the entire object.</summary>
    /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or empty to retrieve errors for the entire
    /// object.</param>
    /// <returns>An enumerable collection of validation errors for the specified property, or for the entire object if <paramref
    /// name="propertyName"/> is null or empty. Returns null if there are no errors.</returns>
    public IEnumerable? GetErrors(string? propertyName) => _currentErrors;

    /// <summary>Returns the validation errors for the specified property or for the entire object.</summary>
    /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or empty to retrieve errors for the entire
    /// object.</param>
    /// <returns>An enumerable collection of validation errors. Returns an empty collection if there are no errors for the
    /// specified property or object.</returns>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _currentErrors ?? Enumerable.Empty<object>();

    /// <summary>Subscribes the specified observer to receive notifications from this observable sequence.</summary>
    /// <remarks>If the observable has already been disposed, the observer's OnCompleted method is called
    /// immediately and a no-op disposable is returned.</remarks>
    /// <param name="observer">The observer that will receive notifications. Cannot be null.</param>
    /// <returns>A disposable object that can be used to unsubscribe the observer from the observable sequence.</returns>
    public IDisposable Subscribe(IObserver<T?> observer)
    {
        if (observer is null)
        {
            return EmptyDisposable.Instance;
        }

        if (IsDisposed)
        {
            observer.OnCompleted();
            return EmptyDisposable.Instance;
        }

        var subscription = _observable!.Subscribe(new SchedulingObserver<T?>(observer, _scheduler));
        _disposables.Add(subscription);
        return subscription;
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposables?.IsDisposed != false || !disposing)
        {
            return;
        }

        _disposables.Dispose();

        _checkValidation.Dispose();
        _valueRefereshed.Dispose();
        _validationDisposable.Dispose();

        _valueChanged.OnCompleted();
        _valueChanged.Dispose();

        _observable?.Dispose();

        if (!_errorChanged.IsValueCreated)
        {
            return;
        }

        _errorChanged.Value.OnCompleted();
        _errorChanged.Value.Dispose();
    }

    /// <summary>Sets the current value and notifies subscribers of the change.</summary>
    /// <param name="value">The new value to assign. May be null if the type parameter allows null values.</param>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void SetValue(T? value)
    {
        _value = value;

        if (IsDisposed)
        {
            return;
        }

        _checkValidation.OnNext(value);
        _valueChanged.OnNext(value);
    }

    /// <summary>Initializes the shared subscription backing <see cref="Subscribe(IObserver{T})"/>.</summary>
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
        // A replay-1 relay multicasts to all subscribers (Replay(1) + RefCount). Value changes pass through the
        // skip-initial and distinct gates; refresh emissions always flow. Per-subscriber scheduler delivery is applied
        // in Subscribe via SchedulingObserver, so the relay itself is the shared source.
        var relay = new ReplaySignal<T?>(1);

        _disposables.Add(_valueChanged
            .Subscribe(new ValueChangeRelay(relay, _skipCurrentValue, _isDistinctUntilChanged, _checkIf)));
        _disposables.Add(_valueRefereshed
            .Subscribe(new DelegateObserver<T?>(relay.OnNext)));

        _observable = relay;
    }

    /// <summary>Applies skip-initial and optional distinct-until-changed to value changes before forwarding them to the shared relay.</summary>
    /// <param name="relay">The shared replay relay that multicasts to subscribers.</param>
    /// <param name="skipCount">The number of initial values to skip.</param>
    /// <param name="isDistinct">Whether consecutive equal values are suppressed.</param>
    /// <param name="comparer">The equality comparer used by the distinct gate.</param>
    private sealed class ValueChangeRelay(IObserver<T?> relay, int skipCount, bool isDistinct, EqualityComparer<T?> comparer) : IObserver<T?>
    {
        /// <summary>The remaining number of initial values to skip.</summary>
        private int _toSkip = skipCount;

        /// <summary>The last forwarded value, used by the distinct gate.</summary>
        private T? _last;

        /// <summary>Whether <see cref="_last"/> holds a value yet.</summary>
        private bool _hasLast;

        /// <inheritdoc/>
        public void OnNext(T? value)
        {
            if (_toSkip > 0)
            {
                _toSkip--;
                return;
            }

            if (isDistinct && _hasLast && comparer.Equals(value, _last))
            {
                return;
            }

            _last = value;
            _hasLast = true;
            relay.OnNext(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => relay.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => relay.OnCompleted();
    }

    /// <summary>
    /// Combines the latest error sequence of every validator, aggregates them, and delivers the result on the
    /// property's scheduler. Fuses the validator combine-latest, aggregation, and scheduling into one sink.
    /// </summary>
    /// <param name="validators">The per-validator error streams.</param>
    /// <param name="scheduler">The scheduler aggregated results are delivered on.</param>
    private sealed class ValidationStream(IObservable<IEnumerable?>[] validators, ISequencer scheduler) : IObservable<IEnumerable?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IEnumerable?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(new SchedulingObserver<IEnumerable?>(observer, scheduler), validators);
        }

        /// <summary>Tracks the latest error sequence of each validator and emits their aggregate once all have reported.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards the latest values and the arrival/completion counters.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the aggregated errors (already scheduled).</summary>
            private readonly IObserver<IEnumerable?> _downstream;

            /// <summary>The latest error sequence reported by each validator.</summary>
            private readonly IEnumerable?[] _latest;

            /// <summary>Whether each validator has reported at least once.</summary>
            private readonly bool[] _has;

            /// <summary>The subscriptions to each validator stream.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of validators that have reported at least once.</summary>
            private int _haveCount;

            /// <summary>The number of validators that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every validator.</summary>
            /// <param name="downstream">The observer receiving the aggregated errors.</param>
            /// <param name="validators">The per-validator error streams.</param>
            public Sink(IObserver<IEnumerable?> downstream, IObservable<IEnumerable?>[] validators)
            {
                _downstream = downstream;
                _latest = new IEnumerable?[validators.Length];
                _has = new bool[validators.Length];
                _subscriptions = new IDisposable?[validators.Length];
                for (var i = 0; i < validators.Length; i++)
                {
                    _subscriptions[i] = validators[i].Subscribe(new Element(this, i));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
                }
            }

            /// <summary>Records a validator's latest errors and emits the aggregate once every validator has reported.</summary>
            /// <param name="index">The validator index.</param>
            /// <param name="value">The validator's latest error sequence.</param>
            private void OnNextAt(int index, IEnumerable? value)
            {
                IEnumerable? aggregated;
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    if (!_has[index])
                    {
                        _has[index] = true;
                        _haveCount++;
                    }

                    _latest[index] = value;
                    if (_haveCount < _latest.Length)
                    {
                        return;
                    }

                    aggregated = BuildAggregate();
                }

                _downstream.OnNext(aggregated);
            }

            /// <summary>
            /// Materializes the combined error sequence from every validator's latest values while the gate is held.
            /// <c>_latest</c> is mutated by later emissions, so a deferred query would re-evaluate against new state
            /// when a downstream consumer enumerates it. A single pass splits strings from the flattened contents of
            /// the remaining sequences, then both are copied once into a pre-sized result (strings first).
            /// </summary>
            /// <returns>The aggregated errors, or <see langword="null"/> when every validator reported null.</returns>
            private object?[]? BuildAggregate()
            {
                if (Array.TrueForAll(_latest, static x => x is null))
                {
                    return null;
                }

                List<object?> stringValues = new(_latest.Length);
                List<object?> otherValues = [];
                foreach (var item in _latest)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    if (item is string stringValue)
                    {
                        stringValues.Add(stringValue);
                        continue;
                    }

                    foreach (var inner in item)
                    {
                        if (inner is not null)
                        {
                            otherValues.Add(inner);
                        }
                    }
                }

                var result = new object?[stringValues.Count + otherValues.Count];
                stringValues.CopyTo(result, 0);
                otherValues.CopyTo(result, stringValues.Count);
                return result;
            }

            /// <summary>Forwards an error from any validator.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every validator has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _latest.Length)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one validator's notifications to the parent sink, tagged with its index.</summary>
            /// <param name="parent">The owning sink.</param>
            /// <param name="index">The validator index.</param>
            private sealed class Element(Sink parent, int index) : IObserver<IEnumerable?>
            {
                /// <inheritdoc/>
                public void OnNext(IEnumerable? value) => parent.OnNextAt(index, value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// For each value of a validator input, runs an asynchronous selector and emits its result when the task completes,
    /// merging concurrent results. Specialised to the asynchronous validator adapters.
    /// </summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The result element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="selector">Produces a task for each source value.</param>
    private sealed class AsyncProjectObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, Task<TOut>> selector) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer, selector);
            return sink.Run(source);
        }

        /// <summary>Runs the selector per source value and merges the task results under a gate.</summary>
        private sealed class Sink : IObserver<TIn>, IDisposable
        {
            /// <summary>Serializes downstream delivery and the active-task accounting.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving merged task results.</summary>
            private readonly IObserver<TOut> _downstream;

            /// <summary>Produces a task for each source value.</summary>
            private readonly Func<TIn, Task<TOut>> _selector;

            /// <summary>The subscription to the source.</summary>
            private IDisposable? _sourceSubscription;

            /// <summary>The number of in-flight tasks plus one while the source is still active.</summary>
            private int _active = 1;

            /// <summary>Whether the downstream has been terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
            /// <param name="downstream">The observer receiving merged task results.</param>
            /// <param name="selector">Produces a task for each source value.</param>
            public Sink(IObserver<TOut> downstream, Func<TIn, Task<TOut>> selector)
            {
                _downstream = downstream;
                _selector = selector;
            }

            /// <summary>Subscribes to the source after construction so <c>this</c> is not exposed during construction.</summary>
            /// <param name="source">The source observable.</param>
            /// <returns>The sink, which disposes the run.</returns>
            public Sink Run(IObservable<TIn> source)
            {
                _sourceSubscription = source.Subscribe(this);
                return this;
            }

            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                Task<TOut> task;
                try
                {
                    task = _selector(value);
                }
                catch (Exception ex)
                {
                    Fail(ex);
                    return;
                }

                _ = Interlocked.Increment(ref _active);
                _ = ForwardAsync(task);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => Fail(error);

            /// <inheritdoc/>
            public void OnCompleted() => Done();

            /// <inheritdoc/>
            public void Dispose() => _sourceSubscription?.Dispose();

            /// <summary>Awaits the selector task, forwarding its result or error, and completes the run once everything is done.</summary>
            /// <param name="task">The selector task.</param>
            /// <returns>A task that completes when the result has been forwarded.</returns>
            private async Task ForwardAsync(Task<TOut> task)
            {
                TOut result;
                try
                {
                    result = await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Fail(ex);
                    return;
                }

                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _downstream.OnNext(result);
                }

                Done();
            }

            /// <summary>Decrements the active count and completes the downstream when the source and all tasks are finished.</summary>
            private void Done()
            {
                if (Interlocked.Decrement(ref _active) != 0)
                {
                    return;
                }

                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                    _downstream.OnCompleted();
                }
            }

            /// <summary>Forwards an error to the downstream exactly once.</summary>
            /// <param name="error">The error to forward.</param>
            private void Fail(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                    _downstream.OnError(error);
                }
            }
        }
    }

    /// <summary>Emits a leading value before forwarding the source, so a validator sees the current value first. Specialised to the validation input.</summary>
    /// <param name="source">The source observable.</param>
    /// <param name="value">The value emitted before the source.</param>
    private sealed class PrependObservable(IObservable<T?> source, T? value) : IObservable<T?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            observer.OnNext(value);
            return source.Subscribe(observer);
        }
    }
}
