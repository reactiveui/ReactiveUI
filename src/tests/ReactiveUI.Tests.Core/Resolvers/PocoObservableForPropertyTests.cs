// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests.Core;
[NonParallelizable]
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
    public void CheckGetAffinityForObjectValues()
    {
        RxApp.EnsureInitialized();
        var instance = new POCOObservableForProperty();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        instance.GetAffinityForObject(
                            typeof(PocoType),
                            null!,
                            false),
                        Is.EqualTo(1));
            Assert.That(
                        instance.GetAffinityForObject(
                            typeof(INPCClass),
                            null!,
                            false),
                        Is.EqualTo(1));
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