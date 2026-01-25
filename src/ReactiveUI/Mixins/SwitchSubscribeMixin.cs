// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Extension methods for subscribing to observables that emit other observables,
/// automatically switching to new inner observables when the source emits.
/// </summary>
/// <remarks>
/// <para>
/// These methods are particularly useful when working with reactive properties that
/// can be replaced, such as command properties. They ensure subscriptions follow
/// the property value changes instead of remaining attached to the old instance.
/// </para>
/// <para>
/// Example: If you have a ViewModel property <c>Command</c> that can be replaced with
/// a new ReactiveCommand instance, using SwitchSubscribe ensures your
/// subscription follows the new command rather than staying attached to the old one.
/// </para>
/// </remarks>
public static class SwitchSubscribeMixin
{
    /// <summary>
    /// Subscribes to the inner observables emitted by the source, automatically switching
    /// to new inner observables when the source emits a new value.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by the inner observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="onNext">Action to invoke for each element in the inner observable sequences.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="onNext"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to values from an observable property that can change
    /// this.WhenAnyValue(x => x.SomeObservableProperty)
    ///     .SwitchSubscribe(value => Console.WriteLine($"Value: {value}"));
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<T>(
        this IObservable<IObservable<T>?> source,
        Action<T> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return source
            .WhereNotNull()
            .Switch()
            .Subscribe(onNext);
    }

    /// <summary>
    /// Subscribes to the inner observables emitted by the source with error and completion handlers,
    /// automatically switching to new inner observables when the source emits a new value.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by the inner observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="onNext">Action to invoke for each element in the inner observable sequences.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<T>(
        this IObservable<IObservable<T>?> source,
        Action<T> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return source
            .WhereNotNull()
            .Switch()
            .Subscribe(onNext, onError, onCompleted);
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <returns>An observable sequence whose elements are the result of invoking the transform function on each inner observable and switching to it.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting from a command property that can change
    /// this.WhenAnyValue(x => x.Command)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsBusy = isExecuting
    ///     );
    ///
    /// // Or use with ToProperty
    /// _isBusy = this.WhenAnyValue(x => x.Command)
    ///     .SwitchSelect(cmd => cmd.IsExecuting)
    ///     .ToProperty(this, x => x.IsBusy);
    /// </code>
    /// </example>
    public static IObservable<TResult> SwitchSelect<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return source
            .WhereNotNull()
            .Select(selector)
            .Switch();
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable and subscribes with the provided action.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <param name="onNext">Action to invoke for each element in the projected observable sequences.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting from a command property that can change
    /// this.WhenAnyValue(x => x.Command)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsBusy = isExecuting
    ///     );
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector,
        Action<TResult> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return source
            .SwitchSelect(selector)
            .Subscribe(onNext);
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable and subscribes with the provided handlers.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <param name="onNext">Action to invoke for each element in the projected observable sequences.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector,
        Action<TResult> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return source
            .SwitchSelect(selector)
            .Subscribe(onNext, onError, onCompleted);
    }

    /// <summary>
    /// Subscribes to command execution results from a command property,
    /// automatically switching to new command instances when the property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="onNext">Action to invoke for each command execution result.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="onNext"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to command results, following command property changes
    /// this.WhenAnyValue(x => x.SaveCommand)
    ///     .SwitchSubscribe(result => Console.WriteLine($"Saved: {result}"));
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TParam, TResult>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Action<TResult> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return source
            .WhereNotNull()
            .Switch()
            .Subscribe(onNext);
    }

    /// <summary>
    /// Subscribes to command execution results from a command property with error and completion handlers,
    /// automatically switching to new command instances when the property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="onNext">Action to invoke for each command execution result.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TParam, TResult>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Action<TResult> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return source
            .WhereNotNull()
            .Switch()
            .Subscribe(onNext, onError, onCompleted);
    }

    /// <summary>
    /// Projects a command property to one of its observables (e.g., IsExecuting, CanExecute),
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command (e.g., <c>cmd => cmd.IsExecuting</c>).</param>
    /// <returns>An observable sequence that switches to the selected observable whenever the command changes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Use with ToProperty to track IsExecuting from a replaceable command
    /// _isBusy = this.WhenAnyValue(x => x.SaveCommand)
    ///     .SwitchSelect(cmd => cmd.IsExecuting)
    ///     .ToProperty(this, x => x.IsBusy);
    ///
    /// // Or subscribe directly
    /// this.WhenAnyValue(x => x.DeleteCommand)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.CanExecute,
    ///         canExecute => DeleteButtonEnabled = canExecute
    ///     );
    /// </code>
    /// </example>
    public static IObservable<TValue> SwitchSelect<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return source
            .WhereNotNull()
            .Select(selector)
            .Switch();
    }

    /// <summary>
    /// Projects a command property to one of its observables and subscribes with the provided action,
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command.</param>
    /// <param name="onNext">Action to invoke for each value from the selected observable.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting, following command property changes
    /// this.WhenAnyValue(x => x.LoadCommand)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsLoading = isExecuting
    ///     );
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector,
        Action<TValue> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return source
            .SwitchSelect(selector)
            .Subscribe(onNext);
    }

    /// <summary>
    /// Projects a command property to one of its observables and subscribes with the provided handlers,
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command.</param>
    /// <param name="onNext">Action to invoke for each value from the selected observable.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector,
        Action<TValue> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return source
            .SwitchSelect(selector)
            .Subscribe(onNext, onError, onCompleted);
    }
}
