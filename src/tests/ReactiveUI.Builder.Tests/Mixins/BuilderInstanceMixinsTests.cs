// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>
/// Tests for the instance- and extension-based WithInstance overloads on the builder.
/// </summary>
public partial class BuilderInstanceMixinsTests
{
    /// <summary>
    /// Registers nine test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices09(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
    }

    /// <summary>
    /// Registers ten test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices10(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
    }

    /// <summary>
    /// Registers eleven test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices11(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
    }

    /// <summary>
    /// Registers twelve test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    /// <param name="s12">The created instance for service 12.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices12(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11,
        out InstanceService12 s12)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
    }

    /// <summary>
    /// Registers thirteen test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    /// <param name="s12">The created instance for service 12.</param>
    /// <param name="s13">The created instance for service 13.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices13(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11,
        out InstanceService12 s12,
        out InstanceService13 s13)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
    }

    /// <summary>
    /// Registers fourteen test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    /// <param name="s12">The created instance for service 12.</param>
    /// <param name="s13">The created instance for service 13.</param>
    /// <param name="s14">The created instance for service 14.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices14(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11,
        out InstanceService12 s12,
        out InstanceService13 s13,
        out InstanceService14 s14)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
    }

    /// <summary>
    /// Registers fifteen test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    /// <param name="s12">The created instance for service 12.</param>
    /// <param name="s13">The created instance for service 13.</param>
    /// <param name="s14">The created instance for service 14.</param>
    /// <param name="s15">The created instance for service 15.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices15(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11,
        out InstanceService12 s12,
        out InstanceService13 s13,
        out InstanceService14 s14,
        out InstanceService15 s15)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
        s15 = new InstanceService15();
        resolver.RegisterConstant(s15, typeof(InstanceService15));
    }

    /// <summary>
    /// Registers sixteen test service instances on the resolver and returns the created instances.
    /// </summary>
    /// <param name="resolver">The resolver to register the services on.</param>
    /// <param name="s1">The created instance for service 1.</param>
    /// <param name="s2">The created instance for service 2.</param>
    /// <param name="s3">The created instance for service 3.</param>
    /// <param name="s4">The created instance for service 4.</param>
    /// <param name="s5">The created instance for service 5.</param>
    /// <param name="s6">The created instance for service 6.</param>
    /// <param name="s7">The created instance for service 7.</param>
    /// <param name="s8">The created instance for service 8.</param>
    /// <param name="s9">The created instance for service 9.</param>
    /// <param name="s10">The created instance for service 10.</param>
    /// <param name="s11">The created instance for service 11.</param>
    /// <param name="s12">The created instance for service 12.</param>
    /// <param name="s13">The created instance for service 13.</param>
    /// <param name="s14">The created instance for service 14.</param>
    /// <param name="s15">The created instance for service 15.</param>
    /// <param name="s16">The created instance for service 16.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void RegisterServices16(
        ModernDependencyResolver resolver,
        out InstanceService01 s1,
        out InstanceService02 s2,
        out InstanceService03 s3,
        out InstanceService04 s4,
        out InstanceService05 s5,
        out InstanceService06 s6,
        out InstanceService07 s7,
        out InstanceService08 s8,
        out InstanceService09 s9,
        out InstanceService10 s10,
        out InstanceService11 s11,
        out InstanceService12 s12,
        out InstanceService13 s13,
        out InstanceService14 s14,
        out InstanceService15 s15,
        out InstanceService16 s16)
    {
        s1 = new InstanceService01();
        resolver.RegisterConstant(s1, typeof(InstanceService01));
        s2 = new InstanceService02();
        resolver.RegisterConstant(s2, typeof(InstanceService02));
        s3 = new InstanceService03();
        resolver.RegisterConstant(s3, typeof(InstanceService03));
        s4 = new InstanceService04();
        resolver.RegisterConstant(s4, typeof(InstanceService04));
        s5 = new InstanceService05();
        resolver.RegisterConstant(s5, typeof(InstanceService05));
        s6 = new InstanceService06();
        resolver.RegisterConstant(s6, typeof(InstanceService06));
        s7 = new InstanceService07();
        resolver.RegisterConstant(s7, typeof(InstanceService07));
        s8 = new InstanceService08();
        resolver.RegisterConstant(s8, typeof(InstanceService08));
        s9 = new InstanceService09();
        resolver.RegisterConstant(s9, typeof(InstanceService09));
        s10 = new InstanceService10();
        resolver.RegisterConstant(s10, typeof(InstanceService10));
        s11 = new InstanceService11();
        resolver.RegisterConstant(s11, typeof(InstanceService11));
        s12 = new InstanceService12();
        resolver.RegisterConstant(s12, typeof(InstanceService12));
        s13 = new InstanceService13();
        resolver.RegisterConstant(s13, typeof(InstanceService13));
        s14 = new InstanceService14();
        resolver.RegisterConstant(s14, typeof(InstanceService14));
        s15 = new InstanceService15();
        resolver.RegisterConstant(s15, typeof(InstanceService15));
        s16 = new InstanceService16();
        resolver.RegisterConstant(s16, typeof(InstanceService16));
    }

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService01;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService02;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService03;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService04;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService05;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService06;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService07;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService08;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService09;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService10;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService11;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService12;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService13;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService14;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService15;

    /// <summary>
    /// Test service type used to verify instance resolution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Marker type for tests.")]
    private sealed class InstanceService16;
}
