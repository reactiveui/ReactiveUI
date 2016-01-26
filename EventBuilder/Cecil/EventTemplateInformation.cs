using EventBuilder.Entities;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace EventBuilder.Cecil
{
    public static class EventTemplateInformation
    {
        private static readonly Dictionary<string, string> SubstitutionList = new Dictionary<string, string>
        {
            {"Windows.UI.Xaml.Data.PropertyChangedEventArgs", "global::System.ComponentModel.PropertyChangedEventArgs"},
            {
                "Windows.UI.Xaml.Data.PropertyChangedEventHandler",
                "global::System.ComponentModel.PropertyChangedEventHandler"
            },
            {"Windows.Foundation.EventHandler", "EventHandler"},
            {"Windows.Foundation.EventHandler`1", "EventHandler"},
            {"Windows.Foundation.EventHandler`2", "EventHandler"}
        };

        private static string RenameBogusWinRTTypes(string typeName)
        {
            if (SubstitutionList.ContainsKey(typeName)) return SubstitutionList[typeName];
            return typeName;
        }

        private static string GetEventArgsTypeForEvent(EventDefinition ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var type = ei.EventType.Resolve();
            var invoke = type.Methods.First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 2) return null;

            var param = invoke.Parameters[1];
            var ret = RenameBogusWinRTTypes(param.ParameterType.FullName);

            var generic = ei.EventType as GenericInstanceType;
            if (generic != null)
            {
                foreach (
                    var kvp in
                        type.GenericParameters.Zip(generic.GenericArguments, (name, actual) => new {name, actual}))
                {
                    var realType = GetRealTypeName(kvp.actual);

                    ret = ret.Replace(kvp.name.FullName, realType);
                }
            }

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static string GetRealTypeName(TypeDefinition t)
        {
            if (t.GenericParameters.Count == 0) return RenameBogusWinRTTypes(t.FullName);

            var ret = string.Format("{0}<{1}>",
                RenameBogusWinRTTypes(t.Namespace + "." + t.Name),
                string.Join(",", t.GenericParameters.Select(x => GetRealTypeName(x.Resolve()))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static string GetRealTypeName(TypeReference t)
        {
            var generic = t as GenericInstanceType;
            if (generic == null) return RenameBogusWinRTTypes(t.FullName);

            var ret = string.Format("{0}<{1}>",
                RenameBogusWinRTTypes(generic.Namespace + "." + generic.Name),
                string.Join(",", generic.GenericArguments.Select(x => GetRealTypeName(x))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static EventDefinition[] GetPublicEvents(TypeDefinition t)
        {
            return
                t.Events.Where(x => x.AddMethod.IsPublic && !x.AddMethod.IsStatic && GetEventArgsTypeForEvent(x) != null)
                    .ToArray();
        }

        public static NamespaceInfo[] Create(AssemblyDefinition[] targetAssemblies)
        {
            var publicTypesWithEvents = targetAssemblies
                .SelectMany(x => SafeTypes.GetSafeTypes(x))
                .Where(x => x.IsPublic && !x.HasGenericParameters)
                .Select(x => new {Type = x, Events = GetPublicEvents(x)})
                .Where(x => x.Events.Length > 0)
                .ToArray();

            var garbageNamespaceList = new[]
            {
                "Windows.UI.Xaml.Data",
                "Windows.UI.Xaml.Interop",
                "Windows.UI.Xaml.Input",
                "MonoTouch.AudioToolbox",
                "MonoMac.AudioToolbox",
                "ReactiveUI.Events"
            };

            var namespaceData = publicTypesWithEvents
                .GroupBy(x => x.Type.Namespace)
                .Where(x => !garbageNamespaceList.Contains(x.Key))
                .Select(x => new NamespaceInfo
                {
                    Name = x.Key,
                    Types = x.Select(y => new PublicTypeInfo
                    {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Events = y.Events.Select(z => new PublicEventInfo
                        {
                            Name = z.Name,
                            EventHandlerType = GetRealTypeName(z.EventType),
                            EventArgsType = GetEventArgsTypeForEvent(z)
                        }).ToArray()
                    }).ToArray()
                }).ToArray();

            foreach (var type in namespaceData.SelectMany(x => x.Types))
            {
                var parentWithEvents = GetParents(type.Type).FirstOrDefault(x => GetPublicEvents(x).Any());
                if (parentWithEvents == null) continue;

                type.Parent = new ParentInfo {Name = parentWithEvents.FullName};
            }

            return namespaceData;
        }

        private static IEnumerable<TypeDefinition> GetParents(TypeDefinition type)
        {
            var current = type.BaseType != null
                ? type.BaseType.Resolve()
                : null;

            while (current != null)
            {
                yield return current.Resolve();

                current = current.BaseType != null
                    ? current.BaseType.Resolve()
                    : null;
            }
        }
    }
}