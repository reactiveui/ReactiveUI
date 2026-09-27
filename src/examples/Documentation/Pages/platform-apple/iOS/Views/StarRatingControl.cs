// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A star-rating control wrapping a <see cref="UIStepper"/> that binds two-way to <see cref="Book.Rating"/>.</summary>
[System.Diagnostics.DebuggerDisplay("StarRatingControl {Stars}")]
public sealed class StarRatingControl : ReactiveControl<Book>
{
    /// <summary>The stepper this control wraps. UIKit has no built-in star control, so a stepper stands in for one.</summary>
    private readonly UIStepper _stepper = new() { MinimumValue = 0, MaximumValue = 5, TranslatesAutoresizingMaskIntoConstraints = false };

    /// <summary>Initializes a new instance of the <see cref="StarRatingControl"/> class.</summary>
    public StarRatingControl() => Initialize();

    /// <summary>Raised whenever <see cref="Stars"/> changes, so the binding layer's naming convention (a
    /// <c>StarsChanged</c> event for a <c>Stars</c> property) can drive a two-way <c>Bind</c>.</summary>
    public event EventHandler? StarsChanged;

    /// <summary>Gets or sets the rating, from 0 to 5.</summary>
    public int Stars
    {
        get => (int)_stepper.Value;
        set => _stepper.Value = value;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stepper.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Builds the stepper, wires its native event to <see cref="StarsChanged"/>, and binds it to <see cref="Book.Rating"/>.</summary>
    private void Initialize()
    {
        AddSubview(_stepper);
        NSLayoutConstraint.ActivateConstraints(
        [
            _stepper.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _stepper.TopAnchor.ConstraintEqualTo(TopAnchor),
            _stepper.BottomAnchor.ConstraintEqualTo(BottomAnchor),
        ]);
        _stepper.ValueChanged += (_, _) => StarsChanged?.Invoke(this, EventArgs.Empty);

        // A rating of 0 never fires a change notification on its own, so start silent rather than log a phantom change.
        using (SuppressChangeNotifications())
        {
            Stars = 0;
        }

        PropertyChanging += static (_, e) => Console.WriteLine($"StarRatingControl.{e.PropertyName} is changing.");
        PropertyChanged += (_, e) => Console.WriteLine($"StarRatingControl.{e.PropertyName} changed to {Stars}.");

        _ = this.WhenActivated(d =>
        {
            d(this.Bind(ViewModel, static vm => vm.Rating, static v => v.Stars));
            d(Changed.Subscribe(static change => Console.WriteLine($"StarRatingControl.{change.PropertyName} changed (Changed stream).")));
            d(Changing.Subscribe(static change => Console.WriteLine($"StarRatingControl.{change.PropertyName} is changing (Changing stream).")));
            d(ThrownExceptions.Subscribe(static error => Console.WriteLine($"StarRatingControl threw: {error.Message}")));
            d(Activated.Subscribe(static _ => Console.WriteLine("StarRatingControl activated.")));
            d(Deactivated.Subscribe(static _ => Console.WriteLine("StarRatingControl deactivated.")));
        });
    }
}
