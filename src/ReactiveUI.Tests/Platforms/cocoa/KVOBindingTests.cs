// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Xunit;
using ReactiveUI.Cocoa;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A sample controller.
    /// </summary>
    public class FooController : ReactiveViewController, IViewFor<PropertyBindViewModel>
    {
        private PropertyBindViewModel _viewModel;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public PropertyBindViewModel ViewModel {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; }
        }
    }

    /// <summary>
    /// Tests for checking KVO bindings.
    /// </summary>
    public class KVOBindingTests
    {
        /// <summary>
        /// Makes the sure kvo bindings bind to kvo things.
        /// </summary>
        [Test]
        public void MakeSureKVOBindingsBindToKVOThings()
        {
            var input = new FooController();
            var fixture = new KVOObservableForProperty();

            Assert.That(fixture.GetAffinityForObject(typeof(FooController, Is.Not.EqualTo(0)), "View"));
            Assert.That(fixture.GetAffinityForObject(typeof(FooController, Is.EqualTo(0)), "ViewModel"));
        }
    }
}
