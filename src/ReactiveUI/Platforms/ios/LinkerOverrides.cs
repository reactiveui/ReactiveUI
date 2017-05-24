using System;
using UIKit;

namespace ReactiveUI.Cocoa
{
#if UIKIT
    /// <summary>
    /// This class exists to force the MT linker to include properties called by RxUI via reflection
    /// </summary>
    [Preserve(AllMembers = true)]
    class LinkerOverrides
    {
        public void KeepMe()
        {
            // UIButon
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
            tv.Text = tf.Text;

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

            EventHandler eh = (s, e) => { };
            ctl.TouchUpInside += eh;
            ctl.TouchUpInside -= eh;

            // UIRefreshControl
            var rc = new UIRefreshControl();
            rc.ValueChanged += eh;
            rc.ValueChanged -= eh;

            // UIBarButtonItem
            var bbi = new UIBarButtonItem();
            bbi.Clicked += eh;
            bbi.Clicked -= eh;

            eh.Invoke(null, null);
        }
    }
#endif
}