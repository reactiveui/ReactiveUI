using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;
using System.Runtime.InteropServices;

#if UIKIT
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// This class provides notifications for Cocoa Framework objects based on
    /// Key-Value Observing. Unfortunately, this class is a bit Tricky™, because
    /// of the caveat mentioned below - there is no way up-front to be able to
    /// tell whether a given property on an object is Key-Value Observable, we 
    /// only have to hope for the best :-/
    /// </summary>
    public class KVOObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            // NB: There is no way to know up-front whether a given property is
            // KVO-observable. This is Unfortunate™.
            return typeof (NSObject).IsAssignableFrom(type) ? 4 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var obj = sender as NSObject;
            if (obj == null) {
                throw new ArgumentException("Sender isn't an NSObject");
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var bobs = new BlockObserveValueDelegate((key,s,_) => {
                    subj.OnNext(new ObservedChange<object, object>() { Sender = s, PropertyName = propertyName });
                });
                var pin = GCHandle.Alloc(bobs);

                var keyPath = (NSString)findCocoaNameFromNetName(sender.GetType(), propertyName);

                obj.AddObserver(bobs, keyPath, beforeChanged ? NSKeyValueObservingOptions.Old : NSKeyValueObservingOptions.New, IntPtr.Zero);

                return Disposable.Create(() => {
                    obj.RemoveObserver(bobs, keyPath);
                    pin.Free();
                });
            });
        }

        string findCocoaNameFromNetName(Type senderType, string propertyName)
        {
            bool propIsBoolean = false;

            var pi = Reflection.GetSafeProperty(senderType, propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (pi == null) goto attemptGuess;

            if (pi.DeclaringType == typeof(bool)) propIsBoolean = true;

            var mi = pi.GetGetMethod();
            if (mi == null) goto attemptGuess;

            var attr = mi.GetCustomAttributes(true).Select(x => x as ExportAttribute).FirstOrDefault(x => x != null);
            if (attr == null) goto attemptGuess;
            return attr.Selector;

        attemptGuess:
            if (propIsBoolean) propertyName = "Is" + propertyName;
            return Char.ToLowerInvariant(propertyName[0]).ToString() + propertyName.Substring(1);
        }
    }
    
    class BlockObserveValueDelegate : NSObject
    {
        Action<string, NSObject, NSDictionary> _block;
        public BlockObserveValueDelegate(Action<string, NSObject, NSDictionary> block)
        {
            _block = block;
        }
    
        public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            _block(keyPath, ofObject, change);
        }
    }
}
