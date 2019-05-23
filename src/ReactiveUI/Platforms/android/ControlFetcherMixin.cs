// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly ConcurrentDictionary<Assembly, Dictionary<string, int>> _controlIds
            = new ConcurrentDictionary<Assembly, Dictionary<string, int>>();

        private static readonly ConditionalWeakTable<object, Dictionary<string, View>> _viewCache =
            new ConditionalWeakTable<object, Dictionary<string, View>>();

        private static readonly MethodInfo _getControlActivity;
        private static readonly MethodInfo _getControlView;

        [SuppressMessage("Design", "CA1065: Not not throw exceptions in static constructor", Justification = "TODO: Future fix")]
        static ControlFetcherMixin()
        {
            var type = typeof(ControlFetcherMixin);
            _getControlActivity = type.GetMethod("GetControl", new[] { typeof(Activity), typeof(string) });
            _getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string) });
        }

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="activity">The activity.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Activity activity, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, activity, () => activity.FindViewById(GetControlIdByName(activity.GetType().Assembly, propertyName)).JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="propertyName">The property.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this View view, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, view, () => view.FindViewById(GetControlIdByName(view.GetType().Assembly, propertyName)).JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="fragment">The fragment.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Fragment fragment, [CallerMemberName]string propertyName = null)
            where T : View => GetControl<T>(fragment.View, propertyName);

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="layoutHost">The layout view host.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this ILayoutViewHost layoutHost, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = layoutHost.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = layoutHost.View.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(layoutHost, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this View view, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = view.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var currentView = view.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(view, currentView);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
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
        public static void WireUpControls(this Fragment fragment, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = fragment.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name from the view
                    var view = inflatedView.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(fragment, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="activity">The Activity.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Activity activity, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = activity.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = activity.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(activity, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        private static View GetControlInternal(this View parent, Type viewType, string name)
        {
            var mi = _getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetControlInternal(this Activity parent, Type viewType, string name)
        {
            var mi = _getControlActivity.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
        {
            var ourViewCache = _viewCache.GetOrCreateValue(rootView);

            if (ourViewCache.TryGetValue(propertyName, out View view))
            {
                return view;
            }

            view = fetchControlFromView();

            ourViewCache.Add(propertyName, view);
            return view;
        }

        private static string GetResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

        private static IEnumerable<PropertyInfo> GetWireUpMembers(this object @this, ResolveStrategy resolveStrategy)
        {
            var members = @this.GetType().GetRuntimeProperties();

            switch (resolveStrategy)
            {
                default: // Implicit matches the Default.
                    return members.Where(m => m.PropertyType.IsSubclassOf(typeof(View))
                                         || m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptIn:
                    return members.Where(m => m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptOut:
                    return members.Where(m => typeof(View).IsAssignableFrom(m.PropertyType)
                                         && m.GetCustomAttribute<IgnoreResourceAttribute>(true) == null);
            }
        }

        private static int GetControlIdByName(Assembly assembly, string name)
        {
            var ids = _controlIds.GetOrAdd(
                assembly,
                currentAssembly =>
                {
                    var resources = currentAssembly.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

                    try
                    {
                        return resources.GetNestedType("Id").GetFields()
                            .Where(x => x.FieldType == typeof(int))
                            .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue(), StringComparer.InvariantCultureIgnoreCase);
                    }
                    catch (ArgumentException argumentException)
                    {
                        var duplicates = resources.GetNestedType("Id").GetFields()
                                                  .Where(x => x.FieldType == typeof(int))
                                                  .GroupBy(k => k.Name.ToLowerInvariant())
                                                  .Where(g => g.Count() > 1)
                                                  .Select(g => "{ " + string.Join(" = ", g.Select(v => v.Name)) + " }")
                                                  .ToList();

                        if (duplicates.Any())
                        {
                            throw new InvalidOperationException("You're using multiple resource ID's with the same name but with different casings which isn't allowed for WireUpControls: " + string.Join(", ", duplicates), argumentException);
                        }

                        throw;
                    }
                });

            return ids[name];
        }
    }
}
