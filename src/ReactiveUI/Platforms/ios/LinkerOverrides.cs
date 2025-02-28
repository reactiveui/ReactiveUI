// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Cocoa;

/// <summary>
/// This class exists to force the MT linker to include properties called by RxUI via reflection.
/// </summary>
[Preserve(AllMembers = true)]
internal class LinkerOverrides
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by linker.")]
    public void KeepMe()
    {
        // UIButton
        var btn = new UIButton();
        var title = btn.Title(UIControlState.Disabled);
        btn.SetTitle("foo", UIControlState.Disabled);
        btn.TitleLabel.Text = btn.TitleLabel.Text;

        // UISlider
        var slider = new UISlider();
        slider.Value = slider.Value; // Get and set

        // UITextView
        var tv = new UITextView();
        tv.Text = tv.Text;

        // UITextField
        var tf = new UITextField();
        tf.Text = tf.Text;

        // var UIImageView
        var iv = new UIImageView();
        iv.Image = iv.Image;

        // UI Label
        var lbl = new UILabel();
        lbl.Text = lbl.Text;

        // UI Control
        var ctl = new UIControl();
        ctl.Enabled = ctl.Enabled;
        ctl.Selected = ctl.Selected;

        static void Eh(object? s, EventArgs e)
        {
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
