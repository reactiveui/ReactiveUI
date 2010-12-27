using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.CodeDom;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ReactiveXaml.Serialization
{
	public static class Utility
	{
	    public static bool ImplementsInterface(Type Target, Type InterfaceToCheck)
        {
            if (Target == null)
                throw new ArgumentNullException("Target");

            if (Target.GetInterfaces().Contains(InterfaceToCheck)) {
                return true;
            }

            if (Target.BaseType != typeof(object)) {
                return ImplementsInterface(Target.BaseType, InterfaceToCheck);
            }

            return false;
        }

	    static Dictionary<string, Type> typeCache = new Dictionary<string,Type>();
        public static Type GetTypeByName(string fullName, IEnumerable<Assembly> targetAssemblies = null)
        {
#if SILVERLIGHT
            // XXX: This is almost certainly going to go badly for us
            return Type.GetType(fullName, true, true);
#else
            lock(typeCache) {
                if (typeCache.Count == 0) {
                    var allTypes = from a in targetAssemblies ?? AppDomain.CurrentDomain.GetAssemblies()
                        from mod in a.GetModules()
                        from type in mod.SafeGetTypes()
                        select type;
                    foreach(var v in allTypes) {
                        typeCache[v.FullName] = v;
                    }
                }

                Type ret;
                if (!typeCache.TryGetValue(fullName, out ret)) {
                    // Try Type.GetType() as a last-ditch
                    ret = Type.GetType(fullName);
                    typeCache[fullName] = ret;
                }

                if (targetAssemblies != null && !targetAssemblies.Contains(ret.Assembly))
                    return null;
                return ret;
            }
#endif
        }
	}

    public static class ModuleHelper
    {
        public static Type[] SafeGetTypes(this Module This)
        {
            try {
                return This.GetTypes();
            } catch(ReflectionTypeLoadException _) {
                return new Type[0];
            }
        }
    }

    public static class HelperExtensions
    {
        public static byte[] MD5Hash(this string obj)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(obj));
        }

        static ThreadLocal<JsonSerializer> _serializer = new ThreadLocal<JsonSerializer>(() => new JsonSerializer());
        public static byte[] ObjectContentsHash(this object This)
        {
            var ms = new MemoryStream();
            using (var writer = new BsonWriter(ms)) {
                _serializer.Value.Serialize(writer, This);
            }
            var md5 = MD5.Create();
            return md5.ComputeHash(ms.ToArray());
        }

        public static Dictionary<TKey, TVal> ToConcreteDictionary<TKey, TVal>(this IDictionary<TKey, TVal> This)
        {
            return This.Keys.ToDictionary(k => k, k => This[k]);
        }

        public static TVal GetOrAdd<TKey, TVal>(this IDictionary<TKey, TVal> This, TKey key)
        {
            TVal ret;
            if (This.TryGetValue(key, out ret)) {
                return ret;
            }

            ret = Activator.CreateInstance<TVal>();
            This.Add(key, ret);
            return ret;
        }

        public static byte[] GetAllBytes(this Stream This)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream()) {
                int read;
                while ((read = This.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
