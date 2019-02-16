// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using EventBuilder.Core.Entities;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using Serilog;

namespace EventBuilder.Core.Reflection.Resolvers
{
    /// <summary>
    /// A namespace resolver that extracts event information.
    /// </summary>
    internal abstract class EventNamespaceResolverBase : INamespaceResolver
    {
        /// <inheritdoc />
        public abstract string TemplatePath { get; }

        /// <inheritdoc />
        public IEnumerable<NamespaceInfo> Create(ICompilation compilation)
        {
            return GetPublicTypesWithEvents(compilation)
                .GroupBy(x => x.Namespace)
                .Where(x => !ReflectionHelpers.SkipNamespaceList.Contains(x.Key))
                .Select(x => new NamespaceInfo
                {
                    Name = x.Key,
                    Types = x.Select(typeDetails => new PublicTypeInfo
                    {
                        Name = typeDetails.Name,
                        FullName = typeDetails.FullName ?? typeDetails.Name,
                        Events = GenerateEventInfo(compilation, typeDetails.Events).ToArray(),
                    }).ToArray()
                }).ToArray();
        }

        protected abstract IEnumerable<ITypeDefinition> GetPublicTypesWithEvents(ICompilation compilation);

        protected abstract IEnumerable<IEvent> GetValidEventDetails(IEnumerable<IEvent> eventDetails);

        private static string GetEventArgsTypeForEvent(ICompilation compilation, IEvent eventDetails)
        {
            // Find the EventArgs type parameter of the event via digging around via reflection
            if (!eventDetails.CanAdd || !eventDetails.CanRemove)
            {
                Log.Debug($"Type for {eventDetails.DeclaringType.FullName} is not valid");
                return null;
            }

            IType type = eventDetails.ReturnType;
            if (type is UnknownType)
            {
                type = compilation.GetReferenceTypeDefinitionsWithFullName(eventDetails.ReturnType.FullName).FirstOrDefault();
            }

            if (type == null)
            {
                Log.Debug($"Type for {eventDetails.DeclaringType.FullName} is not valid");
                return null;
            }

            if (type is ParameterizedType genericType)
            {
                return genericType.GenerateFullGenericName();
            }

            var invoke = type.GetMethods().First(x => x.Name == "Invoke");
            if (invoke.Parameters.Count < 2)
            {
                return null;
            }

            var param = invoke.Parameters[1];

            return param.Type.FullName;
        }

        private IEnumerable<PublicEventInfo> GenerateEventInfo(ICompilation compilation, IEnumerable<IEvent> events)
        {
            foreach (var eventDetails in GetValidEventDetails(events).OrderBy(x => x.Name))
            {
                var eventTypeArgs = GetEventArgsTypeForEvent(compilation, eventDetails);

                if (eventTypeArgs == null)
                {
                    continue;
                }

                yield return new PublicEventInfo
                {
                    Name = eventDetails.Name,
                    EventHandlerType = $"global::{eventDetails.ReturnType.GenerateFullGenericName()}",
                    ObsoleteEventInfo = eventDetails.GetObsoleteInfo(),
                    EventArgsType = $"global::{eventTypeArgs}",
                    DeclaringTypeName = eventDetails.DeclaringType.Name,
                    DeclaringTypeFullName = eventDetails.DeclaringType.FullName,
                };
            }
        }
    }
}
