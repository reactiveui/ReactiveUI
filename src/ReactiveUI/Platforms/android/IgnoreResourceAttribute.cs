﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// Attribute that marks a resource to be ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreResourceAttribute : Attribute
    {
    }
}
