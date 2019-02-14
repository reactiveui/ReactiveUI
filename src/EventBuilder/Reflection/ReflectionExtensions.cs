// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EventBuilder.Entities;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Reflection
{
    /// <summary>
    /// Extension methods associated with the System.Reflection.Metadata and ICSharpCode.Decompiler based classes.
    /// </summary>
    internal static class ReflectionExtensions
    {
        private static readonly ConcurrentDictionary<ICompilation, ImmutableDictionary<string, ImmutableList<ITypeDefinition>>> _typeNameMapping = new ConcurrentDictionary<ICompilation, ImmutableDictionary<string, ImmutableList<ITypeDefinition>>>();
        private static readonly ConcurrentDictionary<ICompilation, ImmutableList<ITypeDefinition>> _publicNonGenericTypeMapping = new ConcurrentDictionary<ICompilation, ImmutableList<ITypeDefinition>>();
        private static readonly ConcurrentDictionary<ICompilation, ImmutableList<ITypeDefinition>> _publicEventsTypeMapping = new ConcurrentDictionary<ICompilation, ImmutableList<ITypeDefinition>>();
        private static readonly ConcurrentDictionary<ICompilation, ImmutableHashSet<IType>> _compilationTypesHash = new ConcurrentDictionary<ICompilation, ImmutableHashSet<IType>>();

        /// <summary>
        /// Get all type definitions where they have public events, aren't generic (no type parameters == 0), and they are public.
        /// </summary>
        /// <param name="compilation">The compilation unit.</param>
        /// <returns>A enumerable of type definitions that match the criteria.</returns>
        public static IEnumerable<ITypeDefinition> GetPublicTypesWithNotStaticEvents(this ICompilation compilation)
        {
            var list = GetPublicTypeDefinitionsWithEvents(compilation);
            return list
                .Where(x => x.Events.Any(eventDetails => !eventDetails.IsStatic))
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name);
        }

        /// <summary>
        /// Get all type definitions where they have public events, aren't generic (no type parameters == 0), and they are public.
        /// </summary>
        /// <param name="compilation">The compilation unit.</param>
        /// <returns>A enumerable of type definitions that match the criteria.</returns>
        public static IEnumerable<ITypeDefinition> GetPublicTypesWithStaticEvents(this ICompilation compilation)
        {
            var list = GetPublicTypeDefinitionsWithEvents(compilation);
            return list
                .Where(x => x.Events.Any(eventDetails => eventDetails.IsStatic))
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name);
        }

        /// <summary>
        /// Gets type definitions matching the full name and in the reference and main libraries.
        /// </summary>
        /// <param name="compilation">The compilation to scan.</param>
        /// <param name="name">The name of the item to get.</param>
        /// <returns>The name of the items.</returns>
        public static IReadOnlyCollection<ITypeDefinition> GetReferenceTypeDefinitionsWithFullName(this ICompilation compilation, string name)
        {
            var map = _typeNameMapping.GetOrAdd(compilation, comp => comp.ReferencedModules.Concat(compilation.Modules).SelectMany(x => x.TypeDefinitions).GroupBy(x => x.FullName).ToImmutableDictionary(x => x.Key, x => x.ToImmutableList()));

            return map.GetValueOrDefault(name);
        }

        /// <summary>
        /// Gets information about the event's obsolete information if any.
        /// </summary>
        /// <param name="eventDetails">The event details.</param>
        /// <returns>The event's obsolete information if there is any.</returns>
        public static ObsoleteEventInfo GetObsoleteInfo(this IEvent eventDetails)
        {
            var obsoleteAttribute = eventDetails.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeType.FullName.Equals("System.ObsoleteAttribute", StringComparison.InvariantCulture));

            if (obsoleteAttribute == null)
            {
                return null;
            }

            return new ObsoleteEventInfo
            {
                Message = obsoleteAttribute.FixedArguments.FirstOrDefault().Value.ToString() ?? string.Empty,
                IsError = bool.Parse(obsoleteAttribute.FixedArguments.ElementAtOrDefault(1).Value?.ToString() ?? bool.FalseString)
            };
        }

        /// <summary>
        /// Determines if the type is in the main modules not the referenced modules.
        /// </summary>
        /// <param name="compilation">The compilation to scan.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>If the type definition is in the main set of modules.</returns>
        public static bool HasTypeInModules(this ICompilation compilation, IType typeDefinition)
        {
            var typesHash = _compilationTypesHash.GetOrAdd(compilation, comp => comp.GetAllTypeDefinitions().Select(x => x.DeclaringType).ToImmutableHashSet());

            return typesHash.Contains(typeDefinition);
        }

        public static IImmutableList<ITypeDefinition> GetPublicNonGenericTypeDefinitions(this ICompilation compilation)
        {
            return _publicNonGenericTypeMapping.GetOrAdd(
                    compilation,
                    comp => comp.GetAllTypeDefinitions().Where(x => x.Accessibility == Accessibility.Public && x.TypeParameterCount == 0).OrderBy(x => x.FullName)
                .ToImmutableList());
        }

        private static IImmutableList<ITypeDefinition> GetPublicTypeDefinitionsWithEvents(ICompilation compilation)
        {
            return _publicEventsTypeMapping.GetOrAdd(
                    compilation,
                    comp => comp.GetPublicNonGenericTypeDefinitions().Where(x => x.Events.Any(eventInfo => eventInfo.Accessibility == Accessibility.Public))
                .ToImmutableList());
        }
    }
}
