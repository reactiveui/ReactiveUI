using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveXaml;
using System.Disposables;

namespace ReactiveXaml.Serialization
{
    public interface ISerializableItemBase : IEnableLogger
    {
        Guid ContentHash { get; }

        IObservable<object> ItemChanging { get; }
        IObservable<object> ItemChanged { get; }

        Guid CalculateHash();
    }

    public interface ISerializableItem : IReactiveNotifyPropertyChanged, ISerializableItemBase { }

    public interface ISerializableList<T> : IReactiveCollection<T>, ISerializableItemBase
        where T : ISerializableItemBase
    {
        IDictionary<Guid, DateTimeOffset> CreatedOn { get; }
        IDictionary<Guid, DateTimeOffset> UpdatedOn { get; }
    }

    public interface ISyncPointInformation : ISerializableItemBase
    {
        Guid RootObjectHash { get; }
        Guid ParentSyncPoint { get; }
        string RootObjectTypeName { get; }

        string Qualifier { get; }
        DateTimeOffset CreatedOn { get; }
    }

    public interface IStorageEngine : IDisposable, IEnableLogger
    {
        T Load<T>(Guid contentHash) where T : ISerializableItemBase;
        object Load(Guid contentHash);
        void Save<T>(T obj) where T : ISerializableItemBase;
        void FlushChanges();

        ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItemBase;
        Guid[] GetOrderedRevisionList(Type type, string qualifier = null);
    }

    public interface IExtendedStorageEngine : IStorageEngine
    {
        T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItemBase;
        T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null)
            where T : ISerializableItemBase;
    }

    public interface IExplicitReferenceBase
    {
        Guid ValueHash { get; set; }
        IDisposable Update();
    }

    public static class SyncPointInformationMixin
    {
        static MemoizingMRUCache<string, Type> _ObjectNameCache = new MemoizingMRUCache<string, Type>(
            (x, _) => Type.GetType(x), 20);
        public static Type GetRootObjectType(this ISyncPointInformation This)
        {
            lock(_ObjectNameCache) {
                return _ObjectNameCache.Get(This.RootObjectTypeName);
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
