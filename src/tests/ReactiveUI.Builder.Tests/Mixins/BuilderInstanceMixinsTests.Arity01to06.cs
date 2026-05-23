// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <content>
/// WithInstance tests for arities one through six.
/// </content>
public partial class BuilderInstanceMixinsTests
{
    /// <summary>
    /// Verifies that the 1-type WithInstance builder instance method invokes the action with the resolved instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_1_Type_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        builder.WithInstance<InstanceService01>(s1 => captured1 = s1);

        await Assert.That(captured1).IsSameReferenceAs(s1);
    }

    /// <summary>
    /// Verifies that the 1-type WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_1_Type_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01>(_ => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 1-type WithInstance extension method invokes the action with the resolved instance.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_1_Type_invokes_action()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
            s1,
            typeof(InstanceService01));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        builder.WithInstance<InstanceService01>(s1 => captured1 = s1);

        await Assert.That(captured1).IsSameReferenceAs(s1);
    }

    /// <summary>
    /// Verifies that the 1-type WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_1_Type_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01>(_ => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 2-types WithInstance builder instance method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_2_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        builder.WithInstance<InstanceService01, InstanceService02>((s1, s2) =>
        {
            captured1 = s1;
            captured2 = s2;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
    }

    /// <summary>
    /// Verifies that the 2-types WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_2_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02>((_, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 2-types WithInstance extension method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_2_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        builder.WithInstance<InstanceService01, InstanceService02>((s1, s2) =>
        {
            captured1 = s1;
            captured2 = s2;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
    }

    /// <summary>
    /// Verifies that the 2-types WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_2_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02>((_, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 3-types WithInstance builder instance method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_3_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((s1, s2, s3) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
    }

    /// <summary>
    /// Verifies that the 3-types WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_3_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 3-types WithInstance extension method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_3_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((s1, s2, s3) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
    }

    /// <summary>
    /// Verifies that the 3-types WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_3_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 4-types WithInstance builder instance method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_4_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((s1, s2, s3, s4) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
            captured4 = s4;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
    }

    /// <summary>
    /// Verifies that the 4-types WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_4_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) =>
            invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 4-types WithInstance extension method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_4_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((s1, s2, s3, s4) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
            captured4 = s4;
        });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
    }

    /// <summary>
    /// Verifies that the 4-types WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_4_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) =>
            invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 5-types WithInstance builder instance method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_5_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05>((s1, s2, s3, s4, s5) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
    }

    /// <summary>
    /// Verifies that the 5-types WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_5_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05>((_, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 5-types WithInstance extension method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_5_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05>((s1, s2, s3, s4, s5) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
    }

    /// <summary>
    /// Verifies that the 5-types WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_5_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05>((_, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 6-types WithInstance builder instance method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_6_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06>((s1, s2, s3, s4, s5, s6) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
    }

    /// <summary>
    /// Verifies that the 6-types WithInstance builder instance method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Builder_WithInstance_6_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06>((_, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    /// <summary>
    /// Verifies that the 6-types WithInstance extension method invokes the action with the resolved instances.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_6_Types_invokes_action()
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06>((s1, s2, s3, s4, s5, s6) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
    }

    /// <summary>
    /// Verifies that the 6-types WithInstance extension method skips the action when the current resolver is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Extension_WithInstance_6_Types_skips_when_null()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
            resolver,
            null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<
                InstanceService01,
                InstanceService02,
                InstanceService03,
                InstanceService04,
                InstanceService05,
                InstanceService06>((_, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }
}
