// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>WithInstance tests for arities seven through nine.</summary>
public partial class BuilderInstanceMixinsTests
{
    /// <summary>Verifies that the 7-types WithInstance builder instance method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_7_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(
            s2,
            typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(
            s3,
            typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(
            s4,
            typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(
            s5,
            typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(
            s6,
            typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(
            s7,
            typeof(InstanceService07));
        _ = builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        _ = builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06,
                InstanceService07>((s1, s2, s3, s4, s5, s6, s7) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
    }

    /// <summary>Verifies that the 7-types WithInstance builder instance method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_7_Types_skips_when_null()
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
                InstanceService07>((_, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 7-types WithInstance extension method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_7_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(
            s2,
            typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(
            s3,
            typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(
            s4,
            typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(
            s5,
            typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(
            s6,
            typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(
            s7,
            typeof(InstanceService07));
        _ = builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        _ = builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06,
                InstanceService07>((s1, s2, s3, s4, s5, s6, s7) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
    }

    /// <summary>Verifies that the 7-types WithInstance extension method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_7_Types_skips_when_null()
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
                InstanceService07>((_, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 8-types WithInstance builder instance method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Builder_WithInstance_8_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(
            s2,
            typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(
            s3,
            typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(
            s4,
            typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(
            s5,
            typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(
            s6,
            typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(
            s7,
            typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(
            s8,
            typeof(InstanceService08));
        _ = builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        _ = builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06,
                InstanceService07,
                InstanceService08>((s1, s2, s3, s4, s5, s6, s7, s8) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
    }

    /// <summary>Verifies that the 8-types WithInstance builder instance method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Builder_WithInstance_8_Types_skips_when_null()
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
                InstanceService08>((_, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 8-types WithInstance extension method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Extension_WithInstance_8_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(
            s2,
            typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(
            s3,
            typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(
            s4,
            typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(
            s5,
            typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(
            s6,
            typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(
            s7,
            typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(
            s8,
            typeof(InstanceService08));
        _ = builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        _ = builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06,
                InstanceService07,
                InstanceService08>((s1, s2, s3, s4, s5, s6, s7, s8) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
    }

    /// <summary>Verifies that the 8-types WithInstance extension method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Extension_WithInstance_8_Types_skips_when_null()
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
                InstanceService08>((_, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 9-types WithInstance builder instance method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_9_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices09(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance09(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9);

        await AssertSameReferences09(
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
            s9);
    }

    /// <summary>Verifies that the 9-types WithInstance builder instance method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Builder_WithInstance_9_Types_skips_when_null()
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
                InstanceService09>((_, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>Verifies that the 9-types WithInstance extension method invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_9_Types_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        RegisterServices09(
            resolver,
            out var s1,
            out var s2,
            out var s3,
            out var s4,
            out var s5,
            out var s6,
            out var s7,
            out var s8,
            out var s9);
        _ = builder.WithCoreServices().Build();

        InvokeWithInstance09(
            builder,
            out var captured1,
            out var captured2,
            out var captured3,
            out var captured4,
            out var captured5,
            out var captured6,
            out var captured7,
            out var captured8,
            out var captured9);

        await AssertSameReferences09(
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
            s9);
    }

    /// <summary>Verifies that the 9-types WithInstance extension method skips the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task Extension_WithInstance_9_Types_skips_when_null()
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
                InstanceService09>((_, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }
}
