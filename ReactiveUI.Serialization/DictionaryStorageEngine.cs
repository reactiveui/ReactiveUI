using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using NLog;

#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif

namespace ReactiveUI.Serialization
{
    internal class DSESerializedObjects
    {
        public Dictionary<Guid, byte[]> allItems { get; set; }
        public Dictionary<Guid, string> itemTypeNames { get; set; }
        public Dictionary<string, Guid> syncPointIndex { get; set; }
    }
    
    /// <summary>
    /// DictionaryStorageEngine is an implementation of IStorageEngine designed
    /// for testing and debugging purposes; it stores all elements in memory,
    /// and optionally persists its store to a JSON file or in Isolated Storage.
    /// 
    /// This engine is only suited to very small production use-cases, as you
    /// will almost certainly run out of memory with large collections.
    /// </summary>
    public class DictionaryStorageEngine : IStorageEngine
    {
        readonly string _backingStorePath;
        Dictionary<Guid, byte[]> _allItems;
        Dictionary<Guid, string> _itemTypeNames;
        Dictionary<string, Guid> _syncPointIndex;

        Func<object, IObjectSerializationProvider> _serializerFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="serializerFactory"></param>
        public DictionaryStorageEngine(string path = null, Func<object, IObjectSerializationProvider> serializerFactory = null)
        {
            this._backingStorePath = path;
            _serializerFactory = serializerFactory ?? (root => new JsonNetObjectSerializationProvider(this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentHash"></param>
        /// <returns></returns>
        public T Load<T>(Guid contentHash) where T : ISerializableItem
        {
            return (T)Load(contentHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentHash"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Engine is inconsistent</exception>
        public object Load(Guid contentHash)
        {
            byte[] ret;

            initializeStoreIfNeeded();
            if (!this._allItems.TryGetValue(contentHash, out ret)) {
                this.Log().Error("Attempted to load '{0}', didn't exist!", contentHash);
                return null;
            }

            this.Log().Debug("Loaded '{0}'", contentHash);
#if DEBUG
            this.Log().Info(_serializerFactory(contentHash).SerializedDataToString(ret));
#endif
            var type = Utility.GetTypeByName(this._itemTypeNames[contentHash]);
            if (type == null) {
                this.Log().Fatal("Type '{0}' cannot be found", this._itemTypeNames[contentHash]);
                throw new Exception("Engine is inconsistent");
            }
            return this._serializerFactory(contentHash).Deserialize(ret, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <exception cref="Exception">Cannot serialize object with zero ContentHash</exception>
        public void Save<T>(T obj) where T : ISerializableItem
        {
            initializeStoreIfNeeded();

            if (obj.ContentHash == Guid.Empty) {
                this.Log().Error("Object of type '{0}' has a zero ContentHash", obj.GetType());
                throw new Exception("Cannot serialize object with zero ContentHash");
            }

            this.Log().Debug("Saving '{0}", obj.ContentHash);
            this._allItems[obj.ContentHash] = this._serializerFactory(obj).Serialize(obj);
            this._itemTypeNames[obj.ContentHash] = obj.GetType().FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FlushChanges()
        {
            if (this._backingStorePath == null) {
                return;
            }

            initializeStoreIfNeeded();
            this.Log().Info("Flushing changes");
            var dseData = new DSESerializedObjects() {allItems = this._allItems, syncPointIndex = this._syncPointIndex, itemTypeNames = this._itemTypeNames};

            using (var sw = getWriteStreamFromBackingStore(this._backingStorePath)) {
                byte[] buf = this._serializerFactory(dseData).Serialize(dseData);
                sw.Write(buf, 0, buf.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Guid[] GetAllObjectHashes()
        {
            initializeStoreIfNeeded();
            return (this._allItems.Keys ?? Enumerable.Empty<Guid>()).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetObjectCount()
        {
            initializeStoreIfNeeded();
            return this._allItems.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="qualifier"></param>
        /// <param name="createdOn"></param>
        /// <returns></returns>
        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem
        {
            initializeStoreIfNeeded();
            Save(obj);

            var key = getKeyFromQualifiedType(typeof (T), qualifier ?? String.Empty);
            var parent = (this._syncPointIndex.ContainsKey(key) ? this._syncPointIndex[key] : Guid.Empty);
            var ret = new SyncPointInformation(obj.ContentHash, parent, typeof (T), qualifier ?? String.Empty, createdOn ?? RxApp.DeferredScheduler.Now);
            Save(ret);
            this._syncPointIndex[key] = ret.ContentHash;

            this.Log().Info("Created sync point: {0}.{1}", obj.ContentHash, qualifier);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        public Guid[] GetOrderedRevisionList(Type type, string qualifier = null)
        {
            initializeStoreIfNeeded();

            var ret = new List<Guid>();
            var key = getKeyFromQualifiedType(type, qualifier ?? String.Empty);

            if (!this._syncPointIndex.ContainsKey(key)) {
                return new Guid[0];
            }

            var current = this._syncPointIndex[key];
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

            this._allItems = null;
            if (this._backingStorePath != null && !disposing)
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
            if (this._allItems != null)
                return;

            if (this._backingStorePath == null) {
                this._allItems = new Dictionary<Guid, byte[]>();
                this._syncPointIndex = new Dictionary<string, Guid>();
                this._itemTypeNames = new Dictionary<Guid, string>();
                return;
            }

            try {
                var serializer = this._serializerFactory(null);
                DSESerializedObjects dseData;

                using (var sr = getReadStreamFromBackingStore(this._backingStorePath)) {
                    dseData = (DSESerializedObjects)serializer.Deserialize(sr.GetAllBytes(), typeof (DSESerializedObjects));
                }

                this._allItems = dseData.allItems;
                this._syncPointIndex = dseData.syncPointIndex;
                this._itemTypeNames = dseData.itemTypeNames;
            } catch(FileNotFoundException) {
                this.Log().Warn("Backing store {0} not found, falling back to empty", this._backingStorePath);
                this._allItems = new Dictionary<Guid, byte[]>();
                this._syncPointIndex = new Dictionary<string, Guid>();
                this._itemTypeNames = new Dictionary<Guid, string>();
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
