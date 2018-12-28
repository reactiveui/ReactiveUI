// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
        private static readonly Dictionary<string, int> controlIds;

        private static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache =
            new ConditionalWeakTable<object, Dictionary<string, View>>();

        private static readonly MethodInfo getControlActivity;
        private static readonly MethodInfo getControlView;

        [SuppressMessage("Design", "CA1065: Not not throw exceptions in static constructor", Justification = "TODO: Future fix")]
        static ControlFetcherMixin()
        {
            // NB: This is some hacky shit, but on Xamarin.Android at the moment,
            // this is always the entry assembly.
            var assm = AppDomain.CurrentDomain.GetAssemblies()[1];
            var resources = assm.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

            try
            {
                controlIds = resources.GetNestedType("Id").GetFields()
                    .Where(x => x.FieldType == typeof(int))
                    .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue());
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

            var type = typeof(ControlFetcherMixin);
            getControlActivity = type.GetMethod("GetControl", new[] { typeof(Activity), typeof(string) });
            getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string) });
        }

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="this">The activity.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Activity @this, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, @this, () => @this.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="this">The view.</param>
        /// <param name="propertyName">The property.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this View @this, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, @this, () => @this.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());

        /// <summary>
        /// Gets the control from an activity.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="this">The fragment.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The return view.</returns>
        public static T GetControl<T>(this Fragment @this, [CallerMemberName]string propertyName = null)
            where T : View => GetControl<T>(@this.View, propertyName);

        /// <summary>
        /// Wires a control to a property.
        /// </summary>
        /// <param name="this">The layout view host.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this ILayoutViewHost @this, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = @this.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = @this.View.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(@this, view);
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
        /// <param name="this">The view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this View @this, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = @this.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = @this.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(@this, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        /// <summary>
        /// Wires a control to a property.
        /// This should be called in the Fragement's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="this">The fragment.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Fragment @this, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = @this.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name from the view
                    var view = inflatedView.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(@this, view);
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
        /// <param name="this">The Activity.</param>
        /// <param name="resolveMembers">The resolve members.</param>
        public static void WireUpControls(this Activity @this, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = @this.GetWireUpMembers(resolveMembers);

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name
                    var view = @this.GetControlInternal(m.PropertyType, m.GetResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(@this, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property " + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        private static View GetControlInternal(this View parent, Type viewType, string name)
        {
            var mi = getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetControlInternal(this Activity parent, Type viewType, string name)
        {
            var mi = getControlActivity.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
        {
            var ourViewCache = viewCache.GetOrCreateValue(rootView);

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
    }
}
