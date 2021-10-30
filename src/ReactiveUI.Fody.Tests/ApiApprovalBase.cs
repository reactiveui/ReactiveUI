// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using DiffEngine;

using PublicApiGenerator;

using Splat;
using Xunit;

namespace ReactiveUI.Fody.Tests;

/// <summary>
/// Tests for API approvals.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ApiApprovalBase
{
    private static readonly Regex _removeCoverletSectionRegex = new(@"^namespace Coverlet\.Core\.Instrumentation\.Tracker.*?^}", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Checks the assembly to detect the public API. Generates a received/approved version of the API.
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <param name="memberName">Auto populated member name.</param>
    /// <param name="filePath">Auto populated file path.</param>
    protected static void CheckApproval(Assembly assembly, [CallerMemberName]string? memberName = null, [CallerFilePath]string? filePath = null)
    {
        var targetFrameworkName = Assembly.GetExecutingAssembly().GetTargetFrameworkName();

        var sourceDirectory = Path.GetDirectoryName(filePath);

        if (sourceDirectory is not null)
        {
            var approvedFileName = Path.Combine(sourceDirectory, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.approved.txt");
            var receivedFileName = Path.Combine(sourceDirectory, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.received.txt");

            var approvedPublicApi = string.Empty;

            if (File.Exists(approvedFileName))
            {
                approvedPublicApi = File.ReadAllText(approvedFileName);
            }

            var receivedPublicApi = Filter(assembly.GeneratePublicApi(new()));

            if (!string.Equals(receivedPublicApi, approvedPublicApi, StringComparison.InvariantCulture))
            {
                File.WriteAllText(receivedFileName, receivedPublicApi);
                DiffRunner.Launch(receivedFileName, approvedFileName);
            }

            Assert.Equal(approvedPublicApi, receivedPublicApi);
        }
    }

    private static string Filter(string text)
    {
        text = _removeCoverletSectionRegex.Replace(text, string.Empty);
        return string.Join(Environment.NewLine, text.Split(
                                                           new[]
                                                           {
                                                               Environment.NewLine,
                                                           },
                                                           StringSplitOptions.RemoveEmptyEntries)
                                                    .Where(l =>
                                                               !l.StartsWith("[assembly: AssemblyVersion(", StringComparison.InvariantCulture) &&
                                                               !l.StartsWith("[assembly: AssemblyFileVersion(", StringComparison.InvariantCulture) &&
                                                               !l.StartsWith("[assembly: AssemblyInformationalVersion(", StringComparison.InvariantCulture) &&
                                                               !string.IsNullOrWhiteSpace(l)));
    }
}
