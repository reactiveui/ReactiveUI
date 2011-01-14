using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace ReactiveXamlSample.Tests
{
    [Export(typeof(IPromptForModelDialog<PersonEntry>))]
    public class DummyAddPartDialog : IPromptForModelDialog<PersonEntry>
    {
        public PersonEntry Prompt(object sender, object parameter)
        {
            return new PersonEntry() { Name = "AddedViaDummyAddPartDialog", PhoneNumber = "444.555.1212", AwesomenessFactor = 50 };
        }
    }

    [TestClass()]
    public class AppViewModelTest
    {
        AppViewModel createFixture()
        {
            var inner_catalog = new AggregateCatalog();
            foreach(var c in new[] { new AssemblyCatalog(this.GetType().Assembly), new AssemblyCatalog(typeof(AppViewModel).Assembly)}) {
                inner_catalog.Catalogs.Add(c);            
            }

            // Hide the real AddPartDialog, so the dummy one gets used
            var catalog = new HijackedPartCatalog(inner_catalog, "wpfAddPersonDialogPrompt");
            var container = new CompositionContainer(catalog);
            return container.GetExportedValue<AppViewModel>();
        }

        [TestMethod()]
        public void CreationSmokeTest()
        {
            var target = createFixture();
        }


        [TestMethod()]
        public void AddPersonTest()
        {
            var target = createFixture();
            var to_add = new PersonEntry() { Name = "Foo", PhoneNumber = "444.444.4444" };

            // Let's try to add a valid object first
            Assert.IsTrue(target.AddPerson.CanExecute(to_add));
            target.AddPerson.Execute(to_add);
            Assert.AreEqual(1, target.People.Count);

            // Now, let's try some evil objects
            target.AddPerson.Execute(new PersonEntry() { Name = null });
            Assert.AreEqual(1, target.People.Count);

            // Try adding a person via the (fake) Add Person dialog
            target.AddPerson.Execute(null);
            Assert.AreEqual(2, target.People.Count);
        }

        [TestMethod()]
        public void RemovePersonTest()
        {
            var target = createFixture();

            // We shouldn't be able to remove anything, it's an empty list
            Assert.IsFalse(target.RemovePerson.CanExecute(null));
            
            // Add a person, make sure it shows up
            var to_add = new PersonEntry() { Name = "Foo", PhoneNumber = "444.444.4444" };
            target.AddPerson.Execute(to_add);
            Assert.AreEqual(1, target.People.Count);

            // SelectedPerson is null, we shouldn't be allowed to remove anything
            Assert.IsFalse(target.RemovePerson.CanExecute(null));

            // Remove the currently selected item
            target.SelectedPerson = to_add;
            target.RemovePerson.Execute(null);
            Assert.AreEqual(0, target.People.Count);
            Assert.IsFalse(target.RemovePerson.CanExecute(null));

            // Now try removing explicitly
            target.AddPerson.Execute(to_add);
            Assert.AreEqual(1, target.People.Count);
            Assert.IsTrue(target.RemovePerson.CanExecute(to_add));
            target.RemovePerson.Execute(to_add);
            Assert.AreEqual(0, target.People.Count);
        }
    }
}
