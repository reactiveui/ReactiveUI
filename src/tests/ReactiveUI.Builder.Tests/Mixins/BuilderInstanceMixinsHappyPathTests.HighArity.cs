// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Tests the happy-path behaviour of the instance-based WithInstance overloads for the higher-arity (9-16 type) overloads.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Layout",
    "SST1523:Members should not be too long",
    Justification = "High-arity variadic WithInstance test methods are intrinsically long; each per-instance setup and assertion sits on its own line under one-statement-per-line formatting.")]
public partial class BuilderInstanceMixinsHappyPathTests
{
    /// <summary>Verifies that the 9-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_9_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var service6 = new Service6();
        var service7 = new Service7();
        var service8 = new Service8();
        var service9 = new Service9();
        var mockInstance = new MockReactiveUIInstance(
            service1,
            service2,
            service3,
            service4,
            service5,
            service6,
            service7,
            service8,
            service9);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;
        Service8? captured8 = null;
        Service9? captured9 = null;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9>((s1, s2, s3, s4, s5, s6, s7, s8, s9) =>
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

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
            await Assert.That(captured3).IsSameReferenceAs(service3);
            await Assert.That(captured4).IsSameReferenceAs(service4);
            await Assert.That(captured5).IsSameReferenceAs(service5);
            await Assert.That(captured6).IsSameReferenceAs(service6);
            await Assert.That(captured7).IsSameReferenceAs(service7);
            await Assert.That(captured8).IsSameReferenceAs(service8);
            await Assert.That(captured9).IsSameReferenceAs(service9);
        }
    }

    /// <summary>Verifies that the 9-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_9_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9>((_, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 10-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_10_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var service6 = new Service6();
        var service7 = new Service7();
        var service8 = new Service8();
        var service9 = new Service9();
        var service10 = new Service10();
        var mockInstance = new MockReactiveUIInstance(
            service1,
            service2,
            service3,
            service4,
            service5,
            service6,
            service7,
            service8,
            service9,
            service10);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;
        Service8? captured8 = null;
        Service9? captured9 = null;
        Service10? captured10 = null;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10) =>
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

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
            await Assert.That(captured3).IsSameReferenceAs(service3);
            await Assert.That(captured4).IsSameReferenceAs(service4);
            await Assert.That(captured5).IsSameReferenceAs(service5);
            await Assert.That(captured6).IsSameReferenceAs(service6);
            await Assert.That(captured7).IsSameReferenceAs(service7);
            await Assert.That(captured8).IsSameReferenceAs(service8);
            await Assert.That(captured9).IsSameReferenceAs(service9);
            await Assert.That(captured10).IsSameReferenceAs(service10);
        }
    }

    /// <summary>Verifies that the 10-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_10_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10>((_, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 11-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_11_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith11Services();
        var captured = new object?[11];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 11-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_11_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11>((_, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 12-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_12_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith12Services();
        var captured = new object?[12];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                    captured[11] = s12;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 12-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_12_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12>((_, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 13-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_13_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith13Services();
        var captured = new object?[13];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                    captured[11] = s12;
                    captured[12] = s13;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 13-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_13_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13>((_, _, _, _, _, _, _, _, _, _, _, _, _) =>
                    actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 14-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_14_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith14Services();
        var captured = new object?[14];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                    captured[11] = s12;
                    captured[12] = s13;
                    captured[13] = s14;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 14-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_14_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) =>
                    actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 15-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_15_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith15Services();
        var captured = new object?[15];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14,
                    Service15>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                    captured[11] = s12;
                    captured[12] = s13;
                    captured[13] = s14;
                    captured[14] = s15;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 15-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_15_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14,
                    Service15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 16-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_16_Types_Invokes_Action_With_Resolved_Services()
    {
        var (mockInstance, services) = CreateMockWith16Services();
        var captured = new object?[16];

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14,
                    Service15,
                    Service16>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16) =>
                {
                    captured[0] = s1;
                    captured[1] = s2;
                    captured[2] = s3;
                    captured[3] = s4;
                    captured[4] = s5;
                    captured[5] = s6;
                    captured[6] = s7;
                    captured[7] = s8;
                    captured[8] = s9;
                    captured[9] = s10;
                    captured[10] = s11;
                    captured[11] = s12;
                    captured[12] = s13;
                    captured[13] = s14;
                    captured[14] = s15;
                    captured[15] = s16;
                });

        await AssertResolvedServices(mockInstance, result, captured, services);
    }

    /// <summary>Verifies that the 16-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_16_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance
                .WithInstance<
                    Service1,
                    Service2,
                    Service3,
                    Service4,
                    Service5,
                    Service6,
                    Service7,
                    Service8,
                    Service9,
                    Service10,
                    Service11,
                    Service12,
                    Service13,
                    Service14,
                    Service15,
                    Service16>(
                    (_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }
}
