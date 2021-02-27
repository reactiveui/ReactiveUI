// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Checks the WinForms API to make sure there aren't any unexpected public API changes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WinformsApiApprovalTests : ApiApprovalBase
    {
        /// <summary>
        /// Checks the approved vs the received API.
        /// </summary>
        [Fact]
        public void Winforms() => CheckApproval(typeof(ReactiveUI.Winforms.WinformsCreatesObservableForProperty).Assembly);
    }
}
