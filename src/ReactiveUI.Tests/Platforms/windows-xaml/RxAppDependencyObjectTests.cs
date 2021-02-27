// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xunit;

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// Checks RxApp dependency objects.
    /// </summary>
    public class RxAppDependencyObjectTests
    {
        /// <summary>
        /// Tests that Dependency Property notifiers should be found.
        /// </summary>
        [Fact]
        public void DepPropNotifierShouldBeFound()
        {
            RxApp.EnsureInitialized();

            Assert.True(Locator.Current.GetServices<ICreatesObservableForProperty>()
                               .Any(x => x is DependencyObjectObservableForProperty));
        }
    }
}
