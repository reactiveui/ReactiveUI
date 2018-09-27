// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Fody;

namespace ReactiveUI.Fody
{
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
}
