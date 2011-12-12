using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using ReactiveUI.Tests;
using Microsoft.Pex.Framework.Generated;
using Xunit;

namespace ReactiveUI.Serialization.Tests
{
    public class ModelTestFixture : ModelBase
    {
        string _TestString;
        public string TestString {
            get { return _TestString; }
            set { this.RaiseAndSetIfChanged(x => x.TestString, value); }
        }
    }

    [PexClass]
    public partial class ModelBaseTest : IEnableLogger
    {
        [PexMethod]
        public void ItemsChangedShouldFire(string[] setters)
        {
            PexAssume.IsNotNull(setters);
            PexAssume.AreElementsNotNull(setters);

            this.Log().InfoFormat("Setting TestString to [{0}]", String.Join(",", setters));

            (new TestScheduler()).With(sched => {
                var output_changed = new List<object>();
                var output_changing = new List<object>();
                var fixture =  new ModelTestFixture();

                fixture.Changing.Subscribe(output_changing.Add);
                fixture.Changed.Subscribe(output_changed.Add);

                foreach (var v in setters) {
                    fixture.TestString = v; 
                }

                sched.Start();

                PexAssert.AreEqual(setters.Uniq().Count(), output_changed.Count);
                PexAssert.AreEqual(setters.Uniq().Count(), output_changing.Count);
            });
        }

        [PexMethod]
        public void GuidsShouldBeUniqueForContent(string[] setters)
        {
            PexAssume.IsNotNull(setters);
            PexAssume.AreElementsNotNull(setters);

            (new TestScheduler()).With(sched => {
                var fixture = new ModelTestFixture();
                var output = new Dictionary<string, Guid>();

                setters.ToObservable(sched).Subscribe(x => {
                    fixture.TestString = x;
                    output[x] = fixture.ContentHash;
                });

                sched.Start();

                PexAssert.AreDistinctValues(output.Values.ToArray());
            });
        }

        [Fact]
        public void ModelBaseShouldBeObservableAfterDeserialization()
        {
            var dse = new DictionaryStorageEngine();

            (new TestScheduler()).With(sched => {
                var input = new ModelTestFixture() {TestString = "Foo"};

                dse.Save(input);
                var fixture = dse.Load<ModelTestFixture>(input.ContentHash);

                string latest = null;
                var changed = fixture.Changed;
                this.Log().InfoFormat("Subscribing to Changed: 0x{0:X}", changed.GetHashCode());
                changed.Subscribe(Console.WriteLine);
                changed.Subscribe(x => latest = x.PropertyName);
                fixture.TestString = "Bar";

                sched.RunToMilliseconds(1000);

                Assert.Equal("TestString", latest);
            });
        }
    }
}
