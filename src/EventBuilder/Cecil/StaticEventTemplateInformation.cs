using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EventBuilder.Entities;
using Mono.Cecil;

namespace EventBuilder.Cecil
{
    /// <summary>
    /// Static event template methods.
    /// </summary>
    public static class StaticEventTemplateInformation
    {
        /// <summary>
        /// Creates the specified target assemblies.
        /// </summary>
        /// <param name="targetAssemblies">The target assemblies.</param>
        /// <returns>The namespace information for the assemblies.</returns>
        public static NamespaceInfo[] Create(AssemblyDefinition[] targetAssemblies)
        {
            var publicTypesWithEvents = targetAssemblies
                .SelectMany(SafeTypes.GetSafeTypes)
                .Where(x => x.IsPublic && !x.HasGenericParameters)
                .Select(x => new { Type = x, Events = GetPublicEvents(x) })
                .Where(x => x.Events.Length > 0)
                .ToArray();

            var garbageNamespaceList = new[]
            {
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
                if (parentWithEvents == null)
                {
                    continue;
                }

                type.Parent = new ParentInfo { Name = parentWithEvents.FullName };
            }

            return namespaceData;
        }

        private static IEnumerable<TypeDefinition> GetParents(TypeDefinition type)
        {
            var current = type.BaseType != null && type.BaseType.ToString() != "System.Object"
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

        [SuppressMessage("Globalization", "CA1307: Specify StringComparison", Justification = "Replace overload is for .NET Standard only")]
        private static string GetEventArgsTypeForEvent(EventDefinition ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var type = ei.EventType.Resolve();
            var invoke = type.Methods.First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 1)
            {
                return null;
            }

            var param = invoke.Parameters.Count == 1 ? invoke.Parameters[0] : invoke.Parameters[1];
            var ret = param.ParameterType.FullName;

            var generic = ei.EventType as GenericInstanceType;
            if (generic != null)
            {
                foreach (
                    var kvp in
                        type.GenericParameters.Zip(generic.GenericArguments, (name, actual) => new { name, actual }))
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
            if (t.GenericParameters.Count == 0)
            {
                return t.FullName;
            }

            var ret = string.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                t.Namespace + "." + t.Name,
                string.Join(",", t.GenericParameters.Select(x => GetRealTypeName(x.Resolve()))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static string GetRealTypeName(TypeReference t)
        {
            var generic = t as GenericInstanceType;
            if (generic == null)
            {
                return t.FullName;
            }

            var ret = string.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                generic.Namespace + "." + generic.Name,
                string.Join(",", generic.GenericArguments.Select(GetRealTypeName)));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static EventDefinition[] GetPublicEvents(TypeDefinition t)
        {
            return
                t.Events

                    .Where(x => x.AddMethod.IsPublic && GetEventArgsTypeForEvent(x) != null)
                    .ToArray();
        }
    }
}
