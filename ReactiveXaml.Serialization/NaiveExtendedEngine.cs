using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public sealed class NaiveExtendedEngine : IExtendedStorageEngine
    {
        IStorageEngine _engine;
        public NaiveExtendedEngine(IStorageEngine engine)
        {
            _engine = engine;
        }

        public T GetLatestRootObject<T>(string qualifier = null, DateTimeOffset? olderThan = null)
            where T : ISerializableItemBase
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
            where T : ISerializableItemBase
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

        public T Load<T>(Guid contentHash) where T : ISerializableItemBase
        {
            return _engine.Load<T>(contentHash);
        }

        public object Load(Guid contentHash)
        {
            return _engine.Load(contentHash);
        }

        public void Save<T>(T obj) where T : ISerializableItemBase
        {
            _engine.Save(obj);
        }

        public void FlushChanges()
        {
            _engine.FlushChanges();
        }

        public ISyncPointInformation CreateSyncPoint<T>(T obj, string qualifier = null, DateTimeOffset? createdOn = null) where T : ISerializableItemBase
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
