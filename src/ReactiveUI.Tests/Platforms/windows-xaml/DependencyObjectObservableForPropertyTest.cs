﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Windows;

using DynamicData;
using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using FactAttribute = Xunit.WpfFactAttribute;
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class DependencyObjectObservableForPropertyTest
    {
        [Fact]
        public void DependencyObjectObservableForPropertySmokeTest()
        {
            var fixture = new DepObjFixture();
            var binder = new DependencyObjectObservableForProperty();
            Assert.NotEqual(0, binder.GetAffinityForObject(typeof(DepObjFixture), "TestString"));
            Assert.Equal(0, binder.GetAffinityForObject(typeof(DepObjFixture), "DoesntExist"));

            var results = new List<IObservedChange<object, object>>();
            Expression<Func<DepObjFixture, object>> expression = x => x.TestString;
            var propertyName = expression.Body.GetMemberInfo().Name;
            var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).Subscribe(results.Add);
            var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).Subscribe(results.Add);

            fixture.TestString = "Foo";
            fixture.TestString = "Bar";

            Assert.Equal(4, results.Count);

            disp1.Dispose();
            disp2.Dispose();
        }

        [Fact]
        public void DerivedDependencyObjectObservableForPropertySmokeTest()
        {
            var fixture = new DerivedDepObjFixture();
            var binder = new DependencyObjectObservableForProperty();
            Assert.NotEqual(0, binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "TestString"));
            Assert.Equal(0, binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "DoesntExist"));

            var results = new List<IObservedChange<object, object>>();
            Expression<Func<DerivedDepObjFixture, object>> expression = x => x.TestString;
            var propertyName = expression.Body.GetMemberInfo().Name;
            var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).Subscribe(results.Add);
            var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).Subscribe(results.Add);

            fixture.TestString = "Foo";
            fixture.TestString = "Bar";

            Assert.Equal(4, results.Count);

            disp1.Dispose();
            disp2.Dispose();
        }

        [Fact]
        public void WhenAnyWithDependencyObjectTest()
        {
            var inputs = new[] { "Foo", "Bar", "Baz" };
            var fixture = new DepObjFixture();

            fixture.WhenAnyValue(x => x.TestString).ToObservableChangeSet().Bind(out var outputs).Subscribe();
            inputs.ForEach(x => fixture.TestString = x);

            Assert.Null(outputs.First());
            Assert.Equal(4, outputs.Count);
            Assert.True(inputs.Zip(outputs.Skip(1), (expected, actual) => expected == actual).All(x => x));
        }

        [Fact]
        public void ListBoxSelectedItemTest()
        {
            var input = new ListBox();
            input.Items.Add("Foo");
            input.Items.Add("Bar");
            input.Items.Add("Baz");

            input.WhenAnyValue(x => x.SelectedItem).ToObservableChangeSet().Bind(out var output).Subscribe();
            Assert.Equal(1, output.Count);

            input.SelectedIndex = 1;
            Assert.Equal(2, output.Count);

            input.SelectedIndex = 2;
            Assert.Equal(3, output.Count);
        }
    }
}
