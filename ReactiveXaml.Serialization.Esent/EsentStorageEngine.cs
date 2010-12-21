using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Collections.Generic;

namespace ReactiveXaml.Serialization.Esent
{
    public class EsentPersistedMetadata
    {
        public Dictionary<string, Guid> SyncPointIndex { get; set; }
        public Dictionary<Guid, string> ItemTypeNames { get; set; }
    }

    public class EsentStorageEngine : IExtendedStorageEngine
    {
        PersistentDictionary<Guid, byte[]> _backingStore;
        Dictionary<Guid, string> _itemTypeNames;
        Dictionary<string, Guid> _syncPointIndex;

        static readonly Lazy<IEnumerable<Type>> allStorageTypes = new Lazy<IEnumerable<Type>>(
            () => Utility.GetAllTypesImplementingInterface(typeof(ISerializableItem)).ToArray());

        Func<object, DataContractSerializationProvider> serializerFactory;

        public EsentStorageEngine(string databasePath)
        {
            _backingStore = new PersistentDictionary<Guid, byte[]>(databasePath);
        }

        public T Load<T>(Guid contentHash) where T : ISerializableItem {
            return (T)Load(contentHash);
        }

        public object Load(Guid contentHash) 
        {
            byte[] data;

            if (!_backingStore.TryGetValue(contentHash, out data)) {
                this.Log().ErrorFormat("Failed to load object: {0}", contentHash);
                return null;
            }

            this.Log().DebugFormat("Loaded {0}", contentHash);
            return serializerFactory(contentHash).Deserialize(data, Utility.GetTypeByName(_itemTypeNames[contentHash]));
        }

        public void Save<T>(T obj) where T : ISerializableItem
        {
            this.Log().DebugFormat("Saving {0}: {1}", obj, obj.ContentHash);
            _itemTypeNames[obj.ContentHash] = obj.GetType().FullName;
            _backingStore[obj.ContentHash] = serializerFactory(obj).Serialize(obj);
        }

        public void FlushChanges()
        {
            persistMetadata();
            _backingStore.Flush();
        }

        public Guid[] GetAllObjectHashes()
        {
            return _backingStore.Keys.ToArray();
        }

        public int GetObjectCount()
        {
            return _backingStore.Count;
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem
        {
            Save(obj);

            var key = getKeyFromQualifiedType(typeof (T), qualifier ?? String.Empty);
            var parent = (_syncPointIndex.ContainsKey(key) ? _syncPointIndex[key] : Guid.Empty);
            var ret = new SyncPointInformation(obj.ContentHash, parent, typeof (T), qualifier ?? String.Empty, createdOn ?? DateTimeOffset.Now);
            Save(ret);
            _syncPointIndex[key] = ret.ContentHash;

            this.Log().InfoFormat("Created sync point: {0}.{1}", obj.ContentHash, qualifier);

            FlushChanges();
            return ret;
        }

        public Guid[] GetOrderedRevisionList(Type type, string qualifier = null)
        {
            var ret = new List<Guid>();
            var key = getKeyFromQualifiedType(type, qualifier ?? String.Empty);

            if (!_syncPointIndex.ContainsKey(key)) {
                return null;
            }

            var current = _syncPointIndex[key];
            while(current != Guid.Empty) {
                ret.Add(current);

                var syncPoint = Load<ISyncPointInformation>(current);
                current = syncPoint.ParentSyncPoint;
            }

            return ret.ToArray();
        }

        public T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null) where T : ISerializableItem {
            throw new NotImplementedException();
        }

        public T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null) where T : ISerializableItem {
            throw new NotImplementedException();
        }

        public void Dispose() {
            FlushChanges();
            _backingStore.Dispose();
        }

        static string getKeyFromQualifiedType(Type type, string qualifier)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}_{1}", type.FullName, qualifier);
        }

        void loadOrInitializeMetadata()
        {
            byte[] data;

            if (!_backingStore.TryGetValue(Guid.Empty, out data)) {
                if (_backingStore.Count != 0) {
                    this.Log().Fatal("Database has been corrupted!");
                    throw new Exception("Database is in an inconsistent state");
                }

                this.Log().Warn("Could not load metadata, initializing blank");
                _itemTypeNames = new Dictionary<Guid, string>();
                _syncPointIndex = new Dictionary<string, Guid>();

                persistMetadata();
            }
        }

        void persistMetadata() {
            var metadata = new EsentPersistedMetadata() { ItemTypeNames = _itemTypeNames, SyncPointIndex = null };
            _backingStore[Guid.Empty] = serializerFactory(metadata).Serialize(metadata);
        }
    }
}