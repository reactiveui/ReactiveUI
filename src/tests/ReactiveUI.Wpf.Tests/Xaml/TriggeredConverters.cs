// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Xaml;

/// <summary>The converters a triggered binding applies in each direction, and the hint handed to them.</summary>
/// <param name="Hint">The conversion hint.</param>
/// <param name="ToView">The converter for view model values, or null for the registered converters.</param>
/// <param name="ToViewModel">The converter for view values, or null for the registered converters.</param>
internal readonly record struct TriggeredConverters(object? Hint, IBindingTypeConverter? ToView, IBindingTypeConverter? ToViewModel);
