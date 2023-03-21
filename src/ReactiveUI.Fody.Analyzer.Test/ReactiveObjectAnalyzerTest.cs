// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace ReactiveUI.Fody.Analyzer.Test
{
    /// <summary>
    /// Unit Tests to check the proper operation of ReactiveObjectAnalyzer.
    /// </summary>
    public class ReactiveObjectAnalyzerTest : DiagnosticVerifier
    {
        /// <summary>
        /// Unit Test to ensure that we do not flag an empty file with errors.
        /// </summary>
        [Fact]
        public void CheckEmptyFileReturnsNoFailures()
        {
            var test = string.Empty;
            VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Check that a class which does not implement IReactiveObject throws an error, when it uses
        /// the [Reactive] attribute in one of its properties.
        /// </summary>
        [Fact]
        public void ShouldGiveAnErrorWhenClassDoesNotImplement()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;

    namespace ConsoleApplication1
    {
        public class TypeName
        {   
            [Reactive] public string Prop { get; set; }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = "RUI_0001",
                Message = "Type 'TypeName' does not implement IReactiveObject",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 15, 14) }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        /// <summary>
        /// Check that a class which does inherits ReactiveObject does not throw
        /// an error, when it uses the [Reactive] attribute in one of its properties.
        /// </summary>
        [Fact]
        public void ShouldNotGiveAnErrorWhenClassInherits()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;

    namespace ConsoleApplication1
    {
        public class TypeName : ReactiveObject
        {   
            [Reactive] public string Prop { get; set; }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Check that a class which does implements IReactiveObject does not throw
        /// an error, when it uses the [Reactive] attribute in one of its properties.
        /// </summary>
        [Fact]
        public void ShouldNotGiveAnErrorWhenClassImplements()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;

    namespace ConsoleApplication1
    {
        public class TypeName : IReactiveObject
        {   
            [Reactive] public string Prop { get; set; }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Check that a class should not be allowed to have a non-auto-property
        /// when used with the [Reactive] attribute.
        /// </summary>
        [Fact]
        public void ShouldGiveErrorForNonAutoProperty()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;

    namespace ConsoleApplication1
    {
        public class TypeName : IReactiveObject
        {   
            [Reactive] public string Prop
            {
                get => _prop;
                set => _prop = value;
            }
            private string _prop;
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = "RUI_0002",
                Message = "Property 'Prop' on 'TypeName' should be an auto property",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 15, 14) }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        /// <summary>
        /// Returns the Roslyn Analyzer under test.
        /// </summary>
        /// <returns>ReactiveObjectAnalyzer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new ReactiveObjectAnalyzer();
    }
}
