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

            try {
                controlIds = resources.GetNestedType("Id").GetFields()
                    .Where(x => x.FieldType == typeof(int))
                    .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue());
            } catch (ArgumentException argumentException) {
                var duplicates = resources.GetNestedType("Id").GetFields()
                                          .Where(x => x.FieldType == typeof(int))
                                          .GroupBy(k => k.Name.ToLowerInvariant())
                                          .Where(g => g.Count() > 1)
                                          .Select(g => "{ " + string.Join(" = ", g.Select(v => v.Name)) + " }");

                if (duplicates.Any())
                    throw new InvalidOperationException("You're using multiple resource ID's with the same name but with different casings which isn't allowed for WireUpControls: " + string.Join(", ", duplicates), argumentException);

                throw argumentException;
            }

            var type = typeof(ControlFetcherMixin);
            getControlActivity = type.GetMethod("GetControl", new[] { typeof(Activity), typeof(string) });
            getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string) });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static T GetControl<T>(this Activity This, [CallerMemberName]string propertyName = null)
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
        ///
        /// </summary>
        /// <returns></returns>
        public static T GetControl<T>(this Fragment This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return GetControl<T>(This.View, propertyName);
        }

        /// <summary>
        ///
        /// </summary>
        public static void WireUpControls(this ILayoutViewHost This, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = This.getWireUpMembers(resolveMembers);

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.View.getControlInternal(m.PropertyType, m.getResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        public static void WireUpControls(this View This, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = This.getWireUpMembers(resolveMembers);

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.getControlInternal(m.PropertyType, m.getResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        /// <summary>
        /// This should be called in the Fragement's OnCreateView, with the newly inflated layout
        /// </summary>
        /// <param name="This"></param>
        /// <param name="inflatedView"></param>
        public static void WireUpControls(this Fragment This, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = This.getWireUpMembers(resolveMembers);

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

        /// <summary>
        ///
        /// </summary>
        public static void WireUpControls(this Activity This, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
        {
            var members = This.getWireUpMembers(resolveMembers);

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.getControlInternal(m.PropertyType, m.getResourceName());

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        static View getControlInternal(this View parent, Type viewType, string name)
        {
            var mi = getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        static View getControlInternal(this Activity parent, Type viewType, string name)
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

        static string getResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

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


        public enum ResolveStrategy
        {
            /// <summary>
            /// Resolve all properties that use a subclass of View.
            /// </summary>
            Implicit,
            /// <summary>
            /// Resolve only properties with an WireUpResource attribute.
            /// </summary>
            ExplicitOptIn,
            /// <summary>
            /// Resolve all View properties and those that use a subclass of View, except those with an IgnoreResource attribute.
            /// </summary>
            ExplicitOptOut
        }
    }


    public class WireUpResourceAttribute : Attribute
    {
        public readonly string ResourceNameOverride;

        public WireUpResourceAttribute()
        {
        }

        public WireUpResourceAttribute(string resourceName)
        {
            ResourceNameOverride = resourceName;
        }
    }


    public class IgnoreResourceAttribute : Attribute
    {
    }
}
