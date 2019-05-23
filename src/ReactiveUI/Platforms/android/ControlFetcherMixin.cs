﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Views;
using Java.Interop;

namespace ReactiveUI
{
    /// <summary>
    /// ControlFetcherMixin helps you automatically wire-up Activities and
    /// Fragments via property names, similar to Butter Knife, as well as allows
    /// you to fetch controls manually.
    /// </summary>
    public static partial class ControlFetcherMixin
    {
        private static readonly ConditionalWeakTable<string, View> _viewCache =
            new ConditionalWeakTable<string, View>();

        static ControlFetcherMixin()
        {
        }

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="activity">The activity.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Activity activity, [CallerMemberName] string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName,
                                                  () => activity
                                                        .FindViewById(GetResourceId(activity, propertyName))
                                                        .JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="propertyName">The property.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this View view, [CallerMemberName] string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName,
                                                  () => view.FindViewById(GetResourceId(view, propertyName))
                                                            .JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="fragment">The fragment.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Fragment fragment, [CallerMemberName] string propertyName = null)
            where T : View => GetControl<T>(fragment.View, propertyName);

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="layoutHost">The layout view host.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this ILayoutViewHost layoutHost,
                                          ReactiveUI.ControlFetcherMixin.ResolveStrategy resolveMembers =
                                              ReactiveUI.ControlFetcherMixin.ResolveStrategy.Implicit)
        {
            var members = layoutHost.GetWireUpMembers(resolveMembers).ToList();
            members.ForEach(m =>
            {
                try
                {
                    var view = layoutHost.View.GetControlInternal(m.GetResourceName());
                    m.SetValue(layoutHost, view);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier",
                                              ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this View view,
                                          ReactiveUI.ControlFetcherMixin.ResolveStrategy resolveMembers =
                                              ReactiveUI.ControlFetcherMixin.ResolveStrategy.Implicit)
        {
            var members = view.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var currentView = view.GetControlInternal(m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(view, currentView);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier",
                                              ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// This should be called in the Fragment's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Fragment fragment, View inflatedView,
                                          ReactiveUI.ControlFetcherMixin.ResolveStrategy resolveMembers =
                                              ReactiveUI.ControlFetcherMixin.ResolveStrategy.Implicit)
        {
            var members = fragment.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name from the view
                    var view = inflatedView.GetControlInternal(m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(fragment, view);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier",
                                              ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Activity activity,
                                          ReactiveUI.ControlFetcherMixin.ResolveStrategy resolveMembers =
                                              ReactiveUI.ControlFetcherMixin.ResolveStrategy.Implicit)
        {
            var members = activity.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = activity.GetControlInternal(m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(activity, view);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier",
                                              ex);
                }
            });
        }

        private static View GetControlInternal(this View parent, string resourceName)
        {
            var context = parent.Context;
            var res = context.Resources;
            var id = res.GetIdentifier(resourceName, "id", context.PackageName);
            return parent.FindViewById(id);
        }

        private static View GetControlInternal(this Activity parent, string resourceName)
        {
            return parent.FindViewById(GetResourceId(parent, resourceName));
        }

        private static int GetResourceId(Activity activity, string resourceName)
        {
            var res = activity.Resources;
            return res.GetIdentifier(resourceName, "id", activity.PackageName);
        }

        private static int GetResourceId(View view, string resourceName)
        {
            var res = view.Context.Resources;
            return res.GetIdentifier(resourceName, "id", view.Context.PackageName);
        }

        private static View GetCachedControl(string propertyName, Func<View> fetchControlFromView)
        {
            var ourViewCache = _viewCache.GetOrCreateValue(propertyName);

            if (ourViewCache != null)
            {
                return ourViewCache;
            }

            var view = fetchControlFromView();
            _viewCache.Add(propertyName, view);
            return view;
        }

        private static string GetResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

        private static IEnumerable<PropertyInfo> GetWireUpMembers(this object @this,
                                                                  ReactiveUI.ControlFetcherMixin.ResolveStrategy
                                                                      resolveStrategy)
        {
            var members = @this.GetType().GetRuntimeProperties();

            switch (resolveStrategy)
            {
                default: // Implicit matches the Default.
                    return members.Where(m => m.PropertyType.IsSubclassOf(typeof(View))
                                              || m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ReactiveUI.ControlFetcherMixin.ResolveStrategy.ExplicitOptIn:
                    return members.Where(m => m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ReactiveUI.ControlFetcherMixin.ResolveStrategy.ExplicitOptOut:
                    return members.Where(m => typeof(View).IsAssignableFrom(m.PropertyType)
                                              && m.GetCustomAttribute<IgnoreResourceAttribute>(true) == null);
            }
        }
    }
}
