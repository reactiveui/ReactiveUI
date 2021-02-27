// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

namespace ReactiveUI.Tests.API
{
    /// <summary>
    /// Checks to make sure that the API is consistent with previous releases, and new API changes are highlighted.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApiApprovalTests : ApiApprovalBase
    {
        /// <summary>
        /// Generates public API for the ReactiveUI.Testing API.
        /// </summary>
        [Fact]
        public void Testing() => CheckApproval(typeof(Testing.SchedulerExtensions).Assembly);

        /// <summary>
        /// Generates public API for the ReactiveUI API.
        /// </summary>
        [Fact]
        public void ReactiveUI() => CheckApproval(typeof(RxApp).Assembly);
    }
}
