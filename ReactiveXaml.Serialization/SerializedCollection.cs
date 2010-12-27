using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Concurrency;
#endif

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
            var buf = new MemoryStream();
            if (this.Count == 0) {
                var bytes = Encoding.Default.GetBytes(this.GetType().FullName);
                buf.Write(bytes, 0, bytes.Length);
            } else {
                foreach (var v in this) {
                    var si = v as ISerializableItem;
                    if (si != null) {
                        buf.Write(si.ContentHash.ToByteArray(), 0, 16);
                    }
                }
            }

            var md5 = MD5.Create();
            return new Guid(md5.ComputeHash(buf.ToArray()));
        }

        public Type GetBaseListType()
        {
            return typeof (T);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :