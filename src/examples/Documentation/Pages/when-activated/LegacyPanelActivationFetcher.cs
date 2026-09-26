// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// Plugs <see cref="LegacyScorePanel"/> into ReactiveUI's activation pipeline, the way a platform package adapts its
/// own controls: it turns the panel's plain <c>Shown</c>/<c>Hidden</c> events into the <see cref="bool"/> stream
/// <see cref="ViewForMixins.WhenActivated(IActivatableView, IObservable{object})"/> and its overloads rely on.
/// </summary>
public sealed class LegacyPanelActivationFetcher : IActivationForViewFetcher
{
    /// <inheritdoc/>
    public int GetAffinityForView(Type view) => view == typeof(LegacyScorePanel) ? BindingAffinity.ExactType : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        LegacyScorePanel panel = (LegacyScorePanel)view;
        return Signal.Create<bool>(witness =>
        {
            EventHandler onShown = (_, _) => witness.OnNext(true);
            EventHandler onHidden = (_, _) => witness.OnNext(false);

            panel.Shown += onShown;
            panel.Hidden += onHidden;

            return new ActionDisposable(() =>
            {
                panel.Shown -= onShown;
                panel.Hidden -= onHidden;
            });
        });
    }
}
