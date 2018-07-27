// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Views;
using Java.Interop;
using Android.Support.V7.App;
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
		static readonly Dictionary<string, int> controlIds;
		static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache =
			new ConditionalWeakTable<object, Dictionary<string, View>>();

		static readonly MethodInfo getControlActivity;
		static readonly MethodInfo getControlView;

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
		///
		/// </summary>
		/// <returns></returns>
		public static T GetControl<T>(this AppCompatActivity This, [CallerMemberName]string propertyName = null)
			where T : View
		{
			return (T)getCachedControl(propertyName, This,
				() => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
		}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static T GetControl<T>(this View This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
        }

		/// <summary>
		/// A helper method to automatically resolve properties in an <see cref="Android.Support.V4.App.Fragment"/> to their respective elements in the layout.
		/// This should be called in the Fragement's OnCreateView, with the newly inflated layout
		/// </summary>
		/// <param name="This"></param>
		/// <param name="inflatedView">The newly inflated <see cref="View"/> returned from <see cref="LayoutInflater.Inflate"/>.</param>
		/// <param name="resolveMembers">The strategy used to resolve properties that either subclass <see cref="View"/>, have a <see cref="WireUpResourceAttribute"/> or have a <see cref="IgnoreResourceAttribute"/></param>
		public static void WireUpControls(this global::Android.Support.V4.App.Fragment This, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
		{
			var members = This.getWireUpMembers(ResolveStrategy.Implicit);

			members.ToList().ForEach(m => {
				try {
					// Find the android control with the same name from the view
					var view = inflatedView.getControlInternal(m.PropertyType, m.getResourceName());

					// Set the activity field's value to the view with that identifier
					m.SetValue(This, view);
				} catch (Exception ex) {
					throw new MissingFieldException("Failed to wire up the Property "
						+ m.Name + " to a View in your layout with a corresponding identifier", ex);
				}
			});
		}

		// Copied from ReactiveUI/Platforms/android/ControlFetcherMixins.cs
		static IEnumerable<PropertyInfo> getWireUpMembers(this object This, ResolveStrategy resolveStrategy)
		{
			var members = This.GetType().GetRuntimeProperties();

			switch (resolveStrategy) {
				default:
				case ResolveStrategy.Implicit:
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
		static string getResourceName(this PropertyInfo member)
		{
			var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
			return resourceNameOverride ?? member.Name;
		}

		static View getControlInternal(this View parent, Type viewType, string name)
		{
			var mi = getControlView.MakeGenericMethod(new[] { viewType });
			return (View)mi.Invoke(null, new object[] { parent, name });
		}

		static View getControlInternal(this AppCompatActivity parent, Type viewType, string name)
		{
			var mi = getControlActivity.MakeGenericMethod(new[] { viewType });
			return (View)mi.Invoke(null, new object[] { parent, name });
		}

		static View getCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
		{
			var ret = default(View);
			var ourViewCache = viewCache.GetOrCreateValue(rootView);

			if (ourViewCache.TryGetValue(propertyName, out ret)) {
				return ret;
			}

			ret = fetchControlFromView();

			ourViewCache.Add(propertyName, ret);
			return ret;
		}
	}
}
