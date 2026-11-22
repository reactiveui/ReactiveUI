// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif

/// <summary>Inspects <c>[DataContract]</c> and <c>[DataMember]</c> metadata for AutoPersist.</summary>
internal static class AutoPersistMetadataInspector
{
    /// <summary>Inspects a type's <c>[DataContract]</c> attribute and <c>[DataMember]</c> properties.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="hasDataContract">Whether the type carries <see cref="DataContractAttribute"/>.</param>
    /// <returns>The set of public instance property names annotated with <see cref="DataMemberAttribute"/>.</returns>
    internal static HashSet<string> Inspect(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        Type type,
        out bool hasDataContract)
    {
        hasDataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true).Length > 0;
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        HashSet<string>? set = null;
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            if (!property.IsDefined(typeof(DataMemberAttribute), true))
            {
                continue;
            }

            set ??= [with(StringComparer.Ordinal)];
            _ = set.Add(property.Name);
        }

        return set ?? [with(StringComparer.Ordinal)];
    }
}
