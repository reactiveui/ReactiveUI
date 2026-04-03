// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;

namespace ReactiveUI.Builder.Tests.Mixins;

[TestExecutor<BuilderInstanceMixinsNullActionTests.NullActionTestExecutor>]
public class BuilderInstanceMixinsNullActionTests
{
    [Test]
    public async Task WithInstance_1_Type_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_2_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_3_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_4_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_5_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_6_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_7_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_8_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_9_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_10_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_11_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_12_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_13_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_14_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_15_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithInstance_16_Types_handles_null_action()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15, InstanceService16>(null!);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    internal sealed class NullActionTestExecutor : BuilderTestExecutorBase
    {
        protected override void ConfigureBuilder()
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            builder.WithRegistrationOnBuild(r =>
            {
                r.RegisterConstant(new InstanceService01(), typeof(InstanceService01));
                r.RegisterConstant(new InstanceService02(), typeof(InstanceService02));
                r.RegisterConstant(new InstanceService03(), typeof(InstanceService03));
                r.RegisterConstant(new InstanceService04(), typeof(InstanceService04));
                r.RegisterConstant(new InstanceService05(), typeof(InstanceService05));
                r.RegisterConstant(new InstanceService06(), typeof(InstanceService06));
                r.RegisterConstant(new InstanceService07(), typeof(InstanceService07));
                r.RegisterConstant(new InstanceService08(), typeof(InstanceService08));
                r.RegisterConstant(new InstanceService09(), typeof(InstanceService09));
                r.RegisterConstant(new InstanceService10(), typeof(InstanceService10));
                r.RegisterConstant(new InstanceService11(), typeof(InstanceService11));
                r.RegisterConstant(new InstanceService12(), typeof(InstanceService12));
                r.RegisterConstant(new InstanceService13(), typeof(InstanceService13));
                r.RegisterConstant(new InstanceService14(), typeof(InstanceService14));
                r.RegisterConstant(new InstanceService15(), typeof(InstanceService15));
                r.RegisterConstant(new InstanceService16(), typeof(InstanceService16));
            });
            builder.WithCoreServices().BuildApp();
        }
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
