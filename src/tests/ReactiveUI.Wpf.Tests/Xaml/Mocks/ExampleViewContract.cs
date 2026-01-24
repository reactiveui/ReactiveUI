// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>
/// A view for <see cref="ExampleViewModel"/> with a contract.
/// </summary>
[ViewContract("contract")]
public class ExampleViewContract : ReactiveUserControl<ExampleViewModel>;
