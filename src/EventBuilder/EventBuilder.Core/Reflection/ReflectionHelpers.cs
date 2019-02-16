// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using EventBuilder.Core.Reflection.Compilation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Core.Reflection
{
    internal static class ReflectionHelpers
    {
        public static ISet<string> SkipNamespaceList { get; } = new HashSet<string>
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

        public static ICompilation GetCompilation(IEnumerable<string> targetAssemblies, IEnumerable<string> searchDirectories)
        {
            var modules = targetAssemblies.Select(x => new PEFile(x, PEStreamOptions.PrefetchMetadata));

            return new EventBuilderCompiler(modules, searchDirectories);
        }
    }
}
