using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using ReactiveXaml;
using System.Reflection;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Disposables;
using System.ComponentModel;
using System.Globalization;

namespace ReactiveXaml.Serialization
{
    public class DSESerializedObjects
    {
        public Dictionary<Guid, byte[]> allItems { get; set; }
        public Dictionary<Guid, string> itemTypeNames { get; set; }
        public Dictionary<string, Guid> syncPointIndex { get; set; }
    }
    
    public class DictionaryStorageEngine : IStorageEngine
    {
        string backingStorePath;
        Dictionary<Guid, byte[]> allItems;
        Dictionary<Guid, string> itemTypeNames;
        Dictionary<string, Guid> syncPointIndex;

        static readonly Lazy<IEnumerable<Type>> allStorageTypes = new Lazy<IEnumerable<Type>>(
            () => Utility.GetAllTypesImplementingInterface(typeof(ISerializableItem)).ToArray());

        Func<object, IObjectSerializationProvider> serializerFactory;

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
            return serializerFactory(ContentHash).Deserialize(ret, Utility.GetTypeByName(itemTypeNames[ContentHash]));
        }

        public void Save<T>(T Obj) where T : ISerializableItem
        {
            initializeStoreIfNeeded();

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
            File.WriteAllBytes(backingStorePath, serializerFactory(dseData).Serialize(dseData));
        }

        public Guid[] GetAllObjectHashes()
        {
            return allItems.Keys.ToArray();
        }

        public int GetObjectCount()
        {
            return allItems.Count;
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItem
        {
            initializeStoreIfNeeded();
            Save(obj);

            var key = getKeyFromQualifiedType(typeof (T), qualifier ?? String.Empty);
            var parent = (syncPointIndex.ContainsKey(key) ? syncPointIndex[key] : Guid.Empty);
            var ret = new SyncPointInformation(obj.ContentHash, parent, typeof (T), qualifier ?? String.Empty, createdOn ?? DateTimeOffset.Now);
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
                return null;
            }

            var current = syncPointIndex[key];
            while(current != Guid.Empty) {
                ret.Add(current);

                var syncPoint = Load<ISyncPointInformation>(current);
                current = syncPoint.ParentSyncPoint;
            }

            return ret.ToArray();
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
                var serializer = new JsonSerializer();
                DSESerializedObjects dseData;
                using(var reader = new JsonTextReader(new StreamReader(backingStorePath))) {
                    dseData = serializer.Deserialize<DSESerializedObjects>(reader);
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
    }
}

// vim: tw=120 ts=4 sw=4 et :