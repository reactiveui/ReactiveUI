using System;
using ReactiveUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveUI.Cocoa
{
	public abstract class UIKitObservableForPropertyBase :
		ICreatesObservableForProperty
	{
		#region ICreatesObservableForProperty implementation

		public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
		{
			if(beforeChanged)
				return 0;

			Dictionary<string, ObservablePropertyInfo> typeProperties;
			if(!config.TryGetValue(type, out typeProperties))
				return 0;

			ObservablePropertyInfo info;
			if(!typeProperties.TryGetValue(propertyName, out info))
				return 0;

			return info.Affinity;
		}

		public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
		{
			var type = sender.GetType();

			if(beforeChanged)
				return Observable.Never<IObservedChange<object, object>>();

			Dictionary<string, ObservablePropertyInfo> typeProperties;
			if(!config.TryGetValue(type, out typeProperties))
				throw new NotSupportedException(string.Format("Notifications for {0} are not supported", type.Name));

			ObservablePropertyInfo info;
			if(!typeProperties.TryGetValue(propertyName, out info))
				throw new NotSupportedException(string.Format("Notifications for {0}.{1} are not supported", type.Name, propertyName));

			return info.CreateObservable((NSObject) sender, propertyName);
		}

		#endregion

		internal class ObservablePropertyInfo
		{
			public int Affinity;
			public Func<NSObject, string, IObservable<IObservedChange<object, object>>> CreateObservable;
		}

		/// <summary>
		/// Configuration map
		/// </summary>
		readonly Dictionary<Type, Dictionary<string, ObservablePropertyInfo>> config =
			new Dictionary<Type, Dictionary<string, ObservablePropertyInfo>>();

		/// <summary>
		/// Registers an observable factory for the specified type and property.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="property">Property.</param>
		/// <param name="createObservable">Create observable.</param>
		protected void Register(Type type, string property, int affinity, Func<NSObject, string, IObservable<IObservedChange<object, object>>> createObservable)
		{
			Dictionary<string, ObservablePropertyInfo> typeProperties;
			if(!config.TryGetValue(type, out typeProperties))
			{
				typeProperties = new Dictionary<string, ObservablePropertyInfo>();
				config[type] = typeProperties;
			}

			var info = new ObservablePropertyInfo { Affinity = affinity, CreateObservable = createObservable };
			typeProperties[property] = info;
		}

		/// <summary>
		/// Creates an Observable for a UIControl Event
		/// </summary>
		/// <returns>An observable</returns>
		/// <param name="sender">The sender</param>
		/// <param name="propertyName">The property name </param>
		/// <param name="evt">The control event to listen for</param>
		protected static IObservable<IObservedChange<object, object>> ObservableFromUIControlEvent(NSObject sender, string propertyName, UIControlEvent evt)
		{
			return Observable.Create<IObservedChange<object, object>>(subj =>
			{
				var control = (UIControl) sender;

				EventHandler handler = (s,e)=>
				{
					subj.OnNext(new ObservedChange<object, object>() { Sender = sender, PropertyName = propertyName });
				};

				control.AddTarget(handler, evt);

				return Disposable.Create(() =>
				{
					control.RemoveTarget(handler, evt);
				});
			});
		}

		/// <summary>
		/// Creates an Observable for a NSNotificationCenter notification
		/// </summary>
		/// <returns>The from notification.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="notification">Notification.</param>
		protected static IObservable<IObservedChange<object, object>> ObservableFromNotification(NSObject sender, string propertyName, NSString notification)
		{
			return Observable.Create<IObservedChange<object, object>>(subj =>
			{
				var handle = NSNotificationCenter.DefaultCenter.AddObserver (notification, (e)=>
				{
					subj.OnNext(new ObservedChange<object, object>() { Sender = sender, PropertyName = propertyName });
				}, sender);

				return Disposable.Create(() =>
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(handle);
				});
			});
		}

		/// <summary>
		/// Creates an Observable for a NSNotificationCenter notification
		/// </summary>
		/// <returns>The from notification.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="notification">Notification.</param>
		protected static IObservable<IObservedChange<object, object>> ObservableFromEvent(NSObject sender, string propertyName, string eventName)
		{
			return Observable.Create<IObservedChange<object, object>>(subj =>
			{
				return Observable.FromEventPattern(sender, eventName).Subscribe((e) =>
				{
					subj.OnNext(new ObservedChange<object, object>() { Sender = sender, PropertyName = propertyName });
				});
			});
		}
	}
}

