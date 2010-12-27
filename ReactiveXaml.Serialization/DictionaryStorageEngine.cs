using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif

namespace ReactiveXaml.Serialization
{
    internal class DSESerializedObjects
    {
        public Dictionary<Guid, byte[]> allItems { get; set; }
        public Dictionary<Guid, string> itemTypeNames { get; set; }
        public Dictionary<string, Guid> syncPointIndex { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryStorageEngine : IStorageEngine
    {
        string backingStorePath;
        Dictionary<Guid, byte[]> allItems;
        Dictionary<Guid, string> itemTypeNames;
        Dictionary<string, Guid> syncPointIndex;

        Func<object, IObjectSerializationProvider> serializerFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public DictionaryStorageEngine(string Path = null)
        {
            backingStorePath = Path;

            serializerFactory = (root => new JsonNetObjectSerializationProvider(this));
        }

        public T Load<T>(Guid ContentHash) where T : ISerializableItem
        {
            return (T)Load(ContentHash);
        }

        public object Load(Guid ContentHash)
        {
            byte[] ret;

            initializeStoreIfNeeded();
            if (!allItems.TryGetValue(ContentHash, out ret)) {
                this.Log().ErrorFormat("Attempted to load '{0}', didn't exist!", ContentHash);
                return null;
            }

            this.Log().DebugFormat("Loaded '{0}'", ContentHash);
#if DEBUG
            this.Log().Info(serializerFactory(ContentHash).SerializedDataToString(ret));
#endif
            var type = Utility.GetTypeByName(itemTypeNames[ContentHash]);
            if (type == null) {
                this.Log().FatalFormat("Type '{0}' cannot be found", itemTypeNames[ContentHash]);
                throw new Exception("Engine is inconsistent");
            }
            return serializerFactory(ContentHash).Deserialize(ret, type);
        }

        public void Save<T>(T Obj) where T : ISerializableItem
        {
            initializeStoreIfNeeded();

            if (Obj.ContentHash == Guid.Empty) {
                this.Log().ErrorFormat("Object of type '{0}' has a zero ContentHash", Obj.GetType());
                throw new Exception("Cannot serialize object with zero ContentHash");
            }

            this.Log().DebugFormat("Saving '{0}", Obj.ContentHash);
            allItems[Obj.ContentHash] = serializerFactory(Obj).Serialize(Obj);
            itemTypeNames[Obj.ContentHash] = Obj.GetType().FullName;
        }

        public void FlushChanges()
        {
            if (backingStorePath == null) {
                return;
            }

            initializeStoreIfNeeded();
            this.Log().Info("Flushing changes");
            var dseData = new DSESerializedObjects() {allItems = this.allItems, syncPointIndex = this.syncPointIndex, itemTypeNames = this.itemTypeNames};

            using (var sw = getWriteStreamFromBackingStore(backingStorePath)) {
                byte[] buf = serializerFactory(dseData).Serialize(dseData);
                sw.Write(buf, 0, buf.Length);
            }
        }

        public Guid[] GetAllObjectHashes()
        {
            initializeStoreIfNeeded();
            return (allItems.Keys ?? Enumerable.Empty<Guid>()).ToArray();
        }

        public int GetObjectCount()
        {
            initializeStoreIfNeeded();
            return allItems.Count;
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem
        {
            initializeStoreIfNeeded();
            Save(obj);

            var key = getKeyFromQualifiedType(typeof (T), qualifier ?? String.Empty);
            var parent = (syncPointIndex.ContainsKey(key) ? syncPointIndex[key] : Guid.Empty);
            var ret = new SyncPointInformation(obj.ContentHash, parent, typeof (T), qualifier ?? String.Empty, createdOn ?? RxApp.DeferredScheduler.Now);
            Save(ret);
            syncPointIndex[key] = ret.ContentHash;

            this.Log().InfoFormat("Created sync point: {0}.{1}", obj.ContentHash, qualifier);

            return ret;
        }

        public Guid[] GetOrderedRevisionList(Type type, string qualifier = null)
        {
            initializeStoreIfNeeded();

            var ret = new List<Guid>();
            var key = getKeyFromQualifiedType(type, qualifier ?? String.Empty);

            if (!syncPointIndex.ContainsKey(key)) {
                return new Guid[0];
            }

            var current = syncPointIndex[key];
            while(current != Guid.Empty) {
                ret.Add(current);

                var syncPoint = Load<SyncPointInformation>(current);
                current = syncPoint.ParentSyncPoint;
            }

            return ret.ToArray();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                initializeStoreIfNeeded();
                FlushChanges();
            }

            allItems = null;
            if (backingStorePath != null && !disposing)
                throw new Exception("Always dispose the Storage Engine");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DictionaryStorageEngine()
        {
            Dispose(false);
        }

        void initializeStoreIfNeeded()
        {
            if (allItems != null)
                return;

            if (backingStorePath == null) {
                allItems = new Dictionary<Guid, byte[]>();
                syncPointIndex = new Dictionary<string, Guid>();
                itemTypeNames = new Dictionary<Guid, string>();
                return;
            }

            try {
                var serializer = serializerFactory(null);
                DSESerializedObjects dseData;

                using (var sr = getReadStreamFromBackingStore(backingStorePath)) {
                    dseData = (DSESerializedObjects)serializer.Deserialize(sr.GetAllBytes(), typeof (DSESerializedObjects));
                }

                allItems = dseData.allItems;
                syncPointIndex = dseData.syncPointIndex;
                itemTypeNames = dseData.itemTypeNames;
            } catch(FileNotFoundException) {
                this.Log().WarnFormat("Backing store {0} not found, falling back to empty", backingStorePath);
                allItems = new Dictionary<Guid, byte[]>();
                syncPointIndex = new Dictionary<string, Guid>();
                itemTypeNames = new Dictionary<Guid, string>();
            }
        }

        static string getKeyFromQualifiedType(Type type, string qualifier)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}_{1}", type.FullName, qualifier);
        }

#if SILVERLIGHT
        protected Stream getReadStreamFromBackingStore(string path)
        {
            using (var fs = IsolatedStorageFile.GetUserStoreForApplication()) {
                return fs.OpenFile(path, FileMode.Open);
            }
        }
        
        protected Stream getWriteStreamFromBackingStore(string path)
        {
            using (var fs = IsolatedStorageFile.GetUserStoreForApplication()) {
                return fs.OpenFile(path, FileMode.Create);
            }
        }
#else 
        protected Stream getReadStreamFromBackingStore(string path) { return File.OpenRead(path); }
        protected Stream getWriteStreamFromBackingStore(string path) { return File.Open(path, FileMode.Create); }
#endif

    }
}

// vim: tw=120 ts=4 sw=4 et :