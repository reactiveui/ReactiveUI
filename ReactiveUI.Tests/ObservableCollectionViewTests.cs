using System.Reactive.Linq;

namespace ReactiveUI.Tests
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Xunit;

    public class ObservableCollectionViewTests
    {
        [Fact]
        public void Degenerate_test()
        {
            var items = new ObservableCollectionView<int>(new int[0]);
        }

        [Fact]
        public void New_collection_gets_populated_from_source()
        {
            var source = new[] { 1, 2, 3 };
            var subject = new ObservableCollectionView<int>(source);

            subject.AssertAreEqual(source);
        }

        [Fact]
        public void Filter_is_respected_by_instantiation()
        {
            var source = new[] { 1, 2, 3, 4 };
            var subject = new ObservableCollectionView<int>(source, x => x % 2 == 0);

            subject.AssertAreEqual(new[] { 2, 4 });
        }

        [Fact]
        public void Collection_accepts_new_items_from_source()
        {
            var source = new ObservableCollection<int> { 1, 2, 3, 4 };
            var subject = new ObservableCollectionView<int>(source);
            source.Add(5);

            Assert.Equal(5, subject.Last());
        }

        [Fact]
        public void Collection_propagates_new_items_notifications()
        {
            var source = new ObservableCollection<int>();
            var subject = new ObservableCollectionView<int>(source);
            var notifications = new List<NotifyCollectionChangedEventArgs>();
            subject.ObserveCollectionChanged().Subscribe(notifications.Add);

            source.Add(5);

            var notification = notifications.Single();
            Assert.Equal(NotifyCollectionChangedAction.Add, notification.Action);
            Assert.Equal(5, notification.NewItems.Cast<int>().Single());
        }

        [Fact]
        public void New_items_notifications_respect_filter()
        {
            var source = new ObservableCollection<int>();
            var subject = new ObservableCollectionView<int>(source, x => x % 2 == 0);
            source.Add(5);
            source.Add(6);

            Assert.Equal(6, subject.Single());
        }

        [Fact]
        public void Sort_items_upon_instantiation()
        {
            var source = new ObservableCollection<int> { 8, 3, 1, 5, 0, 4, 3 };
            var subject = new ObservableCollectionView<int>(
                source,
                null,
                Comparer<int>.Default);

            subject.AssertAreEqual(new[] { 0, 1, 3, 3, 4, 5, 8 });
        }

        [Fact]
        public void New_items_preserve_sort_order()
        {
            var source = new ObservableCollection<int> { 1, 5, 0 };
            var subject = new ObservableCollectionView<int>(
                source,
                null,
                Comparer<int>.Default);

            source.Add(-2);
            source.Add(3);
            source.Add(3);
            source.Add(10);

            subject.AssertAreEqual(new[] { -2, 0, 1, 3, 3, 5, 10 });
        }

        [Fact]
        public void CanTrackCollectionCount()
        {
            var source = new ObservableCollection<int> { 1, 5, 0 };
            var subject = new ObservableCollectionView<int>(source, x => x > 1);
            var list = new List<int>();

            subject.ViewCountChanged
                .Aggregate(list, (acc, x) => { acc.Add(x); return acc; })
                .Subscribe();

            source.Add(6);
            list.AssertAreEqual(new[] { 1, 2 });

            source.Add(7);
            list.AssertAreEqual(new[] { 1, 2, 3 });

            source.Add(0);
            list.AssertAreEqual(new[] { 1, 2, 3 });
        }


        [Fact]
        public void Sort_when_item_property_changes_in_reactive_collection()
        {
            var source = new ReactiveCollection<Mock>(
                new List<Mock>
                {
                    new Mock{Name = "b"},
                    new Mock{Name = "a"},
                    new Mock{Name = "c", Enabled = true}
                })
            {
                ChangeTrackingEnabled = true
            };
            var subject = new ObservableCollectionView<Mock>(
                source,
                null,
                new SortByEnabledAndName());

            subject.Select(x => x.Name).AssertAreEqual(new[] { "c", "a", "b" });

            source.Add(new Mock { Name = "d", Enabled = true });
            subject.Select(x => x.Name).AssertAreEqual(new[] { "c", "d", "a", "b" });

            source.Single(x => x.Name == "c").Enabled = false;
            subject.Select(x => x.Name).AssertAreEqual(new[] { "d", "a", "b", "c" });

            source.Single(x => x.Name == "a").Name = "z";
            subject.Select(x => x.Name).AssertAreEqual(new[] { "d", "b", "c", "z" });
        }

        public class Mock : ReactiveObject
        {
            string name;
            public string Name
            {
                get { return name; }
                set { this.RaiseAndSetIfChanged(x => x.Name, value); }
            }

            bool enabled;
            public bool Enabled
            {
                get { return enabled; }
                set { this.RaiseAndSetIfChanged(x => x.Enabled, value); }
            }
        }

        public class SortByEnabledAndName : Comparer<Mock>
        {
            #region Public Methods

            public override int Compare(Mock x, Mock y)
            {
                if (x == null || y == null) return 0;
                if (y.Enabled.CompareTo(x.Enabled) != 0)
                    return y.Enabled.CompareTo(x.Enabled);

                return x.Name.CompareTo(y.Name);
            }

            #endregion
        }
    }
}