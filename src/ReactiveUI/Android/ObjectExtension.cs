using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Object = Java.Lang.Object;

namespace ReactiveUI
{
    internal class JavaHolder : Object
    {
        public readonly object Instance;

        public JavaHolder(object instance)
        {
            Instance = instance;
        }
    }

    internal static class ObjectExtension
    {
        public static TObject ToNetObject<TObject>(this Object value)
        {
            if (value == null)
                return default(TObject);

            if (!(value is JavaHolder))
                throw new InvalidOperationException("Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");

            return (TObject)((JavaHolder)value).Instance;
        }

        public static Object ToJavaObject<TObject>(this TObject value)
        {
            if (value == null)
                return null;

            var holder = new JavaHolder(value);
            
            return (Object)holder;
        }
    }
}