// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A compound view showing whether an absence has been reported yet, inflated into
/// <see cref="AbsenceActivity"/>'s layout. Wires its own child with the explicit-strategy overload of
/// <c>WireUpControls</c>, opting the label in by name rather than relying on the implicit, "every View property"
/// default.</summary>
[RequiresUnreferencedCode("Wires StatusText by reflecting over this type's properties.")]
[RequiresDynamicCode("Wires StatusText by reflecting over this type's properties.")]
[System.Diagnostics.DebuggerDisplay("{StatusText}")]
public sealed class AbsenceStatusView : FrameLayout
{
    /// <summary>Initializes a new instance of the <see cref="AbsenceStatusView"/> class from an XML layout tag.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The XML attributes supplied by the layout inflater.</param>
    public AbsenceStatusView(Context context, IAttributeSet? attrs)
        : base(context, attrs)
    {
        LayoutInflater.From(context)!.Inflate(Resource.Layout.absence_status, this, true);

        // ExplicitOptIn: only StatusText, which carries [WireUpResource], is wired.
        this.WireUpControls(ControlFetcherMixins.ResolveStrategy.ExplicitOptIn);
    }

    /// <summary>Initializes a new instance of the <see cref="AbsenceStatusView"/> class from a JNI handle; required
    /// by the Android runtime's activation path, not called directly by app code.</summary>
    /// <param name="handle">The JNI handle supplied by the Android runtime.</param>
    /// <param name="ownership">The ownership of <paramref name="handle"/>.</param>
    public AbsenceStatusView(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <summary>Gets or sets the label showing the current status text.</summary>
    [WireUpResource("statusText")]
    public TextView? StatusText { get; set; }

    /// <summary>Updates the status text to reflect whether the absence has been reported.</summary>
    /// <param name="reported">Whether the absence has been recorded by the attendance service yet.</param>
    public void SetReported(bool reported) => StatusText!.Text = reported ? "Reported" : "Pending";
}
