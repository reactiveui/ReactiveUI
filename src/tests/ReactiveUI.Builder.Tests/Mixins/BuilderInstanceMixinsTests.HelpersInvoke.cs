// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Shared helpers that invoke the higher-arity WithInstance overloads and capture the resolved instances.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Layout",
    "SST1523:Members should not be too long",
    Justification = "High-arity variadic WithInstance helper methods are intrinsically long; each out-parameter and assignment sits on its own line under one-statement-per-line formatting.")]
public partial class BuilderInstanceMixinsTests
{
    /// <summary>Invokes the nine-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance09(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
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
                InstanceService09>((s1, s2, s3, s4, s5, s6, s7, s8, s9) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
    }

    /// <summary>Invokes the ten-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance10(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
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
                InstanceService09,
                InstanceService10>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
    }

    /// <summary>Invokes the eleven-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance11(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
    }

    /// <summary>Invokes the twelve-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    /// <param name="captured12">Receives the resolved instance 12.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance12(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11,
        out InstanceService12? captured12)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
        InstanceService12? c12 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11,
                InstanceService12>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
                c12 = s12;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
        captured12 = c12;
    }

    /// <summary>Invokes the thirteen-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    /// <param name="captured12">Receives the resolved instance 12.</param>
    /// <param name="captured13">Receives the resolved instance 13.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance13(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11,
        out InstanceService12? captured12,
        out InstanceService13? captured13)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
        InstanceService12? c12 = null;
        InstanceService13? c13 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11,
                InstanceService12,
                InstanceService13>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
                c12 = s12;
                c13 = s13;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
        captured12 = c12;
        captured13 = c13;
    }

    /// <summary>Invokes the fourteen-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    /// <param name="captured12">Receives the resolved instance 12.</param>
    /// <param name="captured13">Receives the resolved instance 13.</param>
    /// <param name="captured14">Receives the resolved instance 14.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance14(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11,
        out InstanceService12? captured12,
        out InstanceService13? captured13,
        out InstanceService14? captured14)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
        InstanceService12? c12 = null;
        InstanceService13? c13 = null;
        InstanceService14? c14 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11,
                InstanceService12,
                InstanceService13,
                InstanceService14>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
                c12 = s12;
                c13 = s13;
                c14 = s14;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
        captured12 = c12;
        captured13 = c13;
        captured14 = c14;
    }

    /// <summary>Invokes the fifteen-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    /// <param name="captured12">Receives the resolved instance 12.</param>
    /// <param name="captured13">Receives the resolved instance 13.</param>
    /// <param name="captured14">Receives the resolved instance 14.</param>
    /// <param name="captured15">Receives the resolved instance 15.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance15(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11,
        out InstanceService12? captured12,
        out InstanceService13? captured13,
        out InstanceService14? captured14,
        out InstanceService15? captured15)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
        InstanceService12? c12 = null;
        InstanceService13? c13 = null;
        InstanceService14? c14 = null;
        InstanceService15? c15 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11,
                InstanceService12,
                InstanceService13,
                InstanceService14,
                InstanceService15>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
                c12 = s12;
                c13 = s13;
                c14 = s14;
                c15 = s15;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
        captured12 = c12;
        captured13 = c13;
        captured14 = c14;
        captured15 = c15;
    }

    /// <summary>Invokes the sixteen-type WithInstance overload and captures the resolved instances.</summary>
    /// <param name="builder">The builder to invoke the overload on.</param>
    /// <param name="captured1">Receives the resolved instance 1.</param>
    /// <param name="captured2">Receives the resolved instance 2.</param>
    /// <param name="captured3">Receives the resolved instance 3.</param>
    /// <param name="captured4">Receives the resolved instance 4.</param>
    /// <param name="captured5">Receives the resolved instance 5.</param>
    /// <param name="captured6">Receives the resolved instance 6.</param>
    /// <param name="captured7">Receives the resolved instance 7.</param>
    /// <param name="captured8">Receives the resolved instance 8.</param>
    /// <param name="captured9">Receives the resolved instance 9.</param>
    /// <param name="captured10">Receives the resolved instance 10.</param>
    /// <param name="captured11">Receives the resolved instance 11.</param>
    /// <param name="captured12">Receives the resolved instance 12.</param>
    /// <param name="captured13">Receives the resolved instance 13.</param>
    /// <param name="captured14">Receives the resolved instance 14.</param>
    /// <param name="captured15">Receives the resolved instance 15.</param>
    /// <param name="captured16">Receives the resolved instance 16.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static void InvokeWithInstance16(
        ReactiveUIBuilder builder,
        out InstanceService01? captured1,
        out InstanceService02? captured2,
        out InstanceService03? captured3,
        out InstanceService04? captured4,
        out InstanceService05? captured5,
        out InstanceService06? captured6,
        out InstanceService07? captured7,
        out InstanceService08? captured8,
        out InstanceService09? captured9,
        out InstanceService10? captured10,
        out InstanceService11? captured11,
        out InstanceService12? captured12,
        out InstanceService13? captured13,
        out InstanceService14? captured14,
        out InstanceService15? captured15,
        out InstanceService16? captured16)
    {
        InstanceService01? c1 = null;
        InstanceService02? c2 = null;
        InstanceService03? c3 = null;
        InstanceService04? c4 = null;
        InstanceService05? c5 = null;
        InstanceService06? c6 = null;
        InstanceService07? c7 = null;
        InstanceService08? c8 = null;
        InstanceService09? c9 = null;
        InstanceService10? c10 = null;
        InstanceService11? c11 = null;
        InstanceService12? c12 = null;
        InstanceService13? c13 = null;
        InstanceService14? c14 = null;
        InstanceService15? c15 = null;
        InstanceService16? c16 = null;
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
                InstanceService09,
                InstanceService10,
                InstanceService11,
                InstanceService12,
                InstanceService13,
                InstanceService14,
                InstanceService15,
                InstanceService16>((s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16) =>
            {
                c1 = s1;
                c2 = s2;
                c3 = s3;
                c4 = s4;
                c5 = s5;
                c6 = s6;
                c7 = s7;
                c8 = s8;
                c9 = s9;
                c10 = s10;
                c11 = s11;
                c12 = s12;
                c13 = s13;
                c14 = s14;
                c15 = s15;
                c16 = s16;
            });
        captured1 = c1;
        captured2 = c2;
        captured3 = c3;
        captured4 = c4;
        captured5 = c5;
        captured6 = c6;
        captured7 = c7;
        captured8 = c8;
        captured9 = c9;
        captured10 = c10;
        captured11 = c11;
        captured12 = c12;
        captured13 = c13;
        captured14 = c14;
        captured15 = c15;
        captured16 = c16;
    }
}
