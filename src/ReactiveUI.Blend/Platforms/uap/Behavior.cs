// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Xaml.Interactivity;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace ReactiveUI.Blend;

/// <summary>
/// A base class which allows us to declare our own behaviors.
/// This is based on the WPF Blend SDK based Behaviors.
/// </summary>
/// <typeparam name="T">The type of DependencyObject to create a behavior for.</typeparam>
public class Behavior<T> : DependencyObject, IBehavior
    where T : DependencyObject
{
    /// <summary>
    /// Gets the associated object.
    /// </summary>
    public T? AssociatedObject { get; private set; }

    /// <inheritdoc/>
    DependencyObject? IBehavior.AssociatedObject => AssociatedObject;

    /// <inheritdoc/>
    public virtual void Attach(DependencyObject associatedObject)
    {
        if (associatedObject == AssociatedObject || DesignMode.DesignModeEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            throw new InvalidOperationException("Cannot attach multiple objects.");
        }

        AssociatedObject = associatedObject as T;
        OnAttached();
    }

    /// <inheritdoc/>
    public virtual void Detach() => OnDetaching();

    /// <summary>
    /// Called when [attached].
    /// </summary>
    protected virtual void OnAttached()
    {
    }

    /// <summary>
    /// Called when [detaching].
    /// </summary>
    protected virtual void OnDetaching()
    {
    }
}