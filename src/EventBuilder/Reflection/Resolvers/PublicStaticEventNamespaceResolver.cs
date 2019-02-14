// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Reflection.Resolvers
{
    internal class PublicStaticEventNamespaceResolver : EventNamespaceResolverBase
    {
        public override IDictionary<string, string> SubstitutionList { get; } = new Dictionary<string, string>
        {
        };

        public override ISet<string> GarbageNamespaceList { get; } = new HashSet<string>
        {
            "Windows.UI.Xaml.Data",
            "Windows.UI.Xaml.Interop",
            "Windows.UI.Xaml.Input",
            "MonoTouch.AudioToolbox",
            "MonoMac.AudioToolbox",
            "ReactiveUI.Events",

            // Winforms
            "System.Collections.Specialized",
            "System.Configuration",
            "System.ComponentModel.Design",
            "System.ComponentModel.Design.Serialization",
            "System.CodeDom",
            "System.Data.SqlClient",
            "System.Data.OleDb",
            "System.Data.Odbc",
            "System.Data.Common",
            "System.Drawing.Design",
            "System.Media",
            "System.Net",
            "System.Net.Mail",
            "System.Net.NetworkInformation",
            "System.Net.Sockets",
            "System.ServiceProcess.Design",
            "System.Windows.Input",
            "System.Windows.Forms.ComponentModel.Com2Interop",
            "System.Windows.Forms.Design",
            "System.Timers"
        };

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
