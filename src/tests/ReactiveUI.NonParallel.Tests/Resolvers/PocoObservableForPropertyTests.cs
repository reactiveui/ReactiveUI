// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class PocoObservableForPropertyTests : IDisposable
{
    private RxAppSchedulersScope? _schedulersScope;

    [Before(HookType.Test)]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [After(HookType.Test)]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }

    [Test]
    public async Task CheckGetAffinityForObjectValues()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(instance.GetAffinityForObject(
                            typeof(PocoType),
                            null!,
                            false)).IsEqualTo(1);
            await Assert.That(instance.GetAffinityForObject(
                            typeof(INPCClass),
                            null!,
                            false)).IsEqualTo(1);
        }
    }

    [Test]
    public async Task GetNotificationForPropertyReturnsObservable()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), false, true);

        await Assert.That(observable).IsNotNull();
    }

    [Test]
    public async Task GetNotificationForPropertyReturnsSingleValue()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), false, true);
        var result = await observable.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Sender).IsEqualTo(poco);
        }
    }

    [Test]
    public async Task GetNotificationForPropertyWithBeforeChangedParameter()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), true, true);
        var result = await observable.FirstAsync();

        await Assert.That(result).IsNotNull();
    }

    [Test]
    public void GetNotificationForPropertyThrowsOnNullSender()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        Assert.Throws<ArgumentNullException>(() =>
            instance.GetNotificationForProperty(null!, expr.Body, nameof(PocoType.Property1), false, true));
    }

    [Test]
    public async Task GetNotificationForPropertyOnlyWarnsOnce()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco1 = new PocoType { Property1 = "Test1" };
        var poco2 = new PocoType { Property1 = "Test2" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        // First call should trigger warning (but we're suppressing it with suppressWarnings: false for testing)
        var observable1 = instance.GetNotificationForProperty(poco1, expr.Body, nameof(PocoType.Property1), false, false);
        var result1 = await observable1.FirstAsync();

        // Second call with different instance but same type and property should not warn again
        var observable2 = instance.GetNotificationForProperty(poco2, expr.Body, nameof(PocoType.Property1), false, false);
        var result2 = await observable2.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result1).IsNotNull();
            await Assert.That(result2).IsNotNull();
        }
    }

    [Test]
    public async Task GetNotificationForPropertyWithDifferentProperties()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test1", Property2 = "Test2" };
        Expression<Func<PocoType, string?>> expr1 = x => x.Property1;
        Expression<Func<PocoType, string?>> expr2 = x => x.Property2;

        var observable1 = instance.GetNotificationForProperty(poco, expr1.Body, nameof(PocoType.Property1), false, true);
        var observable2 = instance.GetNotificationForProperty(poco, expr2.Body, nameof(PocoType.Property2), false, true);

        var result1 = await observable1.FirstAsync();
        var result2 = await observable2.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result1).IsNotNull();
            await Assert.That(result2).IsNotNull();
            await Assert.That(result1.Sender).IsEqualTo(poco);
            await Assert.That(result2.Sender).IsEqualTo(poco);
        }
    }

    [Test]
    public async Task GetNotificationForPropertyNeverCompletes()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), false, true);

        // Take 2 items - should only get 1 since POCO doesn't change
        var results = new List<IObservedChange<object, object?>>();
        var completed = false;

        observable
            .Take(TimeSpan.FromMilliseconds(100))
            .Subscribe(
                results.Add,
                onCompleted: () => completed = true);

        await Task.Delay(150);

        // Should have received exactly 1 item (the initial value) and completed
        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(completed).IsTrue();
        }
    }

    public void Dispose()
    {
        _schedulersScope?.Dispose();
        _schedulersScope = null;
    }

    [SuppressMessage("Style", "CA1812: Avoid uninstantiated internal classes", Justification = "Used in tests via reflection.")]
    private class PocoType
    {
        public string? Property1 { get; set; }

        public string? Property2 { get; set; }
    }

    [SuppressMessage("Style", "CA1812: Avoid uninstantiated internal classes", Justification = "Used in tests via reflection.")]
    private class INPCClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged() => PropertyChanged?.Invoke(
                                                                       this,
                                                                       new PropertyChangedEventArgs(string.Empty));
    }
}
