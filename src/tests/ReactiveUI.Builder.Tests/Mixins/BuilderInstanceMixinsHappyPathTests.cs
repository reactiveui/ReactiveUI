// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Tests the happy-path behaviour of the instance-based WithInstance overloads.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S4018:Generic methods should provide type parameters",
    Justification = "Test exercises a generic overload with explicit type arguments.")]
public partial class BuilderInstanceMixinsHappyPathTests
{
    /// <summary>Verifies that the 1-type WithInstance overload invokes the action with the resolved instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_1_Type_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var mockInstance = new MockReactiveUIInstance(service1);
        Service1? captured1 = null;

        var result = mockInstance.WithInstance<Service1>(s1 => captured1 = s1);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
        }
    }

    /// <summary>Verifies that the 1-type WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_1_Type_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1>(_ => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 1-type WithInstance overload returns the same instance when given a null action.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_1_Type_Handles_Null_Action()
    {
        var service1 = new Service1();
        var mockInstance = new MockReactiveUIInstance(service1);

        var result = mockInstance.WithInstance<Service1>(null!);

        await Assert.That(result).IsSameReferenceAs(mockInstance);
    }

    /// <summary>Verifies that the 2-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_2_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var mockInstance = new MockReactiveUIInstance(service1, service2);
        Service1? captured1 = null;
        Service2? captured2 = null;

        var result = mockInstance.WithInstance<Service1, Service2>((s1, s2) =>
        {
            captured1 = s1;
            captured2 = s2;
        });

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
        }
    }

    /// <summary>Verifies that the 2-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_2_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2>((_, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 3-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_3_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3>((s1, s2, s3) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
        });

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
            await Assert.That(captured3).IsSameReferenceAs(service3);
        }
    }

    /// <summary>Verifies that the 3-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_3_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3>((_, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 4-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_4_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4>((s1, s2, s3, s4) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
            captured4 = s4;
        });

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
            await Assert.That(captured3).IsSameReferenceAs(service3);
            await Assert.That(captured4).IsSameReferenceAs(service4);
        }
    }

    /// <summary>Verifies that the 4-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_4_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4>((_, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 5-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_5_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5>((s1, s2, s3, s4, s5) =>
        {
            captured1 = s1;
            captured2 = s2;
            captured3 = s3;
            captured4 = s4;
            captured5 = s5;
        });

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
            await Assert.That(captured2).IsSameReferenceAs(service2);
            await Assert.That(captured3).IsSameReferenceAs(service3);
            await Assert.That(captured4).IsSameReferenceAs(service4);
            await Assert.That(captured5).IsSameReferenceAs(service5);
        }
    }

    /// <summary>Verifies that the 5-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_5_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5>((_, _, _, _, _) =>
                actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 6-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_6_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var service6 = new Service6();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6>((s1, s2, s3, s4, s5, s6) =>
            {
                captured1 = s1;
                captured2 = s2;
                captured3 = s3;
                captured4 = s4;
                captured5 = s5;
                captured6 = s6;
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
        }
    }

    /// <summary>Verifies that the 6-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_6_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6>((_, _, _, _, _, _) =>
                actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 7-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_7_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var service6 = new Service6();
        var service7 = new Service7();
        var mockInstance =
            new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7>(
                (s1, s2, s3, s4, s5, s6, s7) =>
                {
                    captured1 = s1;
                    captured2 = s2;
                    captured3 = s3;
                    captured4 = s4;
                    captured5 = s5;
                    captured6 = s6;
                    captured7 = s7;
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
        }
    }

    /// <summary>Verifies that the 7-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithInstance_7_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7>(
                (_, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>Verifies that the 8-types WithInstance overload invokes the action with the resolved instances.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_8_Types_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var service2 = new Service2();
        var service3 = new Service3();
        var service4 = new Service4();
        var service5 = new Service5();
        var service6 = new Service6();
        var service7 = new Service7();
        var service8 = new Service8();
        var mockInstance = new MockReactiveUIInstance(
            service1,
            service2,
            service3,
            service4,
            service5,
            service6,
            service7,
            service8);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;
        Service8? captured8 = null;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8>(
                (s1, s2, s3, s4, s5, s6, s7, s8) =>
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
        }
    }

    /// <summary>Verifies that the 8-types WithInstance overload returns early without invoking the action when the current resolver is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    public async Task WithInstance_8_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(true);
        var actionInvoked = false;

        var result =
            mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8>(
                (_, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    /// <summary>
    /// Asserts that the WithInstance call returned the same instance and that each captured service matches the
    /// corresponding registered service in order.
    /// </summary>
    /// <param name="expectedInstance">The instance the WithInstance call should have returned.</param>
    /// <param name="actualResult">The instance actually returned by the WithInstance call.</param>
    /// <param name="captured">The services captured by the invoked action, in declaration order.</param>
    /// <param name="services">The registered services, in declaration order.</param>
    /// <returns>A task representing the asynchronous assertion.</returns>
    private static async Task AssertResolvedServices(
        MockReactiveUIInstance expectedInstance,
        object? actualResult,
        object?[] captured,
        object[] services)
    {
        using (Assert.Multiple())
        {
            await Assert.That(actualResult).IsSameReferenceAs(expectedInstance);
            for (var i = 0; i < services.Length; i++)
            {
                await Assert.That(captured[i]).IsSameReferenceAs(services[i]);
            }
        }
    }

    /// <summary>Creates a mock instance backed by eleven distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith11Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11()
        ];
        return (new(services), services);
    }

    /// <summary>Creates a mock instance backed by twelve distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith12Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11(), new Service12()
        ];
        return (new(services), services);
    }

    /// <summary>Creates a mock instance backed by thirteen distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith13Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11(), new Service12(),
            new Service13()
        ];
        return (new(services), services);
    }

    /// <summary>Creates a mock instance backed by fourteen distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith14Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11(), new Service12(),
            new Service13(), new Service14()
        ];
        return (new(services), services);
    }

    /// <summary>Creates a mock instance backed by fifteen distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith15Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11(), new Service12(),
            new Service13(), new Service14(), new Service15()
        ];
        return (new(services), services);
    }

    /// <summary>Creates a mock instance backed by sixteen distinct services.</summary>
    /// <returns>The mock instance and the registered services in declaration order.</returns>
    private static (MockReactiveUIInstance MockInstance, object[] Services) CreateMockWith16Services()
    {
        object[] services =
        [
            new Service1(), new Service2(), new Service3(), new Service4(), new Service5(), new Service6(),
            new Service7(), new Service8(), new Service9(), new Service10(), new Service11(), new Service12(),
            new Service13(), new Service14(), new Service15(), new Service16()
        ];
        return (new(services), services);
    }

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service1;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service2;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service3;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service4;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service5;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service6;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service7;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service8;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service9;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service10;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service11;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service12;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service13;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service14;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service15;

    /// <summary>Test service type used to verify instance resolution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class Service16;

    /// <summary>Mock <see cref="IReactiveUIInstance"/> backed by a configurable dependency resolver.</summary>
    private sealed class MockReactiveUIInstance : IReactiveUIInstance
    {
        /// <summary>The resolver backing this instance, or <see langword="null"/> to simulate a null current resolver.</summary>
        private readonly MockDependencyResolver? _resolver;

        /// <summary>Initializes a new instance of the <see cref="MockReactiveUIInstance"/> class with the given services.</summary>
        /// <param name="services">The services to register.</param>
        public MockReactiveUIInstance(params object[] services) => _resolver = new(services);

        /// <summary>Initializes a new instance of the <see cref="MockReactiveUIInstance"/> class, optionally with a null current resolver.</summary>
        /// <param name="hasNullCurrent">If <see langword="true"/>, the current resolver is null.</param>
        public MockReactiveUIInstance(bool hasNullCurrent) =>
            _resolver = hasNullCurrent ? null : new MockDependencyResolver();

        /// <inheritdoc/>
        public IReadonlyDependencyResolver? Current => _resolver;

        /// <inheritdoc/>
        public IMutableDependencyResolver CurrentMutable => throw new NotSupportedException();

        /// <inheritdoc/>
        public ISequencer MainThreadScheduler => throw new NotSupportedException();

        /// <inheritdoc/>
        public ISequencer TaskpoolScheduler => throw new NotSupportedException();
    }

    /// <summary>Mock read-only dependency resolver backed by a dictionary keyed on service type.</summary>
    private sealed class MockDependencyResolver : IReadonlyDependencyResolver, IDisposable
    {
        /// <summary>The registered services keyed by their concrete type.</summary>
        private readonly Dictionary<Type, object> _services = [];

        /// <summary>Initializes a new instance of the <see cref="MockDependencyResolver"/> class with the given services.</summary>
        /// <param name="services">The services to register.</param>
        public MockDependencyResolver(params object[] services)
        {
            foreach (var service in services)
            {
                _services[service.GetType()] = service;
            }
        }

        /// <inheritdoc/>
        public object? GetService(Type? serviceType)
        {
            if (serviceType is null)
            {
                return null;
            }

            return _services.TryGetValue(serviceType, out var service) ? service : null;
        }

        /// <inheritdoc/>
        public object? GetService(Type? serviceType, string? contract)
        {
            _ = contract;
            if (serviceType is null)
            {
                return null;
            }

            return _services.TryGetValue(serviceType, out var service) ? service : null;
        }

        /// <inheritdoc/>
        public T? GetService<T>() => (T?)GetService(typeof(T));

        /// <inheritdoc/>
        public T? GetService<T>(string? contract) => (T?)GetService(typeof(T), contract);

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType)
        {
            if (serviceType is null)
            {
                return [];
            }

            return _services.TryGetValue(serviceType, out var service) ? [service] : [];
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType, string? contract)
        {
            _ = contract;
            if (serviceType is null)
            {
                return [];
            }

            return _services.TryGetValue(serviceType, out var service) ? [service] : [];
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>() => GetServices(typeof(T)).OfType<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>(string? contract) => GetServices(typeof(T), contract).OfType<T>();

        /// <summary>Determines whether a service of the given type is registered.</summary>
        /// <param name="serviceType">The type of service to check for.</param>
        /// <param name="contract">The optional contract; ignored by this mock.</param>
        /// <returns><see langword="true"/> if the service is registered; otherwise, <see langword="false"/>.</returns>
        public bool HasRegistration(Type? serviceType, string? contract)
        {
            _ = contract;
            return serviceType is not null && _services.ContainsKey(serviceType);
        }

        /// <summary>Determines whether a service of the given type is registered.</summary>
        /// <typeparam name="T">The type of service to check for.</typeparam>
        /// <returns><see langword="true"/> if the service is registered; otherwise, <see langword="false"/>.</returns>
        public bool HasRegistration<T>() => HasRegistration(typeof(T), null);

        /// <summary>Determines whether a service of the given type is registered.</summary>
        /// <typeparam name="T">The type of service to check for.</typeparam>
        /// <param name="contract">The optional contract; ignored by this mock.</param>
        /// <returns><see langword="true"/> if the service is registered; otherwise, <see langword="false"/>.</returns>
        public bool HasRegistration<T>(string? contract) => HasRegistration(typeof(T), contract);

        /// <summary>Releases the resources used by this resolver.</summary>
        public void Dispose()
        {
        }
    }
}
