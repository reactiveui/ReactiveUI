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
        where T : ISerializableItem
    {
        public Guid ContentHash { get; protected set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        public SerializedCollection() { setupCollection(); }
        public SerializedCollection(IEnumerable<T> items)
            : base(items)
        {
            setupCollection();
        }

        [OnDeserialized]
        void setupCollection(StreamingContext sc) { setupCollection(); }
        void setupCollection()
        {
            ChangeTrackingEnabled = true;

            var something_changing = Observable.Merge(
                BeforeItemsAdded.Select(_ => new Unit()),
                BeforeItemsRemoved.Select(_ => new Unit()),
                ItemPropertyChanging.Select(_ => new Unit())
            );

            var something_changed = Observable.Merge(
                ItemsAdded.Select(_ => new Unit()),
                ItemsRemoved.Select(_ => new Unit()),
                ItemPropertyChanged.Select(_ => new Unit())
            );

            ItemChanging = something_changing.Select(_ => this);
            ItemChanged = something_changed.Select(_ => this);

            something_changed.Subscribe(_ => {
                UpdatedOn = DateTimeOffset.Now;
                ContentHash = CalculateHash();
            });
        }

        [IgnoreDataMember]
        public IObservable<object> ItemChanging { get; protected set; }

        [IgnoreDataMember]
        public IObservable<object> ItemChanged { get; protected set; }

        public Guid CalculateHash()
        {
            return new Guid(String.Join(",", 
                this.Select(x => x.ContentHash.ToString())).MD5Hash());
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :