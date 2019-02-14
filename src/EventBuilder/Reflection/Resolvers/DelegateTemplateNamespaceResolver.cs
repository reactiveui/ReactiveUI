// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventBuilder.Entities;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Reflection.Resolvers
{
    internal class DelegateTemplateNamespaceResolver : INamespaceResolver
    {
        private const string DelegateName = "MulticastDelegate";
        private static readonly string[] CocoaDelegateNames = new string[]
        {
            "Delegate",
            "UITableViewSource"
        };

        private static readonly string[] BannedMethods = new string[]
        {
            "Dispose",
            "Finalize"
        };

        // NB: Aparrently this used to break "build on device because of reasons". We don't know what these reasons are and this may not be needed anymore.
        private static readonly string[] _garbageTypeList = new string[] { "AVPlayerItemLegibleOutputPushDelegate" };

        public string TemplatePath => TemplateManager.DelegateTemplate;

        public IEnumerable<NamespaceInfo> Create(ICompilation compilation)
        {
            var publicDelegateTypes = compilation.GetPublicNonGenericTypeDefinitions()
                .Where(x => x.Kind != TypeKind.Interface)
                .Where(HasValidDelegateClass)
                .Where(x => !_garbageTypeList.Any(y => x.FullName.Contains(y, StringComparison.InvariantCulture)))
                .Where(x => CocoaDelegateNames.Any(cocoaName => x.FullName.EndsWith(cocoaName, StringComparison.OrdinalIgnoreCase)))
                .Select(x => new { Type = x, Delegates = GetPublicDelegateMethods(x) })
                .Where(x => x.Delegates.Length > 0);

            return publicDelegateTypes
                .GroupBy(x => x.Type.Namespace)
                .Select(x => new NamespaceInfo
                {
                    Name = x.Key,
                    Types = x.Select(y => new PublicTypeInfo
                    {
                        Name = y.Type.Name,
                        FullName = y.Type.FullName,
                        ////Type = y.Type,
                        Abstract = y.Type.IsAbstract ? "abstract" : string.Empty,
                        ZeroParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count == 0).Select(z => new ZeroParameterMethod
                            {
                                Name = z.Name
                            }),
                        SingleParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count == 1).Select(z => new SingleParameterMethod
                            {
                                Name = z.Name,
                                ParameterType = z.Parameters[0].Type.FullName,
                                ParameterName = z.Parameters[0].Name
                            }),
                        MultiParameterMethods =
                            y.Delegates.Where(z => z.Parameters.Count > 1).Select(z => new MultiParameterMethod
                            {
                                Name = z.Name,
                                ParameterList =
                                    string.Join(
                                        ", ",
                                        z.Parameters.Select(
                                            a => string.Format(
                                                CultureInfo.InvariantCulture,
                                                "{0} {1}",
                                                a.Type.FullName,
                                                a.Name))),
                                ParameterTypeList =
                                    string.Join(", ", z.Parameters.Select(a => a.Type.FullName)),
                                ParameterNameList = string.Join(", ", z.Parameters.Select(a => a.Name))
                            })
                    })
                });
        }

        private static bool HasValidDelegateClass(ITypeDefinition typeDefinition)
        {
            return typeDefinition.DirectBaseTypes.Any(x => x.Kind != TypeKind.Interface && x.FullName.Contains(DelegateName, StringComparison.InvariantCulture));
        }

        private static IMethod[] GetPublicDelegateMethods(ITypeDefinition typeDefinition)
        {
            return typeDefinition.Methods
                .Where(x => x.IsVirtual && !x.IsConstructor && !x.IsAccessor && x.ReturnType.FullName == "System.Void" && x.Parameters.All(y => !y.IsRef) && !BannedMethods.Contains(x.Name))
                .GroupBy(x => x.Name)
                .Select(x => x.OrderByDescending(y => y.Parameters.Count).First())
                .ToArray();
        }
    }
}
