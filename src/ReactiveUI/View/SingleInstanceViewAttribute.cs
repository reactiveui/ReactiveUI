// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Indicates that this View should be constructed _once_ and then used
/// every time its ViewModel View is resolved.
/// Obviously, this is not supported on Views that may be reused multiple
/// times in the Visual Tree.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingleInstanceViewAttribute : Attribute;