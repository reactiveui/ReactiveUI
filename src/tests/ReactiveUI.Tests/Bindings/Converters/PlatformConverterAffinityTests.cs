// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_MAUI || HAS_WPF || HAS_WINUI || HAS_UNO
namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
/// Tests for verifying platform-specific converter affinity values.
/// Uses TUnit's MethodDataSource for theory-style testing with compile-time safety.
/// </summary>
public class PlatformConverterAffinityTests
{
    /// <summary>
    /// Verifies that platform-specific converters have the correct affinity values.
    /// Platform converters should have affinity 2 (same as standard converters).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(GetPlatformConverters))]
    public async Task PlatformConverters_ShouldHaveCorrectAffinity(IBindingTypeConverter converter, int expectedAffinity)
    {
        // Act
        var actualAffinity = converter.GetAffinityForObjects();

        // Assert
        await Assert.That(actualAffinity).IsEqualTo(expectedAffinity);
    }

    /// <summary>
    /// Data source for platform-specific converters.
    /// </summary>
    public static IEnumerable<(IBindingTypeConverter converter, int expectedAffinity)> GetPlatformConverters()
    {
#if HAS_WPF || HAS_WINUI || HAS_UNO
        // WPF/WinUI/UNO visibility converters (affinity = 2, same as standard converters)
        yield return (new ReactiveUI.BooleanToVisibilityTypeConverter(), 2);
        yield return (new ReactiveUI.VisibilityToBooleanTypeConverter(), 2);
#endif

#if IS_MAUI
        // MAUI visibility converters (affinity = 2, same as standard converters)
        yield return (new ReactiveUI.Maui.BooleanToVisibilityTypeConverter(), 2);
        yield return (new ReactiveUI.Maui.VisibilityToBooleanTypeConverter(), 2);
#endif
    }
}
#endif
