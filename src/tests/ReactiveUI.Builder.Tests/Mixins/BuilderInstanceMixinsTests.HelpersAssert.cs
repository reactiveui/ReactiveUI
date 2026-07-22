// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Shared helpers that assert the captured instances match the expected instances for the higher-arity overloads.</summary>
public partial class BuilderInstanceMixinsTests
{
    /// <summary>Asserts that each of the nine captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences09(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
    }

    /// <summary>Asserts that each of the ten captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences10(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
    }

    /// <summary>Asserts that each of the eleven captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences11(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
    }

    /// <summary>Asserts that each of the twelve captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <param name="captured12">The captured instance 12.</param>
    /// <param name="expected12">The expected instance 12.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences12(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11,
        InstanceService12? captured12,
        InstanceService12 expected12)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
        await Assert.That(captured12).IsSameReferenceAs(expected12);
    }

    /// <summary>Asserts that each of the thirteen captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <param name="captured12">The captured instance 12.</param>
    /// <param name="expected12">The expected instance 12.</param>
    /// <param name="captured13">The captured instance 13.</param>
    /// <param name="expected13">The expected instance 13.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences13(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11,
        InstanceService12? captured12,
        InstanceService12 expected12,
        InstanceService13? captured13,
        InstanceService13 expected13)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
        await Assert.That(captured12).IsSameReferenceAs(expected12);
        await Assert.That(captured13).IsSameReferenceAs(expected13);
    }

    /// <summary>Asserts that each of the fourteen captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <param name="captured12">The captured instance 12.</param>
    /// <param name="expected12">The expected instance 12.</param>
    /// <param name="captured13">The captured instance 13.</param>
    /// <param name="expected13">The expected instance 13.</param>
    /// <param name="captured14">The captured instance 14.</param>
    /// <param name="expected14">The expected instance 14.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences14(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11,
        InstanceService12? captured12,
        InstanceService12 expected12,
        InstanceService13? captured13,
        InstanceService13 expected13,
        InstanceService14? captured14,
        InstanceService14 expected14)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
        await Assert.That(captured12).IsSameReferenceAs(expected12);
        await Assert.That(captured13).IsSameReferenceAs(expected13);
        await Assert.That(captured14).IsSameReferenceAs(expected14);
    }

    /// <summary>Asserts that each of the fifteen captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <param name="captured12">The captured instance 12.</param>
    /// <param name="expected12">The expected instance 12.</param>
    /// <param name="captured13">The captured instance 13.</param>
    /// <param name="expected13">The expected instance 13.</param>
    /// <param name="captured14">The captured instance 14.</param>
    /// <param name="expected14">The expected instance 14.</param>
    /// <param name="captured15">The captured instance 15.</param>
    /// <param name="expected15">The expected instance 15.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences15(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11,
        InstanceService12? captured12,
        InstanceService12 expected12,
        InstanceService13? captured13,
        InstanceService13 expected13,
        InstanceService14? captured14,
        InstanceService14 expected14,
        InstanceService15? captured15,
        InstanceService15 expected15)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
        await Assert.That(captured12).IsSameReferenceAs(expected12);
        await Assert.That(captured13).IsSameReferenceAs(expected13);
        await Assert.That(captured14).IsSameReferenceAs(expected14);
        await Assert.That(captured15).IsSameReferenceAs(expected15);
    }

    /// <summary>Asserts that each of the sixteen captured instances is the same reference as the expected instance.</summary>
    /// <param name="captured1">The captured instance 1.</param>
    /// <param name="expected1">The expected instance 1.</param>
    /// <param name="captured2">The captured instance 2.</param>
    /// <param name="expected2">The expected instance 2.</param>
    /// <param name="captured3">The captured instance 3.</param>
    /// <param name="expected3">The expected instance 3.</param>
    /// <param name="captured4">The captured instance 4.</param>
    /// <param name="expected4">The expected instance 4.</param>
    /// <param name="captured5">The captured instance 5.</param>
    /// <param name="expected5">The expected instance 5.</param>
    /// <param name="captured6">The captured instance 6.</param>
    /// <param name="expected6">The expected instance 6.</param>
    /// <param name="captured7">The captured instance 7.</param>
    /// <param name="expected7">The expected instance 7.</param>
    /// <param name="captured8">The captured instance 8.</param>
    /// <param name="expected8">The expected instance 8.</param>
    /// <param name="captured9">The captured instance 9.</param>
    /// <param name="expected9">The expected instance 9.</param>
    /// <param name="captured10">The captured instance 10.</param>
    /// <param name="expected10">The expected instance 10.</param>
    /// <param name="captured11">The captured instance 11.</param>
    /// <param name="expected11">The expected instance 11.</param>
    /// <param name="captured12">The captured instance 12.</param>
    /// <param name="expected12">The expected instance 12.</param>
    /// <param name="captured13">The captured instance 13.</param>
    /// <param name="expected13">The expected instance 13.</param>
    /// <param name="captured14">The captured instance 14.</param>
    /// <param name="expected14">The expected instance 14.</param>
    /// <param name="captured15">The captured instance 15.</param>
    /// <param name="expected15">The expected instance 15.</param>
    /// <param name="captured16">The captured instance 16.</param>
    /// <param name="expected16">The expected instance 16.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "SST1472:Signatures should not declare too many parameters",
        Justification = "Test exercises a variadic overload.")]
    private static async Task AssertSameReferences16(
        InstanceService01? captured1,
        InstanceService01 expected1,
        InstanceService02? captured2,
        InstanceService02 expected2,
        InstanceService03? captured3,
        InstanceService03 expected3,
        InstanceService04? captured4,
        InstanceService04 expected4,
        InstanceService05? captured5,
        InstanceService05 expected5,
        InstanceService06? captured6,
        InstanceService06 expected6,
        InstanceService07? captured7,
        InstanceService07 expected7,
        InstanceService08? captured8,
        InstanceService08 expected8,
        InstanceService09? captured9,
        InstanceService09 expected9,
        InstanceService10? captured10,
        InstanceService10 expected10,
        InstanceService11? captured11,
        InstanceService11 expected11,
        InstanceService12? captured12,
        InstanceService12 expected12,
        InstanceService13? captured13,
        InstanceService13 expected13,
        InstanceService14? captured14,
        InstanceService14 expected14,
        InstanceService15? captured15,
        InstanceService15 expected15,
        InstanceService16? captured16,
        InstanceService16 expected16)
    {
        await Assert.That(captured1).IsSameReferenceAs(expected1);
        await Assert.That(captured2).IsSameReferenceAs(expected2);
        await Assert.That(captured3).IsSameReferenceAs(expected3);
        await Assert.That(captured4).IsSameReferenceAs(expected4);
        await Assert.That(captured5).IsSameReferenceAs(expected5);
        await Assert.That(captured6).IsSameReferenceAs(expected6);
        await Assert.That(captured7).IsSameReferenceAs(expected7);
        await Assert.That(captured8).IsSameReferenceAs(expected8);
        await Assert.That(captured9).IsSameReferenceAs(expected9);
        await Assert.That(captured10).IsSameReferenceAs(expected10);
        await Assert.That(captured11).IsSameReferenceAs(expected11);
        await Assert.That(captured12).IsSameReferenceAs(expected12);
        await Assert.That(captured13).IsSameReferenceAs(expected13);
        await Assert.That(captured14).IsSameReferenceAs(expected14);
        await Assert.That(captured15).IsSameReferenceAs(expected15);
        await Assert.That(captured16).IsSameReferenceAs(expected16);
    }
}
