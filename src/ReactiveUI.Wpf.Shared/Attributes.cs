// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Markup;

#if REACTIVE_SHIM
[assembly: XmlnsDefinition("http://reactiveui.net", "ReactiveUI.Reactive")]
#else
[assembly: XmlnsDefinition("http://reactiveui.net", "ReactiveUI")]
#endif
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]
