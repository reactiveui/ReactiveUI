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
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needed for linking.")]
    [SuppressMessage("Major Bug", "SST1189:Remove this self-assignment", Justification = "Deliberate self-assignment preserves both the getter and setter from the linker.")]
    public void KeepMe()
    {
        // UIButton
        var btn = new UIButton();
        var title = btn.Title(UIControlState.Disabled);
        btn.SetTitle("foo", UIControlState.Disabled);
        btn.TitleLabel.Text = btn.TitleLabel.Text;

        // Each control references both the setter (via the initializer) and the getter (via the
        // discard read) so the trimmer/linker preserves both accessors (resolved at runtime). A
        // self-assign (`x.Prop = x.Prop`) does the same but can't be written as an initializer.
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

        // UIBarButtonItem
        var bbi = new UIBarButtonItem();
        bbi.Clicked += Eh;
        bbi.Clicked -= Eh;

        Eh(null, EventArgs.Empty);
    }
}
