// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Object = Java.Lang.Object;

namespace ReactiveUI;

internal class JavaHolder : Object
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401: Field should be private", Justification = "Used for interop purposes")]
    public readonly object Instance;

    public JavaHolder(object instance) => Instance = instance;
}
