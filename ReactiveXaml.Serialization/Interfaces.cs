using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveXaml.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISerializableItem : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        Guid ContentHash { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Guid CalculateHash();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISerializableList : IEnumerable, ISerializableItem
    {
        /// <summary>
        /// 
        /// </summary>
        IDictionary<Guid, DateTimeOffset> CreatedOn { get; }

        /// <summary>
        /// 
        /// </summary>
        IDictionary<Guid, DateTimeOffset> UpdatedOn { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Type GetBaseListType();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializableList<T> : IReactiveCollection<T>, ISerializableList
        where T : ISerializableItem
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISyncPointInformation : ISerializableItem
    {
        /// <summary>
        /// 
        /// </summary>
        Guid RootObjectHash { get; }

        /// <summary>
        /// 
        /// </summary>
        Guid ParentSyncPoint { get; }

        /// <summary>
        /// 
        /// </summary>
        string RootObjectTypeName { get; }

        /// <summary>
        /// 
        /// </summary>
        string Qualifier { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTimeOffset CreatedOn { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public interface IStorageEngine : IDisposable, IEnableLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentHash"></param>
        /// <returns></returns>
        T Load<T>(Guid contentHash) where T : ISerializableItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentHash"></param>
        /// <returns></returns>
        object Load(Guid contentHash);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        void Save<T>(T obj) where T : ISerializableItem;

        /// <summary>
        /// 
        /// </summary>
        void FlushChanges();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Guid[] GetAllObjectHashes();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetObjectCount();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="qualifier"></param>
        /// <param name="createdOn"></param>
        /// <returns></returns>
        ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        Guid[] GetOrderedRevisionList(Type type, string qualifier = null);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IExtendedStorageEngine : IStorageEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qualifier"></param>
        /// <param name="olderThan"></param>
        /// <returns></returns>
        T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItem;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qualifier"></param>
        /// <param name="olderThan"></param>
        /// <param name="newerThan"></param>
        /// <returns></returns>
        T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null)
            where T : ISerializableItem;
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IObjectSerializationProvider : IEnableLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(byte[] data, Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string SerializedDataToString(byte[] data);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IExplicitReferenceBase
    {
        /// <summary>
        /// 
        /// </summary>
        Guid ValueHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDisposable Update();
    }

    public static class SyncPointInformationMixin
    {
        static MemoizingMRUCache<string, Type> _ObjectNameCache = new MemoizingMRUCache<string, Type>(
            (x, _) => Type.GetType(x), 20);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static Type GetRootObjectType(this ISyncPointInformation This)
        {
            lock(_ObjectNameCache) {
                return _ObjectNameCache.Get(This.RootObjectTypeName);
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :