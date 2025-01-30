// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

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
    private readonly EqualityComparer<T?> _checkIf = EqualityComparer<T?>.Default;
    private readonly Subject<T?> _checkValidation = new();
    private readonly Subject<T?> _valueRefereshed = new();
    private readonly SerialDisposable _validationDisposable = new();
    private readonly Lazy<BehaviorSubject<IEnumerable?>> _errorChanged;
    private readonly Lazy<List<Func<IObservable<T?>, IObservable<IEnumerable?>>>> _validatorStore = new(() => []);
    private readonly int _skipCurrentValue;
    private readonly bool _isDistinctUntilChanged;
    private IObservable<T?>? _observable;
    private T? _value;
    private IEnumerable? _currentErrors;
    private bool _hasSubscribed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// The Value will be default(T). DistinctUntilChanged is true. Current Value is published on subscribe.
    /// </summary>
    public ReactiveProperty()
        : this(default, RxApp.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}"/> class.
    /// The Value will be initialValue. DistinctUntilChanged is true. Current Value is published on subscribe.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public ReactiveProperty(T? initialValue)
        : this(initialValue, RxApp.TaskpoolScheduler, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}" /> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    public ReactiveProperty(T? initialValue, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
        : this(initialValue, RxApp.TaskpoolScheduler, skipCurrentValueOnSubscribe, allowDuplicateValues)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveProperty{T}" /> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="skipCurrentValueOnSubscribe">if set to <c>true</c> [skip current value on subscribe].</param>
    /// <param name="allowDuplicateValues">if set to <c>true</c> [allow duplicate concurrent values].</param>
    public ReactiveProperty(T? initialValue, IScheduler? scheduler, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)
    {
        _skipCurrentValue = skipCurrentValueOnSubscribe ? 1 : 0;
        _isDistinctUntilChanged = !allowDuplicateValues;
        _value = initialValue;
        _scheduler = scheduler ?? RxApp.TaskpoolScheduler;
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
                    .Where(x => x is not string or null)
                    .SelectMany(x => x!.Cast<object?>());

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
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Check validation.
    /// </summary>
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

        if (_hasSubscribed)
        {
            observer.OnNext(_value);
        }

        _hasSubscribed = true;

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
            if (_errorChanged.IsValueCreated)
            {
                _errorChanged.Value.OnCompleted();
                _errorChanged.Value.Dispose();
            }
        }
    }

    private void SetValue(T? value)
    {
        _value = value;
        if (!IsDisposed)
        {
            _checkValidation.OnNext(value);
        }
    }

    private void GetSubscription()
    {
        _observable = this.WhenAnyValue(vm => vm.Value)
            .Skip(_skipCurrentValue);

        if (_isDistinctUntilChanged)
        {
            _observable = _observable.DistinctUntilChanged();
        }

        _observable = _observable.Merge(_valueRefereshed)
            .Publish()
            .RefCount()
            .ObserveOn(_scheduler);
    }
}
