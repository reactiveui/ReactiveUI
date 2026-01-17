// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Optional interface that platforms can register to customize command rebinding behavior.
/// This allows platforms to optimize how commands are updated when the command instance
/// changes but the control remains the same.
/// </summary>
/// <remarks>
/// <para>
/// Platforms like WPF can implement this to update the Command property directly
/// instead of doing a full rebind (dispose old binding + create new binding), which
/// is more efficient and avoids unnecessary event handler churn.
/// </para>
/// <para>
/// If no implementation is registered, <see cref="CommandBinderImplementation"/> will
/// always perform full rebinding, which works correctly for all platforms.
/// </para>
/// </remarks>
public interface ICreatesCustomizedCommandRebinding
{
    /// <summary>
    /// Attempts to update the command on a control without full rebinding.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="control">The control instance.</param>
    /// <param name="command">The new command to set.</param>
    /// <returns>
    /// <see langword="true"/> if the command was updated successfully;
    /// <see langword="false"/> if full rebinding is required.
    /// </returns>
    bool TryUpdateCommand<TControl>(TControl? control, ICommand? command)
        where TControl : class;
}
