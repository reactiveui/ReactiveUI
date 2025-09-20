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
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for the dependency object property binding.
/// </summary>
[TestFixture]
[Apartment(ApartmentState.STA)]
public class DependencyObjectObservableForPropertyTest
{
    /// <summary>
    /// Runs a smoke test for dependency object observables for property.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void DependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture), "TestString"), Is.Not.Zero);
            Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture), "DoesntExist"), Is.Zero);
        }

        var results = new List<IObservedChange<object, object?>>();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name
                           ?? throw new InvalidOperationException("There is no valid property name");

        var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                          .WhereNotNull()
                          .Subscribe(results.Add);
        var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                          .WhereNotNull()
                          .Subscribe(results.Add);

        fixture.TestString = "Foo";
        fixture.TestString = "Bar";

        Assert.That(results, Has.Count.EqualTo(4));

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Runs a smoke test for derived dependency object observables for property.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void DerivedDependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DerivedDepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "TestString"), Is.Not.Zero);
            Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "DoesntExist"), Is.Zero);
        }

        var results = new List<IObservedChange<object, object?>>();
        Expression<Func<DerivedDepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name
                           ?? throw new InvalidOperationException("There is no valid property name");

        var disp1 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                          .WhereNotNull()
                          .Subscribe(results.Add);
        var disp2 = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                          .WhereNotNull()
                          .Subscribe(results.Add);

        fixture.TestString = "Foo";
        fixture.TestString = "Bar";

        Assert.That(results, Has.Count.EqualTo(4));

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Tests WhenAny with dependency object test.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void WhenAnyWithDependencyObjectTest()
    {
        var inputs = new[] { "Foo", "Bar", "Baz" };
        var fixture = new DepObjFixture();

        fixture.WhenAnyValue(static x => x.TestString)
               .ToObservableChangeSet()
               .Bind(out var outputs)
               .Subscribe();

        foreach (var x in inputs)
        {
            fixture.TestString = x;
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(outputs.First(), Is.Null);
            Assert.That(outputs, Has.Count.EqualTo(4));
        }

        Assert.That(outputs.Skip(1), Is.EqualTo(inputs));
    }

    /// <summary>
    /// Tests ListBoxes the selected item test.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ListBoxSelectedItemTest()
    {
        var input = new ListBox();
        input.Items.Add("Foo");
        input.Items.Add("Bar");
        input.Items.Add("Baz");

        input.WhenAnyValue(static x => x.SelectedItem)
             .ToObservableChangeSet()
             .Bind(out var output)
             .Subscribe();

        Assert.That(output, Has.Count.EqualTo(1));

        input.SelectedIndex = 1;
        Assert.That(output, Has.Count.EqualTo(2));

        input.SelectedIndex = 2;
        Assert.That(output, Has.Count.EqualTo(3));
    }
}
