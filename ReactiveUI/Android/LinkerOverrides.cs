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

namespace ReactiveUI.Android
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

            var rb = new RatingBar(null);
            rb.Rating = rb.Rating;

#if ANDROID_4
            
            var np = new NumberPicker(null);
            np.Value = np.Value;

            var cv = new CalendarView(null);
            cv.Date = cv.Date;
#endif
            var th = new TabHost(null);
            th.CurrentTab = th.CurrentTab;

            var tp = new TimePicker(null);
            tp.CurrentHour = tp.CurrentHour;
            tp.CurrentMinute = tp.CurrentMinute;

            
        }
    }
}