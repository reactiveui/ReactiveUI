// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Android.Views;
using static ReactiveUI.ControlFetcherMixin;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace ReactiveUI.AndroidX
{
    /// <summary>
    /// ControlFetcherMixin helps you automatically wire-up Activities and
    /// Fragments via property names, similar to Butter Knife, as well as allows
    /// you to fetch controls manually.
    /// </summary>
   public static class ControlFetcherMixin
    {
        /// <summary>
        /// Wires a control to a property.
        /// This should be called in the Fragment's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Fragment fragment, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            if (fragment is null)
            {
                throw new ArgumentNullException(nameof(fragment));
            }

            var members = fragment.GetWireUpMembers(resolveMembers);

            foreach (var member in members)
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
                    throw new
                        MissingFieldException("Failed to wire up the Property " + member.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            }
        }
    }
}
