// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Testing.Tests
{
    /// <summary>
    /// TestSequencerTests.
    /// </summary>
    public class TestSequencerTests
    {
        /// <summary>
        /// Shoulds the execute tests in order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Should_Execute_Tests_In_Order()
        {
            using var testSequencer = new TestSequencer();
            var subject = new Subject<Unit>();
            subject.Subscribe(async _ => await testSequencer.AdvancePhaseAsync());

            Assert.Equal(0, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);
            subject.OnNext(Unit.Default);
            Assert.Equal(1, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);
            await testSequencer.AdvancePhaseAsync("Phase 1");
            Assert.Equal(1, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);
            subject.OnNext(Unit.Default);
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);
            await testSequencer.AdvancePhaseAsync("Phase 2");
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(2, testSequencer.CompletedPhases);
        }

        /// <summary>
        /// Shoulds the execute tests in order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Should_Execute_Tests_In_Order_With_Complete()
        {
            using var testSequencer = new TestSequencer();
            var subject = new Subject<Unit>();
            subject.Subscribe(async _ => await testSequencer.AdvancePhaseAsync());

            Assert.Equal(0, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);
            subject.OnNext(Unit.Default);
            Assert.Equal(1, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);
            await testSequencer.CompletePhaseAsync("Phase 1");
            Assert.Equal(1, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);
            subject.OnNext(Unit.Default);
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);
            await testSequencer.CompletePhaseAsync("Phase 2");
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(2, testSequencer.CompletedPhases);
        }

        /// <summary>
        /// Shoulds the execute tests in expected order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Should_Execute_Tests_In_Expected_Order()
        {
            using var testSequencer = new TestSequencer();
            var subject = new Subject<Unit>();
            var expectedPhase = -1;
            subject.Subscribe(async _ =>
            {
                expectedPhase++;
                await testSequencer.AdvancePhaseAsync(string.Empty, expectedPhase);
            });

            Assert.Equal(0, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);

            // Phase 1
            subject.OnNext(Unit.Default);

            // Phase 2
            subject.OnNext(Unit.Default);
            Assert.Equal(1, testSequencer.CurrentPhase);
            Assert.Equal(0, testSequencer.CompletedPhases);
            await testSequencer.CompletePhaseAsync("Phase 1");
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);

            // Phase 3
            subject.OnNext(Unit.Default);
            Assert.Equal(2, testSequencer.CurrentPhase);
            Assert.Equal(1, testSequencer.CompletedPhases);
            await testSequencer.CompletePhaseAsync("Phase 2");
            Assert.Equal(3, testSequencer.CurrentPhase);
            Assert.Equal(2, testSequencer.CompletedPhases);
            await testSequencer.CompletePhaseAsync("Phase 3");
            Assert.Equal(3, testSequencer.CurrentPhase);
            Assert.Equal(3, testSequencer.CompletedPhases);
        }
    }
}
