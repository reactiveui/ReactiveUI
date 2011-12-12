using System;
using System.Linq;

namespace ReactiveUI.Serialization
{
    /// <summary>
    /// This class is automatically used by RxStorage when the Engine is
    /// initialized and the Engine class does not support
    /// IExtendedStorageEngine; in this case, NaiveExtendedEngine wraps the
    /// Engine and implements the extended methods in a very naive, brute-force
    /// way.
    /// </summary>
    public sealed class NaiveExtendedEngine : IExtendedStorageEngine
    {
        IStorageEngine _engine;
        public NaiveExtendedEngine(IStorageEngine engine)
        {
            _engine = engine;
        }

        public T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItem
        {
            olderThan = olderThan ?? DateTimeOffset.MaxValue;
            Guid[] revisions = GetOrderedRevisionList(typeof (T), qualifier);
            if (revisions == null || revisions.Length == 0) {
                return default(T);
            }

            foreach(Guid current in revisions) {
                var syncPoint = Load<ISyncPointInformation>(current);
                if (syncPoint.CreatedOn < olderThan) {
                    return Load<T>(syncPoint.RootObjectHash);
                }
            }

            return default(T);
        }

        public T[] GetRootObjectsInDateRange<T>(string qualifier = null, DateTimeOffset? olderThan = null, DateTimeOffset? newerThan = null) 
            where T : ISerializableItem
        {
            olderThan = olderThan ?? DateTimeOffset.MaxValue;
            newerThan = newerThan ?? DateTimeOffset.MinValue;
            Guid[] revisions = GetOrderedRevisionList(typeof (T), qualifier);

            if (revisions == null || revisions.Length == 0) {
                return default(T[]);
            }

            return revisions
                .Select(Load<ISyncPointInformation>)
                .Where(x => x.CreatedOn < olderThan && x.CreatedOn > newerThan)
                .Select(x => Load<T>(x.RootObjectHash))
                .ToArray();
        }

        public T Load<T>(Guid contentHash) where T : ISerializableItem
        {
            return _engine.Load<T>(contentHash);
        }

        public object Load(Guid contentHash)
        {
            return _engine.Load(contentHash);
        }

        public void Save<T>(T obj) where T : ISerializableItem
        {
            _engine.Save(obj);
        }

        public void FlushChanges()
        {
            _engine.FlushChanges();
        }

        public Guid[] GetAllObjectHashes()
        {
            return _engine.GetAllObjectHashes();
        }

        public int GetObjectCount()
        {
            return _engine.GetObjectCount();
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null) where T : ISerializableItem
        {
            return _engine.CreateSyncPoint(obj, qualifier, createdOn);
        }

        public Guid[] GetOrderedRevisionList(Type type, string qualifier = null)
        {
            return _engine.GetOrderedRevisionList(type, qualifier);
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
