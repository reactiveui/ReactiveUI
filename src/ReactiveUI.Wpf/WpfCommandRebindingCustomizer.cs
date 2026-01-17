// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI.Wpf;

/// <summary>
/// WPF-specific command rebinding customizer that updates the Command property directly
/// instead of doing a full rebind when only the command changes.
/// </summary>
/// <remarks>
/// <para>
/// This optimization is safe for WPF controls because they use the Command/CommandParameter
/// pattern. When the command instance changes but the control stays the same, we can simply
/// update the Command property without disposing and recreating the binding.
/// </para>
/// <para>
/// This avoids unnecessary overhead and prevents issues with CanExecuteChanged subscriptions
/// being torn down and recreated.
/// </para>
/// </remarks>
internal sealed class WpfCommandRebindingCustomizer : ICreatesCustomizedCommandRebinding
{
    /// <inheritdoc/>
    public bool TryUpdateCommand<TControl>(TControl? control, ICommand? command)
        where TControl : class
    {
        if (control is null)
        {
            return false;
        }

        // Try to get the Command property using reflection
        var commandProperty = control.GetType().GetProperty("Command");

        // If the control has a writable Command property, update it directly
        if (commandProperty is not null && commandProperty.CanWrite)
        {
            commandProperty.SetValue(control, command);
            return true;
        }

        // Fall back to full rebind if Command property doesn't exist or isn't writable
        return false;
    }
}
