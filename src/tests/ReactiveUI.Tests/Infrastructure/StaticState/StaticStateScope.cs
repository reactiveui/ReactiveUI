// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Infrastructure.StaticState;

/// <summary>
/// A generic disposable scope for capturing and restoring arbitrary static state.
/// Use this when you need to snapshot and restore state that doesn't have a dedicated scope class.
/// </summary>
/// <remarks>
/// This helper allows you to capture any state via getter functions and restore it via setter actions.
/// Tests using this scope should also be marked with [NotInParallel] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
/// <example>
/// <code>
/// [TestFixture]
/// [NotInParallel]
/// public class MyTests
/// {
///     private StaticStateScope _stateScope;
///
///     [SetUp]
///     public void SetUp()
///     {
///         _stateScope = new StaticStateScope(
///             () => MyClass.StaticProperty,
///             value => MyClass.StaticProperty = value,
///             () => AnotherClass.StaticField,
///             value => AnotherClass.StaticField = value);
///
///         // Now safe to modify static state
///     }
///
///     [TearDown]
///     public void TearDown()
///     {
///         _stateScope?.Dispose();
///     }
/// }
/// </code>
/// </example>
public sealed class StaticStateScope : IDisposable
{
    private readonly List<Action> _restoreActions = [];
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticStateScope"/> class with multiple state capture/restore pairs.
    /// </summary>
    /// <param name="stateCaptures">Pairs of getter functions and setter actions. Each pair should be: getter function, setter action.</param>
    public StaticStateScope(params object[] stateCaptures)
    {
        ArgumentNullException.ThrowIfNull(stateCaptures);

        if (stateCaptures.Length % 2 != 0)
        {
            throw new ArgumentException("State captures must come in pairs of (getter, setter)", nameof(stateCaptures));
        }

        for (var i = 0; i < stateCaptures.Length; i += 2)
        {
            if (stateCaptures[i] is not Delegate getter)
            {
                throw new ArgumentException($"Element at index {i} must be a Func<T> getter", nameof(stateCaptures));
            }

            if (stateCaptures[i + 1] is not Delegate setter)
            {
                throw new ArgumentException($"Element at index {i + 1} must be an Action<T> setter", nameof(stateCaptures));
            }

            // Capture the current value by invoking the getter
            var currentValue = getter.DynamicInvoke();

            // Store a restore action that will set the value back
            _restoreActions.Add(() => setter.DynamicInvoke(currentValue));
        }
    }

    /// <summary>
    /// Restores all captured static state to their original values.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var restoreAction in _restoreActions)
        {
            restoreAction();
        }

        _disposed = true;
    }
}
