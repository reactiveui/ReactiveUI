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
    public class DictionaryStorageEngine : IStorageEngine
    {
        string backingStorePath;
        Dictionary<Guid, ISerializableItemBase> allItems;

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
            File.WriteAllText(backingStorePath, JSONHelper.Serialize(allItems, allStorageTypes.Value), Encoding.UTF8);
        }

        public T GetNewestItemByType<T>(DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            
            OlderThan = OlderThan ?? DateTimeOffset.MaxValue;
            return allItems.Values.OfType<T>()
                .Where(x => x.UpdatedOn <= OlderThan)
                .OrderByDescending(x => x.UpdatedOn)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetItemsByDate<T>(DateTimeOffset? NewerThan = null, DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            initializeStoreIfNeeded();
            NewerThan = NewerThan ?? DateTimeOffset.MinValue;
            OlderThan = OlderThan ?? DateTimeOffset.MaxValue;

            return allItems.Values.OfType<T>()
                .Where(x => x.UpdatedOn >= NewerThan && x.UpdatedOn <= OlderThan)
                .ToArray();
        }

        void initializeStoreIfNeeded()
        {
            if (allItems != null)
                return;

            if (backingStorePath == null) {
                allItems = new Dictionary<Guid,ISerializableItemBase>();
                return;
            }

            try {
                string text = File.ReadAllText(backingStorePath, Encoding.UTF8);
                allItems = JSONHelper.Deserialize<Dictionary<Guid, ISerializableItemBase>>(text, allStorageTypes.Value);
            } catch(FileNotFoundException) {
                this.Log().WarnFormat("Backing store {0} not found, falling back to empty", backingStorePath);
                allItems = new Dictionary<Guid,ISerializableItemBase>();
            }
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
