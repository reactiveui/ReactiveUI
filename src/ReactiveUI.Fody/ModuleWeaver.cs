// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Fody;

namespace ReactiveUI.Fody;

/// <summary>
/// ReactiveUI module weaver.
/// </summary>
/// <seealso cref="BaseModuleWeaver" />
public class ModuleWeaver : BaseModuleWeaver
{
    /// <inheritdoc/>
    public override void Execute()
    {
        var propertyWeaver = new ReactiveUIPropertyWeaver
        {
            ModuleDefinition = ModuleDefinition,
            LogInfo = WriteInfo,
            LogError = WriteError
        };
        propertyWeaver.Execute();

        var observableAsPropertyWeaver = new ObservableAsPropertyWeaver
        {
            ModuleDefinition = ModuleDefinition,
            LogInfo = WriteInfo,
            FindType = FindTypeDefinition
        };
        observableAsPropertyWeaver.Execute();

        var reactiveDependencyWeaver = new ReactiveDependencyPropertyWeaver
        {
            ModuleDefinition = ModuleDefinition,
            LogInfo = WriteInfo,
            LogError = WriteError
        };
        reactiveDependencyWeaver.Execute();
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "netstandard";
        yield return "System";
        yield return "System.Runtime";
        yield return "ReactiveUI";
        yield return "ReactiveUI.Fody.Helpers";
    }
}