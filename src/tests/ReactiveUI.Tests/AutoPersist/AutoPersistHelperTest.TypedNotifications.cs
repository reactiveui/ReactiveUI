// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>Persistence follows the concrete sender after bindings initialize interface-typed state.</summary>
public partial class AutoPersistHelperTest
{
    /// <summary>Interface-first initialization cannot change whether persisted property edits are saved.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_InterfaceInitializedState_SavesConcreteSender()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var fixture = new TestFixture();
        _ = ((IReactiveObject)fixture).GetChangedObservable();
        TestFixture? saved = null;
        var metadata = new AutoPersistHelperMixins.AutoPersistMetadata(true, new HashSet<string> { nameof(TestFixture.IsOnlyOneWord) });
        using var persistence = fixture.AutoPersist(
            value =>
            {
                saved = value;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            metadata,
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsOnlyOneWord = nameof(fixture);
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saved).IsSameReferenceAs(fixture);
    }
}
