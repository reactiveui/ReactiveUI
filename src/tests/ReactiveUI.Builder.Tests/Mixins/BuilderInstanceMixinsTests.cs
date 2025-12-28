// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

public class BuilderInstanceMixinsTests
{
    [Test]
    public async Task Builder_WithInstance_1_Type_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
                                  s1,
                                  typeof(InstanceService01));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        builder.WithInstance<InstanceService01>((s1) => { captured1 = s1; });

        await Assert.That(captured1).IsSameReferenceAs(s1);
    }

    [Test]
    public async Task Builder_WithInstance_1_Type_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01>((_) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_1_Type_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(
                                  s1,
                                  typeof(InstanceService01));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        builder.WithInstance<InstanceService01>((s1) => { captured1 = s1; });

        await Assert.That(captured1).IsSameReferenceAs(s1);
    }

    [Test]
    public async Task Extension_WithInstance_1_Type_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01>((_) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_2_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Builder_WithInstance_2_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02>((_, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_2_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Extension_WithInstance_2_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02>((_, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_3_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Builder_WithInstance_3_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_3_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Extension_WithInstance_3_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_4_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Builder_WithInstance_4_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) =>
            invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_4_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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

    [Test]
    public async Task Extension_WithInstance_4_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) =>
            invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_5_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04,
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

    [Test]
    public async Task Builder_WithInstance_5_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04,
                InstanceService05>((_, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_5_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04,
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

    [Test]
    public async Task Extension_WithInstance_5_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04,
                InstanceService05>((_, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_6_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
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

    [Test]
    public async Task Builder_WithInstance_6_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06>((_, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_6_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
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

    [Test]
    public async Task Extension_WithInstance_6_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06>((_, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_7_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07>((s1, s2, s3, s4, s5, s6, s7) =>
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

    [Test]
    public async Task Builder_WithInstance_7_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07>((_, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_7_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07>((s1, s2, s3, s4, s5, s6, s7) =>
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

    [Test]
    public async Task Extension_WithInstance_7_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07>((_, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_8_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08>((s1, s2, s3, s4, s5, s6, s7, s8) =>
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

    [Test]
    public async Task Builder_WithInstance_8_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08>((_, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_8_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08>((s1, s2, s3, s4, s5, s6, s7, s8) =>
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

    [Test]
    public async Task Extension_WithInstance_8_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08>((_, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_9_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09>((s1, s2, s3, s4, s5, s6, s7, s8, s9) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
    }

    [Test]
    public async Task Builder_WithInstance_9_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08,
                InstanceService09>((_, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_9_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09>((s1, s2, s3, s4, s5, s6, s7, s8, s9) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
    }

    [Test]
    public async Task Extension_WithInstance_9_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08,
                InstanceService09>((_, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_10_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09,
                InstanceService10>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
    }

    [Test]
    public async Task Builder_WithInstance_10_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09,
                InstanceService10>((_, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_10_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09,
                InstanceService10>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
    }

    [Test]
    public async Task Extension_WithInstance_10_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09,
                InstanceService10>((_, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_11_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
    }

    [Test]
    public async Task Builder_WithInstance_11_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11>((_, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_11_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
    }

    [Test]
    public async Task Extension_WithInstance_11_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11>((_, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_12_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
    }

    [Test]
    public async Task Builder_WithInstance_12_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12>((_, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_12_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
    }

    [Test]
    public async Task Extension_WithInstance_12_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12>((_, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_13_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
    }

    [Test]
    public async Task Builder_WithInstance_13_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13>((_, _, _, _, _, _, _, _, _, _, _, _, _) =>
                                                                             invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_13_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
    }

    [Test]
    public async Task Extension_WithInstance_13_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13>((_, _, _, _, _, _, _, _, _, _, _, _, _) =>
                                                                             invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_14_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
    }

    [Test]
    public async Task Builder_WithInstance_14_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_14_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
    }

    [Test]
    public async Task Extension_WithInstance_14_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_15_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(
                                  s15,
                                  typeof(InstanceService15));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        InstanceService15? captured15 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14,
                InstanceService15>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
                captured15 = s15;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
        await Assert.That(captured15).IsSameReferenceAs(s15);
    }

    [Test]
    public async Task Builder_WithInstance_15_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14,
                InstanceService15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_15_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(
                                  s15,
                                  typeof(InstanceService15));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        InstanceService15? captured15 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14,
                InstanceService15>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
                captured15 = s15;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
        await Assert.That(captured15).IsSameReferenceAs(s15);
    }

    [Test]
    public async Task Extension_WithInstance_15_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14,
                InstanceService15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Builder_WithInstance_16_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(
                                  s15,
                                  typeof(InstanceService15));
        var s16 = new InstanceService16();
        resolver.RegisterConstant(
                                  s16,
                                  typeof(InstanceService16));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        InstanceService15? captured15 = null;
        InstanceService16? captured16 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15,
                InstanceService16>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
                captured15 = s15;
                captured16 = s16;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
        await Assert.That(captured15).IsSameReferenceAs(s15);
        await Assert.That(captured16).IsSameReferenceAs(s16);
    }

    [Test]
    public async Task Builder_WithInstance_16_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15,
                InstanceService16>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    [Test]
    public async Task Extension_WithInstance_16_Types_invokes_action()
    {
        AppBuilder.ResetBuilderStateForTests();
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
        var s9 = new InstanceService09();
        resolver.RegisterConstant(
                                  s9,
                                  typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(
                                  s10,
                                  typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(
                                  s11,
                                  typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(
                                  s12,
                                  typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(
                                  s13,
                                  typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(
                                  s14,
                                  typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(
                                  s15,
                                  typeof(InstanceService15));
        var s16 = new InstanceService16();
        resolver.RegisterConstant(
                                  s16,
                                  typeof(InstanceService16));
        builder.WithCoreServices().Build();

        InstanceService01? captured1 = null;
        InstanceService02? captured2 = null;
        InstanceService03? captured3 = null;
        InstanceService04? captured4 = null;
        InstanceService05? captured5 = null;
        InstanceService06? captured6 = null;
        InstanceService07? captured7 = null;
        InstanceService08? captured8 = null;
        InstanceService09? captured9 = null;
        InstanceService10? captured10 = null;
        InstanceService11? captured11 = null;
        InstanceService12? captured12 = null;
        InstanceService13? captured13 = null;
        InstanceService14? captured14 = null;
        InstanceService15? captured15 = null;
        InstanceService16? captured16 = null;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15,
                InstanceService16>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
                captured7 = s7;
                captured8 = s8;
                captured9 = s9;
                captured10 = s10;
                captured11 = s11;
                captured12 = s12;
                captured13 = s13;
                captured14 = s14;
                captured15 = s15;
                captured16 = s16;
            });

        await Assert.That(captured1).IsSameReferenceAs(s1);
        await Assert.That(captured2).IsSameReferenceAs(s2);
        await Assert.That(captured3).IsSameReferenceAs(s3);
        await Assert.That(captured4).IsSameReferenceAs(s4);
        await Assert.That(captured5).IsSameReferenceAs(s5);
        await Assert.That(captured6).IsSameReferenceAs(s6);
        await Assert.That(captured7).IsSameReferenceAs(s7);
        await Assert.That(captured8).IsSameReferenceAs(s8);
        await Assert.That(captured9).IsSameReferenceAs(s9);
        await Assert.That(captured10).IsSameReferenceAs(s10);
        await Assert.That(captured11).IsSameReferenceAs(s11);
        await Assert.That(captured12).IsSameReferenceAs(s12);
        await Assert.That(captured13).IsSameReferenceAs(s13);
        await Assert.That(captured14).IsSameReferenceAs(s14);
        await Assert.That(captured15).IsSameReferenceAs(s15);
        await Assert.That(captured16).IsSameReferenceAs(s16);
    }

    [Test]
    public async Task Extension_WithInstance_16_Types_skips_when_null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(
                                            resolver,
                                            current: null);
        builder.WithCoreServices();

        var invoked = false;
        builder
            .WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05,
                InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10,
                InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15,
                InstanceService16>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => invoked = true);

        await Assert.That(invoked).IsFalse();
    }

    private sealed class InstanceService01;

    private sealed class InstanceService02;

    private sealed class InstanceService03;

    private sealed class InstanceService04;

    private sealed class InstanceService05;

    private sealed class InstanceService06;

    private sealed class InstanceService07;

    private sealed class InstanceService08;

    private sealed class InstanceService09;

    private sealed class InstanceService10;

    private sealed class InstanceService11;

    private sealed class InstanceService12;

    private sealed class InstanceService13;

    private sealed class InstanceService14;

    private sealed class InstanceService15;

    private sealed class InstanceService16;
}
