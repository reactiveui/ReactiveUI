using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetAssemblies = args.TakeWhile(x => !x.EndsWith(".mustache"))
                .Select(x => Assembly.ReflectionOnlyLoadFrom(x)).ToArray();

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (o, e) => {
                return Assembly.ReflectionOnlyLoad(e.Name);
            };

            var template = File.ReadAllText(args.Last(), Encoding.UTF8);

            var eventsToFind = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var publicTypesWithEvents = targetAssemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsPublic && !x.IsGenericTypeDefinition)
                .Select(x => new { Type = x, Events = x.GetEvents(eventsToFind) })
                .Where(x => x.Events.Length > 0)
                .ToArray();

            var namespaceData = publicTypesWithEvents
                .GroupBy(x => x.Type.Namespace)
                .Select(x => new NamespaceInfo() { 
                    Name = x.Key, 
                    Types = x.Select(y => new PublicTypeInfo() {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Events = y.Events.Select(z => new PublicEventInfo() { 
                            Name = z.Name,
                            EventHandlerType = GetRealTypeName(z.EventHandlerType),
                            EventArgsType = GetRealTypeName(GetEventArgsTypeForEvent(z)),
                        }).ToArray(),
                    }).ToArray()
                }).ToArray();

            foreach (var type in namespaceData.SelectMany(x => x.Types)) {
                var parentWithEvents = GetParents(type.Type).FirstOrDefault(x => x.GetEvents(eventsToFind).Any());
                if (parentWithEvents == null) continue;

                type.Parent = new ParentInfo() { Name = parentWithEvents.FullName };
            }

            var result = Render.StringToString(template, new { Namespaces = namespaceData })
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("`1", "");

            Console.WriteLine(result);
        }

        public static string GetRealTypeName(Type t)
        {
            if (!t.IsGenericType) return t.FullName;

            return String.Format("{0}<{1}>",
                t.GetGenericTypeDefinition().FullName,
                String.Join(",", t.GetGenericArguments().Select(x => GetRealTypeName(x))));
        }

        public static Type GetEventArgsTypeForEvent(EventInfo ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var eventArgsType = ei.EventHandlerType.GetMethods().First(x => x.Name == "Invoke").GetParameters()[1].ParameterType;
            return eventArgsType;
        }

        public static IEnumerable<Type> GetParents(Type type)
        {
            var current = type.BaseType;
            while (current != null) {
                yield return current;
                current = current.BaseType;
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
        public Type Type { get; set; }
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
}
