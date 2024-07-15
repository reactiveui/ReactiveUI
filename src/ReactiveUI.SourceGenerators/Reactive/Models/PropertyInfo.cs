// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI.SourceGenerators.Helpers;

namespace ReactiveUI.SourceGenerators.Input.Models;

/// <summary>
/// A model with gathered info on a given field.
/// </summary>
internal sealed record PropertyInfo(
    string TypeNameWithNullabilityAnnotations,
    string FieldName,
    string PropertyName,
    bool IsReferenceTypeOrUnconstraindTypeParameter,
    bool IncludeMemberNotNullOnSetAccessor);
