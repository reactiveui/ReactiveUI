// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>WithInstance tests for arities thirteen and fourteen.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Layout",
    "SST1523:Members should not be too long",
    Justification = "High-arity variadic WithInstance test methods are intrinsically long; each per-instance setup and assertion sits on its own line under one-statement-per-line formatting.")]
public partial class BuilderInstanceMixinsTests
{
    /// <summary>Verifies that the 13-types WithInstance builder instance method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_13_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices13(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9,
            out var s10,
            out var s11,
            out var s12,
            out var s13);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance13(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9,
            out var captured10,
            out var captured11,
            out var captured12,
            out var captured13);

        await AssertSameReferences13(
            captured1,
            s1,
            captured2,
            s2,
            captured3,
            s3,
            captured4,
            s4,
            captured5,
            s5,
            captured6,
            s6,
            captured7,
            s7,
            captured8,
            s8,
            captured9,
            s9,
            captured10,
            s10,
            captured11,
            s11,
            captured12,
            s12,
            captured13,
            s13);
    }

    /// <summary>Verifies that the 13-types WithInstance builder instance method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Builder_WithInstance_13_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        _ = builder.WithCoreServices();

        var invoked = false;
        _ = builder
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
                invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 13-types WithInstance extension method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Extension_WithInstance_13_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices13(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9,
            out var s10,
            out var s11,
            out var s12,
            out var s13);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance13(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9,
            out var captured10,
            out var captured11,
            out var captured12,
            out var captured13);

        await AssertSameReferences13(
            captured1,
            s1,
            captured2,
            s2,
            captured3,
            s3,
            captured4,
            s4,
            captured5,
            s5,
            captured6,
            s6,
            captured7,
            s7,
            captured8,
            s8,
            captured9,
            s9,
            captured10,
            s10,
            captured11,
            s11,
            captured12,
            s12,
            captured13,
            s13);
    }

    /// <summary>Verifies that the 13-types WithInstance extension method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Extension_WithInstance_13_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        _ = builder.WithCoreServices();

        var invoked = false;
        _ = builder
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
                invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 14-types WithInstance builder instance method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_14_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices14(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9,
            out var s10,
            out var s11,
            out var s12,
            out var s13,
            out var s14);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance14(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9,
            out var captured10,
            out var captured11,
            out var captured12,
            out var captured13,
            out var captured14);

        await AssertSameReferences14(
            captured1,
            s1,
            captured2,
            s2,
            captured3,
            s3,
            captured4,
            s4,
            captured5,
            s5,
            captured6,
            s6,
            captured7,
            s7,
            captured8,
            s8,
            captured9,
            s9,
            captured10,
            s10,
            captured11,
            s11,
            captured12,
            s12,
            captured13,
            s13,
            captured14,
            s14);
    }

    /// <summary>Verifies that the 14-types WithInstance builder instance method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Builder_WithInstance_14_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        _ = builder.WithCoreServices();

        var invoked = false;
        _ = builder
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
                InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 14-types WithInstance extension method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Extension_WithInstance_14_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices14(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9,
            out var s10,
            out var s11,
            out var s12,
            out var s13,
            out var s14);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance14(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9,
            out var captured10,
            out var captured11,
            out var captured12,
            out var captured13,
            out var captured14);

        await AssertSameReferences14(
            captured1,
            s1,
            captured2,
            s2,
            captured3,
            s3,
            captured4,
            s4,
            captured5,
            s5,
            captured6,
            s6,
            captured7,
            s7,
            captured8,
            s8,
            captured9,
            s9,
            captured10,
            s10,
            captured11,
            s11,
            captured12,
            s12,
            captured13,
            s13,
            captured14,
            s14);
    }

    /// <summary>Verifies that the 14-types WithInstance extension method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Extension_WithInstance_14_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        _ = builder.WithCoreServices();

        var invoked = false;
        _ = builder
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
                InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }
}
