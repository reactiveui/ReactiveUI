// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// Plugs <see cref="VendorDialog"/> into ReactiveUI's activation pipeline, the way a platform package's own
/// activation fetcher adapts its base classes: it turns the dialog's plain <see cref="VendorDialog.Opened"/> and
/// <see cref="VendorDialog.Closed"/> events into the <see cref="bool"/> stream
/// <see cref="ViewForMixins.WhenActivated(IActivatableView, IObservable{object})"/> and its overloads rely on.
/// Without this fetcher registered, <c>WhenActivated</c> on a <see cref="ReactiveVendorDialog{TViewModel}"/> never
/// fires, because nothing tells it when the dialog opens or closes.
/// </summary>
public sealed class VendorDialogActivationFetcher : IActivationForViewFetcher
{
    /// <inheritdoc/>
    public int GetAffinityForView(Type view) => typeof(VendorDialog).IsAssignableFrom(view) ? BindingAffinity.ExactType : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        VendorDialog dialog = (VendorDialog)view;
        return Signal.Create<bool>(witness =>
        {
            EventHandler onOpened = (_, _) => witness.OnNext(true);
            EventHandler onClosed = (_, _) => witness.OnNext(false);

            dialog.Opened += onOpened;
            dialog.Closed += onClosed;

            return new ActionDisposable(() =>
            {
                dialog.Opened -= onOpened;
                dialog.Closed -= onClosed;
            });
        });
    }
}
