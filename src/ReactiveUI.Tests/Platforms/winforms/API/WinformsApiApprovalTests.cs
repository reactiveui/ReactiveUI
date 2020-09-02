﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicApiGenerator;
using Shouldly;
using Xunit;

namespace ReactiveUI.Tests
{
    [ExcludeFromCodeCoverage]
    public class WinformsApiApprovalTests : ApiApprovalBase
    {
        [Fact]
        public void Winforms()
        {
            CheckApproval(typeof(ReactiveUI.Winforms.WinformsCreatesObservableForProperty).Assembly);
        }
    }
}
