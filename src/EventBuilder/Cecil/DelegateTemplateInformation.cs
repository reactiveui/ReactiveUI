﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using EventBuilder.Entities;
using Mono.Cecil;
using System;
using System.Linq;

namespace EventBuilder.Cecil
{
    public static class DelegateTemplateInformation
    {
        public static NamespaceInfo[] Create(AssemblyDefinition[] targetAssemblies)
        {
            var garbageTypeList = new[]
            {
                "AVPlayerItemLegibleOutputPushDelegate"
                // NB: Aparrently this used to break "build on device because of reasons". We don't know what these reasons are and this may not be needed anymore.
            };

            var publicDelegateTypes = targetAssemblies
                .SelectMany(x => SafeTypes.GetSafeTypes(x))
                .Where(x => x.IsPublic && !x.IsInterface && !x.HasGenericParameters && IsCocoaDelegateName(x.Name))
                .Where(x => x.BaseType == null || !x.BaseType.FullName.Contains("MulticastDelegate"))
                .Where(x => !garbageTypeList.Any(y => x.FullName.Contains(y)))
                .Select(x => new {Type = x, Delegates = GetPublicDelegateMethods(x)})
                .Where(x => x.Delegates.Length > 0)
                .ToArray();

            var namespaceData = publicDelegateTypes
                .GroupBy(x => x.Type.Namespace)
                //.Where(x => !garbageNamespaceList.Contains(x.Key))    // NB: We don't know why this is disabled.
                .Select(x => new NamespaceInfo
                {
                    Name = x.Key,
                    Types = x.Select(y => new PublicTypeInfo
                    {
                        Name = y.Type.Name,
                        Type = y.Type,
                        Abstract = y.Type.IsAbstract ? "abstract" : "",
                        ZeroParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count == 0).Select(z => new ParentInfo
                            {
                                Name = z.Name
                            }).ToArray(),
                        SingleParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count == 1).Select(z => new SingleParameterMethod
                            {
                                Name = z.Name,
                                ParameterType = z.Parameters[0].ParameterType.FullName,
                                ParameterName = z.Parameters[0].Name
                            }).ToArray(),
                        MultiParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count > 1).Select(z => new MultiParameterMethod
                            {
                                Name = z.Name,
                                ParameterList =
                                    string.Join(", ",
                                        z.Parameters.Select(
                                            a => string.Format("{0} {1}", a.ParameterType.FullName, a.Name))),
                                ParameterTypeList =
                                    string.Join(", ", z.Parameters.Select(a => a.ParameterType.FullName)),
                                ParameterNameList = string.Join(", ", z.Parameters.Select(a => a.Name))
                            }).ToArray()
                    }).ToArray()
                }).ToArray();

            return namespaceData;
        }

        private static bool IsCocoaDelegateName(string name)
        {
            if (name.EndsWith("Delegate", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("UITableViewSource", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static MethodDefinition[] GetPublicDelegateMethods(TypeDefinition t)
        {
            var bannedMethods = new[] {"Dispose", "Finalize"};
            return t.Methods
                .Where(x => x.IsVirtual && !x.IsConstructor && !x.IsSetter && x.ReturnType.FullName == "System.Void")
                .Where(x => x.Parameters.All(y => y.ParameterType.FullName.Contains("&") == false))
                .Where(x => !bannedMethods.Contains(x.Name))
                .GroupBy(x => x.Name).Select(x => x.OrderByDescending(y => y.Parameters.Count).First())
                .ToArray();
        }
    }
}