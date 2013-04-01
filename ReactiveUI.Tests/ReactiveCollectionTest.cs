using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI.Testing;
using Xunit;

using Microsoft.Reactive.Testing;
using System.Collections.Specialized;
using System.Reactive.Subjects;
using System.Reactive;
using System.Diagnostics;

namespace ReactiveUI.Tests
{
    public class ReactiveCollectionTest
    {
        [Fact]
        public void CollectionCountChangedTest()
        {
            var fixture = new ReactiveCollection<int>();
            var before_output = new List<int>();
            var output = new List<int>();

            fixture.CountChanging.Subscribe(before_output.Add);
            fixture.CountChanged.Subscribe(output.Add);

            fixture.Add(10);
            fixture.Add(20);
            fixture.Add(30);
            fixture.RemoveAt(1);
            fixture.Clear();

            var before_results = new[] {0,1,2,3,2};
            Assert.Equal(before_results.Length, before_output.Count);
            before_results.AssertAreEqual(before_output);

            var results = new[]{1,2,3,2,0};
            Assert.Equal(results.Length, output.Count);
            results.AssertAreEqual(output);
        }

        [Fact]           
        public void CollectionCountChangedFiresWhenClearing()
        {
            var items = new ReactiveCollection<object>(new []{new object()});
            bool countChanged = false;
            items.CountChanged.Subscribe(_ => {countChanged = true;});

            items.Clear();

            Assert.True(countChanged);
        }

        [Fact]
        public void ItemsAddedAndRemovedTest()
        {
            var fixture = new ReactiveCollection<int>();
            var before_added = new List<int>();
            var before_removed = new List<int>();
            var added = new List<int>();
            var removed = new List<int>();

            fixture.BeforeItemsAdded.Subscribe(before_added.Add);
            fixture.BeforeItemsRemoved.Subscribe(before_removed.Add);
            fixture.ItemsAdded.Subscribe(added.Add);
            fixture.ItemsRemoved.Subscribe(removed.Add);

            fixture.Add(10);
            fixture.Add(20);
            fixture.Add(30);
            fixture.RemoveAt(1);
            fixture.Clear();

            var added_results = new[]{10,20,30};
            Assert.Equal(added_results.Length, added.Count);
            added_results.AssertAreEqual(added);

            var removed_results = new[]{20};
            Assert.Equal(removed_results.Length, removed.Count);
            removed_results.AssertAreEqual(removed);

            Assert.Equal(before_added.Count, added.Count);
            added.AssertAreEqual(before_added);

            Assert.Equal(before_removed.Count, removed.Count);
            removed.AssertAreEqual(before_removed);
        }

        [Fact]
        public void ReactiveCollectionIsRoundTrippable()
        {
            var output = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = new ReactiveCollection<string>(output);

            string json = JSONHelper.Serialize(fixture);
            var results = JSONHelper.Deserialize<ReactiveCollection<string>>(json);

            output.AssertAreEqual(results);

            bool should_die = true;
            results.ItemsAdded.Subscribe(_ => should_die = false);
            results.Add("Foobar");
            Assert.False(should_die);
        }

        [Fact]
        public void ChangeTrackingShouldFireNotifications()
        {
            var fixture = new ReactiveCollection<TestFixture>() { ChangeTrackingEnabled = true };
            var before_output = new List<Tuple<TestFixture, string>>();
            var output = new List<Tuple<TestFixture, string>>();
            var item1 = new TestFixture() { IsOnlyOneWord = "Foo" };
            var item2 = new TestFixture() { IsOnlyOneWord = "Bar" };

            fixture.ItemChanging.Subscribe(x => {
                before_output.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture.ItemChanged.Subscribe(x => {
                output.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture.Add(item1);
            fixture.Add(item2);

            item1.IsOnlyOneWord = "Baz";
            Assert.Equal(1, output.Count);
            item2.IsNotNullString = "FooBar";
            Assert.Equal(2, output.Count);

            fixture.Remove(item2);
            item2.IsNotNullString = "FooBarBaz";
            Assert.Equal(2, output.Count);

            fixture.ChangeTrackingEnabled = false;
            item1.IsNotNullString = "Bamf";
            Assert.Equal(2, output.Count);

            new[]{item1, item2}.AssertAreEqual(output.Select(x => x.Item1));
            new[]{item1, item2}.AssertAreEqual(before_output.Select(x => x.Item1));
            new[]{"IsOnlyOneWord", "IsNotNullString"}.AssertAreEqual(output.Select(x => x.Item2));
        }

        [Fact]
        public void ChangeTrackingShouldWorkWhenAddingTheSameThingMoreThanOnce()
        {
            var fixture = new ReactiveCollection<TestFixture>() { ChangeTrackingEnabled = true };
            var output = new List<Tuple<TestFixture, string>>();
            var item1 = new TestFixture() { IsOnlyOneWord = "Foo" };

            fixture.ItemChanged.Subscribe(x => {
                output.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture.Add(item1);
            fixture.Add(item1);
            fixture.Add(item1);

            item1.IsOnlyOneWord = "Bar";
            Assert.Equal(1, output.Count);

            fixture.RemoveAt(0);

            item1.IsOnlyOneWord = "Baz";
            Assert.Equal(2, output.Count);

            fixture.RemoveAt(0);
            fixture.RemoveAt(0);

            // We've completely removed item1, we shouldn't be seeing any 
            // notifications from it
            item1.IsOnlyOneWord = "Bamf";
            Assert.Equal(2, output.Count);

            fixture.ChangeTrackingEnabled = false;
            fixture.Add(item1);
            fixture.Add(item1);
            fixture.Add(item1);
            fixture.ChangeTrackingEnabled = true;

            item1.IsOnlyOneWord = "Bonk";
            Assert.Equal(3, output.Count);
        }

        [Fact]
        public void CollectionsShouldntShareSubscriptions()
        {
            var fixture1 = new ReactiveCollection<TestFixture>() { ChangeTrackingEnabled = true };
            var fixture2 = new ReactiveCollection<TestFixture>() { ChangeTrackingEnabled = true };
            var item1 = new TestFixture() { IsOnlyOneWord = "Foo" };
            var output1 = new List<Tuple<TestFixture, string>>();
            var output2 = new List<Tuple<TestFixture, string>>();

            fixture1.ItemChanged.Subscribe(x => {
                output1.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture2.ItemChanged.Subscribe(x => {
                output2.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture1.Add(item1);
            fixture1.Add(item1);
            fixture2.Add(item1);
            fixture2.Add(item1);

            item1.IsOnlyOneWord = "Bar";
            Assert.Equal(1, output1.Count);
            Assert.Equal(1, output2.Count);

            fixture2.RemoveAt(0);

            item1.IsOnlyOneWord = "Baz";
            Assert.Equal(2, output1.Count);
            Assert.Equal(2, output2.Count);
        }

        [Fact]
        public void CreateCollectionWithoutTimer()
        {
            var input = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = (new TestScheduler()).With(sched => {
                var f = input.ToObservable(sched).CreateCollection();

                sched.Start();
                return f;
            });
            
            input.AssertAreEqual(fixture);
        }

        [Fact]
        public void CreateCollectionWithTimer()
        {
            var input = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var sched = new TestScheduler();

            using (TestUtils.WithScheduler(sched)) {
                ReactiveCollection<string> fixture;

                fixture = input.ToObservable(sched).CreateCollection(TimeSpan.FromSeconds(0.5));
                sched.AdvanceToMs(1005);
                fixture.AssertAreEqual(input.Take(2));
                
                sched.AdvanceToMs(1505);
                fixture.AssertAreEqual(input.Take(3));
    
                sched.AdvanceToMs(10000);
                fixture.AssertAreEqual(input);
            }
        }

        [Fact]
        public void DerivedCollectionsShouldFollowBaseCollection()
        {
            var input = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = new ReactiveCollection<TestFixture>(
                input.Select(x => new TestFixture() { IsOnlyOneWord = x }));

            var output = fixture.CreateDerivedCollection(new Func<TestFixture, string>(x => x.IsOnlyOneWord));

            input.AssertAreEqual(output);

            fixture.Add(new TestFixture() { IsOnlyOneWord = "Hello" });
            Assert.Equal(5, output.Count);
            Assert.Equal("Hello", output[4]);

            fixture.RemoveAt(4);
            Assert.Equal(4, output.Count);

            fixture[1] = new TestFixture() { IsOnlyOneWord = "Goodbye" };
            Assert.Equal(4, output.Count);
            Assert.Equal("Goodbye", output[1]);

            fixture.Clear();
            Assert.Equal(0, output.Count);
        }

        [Fact]
        public void DerivedCollectionsShouldBeFiltered()
        {
            var input = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = new ReactiveCollection<TestFixture>(
                input.Select(x => new TestFixture() { IsOnlyOneWord = x }));
            var itemsAdded = new List<TestFixture>();
            var itemsRemoved = new List<TestFixture>();

            var output = fixture.CreateDerivedCollection(x => x, x => x.IsOnlyOneWord[0] == 'F', (l,r) => l.IsOnlyOneWord.CompareTo(r.IsOnlyOneWord));
            output.ItemsAdded.Subscribe(itemsAdded.Add);
            output.ItemsRemoved.Subscribe(itemsRemoved.Add);

            Assert.Equal(1, output.Count);
            Assert.Equal(0, itemsAdded.Count);
            Assert.Equal(0, itemsRemoved.Count);

            fixture.Add(new TestFixture() {IsOnlyOneWord = "Boof"});
            Assert.Equal(1, output.Count);
            Assert.Equal(0, itemsAdded.Count);
            Assert.Equal(0, itemsRemoved.Count);

            fixture.Add(new TestFixture() {IsOnlyOneWord = "Far"});
            Assert.Equal(2, output.Count);
            Assert.Equal(1, itemsAdded.Count);
            Assert.Equal(0, itemsRemoved.Count);

            fixture.RemoveAt(1); // Remove "Bar"
            Assert.Equal(2, output.Count);
            Assert.Equal(1, itemsAdded.Count);
            Assert.Equal(0, itemsRemoved.Count);

            fixture.RemoveAt(0); // Remove "Foo"
            Assert.Equal(1, output.Count);
            Assert.Equal(1, itemsAdded.Count);
            Assert.Equal(1, itemsRemoved.Count);
        }

        [Fact]
        public void DerivedCollectionShouldBeSorted()
        {
            var input = new[] { "Foo", "Bar", "Baz" };
            var fixture = new ReactiveCollection<string>(input);

            var output = fixture.CreateDerivedCollection(x => x, orderer: String.CompareOrdinal);

            Assert.Equal(3, output.Count);
            Assert.True(new[] { "Bar", "Baz", "Foo" }.Zip(output, (expected, actual) => expected == actual).All(x => x));

            fixture.Add("Bamf");
            Assert.Equal(4, output.Count);
            Assert.True(new[] { "Bamf", "Bar", "Baz", "Foo" }.Zip(output, (expected, actual) => expected == actual).All(x => x));

            fixture.Add("Eoo");
            Assert.Equal(5, output.Count);
            Assert.True(new[] { "Bamf", "Bar", "Baz", "Eoo", "Foo" }.Zip(output, (expected, actual) => expected == actual).All(x => x));

            fixture.Add("Roo");
            Assert.Equal(6, output.Count);
            Assert.True(new[] { "Bamf", "Bar", "Baz", "Eoo", "Foo", "Roo" }.Zip(output, (expected, actual) => expected == actual).All(x => x));

            fixture.Add("Bar");
            Assert.Equal(7, output.Count);
            Assert.True(new[] { "Bamf", "Bar", "Bar", "Baz", "Eoo", "Foo", "Roo" }.Zip(output, (expected, actual) => expected == actual).All(x => x));
        }

        [Fact]
        public void DerivedCollectionSignalledToResetShouldFireExactlyOnce()
        {
            var input = new List<string> { "Foo" };
            var resetSubject = new Subject<Unit>();
            var derived = input.CreateDerivedCollection(x => x, signalReset: resetSubject);
            
            var changeNotifications = new List<NotifyCollectionChangedEventArgs>();
            derived.Changed.Subscribe(changeNotifications.Add);

            Assert.Equal(0, changeNotifications.Count);
            Assert.Equal(1, derived.Count);

            input.Add("Bar");

            // Shouldn't have picked anything up since the input isn't reactive
            Assert.Equal(0, changeNotifications.Count);
            Assert.Equal(1, derived.Count);

            resetSubject.OnNext(Unit.Default);

            Assert.Equal(1, changeNotifications.Count);
            Assert.Equal(2, derived.Count);
        }

        public class DerivedPropertyChanges
        {
            private class ReactiveVisibilityItem<T> : ReactiveObject
            {
                private T _Value;

                public T Value
                {
                    get { return _Value; }
                    set { this.RaiseAndSetIfChanged(value); }
                }

                private bool _IsVisible;
                public bool IsVisible
                {
                    get { return _IsVisible; }
                    set { this.RaiseAndSetIfChanged(value); }
                }

                public ReactiveVisibilityItem(T item1, bool isVisible)
                {
                    this._Value = item1;
                    this._IsVisible = isVisible;
                }

            }

            [DebuggerDisplay("{Name} is {Age} years old and makes ${Salary}")]
            private class ReactiveEmployee : ReactiveObject
            {
                string _Name;
                public string Name
                {
                    get { return _Name; }
                    set { this.RaiseAndSetIfChanged(ref _Name, value); }
                }

                int _Age;
                public int Age
                {
                    get { return _Age; }
                    set { this.RaiseAndSetIfChanged(ref _Age, value); }
                }

                int _Salary;
                public int Salary
                {
                    get { return _Salary; }
                    set { this.RaiseAndSetIfChanged(ref _Salary, value); }
                }
            }

            public class DerivedCollectionTestContainer
            {
                public static DerivedCollectionTestContainer<TSource, TValue> Create<TSource, TValue>(
                    IEnumerable<TSource> source,
                    Func<TSource, TValue> selector,
                    Func<TSource, bool> filter = null,
                    IComparer<TValue> orderer = null)
                {
                    var comparison = orderer == null ? (Func<TValue, TValue, int>)null : orderer.Compare;
                    var derived = source.CreateDerivedCollection(selector, filter, comparison);

                    return new DerivedCollectionTestContainer<TSource, TValue>
                    {
                        Source = source,
                        Selector = selector,
                        Derived = derived,
                        Filter = filter,
                        Orderer = orderer
                    };
                }

                public virtual void Test() { }
            }

            public class DerivedCollectionTestContainer<TSource, TValue> : DerivedCollectionTestContainer
            {
                public IEnumerable<TSource> Source { get; set; }
                public ReactiveDerivedCollection<TValue> Derived { get; set; }
                public Func<TSource, TValue> Selector { get; set; }
                public Func<TSource, bool> Filter { get; set; }
                public IComparer<TValue> Orderer { get; set; }

                public override void Test()
                {
                    var filtered = Source;

                    if (Filter != null)
                        filtered = filtered.Where(Filter);

                    var projected = filtered.Select(Selector);

                    var ordered = projected;

                    if (Orderer != null)
                        ordered = ordered.OrderBy(x => x, Orderer);

                    var shouldBe = ordered;
                    var isEqual = Derived.SequenceEqual(shouldBe);

                    Assert.True(isEqual);
                }
            }

            [Fact]
            public void DerivedCollectionsSmokeTest()
            {
                var adam = new ReactiveEmployee { Name = "Adam", Age = 20, Salary = 100 };
                var bob = new ReactiveEmployee { Name = "Bob", Age = 30, Salary = 150 };
                var carol = new ReactiveEmployee { Name = "Carol", Age = 40, Salary = 200 };
                var dan = new ReactiveEmployee { Name = "Dan", Age = 50, Salary = 250 };
                var eve = new ReactiveEmployee { Name = "Eve", Age = 60, Salary = 300 };

                var start = new[] { adam, bob, carol, dan, eve };

                var employees = new ReactiveCollection<ReactiveEmployee>(start)
                {
                    ChangeTrackingEnabled = true
                };

                var employeesByName = DerivedCollectionTestContainer.Create(
                    employees,
                    selector: x => x,
                    orderer: OrderedComparer<ReactiveEmployee>.OrderBy(x => x.Name)
                );

                var employeesByAge = DerivedCollectionTestContainer.Create(
                    employees,
                    selector: x => x,
                    orderer: OrderedComparer<ReactiveEmployee>.OrderBy(x => x.Age)
                );

                var employeesBySalary = DerivedCollectionTestContainer.Create(
                    employees,
                    selector: x => x,
                    orderer: OrderedComparer<ReactiveEmployee>.OrderBy(x => x.Salary)
                );

                // special

                // filtered, ordered, reference
                var oldEmployeesByAge = DerivedCollectionTestContainer.Create(
                    employees,
                    selector: x => x,
                    filter: x => x.Age >= 50,
                    orderer: OrderedComparer<ReactiveEmployee>.OrderBy(x => x.Age)
                );

                // ordered, not reference
                var employeeSalaries = DerivedCollectionTestContainer.Create(
                    employees,
                    selector: x => x.Salary,
                    orderer: Comparer<int>.Default
                );

                // not filtered (derived filter), not reference, not ordered (derived order)
                oldEmployeesByAge.Derived.ChangeTrackingEnabled = true;
                var oldEmployeesSalariesByAge = DerivedCollectionTestContainer.Create(
                    oldEmployeesByAge.Derived,
                    selector: x => x.Salary
                );

                var containers = new List<DerivedCollectionTestContainer> {
                    employeesByName, employeesByAge, employeesBySalary, oldEmployeesByAge, 
                    employeeSalaries, oldEmployeesSalariesByAge
                };

                Action<Action> testAll = a => { a(); containers.ForEach(x => x.Test()); };

                containers.ForEach(x => x.Test());

                // if (isIncluded && !shouldBeIncluded)
                testAll(() => { dan.Age = 49; });

                // else if (!isIncluded && shouldBeIncluded)
                testAll(() => { dan.Age = eve.Age + 1; });

                // else if (isIncluded && shouldBeIncluded)
                testAll(() => { adam.Salary = 350; });
                
                testAll(() => { dan.Age = 50; });
                testAll(() => { dan.Age = 51; });
            }

            [Fact]
            public void FilteredDerivedCollectionsShouldReactToPropertyChanges()
            {
                // Naturally this isn't done by magic, it only works if the source implements IReactiveCollection.

                var a = new ReactiveVisibilityItem<string>("a", true);
                var b = new ReactiveVisibilityItem<string>("b", true);
                var c = new ReactiveVisibilityItem<string>("c", true);

                var items = new ReactiveCollection<ReactiveVisibilityItem<string>>(new[] { a, b, c })
                {
                    ChangeTrackingEnabled = true
                };

                var onlyVisible = items.CreateDerivedCollection(
                    x => x.Value, 
                    x => x.IsVisible, 
                    StringComparer.Ordinal.Compare
                );
                
                var onlyNonVisible = items.CreateDerivedCollection(
                    x => x.Value, 
                    x => !x.IsVisible, 
                    StringComparer.Ordinal.Compare
                );

                var onlVisibleStartingWithB = items.CreateDerivedCollection(
                    x => x.Value, 
                    x => x.IsVisible && x.Value.StartsWith("b"), 
                    StringComparer.Ordinal.Compare
                );

                Assert.Equal(3, onlyVisible.Count);
                Assert.Equal(0, onlyNonVisible.Count);
                Assert.Equal(1, onlVisibleStartingWithB.Count);

                a.IsVisible = false;

                Assert.Equal(2, onlyVisible.Count);
                Assert.Equal(1, onlyNonVisible.Count);
                Assert.Equal(1, onlVisibleStartingWithB.Count);

                b.Value = "D";

                Assert.Equal(0, onlVisibleStartingWithB.Count);
            }

            [Fact]
            public void FilteredProjectedDerivedCollectionsShouldReactToPropertyChanges()
            {
                // This differs from the FilteredDerivedCollectionsShouldReactToPropertyChanges as it tests providing a 
                // non-identity selector (ie x=>x.Value).

                var a = new ReactiveVisibilityItem<string>("a", true);
                var b = new ReactiveVisibilityItem<string>("b", true);
                var c = new ReactiveVisibilityItem<string>("c", true);

                var items = new ReactiveCollection<ReactiveVisibilityItem<string>>(new[] { a, b, c })
                {
                    ChangeTrackingEnabled = true
                };

                var onlyVisible = items.CreateDerivedCollection(
                    x => x.Value.ToUpper(), // Note, not an identity function.
                    x => x.IsVisible,
                    StringComparer.Ordinal.Compare
                );

                Assert.Equal(3, onlyVisible.Count);
                Assert.True(onlyVisible.SequenceEqual(new[] { "A", "B", "C" }));

                a.IsVisible = false;

                Assert.Equal(2, onlyVisible.Count);
                Assert.True(onlyVisible.SequenceEqual(new[] { "B", "C" }));
            }

            [Fact]
            public void DerivedCollectionsShouldReactToPropertyChanges()
            {
                // This differs from the FilteredDerivedCollectionsShouldReactToPropertyChanges as it tests providing a 
                // non-identity selector (ie x=>x.Value).

                var foo = new ReactiveVisibilityItem<string>("Foo", true);
                var bar = new ReactiveVisibilityItem<string>("Bar", true);
                var baz = new ReactiveVisibilityItem<string>("Baz", true);

                var items = new ReactiveCollection<ReactiveVisibilityItem<string>>(new[] { foo, bar, baz })
                {
                    ChangeTrackingEnabled = true
                };

                var onlyVisible = items.CreateDerivedCollection(
                    x => new string('*', x.Value.Length), // Note, not an identity function.
                    x => x.IsVisible,
                    StringComparer.Ordinal.Compare
                );

                Assert.Equal(3, onlyVisible.Count);
                Assert.True(onlyVisible.SequenceEqual(new[] { "***", "***", "***" }));

                foo.IsVisible = false;

                Assert.Equal(2, onlyVisible.Count);
                Assert.True(onlyVisible.SequenceEqual(new[] { "***", "***" }));
            }
        }

        [Fact]
        public void AddRangeSmokeTest()
        {
            var fixture = new ReactiveCollection<string>();
            var output = fixture.CreateDerivedCollection(x => "Prefix" + x);

            fixture.Add("Bamf");
            Assert.Equal(1, fixture.Count);
            Assert.Equal(1, output.Count);
            Assert.Equal("Bamf", fixture[0]);
            Assert.Equal("PrefixBamf", output[0]);

            fixture.AddRange(Enumerable.Repeat("Bar", 4));
            Assert.Equal(5, fixture.Count);
            Assert.Equal(5, output.Count);
            Assert.Equal("Bamf", fixture[0]);
            Assert.Equal("PrefixBamf", output[0]);

            Assert.True(fixture.Skip(1).All(x => x == "Bar"));
            Assert.True(output.Skip(1).All(x => x == "PrefixBar"));

            // Trigger the Reset by adding a ton of items
            fixture.AddRange(Enumerable.Repeat("Bar", 35));
            Assert.Equal(40, fixture.Count);
            Assert.Equal(40, output.Count);
            Assert.Equal("Bamf", fixture[0]);
            Assert.Equal("PrefixBamf", output[0]);
        }

        [Fact]
        public void InsertRangeSmokeTest()
        {
            var fixture = new ReactiveCollection<string>();
            var output = fixture.CreateDerivedCollection(x => "Prefix" + x);

            fixture.Add("Bamf");
            Assert.Equal(1, fixture.Count);
            Assert.Equal(1, output.Count);
            Assert.Equal("Bamf", fixture[0]);
            Assert.Equal("PrefixBamf", output[0]);

            fixture.InsertRange(0, Enumerable.Repeat("Bar", 4));
            Assert.Equal(5, fixture.Count);
            Assert.Equal(5, output.Count);
            Assert.Equal("Bamf", fixture[4]);
            Assert.Equal("PrefixBamf", output[4]);

            Assert.True(fixture.Take(4).All(x => x == "Bar"));
            Assert.True(output.Take(4).All(x => x == "PrefixBar"));

            // Trigger the Reset by adding a ton of items
            fixture.InsertRange(0, Enumerable.Repeat("Bar", 35));
            Assert.Equal(40, fixture.Count);
            Assert.Equal(40, output.Count);
            Assert.Equal("Bamf", fixture[39]);
            Assert.Equal("PrefixBamf", output[39]);
        }

        [Fact]
        public void SortShouldActuallySort()
        {
            var fixture = new ReactiveCollection<int>(new[] {5, 1, 3, 2, 4,});
            fixture.Sort();

            Assert.True(new[] {1, 2, 3, 4, 5,}.Zip(fixture, (expected, actual) => expected == actual).All(x => x));
        }

        [Fact]
        public void DerivedCollectionShouldOrderCorrectly()
        {
            var collection = new ReactiveCollection<int>();
            var orderedCollection = collection.CreateDerivedCollection(x => x, null, (x, y) => x.CompareTo(y));

            collection.Add(1);
            collection.Add(2);

            Assert.Equal(2, orderedCollection.Count);
            Assert.Equal(1, orderedCollection[0]);
            Assert.Equal(2, orderedCollection[1]);
        }

        [Fact]
        public void DerivedCollectionShouldStopFollowingAfterDisposal()
        {
            var collection = new ReactiveCollection<int>();

            var orderedCollection = collection.CreateDerivedCollection(
                x => x.ToString(), 
                null, 
                (x, y) => x.CompareTo(y)
            );

            collection.Add(1);
            collection.Add(2);

            Assert.Equal(2, orderedCollection.Count);

            orderedCollection.Dispose();

            collection.Add(3);
            Assert.Equal(2, orderedCollection.Count);
        }


        [Fact]
        public void IListTSmokeTest() {
            var fixture = new ReactiveCollection<string>() as IList<string>;
            Assert.NotNull(fixture);

            fixture.Add("foo");
            Assert.Equal(1, fixture.Count);
            Assert.True(fixture.Contains("foo"));

            fixture.Insert(0, "bar");
            Assert.Equal(0, fixture.IndexOf("bar"));
            Assert.Equal(1, fixture.IndexOf("foo"));
            Assert.Equal("bar", fixture[0]);
            Assert.Equal("foo", fixture[1]);

            var genericEnum = ((IEnumerable<string>)fixture).GetEnumerator();
            Assert.NotNull(genericEnum);
            bool result = genericEnum.MoveNext();
            Assert.True(result);
            Assert.Equal("bar", genericEnum.Current);
            result = genericEnum.MoveNext();
            Assert.True(result);
            Assert.Equal("foo", genericEnum.Current);
            result = genericEnum.MoveNext();
            Assert.False(result);

            var plainEnum = ((IEnumerable)fixture).GetEnumerator();
            Assert.NotNull(plainEnum);
            result = plainEnum.MoveNext();
            Assert.True(result);
            Assert.Equal("bar", plainEnum.Current as string);
            result = plainEnum.MoveNext();
            Assert.True(result);
            Assert.Equal("foo", plainEnum.Current as string);
            result = plainEnum.MoveNext();
            Assert.False(result);

            var arr = new string[2];
            fixture.CopyTo(arr, 0);
            Assert.Equal(2, arr.Length);
            Assert.Equal("bar", arr[0]);
            Assert.Equal("foo", arr[1]);

            fixture[1] = "baz";
            Assert.Equal(1, fixture.IndexOf("baz"));
            Assert.Equal(-1, fixture.IndexOf("foo"));
            Assert.Equal("baz", fixture[1]);
            Assert.False(fixture.Contains("foo"));
            Assert.True(fixture.Contains("baz"));

            fixture.RemoveAt(1);
            Assert.False(fixture.Contains("baz"));

            fixture.Remove("bar");
            Assert.Equal(0, fixture.Count);
            Assert.False(fixture.Contains("bar"));
        }

        [Fact]
        public void IListSmokeTest()
        {
            var fixture = new ReactiveCollection<string>() as IList;
            Assert.NotNull(fixture);

            var pos = fixture.Add("foo");
            Assert.Equal(0, pos);
            Assert.Equal(1, fixture.Count);
            Assert.True(fixture.Contains("foo"));

            fixture.Insert(0, "bar");
            Assert.Equal(0, fixture.IndexOf("bar"));
            Assert.Equal(1, fixture.IndexOf("foo"));
            Assert.Equal("bar", fixture[0] as string);
            Assert.Equal("foo", fixture[1] as string);

            var arr = new string[2];
            fixture.CopyTo(arr, 0);
            Assert.Equal(2, arr.Length);
            Assert.Equal("bar", arr[0]);
            Assert.Equal("foo", arr[1]);

            fixture[1] = "baz";
            Assert.Equal(1, fixture.IndexOf("baz"));
            Assert.Equal(-1, fixture.IndexOf("foo"));
            Assert.Equal("baz", fixture[1] as string);
            Assert.False(fixture.Contains("foo"));
            Assert.True(fixture.Contains("baz"));

            fixture.Remove("bar");
            Assert.Equal(1, fixture.Count);
            Assert.False(fixture.Contains("bar"));
        }
    }

#if SILVERLIGHT
    public class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            using (var mstream = new MemoryStream()) { 
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());  
                serializer.WriteObject(mstream, obj);  
                mstream.Position = 0;  
  
                using (var sr = new StreamReader(mstream)) {  
                    return sr.ReadToEnd();  
                }  
            }
        }

        public static T Deserialize<T>(string json)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(
                new MemoryStream(System.Text.Encoding.Unicode.GetBytes(json)));
        }
    }
#else
    public class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
#endif
}
