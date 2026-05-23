// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ReactiveUI.Tests.Utilities;

/// <summary>
/// Helper methods for serializing and deserializing objects to and from JSON in tests.
/// </summary>
[SuppressMessage("Major Code Smell", "S4018:Generic methods should provide type parameters", Justification = "Type parameter cannot be inferred.")]
public static class JsonHelper
{
    /// <summary>
    /// Deserializes the supplied JSON string into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The reference type to deserialize into.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized instance, or <see langword="null"/> if deserialization fails.</returns>
    public static T? Deserialize<T>(string json)
        where T : class
    {
        var obj = Activator.CreateInstance<T>();

        var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
        var serializer = new DataContractJsonSerializer(obj.GetType());
        obj = serializer.ReadObject(ms) as T;
        ms.Close();
        return obj;
    }

    /// <summary>
    /// Serializes the supplied object into its JSON representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="serializeObject">The object to serialize.</param>
    /// <returns>The JSON string, or <see langword="null"/> if <paramref name="serializeObject"/> is <see langword="null"/>.</returns>
    public static string? Serialize<T>(T serializeObject)
    {
        if (serializeObject is null)
        {
            return null;
        }

        var serializer = new DataContractJsonSerializer(serializeObject.GetType());
        var ms = new MemoryStream();
        serializer.WriteObject(ms, serializeObject);
        return Encoding.Default.GetString(ms.ToArray());
    }
}
