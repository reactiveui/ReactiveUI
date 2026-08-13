// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using ReactiveUI.Tests.WinUI.Markup;

namespace ReactiveUI.Tests.WinUI;

/// <summary>The XAML application instance that keeps the test process's XAML core alive.</summary>
/// <remarks>
/// <see cref="Application.Start"/> requires an <see cref="Application"/> to be constructed inside its
/// initialization callback; it becomes <see cref="Application.Current"/> for the lifetime of the process. The XAML
/// runtime resolves the types named in markup through the current application, which is why this one also carries
/// a metadata provider — an application with compiled markup would get an equivalent one from the XAML compiler.
/// </remarks>
internal sealed partial class WinUITestApplication : Application, IXamlMetadataProvider
{
    /// <summary>Describes the ReactiveUI types the tests' markup names.</summary>
    private readonly TestXamlMetadataProvider _metadata = new();

    /// <inheritdoc/>
    public IXamlType GetXamlType(Type type) => _metadata.GetXamlType(type);

    /// <inheritdoc/>
    public IXamlType? GetXamlType(string fullName) => _metadata.GetXamlType(fullName);

    /// <inheritdoc/>
    public XmlnsDefinition[] GetXmlnsDefinitions() => _metadata.GetXmlnsDefinitions();
}
