// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerifyXunit;

using Xunit;

namespace ReactiveUI.Tests.Wpf
{
    /// <summary>
    /// Checks the WPF API to make sure there aren't any unexpected public API changes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [UsesVerify]
    public class WpfApiApprovalTests : ApiApprovalBase
    {
        /// <summary>
        /// Checks the approved vs the received API.
        /// </summary>
        /// <returns>A task to monitor the process.</returns>
        [Fact(Skip = "Need to resolve GeneratedInternalTypeHelper")]
        public Task Wpf() => CheckApproval(typeof(ReactiveWindow<>).Assembly);
    }
}
