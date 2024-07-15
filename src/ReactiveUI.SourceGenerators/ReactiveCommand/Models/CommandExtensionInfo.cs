// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Input.Models;

internal record CommandExtensionInfo(
    string MethodName,
    ITypeSymbol MethodReturnType,
    ITypeSymbol? ArgumentType,
    bool IsTask,
    bool IsReturnTypeVoid,
    bool IsObservable)
{
    private const string UnitTypeName = "global::System.Reactive.Unit";

    public string GetOutputTypeText() => IsReturnTypeVoid
            ? UnitTypeName
            : MethodReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public string GetInputTypeText() => ArgumentType == null
            ? UnitTypeName
            : ArgumentType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}
