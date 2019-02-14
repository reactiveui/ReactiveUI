// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Reflection.Resolvers
{
    /// <summary>
    /// A namespace resolver that extracts event information.
    /// </summary>
    internal class PublicEventNamespaceResolver : EventNamespaceResolverBase
    {
        public override IDictionary<string, string> SubstitutionList { get; } = new Dictionary<string, string>
        {
            ////["Windows.UI.Xaml.Data.PropertyChangedEventArgs"] = "global::System.ComponentModel.PropertyChangedEventArgs",
            ////["Windows.UI.Xaml.Data.PropertyChangedEventHandler"] = "global::System.ComponentModel.PropertyChangedEventHandler",
            ////["Windows.Foundation.EventHandler"] = "EventHandler",
            ////["Windows.Foundation.EventHandler`1"] = "EventHandler",
            ////["Windows.Foundation.EventHandler`2"] = "EventHandler",
            ////["System.Boolean"] = "Boolean",
            ////["System.Boolean`1"] = "Boolean",
            ////["System.EventHandler"] = "EventHandler",
            ////["System.EventHandler`1"] = "EventHandler",
            ////["System.EventHandler`2"] = "EventHandler",
            ////["System.EventArgs"] = "EventArgs",
            ////["System.EventArgs`1"] = "EventArgs",
            ////["System.EventArgs`2"] = "EventArgs",
            ////["Tizen.NUI.EventHandlerWithReturnType"] = "Tizen.NUI.EventHandlerWithReturnType",
            ////["Tizen.NUI.EventHandlerWithReturnType`1"] = "Tizen.NUI.EventHandlerWithReturnType",
            ////["Tizen.NUI.EventHandlerWithReturnType`2"] = "Tizen.NUI.EventHandlerWithReturnType",
            ////["Tizen.NUI.EventHandlerWithReturnType`3"] = "Tizen.NUI.EventHandlerWithReturnType",
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

        /// <inheritdoc />
        public override string TemplatePath => TemplateManager.DefaultMustacheTemplate;

        protected override IEnumerable<IEvent> GetValidEventDetails(IEnumerable<IEvent> eventDetails)
        {
            return eventDetails.Where(x => x.Accessibility == Accessibility.Public && !x.IsStatic);
        }

        protected override IEnumerable<ITypeDefinition> GetPublicTypesWithEvents(ICompilation compilation)
        {
            return compilation.GetPublicTypesWithNotStaticEvents();
        }
    }
}
