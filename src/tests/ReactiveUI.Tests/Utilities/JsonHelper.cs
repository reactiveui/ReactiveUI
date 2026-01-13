// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ReactiveUI.Tests.Utilities;

public static class JSONHelper
{
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
