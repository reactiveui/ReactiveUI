// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using VerifyXunit;

using Xunit;

namespace ReactiveUI.Tests.API
{
    /// <summary>
    /// Checks to make sure that the API is consistent with previous releases, and new API changes are highlighted.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [UsesVerify]
    public class ApiApprovalTests : ApiApprovalBase
    {
        /// <summary>
        /// Generates public API for the ReactiveUI.Testing API.
        /// </summary>
        /// <returns>A task to monitor the process.</returns>
        [Fact]
        public Task Testing() => CheckApproval(typeof(Testing.SchedulerExtensions).Assembly);

        /// <summary>
        /// Generates public API for the ReactiveUI API.
        /// </summary>
        /// <returns>A task to monitor the process.</returns>
        [Fact]
        public Task ReactiveUI() => CheckApproval(typeof(RxApp).Assembly);
    }
}
