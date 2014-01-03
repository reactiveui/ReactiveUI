using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Views;


namespace ReactiveUI.Android
{
    public static class ControlFetcherMixin
    {
        static readonly Dictionary<string, int> controlIds;
        static ConditionalWeakTable<object, Dictionary<string, View>> viewCache = new ConditionalWeakTable<object, Dictionary<string, View>>();

        static ControlFetcherMixin()
        {
            // NB: This is some hacky shit, but on MonoAndroid at the moment, 
            // this is always the entry assembly.
            var assm = AppDomain.CurrentDomain.GetAssemblies()[1];
            var resources = assm.GetModules().SelectMany(x => x.GetTypes()).First(x => x.Name == "Resource");

            controlIds = resources.GetNestedType("Id").GetFields()
                .Where(x => x.FieldType == typeof(int))
                .ToDictionary(k => k.Name.ToLowerInvariant(), v => (int)v.GetRawConstantValue());
        }

        public static T GetControl<T>(this Activity This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]));
        }

        public static T GetControl<T>(this View This, [CallerMemberName]string propertyName = null)
            where T : View
        {
            return (T)getCachedControl(propertyName, This,
                () => This.FindViewById(controlIds[propertyName.ToLowerInvariant()]));
        }

        public static void WireUpControls(this View This)
        {
            // Auto wire-up
            //Get all the View properties from the activity
            var members = from m in This.GetType().GetRuntimeProperties()
                          where m.PropertyType.IsSubclassOf(typeof(View))
                          select m;

            members.ToList().ForEach(m =>
            {
                try
                {
                    //Find the android control with the same name
                    var view = This.GetControl<View>(m.Name);
                    //Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property "
                                                     + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        public static void WireUpControls(this Activity This)
        {
            // Auto wire-up
            //Get all the View properties from the activity
            var members = from m in This.GetType().GetRuntimeProperties()
                          where m.PropertyType.IsSubclassOf(typeof(View))
                          select m;

            members.ToList().ForEach(m =>
            {
                try
                {
                    //Find the android control with the same name
                    var view = This.GetControl<View>(m.Name);
                    //Set the activity field's value to the view with that identifier
                    m.SetValue(This, view);
                }
                catch (Exception ex)
                {
                    throw new MissingFieldException("Failed to wire up the Property "
                                                     + m.Name + " to a View in your layout with a corresponding identifier", ex);
                }
            });
        }

        static View getCachedControl(string propertyName, object rootView, Func<View> fetchControlFromView)
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