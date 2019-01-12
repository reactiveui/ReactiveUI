﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public static class JSONHelper
    {
        public static string Serialize<T>(T serializeObject)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(serializeObject.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, serializeObject);
            return Encoding.Default.GetString(ms.ToArray());
        }

        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
