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
    /// ControlFetcherMixin helps you automatically wire-up Activities and Fragments via property
    /// names, similar to Butter Knife, as well as allows you to fetch controls manually.
    /// </summary>
    public static class ControlFetcherMixin
    {
        private static readonly Dictionary<string, int> controlIds;
        private static readonly MethodInfo getControlActivity;

        private static readonly MethodInfo getControlView;

        private static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache =
                            new ConditionalWeakTable<object, Dictionary<string, View>>();

        static ControlFetcherMixin()
        {
            // NB: This is some hacky shit, but on Xamarin.Android at the moment, this is always the
            // entry assembly.
            var assm = AppDomain.CurrentDomain.GetAssemblies()[1];
            var resources = assm.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

            controlIds = resources.GetNestedType("Id").GetFields()
                .Where(x => x.FieldType == typeof(int))
                .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue());

            var type = typeof(ControlFetcherMixin);
            getControlActivity = type.GetMethod("GetControl", new[] { typeof(Activity), typeof(string) });
            getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string) });
        }

        /// <summary>
        /// Resolve Strategy
        /// </summary>
        public enum ResolveStrategy
        {
            /// <summary>
            /// Resolve all properties that use a subclass of View.
            /// </summary>
            Implicit = 0,

            /// <summary>
            /// Resolve only properties with an WireUpResource attribute.
            /// </summary>
            ExplicitOptIn = 1,

            /// <summary>
            /// Resolve all View properties and those that use a subclass of View, except those with
            /// an IgnoreResource attribute.
            /// </summary>
            ExplicitOptOut = 2
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetControl<T>(this Activity This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetControl<T>(this View This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetControl<T>(this Fragment This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return GetControl<T>(This.View, propertyName);
        }

        /// <summary>
        /// Wires up controls.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="resolveMembers">The resolve members.</param>
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
        /// Wires up controls.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="resolveMembers">The resolve members.</param>
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
        /// <param name="This">The this.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="resolveMembers">The resolve members.</param>
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
        /// Wires up controls.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="resolveMembers">The resolve members.</param>
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

        private static View getCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
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

        private static View getControlInternal(this View parent, Type viewType, string name)
        {
            var mi = getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static View getControlInternal(this Activity parent, Type viewType, string name)
        {
            var mi = getControlActivity.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name });
        }

        private static string getResourceName(this PropertyInfo member)
        {
            var resourceNameOverride = member.GetCustomAttribute<WireUpResourceAttribute>()?.ResourceNameOverride;
            return resourceNameOverride ?? member.Name;
        }

        private static IEnumerable<PropertyInfo> getWireUpMembers(this object This, ResolveStrategy resolveStrategy)
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
    }

    /// <summary>
    /// Ignore Resource Attribute
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    public class IgnoreResourceAttribute : Attribute
    {
    }

    /// <summary>
    /// Wire Up Resource Attribute
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    public class WireUpResourceAttribute : Attribute
    {
        /// <summary>
        /// The resource name override
        /// </summary>
        public readonly string ResourceNameOverride;

        /// <summary>
        /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
        /// </summary>
        public WireUpResourceAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public WireUpResourceAttribute(string resourceName)
        {
            this.ResourceNameOverride = resourceName;
        }
    }
}