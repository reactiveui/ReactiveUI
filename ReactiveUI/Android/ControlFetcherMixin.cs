using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Views;
using Java.Interop;
#if ANDROID_2
using SupportFragment = Android.Support.V4.App.Fragment;
#endif

namespace ReactiveUI.Android
{
    public static class ControlFetcherMixin
    {
        static readonly Dictionary<string, int> controlIds;
        static readonly ConditionalWeakTable<object, Dictionary<string, View>> viewCache = new ConditionalWeakTable<object, Dictionary<string, View>>();

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
            getControlActivity = type.GetMethod("GetControl", new[] { typeof(Activity), typeof(string), typeof(string) });
            getControlView = type.GetMethod("GetControl", new[] { typeof(View), typeof(string), typeof(string) });
        }

        public static T GetControl<T>(this Activity This, [CallerMemberName]string propertyName = null, string prefix = null)
            where T : View
        {
            if (!String.IsNullOrEmpty(prefix) && !String.IsNullOrEmpty(propertyName))
            {
                propertyName = string.Format("{0}_{1}", prefix, propertyName);
            }
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
        }

        public static T GetControl<T>(this View This, [CallerMemberName]string propertyName = null, string prefix = null)
            where T : View
        {
            if (!String.IsNullOrEmpty(prefix) && !String.IsNullOrEmpty(propertyName))
            {
                propertyName = string.Format("{0}_{1}", prefix, propertyName);
            }
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]).JavaCast<T>());
        }
#if ANDROID_4
        public static T GetControl<T>(this Fragment This, [CallerMemberName]string propertyName = null, string prefix = null)
            where T : View
        {
            return GetControl<T>(This.View, propertyName, prefix);
        }
#endif
#if ANDROID_2
        public static T GetControl<T>(this SupportFragment This, [CallerMemberName]string propertyName = null, string prefix = null)
            where T : View
        {
            return GetControl<T>(This.View, propertyName, prefix);
        }
#endif
        public static void WireUpControls(this ILayoutViewHost This, string prefix = null)
        {
            var members = This.GetType().GetRuntimeProperties()
                .Where(m => m.PropertyType.IsSubclassOf(typeof(View)));

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.View.getControlInternal(m.PropertyType, m.Name, prefix);

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
        /// <param name="prefix">The prefix.</param>
        public static void WireUpControls(this View This, string prefix = null)
        {
            var members = This.GetType().GetRuntimeProperties()
                .Where(m => m.PropertyType.IsSubclassOf(typeof(View)));

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.getControlInternal(m.PropertyType, m.Name, prefix);

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

#if ANDROID_2
        /// <summary>
        /// This should be called in the Fragement's OnCreateView, with the newly inflated layout
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="prefix">The prefix.</param>
        public static void WireUpControls(this SupportFragment This, View inflatedView, string prefix = null)
        {
            var members = This.GetType().GetRuntimeProperties()
                .Where(m => m.PropertyType.IsSubclassOf(typeof(View)));

            members.ToList().ForEach(m =>
            {
                try
                {
                    // Find the android control with the same name from the view
                    var view = inflatedView.getControlInternal(m.PropertyType, m.Name, prefix);

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }
#endif
#if ANDROID_4
        /// <summary>
        /// This should be called in the Fragement's OnCreateView, with the newly inflated layout
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="inflatedView">The inflated view.</param>
        /// <param name="prefix">The prefix.</param>
        public static void WireUpControls(this Fragment This, View inflatedView, string prefix = null)
        {
            var members = This.GetType().GetRuntimeProperties()
                .Where(m => m.PropertyType.IsSubclassOf(typeof(View)));

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name from the view
                    var view = inflatedView.getControlInternal(m.PropertyType, m.Name, prefix);

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }
#endif
        public static void WireUpControls(this Activity This, string prefix = null)
        {
            var members = This.GetType().GetRuntimeProperties()
                .Where(m => m.PropertyType.IsSubclassOf(typeof(View)));

            members.ToList().ForEach(m => {
                try {
                    // Find the android control with the same name
                    var view = This.getControlInternal(m.PropertyType, m.Name, prefix);

                    // Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                } catch (Exception ex) {
                    throw new MissingFieldException("Failed to wire up the Property "
                        + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        static View getControlInternal(this View parent, Type viewType, string name, string prefix = null)
        {
            var mi = getControlView.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name, prefix });
        }

        static View getControlInternal(this Activity parent, Type viewType, string name, string prefix = null)
        {
            var mi = getControlActivity.MakeGenericMethod(new[] { viewType });
            return (View)mi.Invoke(null, new object[] { parent, name, prefix });
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
