// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PublicApiGenerator;
using Shouldly;
using Xunit;

namespace ReactiveUI.Tests.API
{
    [ExcludeFromCodeCoverage]
    public class ApiApprovalTests
    {
        [Fact]
        public void Blend()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(Blend.ObservableTrigger).Assembly));
            publicApi.ShouldMatchApproved();
        }

        [Fact]
        public void Testing()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(Testing.TestUtils).Assembly));
            publicApi.ShouldMatchApproved();
        }

        [Fact]
        public void ReactiveUI()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(RxApp).Assembly));
            publicApi.ShouldMatchApproved();
        }

        [Fact]
        public void Winforms()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveUI.Winforms.WinformsCreatesObservableForProperty).Assembly));

            publicApi.ShouldMatchApproved();
        }

        private static string Filter(string text)
        {
            return string.Join(Environment.NewLine, text.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.StartsWith("[assembly: AssemblyVersion("))
                .Where(l => !l.StartsWith("[assembly: AssemblyFileVersion("))
                .Where(l => !l.StartsWith("[assembly: AssemblyInformationalVersion("))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                );
        }
    }
}
