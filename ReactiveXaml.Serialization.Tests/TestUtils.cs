using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Disposables;
using System.Linq;
using System.Reactive.Testing;
using System.Reactive.Testing.Mocks;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveXaml.Serialization.Tests
{
    public static class PexTestUtils
    {
        public static ColdObservable<T> CreateColdPexObservable<T>(this TestScheduler scheduler, T[] items, int[] deltaTimes, bool failOnError = false)
        {
            var ret = createPexObservable(items, deltaTimes, failOnError);
            return scheduler.CreateColdObservable(ret);
        }

        public static HotObservable<T> CreateHotPexObservable<T>(this TestScheduler scheduler, T[] items, int[] deltaTimes, bool failOnError = false)
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
        SubobjectTestObject _SubObject;
        public SubobjectTestObject SubObject {
            get { return _SubObject; }
            set { this.RaiseAndSetIfChanged(x => x.SubObject, value); }
        }
    }

    public class SubobjectTestObject : ModelBase
    {
        string _SomeProperty;
        public string SomeProperty {
            get { return _SomeProperty;  }
            set { this.RaiseAndSetIfChanged(x => x.SomeProperty, value); }
        }
    }

    public class RootObjectWithAListTestObject : ModelBase
    {
        SerializedCollection<ISerializableItem> _SomeList;
        public SerializedCollection<ISerializableItem> SomeList {
            get { return _SomeList; }
            set { this.RaiseAndSetIfChanged(x => x.SomeList, value); }
        }

        RootSerializationTestObject _RootObject;
        public RootSerializationTestObject RootObject {
            get { return _RootObject;  }
            set { this.RaiseAndSetIfChanged(x => x.RootObject, value); }
        }
    }
}
