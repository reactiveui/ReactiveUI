using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Isam.Esent.Collections.Generic;
using NLog;

namespace ReactiveUI.Serialization.Esent
{
    public class EsentPersistedMetadata
    {
        public Dictionary<string, Guid> SyncPointIndex { get; set; }
        public Dictionary<Guid, string> ItemTypeNames { get; set; }
    }

    public class EsentStorageEngine : IExtendedStorageEngine
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger();

        PersistentDictionary<Guid, byte[]> _backingStore;
        Dictionary<Guid, string> _itemTypeNames;
        Dictionary<string, Guid> _syncPointIndex;
        Dictionary<string, SortedSet<ISyncPointInformation>> _syncPoints;

        Func<object, IObjectSerializationProvider> _serializerFactory;

        public EsentStorageEngine(string databasePath, Func<object, IObjectSerializationProvider> serializerFactory = null)
        {
            _backingStore = new PersistentDictionary<Guid, byte[]>(databasePath);
#if DEBUG
            _backingStore.TraceSwitch.Level = System.Diagnostics.TraceLevel.Verbose;
#endif

            _serializerFactory = serializerFactory ?? (root => new JsonNetObjectSerializationProvider(this));

            loadOrInitializeMetadata();
        }

        public T Load<T>(Guid contentHash) where T : ISerializableItem {
            return (T)Load(contentHash);
        }

        public object Load(Guid contentHash) 
        {
            byte[] data;

            if (!_backingStore.TryGetValue(contentHash, out data)) {
                log.Error("Failed to load object: {0}", contentHash);
                return null;
            }

            log.Debug("Loaded {0}", contentHash);
            return this._serializerFactory(contentHash).Deserialize(data, Utility.GetTypeByName(_itemTypeNames[contentHash]));
        }

        public void Save<T>(T obj) where T : ISerializableItem
        {
            if (obj.ContentHash == Guid.Empty) {
                log.Error("Object of type '{0}' has a zero ContentHash", obj.GetType());
                throw new Exception("Cannot serialize object with zero ContentHash");
            }

            log.Debug("Saving {0}: {1}", obj, obj.ContentHash);
            _itemTypeNames[obj.ContentHash] = obj.GetType().FullName;
            _backingStore[obj.ContentHash] = this._serializerFactory(obj).Serialize(obj);
        }

        public void FlushChanges()
        {
            persistMetadata();
            _backingStore.Flush();
        }

        public Guid[] GetAllObjectHashes()
        {
            return _backingStore.Keys.Where(x => x != Guid.Empty).ToArray();
        }

        public int GetObjectCount()
        {
            return _backingStore.Count - 1; // One for the metadata object
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
            _syncPoints.GetOrAdd(key).Add(ret);

            log.Info("Created sync point: {0}.{1}", obj.ContentHash, qualifier);

            FlushChanges();
            return ret;
        }

        public Guid[] GetOrderedRevisionList(Type type, string qualifier = null)
        {
            var fullName = type.FullName;
            return _syncPoints.GetOrAdd(getKeyFromQualifiedType(type, qualifier))
                .Select(x => x.ContentHash)
                .ToArray();
        }

        public T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null) where T : ISerializableItem
        {
            var ret = _syncPoints.GetOrAdd(getKeyFromQualifiedType(typeof (T), qualifier))
                .FirstOrDefault(x => x.CreatedOn <= (olderThan ?? DateTimeOffset.MaxValue));
            return (ret != null ? Load<T>(ret.RootObjectHash) : default(T));
        }

        public T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null) where T : ISerializableItem
        {
            var set = _syncPoints.GetOrAdd(getKeyFromQualifiedType(typeof (T), qualifier));
            var lower = new SyncPointInformation(Guid.Empty, Guid.Empty, typeof (int), null, newerThan ?? DateTimeOffset.MinValue);
            var upper = new SyncPointInformation(Guid.Empty, Guid.Empty, typeof (int), null, olderThan ?? DateTimeOffset.MaxValue);

            return set.GetViewBetween(lower, upper).Select(x => Load<T>(x.RootObjectHash)).ToArray();
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
                    log.Fatal("Database has been corrupted!");
                    throw new Exception("Database is in an inconsistent state");
                }

                log.Warn("Could not load metadata, initializing blank");
                _itemTypeNames = new Dictionary<Guid, string>();
                _syncPointIndex = new Dictionary<string, Guid>();

                persistMetadata();
            } else {
                var metadata = (EsentPersistedMetadata)this._serializerFactory(Guid.Empty).Deserialize(data, typeof (EsentPersistedMetadata));
                _itemTypeNames = metadata.ItemTypeNames;
                _syncPointIndex = metadata.SyncPointIndex;
                if (_itemTypeNames == null || _syncPointIndex == null) {
                    log.Fatal("Database has been corrupted, metadata structures are null");
                    throw new Exception("Database is in an inconsistent state");
                }
            }

            _syncPoints = new Dictionary<string, SortedSet<ISyncPointInformation>>();
            foreach(var v in _syncPointIndex.Values) {
                var item = Load<SyncPointInformation>(v);
                _syncPoints
                    .GetOrAdd(getKeyFromQualifiedType(Utility.GetTypeByName(item.RootObjectTypeName), item.Qualifier))
                    .Add(item);
            }
        }

        void persistMetadata() {
            var metadata = new EsentPersistedMetadata() { ItemTypeNames = _itemTypeNames, SyncPointIndex = _syncPointIndex };
            _backingStore[Guid.Empty] = this._serializerFactory(metadata).Serialize(metadata);
        }
    }
}
