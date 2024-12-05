// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Data;

namespace ReactiveUI.Avalonia;

public static class AvaloniaObjectReactiveExtensions
{
    /// <summary>
    /// Gets a subject for an <see cref="AvaloniaProperty"/>.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="priority">
    /// The priority with which binding values are written to the object.
    /// </param>
    /// <returns>
    /// An <see cref="ISubject{T}"/> which can be used for two-way binding to/from the 
    /// property.
    /// </returns>
    public static ISubject<object?> GetSubject(
        this AvaloniaObject o,
        AvaloniaProperty property,
        BindingPriority priority = BindingPriority.LocalValue)
    {
        return Subject.Create<object?>(
            Observer.Create<object?>(x => o.SetValue(property, x, priority)),
            o.GetObservable(property));
    }

    /// <summary>
    /// Gets a subject for an <see cref="AvaloniaProperty"/>.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="priority">
    /// The priority with which binding values are written to the object.
    /// </param>
    /// <returns>
    /// An <see cref="ISubject{T}"/> which can be used for two-way binding to/from the 
    /// property.
    /// </returns>
    public static ISubject<T> GetSubject<T>(
        this AvaloniaObject o,
        AvaloniaProperty<T> property,
        BindingPriority priority = BindingPriority.LocalValue)
    {
        return Subject.Create<T>(
            Observer.Create<T>(x => o.SetValue(property, x, priority)),
            o.GetObservable(property));
    }

    /// <summary>
    /// Gets a subject for a <see cref="AvaloniaProperty"/>.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="priority">
    /// The priority with which binding values are written to the object.
    /// </param>
    /// <returns>
    /// An <see cref="ISubject{Object}"/> which can be used for two-way binding to/from the 
    /// property.
    /// </returns>
    public static ISubject<BindingValue<object?>> GetBindingSubject(
        this AvaloniaObject o,
        AvaloniaProperty property,
        BindingPriority priority = BindingPriority.LocalValue)
    {
        return Subject.Create<BindingValue<object?>>(
            Observer.Create<BindingValue<object?>>(x =>
            {
                if (x.HasValue)
                {
                    o.SetValue(property, x.Value, priority);
                }
            }),
            o.GetBindingObservable(property));
    }

    /// <summary>
    /// Gets a subject for a <see cref="AvaloniaProperty"/>.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="priority">
    /// The priority with which binding values are written to the object.
    /// </param>
    /// <returns>
    /// An <see cref="ISubject{T}"/> which can be used for two-way binding to/from the 
    /// property.
    /// </returns>
    public static ISubject<BindingValue<T>> GetBindingSubject<T>(
        this AvaloniaObject o,
        AvaloniaProperty<T> property,
        BindingPriority priority = BindingPriority.LocalValue)
    {
        return Subject.Create<BindingValue<T>>(
            Observer.Create<BindingValue<T>>(x =>
            {
                if (x.HasValue)
                {
                    o.SetValue(property, x.Value, priority);
                }
            }),
            o.GetBindingObservable(property));
    }
}
