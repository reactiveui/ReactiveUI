// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>UIKit platform registrations.</summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    [DynamicDependency(nameof(UIButton.Title), typeof(UIButton))]
    [DynamicDependency(nameof(UIButton.SetTitle), typeof(UIButton))]
    [DynamicDependency(nameof(UIButton.TitleLabel), typeof(UIButton))]
    [DynamicDependency(nameof(UILabel.Text), typeof(UILabel))]
    [DynamicDependency(nameof(UITextView.Text), typeof(UITextView))]
    [DynamicDependency(nameof(UITextField.Text), typeof(UITextField))]
    [DynamicDependency(nameof(UIImageView.Image), typeof(UIImageView))]
    [DynamicDependency(nameof(UIControl.Enabled), typeof(UIControl))]
    [DynamicDependency(nameof(UIControl.Selected), typeof(UIControl))]
    [DynamicDependency(nameof(UIControl.TouchUpInside), typeof(UIControl))]
    [DynamicDependency(nameof(UIBarButtonItem.Clicked), typeof(UIBarButtonItem))]
#if IOS || MACCATALYST
    [DynamicDependency(nameof(UISlider.Value), typeof(UISlider))]
    [DynamicDependency(nameof(UIControl.ValueChanged), typeof(UIControl))]
    [DynamicDependency(nameof(UISwitch.On), typeof(UISwitch))]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Register(IRegistrar registrar) =>
        ApplePlatformRegistrations.Register(registrar);
}
