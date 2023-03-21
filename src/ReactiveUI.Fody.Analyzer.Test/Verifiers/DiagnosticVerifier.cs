// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Xunit;

namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit Tests for DiagnosticAnalyzers.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>DiagnosticAnalyzer to be tested.</returns>
        protected virtual DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => null!;

        /// <summary>
        /// Get the Visual Basic analyzer being tested (C#) - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>DiagnosticAnalyzer to be tested.</returns>
        protected virtual DiagnosticAnalyzer GetBasicDiagnosticAnalyzer() => null!;

        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the single inputted string as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source.</param>
        protected void VerifyCSharpDiagnostic(string source, params DiagnosticResult[] expected) => VerifyDiagnostics(new[] { source }, LanguageNames.CSharp, GetCSharpDiagnosticAnalyzer(), expected);

        /// <summary>
        /// Called to test a VB DiagnosticAnalyzer when applied on the single inputted string as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the source.</param>
        protected void VerifyBasicDiagnostic(string source, params DiagnosticResult[] expected) => VerifyDiagnostics(new[] { source }, LanguageNames.VisualBasic, GetBasicDiagnosticAnalyzer(), expected);

        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the inputted strings as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources.</param>
        protected void VerifyCSharpDiagnostic(string[] sources, params DiagnosticResult[] expected) => VerifyDiagnostics(sources, LanguageNames.CSharp, GetCSharpDiagnosticAnalyzer(), expected);

        /// <summary>
        /// Called to test a VB DiagnosticAnalyzer when applied on the inputted strings as a source
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources.</param>
        protected void VerifyBasicDiagnostic(string[] sources, params DiagnosticResult[] expected) => VerifyDiagnostics(sources, LanguageNames.VisualBasic, GetBasicDiagnosticAnalyzer(), expected);

        /// <summary>
        /// Checks each of the actual Diagnostics found and compares them with the corresponding DiagnosticResult in the array of expected results.
        /// Diagnostics are considered equal only if the DiagnosticResultLocation, Id, Severity, and Message of the DiagnosticResult match the actual diagnostic.
        /// </summary>
        /// <param name="actualResults">The Diagnostics found by the compiler after running the analyzer on the source code.</param>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="expectedResults">Diagnostic Results that should have appeared in the code.</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            var expectedCount = expectedResults.Length;
            var actualCountList = actualResults.ToList();
            var actualCount = actualCountList.Count;

            if (expectedCount != actualCount)
            {
                var diagnosticsOutput = actualCountList.Count > 0 ? FormatDiagnostics(analyzer, actualCountList.ToArray()) : "    NONE.";

                Assert.True(
                    false,
                    $"Mismatch between number of diagnostics returned, expected \"{expectedCount}\" actual \"{actualCount}\"\r\n\r\nDiagnostics:\r\n{diagnosticsOutput}\r\n");
            }

            for (var i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualCountList[i];
                var expected = expectedResults[i];

                if (expected.Line == -1 && expected.Column == -1)
                {
                    if (actual.Location != Location.None)
                    {
                        Assert.True(
                            false,
                            $"Expected:\nA project diagnostic with No location\nActual:\n{FormatDiagnostics(analyzer, actual)}");
                    }
                }
                else
                {
                    VerifyDiagnosticLocation(analyzer, actual, actual.Location, expected.Locations.First());
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    if (additionalLocations.Length != expected.Locations.Count - 1)
                    {
                        Assert.True(
                                      false,
                                      $"Expected {expected.Locations.Count - 1} additional locations but got {additionalLocations.Length} for Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");
                    }

                    for (var j = 0; j < additionalLocations.Length; ++j)
                    {
                        VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j], expected.Locations[j + 1]);
                    }
                }

                if (actual.Id != expected.Id)
                {
                    Assert.True(
                        false,
                        $"Expected diagnostic id to be \"{expected.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");
                }

                if (actual.Severity != expected.Severity)
                {
                    Assert.True(
                        false,
                        $"Expected diagnostic severity to be \"{expected.Severity}\" was \"{actual.Severity}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");
                }

                if (actual.GetMessage() != expected.Message)
                {
                    Assert.True(
                        false,
                        $"Expected diagnostic message to be \"{expected.Message}\" was \"{actual.GetMessage()}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");
                }
            }
        }

        /// <summary>
        /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and compares it with the location in the expected DiagnosticResult.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The Location of the Diagnostic found in the code.</param>
        /// <param name="expected">The DiagnosticResultLocation that should have been found.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in NET472")]
        private static void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            Assert.True(
                          actualSpan.Path == expected.Path || (actualSpan.Path is not null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")),
                          $"Expected diagnostic to be in file \"{expected.Path}\" was actually in file \"{actualSpan.Path}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}\r\n");

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    Assert.True(
                        false,
                        $"Expected diagnostic to be on line \"{expected.Line}\" was actually on line \"{actualLinePosition.Line + 1}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}\r\n");
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    Assert.True(
                        false,
                        $"Expected diagnostic to start at column \"{expected.Column}\" was actually at column \"{actualLinePosition.Character + 1}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}\r\n");
                }
            }
        }

        /// <summary>
        /// Helper method to format a Diagnostic into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests.</param>
        /// <param name="diagnostics">The Diagnostics to be formatted.</param>
        /// <returns>The Diagnostics formatted as a string.</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < diagnostics.Length; ++i)
            {
                builder.Append("// ").AppendLine(diagnostics[i].ToString());

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule is not null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.AppendFormat(CultureInfo.InvariantCulture, "GetGlobalResult({0}.{1})", analyzerType.Name, rule.Id);
                        }
                        else
                        {
                            Assert.True(
                                location.IsInSource,
                                $"Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata: {diagnostics[i]}\r\n");

                            var resultMethodName = diagnostics[i].Location.SourceTree!.FilePath.EndsWith(".cs", StringComparison.Ordinal) ? "GetCSharpResultAt" : "GetBasicResultAt";
                            var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.Append(resultMethodName).Append('(').Append(linePosition.Line + 1).Append(", ").Append(linePosition.Character + 1).Append(", ").Append(analyzerType.Name).Append('.').Append(rule.Id).Append(')');
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzer">The analyzer to be run on the source code.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources.</param>
        private static void VerifyDiagnostics(string[] sources, string language, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expected)
        {
            var diagnostics = GetSortedDiagnostics(sources, language, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }
    }
}
