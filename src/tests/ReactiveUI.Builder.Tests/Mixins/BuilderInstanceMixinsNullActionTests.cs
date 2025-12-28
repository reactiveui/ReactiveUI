// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

public class BuilderInstanceMixinsNullActionTests
{
    [Test]
    public async Task WithInstance_1_Type_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_2_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_3_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_4_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_5_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_6_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_7_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_8_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_9_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_10_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_11_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_12_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_13_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_14_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_15_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(s15, typeof(InstanceService15));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_16_Types_handles_null_action()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        var s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        var s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        var s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        var s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        var s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        var s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        var s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        var s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        var s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        var s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        var s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        var s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        var s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
        var s15 = new InstanceService15();
        resolver.RegisterConstant(s15, typeof(InstanceService15));
        var s16 = new InstanceService16();
        resolver.RegisterConstant(s16, typeof(InstanceService16));
        builder.WithCoreServices().Build();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15, InstanceService16>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
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
