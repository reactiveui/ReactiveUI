// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// Shows extending <see cref="IViewFor{T}"/> onto a base class this app does not control: <see cref="VendorDialog"/>
/// stands in for a base a third-party dialog library requires, and <see cref="BookReturnDialog"/> shows one built
/// on it. <see cref="VendorDialogActivationFetcher"/>, registered when the app starts, is what lets
/// <c>WhenActivated</c> find out when a <see cref="VendorDialog"/> opens and closes.
/// </summary>
public static class ExtendingIViewForExamples
{
    /// <summary>
    /// Setting <see cref="ReactiveVendorDialog{TViewModel}.ViewModel"/> binds the dialog's controls; showing and
    /// closing the dialog activates and deactivates them through <see cref="VendorDialogActivationFetcher"/>, the
    /// same way a platform's own activation fetcher drives its own base classes.
    /// </summary>
    public static void ConfirmABookReturnThroughAVendorDialog()
    {
        BookReturnViewModel viewModel = new("Clean Code");
        BookReturnDialog dialog = new() { ViewModel = viewModel };

        dialog.Show();
        Console.WriteLine(dialog.TitleLabel.Text);

        dialog.DamagedCheckBox.IsChecked = true;
        Console.WriteLine(viewModel.IsDamaged);

        dialog.Close();
        dialog.DamagedCheckBox.IsChecked = false;
        Console.WriteLine(viewModel.IsDamaged);

        // Output:
        // Clean Code
        // True
        // True
    }

    /// <summary>
    /// <see cref="ViewLocator.GetCurrent"/> finds <see cref="BookReturnDialog"/> for a <see cref="BookReturnViewModel"/>
    /// the same way it finds any other <see cref="IViewFor{T}"/>: implementing the interface is all a view needs,
    /// whatever it derives from.
    /// </summary>
    public static void LocateTheVendorDialogLikeAnyOtherView()
    {
        BookReturnViewModel viewModel = new("Refactoring");

        IViewFor? view = ViewLocator.GetCurrent().ResolveView(viewModel);

        Console.WriteLine(view?.GetType().Name);
        Console.WriteLine(ReferenceEquals(view?.ViewModel, viewModel));

        // Output:
        // BookReturnDialog
        // True
    }
}
