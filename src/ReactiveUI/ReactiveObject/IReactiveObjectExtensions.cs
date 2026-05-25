// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ReactiveUI.Helpers;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IReactiveObject interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class IReactiveObjectExtensions
{
#if NETSTANDARD || NETFRAMEWORK
    /// <summary>Stores per-instance extension state keyed by reactive object.</summary>
    private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new();
#else
    /// <summary>Stores per-instance extension state keyed by reactive object.</summary>
    private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = [];
#endif

    /// <summary>
    /// RaiseAndSetIfChanged fully implements a Setter for a read-write
    /// property on a ReactiveObject, using CallerMemberName to raise the notification
    /// and the ref to the backing field to set the property.
    /// </summary>
    /// <typeparam name="TObj">The type of the This.</typeparam>
    /// <typeparam name="TRet">The type of the return value.</typeparam>
    /// <param name="reactiveObject">The <see cref="ReactiveObject"/> raising the notification.</param>
    /// <param name="backingField">A Reference to the backing field for this
    /// property.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">The name of the property, usually
    /// automatically provided through the CallerMemberName attribute.</param>
    /// <returns>The newly set value, normally discarded.</returns>
    [SuppressMessage("Design", "CA1045:Do not pass types by reference", Justification = "By default for this operator.")]
    public static TRet RaiseAndSetIfChanged<TObj, TRet>(
        this TObj reactiveObject,
        ref TRet backingField,
        TRet newValue,
        [CallerMemberName] string? propertyName = null)
        where TObj : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return newValue;
        }

        reactiveObject.RaisingPropertyChanging(propertyName);
        backingField = newValue;
        reactiveObject.RaisingPropertyChanged(propertyName);
        return newValue;
    }

    /// <summary>
    /// Use this method in your ReactiveObject classes when creating custom
    /// properties where raiseAndSetIfChanged doesn't suffice.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of ReactiveObject on which the property has changed.</param>
    /// <param name="propertyName">
    /// A string representing the name of the property that has been changed.
    /// Leave <c>null</c> to let the runtime set to caller member name.
    /// </param>
    public static void RaisePropertyChanged<TSender>(
        this TSender reactiveObject,
        [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is null)
        {
            return;
        }

        reactiveObject.RaisingPropertyChanged(propertyName);
    }

    /// <summary>
    /// Use this method in your ReactiveObject classes when creating custom
    /// properties where raiseAndSetIfChanged doesn't suffice.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of ReactiveObject on which the property has changed.</param>
    /// <param name="propertyName">
    /// A string representing the name of the property that has been changed.
    /// Leave <c>null</c> to let the runtime set to caller member name.
    /// </param>
    public static void RaisePropertyChanging<TSender>(
        this TSender reactiveObject,
        [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is null)
        {
            return;
        }

        reactiveObject.RaisingPropertyChanging(propertyName);
    }

    /// <summary>
    /// Use this method for enabling classic PropertyChanging events when you
    /// are implementing IReactiveObject manually.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of IReactiveObject which should propagate property changes.</param>
    public static void SubscribePropertyChangingEvents<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);

        s.SubscribeChanging();
    }

    /// <summary>
    /// Use this method for enabling classic PropertyChanged events when you
    /// are implementing IReactiveObject manually.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of IReactiveObject which should propagate property changes.</param>
    public static void SubscribePropertyChangedEvents<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);

        s.SubscribeChanged();
    }

    /// <summary>
    /// Returns an observable sequence that signals when a property on the specified reactive object has changed.
    /// </summary>
    /// <remarks>The returned observable emits events for all property changes on the provided reactive
    /// object. Subscribers can use this to react to changes in any property. The observable completes only when the
    /// reactive object is disposed, if applicable.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object to observe for property change notifications. Cannot be null.</param>
    /// <returns>An observable sequence of property change event arguments for the specified reactive object. The sequence emits
    /// a value each time a property changes.</returns>
    public static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangedObservable<TSender>(
        this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = GetState(reactiveObject);
        return new ChangeArgsCastObservable<TSender>(val.Changed);
    }

    /// <summary>
    /// Returns an observable sequence that signals before a property value changes on the specified reactive object.
    /// </summary>
    /// <remarks>Subscribers can use the returned observable to react to property changes before the values
    /// are updated. This is useful for scenarios where actions need to be taken prior to a property's value being
    /// modified.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements the IReactiveObject interface.</typeparam>
    /// <param name="reactiveObject">The reactive object to observe for property changing notifications. Must implement IReactiveObject.</param>
    /// <returns>An observable sequence of IReactivePropertyChangedEventArgs{TSender} that emits a value each time a property on
    /// the reactive object is about to change.</returns>
    public static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangingObservable<TSender>(
        this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = GetState(reactiveObject);
        return new ChangeArgsCastObservable<TSender>(val.Changing);
    }

    /// <summary>
    /// Returns an observable sequence that emits exceptions thrown by the specified reactive object during property
    /// change notifications or command executions.
    /// </summary>
    /// <remarks>Subscribers can use the returned observable to monitor and handle exceptions that occur
    /// within the reactive object's reactive operations. This is useful for centralized error handling in reactive UI
    /// or data models.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements the IReactiveObject interface.</typeparam>
    /// <param name="reactiveObject">The reactive object for which to observe thrown exceptions. Must implement IReactiveObject.</param>
    /// <returns>An observable sequence of Exception objects representing errors thrown by the reactive object. The sequence
    /// completes when the reactive object is disposed.</returns>
    public static IObservable<Exception> GetThrownExceptionsObservable<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);
        return s.ThrownExceptions;
    }

    /// <summary>
    /// Temporarily suppresses change notifications for the specified reactive object.
    /// </summary>
    /// <remarks>While change notifications are suppressed, property change and other reactive notifications
    /// will not be raised. Dispose the returned object to resume normal notification behavior. This method is typically
    /// used to batch multiple changes and avoid triggering notifications for each individual change.</remarks>
    /// <typeparam name="TSender">The type of the reactive object for which to suppress change notifications. Must implement <see
    /// cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object whose change notifications are to be suppressed.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, restores change notifications for the specified object.</returns>
    public static IDisposable SuppressChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);

        return s.Suppress();
    }

    /// <summary>
    /// Determines whether change notifications are currently enabled for the specified reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object. Must implement <see cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object to check for change notification support. Cannot be null.</param>
    /// <returns>true if change notifications are enabled for the specified object; otherwise, false.</returns>
    public static bool AreChangeNotificationsEnabled<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);

        return s.NotificationsEnabled();
    }

    /// <summary>
    /// Temporarily suspends change notifications for the specified reactive object until the returned disposable is
    /// disposed.
    /// </summary>
    /// <remarks>Use this method to batch multiple changes to a reactive object and suppress intermediate
    /// change notifications. Change notifications are resumed automatically when the returned disposable is disposed.
    /// This is useful for improving performance or preventing unnecessary updates when making several changes in quick
    /// succession.</remarks>
    /// <typeparam name="TSender">The type of the reactive object for which change notifications are to be delayed. Must implement <see
    /// cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object whose change notifications will be delayed. Cannot be null.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, resumes change notifications for the specified object.</returns>
    internal static IDisposable DelayChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = GetState(reactiveObject);

        return s.Delay();
    }

    /// <summary>
    /// Raises the PropertyChanging event for the specified property on the given reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object on which to raise the PropertyChanging event.</param>
    /// <param name="propertyName">The name of the property for which the PropertyChanging event is raised. Cannot be null.</param>
    internal static void RaisingPropertyChanging<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        var s = GetState(reactiveObject);

        s.RaiseChanging(propertyName);
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property on the given reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object on which to raise the PropertyChanged event.</param>
    /// <param name="propertyName">The name of the property for which the PropertyChanged event is raised. Cannot be null.</param>
    internal static void RaisingPropertyChanged<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        var s = GetState(reactiveObject);

        s.RaiseChanged(propertyName);
    }

    /// <summary>
    /// Gets the per-instance extension state for <paramref name="reactiveObject"/>, creating it on first access.
    /// When the object implements <see cref="IReactiveObjectStateSlot"/> (the fast path used by
    /// <see cref="ReactiveObject"/> and the platform UI base types) the state is stored directly on the instance —
    /// no process-wide table lookup, and one allocation per object. Hand-rolled <see cref="IReactiveObject"/>
    /// implementers that do not expose a slot fall back to a <see cref="ConditionalWeakTable{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TSender">The reactive object type.</typeparam>
    /// <param name="reactiveObject">The reactive object whose state is required.</param>
    /// <returns>The extension state for the object.</returns>
    private static IExtensionState<IReactiveObject> GetState<TSender>(TSender reactiveObject)
        where TSender : IReactiveObject
    {
        if (reactiveObject is not IReactiveObjectStateSlot slotHost)
        {
            // Use the callback's key argument rather than capturing reactiveObject: a capturing lambda forces the
            // compiler to allocate a closure at method entry on *every* GetState call (including the slot fast path
            // below), and GetState runs on every property-change notification.
            return state.GetValue(
                reactiveObject,
                static key => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>((TSender)key));
        }

        ref var slot = ref slotHost.GetReactiveStateSlot();
        if (Volatile.Read(ref slot) is IExtensionState<IReactiveObject> existing)
        {
            return existing;
        }

        var created = (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject);
        return Interlocked.CompareExchange(ref slot, created, null) as IExtensionState<IReactiveObject> ?? created;
    }

    /// <summary>
    /// Re-types the reactive object's change-argument stream from the non-generic <see cref="IReactiveObject"/> form to
    /// the caller's <typeparamref name="TSender"/>. Specialised to <see cref="GetChangedObservable{TSender}"/> and
    /// <see cref="GetChangingObservable{TSender}"/>.
    /// </summary>
    /// <typeparam name="TSender">The reactive object type observed.</typeparam>
    /// <param name="source">The source change-argument stream.</param>
    private sealed class ChangeArgsCastObservable<TSender>(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> source)
        : IObservable<IReactivePropertyChangedEventArgs<TSender>>
        where TSender : IReactiveObject
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IReactivePropertyChangedEventArgs<TSender>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Re-types each change-argument value to the caller's sender type.</summary>
        /// <param name="downstream">The observer receiving re-typed change arguments.</param>
        private sealed class Sink(IObserver<IReactivePropertyChangedEventArgs<TSender>> downstream)
            : IObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactivePropertyChangedEventArgs<IReactiveObject> value) =>
                downstream.OnNext((IReactivePropertyChangedEventArgs<TSender>)(object)value);

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
