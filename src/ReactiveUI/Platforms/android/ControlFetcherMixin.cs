﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// ControlFetcherMixin helps you automatically wire-up Activities and
    /// Fragments via property names, similar to Butter Knife, as well as allows
    /// you to fetch controls manually.
    /// </summary>
   public static partial class ControlFetcherMixin
    {
        private static readonly ConcurrentDictionary<Assembly, Dictionary<string, int>> _controlIds
            = new ConcurrentDictionary<Assembly, Dictionary<string, int>>();

        private static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache
            = new ConditionalWeakTable<object, Dictionary<string, View>>();

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static View GetControl(this Activity activity, [CallerMemberName] string propertyName = null)
            => GetCachedControl(propertyName, activity, () => activity.FindViewById(GetControlIdByName(activity.GetType().Assembly, propertyName)));

        /// <summary>
        /// Gets the control from a view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="assembly">The assembly containing the user-defined view.</param>
        /// <param name="propertyName">The property.</param>
        /// <returns>The return view.</returns>
        public static View GetControl(this View view, Assembly assembly, [CallerMemberName] string propertyName = null)
            => GetCachedControl(propertyName, view, () => view.FindViewById(GetControlIdByName(assembly, propertyName)));

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="layoutHost">The layout view host.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this ILayoutViewHost layoutHost, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = layoutHost.GetWireUpMembers(resolveMembers).ToList();
            foreach (var member in members)
            {
                try
                {
                    var view = layoutHost.View.GetControl(layoutHost.GetType().Assembly, member.GetResourceName());
                    member.SetValue(layoutHost, view);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + member.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            }
        }

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this View view, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = view.GetWireUpMembers(resolveMembers);

            foreach (var member in members)
            {
                try
                {
                    // Find the android control with the same name
                    var currentView = view.GetControl(view.GetType().Assembly, member.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    member.SetValue(view, currentView);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + member.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            }
        }

        /// <summary>
        /// Wires a control to a property.
        /// This should be called in the Fragment's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Fragment fragment, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
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

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Activity activity, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = activity.GetWireUpMembers(resolveMembers);

            foreach (var member in members)
            {
                try
                {
                    // Find the android control with the same name
                    var view = activity.GetControl(member.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    member.SetValue(activity, view);
                }
                catch (Exception ex)
                {
                    throw new
                        MissingFieldException("Failed to wire up the Property " + member.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            }
        }

        internal static IEnumerable<PropertyInfo> GetWireUpMembers(this object @this, ResolveStrategy resolveStrategy)
        {
            var members = @this.GetType().GetRuntimeProperties();

            switch (resolveStrategy)
            {
                default: // Implicit matches the Default.
                    return members.Where(member => member.PropertyType.IsSubclassOf(typeof(View))
                                              || member.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptIn:
                    return members.Where(member => member.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptOut:
                    return members.Where(member => typeof(View).IsAssignableFrom(member.PropertyType)
                                              && member.GetCustomAttribute<IgnoreResourceAttribute>(true) == null);
            }
        }

        internal static string GetResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

        private static View GetCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
        {
            View ret;
            var ourViewCache = viewCache.GetOrCreateValue(rootView);

            if (ourViewCache.TryGetValue(propertyName, out ret))
            {
                return ret;
            }

            ret = fetchControlFromView();

            ourViewCache.Add(propertyName, ret);
            return ret;
        }

        private static int GetControlIdByName(Assembly assembly, string name)
        {
            var ids = _controlIds.GetOrAdd(
                assembly,
                currentAssembly =>
                {
                    var resources = currentAssembly.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

                    return resources.GetNestedType("Id").GetFields()
                        .Where(x => x.FieldType == typeof(int))
                        .ToDictionary(k => k.Name, v => (int)v.GetRawConstantValue(), StringComparer.InvariantCultureIgnoreCase);
                });

            return ids[name];
        }
    }
}
