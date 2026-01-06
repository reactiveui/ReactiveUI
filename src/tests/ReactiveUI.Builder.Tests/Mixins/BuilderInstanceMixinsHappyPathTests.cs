// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

public class BuilderInstanceMixinsHappyPathTests
{
    [Test]
    public async Task WithInstance_1_Type_Invokes_Action_With_Resolved_Services()
    {
        var service1 = new Service1();
        var mockInstance = new MockReactiveUIInstance(service1);
        Service1? captured1 = null;

        var result = mockInstance.WithInstance<Service1>((s1) =>
        {
            captured1 = s1;
        });

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(captured1).IsSameReferenceAs(service1);
        }
    }

    [Test]
    public async Task WithInstance_1_Type_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1>((_) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_1_Type_Handles_Null_Action()
    {
        var service1 = new Service1();
        var mockInstance = new MockReactiveUIInstance(service1);

        var result = mockInstance.WithInstance<Service1>(null!);

        await Assert.That(result).IsSameReferenceAs(mockInstance);
    }

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

    [Test]
    public async Task WithInstance_2_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2>((_, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

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

    [Test]
    public async Task WithInstance_3_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3>((_, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

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

    [Test]
    public async Task WithInstance_4_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4>((_, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

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

    [Test]
    public async Task WithInstance_5_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5>((_, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

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

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6>((s1, s2, s3, s4, s5, s6) =>
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

    [Test]
    public async Task WithInstance_6_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6>((_, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

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
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7>((s1, s2, s3, s4, s5, s6, s7) =>
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

    [Test]
    public async Task WithInstance_7_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7>((_, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
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
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;
        Service8? captured8 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8>((s1, s2, s3, s4, s5, s6, s7, s8) =>
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

    [Test]
    public async Task WithInstance_8_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8>((_, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
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
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9);
        Service1? captured1 = null;
        Service2? captured2 = null;
        Service3? captured3 = null;
        Service4? captured4 = null;
        Service5? captured5 = null;
        Service6? captured6 = null;
        Service7? captured7 = null;
        Service8? captured8 = null;
        Service9? captured9 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9>((s1, s2, s3, s4, s5, s6, s7, s8, s9) =>
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

    [Test]
    public async Task WithInstance_9_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9>((_, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
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
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10);
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

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10) =>
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

    [Test]
    public async Task WithInstance_10_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10>((_, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_11_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11);
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
        Service11? captured11 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
        }
    }

    [Test]
    public async Task WithInstance_11_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11>((_, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_12_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var service12 = new Service12();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11, service12);
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
        Service11? captured11 = null;
        Service12? captured12 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
            await Assert.That(captured12).IsSameReferenceAs(service12);
        }
    }

    [Test]
    public async Task WithInstance_12_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12>((_, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_13_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var service12 = new Service12();
        var service13 = new Service13();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11, service12, service13);
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
        Service11? captured11 = null;
        Service12? captured12 = null;
        Service13? captured13 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
            await Assert.That(captured12).IsSameReferenceAs(service12);
            await Assert.That(captured13).IsSameReferenceAs(service13);
        }
    }

    [Test]
    public async Task WithInstance_13_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13>((_, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_14_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var service12 = new Service12();
        var service13 = new Service13();
        var service14 = new Service14();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11, service12, service13, service14);
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
        Service11? captured11 = null;
        Service12? captured12 = null;
        Service13? captured13 = null;
        Service14? captured14 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
            await Assert.That(captured12).IsSameReferenceAs(service12);
            await Assert.That(captured13).IsSameReferenceAs(service13);
            await Assert.That(captured14).IsSameReferenceAs(service14);
        }
    }

    [Test]
    public async Task WithInstance_14_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14>((_, _, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_15_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var service12 = new Service12();
        var service13 = new Service13();
        var service14 = new Service14();
        var service15 = new Service15();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11, service12, service13, service14, service15);
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
        Service11? captured11 = null;
        Service12? captured12 = null;
        Service13? captured13 = null;
        Service14? captured14 = null;
        Service15? captured15 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14, Service15>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
            await Assert.That(captured12).IsSameReferenceAs(service12);
            await Assert.That(captured13).IsSameReferenceAs(service13);
            await Assert.That(captured14).IsSameReferenceAs(service14);
            await Assert.That(captured15).IsSameReferenceAs(service15);
        }
    }

    [Test]
    public async Task WithInstance_15_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14, Service15>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    [Test]
    public async Task WithInstance_16_Types_Invokes_Action_With_Resolved_Services()
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
        var service11 = new Service11();
        var service12 = new Service12();
        var service13 = new Service13();
        var service14 = new Service14();
        var service15 = new Service15();
        var service16 = new Service16();
        var mockInstance = new MockReactiveUIInstance(service1, service2, service3, service4, service5, service6, service7, service8, service9, service10, service11, service12, service13, service14, service15, service16);
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
        Service11? captured11 = null;
        Service12? captured12 = null;
        Service13? captured13 = null;
        Service14? captured14 = null;
        Service15? captured15 = null;
        Service16? captured16 = null;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14, Service15, Service16>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16) =>
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
            await Assert.That(captured11).IsSameReferenceAs(service11);
            await Assert.That(captured12).IsSameReferenceAs(service12);
            await Assert.That(captured13).IsSameReferenceAs(service13);
            await Assert.That(captured14).IsSameReferenceAs(service14);
            await Assert.That(captured15).IsSameReferenceAs(service15);
            await Assert.That(captured16).IsSameReferenceAs(service16);
        }
    }

    [Test]
    public async Task WithInstance_16_Types_Returns_Early_When_Current_Is_Null()
    {
        var mockInstance = new MockReactiveUIInstance(hasNullCurrent: true);
        var actionInvoked = false;

        var result = mockInstance.WithInstance<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8, Service9, Service10, Service11, Service12, Service13, Service14, Service15, Service16>((_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => actionInvoked = true);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsSameReferenceAs(mockInstance);
            await Assert.That(actionInvoked).IsFalse();
        }
    }

    private class Service1;

    private class Service2;

    private class Service3;

    private class Service4;

    private class Service5;

    private class Service6;

    private class Service7;

    private class Service8;

    private class Service9;

    private class Service10;

    private class Service11;

    private class Service12;

    private class Service13;

    private class Service14;

    private class Service15;

    private class Service16;

    private class MockReactiveUIInstance : IReactiveUIInstance
    {
        private readonly MockDependencyResolver? _resolver;

        public MockReactiveUIInstance(params object[] services)
        {
            _resolver = new MockDependencyResolver(services);
        }

        public MockReactiveUIInstance(bool hasNullCurrent)
        {
            _resolver = hasNullCurrent ? null : new MockDependencyResolver();
        }

        public IReadonlyDependencyResolver? Current => _resolver;

        public IMutableDependencyResolver CurrentMutable => throw new NotImplementedException();

        public IScheduler MainThreadScheduler => throw new NotImplementedException();

        public IScheduler TaskpoolScheduler => throw new NotImplementedException();
    }

    private class MockDependencyResolver : IReadonlyDependencyResolver
    {
        private readonly Dictionary<Type, object> _services = [];

        public MockDependencyResolver(params object[] services)
        {
            foreach (var service in services)
            {
                _services[service.GetType()] = service;
            }
        }

        public object? GetService(Type? serviceType)
        {
            if (serviceType == null)
            {
                return null;
            }

            return _services.TryGetValue(serviceType, out var service) ? service : null;
        }

        public object? GetService(Type? serviceType, string? contract)
        {
            if (serviceType == null)
            {
                return null;
            }

            return _services.TryGetValue(serviceType, out var service) ? service : null;
        }

        public T? GetService<T>() => (T?)GetService(typeof(T));

        public T? GetService<T>(string? contract) => (T?)GetService(typeof(T), contract);

        public IEnumerable<object> GetServices(Type? serviceType)
        {
            if (serviceType == null)
            {
                return [];
            }

            return _services.TryGetValue(serviceType, out var service) ? [service] : [];
        }

        public IEnumerable<object> GetServices(Type? serviceType, string? contract)
        {
            if (serviceType == null)
            {
                return [];
            }

            return _services.TryGetValue(serviceType, out var service) ? [service] : [];
        }

        public IEnumerable<T> GetServices<T>() => GetServices(typeof(T)).OfType<T>();

        public IEnumerable<T> GetServices<T>(string? contract) => GetServices(typeof(T), contract).OfType<T>();

        public bool HasRegistration(Type? serviceType, string? contract) =>
            serviceType != null && _services.ContainsKey(serviceType);

        public bool HasRegistration<T>() => HasRegistration(typeof(T), null);

        public bool HasRegistration<T>(string? contract) => HasRegistration(typeof(T), contract);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
