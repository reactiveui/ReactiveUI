// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>
/// Tests that the instance-based WithInstance overloads gracefully handle a null action.
/// </summary>
[TestExecutor<NullActionTestExecutor>]
public class BuilderInstanceMixinsNullActionTests
{
    /// <summary>
    /// Verifies that the 1-type WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_1_Type_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 2-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_2_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 3-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_3_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 4-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_4_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 5-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_5_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
                .WithInstance<
                    InstanceService01,
                    InstanceService02,
                    InstanceService03,
                    InstanceService04,
                    InstanceService05>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 6-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_6_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
                .WithInstance<
                    InstanceService01,
                    InstanceService02,
                    InstanceService03,
                    InstanceService04,
                    InstanceService05,
                    InstanceService06>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 7-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_7_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
                .WithInstance<
                    InstanceService01,
                    InstanceService02,
                    InstanceService03,
                    InstanceService04,
                    InstanceService05,
                    InstanceService06,
                    InstanceService07>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 8-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_8_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
                .WithInstance<
                    InstanceService01,
                    InstanceService02,
                    InstanceService03,
                    InstanceService04,
                    InstanceService05,
                    InstanceService06,
                    InstanceService07,
                    InstanceService08>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 9-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_9_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
                .WithInstance<
                    InstanceService01,
                    InstanceService02,
                    InstanceService03,
                    InstanceService04,
                    InstanceService05,
                    InstanceService06,
                    InstanceService07,
                    InstanceService08,
                    InstanceService09>(
                    null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 10-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_10_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result =
            builder
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
                    InstanceService10>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 11-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_11_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService11>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 12-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_12_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService12>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 13-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_13_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService13>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 14-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_14_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService14>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 15-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_15_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService15>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Verifies that the 16-types WithInstance overload returns the same builder when given a null action.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_16_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder
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
                InstanceService16>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Executor that registers all instance services then builds the app for the null-action tests.
    /// </summary>
    internal sealed class NullActionTestExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            builder.WithRegistrationOnBuild(r =>
            {
                r.RegisterConstant(new InstanceService01());
                r.RegisterConstant(new InstanceService02());
                r.RegisterConstant(new InstanceService03());
                r.RegisterConstant(new InstanceService04());
                r.RegisterConstant(new InstanceService05());
                r.RegisterConstant(new InstanceService06());
                r.RegisterConstant(new InstanceService07());
                r.RegisterConstant(new InstanceService08());
                r.RegisterConstant(new InstanceService09());
                r.RegisterConstant(new InstanceService10());
                r.RegisterConstant(new InstanceService11());
                r.RegisterConstant(new InstanceService12());
                r.RegisterConstant(new InstanceService13());
                r.RegisterConstant(new InstanceService14());
                r.RegisterConstant(new InstanceService15());
                r.RegisterConstant(new InstanceService16());
            });
            builder.WithCoreServices().BuildApp();
        }
    }

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService01;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService02;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService03;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService04;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService05;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService06;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService07;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService08;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService09;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService10;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService11;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService12;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService13;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService14;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService15;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService16;
}
