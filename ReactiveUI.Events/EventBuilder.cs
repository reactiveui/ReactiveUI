using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using System.Text;
using System.Threading.Tasks;

namespace EventBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetAssemblyNames = args.TakeWhile(x => !x.EndsWith(".mustache"));
            var targetAssemblyDirs = targetAssemblyNames.Select(x => Path.GetDirectoryName(x)).Distinct().ToArray();

            /*
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (o, e) => {

                return Assembly.ReflectionOnlyLoadFrom(fullPath);
            };
            */

            var rp = new ReaderParameters() { AssemblyResolver = new PathSearchAssemblyResolver(targetAssemblyDirs) };
            var targetAssemblies = targetAssemblyNames
                .Select(x => AssemblyDefinition.ReadAssembly(x, rp)).ToArray();

            var template = File.ReadAllText(args.Last(), Encoding.UTF8);

            var publicTypesWithEvents = targetAssemblies
                .SelectMany(x => SafeGetTypes(x))
                .Where(x => x.IsPublic && !x.HasGenericParameters)
                .Select(x => new { Type = x, Events = GetPublicEvents(x) })
                .Where(x => x.Events.Length > 0)
                .ToArray();

            var namespaceData = publicTypesWithEvents
                .GroupBy(x => x.Type.Namespace)
                .Select(x => new NamespaceInfo() { 
                    Name = x.Key, 
                    Types = x.Select(y => new PublicTypeInfo() {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Events = y.Events
                            .Select(z => new PublicEventInfo() { 
                                Name = z.Name,
                                EventHandlerType = GetRealTypeName(z.EventType),
                                EventArgsType = GetEventArgsTypeForEvent(z),
                            }).ToArray(),
                    }).ToArray()
                }).ToArray();

            foreach (var type in namespaceData.SelectMany(x => x.Types)) {
                var parentWithEvents = GetParents(type.Type).FirstOrDefault(x => GetPublicEvents(x).Any());
                if (parentWithEvents == null) continue;

                type.Parent = new ParentInfo() { Name = parentWithEvents.FullName };
            }

            var result = Render.StringToString(template, new { Namespaces = namespaceData })
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("`1", "")
                .Replace("`2", "");

            Console.WriteLine(result);
        }

        public static EventDefinition[] GetPublicEvents(TypeDefinition t)
        {
            return t.Events.Where(x => x.AddMethod.IsPublic && !x.AddMethod.IsStatic && GetEventArgsTypeForEvent(x) != null).ToArray();
        }

        public static TypeDefinition[] SafeGetTypes(AssemblyDefinition a)
        {
            return a.Modules.SelectMany(x => x.GetTypes()).ToArray();
        }

        public static string GetRealTypeName(TypeDefinition t)
        {
            if (t.GenericParameters.Count == 0) return t.FullName;

            return String.Format("{0}.{1}<{2}>",
                t.Namespace, t.Name,
                String.Join(",", t.GenericParameters.Select(x => GetRealTypeName(x.Resolve()))));
        }

        public static string GetRealTypeName(TypeReference t)
        {
            var generic = t as GenericInstanceType;
            if (generic == null) return t.FullName;

            var ret = String.Format("{0}.{1}<{2}>",
                generic.Namespace, generic.Name,
                String.Join(",", generic.GenericArguments.Select(x => GetRealTypeName(x))));
            return "global::" + ret;
        }

        public static string GetEventArgsTypeForEvent(EventDefinition ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var type = ei.EventType.Resolve();
            var invoke = type.Methods.First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 2) return null;

            var param = invoke.Parameters[1];
            var ret = param.ParameterType.FullName;

            var generic = ei.EventType as GenericInstanceType;
            if (generic != null) {
                foreach(var kvp in type.GenericParameters.Zip(generic.GenericArguments, (name, actual) => new { name, actual })) {
                    ret = ret.Replace(kvp.name.FullName, GetRealTypeName(kvp.actual));
                }
            }

            return ret;
        }

        public static IEnumerable<TypeDefinition> GetParents(TypeDefinition type)
        {
            var current = type.BaseType != null ?
                type.BaseType.Resolve() :
                null;

            while (current != null) {
                yield return current.Resolve();

                current = current.BaseType != null ?
                    current.BaseType.Resolve() :
                    null;
            }
        }
    }

    class NamespaceInfo
    {
        public string Name { get; set; }
        public IEnumerable<PublicTypeInfo> Types { get; set; }
    }

    class PublicTypeInfo
    {
        public string Name { get; set; }
        public TypeDefinition Type { get; set; }
        public ParentInfo Parent { get; set; }
        public IEnumerable<PublicEventInfo> Events { get; set; }
    }

    class ParentInfo
    {
        public string Name { get; set; }
    }

    class PublicEventInfo
    {
        public string Name { get; set; }
        public string EventHandlerType { get; set; }
        public string EventArgsType { get; set; }
    }

    class PathSearchAssemblyResolver : IAssemblyResolver
    {
        string[] targetAssemblyDirs;
        
        public PathSearchAssemblyResolver(string[] targetAssemblyDirs)
        {
            this.targetAssemblyDirs = targetAssemblyDirs;
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            return Resolve(fullName);
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            if (fullPath == null)
            {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            }

            if (fullPath == null)
            {
                return null;
            }

            return AssemblyDefinition.ReadAssembly(fullPath, parameters);
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            if (fullPath == null)
            {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            }

            if (fullPath == null)
            {
                return null;
            }

            return AssemblyDefinition.ReadAssembly(fullPath);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name.FullName, parameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name.FullName);
        }
    }
}
