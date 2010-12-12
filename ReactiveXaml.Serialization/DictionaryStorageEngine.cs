using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    class DSESerializedObjects
    {
        public Dictionary<Guid, ISerializableItemBase> allItems;
        public Dictionary<string, Guid> syncPointIndex;
    }

    public class DictionaryStorageEngine : IStorageEngine
    {
        string backingStorePath;
        Dictionary<Guid, ISerializableItemBase> allItems;
        Dictionary<string, Guid> syncPointIndex;

        static readonly Lazy<IEnumerable<Type>> allStorageTypes = new Lazy<IEnumerable<Type>>(
            () => Utility.GetAllTypesImplementingInterface(typeof(ISerializableItemBase), Assembly.GetExecutingAssembly()).ToArray());

        public DictionaryStorageEngine(string Path = null)
        {
            backingStorePath = Path;
        }

        public T Load<T>(Guid ContentHash) where T : ISerializableItemBase
        {
            ISerializableItemBase ret = null;

            initializeStoreIfNeeded();
            allItems.TryGetValue(ContentHash, out ret);
            return (T)ret;
        }

        public object Load(Guid ContentHash)
        {
            initializeStoreIfNeeded();
            return allItems[ContentHash];
        }

        public void Save<T>(T Obj) where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            allItems[Obj.ContentHash] = Obj;
        }

        public void FlushChanges()
        {
            if (backingStorePath == null) {
                return;
            }

            initializeStoreIfNeeded();
            var dseData = new DSESerializedObjects() {allItems = this.allItems, syncPointIndex = this.syncPointIndex};
            File.WriteAllText(backingStorePath, JSONHelper.Serialize(dseData, allStorageTypes.Value), Encoding.UTF8);
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null)
            where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            Save(obj);

            var key = getKeyFromQualifiedType(typeof (T), qualifier ?? String.Empty);
            var parent = (syncPointIndex.ContainsKey(key) ? syncPointIndex[key] : Guid.Empty);
            var ret = new SyncPointInformation(obj.ContentHash, parent, typeof (T), qualifier ?? String.Empty, createdOn ?? DateTimeOffset.Now);
            Save(ret);
            syncPointIndex[key] = ret.ContentHash;

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

        public T GetNewestItemByType<T>(DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            
            OlderThan = OlderThan ?? DateTimeOffset.MaxValue;
            /*
            return allItems.Values.OfType<T>()
                .Where(x => x.UpdatedOn <= OlderThan)
                .OrderByDescending(x => x.UpdatedOn)
                .FirstOrDefault();
             */
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetItemsByDate<T>(DateTimeOffset? NewerThan = null, DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            NewerThan = NewerThan ?? DateTimeOffset.MinValue;
            OlderThan = OlderThan ?? DateTimeOffset.MaxValue;

            /*
            return allItems.Values.OfType<T>()
                .Where(x => x.UpdatedOn >= NewerThan && x.UpdatedOn <= OlderThan)
                .ToArray();
             */
            throw new NotImplementedException();
        }

        void initializeStoreIfNeeded()
        {
            if (allItems != null)
                return;

            if (backingStorePath == null) {
                allItems = new Dictionary<Guid,ISerializableItemBase>();
                syncPointIndex = new Dictionary<string, Guid>();
                return;
            }

            try {
                string text = File.ReadAllText(backingStorePath, Encoding.UTF8);
                var dseData = JSONHelper.Deserialize<DSESerializedObjects>(text, allStorageTypes.Value);
                allItems = dseData.allItems;
                syncPointIndex = dseData.syncPointIndex;
            } catch(FileNotFoundException) {
                this.Log().WarnFormat("Backing store {0} not found, falling back to empty", backingStorePath);
                allItems = new Dictionary<Guid,ISerializableItemBase>();
                syncPointIndex = new Dictionary<string, Guid>();
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

// vim: tw=120 ts=4 sw=4 et enc=utf8 :
