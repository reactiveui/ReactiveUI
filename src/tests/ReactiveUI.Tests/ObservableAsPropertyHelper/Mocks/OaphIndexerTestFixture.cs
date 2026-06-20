// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.ObservableAsPropertyHelper.Mocks;

/// <summary>A test fixture for OAPH.</summary>
internal sealed class OaphIndexerTestFixture : ReactiveObject
{
    /// <summary>The test selector value for the Observable.Return scenario.</summary>
    private const int ObservableReturnScenario = 2;

    /// <summary>Initializes a new instance of the <see cref="OaphIndexerTestFixture" /> class.</summary>
    /// <param name="test">The test scenario selector.</param>
    /// <param name="scheduler">The scheduler used for the property binding.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3366:Make sure the use of this in constructors is safe here",
        Justification = "OAPH initialization requires 'this' in the constructor; single-threaded test fixture.")]
    public OaphIndexerTestFixture(int test, ISequencer scheduler)
    {
        switch (test)
        {
            case 0:
                {
                    _ = this.WhenAnyValue(static f => f.Text).ToProperty(this, static f => f["Whatever"], scheduler: scheduler)
                        .Value;
                    break;
                }

            case 1:
                {
                    _ = this.WhenAnyValue(static f => f.Text).ToProperty(new ReactiveObject(), static f => f.ToString(), scheduler: scheduler)
                        .Value;
                    break;
                }

            case ObservableReturnScenario:
                {
                    _ = Signal.Emit("happy").ToProperty(this, string.Empty, scheduler: scheduler)
                        .Value;
                    break;
                }
        }
    }

    /// <summary>Gets or sets the text.</summary>
    public string? Text
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the string with the specified property name.</summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The string.</returns>
    public string? this[string propertyName] => string.Empty;
}
