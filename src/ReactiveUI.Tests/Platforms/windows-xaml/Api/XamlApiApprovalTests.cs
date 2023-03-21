// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// API approvals for the xaml project.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XamlApiApprovalTests : ApiApprovalBase
    {
        /// <summary>
        /// Generates the public API for the blend project.
        /// </summary>
        /// <returns>A task to monitor the process.</returns>
        [Fact]
        public Task Blend() => CheckApproval(typeof(Blend.FollowObservableStateBehavior).Assembly);
    }
}
