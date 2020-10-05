using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using DryIoc;
using Ninject;
using Shouldly;
using Splat;
using Splat.Autofac;
using Splat.DryIoc;
using Splat.Ninject;
using Xunit;

namespace ReactiveUI.Splat.Tests
{
    public class SplatAdapterTests
    {
        /// <summary>
        /// Shoulds register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void DryIocDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var scheduler = RxApp.MainThreadScheduler;
            var container = new Container();
            container.UseDryIocDependencyResolver();

            var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(StringConverter));
            converters.ShouldContain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Shoulds register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void DryIocDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var scheduler = RxApp.MainThreadScheduler;
            var container = new Container();
            container.UseDryIocDependencyResolver();

            var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }

        /// <summary>
        /// Shoulds register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void AutofacDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var builder = new ContainerBuilder();
            Locator.SetLocator(new AutofacDependencyResolver(builder));
            var container = builder.Build();

            var converters = container.Resolve<IEnumerable<IBindingTypeConverter>>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(StringConverter));
            converters.ShouldContain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Shoulds register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void AutofacDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var builder = new ContainerBuilder();
            Locator.SetLocator(new AutofacDependencyResolver(builder));
            var container = builder.Build();

            var converters = container.Resolve<IEnumerable<ICreatesCommandBinding>>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }

        /// <summary>
        /// Shoulds register ReactiveUI binding type converters.
        /// </summary>
        [Fact]
        public void NinjectDependencyResolver_Should_Register_ReactiveUI_BindingTypeConverters()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var container = new StandardKernel();
            container.UseNinjectDependencyResolver();

            var converters = container.GetAll<IBindingTypeConverter>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(StringConverter));
            converters.ShouldContain(x => x.GetType() == typeof(EqualityTypeConverter));
        }

        /// <summary>
        /// Shoulds register ReactiveUI creates command bindings.
        /// </summary>
        [Fact]
        public void NinjectDependencyResolver_Should_Register_ReactiveUI_CreatesCommandBinding()
        {
            // Invoke RxApp which initializes the ReactiveUI platform.
            var scheduler = RxApp.MainThreadScheduler;
            var container = new StandardKernel();
            container.UseNinjectDependencyResolver();

            var converters = container.GetAll<ICreatesCommandBinding>().ToList();

            converters.ShouldNotBeNull();
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaEvent));
            converters.ShouldContain(x => x.GetType() == typeof(CreatesCommandBindingViaCommandParameter));
        }
    }
}
