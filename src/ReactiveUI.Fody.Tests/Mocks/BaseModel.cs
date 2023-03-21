// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Fody.Tests;

/// <summary>
/// A base model for the mocks.
/// </summary>
public class BaseModel : ReactiveObject
{
    /// <summary>
    /// Gets or sets a integer property with a initial value.
    /// </summary>
    public virtual int IntProperty { get; set; } = 5;

    /// <summary>
    /// Gets or sets a string property with a initial value.
    /// </summary>
    public virtual string? StringProperty { get; set; } = "Initial Value";
}