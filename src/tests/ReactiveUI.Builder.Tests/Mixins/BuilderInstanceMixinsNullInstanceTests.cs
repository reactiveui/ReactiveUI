// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Tests that the instance-based WithInstance overloads throw when invoked on a null instance.</summary>
public class BuilderInstanceMixinsNullInstanceTests
{
    /// <summary>Verifies that the 1-type WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_1_Type_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01>(_ => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 2-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_2_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02>((_, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 3-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_3_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 4-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_4_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) =>
                {
                }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 5-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_5_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05>((_, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 6-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_6_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06>((_, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 7-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_7_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07>((_, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 8-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_8_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08>((_, _, _, _, _, _, _, _) =>
                    {
                    }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 9-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_9_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09>((_, _, _, _, _, _, _, _, _) =>
                    {
                    }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 10-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_10_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10>((_, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 11-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_11_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11>((_, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 12-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_12_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11,
                        InstanceService12>((_, _, _, _, _, _, _, _, _, _, _, _) =>
                    {
                    }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 13-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_13_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11,
                        InstanceService12,
                        InstanceService13>((_, _, _, _, _, _, _, _, _, _, _, _, _) =>
                    {
                    }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 14-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_14_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11,
                        InstanceService12,
                        InstanceService13,
                        InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) =>
                    {
                    }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 15-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_15_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11,
                        InstanceService12,
                        InstanceService13,
                        InstanceService14,
                        InstanceService15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the 16-types WithInstance overload throws when invoked on a null instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_16_Types_throws_when_instance_null()
    {
        const IReactiveUIInstance nullInstance = null!;

        await Assert.That(() =>
                nullInstance
                    .WithInstance<
                        InstanceService01,
                        InstanceService02,
                        InstanceService03,
                        InstanceService04,
                        InstanceService05,
                        InstanceService06,
                        InstanceService07,
                        InstanceService08,
                        InstanceService09,
                        InstanceService10,
                        InstanceService11,
                        InstanceService12,
                        InstanceService13,
                        InstanceService14,
                        InstanceService15,
                        InstanceService16>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService01;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService02;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService03;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService04;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService05;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService06;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService07;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService08;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService09;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService10;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService11;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService12;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService13;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService14;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService15;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService16;
}
