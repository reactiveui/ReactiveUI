using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public class NullStorageEngine : IStorageEngine
    {
        public T Load<T>(Guid ContentHash) where T : ISerializableItemBase
        {
            this.Log().DebugFormat("Loading {0}, returning null", ContentHash);
            return default(T);
        }

        public object Load(Guid ContentHash) 
        {
            this.Log().DebugFormat("Loading {0}, returning null", ContentHash);
            return null;
        }

        public void Save<T>(T Obj) where T : ISerializableItemBase
        {
            this.Log().DebugFormat("Saving {0}", Obj.ContentHash);
        }

        public void FlushChanges()
        {
            this.Log().Debug("Flush");
        }

        public T GetNewestItemByType<T>(DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            return default(T);
        }

        public IEnumerable<T> GetItemsByDate<T>(DateTimeOffset? NewerThan = null, DateTimeOffset? OlderThan = null) where T : ISerializableItemBase
        {
            return Enumerable.Empty<T>();
        }

        public void Dispose()
        {
            
        }
    }
}