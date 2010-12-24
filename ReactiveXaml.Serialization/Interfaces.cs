using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveXaml.Serialization
{
    public interface ISerializableItem : IReactiveNotifyPropertyChanged
    {
        Guid ContentHash { get; }
        Guid CalculateHash();
    }

    public interface ISerializableList : IEnumerable, ISerializableItem
    {
        IDictionary<Guid, DateTimeOffset> CreatedOn { get; }
        IDictionary<Guid, DateTimeOffset> UpdatedOn { get; }
        Type GetBaseListType();
    }

    public interface ISerializableList<T> : IReactiveCollection<T>, ISerializableList
        where T : ISerializableItem
    {
    }

    public interface ISyncPointInformation : ISerializableItem
    {
        Guid RootObjectHash { get; }
        Guid ParentSyncPoint { get; }
        string RootObjectTypeName { get; }

        string Qualifier { get; }
        DateTimeOffset CreatedOn { get; }
    }

    public interface IStorageEngine : IDisposable, IEnableLogger
    {
        T Load<T>(Guid contentHash) where T : ISerializableItem;
        object Load(Guid contentHash);
        void Save<T>(T obj) where T : ISerializableItem;
        void FlushChanges();

        Guid[] GetAllObjectHashes();
        int GetObjectCount();

        ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem;
        Guid[] GetOrderedRevisionList(Type type, string qualifier = null);
    }

    public interface IExtendedStorageEngine : IStorageEngine
    {
        T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItem;
        T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null)
            where T : ISerializableItem;
    }

    public interface IObjectSerializationProvider : IEnableLogger
    {
        byte[] Serialize(object obj);
        object Deserialize(byte[] data, Type type);
        string SerializedDataToString(byte[] data);
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

    public static class ObjectSerializationProviderMixin
    {
        public static T Clone<T>(this IObjectSerializationProvider This, T obj)
        {
            return (T)This.Deserialize(This.Serialize(obj), typeof(T));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :