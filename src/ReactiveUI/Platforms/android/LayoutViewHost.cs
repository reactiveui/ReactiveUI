// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.Views;

using static ReactiveUI.ControlFetcherMixin;

namespace ReactiveUI;

/// <summary>
/// Base class implementing the Android ViewHolder pattern.
/// <para>
/// <see cref="LayoutViewHost"/> owns a single inflated <see cref="Android.Views.View"/> instance and
/// optionally wires child controls to properties on the host.
/// </para>
/// <para>
/// This type provides both AOT-safe construction paths and a legacy reflection-based
/// auto-wireup path for compatibility.
/// </para>
/// </summary>
public abstract class LayoutViewHost : ILayoutViewHost, IEnableLogger
{
    private View? _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutViewHost"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor performs no inflation or wiring and exists to support
    /// derived types that manage view creation manually.
    /// </remarks>
    protected LayoutViewHost()
    {
    }

   /// <summary>
    /// Initializes a new instance of the <see cref="LayoutViewHost"/> class by inflating
    /// a layout resource.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <remarks>
    /// <para>
    /// This constructor is fully AOT- and trimming-safe.
    /// </para>
    /// <para>
    /// No automatic control wiring is performed. Consumers are expected to
    /// wire controls explicitly.
    /// </para>
    /// </remarks>
    protected LayoutViewHost(
        Context context,
        int layoutId,
        ViewGroup parent,
        bool attachToRoot)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(parent);

        View = Inflate(context, layoutId, parent, attachToRoot);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutViewHost"/> class by inflating
    /// a layout resource and invoking an explicit, AOT-safe binder.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <param name="bind">
    /// A callback responsible for explicitly wiring child views to the host.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor is fully AOT-safe and avoids reflection entirely.
    /// </para>
    /// <para>
    /// The <paramref name="bind"/> callback is invoked only after <see cref="View"/> has been assigned.
    /// </para>
    /// </remarks>
    protected LayoutViewHost(
        Context context,
        int layoutId,
        ViewGroup parent,
        bool attachToRoot,
        Action<LayoutViewHost, View> bind)
        : this(context, layoutId, parent, attachToRoot)
    {
        ArgumentNullException.ThrowIfNull(bind);

        if (View is not null)
        {
            bind(this, View);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutViewHost"/> class by inflating
    /// a layout resource and optionally performing reflection-based auto-wireup.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <param name="performAutoWireup">
    /// If <see langword="true"/>, performs automatic wiring using reflection.
    /// </param>
    /// <param name="resolveStrategy">
    /// The member resolution strategy used during auto-wireup.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor is not trimming- or AOT-safe when auto-wireup is enabled.
    /// </para>
    /// <para>
    /// It exists for backward compatibility and should be avoided in new code.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Auto wire-up uses reflection and member discovery.")]
    [RequiresDynamicCode("Auto wire-up relies on runtime type inspection.")]
    protected LayoutViewHost(
        Context context,
        int layoutId,
        ViewGroup parent,
        bool attachToRoot,
        bool performAutoWireup,
        ResolveStrategy resolveStrategy)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(parent);

        View = Inflate(context, layoutId, parent, attachToRoot);

        if (performAutoWireup)
        {
            this.WireUpControls(resolveStrategy);
        }
    }

    /// <inheritdoc />
    public View? View
    {
        get => _view;
        set
        {
            if (ReferenceEquals(_view, value))
            {
                return;
            }

            _view = value;

            // Associate the host with the view for retrieval via ViewMixins.
            _view?.SetTag(ViewMixins.ViewHostTag, this.ToJavaObject());
        }
    }

    /// <summary>
    /// Implicitly converts a <see cref="LayoutViewHost"/> to its backing <see cref="View"/>.
    /// </summary>
    /// <param name="host">The host instance.</param>
    public static implicit operator View?(LayoutViewHost host)
    {
        ArgumentExceptionHelper.ThrowIfNull(host);
        return host._view;
    }

    /// <summary>
    /// Inflates an Android layout resource into a <see cref="View"/> using the provided context.
    /// </summary>
    /// <param name="context">The Android context used to obtain a <see cref="LayoutInflater"/>.</param>
    /// <param name="layoutId">The layout resource identifier to inflate.</param>
    /// <param name="parent">
    /// The parent view group to associate with the inflated view during inflation.
    /// This parameter may influence layout parameters even when <paramref name="attachToRoot"/> is <see langword="false"/>.
    /// </param>
    /// <param name="attachToRoot">
    /// Whether the inflated view should be attached to <paramref name="parent"/> during inflation.
    /// </param>
    /// <returns>
    /// The inflated <see cref="View"/> instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a <see cref="LayoutInflater"/> cannot be obtained from the provided <paramref name="context"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method centralizes layout inflation to avoid duplication across constructors and
    /// to ensure consistent error handling.
    /// </para>
    /// <para>
    /// The method performs no reflection and is fully compatible with AOT and trimming.
    /// </para>
    /// </remarks>
    private static View Inflate(Context context, int layoutId, ViewGroup parent, bool attachToRoot)
    {
        var inflater = LayoutInflater.FromContext(context);
        return inflater?.Inflate(layoutId, parent, attachToRoot)
               ?? throw new InvalidOperationException("LayoutInflater could not be obtained from context.");
    }
}
