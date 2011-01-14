using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.iOS;

namespace ReactiveUI.Sample.iOS
{
	// The name AppDelegateIPhone is referenced in the MainWindowIPhone.xib file.
	public partial class AppDelegateIPhone : UIApplicationDelegateRx
	{
		public AppDelegateIPhone()
		{
		}
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			RxApp.DeferredScheduler = new NSRunloopScheduler(app);
		
			window.AddSubview(navigationController.View);
			window.MakeKeyAndVisible();
			return true;
		}
	}
	
	public partial class PeopleViewController : UITableViewController
	{
		public PeopleViewController(IntPtr _) : base(_) {}
		public PeopleViewController(NSCoder _) : base(_) {}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.TableView.Source = new PeopleDataSource(this);
		}
		
		class PeopleDataSource : UITableViewSource
		{
			UITableViewController _controller;
			string[] _items;
			public PeopleDataSource(UITableViewController viewController)
			{
				_controller = viewController;
				_items = new[] {
					"Foo",
					"Bar",
					"Baz",
					"Bamf",
				};
			}
			
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return _items.Length;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var ret = new UITableViewCell();
				ret.TextLabel.Text = _items[indexPath.Row];
				ret.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
				
				return ret;	
			}
		}
	}
}
