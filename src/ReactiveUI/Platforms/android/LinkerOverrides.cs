using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ReactiveUI
{
    [Preserve(AllMembers = true)]
    class LinkerOverrides
    {
        void KeepMe()
        {
            var txt = new TextView(null);
            txt.Text = txt.Text;

            var iv = new ImageView(null);
            var obj = iv.Drawable;

            var prog = new ProgressBar(null);
            prog.Progress = prog.Progress;

            var cb = new RadioButton(null);
            cb.Checked = cb.Checked;

            var np = new NumberPicker(null);
            np.Value = np.Value;

            var rb = new RatingBar(null);
            rb.Rating = rb.Rating;

            var cv = new CalendarView(null);
            cv.Date = cv.Date;

            var th = new TabHost(null);
            th.CurrentTab = th.CurrentTab;

            var tp = new TimePicker(null);
            tp.Hour = tp.Hour;
            tp.Minute = tp.Minute;

            
        }
    }
}