// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PublicApiGenerator;
using ReactiveUI.Fody.Helpers;
using Shouldly;
using Xunit;

namespace ReactiveUI.Fody.Tests.API
{
    [ExcludeFromCodeCoverage]
    public class ApiApprovalTests
    {
        [Fact]
        public void ReactiveUIFody()
        {
            var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(ReactiveAttribute).Assembly));
            publicApi.ShouldMatchApproved();
        }

        private static string Filter(string text)
        {
            return string.Join(Environment.NewLine, text.Split(
                new[]
                {
                    Environment.NewLine
                }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => !l.StartsWith("[assembly: AssemblyVersion("))
                    .Where(l => !l.StartsWith("[assembly: AssemblyFileVersion("))
                    .Where(l => !l.StartsWith("[assembly: AssemblyInformationalVersion("))
                    .Where(l => !string.IsNullOrWhiteSpace(l)));
        }
    }
}
