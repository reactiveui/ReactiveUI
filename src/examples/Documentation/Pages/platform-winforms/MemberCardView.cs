// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// The member card shown in <see cref="ViewModelControlHost"/>. A plain <see cref="ReactiveUserControl{TViewModel}"/>
/// is the natural shape here: the view locator's source generator finds it by its <c>IViewFor&lt;MemberCardViewModel&gt;</c>,
/// so <see cref="ViewModelControlHost"/> resolves it even though it only knows the view model as <see cref="object"/>.
/// </summary>
[DebuggerDisplay("MemberCardView ViewModel = {ViewModel}")]
public sealed class MemberCardView : ReactiveUserControl<MemberCardViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MemberCardView"/> class.</summary>
    public MemberCardView()
    {
        Controls.Add(Summary);

        _ = this.WhenActivated(disposables =>
        {
            _ = this.WhenAnyValue(static x => x.ViewModel)
                .Subscribe(UpdateSummary)
                .DisposeWith(disposables);
        });
    }

    /// <summary>Gets the label that shows the member's name, card number and current loan count.</summary>
    internal Label Summary { get; } = new() { AutoSize = true, Name = "MemberCardSummary" };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Summary.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Redraws <see cref="Summary"/> for the current view model, including when a cached view is reused.</summary>
    /// <param name="viewModel">The member card to show, or null while none is picked.</param>
    private void UpdateSummary(MemberCardViewModel? viewModel) =>
        Summary.Text = viewModel is null
            ? string.Empty
            : $"{viewModel.Member.Name} ({viewModel.Member.Id}) — {viewModel.LoanCount} on loan";
}
