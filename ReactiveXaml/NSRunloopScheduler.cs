using System;
using System.Linq;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace ReactiveXaml
{
	public class NSRunloopScheduler : IScheduler
	{
		NSObject theApp;
		
#if IOS
		public NSRunloopScheduler (UIApplication app)
		{
			theApp = app;
		}
#else
		public NSRunloopScheduler (NSApplication app)
		{
			theApp = app;
		}
#endif
		
		public DateTimeOffset Now {
			get { return DateTimeOffset.Now; }
		}
		
		public IDisposable Schedule(Action action)
		{
			theApp.BeginInvokeOnMainThread(new NSAction(action));
			return Disposable.Empty;
		}
		
		public IDisposable Schedule(Action action, TimeSpan dueTime)
		{
			var timer = NSTimer.CreateScheduledTimer(dueTime, new NSAction(action));
			return Disposable.Create(() => timer.Invalidate());
		}
	}
}

