using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Serialization
{
    /// <summary>
    /// ISerializableItem represents any object that can be serialized via a
    /// Sync Point, either directly or via a property in the object graph. Its
    /// core feature is that it can return a unique hash representing its
    /// content.
    /// </summary>
    public interface ISerializableItem : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// A hash representing the content of the object. Note that this should
        /// be implemented using a cache-pattern, calling CalculateHash as
        /// needed. This property *cannot* be Guid.Empty.
        /// </summary>
        Guid ContentHash { get; }

        /// <summary>
        /// Explicitly recalculates the value of ContentHash - note that
        /// similarly to CalculateHash, this cannot return Guid.Empty (a
        /// suggested solution is to return the MD5 hash of GetType().FullName
        /// if an object has no hashable data)
        /// </summary>
        /// <returns>The hash of the content.</returns>
        Guid CalculateHash();
    }

    /// <summary>
    /// ISerializableList represents a list of serializable objects, which can
    /// be serialized as their hashes instead of the actual list items. 
    ///
    /// This interface exists mainly for the framework, and ISerializableList of
    /// T should be used instead.
    /// </summary>
    public interface ISerializableList : IList, ISerializableItem, IReactiveCollection
    {
        /// <summary>
        /// CreatedOn returns the date each item was added to the list.
        /// </summary>
        IDictionary<Guid, DateTimeOffset> CreatedOn { get; }

        /// <summary>
        /// XXX: YOU SHOULD PROBABLY REMOVE THIS
        /// </summary>
        IDictionary<Guid, DateTimeOffset> UpdatedOn { get; }

        /// <summary>
        /// GetBaseListType returns the Type of all the items in the list - if
        /// the list is heterogenous, return typeof(object).
        /// </summary>
        /// <returns>The Type of all the items in the list.</returns>
        Type GetBaseListType();
    }

    /// <summary>
    /// ISerializableList of T represents a list of serializable objects, which can
    /// be serialized as their hashes instead of the actual list items.
    /// </summary>
    public interface ISerializableList<T> : IReactiveCollection<T>, ISerializableList
        where T : ISerializableItem
    {
    }

    /// <summary>
    /// ISyncPointInformation represents a Sync Point, a commit to the
    /// serialization store where we will record the object state. Override this
    /// only if you want to commit custom information with a sync point in an
    /// Engine implementation, otherwise use SyncPointInformation.
    /// </summary>
    public interface ISyncPointInformation : ISerializableItem
    {
        /// <summary>
        /// The content hash of the root object serialized in this sync point.
        /// </summary>
        Guid RootObjectHash { get; }

        /// <summary>
        /// The content hash of the parent ISyncPointInformation object, or
        /// Guid.Empty if this is the initial commit.
        /// </summary>
        Guid ParentSyncPoint { get; }

        /// <summary>
        /// The full type name (i.e. Type.FullName) of the root object.
        /// </summary>
        string RootObjectTypeName { get; }

        /// <summary>
        /// An optional string to create separate sync point lists (i.e.
        /// "branches") - this is a string provided by the application or null.
        /// </summary>
        string Qualifier { get; }

        /// <summary>
        /// The date that the sync point was created.
        /// </summary>
        DateTimeOffset CreatedOn { get; }
    }

    /// <summary>
    /// IStorageEngine is the core interface for classes that can maintain and
    /// persist list of sync points. Its core responsibilities are to quickly
    /// Load and Save individual objects, and create and list Sync Points
    /// (objects that represent an atomic commit to the Storage engine)
    /// </summary>
    public interface IStorageEngine : IDisposable
    {
        /// <summary>
        /// Loads an object given its Content Hash. Note that it is critical
        /// that separate calls to Load with the same ContentHash return two
        /// distinct in-memory copies of the object - i.e. maintaining an
        /// "object cache" will result in object corruption. 
        /// </summary>
        /// <param name="contentHash">The hash of the object to load.</param>
        /// <returns>The deserialized object, or null if the object is not
        /// present.</returns>
        T Load<T>(Guid contentHash) where T : ISerializableItem;

        /// <summary>
        /// Loads an object given its Content Hash. Note that it is critical
        /// that separate calls to Load with the same ContentHash return two
        /// distinct in-memory copies of the object - i.e. maintaining an
        /// "object cache" will result in object corruption. 
        /// </summary>
        /// <param name="contentHash">The hash of the object to load.</param>
        /// <returns>The deserialized object, or null if the object is not
        /// present.</returns>
        object Load(Guid contentHash);

        /// <summary>
        /// Saves an object to the persistence engine. Note that this does *not*
        /// imply that the object is immediately written to the backing store,
        /// the engine is free to postpone serialization.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        void Save<T>(T obj) where T : ISerializableItem;

        /// <summary>
        /// When called, guarantees all objects are serialized to a persistent
        /// store.
        /// </summary>
        void FlushChanges();

        /// <summary>
        /// Returns the content hash of all objects in the store.
        /// </summary>
        /// <returns></returns>
        Guid[] GetAllObjectHashes();

        /// <summary>
        /// Returns the count of available objects.
        /// </summary>
        /// <returns>The count of available objects.</returns>
        int GetObjectCount();

        /// <summary>
        /// Creates a Sync Point using the specified root object and qualifier
        /// and persists it to the engine. This sync point will be attached to
        /// the parent sync point for this type and qualifier.
        /// </summary>
        /// <param name="obj">The Root Object to attach to this sync point.</param>
        /// <param name="qualifier">An optional string to create separate sync
        /// point lists (i.e. "branches") - this is a string provided by the
        /// application or null.</param>
        /// <param name="createdOn">The date that the sync point was created, or
        /// null to use the current time.</param>
        /// <returns>A reference to the newly created sync point</returns>
        ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem;

        /// <summary>
        /// GetOrderedRevisionList returns the list of Sync Points commited for
        /// the given root object Type and qualifier.
        /// </summary>
        /// <param name="type">The Type of the root object associated with the
        /// Sync Point.</param>
        /// <param name="qualifier">An optional string to create separate sync
        /// point lists (i.e. "branches") - this is a string provided by the
        /// application or null.</param>
        /// <returns>A list of content hashes, in the order that they were
        /// committed. Note that this does *not* necessarily correlate to the
        /// CreatedOn date since it can be explicitly set.</returns>
        Guid[] GetOrderedRevisionList(Type type, string qualifier = null);
    }

    /// <summary>
    /// IExtendedStorageEngine provides additional methods that would most
    /// likely be needed by applications. If this interface is not explicitly
    /// implemented in storage implementations, a naive low performance
    /// implementation that uses the IStorageEngine methods will be used.
    /// </summary>
    public interface IExtendedStorageEngine : IStorageEngine
    {
        /// <summary>
        /// Load the root object associated with the the latest Sync Point for
        /// the given type and qualifier whose CreatedAt date is older than the
        /// given date.
        /// </summary>
        /// <param name="qualifier">An optional string to create separate sync
        /// point lists (i.e. "branches") - this is a string provided by the
        /// application or null.</param>
        /// <param name="olderThan">An optional paramter that requires the
        /// return value to be older than the given date. If no date is given,
        /// the newest root object is returned.</param>
        /// <returns>The root object from the latest Sync Point that satisfies
        /// the given constraints.</returns>
        T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItem;

        /// <summary>
        /// Loads the root objects whose CreatedAt date fall within the given
        /// date ranges.
        /// </summary>
        /// <param name="qualifier">An optional string to create separate sync
        /// point lists (i.e. "branches") - this is a string provided by the
        /// application or null.</param>
        /// <param name="olderThan">An optional paramter that requires the
        /// return values to be older than the given date. If no date is given,
        /// this constraint is ignored.</param>
        /// <param name="newerThan">An optional paramter that requires the
        /// return values to be newer than the given date. If no date is given,
        /// this constraint is ignored.</param>
        /// <returns>All root objects whose Sync Points satisfy the given
        /// constraints, or an empty array if none of them qualify.</returns>
        T[] GetRootObjectsInDateRange<T>(
                string qualifier = null, 
                DateTimeOffset? olderThan = null, 
                DateTimeOffset? newerThan = null)
            where T : ISerializableItem;
    }

    /// <summary>
    /// IObjectSerializationProvider decouples the task of serialization (i.e.
    /// reducing an object to its on-disk form) from persistence (i.e.
    /// reading/writing the on-disk form to persistent storage). 
    ///
    /// Most engine implementations will use JsonNetObjectSerializationProvider,
    /// but should take a constructor parameter to allow this to be pluggable.
    ///
    /// Note that this interface is the interface that does the "Git-like"
    /// magic; that is, deconstructing objects into their pieces and instead
    /// serializing their content hashes.
    /// </summary>
    public interface IObjectSerializationProvider
    {
        /// <summary>
        /// Write an object to memory, including serializing all of the child
        /// objects in the object graph. This object often needs a reference to
        /// the IStorageEngine, to be able to ensure sub-objects are already
        /// serialized.
        /// </summary>
        /// <param name="obj">The root object to serialize to disk.</param>
        /// <returns>A byte representation of the object.</returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// Reads an object from the data returned by Serialize.
        /// </summary>
        /// <param name="data">The byte representation of the object.</param>
        /// <param name="type">The type of the object to reconstruct.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(byte[] data, Type type);

        /// <summary>
        /// SerializedDataToString is a method used for debugging purposes to
        /// dump a serialized object out as a string. Production
        /// implementations are free to return an empty string.
        /// </summary>
        /// <param name="data">A serialized object to examine.</param>
        /// <returns>The string representation of the byte data (i.e. the JSON
        /// string).</returns>
        string SerializedDataToString(byte[] data);
    }

    public static class SyncPointInformationMixin
    {
        static MemoizingMRUCache<string, Type> _ObjectNameCache = new MemoizingMRUCache<string, Type>(
            (x, _) => Type.GetType(x), 20);

        /// <summary>
        /// GetRootObjectType returns the Type of the root object associated with the given Sync
        /// Point.
        /// </summary>
        /// <returns>The Type of the root object.</returns>
        public static Type GetRootObjectType(this ISyncPointInformation This)
        {
            lock(_ObjectNameCache) {
                return _ObjectNameCache.Get(This.RootObjectTypeName);
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
