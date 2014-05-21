using System;
using System.Linq.Expressions;
using System.Reflection;
using Android.Views;

namespace ReactiveUI.Android
{
    public class AndroidCommandBinders : FlexibleCommandBinder
    {
        public static Lazy<AndroidCommandBinders> Instance = new Lazy<AndroidCommandBinders>();

        public AndroidCommandBinders()
        {
            Type view = typeof(View);
            Expression enabledExporession = Expression.MakeMemberAccess(Expression.Parameter(view), view.GetRuntimeProperty("Enabled"));
            Register(view, 9, (cmd, t, cp)=> ForEvent(cmd, t, cp, "Click", enabledExporession));
        }
    }
}