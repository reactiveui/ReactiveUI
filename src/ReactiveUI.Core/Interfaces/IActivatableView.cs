// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Use this Interface when you want to mark a control as receiving View
/// Activation when it doesn't have a backing ViewModel.
/// </summary>
[SuppressMessage(
    "Design",
    "SST1437:Avoid empty interfaces",
    Justification = "Marker interface for activatable views.")]
public interface IActivatableView;
