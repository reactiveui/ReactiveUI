// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

public class BuilderInstanceMixinsNullInstanceTests
{
    [Test]
    public async Task WithInstance_1_Type_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01>((_) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_2_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02>((_, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_3_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03>((_, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_4_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04>((_, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_5_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05>((_, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_6_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06>((_, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_7_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07>((_, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_8_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08>((_, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_9_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09>((_, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_10_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10>((_, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_11_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11>((_, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_12_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12>((_, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_13_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13>((_, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_14_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_15_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WithInstance_16_Types_throws_when_instance_null()
    {
        IReactiveUIInstance nullInstance = null!;

        await Assert.That(() => nullInstance.WithInstance<InstanceService01, InstanceService02, InstanceService03, InstanceService04, InstanceService05, InstanceService06, InstanceService07, InstanceService08, InstanceService09, InstanceService10, InstanceService11, InstanceService12, InstanceService13, InstanceService14, InstanceService15, InstanceService16>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => { }))
            .Throws<ArgumentNullException>();
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
