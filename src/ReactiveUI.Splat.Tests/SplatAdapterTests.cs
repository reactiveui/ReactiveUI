// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using DryIoc;

using FluentAssertions;

using Ninject;

using Splat;
using Splat.Autofac;
using Splat.DryIoc;
using Splat.Ninject;

using Xunit;

namespace ReactiveUI.Splat.Tests
{
    /// <summary>
    /// Tests for checking the various adapters in splat.
    /// </summary>
    public class SplatAdapterTests
    {
        /// <summary>
        /// Should register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void DryIocDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var container = new Container();
            container.UseDryIocDependencyResolver();
            Locator.CurrentMutable.InitializeReactiveUI();

            var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(StringConverter));
            converters.Should().Contain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Should register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void DryIocDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var container = new Container();
            container.UseDryIocDependencyResolver();
            Locator.CurrentMutable.InitializeReactiveUI();

            var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }

        /// <summary>
        /// Should register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void AutofacDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var builder = new ContainerBuilder();
            var locator = new AutofacDependencyResolver(builder);
            locator.InitializeReactiveUI();
            var container = builder.Build();

            var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(StringConverter));
            converters.Should().Contain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Should register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void AutofacDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var builder = new ContainerBuilder();
            var locator = new AutofacDependencyResolver(builder);
            locator.InitializeReactiveUI();
            Locator.SetLocator(locator);
            var container = builder.Build();

            var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }

        /// <summary>
        /// Should register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void NinjectDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var container = new StandardKernel();
            container.UseNinjectDependencyResolver();
            Locator.CurrentMutable.InitializeReactiveUI();

            var converters = container.GetAll<IBindingTypeConverter>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(StringConverter));
            converters.Should().Contain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Should register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void NinjectDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var container = new StandardKernel();
            container.UseNinjectDependencyResolver();
            Locator.CurrentMutable.InitializeReactiveUI();

            var converters = container.GetAll<ICreatesCommandBinding>().ToList();

            converters.Should().NotBeNull();
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.Should().Contain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }
    }
}
