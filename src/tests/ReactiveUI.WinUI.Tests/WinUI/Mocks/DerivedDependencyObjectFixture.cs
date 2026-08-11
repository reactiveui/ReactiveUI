// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A dependency object that inherits its dependency property from its base type.</summary>
/// <remarks>Resolving the property here forces the hierarchy walk that looks past the declaring type.</remarks>
public class DerivedDependencyObjectFixture : DependencyObjectFixture;
