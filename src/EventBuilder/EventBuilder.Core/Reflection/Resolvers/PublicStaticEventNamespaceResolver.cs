// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Core.Reflection.Resolvers
{
    internal class PublicStaticEventNamespaceResolver : EventNamespaceResolverBase
    {
        public override string TemplatePath => TemplateManager.StaticMustacheTemplate;

        protected override IEnumerable<ITypeDefinition> GetPublicTypesWithEvents(ICompilation compilation)
        {
            return compilation.GetPublicTypesWithStaticEvents();
        }

        protected override IEnumerable<IEvent> GetValidEventDetails(IEnumerable<IEvent> eventDetails)
        {
            return eventDetails.Where(x => x.Accessibility == Accessibility.Public && x.IsStatic);
        }
    }
}
