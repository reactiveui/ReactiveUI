// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>
///     A single instance example view model.
/// </summary>
[SuppressMessage(
    "Minor Code Smell",
    "S2094:Classes should not be empty",
    Justification = "Empty type used as a test marker.")]
public class SingleInstanceExampleViewModel : ReactiveObject;
