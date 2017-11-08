// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using Mono.Cecil;

namespace ReactiveUI.Fody
{
    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        // Will log an MessageImportance.High message to MSBuild. 
        public Action<string> LogInfo  { get; set; }

        // Will log an error message to MSBuild. OPTIONAL
        public Action<string> LogError { get; set; }

        public void Execute()
        {
            var propertyWeaver = new ReactiveUIPropertyWeaver
            {
                ModuleDefinition = ModuleDefinition,
                LogInfo = LogInfo,
                LogError = LogError
            };
            propertyWeaver.Execute();

            var observableAsPropertyWeaver = new ObservableAsPropertyWeaver
            {
                ModuleDefinition = ModuleDefinition,
                LogInfo = LogInfo
            };
            observableAsPropertyWeaver.Execute();

            var reactiveDependencyWeaver = new ReactiveDependencyPropertyWeaver
            {
                ModuleDefinition = ModuleDefinition,
                LogInfo = LogInfo,
                LogError = LogError
            };
            reactiveDependencyWeaver.Execute();
        }
    }
}