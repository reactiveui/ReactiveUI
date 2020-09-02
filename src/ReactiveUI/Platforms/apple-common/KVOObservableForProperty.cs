// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
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
    /// only have to hope for the best :-/.
    /// </summary>
    public class KVOObservableForProperty : ICreatesObservableForProperty
    {
        private static readonly MemoizingMRUCache<(Type type, string propertyName), bool> declaredInNSObject;

        static KVOObservableForProperty()
        {
            var monotouchAssemblyName = typeof(NSObject).Assembly.FullName;

            declaredInNSObject = new MemoizingMRUCache<(Type type, string propertyName), bool>(
                (pair, _) =>
            {
                var thisType = pair.type;

                // Types that aren't NSObjects at all are uninteresting to us
                if (typeof(NSObject).IsAssignableFrom(thisType) == false)
                {
                    return false;
                }

                while (thisType != null)
                {
                    if (thisType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Any(x => x.Name == pair.propertyName))
                    {
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

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            return declaredInNSObject.Get((type, propertyName)) ? 15 : 0;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object>>? GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            var obj = sender as NSObject;
            if (obj == null)
            {
                throw new ArgumentException("Sender isn't an NSObject");
            }

            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var bobs = new BlockObserveValueDelegate((key, s, _) =>
                {
                    subj.OnNext(new ObservedChange<object, object>(s, expression));
                });
                var pin = GCHandle.Alloc(bobs);

                var keyPath = (NSString)FindCocoaNameFromNetName(sender.GetType(), propertyName);

                obj.AddObserver(bobs, keyPath, beforeChanged ? NSKeyValueObservingOptions.Old : NSKeyValueObservingOptions.New, IntPtr.Zero);

                return Disposable.Create(() =>
                {
                    obj.RemoveObserver(bobs, keyPath);
                    pin.Free();
                });
            });
        }

        private static string FindCocoaNameFromNetName(Type senderType, string propertyName)
        {
            bool propIsBoolean = false;

            var pi = senderType.GetTypeInfo().DeclaredProperties.FirstOrDefault(x => !x.IsStatic());
            if (pi == null)
            {
                goto attemptGuess;
            }

            if (pi.DeclaringType == typeof(bool))
            {
                propIsBoolean = true;
            }

            var mi = pi.GetGetMethod();
            if (mi == null)
            {
                goto attemptGuess;
            }

            var attr = mi.GetCustomAttributes(true).Select(x => x as ExportAttribute).FirstOrDefault(x => x != null);
            if (attr == null)
            {
                goto attemptGuess;
            }

            return attr.Selector;

        attemptGuess:
            if (propIsBoolean)
            {
                propertyName = "Is" + propertyName;
            }

            return char.ToLowerInvariant(propertyName[0]).ToString(CultureInfo.InvariantCulture) + propertyName.Substring(1);
        }
    }
}
