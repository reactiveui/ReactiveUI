// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;

namespace ReactiveUI.Tests.WinUI;

/// <summary>The XAML application instance that keeps the test process's XAML core alive.</summary>
/// <remarks>
/// <see cref="Application.Start"/> requires an <see cref="Application"/> to be constructed inside its
/// initialization callback; it becomes <see cref="Application.Current"/> for the lifetime of the process. It has no
/// compiled markup and no <c>IXamlMetadataProvider</c>, like a code-only app, so any markup ReactiveUI parses at run
/// time has to name framework types only.
/// </remarks>
internal sealed partial class WinUITestApplication : Application;
