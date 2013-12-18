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
            var targetAssemblyDirs = targetAssemblyNames.Select(x => Path.GetDirectoryName(x)).Distinct().ToList();

            // NB: I'm too lazy to fix this properly
            var monoDroidDir = targetAssemblyDirs.FirstOrDefault(x => x.ToLowerInvariant().Contains("monoandroid"));
            if (monoDroidDir != null) {
                targetAssemblyDirs.Add(Path.Combine(monoDroidDir, "..", "..", "..", "mono", "2.1"));
            }

            // NB: Double down on Laziness
            var xamMacDir = targetAssemblyDirs.FirstOrDefault(x => x.ToLowerInvariant().Contains("xamarin.mac"));
            if (xamMacDir != null) {
                targetAssemblyDirs.Add("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5");
            }

            var rp = new ReaderParameters() { AssemblyResolver = new PathSearchAssemblyResolver(targetAssemblyDirs.ToArray()) };
            var targetAssemblies = targetAssemblyNames
                .Select(x => AssemblyDefinition.ReadAssembly(x, rp)).ToArray();

            var template = File.ReadAllText(args.Last(), Encoding.UTF8);

            var namespaceData = CreateEventTemplateInformation(targetAssemblies);
            var delegateData = CreateDelegateTemplateInformation(targetAssemblies);

            var result = Render.StringToString(template, new { Namespaces = namespaceData, DelegateNamespaces = delegateData })
                .Replace("System.String", "string")
                .Replace("System.Object", "object")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("`1", "")
                .Replace("`2", "");

            Console.WriteLine(result);
        }

        public static NamespaceInfo[] CreateEventTemplateInformation(AssemblyDefinition[] targetAssemblies)
        {
            var publicTypesWithEvents = targetAssemblies
                .SelectMany(x => SafeGetTypes(x))
                .Where(x => x.IsPublic && !x.HasGenericParameters)
                .Select(x => new { Type = x, Events = GetPublicEvents(x) })
                .Where(x => x.Events.Length > 0)
                .ToArray();

            var garbageNamespaceList = new[] {
                "Windows.UI.Xaml.Data",
                "Windows.UI.Xaml.Interop",
                "Windows.UI.Xaml.Input",
                "MonoTouch.AudioToolbox",
                "MonoMac.AudioToolbox",
                "ReactiveUI.Events",
            };

            var namespaceData = publicTypesWithEvents
                .GroupBy(x => x.Type.Namespace)
                .Where(x => !garbageNamespaceList.Contains(x.Key))
                .Select(x => new NamespaceInfo() {
                    Name = x.Key,
                    Types = x.Select(y => new PublicTypeInfo() {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Events = y.Events.Select(z => new PublicEventInfo() {
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

            return namespaceData;
        }

        public static NamespaceInfo[] CreateDelegateTemplateInformation(AssemblyDefinition[] targetAssemblies)
        {
            var garbageTypeList = new[] {
                "AVPlayerItemLegibleOutputPushDelegate",  // NB: Breaks build on device because reasons.
            };

            var publicDelegateTypes = targetAssemblies
                .SelectMany(x => SafeGetTypes(x))
                .Where(x => x.IsPublic && !x.IsInterface && !x.HasGenericParameters && isCocoaDelegateName(x.Name))
                .Where(x => x.BaseType == null || !x.BaseType.FullName.Contains("MulticastDelegate"))
		.Where(x => !garbageTypeList.Any(y => x.FullName.Contains(y)))
                .Select(x => new { Type = x, Delegates = GetPublicDelegateMethods(x) })
                .Where(x => x.Delegates.Length > 0)
                .ToArray();

            var namespaceData = publicDelegateTypes
                .GroupBy(x => x.Type.Namespace)
                //.Where(x => !garbageNamespaceList.Contains(x.Key))
                .Select(x => new NamespaceInfo() {
                    Name = x.Key,
                    Types = x.Select(y => new PublicTypeInfo() {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Abstract = y.Type.IsAbstract ? "abstract" : "",
                        ZeroParameterMethods = y.Delegates.Where(z => z.Parameters.Count == 0).Select(z => new ParentInfo() {
                            Name = z.Name,
                        }).ToArray(),
                        SingleParameterMethods = y.Delegates.Where(z => z.Parameters.Count == 1).Select(z => new SingleParameterMethod() {
                            Name = z.Name,
                            ParameterType = z.Parameters[0].ParameterType.FullName,
                            ParameterName = z.Parameters[0].Name,
                        }).ToArray(),
                        MultiParameterMethods = y.Delegates.Where(z => z.Parameters.Count > 1).Select(z => new MultiParameterMethod() {
                            Name = z.Name,
                            ParameterList = String.Join(", ", z.Parameters.Select(a => String.Format("{0} {1}", a.ParameterType.FullName, a.Name))),
                            ParameterTypeList = String.Join(", ", z.Parameters.Select(a => a.ParameterType.FullName)),
                            ParameterNameList = String.Join(", ", z.Parameters.Select(a => a.Name)),
                        }).ToArray(),
                    }).ToArray()
                }).ToArray();

            return namespaceData;
        }

        static bool isCocoaDelegateName(string name)
        {
            if (name.EndsWith("Delegate", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("UITableViewSource", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static EventDefinition[] GetPublicEvents(TypeDefinition t)
        {
            return t.Events.Where(x => x.AddMethod.IsPublic && !x.AddMethod.IsStatic && GetEventArgsTypeForEvent(x) != null).ToArray();
        }

        public static MethodDefinition[] GetPublicDelegateMethods(TypeDefinition t)
        {
            var bannedMethods = new[] { "Dispose", "Finalize" };
            return t.Methods
                .Where(x => x.IsVirtual && !x.IsConstructor && !x.IsSetter && x.ReturnType.FullName == "System.Void")
                .Where(x => x.Parameters.All(y => y.ParameterType.FullName.Contains("&") == false))
                .Where(x => !bannedMethods.Contains(x.Name))
                .GroupBy(x => x.Name).Select(x => x.OrderByDescending(y => y.Parameters.Count).First())
                .ToArray();
        }

        public static TypeDefinition[] SafeGetTypes(AssemblyDefinition a)
        {
            return a.Modules.SelectMany(x => x.GetTypes()).ToArray();
        }

        public static string GetRealTypeName(TypeDefinition t)
        {
            if (t.GenericParameters.Count == 0) return RenameBogusWinRTTypes(t.FullName);

            var ret = String.Format("{0}<{1}>",
                RenameBogusWinRTTypes(t.Namespace + "." + t.Name),
                String.Join(",", t.GenericParameters.Select(x => GetRealTypeName(x.Resolve()))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        public static string GetRealTypeName(TypeReference t)
        {
            var generic = t as GenericInstanceType;
            if (generic == null) return RenameBogusWinRTTypes(t.FullName);

            var ret = String.Format("{0}<{1}>",
                RenameBogusWinRTTypes(generic.Namespace + "." + generic.Name),
                String.Join(",", generic.GenericArguments.Select(x => GetRealTypeName(x))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        static Dictionary<string, string> substitutionList = new Dictionary<string, string> {
            { "Windows.UI.Xaml.Data.PropertyChangedEventArgs", "global::System.ComponentModel.PropertyChangedEventArgs" },
            { "Windows.UI.Xaml.Data.PropertyChangedEventHandler", "global::System.ComponentModel.PropertyChangedEventHandler" },
            { "Windows.Foundation.EventHandler", "EventHandler" },
            { "Windows.Foundation.EventHandler`1", "EventHandler" },
            { "Windows.Foundation.EventHandler`2", "EventHandler" },
        };

        public static string RenameBogusWinRTTypes(string typeName)
        {
            if (substitutionList.ContainsKey(typeName)) return substitutionList[typeName];
            return typeName;
        }

        public static string GetEventArgsTypeForEvent(EventDefinition ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var type = ei.EventType.Resolve();
            var invoke = type.Methods.First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 2) return null;

            var param = invoke.Parameters[1];
            var ret = RenameBogusWinRTTypes(param.ParameterType.FullName);

            var generic = ei.EventType as GenericInstanceType;
            if (generic != null) {
                foreach(var kvp in type.GenericParameters.Zip(generic.GenericArguments, (name, actual) => new { name, actual })) {
                    var realType = GetRealTypeName(kvp.actual);

                    ret = ret.Replace(kvp.name.FullName, realType);
                }
            }

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
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
        public string Abstract { get; set; } 
        public TypeDefinition Type { get; set; }
        public ParentInfo Parent { get; set; }
        public IEnumerable<PublicEventInfo> Events { get; set; }
        public IEnumerable<ParentInfo> ZeroParameterMethods { get; set; }
        public IEnumerable<SingleParameterMethod> SingleParameterMethods { get; set; }
        public IEnumerable<MultiParameterMethod> MultiParameterMethods { get; set; }
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

    class SingleParameterMethod
    {
        public string Name { get; set; }
        public string ParameterType { get; set; }
        public string ParameterName { get; set; }
    }

    class MultiParameterMethod
    {
        public string Name { get; set; }
        public string ParameterList { get; set; }  // "FooType foo, BarType bar, BazType baz"
        public string ParameterTypeList { get; set; }  // "FooType, BarType, BazType"
        public string ParameterNameList { get; set; }  // "foo, bar, baz"
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
            var dllName = fullName.Split(',')[0] + ".dll";

            var fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            if (fullPath == null)
            {
                dllName = fullName.Split(',')[0] + ".winmd";
                fullPath = targetAssemblyDirs.Select(x => Path.Combine(x, dllName)).FirstOrDefault(x => File.Exists(x));
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            if (fullName.Contains("mscorlib") && fullName.Contains("255"))
            {
                fullPath = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null)
            {
                var err = String.Format("Failed to resolve!!! {0}", fullName);
                Console.Error.WriteLine(err);
                throw new Exception(err);
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

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            if (fullName.Contains("mscorlib") && fullName.Contains("255"))
            {
                fullPath = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null)
            {
                var err = String.Format("Failed to resolve!!! {0}", fullName);
                Console.Error.WriteLine(err);
                throw new Exception(err);
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
