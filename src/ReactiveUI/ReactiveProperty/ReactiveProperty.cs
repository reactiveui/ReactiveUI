// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Runtime.CompilerServices;

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
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// The Value will be default(T). DistinctUntilChanged is true. Current Value is published on subscribe.
    /// </summary>
    public ReactiveProperty()
        : this(default, RxSchedulers.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// The Value will be initialValue. DistinctUntilChanged is true. Current Value is published on subscribe.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public ReactiveProperty(T? initialValue)
        : this(initialValue, RxSchedulers.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}" /> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    public ReactiveProperty(T? initialValue, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
        : this(initialValue, RxSchedulers.TaskpoolScheduler, skipCurrentValueOnSubscribe, allowDuplicateValues)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}" /> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> is <see langword="null"/>.</exception>
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
    /// Gets a value indicating whether this instance has errors.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
    /// </value>
    public bool HasErrors => _currentErrors != null;

    /// <summary>
    /// Gets the observe error changed.
    /// </summary>
    /// <value>
    /// The observe error changed.
    /// </value>
    public IObservable<IEnumerable?> ObserveErrorChanged => _errorChanged.Value.AsObservable();

    /// <summary>
    /// Gets the observe has errors.
    /// </summary>
    /// <value>
    /// The observe has errors.
    /// </value>
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
    /// Set INotifyDataErrorInfo's asynchronous validation, return value is self.
    /// </summary>
    /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
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
    /// Set INotifyDataErrorInfo's asynchronous validation, return value is self.
    /// </summary>
    /// <param name="validator">If success return IO&lt;null&gt;, failure return IO&lt;IEnumerable&gt;(Errors).</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
    public ReactiveProperty<T> AddValidationError(Func<IObservable<T?>, IObservable<string?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => validator(xs).Select(x => (IEnumerable?)x), ignoreInitialError);

    /// <summary>
    /// Set INotifyDataErrorInfo's asynchronous validation.
    /// </summary>
    /// <param name="validator">Validation logic.</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<IEnumerable?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.SelectMany(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Set INotifyDataErrorInfo's asynchronous validation.
    /// </summary>
    /// <param name="validator">Validation logic.</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, Task<string?>> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.SelectMany(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Set INotifyDataErrorInfo validation.
    /// </summary>
    /// <param name="validator">Validation logic.</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
    public ReactiveProperty<T> AddValidationError(Func<T?, IEnumerable?> validator, bool ignoreInitialError = false) =>
        AddValidationError(xs => xs.Select(x => validator(x)), ignoreInitialError);

    /// <summary>
    /// Set INotifyDataErrorInfo validation.
    /// </summary>
    /// <param name="validator">Validation logic.</param>
    /// <param name="ignoreInitialError">if set to <c>true</c> [ignore initial error].</param>
    /// <returns>
    /// Self.
    /// </returns>
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
    /// Check validation.
    /// </summary>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void CheckValidation() => _checkValidation.OnNext(_value);

    /// <summary>
    /// Invoke OnNext.
    /// </summary>
    public void Refresh()
    {
        SetValue(_value);
        _valueRefereshed.OnNext(_value);
        this.RaisePropertyChanged(nameof(Value));
    }

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>A IEnumerable.</returns>
    public IEnumerable? GetErrors(string? propertyName) => _currentErrors;

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>A IEnumerable.</returns>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => _currentErrors ?? Enumerable.Empty<object>();

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>
    /// A reference to an interface that allows observers to stop receiving notifications before
    /// the provider has finished sending them.
    /// </returns>
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
    /// Sets the backing value, publishes to the validation stream, and publishes to the value-change stream.
    /// </summary>
    /// <param name="value">The new value.</param>
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
