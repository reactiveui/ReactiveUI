using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveXaml;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReactiveXaml.Serialization
{
    public class SerializedCollection<T> : ReactiveCollection<T>, ISerializableList<T>
        where T : ISerializableItemBase
    {
        public Guid ContentHash { get; protected set; }

        [DataMember]
        public Dictionary<Guid, DateTimeOffset> CreatedOn { get; protected set; }

        [DataMember]
        public Dictionary<Guid, DateTimeOffset> UpdatedOn { get; protected set; }

        [IgnoreDataMember]
        IDictionary<Guid, DateTimeOffset> ISerializableList<T>.CreatedOn {
            get { return this.CreatedOn;  }
        }
        [IgnoreDataMember]
        IDictionary<Guid, DateTimeOffset> ISerializableList<T>.UpdatedOn {
            get { return this.UpdatedOn;  }
        }

        public SerializedCollection()
        {
            CreatedOn = new Dictionary<Guid, DateTimeOffset>();
            UpdatedOn = new Dictionary<Guid, DateTimeOffset>();
            setupCollection();
        }

        public SerializedCollection(IEnumerable<T> items)
            : base(items)
        {
            CreatedOn = items.ToDictionary(k => k.ContentHash, _ => RxApp.DeferredScheduler.Now);
            UpdatedOn = items.ToDictionary(k => k.ContentHash, _ => RxApp.DeferredScheduler.Now);
            setupCollection();
        }

        [OnDeserialized]
        void setupCollection(StreamingContext sc) { setupCollection(); }
        void setupCollection()
        {
            ChangeTrackingEnabled = true;

            ItemsAdded.Subscribe(x => {
                CreatedOn[x.ContentHash] = RxApp.DeferredScheduler.Now;
                UpdatedOn[x.ContentHash] = RxApp.DeferredScheduler.Now;
            });

            ItemsRemoved.Subscribe(x => {
                CreatedOn.Remove(x.ContentHash);
                UpdatedOn.Remove(x.ContentHash);
            });

            ItemPropertyChanged.Subscribe(x => {
                UpdatedOn[x.Sender.ContentHash] = RxApp.DeferredScheduler.Now;
            });

            ItemChanging = Observable.Merge(
                BeforeItemsAdded.Select(_ => this),
                BeforeItemsRemoved.Select(_ => this),
                ItemPropertyChanging.Select(_ => this)
            );

            ItemChanged = Observable.Merge(
                ItemsAdded.Select(_ => this),
                ItemsRemoved.Select(_ => this),
                ItemPropertyChanged.Select(_ => this)
            );

            ItemChanged.Subscribe(_ => {
                ContentHash = CalculateHash();
                RxStorage.Engine.Save(this);
            });
        }

        [IgnoreDataMember]
        public IObservable<object> ItemChanging { get; protected set; }

        [IgnoreDataMember]
        public IObservable<object> ItemChanged { get; protected set; }

        public Guid CalculateHash()
        {
            // XXX: This is massively inefficient, we can do way better
            return new Guid(String.Join(",",
                this.Select(x => {
                    var si = x as ISerializableItemBase;
                    return (si != null ? si.ContentHash.ToString() : x.ToString().MD5Hash().ToString());
                })).MD5Hash());
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :