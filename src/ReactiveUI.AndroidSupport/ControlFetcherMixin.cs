// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Support.V7.App;
using Android.Views;
using Java.Interop;
using static ReactiveUI.ControlFetcherMixin;

namespace ReactiveUI.AndroidSupport
{
    /// <summary>
    /// ControlFetcherMixin helps you automatically wire-up Activities and
    /// Fragments via property names, similar to Butter Knife, as well as allows
    /// you to fetch controls manually.
    /// </summary>
    public static class ControlFetcherMixin
    {
        private static readonly Dictionary<string, int> controlIds;

        private static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache =
            new ConditionalWeakTable<object, Dictionary<string, View>>();

        private static readonly MethodInfo getControlActivity;
        private static readonly MethodInfo getControlView;

        static ControlFetcherMixin()
        {
            // NB: This is some hacky shit, but on Xamarin.Android at the moment,
            // this is always the entry assembly.
            var assm = AppDomain.CurrentDomain.GetAssemblies()[1];
            var resources = assm.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

            controlIds = resources.GetNestedType("Id").GetFields()
                .Where(x => x.FieldType == typeof(int))
                .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue());

            var type = typeof(ControlFetcherMixin);
            getControlActivity = type.GetMethod("GetControl", new[] { typeof(AppCompatActivity), typeof(string) });
            getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string) });
        }

        /// <summary>
        /// Gets the control from an activiy.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="this">The activity.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns a view.</returns>
        public static T GetControl<T>(this AppCompatActivity @this, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, @this, () => @this.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());

        /// <summary>
        /// Gets the control from a view.
        /// </summary>
        /// <typeparam name="T">The control type.</typeparam>
        /// <param name="this">The view.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>A <see cref="View"/>.</returns>
        public static T GetControl<T>(this View @this, [CallerMemberName]string propertyName = null)
            where T : View => (T)GetCachedControl(propertyName, @this, () => @this.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());

        /// <summary>
        /// A helper method to automatically resolve properties in an <see cref="Android.Support.V4.App.Fragment"/> to their respective elements in the layout.
        /// This should be called in the Fragement's OnCreateView, with the newly inflated layout.
        /// </summary>
        /// <param name="this">The fragment.</param>
        /// <param name="inflatedView">The newly inflated <see cref="View"/> returned from Inflate.</param>
        /// <param name="resolveMembers">The strategy used to resolve properties that either subclass <see cref="View"/>, have a <see cref="WireUpResourceAttribute"/> or have a <see cref="IgnoreResourceAttribute"/>.</param>
        public static void WireUpControls(this Android.Support.V4.App.Fragment @this, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
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
                    throw new MissingFieldException(
                        "Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        // Copied from ReactiveUI/Platforms/android/ControlFetcherMixins.cs
        private static IEnumerable<PropertyInfo> GetWireUpMembers(this object @this, ResolveStrategy resolveStrategy)
        {
            var members = @this.GetType().GetRuntimeProperties();

            switch (resolveStrategy)
            {
                default: // Implicit uses the default case.
                    return members.Where(m => m.PropertyType.IsSubclassOf(typeof(View))
                                         || m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptIn:
                    return members.Where(m => m.GetCustomAttribute<WireUpResourceAttribute>(true) != null);

                case ResolveStrategy.ExplicitOptOut:
                    return members.Where(m => typeof(View).IsAssignableFrom(m.PropertyType)
                                         && m.GetCustomAttribute<IgnoreResourceAttribute>(true) == null);
            }
        }

        // Also copied from ReactiveUI/Platforms/android/ControlFetcherMixins.cs
        private static string GetResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

        private static View GetControlInternal(this View parent, Type viewType, string name)
        {
            var mi = getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetControlInternal(this AppCompatActivity parent, Type viewType, string name)
        {
            var mi = getControlActivity.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View GetCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
        {
            var ret = default(View);
            var ourViewCache = viewCache.GetOrCreateValue(rootView);

            if (ourViewCache.TryGetValue(propertyName, out ret))
            {
                return ret;
            }

            ret = fetchControlFromView();

            ourViewCache.Add(propertyName, ret);
            return ret;
        }
    }
}
