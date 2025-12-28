// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using DynamicData;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for the dependency object property binding.
/// </summary>
public class DependencyObjectObservableForPropertyTest
{
    /// <summary>
    /// Runs a smoke test for dependency object observables for property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task DependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture), "TestString")).IsNotEqualTo(0);
            await Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture), "DoesntExist")).IsEqualTo(0);
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

        await Assert.That(results).Count().IsEqualTo(4);

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Runs a smoke test for derived dependency object observables for property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task DerivedDependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DerivedDepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "TestString")).IsNotEqualTo(0);
            await Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture), "DoesntExist")).IsEqualTo(0);
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

        await Assert.That(results).Count().IsEqualTo(4);

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Tests WhenAny with dependency object test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task WhenAnyWithDependencyObjectTest()
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

        using (Assert.Multiple())
        {
            await Assert.That(outputs.First()).IsNull();
            await Assert.That(outputs).Count().IsEqualTo(4);
        }

        await Assert.That(outputs.Skip(1)).IsEquivalentTo(inputs);
    }

    /// <summary>
    /// Tests ListBoxes the selected item test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ListBoxSelectedItemTest()
    {
        var input = new ListBox();
        input.Items.Add("Foo");
        input.Items.Add("Bar");
        input.Items.Add("Baz");

        input.WhenAnyValue(static x => x.SelectedItem)
             .ToObservableChangeSet()
             .Bind(out var output)
             .Subscribe();

        await Assert.That(output).Count().IsEqualTo(1);

        input.SelectedIndex = 1;
        await Assert.That(output).Count().IsEqualTo(2);

        input.SelectedIndex = 2;
        await Assert.That(output).Count().IsEqualTo(3);
    }
}
