using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

#if UIKIT
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace ReactiveUI.Cocoa
{
    public class KVOObservableForProperty : ICreatesObservableForProperty
    {
        public KVOObservableForProperty ()
        {
        }
        
        public int GetAffinityForObject(Type type, bool beforeChanged = false)
        {
            // NB: Since every IRNPC is also an INPC, we need to bind more 
            // tightly than INPCObservableForProperty, so we return 10 here 
            // instead of one
            return typeof (NSObject).IsAssignableFrom(type) ? 10 : 0;
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
                
                obj.AddObserver(bobs, (NSString)propertyName, beforeChanged ? NSKeyValueObservingOptions.Old : NSKeyValueObservingOptions.New, IntPtr.Zero);
                return Disposable.Create(() => {
                    obj.RemoveObserver(bobs, (NSString) propertyName);
                });
            });
        }
    }
    
    internal class BlockObserveValueDelegate : NSObject
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
