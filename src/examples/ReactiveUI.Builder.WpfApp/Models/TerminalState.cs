// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Builder.WpfApp.Models;

/// <summary>The persisted terminal state: the journal of completed transactions.</summary>
public sealed class TerminalState
{
    /// <summary>Gets the journal of completed transactions, most recent first.</summary>
    public ObservableCollection<Transaction> Journal { get; } = [];
}
