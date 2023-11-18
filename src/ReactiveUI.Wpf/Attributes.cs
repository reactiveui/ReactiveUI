// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Markup;

#pragma warning disable SA1114, SA1115

[assembly: XmlnsDefinition("http://reactiveui.net", "ReactiveUI")]
[assembly: ThemeInfo(

   // where theme specific resource dictionaries are located
   // (used if a resource is not found in the page,
   // or application resource dictionaries)
   ResourceDictionaryLocation.None,

   // where the generic resource dictionary is located
   // (used if a resource is not found in the page,
   // app, or any theme specific resource dictionaries)
   ResourceDictionaryLocation.SourceAssembly)
]
