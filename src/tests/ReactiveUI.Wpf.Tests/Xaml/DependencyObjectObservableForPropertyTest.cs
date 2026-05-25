// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Windows.Controls;

using DynamicData;

using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for the dependency object property binding.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class DependencyObjectObservableForPropertyTest
{
    private const int ExpectedCountTwo = 2;
    private const int ExpectedCountThree = 3;
    private const int ExpectedCountFour = 4;
    private const int ExpectedAffinity = 4;
    private const int SecondSelectedIndex = 2;
    private const string TestStringPropertyName = "TestString";
    private const string FooValue = "Foo";
    private const string BarValue = "Bar";
    private const string TestValue = "Test";

    /// <summary>
    /// Runs a smoke test for dependency object observables for property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(binder.GetAffinityForObject(typeof(DepObjFixture), TestStringPropertyName)).IsNotEqualTo(0);
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

        fixture.TestString = FooValue;
        fixture.TestString = BarValue;

        await Assert.That(results).Count().IsEqualTo(ExpectedCountFour);

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Runs a smoke test for derived dependency object observables for property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DerivedDependencyObjectObservableForPropertySmokeTest()
    {
        var fixture = new DerivedDepObjFixture();
        var binder = new DependencyObjectObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(binder.GetAffinityForObject(typeof(DerivedDepObjFixture), TestStringPropertyName)).IsNotEqualTo(0);
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

        fixture.TestString = FooValue;
        fixture.TestString = BarValue;

        await Assert.That(results).Count().IsEqualTo(ExpectedCountFour);

        disp1.Dispose();
        disp2.Dispose();
    }

    /// <summary>
    /// Tests WhenAny with dependency object test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyWithDependencyObjectTest()
    {
        var inputs = new[] { FooValue, BarValue, "Baz" };
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
            await Assert.That(outputs[0]).IsNull();
            await Assert.That(outputs).Count().IsEqualTo(ExpectedCountFour);
        }

        await Assert.That(outputs.Skip(1)).IsEquivalentTo(inputs);
    }

    /// <summary>
    /// Tests ListBoxes the selected item test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ListBoxSelectedItemTest()
    {
        var input = new ListBox();
        input.Items.Add(FooValue);
        input.Items.Add(BarValue);
        input.Items.Add("Baz");

        input.WhenAnyValue(static x => x.SelectedItem)
             .ToObservableChangeSet()
             .Bind(out var output)
             .Subscribe();

        await Assert.That(output).Count().IsEqualTo(1);

        input.SelectedIndex = 1;
        await Assert.That(output).Count().IsEqualTo(ExpectedCountTwo);

        input.SelectedIndex = SecondSelectedIndex;
        await Assert.That(output).Count().IsEqualTo(ExpectedCountThree);
    }

    /// <summary>
    /// Tests GetAffinityForObject returns 0 for non-DependencyObject types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetAffinityForObject_NonDependencyObject_ReturnsZero()
    {
        var binder = new DependencyObjectObservableForProperty();

        var affinity = binder.GetAffinityForObject(typeof(string), "Length");

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests GetAffinityForObject returns 4 for valid DependencyProperty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetAffinityForObject_ValidDependencyProperty_ReturnsFour()
    {
        var binder = new DependencyObjectObservableForProperty();

        var affinity = binder.GetAffinityForObject(typeof(DepObjFixture), TestStringPropertyName);

        await Assert.That(affinity).IsEqualTo(ExpectedAffinity);
    }

    /// <summary>
    /// Tests GetAffinityForObject returns 0 for non-existent property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetAffinityForObject_NonExistentProperty_ReturnsZero()
    {
        var binder = new DependencyObjectObservableForProperty();

        var affinity = binder.GetAffinityForObject(typeof(DepObjFixture), "NonExistentProperty");

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests GetNotificationForProperty throws for null sender.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_NullSender_Throws()
    {
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;

        await Assert.That(() => binder.GetNotificationForProperty(null!, expression.Body, TestStringPropertyName))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests GetNotificationForProperty throws for non-dependency property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_NonDependencyProperty_Throws()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;

        await Assert.That(() => binder.GetNotificationForProperty(fixture, expression.Body, "NonExistentProperty"))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests GetNotificationForProperty with suppressWarnings parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_WithSuppressWarnings_DoesNotLog()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;

        var results = new List<IObservedChange<object, object?>>();
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, TestStringPropertyName, beforeChanged: false, suppressWarnings: true)
                         .Subscribe(results.Add);

        fixture.TestString = TestValue;

        await Assert.That(results).Count().IsEqualTo(1);

        disp.Dispose();
    }

    /// <summary>
    /// Tests GetNotificationForProperty notifies on dependency property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_DependencyPropertyChange_Notifies()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        var results = new List<IObservedChange<object, object?>>();
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                         .Subscribe(results.Add);

        await Assert.That(results).IsEmpty();

        fixture.TestString = "First";
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].Sender).IsEqualTo(fixture);

        fixture.TestString = "Second";
        await Assert.That(results).Count().IsEqualTo(ExpectedCountTwo);

        disp.Dispose();
    }

    /// <summary>
    /// Tests disposal stops notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_Disposal_StopsNotifications()
    {
        var fixture = new DepObjFixture();

        // Detach the value-changed handler synchronously on dispose so the assertions below observe the unsubscribe
        // immediately. In production the handler is removed on the dispatcher (RxSchedulers.MainThreadScheduler);
        // here there is no running message pump, so an immediate scheduler keeps the test deterministic.
        var binder = new DependencyObjectObservableForProperty { Scheduler = ImmediateScheduler.Instance };
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        var results = new List<IObservedChange<object, object?>>();
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                         .Subscribe(results.Add);

        fixture.TestString = "First";
        await Assert.That(results).Count().IsEqualTo(1);

        disp.Dispose();

        fixture.TestString = "Second";
        await Assert.That(results).Count().IsEqualTo(1); // Should not increase

        fixture.TestString = "Third";
        await Assert.That(results).Count().IsEqualTo(1); // Should still not increase
    }

    /// <summary>
    /// Tests multiple subscribers receive notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_MultipleSubscribers_AllReceiveNotifications()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        var results1 = new List<IObservedChange<object, object?>>();
        var results2 = new List<IObservedChange<object, object?>>();
        var results3 = new List<IObservedChange<object, object?>>();

        var observable = binder.GetNotificationForProperty(fixture, expression.Body, propertyName);
        var disp1 = observable.Subscribe(results1.Add);
        var disp2 = observable.Subscribe(results2.Add);
        var disp3 = observable.Subscribe(results3.Add);

        fixture.TestString = "Value1";
        fixture.TestString = "Value2";

        using (Assert.Multiple())
        {
            await Assert.That(results1).Count().IsEqualTo(ExpectedCountTwo);
            await Assert.That(results2).Count().IsEqualTo(ExpectedCountTwo);
            await Assert.That(results3).Count().IsEqualTo(ExpectedCountTwo);
        }

        disp1.Dispose();
        disp2.Dispose();
        disp3.Dispose();
    }

    /// <summary>
    /// Tests beforeChanged parameter has no effect (not supported for DependencyProperty).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_BeforeChanged_NotSupported()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        var results = new List<IObservedChange<object, object?>>();
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, propertyName, beforeChanged: true)
                         .Subscribe(results.Add);

        fixture.TestString = TestValue;

        // beforeChanged is not supported for DependencyProperty, but should still get notifications
        await Assert.That(results).Count().IsEqualTo(1);

        disp.Dispose();
    }

    /// <summary>
    /// Tests GetAffinityForObject with beforeChanged parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetAffinityForObject_WithBeforeChanged_ReturnsCorrectAffinity()
    {
        var binder = new DependencyObjectObservableForProperty();

        var affinity = binder.GetAffinityForObject(typeof(DepObjFixture), TestStringPropertyName, beforeChanged: true);

        await Assert.That(affinity).IsEqualTo(ExpectedAffinity);
    }

    /// <summary>
    /// Tests GetNotificationForProperty observable expression is passed through.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_ObservableExpression_PassedThrough()
    {
        var fixture = new DepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        IObservedChange<object, object?>? result = null;
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                         .Subscribe(x => result = x);

        fixture.TestString = TestValue;

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Expression).IsEqualTo(expression.Body);

        disp.Dispose();
    }

    /// <summary>
    /// Tests with derived dependency object properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task GetNotificationForProperty_DerivedDependencyObject_Works()
    {
        var fixture = new DerivedDepObjFixture();
        var binder = new DependencyObjectObservableForProperty();
        Expression<Func<DerivedDepObjFixture, object?>> expression = static x => x.TestString;
        var propertyName = expression.Body.GetMemberInfo()?.Name!;

        var results = new List<IObservedChange<object, object?>>();
        var disp = binder.GetNotificationForProperty(fixture, expression.Body, propertyName)
                         .Subscribe(results.Add);

        fixture.TestString = "DerivedTest";

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].Sender).IsEqualTo(fixture);

        disp.Dispose();
    }
}
