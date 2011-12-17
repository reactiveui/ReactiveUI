using System;
using System.Runtime.Serialization;
using NLog;

namespace ReactiveUI.Serialization
{
#if WINDOWS_PHONE
    /// <summary>
    /// ModelBase represents the base implementation of ISerializableItem and
    /// handles a lot of the infrastructure plumbing around maintaining the
    /// Content Hash.
    ///
    /// For objects who are frequently serialized/deserialized, derived classes
    /// should override CalculateHash and implement it in a more specific
    /// manner.
    /// </summary>
    [DataContract]
    public abstract class ModelBase : ReactiveObject, ISerializableItem
#else
    /// <summary>
    /// ModelBase represents the base implementation of ISerializableItem and
    /// handles a lot of the infrastructure plumbing around maintaining the
    /// Content Hash.
    ///
    /// For objects who are frequently serialized/deserialized, derived classes
    /// should override CalculateHash and implement it in a more specific
    /// manner.
    /// </summary>
    [DataContract]
    public abstract class ModelBase : ReactiveValidatedObject, ISerializableItem
#endif
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger();

        static Guid inProgressGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        [IgnoreDataMember] Guid _ContentHash;
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

        public ModelBase()
        {
            setupModelBase();
        }

        [OnDeserialized]
        void setupModelBase(StreamingContext sc) { setupModelBase(); }
        void setupModelBase()
        {
            log.Info("Deserialized ModelBase 0x{0:X}", this.GetHashCode());
            Changed.Subscribe(_ => invalidateHash());
            invalidateHash();
        }

        public virtual Guid CalculateHash()
        {
            return new Guid(this.ObjectContentsHash());
        }

        protected virtual void invalidateHash()
        {
            ContentHash = Guid.Empty;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
