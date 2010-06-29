using ReactiveXamlSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ReactiveXamlSample.Tests
{
    [TestClass()]
    public class PersonEntryTest
    {
        PersonEntry createFixture()
        {
            return new PersonEntry() { Name = "Foo", PhoneNumber = "123.456.7890", AwesomenessFactor = 50 };
        }

        [TestMethod()]
        public void PersonEntrySmokeTest()
        {
            var target = createFixture();

            // We didn't blow up? Win!
        }

        [TestMethod()]
        public void AwesomenessFactorTest()
        {
            // 50 is a valid Awesomeness Factor
            var target = createFixture();
            target.AwesomenessFactor = 50;
            Assert.IsTrue(target.IsValid());

            // Awesomeness Factors must be *even*, so this should now be invalid
            target.AwesomenessFactor = 49;
            Assert.IsFalse(target.IsValid());
        }

        [TestMethod()]
        public void NameTest()
        {
            // Foo is a valid name
            var target = createFixture();
            target.Name = "Foo";
            Assert.IsTrue(target.IsValid());

            // Names shouldn't be able to be null
            target.Name = null;
            Assert.IsFalse(target.IsValid());

            // Names shouldn't be too short
            target.Name = "a";
            Assert.IsFalse(target.IsValid());

            // Names shouldn't be too long
            target.Name = String.Join("", Enumerable.Range(0, 100).Select(x => "F"));
            Assert.IsFalse(target.IsValid());

            // Bamf is a valid name too
            target.Name = "Bamf";
            Assert.IsTrue(target.IsValid());
        }

        [TestMethod()]
        public void PhoneNumberTest()
        {
            var target = createFixture();
            var tests = new[] { "555.555.1234", null, "Elephant Robot", "333.3333.3333", "444.444.2222" };
            var results = new[] { true, false, false, false, true };

            foreach(var testcase in tests.Zip(results, (test, result) => new {test, result})) {
                target.PhoneNumber = testcase.test;
                Assert.AreEqual(testcase.result, target.IsValid(), "Testcase: " + (testcase.test ?? "(null)"));
            }
        }
    }
}
