using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace ReactiveUI.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializedCollection<T> : ReactiveCollection<T>, ISerializableList<T>
        where T : ISerializableItem
    {
        static Guid inProgressGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        [IgnoreDataMember] Guid _ContentHash;

        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public Guid ContentHash {
            get {
                if (_ContentHash == Guid.Empty) {
                    // XXX: This is a blatant hack to make sure that the JSON serializer
                    // doesn't end up recursively calling CalculateHash
                    _ContentHash = inProgressGuid;
                    _ContentHash = CalculateHash();
                }
                return _ContentHash;
            }
            protected set { _ContentHash = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Dictionary<Guid, DateTimeOffset> CreatedOn { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
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
        
        /// <summary>
        /// 
        /// </summary>
        public SerializedCollection() : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sched"></param>
        public SerializedCollection(IScheduler sched = null)
        {
            CreatedOn = new Dictionary<Guid, DateTimeOffset>();
            UpdatedOn = new Dictionary<Guid, DateTimeOffset>();
            setupCollection(sched);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="sched"></param>
        /// <param name="createdOn"></param>
        /// <param name="updatedOn"></param>
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

            setupCollection(sched);
        }

        [OnDeserialized]
        void setupCollection(StreamingContext sc) { setupCollection(null); }
        void setupCollection(IScheduler sched)
        {
            ChangeTrackingEnabled = true;
            _sched = sched ?? RxApp.DeferredScheduler;
            invalidateHash();

            ItemsAdded.Subscribe(x => {
                if (ChangeTrackingEnabled == false) {
                    return;
                }
                CreatedOn[x.ContentHash] = _sched.Now;
                UpdatedOn[x.ContentHash] = _sched.Now;
            });

            ItemsRemoved.Subscribe(x => {
                if (ChangeTrackingEnabled == false) {
                    return;
                }
                CreatedOn.Remove(x.ContentHash);
                UpdatedOn.Remove(x.ContentHash);
            });

            ItemChanged.Subscribe(x => {
                if (ChangeTrackingEnabled == false) {
                    return;
                }
                UpdatedOn[x.Sender.ContentHash] = _sched.Now;
            });

            Changed.Subscribe(_ => invalidateHash());
        }

        public virtual Guid CalculateHash()
        {
            var buf = new MemoryStream();
            if (this.Count == 0 || this.All(x => x == null)) {
                var bytes = Encoding.Unicode.GetBytes(this.GetType().FullName);
                buf.Write(bytes, 0, bytes.Length);
            } else {
                foreach (var v in this) {
                    var si = (ISerializableItem)v;
                    if (si != null) {
                        buf.Write(si.ContentHash.ToByteArray(), 0, 16);
                    }
                }
            }
            buf.Write(BitConverter.GetBytes(this.Count), 0, 4);

            if (buf.Length == 0) {
                throw new Exception("Error calculating hash");
            }

            var md5 = MD5.Create();
            var ret = new Guid(md5.ComputeHash(buf.ToArray()));
            return ret;
        }

        protected virtual void invalidateHash()
        {
            ContentHash = Guid.Empty;
        }

        public Type GetBaseListType()
        {
            return typeof (T);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
