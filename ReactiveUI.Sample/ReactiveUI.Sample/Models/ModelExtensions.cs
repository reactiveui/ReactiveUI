using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveUI.Sample.Models
{
    public static class BlockItemHelper
    {
        public static TimeSpan DurationOfPauses(this BlockItem This)
        {
            var ret = This.PauseList.Aggregate(TimeSpan.Zero, 
                (acc, x) => acc + (x.EndedAt - x.StartedAt));
            This.Log().Info("Pause duration: {0}", ret);
            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
