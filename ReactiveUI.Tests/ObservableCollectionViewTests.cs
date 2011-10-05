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
    }
}