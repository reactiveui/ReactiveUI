namespace ReactiveUI.Tests.Winforms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using ReactiveUI.Winforms;

    using Xunit;

    public class ReactiveBindingListTests
    {
        [Fact]
        public void RaisesListChangedEventOnAdd()
        {
            var target = new ReactiveBindingList<string>();
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
                { capturedEvents.Add(e); };

            target.Add("item");

            Assert.Equal(1,capturedEvents.Count);
            Assert.True(capturedEvents[0].ListChangedType == ListChangedType.ItemAdded);
            Assert.True(capturedEvents[0].NewIndex == 0);
        }

        [Fact]
        public void RaisesListChangedEventOnRemove()
        {
            var target = new ReactiveBindingList<string>(new string[]{"item1","item2"});
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.Remove("item2");

            Assert.Equal(1, capturedEvents.Count);
            Assert.True(capturedEvents[0].ListChangedType == ListChangedType.ItemDeleted);
            Assert.Equal(1,capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);
        }

        [Fact]
        public void RaisesListChangedEventOnChange()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target[0] = "changed item1";

            Assert.Equal(1, capturedEvents.Count);
            Assert.True(capturedEvents[0].ListChangedType == ListChangedType.ItemChanged);
            Assert.Equal(0, capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);
        }

        [Fact]
        public void RaisesListChangedEventOnMove()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.Move(0,1);
          
            Assert.Equal(1, capturedEvents.Count);
          
            Assert.True(capturedEvents[0].ListChangedType == ListChangedType.ItemMoved);
            Assert.Equal(1, capturedEvents[0].NewIndex);
            Assert.Equal(0, capturedEvents[0].OldIndex);
        }

        [Fact]
        public void RaisesListChangedEventOnReset()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.Reset();
            Assert.Equal(1, capturedEvents.Count);

            Assert.True(capturedEvents[0].ListChangedType == ListChangedType.Reset);
            Assert.Equal(-1, capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);
        }

        [Fact]
        public void RaisesMultipleEventsListChangedEventOnRemoveRange()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2" ,"item3"});
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.RemoveRange(1,2);
            Assert.Equal(2, capturedEvents.Count);

            Assert.True(capturedEvents.All(x=>x.ListChangedType == ListChangedType.ItemDeleted));
            Assert.Equal(1, capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);

            Assert.Equal(2, capturedEvents[1].NewIndex);
            Assert.Equal(-1, capturedEvents[1].OldIndex);
        }

        [Fact]
        public void RaisesMultipleEventsListChangedEventOnAddRange()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2", "item3" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.AddRange(new[]{"item4","item5","item6"});
            Assert.Equal(3, capturedEvents.Count);

            Assert.True(capturedEvents.All(x => x.ListChangedType == ListChangedType.ItemAdded));
            Assert.Equal(3, capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);

            Assert.Equal(4, capturedEvents[1].NewIndex);
            Assert.Equal(-1, capturedEvents[1].OldIndex);

            Assert.Equal(5, capturedEvents[2].NewIndex);
            Assert.Equal(-1, capturedEvents[2].OldIndex);
        }

        [Fact]
        public void RaisesMultipleEventsListChangedEventOnInsertRange()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2", "item3" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.InsertRange(1,new[] { "item4", "item5", "item6" });
            Assert.Equal(3, capturedEvents.Count);

            Assert.True(capturedEvents.All(x => x.ListChangedType == ListChangedType.ItemAdded));
            Assert.Equal(1, capturedEvents[0].NewIndex);
            Assert.Equal(-1, capturedEvents[0].OldIndex);

            Assert.Equal(2, capturedEvents[1].NewIndex);
            Assert.Equal(-1, capturedEvents[1].OldIndex);

            Assert.Equal(3, capturedEvents[2].NewIndex);
            Assert.Equal(-1, capturedEvents[2].OldIndex);
        }

        [Fact]
        public void RaisesResetEventWhileItemsAddedOnSuppressChanges()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2", "item3" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };
            
            using (target.SuppressChangeNotifications()) {
                target.InsertRange(1, new[] { "item4", "item5", "item6" });
            }

            Assert.Equal(1, capturedEvents.Count);
            Assert.Equal(ListChangedType.Reset, capturedEvents[0].ListChangedType);
        }

        [Fact]
        public void RaisesResetEventWhenAboveTreshold()
        {
            var target = new ReactiveBindingList<string>(new string[] { "item1", "item2", "item3" });
            var capturedEvents = new List<ListChangedEventArgs>();
            target.ListChanged += (o, e) =>
            { capturedEvents.Add(e); };

            target.ResetChangeThreshold = 0;
                target.InsertRange(1, Enumerable.Repeat("added1",35));
            

            Assert.Equal(1, capturedEvents.Count);
            Assert.Equal(ListChangedType.Reset, capturedEvents[0].ListChangedType);
        }
    }
}