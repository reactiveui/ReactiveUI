// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Android platform registrations.</summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    [DynamicDependency(nameof(TextView.Text), typeof(TextView))]
    [DynamicDependency(nameof(ImageView.Drawable), typeof(ImageView))]
    [DynamicDependency(nameof(ProgressBar.Progress), typeof(ProgressBar))]
    [DynamicDependency(nameof(CompoundButton.Checked), typeof(CompoundButton))]
    [DynamicDependency(nameof(NumberPicker.Value), typeof(NumberPicker))]
    [DynamicDependency(nameof(RatingBar.Rating), typeof(RatingBar))]
    [DynamicDependency(nameof(CalendarView.Date), typeof(CalendarView))]
    [DynamicDependency(nameof(TabHost.CurrentTab), typeof(TabHost))]
    [DynamicDependency(nameof(TimePicker.Hour), typeof(TimePicker))]
    [DynamicDependency(nameof(TimePicker.Minute), typeof(TimePicker))]
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = TaskPoolSequencer.Default;
            RxSchedulers.MainThreadScheduler = HandlerSequencer.Main;
        }

        registrar.RegisterConstant<ISuspensionDriver>(static () => new BundleSuspensionDriver());
    }
}
