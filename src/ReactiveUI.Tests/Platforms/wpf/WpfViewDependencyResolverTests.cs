﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
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

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Wpf
{
    /// <summary>
    /// Tests for the WPF View Resolver.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class WpfViewDependencyResolverTests : IDisposable
    {
        private readonly IDependencyResolver _resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfViewDependencyResolverTests"/> class.
        /// </summary>
        public WpfViewDependencyResolverTests()
        {
            _resolver = new ModernDependencyResolver();
            _resolver.InitializeSplat();
            _resolver.InitializeReactiveUI();
            _resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        /// <summary>
        /// Tests that  Register views for view model should register all views.
        /// </summary>
        [Fact]
        public void RegisterViewsForViewModelShouldRegisterAllViews()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>());
            }
        }

        /// <inheritdoc/>
        public void Dispose() => _resolver?.Dispose();
    }
}
