// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EventBuilder.Entities;
using Mono.Cecil;
using Serilog;

namespace EventBuilder.Cecil
{
    /// <summary>
    /// Event template information about events and handlers.
    /// </summary>
    public static class EventTemplateInformation
    {
        private static readonly Dictionary<string, string> SubstitutionList = new Dictionary<string, string>
        {
            { "Windows.UI.Xaml.Data.PropertyChangedEventArgs", "global::System.ComponentModel.PropertyChangedEventArgs" },
            { "Windows.UI.Xaml.Data.PropertyChangedEventHandler", "global::System.ComponentModel.PropertyChangedEventHandler" },
            { "Windows.Foundation.EventHandler", "EventHandler" },
            { "Windows.Foundation.EventHandler`1", "EventHandler" },
            { "Windows.Foundation.EventHandler`2", "EventHandler" },
            { "System.Boolean", "Boolean" },
            { "System.Boolean`1", "Boolean" },
            { "System.EventHandler", "EventHandler" },
            { "System.EventHandler`1", "EventHandler" },
            { "System.EventHandler`2", "EventHandler" },
            { "System.EventArgs", "EventArgs" },
            { "System.EventArgs`1", "EventArgs" },
            { "System.EventArgs`2", "EventArgs" },
            { "Tizen.NUI.EventHandlerWithReturnType", "Tizen.NUI.EventHandlerWithReturnType" },
            { "Tizen.NUI.EventHandlerWithReturnType`1", "Tizen.NUI.EventHandlerWithReturnType" },
            { "Tizen.NUI.EventHandlerWithReturnType`2", "Tizen.NUI.EventHandlerWithReturnType" },
            { "Tizen.NUI.EventHandlerWithReturnType`3", "Tizen.NUI.EventHandlerWithReturnType" },
        };

        /// <summary>
        /// Creates namespace information from the specified target assemblies.
        /// </summary>
        /// <param name="targetAssemblies">The target assemblies.</param>
        /// <returns>The namespaces in the target assemblies.</returns>
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
                "Windows.UI.Xaml.Data",
                "Windows.UI.Xaml.Interop",
                "Windows.UI.Xaml.Input",
                "MonoTouch.AudioToolbox",
                "MonoMac.AudioToolbox",
                "ReactiveUI.Events",

                // Winforms
                "System.Collections.Specialized",
                "System.Configuration",
                "System.ComponentModel.Design",
                "System.ComponentModel.Design.Serialization",
                "System.CodeDom",
                "System.Data.SqlClient",
                "System.Data.OleDb",
                "System.Data.Odbc",
                "System.Data.Common",
                "System.Drawing.Design",
                "System.Media",
                "System.Net",
                "System.Net.Mail",
                "System.Net.NetworkInformation",
                "System.Net.Sockets",
                "System.ServiceProcess.Design",
                "System.Windows.Input",
                "System.Windows.Forms.ComponentModel.Com2Interop",
                "System.Windows.Forms.Design",
                "System.Timers"
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
                            EventArgsType = GetEventArgsTypeForEvent(z),
                            ObsoleteEventInfo = GetObsoleteInfo(z)
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

        private static string RenameBogusTypes(string typeName)
        {
            if (SubstitutionList.ContainsKey(typeName))
            {
                return SubstitutionList[typeName];
            }

            return typeName;
        }

        [SuppressMessage("Globalization", "CA1307: Specify StringComparison", Justification = "Replace overload is for .NET Standard only")]
        private static string GetEventArgsTypeForEvent(EventDefinition ei)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            var type = ei.EventType.Resolve();

            if (type == null)
            {
                Log.Debug($"Type for {ei.EventType} is not valid");
                return null;
            }

            var invoke = type.Methods.First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 2)
            {
                return null;
            }

            var param = invoke.Parameters[1];
            var ret = RenameBogusTypes(param.ParameterType.FullName);

            var generic = ei.EventType as GenericInstanceType;
            if (generic != null)
            {
                foreach (
                    var kvp in
                        type.GenericParameters.Zip(generic.GenericArguments, (name, actual) => new { name, actual }))
                        {
                    var realType = GetRealTypeName(kvp.actual);

                    var temp = ret.Replace(kvp.name.FullName, realType);
                    if (temp != ret)
                    {
                        ret = temp;
                        break;
                    }
                }
            }

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static string GetRealTypeName(TypeDefinition t)
        {
            if (t.GenericParameters.Count == 0)
            {
                return RenameBogusTypes(t.FullName);
            }

            var ret = string.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                RenameBogusTypes(t.Namespace + "." + t.Name),
                string.Join(",", t.GenericParameters.Select(x => GetRealTypeName(x.Resolve()))));

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static string GetRealTypeName(TypeReference t)
        {
            var generic = t as GenericInstanceType;
            if (generic == null)
            {
                return RenameBogusTypes(t.FullName);
            }

            var ret = string.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                RenameBogusTypes(generic.Namespace + "." + generic.Name),
                string.Join(",", generic.GenericArguments.Select(GetRealTypeName)));

            // NB: Handy place to hook to troubleshoot if something needs to be added to SubstitutionList
            // if (generic.FullName.Contains("MarkReachedEventArgs")) {
            //    // Tizen.NUI.EventHandlerWithReturnType`3
            //    //<System.Object,Tizen.NUI.UIComponents.Slider/
            //    //MarkReachedEventArgs,
            //    //System.Boolean>
            // }

            // NB: Inner types in Mono.Cecil get reported as 'Foo/Bar'
            return ret.Replace('/', '.');
        }

        private static ObsoleteEventInfo GetObsoleteInfo(EventDefinition ei)
        {
            var obsoleteAttribute = ei.CustomAttributes
                .FirstOrDefault(attr => attr.AttributeType.FullName.Equals("System.ObsoleteAttribute", StringComparison.InvariantCulture));

            if (obsoleteAttribute == null)
            {
                return null;
            }

            return new ObsoleteEventInfo
            {
                Message = obsoleteAttribute.ConstructorArguments?.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty,
                IsError = bool.Parse(obsoleteAttribute.ConstructorArguments?.ElementAtOrDefault(1).Value?.ToString() ?? bool.FalseString)
            };
        }

        private static EventDefinition[] GetPublicEvents(TypeDefinition t)
        {
            return
                t.Events.Where(x => x.AddMethod.IsPublic && !x.AddMethod.IsStatic && GetEventArgsTypeForEvent(x) != null)
                    .ToArray();
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
