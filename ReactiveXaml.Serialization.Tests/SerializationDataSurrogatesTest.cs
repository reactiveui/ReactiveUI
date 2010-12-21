using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveXaml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ReactiveXaml.Serialization.Tests
{
    [TestClass()]
    public abstract class SerializationItemDataSurrogateTest : IEnableLogger
    {
        protected abstract IObjectSerializationProvider createFixture(IEnumerable<IDataContractSurrogate> surrogates);

        [TestMethod()]
        public void SerializationItemSmokeTest()
        {
            var engine = new NullStorageEngine();
            var input = new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}};
            var fixture = new SerializationItemDataSurrogate(engine, input);
            var serializer = createFixture(new IDataContractSurrogate[] {fixture});

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }
    }

    [TestClass()]
    public abstract class SerializedListDataSurrogateTest : IEnableLogger
    {
        protected abstract IObjectSerializationProvider createFixture(IEnumerable<IDataContractSurrogate> surrogates);

        [TestMethod]
        public void SerializedListSmokeTest()
        {
            var engine = new NullStorageEngine();
            var input = new SerializedCollection<SubobjectTestObject>(new[] {
                new SubobjectTestObject() {SomeProperty = "Foo"},
            });
            var surrogates = new IDataContractSurrogate[] {
                new SerializedListDataSurrogate(engine, input),
                new SerializationItemDataSurrogate(engine, input),
            };
            var serializer = createFixture(surrogates);

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }

        [TestMethod]
        public void SerializedListShouldSerializeSubitemsOfEachListItem()
        {
            var engine = new NullStorageEngine();
            var input = new SerializedCollection<ISerializableItem>(new ISerializableItem[] {
                new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}},
                new SubobjectTestObject() {SomeProperty = "Foo"},
            });
            var surrogates = new IDataContractSurrogate[] {
                new SerializedListDataSurrogate(engine, input),
                new SerializationItemDataSurrogate(engine, input),
            };
            var serializer = createFixture(surrogates);

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }

        [TestMethod]
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
            var surrogates = new IDataContractSurrogate[] {
                new SerializedListDataSurrogate(engine, input),
                new SerializationItemDataSurrogate(engine, input),
            };
            var serializer = createFixture(surrogates);

            string json = serializer.SerializedDataToString(serializer.Serialize(input));
            this.Log().Info(json);

            // TODO: Finish this test
        }
    }
}