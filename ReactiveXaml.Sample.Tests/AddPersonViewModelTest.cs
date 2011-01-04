using ReactiveXamlSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace ReactiveXamlSample.Tests
{
    [TestClass()]
    public class AddPersonViewModelTest
    {
        [TestMethod]
        public void OkButtonShouldBeDisabledWhenSomeOfThePropertiesAreInvalid()
        {
            var target = new AddPersonViewModel();

            target.Person = new PersonEntry() {AwesomenessFactor = 50, Name = "Bob", PhoneNumber = "NotAPhoneNumber"};

            Assert.IsFalse(target.OkCommand.CanExecute(null));

            target.Person.PhoneNumber = "555.333.1222";
        
            Assert.IsTrue(target.OkCommand.CanExecute(null));

            target.Person.Name = "";

            Assert.IsFalse(target.OkCommand.CanExecute(null));
        }
    }
}
