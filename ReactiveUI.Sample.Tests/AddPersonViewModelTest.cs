using ReactiveUI;
using ReactiveUISample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace ReactiveUISample.Tests
{
    [TestClass()]
    public class AddPersonViewModelTest : IEnableLogger
    {
        [TestMethod]
        public void OkButtonShouldBeDisabledWhenSomeOfThePropertiesAreInvalid()
        {
            var target = new AddPersonViewModel();

            target.Person = new PersonEntry() {AwesomenessFactor = 50, Name = "Steve", PhoneNumber = "NotAPhoneNumber"};

            Assert.IsFalse(target.OkCommand.CanExecute(null));

            this.Log().Warn("Setting Phone Number");
            target.Person.PhoneNumber = "555.333.1222";

            this.Log().Info(target.SetImageViaFlickr.CanExecute(null));
            this.Log().Info(target.Person.IsObjectValid());

            this.Log().Warn("About to Assert");
            Assert.IsTrue(target.OkCommand.CanExecute(null));

            target.Person.Name = "";

            Assert.IsFalse(target.OkCommand.CanExecute(null));
        }
    }
}
