// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A test fixture for OAPH.
/// </summary>
internal class OAPHIndexerTestFixture : ReactiveObject
{
    private string? _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAPHIndexerTestFixture"/> class.
    /// </summary>
    public OAPHIndexerTestFixture(int test, IScheduler scheduler)
    {
        switch (test)
        {
            case 0:
                var temp = this.WhenAnyValue(static f => f.Text)
                                           .ToProperty(this, static f => f["Whatever"], scheduler: scheduler)
                                           .Value;
                break;

            case 1:
                var temp1 = this.WhenAnyValue(static f => f.Text)
                                           .ToProperty(new ReactiveObject(), static f => f.ToString(), scheduler: scheduler)
                                           .Value;
                break;

            case 2:
                var temp2 = Observable.Return("happy")
                                            .ToProperty(this, string.Empty, scheduler: scheduler)
                                            .Value;
                break;
        }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string? Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    /// <summary>
    /// Gets the string with the specified property name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The string.</returns>
    public string? this[string propertyName] => string.Empty;
}
