// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="ObservableMixins" />.</summary>
public class ObservableMixinsTest
{
    /// <summary>Tests that WhereNotNull emits all non-null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_EmitsAllNonNullValues()
    {
        var subject = new Signal<int?>();
        var results = new List<int?>();

        const int SecondValue = 2;
        const int ThirdValue = 3;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnNext(ThirdValue);

        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo(1);
        await Assert.That(results[1]).IsEqualTo(SecondValue);
        await Assert.That(results[ThirdIndex]).IsEqualTo(ThirdValue);
    }

    /// <summary>Tests that WhereNotNull filters out null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_FiltersNullValues()
    {
        var subject = new Signal<string?>();
        var results = new List<string>();

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext("value1");
        subject.OnNext(null);
        subject.OnNext("value2");
        subject.OnNext(null);
        subject.OnNext("value3");

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;
        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo("value1");
        await Assert.That(results[1]).IsEqualTo("value2");
        await Assert.That(results[ThirdIndex]).IsEqualTo("value3");
    }

    /// <summary>Tests that WhereNotNull emits nothing when only nulls are sent.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_WithOnlyNulls_EmitsNothing()
    {
        var subject = new Signal<string?>();
        var results = new List<string>();

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(null);
        subject.OnNext(null);
        subject.OnNext(null);

        await Assert.That(results).Count().IsEqualTo(0);
    }

    /// <summary>Tests that WhereNotNull works with reference types.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_WorksWithReferenceTypes()
    {
        var subject = new Signal<TestClass?>();
        var results = new List<TestClass>();
        var obj1 = new TestClass { Value = "test1" };
        var obj2 = new TestClass { Value = "test2" };

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(obj1);
        subject.OnNext(null);
        subject.OnNext(obj2);

        const int ExpectedCount = 2;
        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo(obj1);
        await Assert.That(results[1]).IsEqualTo(obj2);
    }

    /// <summary>Test class for reference type testing.</summary>
    private sealed class TestClass
    {
        /// <summary>Gets or sets the test value.</summary>
        public string? Value { get; set; }
    }
}
