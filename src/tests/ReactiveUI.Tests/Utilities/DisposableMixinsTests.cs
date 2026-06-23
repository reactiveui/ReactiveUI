// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>Tests for DisposableMixins utility methods.</summary>
public class DisposableMixinsTests
{
    /// <summary>Tests that DisposeWith adds disposable to composite.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWith_AddsToComposite()
    {
        // Arrange
        var disposable1 = Scope.Create(() => { });
        var disposable2 = Scope.Create(() => { });
        var compositeDisposable = new MultipleDisposable();

        // Act
        const int ExpectedCount = 2;
        _ = disposable1.DisposeWith(compositeDisposable);
        _ = disposable2.DisposeWith(compositeDisposable);

        // Assert
        await Assert.That(compositeDisposable).Count().IsEqualTo(ExpectedCount);
    }

    /// <summary>Tests that DisposeWith disposes when composite is disposed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWith_DisposesWhenCompositeDisposed()
    {
        // Arrange
        var disposed = false;
        var disposable = Scope.Create(() => disposed = true);
        var compositeDisposable = new MultipleDisposable();

        // Act
        _ = disposable.DisposeWith(compositeDisposable);
        compositeDisposable.Dispose();

        // Assert
        await Assert.That(disposed).IsTrue();
    }

    /// <summary>Tests that DisposeWith returns original disposable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWith_ReturnsOriginalDisposable()
    {
        // Arrange
        var disposable = Scope.Create(() => { });
        var compositeDisposable = new MultipleDisposable();

        // Act
        var result = disposable.DisposeWith(compositeDisposable);

        // Assert
        await Assert.That(result).IsSameReferenceAs(disposable);
    }
}
