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
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ReactiveXaml.Serialization
{
	public static class Utility
	{
	    public static bool ImplementsInterface(Type Target, Type InterfaceToCheck)
        {
            if (Target == (Type)null)
                throw new ArgumentNullException("Target");

            var ret = Target.FindInterfaces(new TypeFilter((t, o) => t == InterfaceToCheck), null);

            // FIXME: This check might need to be more thorough
            return (ret != null && ret.GetLength(0) > 0 && !Target.IsAbstract);
        }

        public static IEnumerable<Type> GetAllTypesImplementingInterface(Type InterfaceToCheck, Assembly TargetAssembly = null)
        {
            var allAssemblies = (TargetAssembly != null ? new[] {TargetAssembly} : AppDomain.CurrentDomain.GetAssemblies());

            // I shudder to think what this actually gets turned into in MSIL
            var ret = from a in allAssemblies
                      from mod in a.GetModules()
                      from type in mod.SafeGetTypes()
                      where Utility.ImplementsInterface(type, InterfaceToCheck)
                      select type;

            return ret.ToArray();
        }

	    static Dictionary<string, Type> typeCache = new Dictionary<string,Type>();
        public static Type GetTypeByName(string fullName, IEnumerable<Assembly> targetAssemblies = null)
        {
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
                typeCache.TryGetValue(fullName, out ret);
                if (targetAssemblies != null && !targetAssemblies.Contains(ret.Assembly))
                    return null;
                return ret;
            }
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
    }
}

// vim: tw=120 ts=4 sw=4 et :
