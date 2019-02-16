// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventBuilder.Core.Entities;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Core.Reflection.Resolvers
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
                .Where(
                    x => x.Kind != TypeKind.Interface
                    && (!IsMulticastDelegateDerived(x)
                    || !x.DirectBaseTypes.Any())
                    && !_garbageTypeList.Any(y => x.FullName.Contains(y))
                    && CocoaDelegateNames.Any(cocoaName => x.FullName.EndsWith(cocoaName, StringComparison.OrdinalIgnoreCase)))
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
                                ParameterType = z.Parameters[0].Type.GenerateFullGenericName(),
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
                                                a.Type.GenerateFullGenericName(),
                                                a.Name))),
                                ParameterTypeList =
                                    string.Join(", ", z.Parameters.Select(a => a.Type.GenerateFullGenericName())),
                                ParameterNameList = string.Join(", ", z.Parameters.Select(a => a.Name))
                            })
                    })
                });
        }

        private static bool IsMulticastDelegateDerived(ITypeDefinition typeDefinition)
        {
            return typeDefinition.DirectBaseTypes.Any(x => x.FullName.Contains(DelegateName));
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
