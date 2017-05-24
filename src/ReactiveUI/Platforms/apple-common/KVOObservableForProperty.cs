using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using Splat;


namespace ReactiveUI
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
        static readonly MemoizingMRUCache<Tuple<Type, string>, bool> declaredInNSObject;

        static KVOObservableForProperty()
        {
            var monotouchAssemblyName = typeof(NSObject).Assembly.FullName;

            declaredInNSObject = new MemoizingMRUCache<Tuple<Type, string>, bool>((pair, _) => {
                var thisType = pair.Item1;

                // Types that aren't NSObjects at all are uninteresting to us
                if (typeof(NSObject).IsAssignableFrom(thisType) == false) {
                    return false;
                }

                while (thisType != null) {
                    if (thisType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Any(x => x.Name == pair.Item2)) {
                        // NB: This is a not-completely correct way to detect if
                        // an object is defined in an Obj-C class (it will fail if
                        // you're using a binding to a 3rd-party Obj-C library).
                        return thisType.Assembly.FullName == monotouchAssemblyName;
                    }

                    thisType = thisType.BaseType;
                }

                // The property doesn't exist at all
                return false;
            }, RxApp.BigCacheLimit);
        }


        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            lock (declaredInNSObject) {
                return declaredInNSObject.Get(Tuple.Create(type, propertyName)) ? 15 : 0;
            }
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var obj = sender as NSObject;
            if (obj == null) {
                throw new ArgumentException("Sender isn't an NSObject");
            }
            var propertyName = expression.GetMemberInfo().Name;

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var bobs = new BlockObserveValueDelegate((key, s, _) => {
                    subj.OnNext(new ObservedChange<object, object>(s, expression));
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

            var pi = senderType.GetTypeInfo().DeclaredProperties.FirstOrDefault(x => !x.IsStatic());
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

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            _block(keyPath, ofObject, change);
        }
    }
}
