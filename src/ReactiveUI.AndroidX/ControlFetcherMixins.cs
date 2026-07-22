// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Views;
#if REACTIVE_SHIM
using static ReactiveUI.Reactive.ControlFetcherMixins;
#else
using static ReactiveUI.ControlFetcherMixins;
#endif
using Fragment = AndroidX.Fragment.App.Fragment;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>
/// ControlFetcherMixins helps you automatically wire-up Activities and
/// Fragments via property names, similar to Butter Knife, as well as allows
/// you to fetch controls manually.
/// </summary>
public static class ControlFetcherMixins
{
    /// <summary>Provides control wire-up extension members for <see cref="Fragment"/>.</summary>
    /// <param name="fragment">The fragment whose controls are wired up.</param>
    extension(Fragment fragment)
    {
        /// <summary>
        /// Wires a control to a property using the <see cref="ResolveStrategy.Implicit"/> strategy.
        /// This should be called in the Fragment's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="inflatedView">The inflated view.</param>
        [RequiresUnreferencedCode(
            "Android resource discovery uses reflection over generated resource types that may be trimmed.")]
        [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
        public void WireUpControls(View inflatedView) =>
            fragment.WireUpControls(inflatedView, ResolveStrategy.Implicit);

        /// <summary>Wires a control to a property. This should be called in the Fragment's OnCreateView, with the newly inflated layout.</summary>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        [RequiresUnreferencedCode(
            "Android resource discovery uses reflection over generated resource types that may be trimmed.")]
        [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
        [SuppressMessage(
            "Design",
            "SST1448:Let the compiler supply caller-info arguments",
            Justification = "GetResourceName() supplies the control name, which differs from the caller member name; the [CallerMemberName] default would resolve the wrong control.")]
        public void WireUpControls(
            View inflatedView,
            ResolveStrategy resolveMembers)
        {
            ArgumentExceptionHelper.ThrowIfNull(fragment);

            foreach (var member in fragment.GetWireUpMembers(resolveMembers))
            {
                try
                {
                    // Find the android control with the same name from the view
                    var view = inflatedView.GetControl(fragment.GetType().Assembly, member.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    member.SetValue(fragment, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException(
                        $"Failed to wire up the Property {member.Name} to a View in your layout with a corresponding identifier",
                        ex);
                }
            }
        }
    }
}
