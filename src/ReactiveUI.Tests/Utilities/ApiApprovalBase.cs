// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
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

namespace ReactiveUI.Tests
{
    [ExcludeFromCodeCoverage]
    public abstract class ApiApprovalBase
    {
        private static readonly Regex _removeCoverletSectionRegex = new(@"^namespace Coverlet\.Core\.Instrumentation\.Tracker.*?^}", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled);

        protected static void CheckApproval(Assembly assembly, [CallerMemberName]string? memberName = null, [CallerFilePath]string? filePath = null)
        {
            var targetFrameworkName = Assembly.GetExecutingAssembly().GetTargetFrameworkName();

            var sourceDirectory = Path.GetDirectoryName(filePath);

            if (sourceDirectory is null)
            {
                throw new ArgumentNullException(filePath);
            }

            var approvedFileName = Path.Combine(sourceDirectory, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.approved.txt");
            var receivedFileName = Path.Combine(sourceDirectory, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.received.txt");

            string approvedPublicApi = string.Empty;

            if (File.Exists(approvedFileName))
            {
                approvedPublicApi = File.ReadAllText(approvedFileName);
            }

            var generatorOptions = new ApiGeneratorOptions { WhitelistedNamespacePrefixes = new[] { "ReactiveUI", "System.Reactive" } };
            var receivedPublicApi = Filter(ApiGenerator.GeneratePublicApi(assembly, generatorOptions));

            if (!string.Equals(receivedPublicApi, approvedPublicApi, StringComparison.InvariantCulture))
            {
                File.WriteAllText(receivedFileName, receivedPublicApi);
                try
                {
                    DiffRunner.Launch(receivedFileName, approvedFileName);
                }
                catch (Exception)
                {
                    var process = new Process
                    {
                      StartInfo = new ProcessStartInfo
                      {
                          Arguments = $"\"{approvedFileName}\" \"{receivedFileName}\"",
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          CreateNoWindow = true
                      }
                    };
#if NET_461
                    process.StartInfo.FileName = "FC";
#else
                    process.StartInfo.FileName = "diff";
#endif
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    throw new Exception("Invalid API configuration: " + Environment.NewLine + output);
                }
            }

            Assert.Equal(approvedPublicApi, receivedPublicApi);
        }

        private static string Filter(string text)
        {
            text = _removeCoverletSectionRegex.Replace(text, string.Empty);
            return string.Join(Environment.NewLine, text.Split(
                new[]
                {
                    Environment.NewLine
                },
                StringSplitOptions.RemoveEmptyEntries)
                    .Where(l =>
                    !l.StartsWith("[assembly: AssemblyVersion(", StringComparison.InvariantCulture) &&
                    !l.StartsWith("[assembly: AssemblyFileVersion(", StringComparison.InvariantCulture) &&
                    !l.StartsWith("[assembly: AssemblyInformationalVersion(", StringComparison.InvariantCulture) &&
                    !string.IsNullOrWhiteSpace(l)));
        }
    }
}
