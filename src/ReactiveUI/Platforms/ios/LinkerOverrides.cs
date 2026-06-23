// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Cocoa;
#else
namespace ReactiveUI.Cocoa;
#endif
/// <summary>This class exists to force the MT linker to include properties called by RxUI via reflection.</summary>
[Preserve(AllMembers = true)]
[SuppressMessage("Major Code Smell", "S1656:Useless self-assignment", Justification = "Self-assignments force the linker to preserve these members.")]
[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "Self-assignments force the linker to preserve these members.")]
internal class LinkerOverrides
{
    /// <summary>Forces the linker to preserve UIKit members accessed by ReactiveUI via reflection.</summary>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by linker.")]
    [SuppressMessage("Major Bug", "SST1189:Remove this self-assignment", Justification = "Deliberate self-assignment preserves both the getter and setter from the linker.")]
    public void KeepMe()
    {
        // UIButton
        var btn = new UIButton();
        var title = btn.Title(UIControlState.Disabled);
        btn.SetTitle("foo", UIControlState.Disabled);
        btn.TitleLabel.Text = btn.TitleLabel.Text;

        // Each control below references both the setter and the getter of the property so the
        // trimmer/linker preserves both accessors (these members are resolved at runtime). The
        // initializer (`{ Prop = default }`) keeps the setter; the discard read (`_ = x.Prop`)
        // keeps the getter. A self-assign (`x.Prop = x.Prop`) would do the same but can't be
        // expressed as an initializer, so it is split here to keep the get + set intent explicit.
        // UISlider
        var slider = new UISlider { Value = default };
        _ = slider.Value;

        // UITextView
        var tv = new UITextView { Text = default };
        _ = tv.Text;

        // UITextField
        var tf = new UITextField { Text = default };
        _ = tf.Text;

        // UIImageView
        var iv = new UIImageView { Image = default };
        _ = iv.Image;

        // UI Label
        var lbl = new UILabel { Text = default };
        _ = lbl.Text;

        // UI Control
        var ctl = new UIControl { Enabled = default, Selected = default };
        _ = ctl.Enabled;
        _ = ctl.Selected;

        static void Eh(object? s, EventArgs e)
        {
            // Intentionally empty: the handler exists only to force the linker to preserve
            // the event add/remove accessors; no runtime behavior is needed.
        }

        ctl.TouchUpInside += Eh;
        ctl.TouchUpInside -= Eh;

        // UIRefreshControl
        var rc = new UIRefreshControl();
        rc.ValueChanged += Eh;
        rc.ValueChanged -= Eh;

        // UIBarButtonItem
        var bbi = new UIBarButtonItem();
        bbi.Clicked += Eh;
        bbi.Clicked -= Eh;

        // UISwitch
        var sw = new UISwitch();
        sw.ValueChanged += Eh;
        sw.On = true;

        Eh(null, EventArgs.Empty);
    }
}
