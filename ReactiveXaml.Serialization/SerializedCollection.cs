using System;
using System.Collections.Generic;
using System.Concurrency;
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
        where T : ISerializableItem
    {
        [IgnoreDataMember]
        public Guid ContentHash { get; protected set; }

        [DataMember]
        public Dictionary<Guid, DateTimeOffset> CreatedOn { get; protected set; }

        [DataMember]
        public Dictionary<Guid, DateTimeOffset> UpdatedOn { get; protected set; }

        [IgnoreDataMember]
        IDictionary<Guid, DateTimeOffset> ISerializableList.CreatedOn {
            get { return this.CreatedOn;  }
        }
        [IgnoreDataMember]
        IDictionary<Guid, DateTimeOffset> ISerializableList.UpdatedOn {
            get { return this.UpdatedOn;  }   
        }

        IScheduler _sched;
        
        public SerializedCollection() : this(null)
        {
        }

        public SerializedCollection(IScheduler sched = null)
        {
            CreatedOn = new Dictionary<Guid, DateTimeOffset>();
            UpdatedOn = new Dictionary<Guid, DateTimeOffset>();
            setupCollection(sched);
        }

        public SerializedCollection(IEnumerable<T> items,
            IScheduler sched = null,
            IDictionary<Guid, DateTimeOffset> createdOn = null,
            IDictionary<Guid, DateTimeOffset> updatedOn = null)
            : base(items)
        {
            if (createdOn != null) {
                CreatedOn = createdOn.Keys.ToDictionary(k => k, k => createdOn[k]);
            } else {
                CreatedOn = new Dictionary<Guid, DateTimeOffset>();
                foreach (var v in items) {
                    CreatedOn[v.ContentHash] = RxApp.DeferredScheduler.Now;
                }
            }

            if (updatedOn != null) {
                UpdatedOn = updatedOn.Keys.ToDictionary(k => k, k => updatedOn[k]);
            } else {
                UpdatedOn = new Dictionary<Guid, DateTimeOffset>();
                foreach (var v in items) {
                    UpdatedOn[v.ContentHash] = RxApp.DeferredScheduler.Now;
                }
            }

            ContentHash = CalculateHash();
            setupCollection(sched);
        }

        [OnDeserialized]
        void setupCollection(StreamingContext sc) { setupCollection(null); }
        void setupCollection(IScheduler sched)
        {
            ChangeTrackingEnabled = true;
            _sched = sched ?? RxApp.DeferredScheduler;

            ItemsAdded.Subscribe(x => {
                CreatedOn[x.ContentHash] = _sched.Now;
                UpdatedOn[x.ContentHash] = _sched.Now;
            });

            ItemsRemoved.Subscribe(x => {
                CreatedOn.Remove(x.ContentHash);
                UpdatedOn.Remove(x.ContentHash);
            });

            ItemChanged.Subscribe(x => {
                UpdatedOn[x.Sender.ContentHash] = _sched.Now;
            });

            ItemChanged.Subscribe(_ => {
                this.Log().DebugFormat("Saving list {0:X}", this.GetHashCode());
                ContentHash = CalculateHash();
                RxStorage.Engine.Save(this);
            });
        }

        public Guid CalculateHash()
        {
            // XXX: This is massively inefficient, we can do way better
            return new Guid(String.Join(",",
                this.Select(x => {
                    var si = x as ISerializableItem;
                    return (si != null ? si.ContentHash.ToString() : x.ToString().MD5Hash().ToString());
                })).MD5Hash());
        }

        public Type GetBaseListType()
        {
            return typeof (T);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :