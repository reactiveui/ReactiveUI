// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Windows;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

/// <summary>
/// Creates a observable for a property if available that is based on a DependencyProperty.
/// </summary>
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <summary>The affinity returned when the type exposes a matching dependency property.</summary>
    private const int DependencyPropertyAffinity = 4;

    /// <summary>The scheduler used to detach the value-changed handler on disposal, or null to use the default.</summary>
    private IScheduler? _scheduler;

    /// <summary>
    /// Gets or sets the scheduler on which the dependency-property value-changed handler is detached when the
    /// subscription is disposed. The detach must run on the owning dispatcher thread, so this defaults to
    /// <see cref="RxSchedulers.MainThreadScheduler"/> (the dispatcher in production). Tests can override it with a
    /// synchronous scheduler (e.g. <see cref="ImmediateScheduler"/>) so the handler is detached inline.
    /// </summary>
    internal IScheduler Scheduler
    {
        get => _scheduler ?? RxSchedulers.MainThreadScheduler;
        set => _scheduler = value;
    }

    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc/>
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        if (type is null || !typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        return GetDependencyProperty(type, propertyName) is not null ? DependencyPropertyAffinity : 0;
    }

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        System.Linq.Expressions.Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        System.Linq.Expressions.Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        System.Linq.Expressions.Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);

        var type = sender.GetType();

        var dependencyProperty = GetDependencyProperty(type, propertyName) ?? throw new ArgumentException(
            $"The property {propertyName} does not have a dependency property.",
            nameof(propertyName));
        var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, type);

        if (dependencyPropertyDescriptor is null)
        {
            if (!suppressWarnings)
            {
                this.Log().Error("Couldn't find dependency property " + propertyName + " on " + type.Name);
            }

            throw new InvalidOperationException("Couldn't find dependency property " + propertyName + " on " + type.Name);
        }

        return new FromEventObservable<IObservedChange<object, object?>>(onNext =>
        {
            var handler = new EventHandler((_, _) =>
                onNext(new ObservedChange<object, object?>(sender, expression, null)));

            dependencyPropertyDescriptor.AddValueChanged(sender, handler);

            return new ActionDisposable(() =>
                Scheduler.Schedule(
                    (Descriptor: dependencyPropertyDescriptor, Sender: sender, Handler: handler),
                    static (_, state) =>
                    {
                        state.Descriptor.RemoveValueChanged(state.Sender, state.Handler);
                        return EmptyDisposable.Instance;
                    }));
        });
    }

    /// <summary>Gets the dependency property for the named property on the supplied type, if any.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="propertyName">The property name to resolve.</param>
    /// <returns>The matching dependency property, or null when none exists.</returns>
    private static DependencyProperty? GetDependencyProperty(Type type, string propertyName)
    {
        var fi = Array.Find(
            type.GetTypeInfo().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public),
            x => x.Name == propertyName + "Property" && x.IsStatic);

        return (DependencyProperty?)fi?.GetValue(null);
    }
}
