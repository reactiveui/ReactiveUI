using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI.Serialization;
using Xunit;
using System;

namespace ReactiveUI.Serialization.Tests
{
    public abstract class SerializationItemDataSurrogateTest : IEnableLogger
    {
        protected abstract IObjectSerializationProvider createFixture();

        [Fact()]
        public void SerializationItemSmokeTest()
        {
            var engine = new NullStorageEngine();
            var input = new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}};
            var serializer = createFixture();

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }
    }

    public class JsonNetSerializationItemDataSurrogateTest : SerializationItemDataSurrogateTest
    {
        protected override IObjectSerializationProvider createFixture()
        {
            return new JsonNetObjectSerializationProvider(new NullStorageEngine());
        }
    }

    public abstract class SerializedListDataSurrogateTest : IEnableLogger
    {
        protected abstract IObjectSerializationProvider createFixture();

        [Fact]
        public void SerializedListSmokeTest()
        {
            var engine = new NullStorageEngine();
            var input = new SerializedCollection<SubobjectTestObject>(new[] {
                new SubobjectTestObject() {SomeProperty = "Foo"},
            });
            
            var serializer = createFixture();

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }

        [Fact]
        public void SerializedListShouldSerializeSubitemsOfEachListItem()
        {
            var engine = new NullStorageEngine();
            var input = new SerializedCollection<ISerializableItem>(new ISerializableItem[] {
                new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}},
                new SubobjectTestObject() {SomeProperty = "Foo"},
            });
            var serializer = createFixture();

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }

        [Fact]
        public void ItemsWithSerializedListsShouldOnlyHaveTheHashOfTheList()
        {
            var engine = new NullStorageEngine();
            var list = new SerializedCollection<ISerializableItem>(new ISerializableItem[] {
                new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}},
                new SubobjectTestObject() {SomeProperty = "Foo"},
            });
            var input = new RootObjectWithAListTestObject() {
                SomeList = list,
                RootObject = new RootSerializationTestObject() {SubObject = new SubobjectTestObject {SomeProperty = "Foo"}},
            };

            var serializer = createFixture();

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }
    }

    public class JsonNetSerializedListDataSurrogateTest : SerializedListDataSurrogateTest
    {
        protected override IObjectSerializationProvider createFixture()
        {
            return new JsonNetObjectSerializationProvider(new NullStorageEngine());
        }
    }
}
