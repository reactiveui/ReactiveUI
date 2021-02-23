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

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// Tests associated with UI and the <see cref="IDependencyResolver"/>.
    /// </summary>
    public sealed class XamlViewDependencyResolverTests : IDisposable
    {
        private readonly IDependencyResolver _resolver;

        public XamlViewDependencyResolverTests()
        {
            _resolver = new ModernDependencyResolver();
            _resolver.InitializeSplat();
            _resolver.InitializeReactiveUI();
            _resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        [Fact]
        public void RegisterViewsForViewModelShouldRegisterAllViews()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices<IViewFor<ExampleViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<AnotherViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>());
            }
        }

        [Fact]
        public void RegisterViewsForViewModelShouldIncludeContracts()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        public void Dispose() => _resolver?.Dispose();
    }
}
