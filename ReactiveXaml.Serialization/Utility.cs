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
            var ret = new List<Type>();
            foreach (Assembly a in allAssemblies) {
                foreach (Module m in a.GetModules()) {
                    Type[] type_list = m.GetTypes();
                    foreach (Type t in type_list) {
                        if (!Utility.ImplementsInterface(t, InterfaceToCheck))
                            continue;

                        ret.Add(t);
                    }
                }
            }
            return ret;
        }
	}

    public static class HelperExtensions
    {
        public static byte[] MD5Hash(this string obj)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(obj));
        }
    }

    public static class JSONHelper
    {
        public static string Serialize<T>(T obj, IEnumerable<Type> knownTypes = null)
        {
            knownTypes = knownTypes ?? Enumerable.Empty<Type>();
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType(), knownTypes);
            var ms = new MemoryStream();

            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json, IEnumerable<Type> knownTypes = null)
        {
            knownTypes = knownTypes ?? Enumerable.Empty<Type>();

            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType(), knownTypes);

            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }

    public class AggregateDataContractSurrogate : IDataContractSurrogate
    {
        IEnumerable<IDataContractSurrogate> dataContracts;

        public AggregateDataContractSurrogate(IEnumerable<IDataContractSurrogate> dataContracts)
        {
            this.dataContracts = dataContracts;
        }

        public Type GetDataContractType(Type type)
        {
            return dataContracts.Select(x => x.GetDataContractType(type))
                .FirstOrDefault(x => x != type) ?? type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            return dataContracts.Select(x => x.GetDeserializedObject(obj, targetType))
                .FirstOrDefault(x => x != obj) ?? obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            return dataContracts.Select(x => x.GetObjectToSerialize(obj, targetType))
                .FirstOrDefault(x => x != obj) ?? obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return dataContracts.Select(x => x.GetReferencedTypeOnImport(typeName, typeNamespace, customData))
                .FirstOrDefault(x => x != null);
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType) { return null; }
        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType) { return null; }
        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes) { }
        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit) { return typeDeclaration; }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :
