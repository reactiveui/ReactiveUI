using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using System.Runtime.Serialization;
using Microsoft.Reactive.Testing;

namespace ReactiveUI.Serialization.Tests
{
#if !SILVERLIGHT && FALSE
    public static class PexTestUtils
    {
        public static IObservable<T> CreateColdPexObservable<T>(this TestScheduler scheduler, T[] items, int[] deltaTimes, bool failOnError = false)
        {
            var ret = createPexObservable(items, deltaTimes, failOnError);
            return scheduler.CreateColdObservable(ret);
        }

        public static IObservable<T> CreateHotPexObservable<T>(this TestScheduler scheduler, T[] items, int[] deltaTimes, bool failOnError = false)
        {
            var ret = createPexObservable(items, deltaTimes, failOnError);
            return scheduler.CreateHotObservable(ret);
        }

        static Recorded<Notification<T>>[] createPexObservable<T>(T[] items, int[] deltaTimes, bool failOnError)
        {
            Contract.Requires(items != null);
            Contract.Requires(deltaTimes != null);
            Contract.Requires(items.Length == deltaTimes.Length);

            var ret = new Recorded<Notification<T>>[items.Length + 1];

            items.Zip(deltaTimes.Scan(0, (acc, x) => acc + x), (item, time) =>
                new Recorded<Notification<T>>(time, new Notification<T>.OnNext(item))).ToArray().CopyTo(ret, 0);

            var finish = (failOnError ? (Notification<T>)new Notification<T>.OnError(new Exception("Fail")) : new Notification<T>.OnCompleted());
            ret[items.Length] = new Recorded<Notification<T>>(ret[items.Length - 1].Time + 10, finish);
            return ret;
        }
    }
#endif

    public static class UniqEnumerableMixin
    {
        public static IEnumerable<T> Uniq<T>(this IEnumerable<T> This)
        {
            bool prevIsSet = false;
            T prev = default(T);

            foreach(var v in This) {
                if (!prevIsSet || !EqualityComparer<T>.Default.Equals(prev, v)) {
                    yield return v;
                }

                prev = v;
                prevIsSet = true;
            }
        }
    }

    public static class TestEngineMixins
    {
        public static IDisposable AsPrimaryEngine(this IStorageEngine This)
        {
            var origEngine = RxStorage.Engine;
            RxStorage.InitializeWithEngine(This);

            return Disposable.Create(() => RxStorage.Engine = origEngine);
        }
    }

    public class RootSerializationTestObject : ModelBase
    {
        [IgnoreDataMember]
        public SubobjectTestObject _SubObject;
        public SubobjectTestObject SubObject {
            get { return _SubObject; }
            set { this.RaiseAndSetIfChanged(x => x.SubObject, value); }
        }

        [IgnoreDataMember]
        public int _SomeInteger;
        public int SomeInteger {
            get { return _SomeInteger;  }
            set { this.RaiseAndSetIfChanged(x => x.SomeInteger, value);  }
        }
    }

    public class SubobjectTestObject : ModelBase
    {
        [IgnoreDataMember]
        public string _SomeProperty;
        public string SomeProperty {
            get { return _SomeProperty;  }
            set { this.RaiseAndSetIfChanged(x => x.SomeProperty, value); }
        }
    }

    public class RootObjectWithAListTestObject : ModelBase
    {
        [IgnoreDataMember]
        public SerializedCollection<ISerializableItem> _SomeList;
        public SerializedCollection<ISerializableItem> SomeList {
            get { return _SomeList; }
            set { this.RaiseAndSetIfChanged(x => x.SomeList, value); }
        }

        [IgnoreDataMember]
        public RootSerializationTestObject _RootObject;
        public RootSerializationTestObject RootObject {
            get { return _RootObject;  }
            set { this.RaiseAndSetIfChanged(x => x.RootObject, value); }
        }
    }
}
