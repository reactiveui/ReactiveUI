// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

using Foundation;

namespace ReactiveUI;

/// <summary>
/// This class provides notifications for Cocoa Framework objects based on
/// Key-Value Observing. Unfortunately, this class is a bit Tricky™, because
/// of the caveat mentioned below - there is no way up-front to be able to
/// tell whether a given property on an object is Key-Value Observable, we
/// only have to hope for the best :-/.
/// </summary>
[Preserve(AllMembers = true)]
[RequiresUnreferencedCode("KVOObservableForProperty uses methods that may require unreferenced code")]
public class KVOObservableForProperty : ICreatesObservableForProperty
{
    private static readonly MemoizingMRUCache<(Type type, string propertyName), bool> _declaredInNSObject;

    static KVOObservableForProperty()
    {
        var monotouchAssemblyName = typeof(NSObject).Assembly.FullName;

        _declaredInNSObject = new MemoizingMRUCache<(Type type, string propertyName), bool>(
         (pair, _) =>
         {
             var thisType = pair.type;

             // Types that aren't NSObjects at all are uninteresting to us
             if (!typeof(NSObject).IsAssignableFrom(thisType))
             {
                 return false;
             }

             while (thisType is not null)
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
         },
         RxApp.BigCacheLimit);
    }

    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false) =>
        _declaredInNSObject.Get((type, propertyName)) ? 15 : 0;

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        if (sender is not NSObject obj)
        {
            throw new ArgumentException("Sender isn't an NSObject");
        }

        return Observable.Create<IObservedChange<object, object?>>(subj =>
        {
            var bobs = new BlockObserveValueDelegate((__, s, _) =>
                                                         subj.OnNext(new ObservedChange<object, object?>(s, expression, default)));

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

    private static string FindCocoaNameFromNetName([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type senderType, string propertyName)
    {
        var propIsBoolean = false;

        var pi = senderType.GetTypeInfo().DeclaredProperties.FirstOrDefault(static x => !x.IsStatic());
        if (pi is null)
        {
            goto attemptGuess;
        }

        if (pi.DeclaringType == typeof(bool))
        {
            propIsBoolean = true;
        }

        var mi = pi.GetGetMethod();
        if (mi is null)
        {
            goto attemptGuess;
        }

        var attr = mi.GetCustomAttributes(true).OfType<ExportAttribute?>().FirstOrDefault();
        if (attr is null)
        {
            goto attemptGuess;
        }

        if (attr.Selector is not null)
        {
            return attr.Selector;
        }

    attemptGuess:
        if (propIsBoolean)
        {
            propertyName = "Is" + propertyName;
        }

        return string.Concat(char.ToLowerInvariant(propertyName[0]).ToString(CultureInfo.InvariantCulture), propertyName.AsSpan(1));
    }
}
