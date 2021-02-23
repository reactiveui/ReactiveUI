// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

using ReactiveUI.Fody.Helpers;

using Xunit;

namespace ReactiveUI.Fody.Tests.API
{
    [ExcludeFromCodeCoverage]
    public class ApiApprovalTests : ApiApprovalBase
    {
        [Fact]
        public void ReactiveUIFody() => CheckApproval(typeof(ReactiveAttribute).Assembly);
    }
}
