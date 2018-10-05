// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Mono.Cecil;

namespace EventBuilder.Cecil
{
    public static class SafeTypes
    {
        public static TypeDefinition[] GetSafeTypes(AssemblyDefinition a) =>
            a.Modules.SelectMany(x => x.GetTypes()).ToArray();
    }
}
