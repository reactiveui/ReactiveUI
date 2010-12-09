using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveXaml;
using System.Disposables;

namespace ReactiveXaml.Serialization
{
    public interface ISerializableItemBase : IEnableLogger
    {
        Guid ContentHash { get; }

        IObservable<object> ItemChanging { get; }
        IObservable<object> ItemChanged { get; }

        Guid CalculateHash();
    }

    public interface ISerializableItem : IReactiveNotifyPropertyChanged, ISerializableItemBase { }

    public interface ISerializableList<T> : IReactiveCollection<T>, ISerializableItemBase
        where T : ISerializableItemBase
    {
        IDictionary<Guid, DateTimeOffset> CreatedOn { get; }
        IDictionary<Guid, DateTimeOffset> UpdatedOn { get; }
    }
    
    public interface IStorageEngine : IDisposable, IEnableLogger
    {
        T Load<T>(Guid ContentHash) where T : ISerializableItemBase;
        object Load(Guid ContentHash);
        void Save<T>(T Obj) where T : ISerializableItemBase;
        void FlushChanges();

        T GetNewestItemByType<T>(DateTimeOffset? OlderThan = null) 
            where T : ISerializableItemBase;
        IEnumerable<T> GetItemsByDate<T>(DateTimeOffset? NewerThan = null, DateTimeOffset? OlderThan = null)
            where T : ISerializableItemBase;
    }

    public interface IExplicitReferenceBase
    {
        Guid ValueHash { get; set; }
        IDisposable Update();
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :