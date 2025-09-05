// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Threading;
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for the dependency object property binding.
/// </summary>
public class DependencyObjectObservableForPropertyTest
{
    /// <summary>
    /// Runs a smoke test for dependency object observables for property.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void DependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture, Is.Not.EqualTo(0)), "TestString"));
        Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture, Is.EqualTo(0)), "DoesntExist"));

        var results = new List<IObservedChange<object, object?>>();
        Expression<Func<DepObjFixture, object>> expression = x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("There is no valid property name");
        var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).WhereNotNull().Subscribe(results.Add);
        var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).WhereNotNull().Subscribe(results.Add);

        fixture.TestString = "Foo";
        fixture.TestString = "Bar";

        Assert.That(results.Count, Is.EqualTo(4));

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Runs a smoke test for derived dependency object observables for property.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void DerivedDependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DerivedDepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture, Is.Not.EqualTo(0)), "TestString"));
        Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture, Is.EqualTo(0)), "DoesntExist"));

        var results = new List<IObservedChange<object, object?>>();
        Expression<Func<DerivedDepObjFixture, object>> expression = x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("There is no valid property name");
        var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).WhereNotNull().Subscribe(results.Add);
        var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName).WhereNotNull().Subscribe(results.Add);

        fixture.TestString = "Foo";
        fixture.TestString = "Bar";

        Assert.That(results.Count, Is.EqualTo(4));

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Tests WhenAny with dependency object test.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void WhenAnyWithDependencyObjectTest()
    {
        var inputs = new[] { "Foo", "Bar", "Baz" };
        var fixture = new DepObjFixture();

        fixture.WhenAnyValue(x => x.TestString).ToObservableChangeSet().Bind(out var outputs).Subscribe();
        inputs.ForEach(x => fixture.TestString = x);

        Assert.That(outputs.First(, Is.Null));
        Assert.That(outputs.Count, Is.EqualTo(4));
        Assert.That(inputs.Zip(outputs.Skip(1, Is.True), (expected, actual) => expected == actual).All(x => x));
    }

    /// <summary>
    /// Tests ListBoxes the selected item test.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void ListBoxSelectedItemTest()
    {
        var input = new ListBox();
        input.Items.Add("Foo");
        input.Items.Add("Bar");
        input.Items.Add("Baz");

        input.WhenAnyValue(x => x.SelectedItem).ToObservableChangeSet().Bind(out var output).Subscribe();
        Assert.That(output.Count, Is.EqualTo(1));

        input.SelectedIndex = 1;
        Assert.That(output.Count, Is.EqualTo(2));

        input.SelectedIndex = 2;
        Assert.That(output.Count, Is.EqualTo(3));
    }
}
