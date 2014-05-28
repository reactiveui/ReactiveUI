using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ReactiveUI.Winforms;
using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    public class ActivationTests
    {
        [Fact]
        public void ActivationForViewFetcherSupportsDefaultWinformsComponents()
        {
            var target = new ActivationForViewFetcher();
            var supportedComponents = new[] { typeof(Control), typeof(UserControl), typeof(Form) };

            foreach (var c in supportedComponents) {
                Assert.Equal(10, target.GetAffinityForView(c));
            }
        }

        [Fact]
        public void CanFetchActivatorForForm()
        {
            var form = new TestForm();
            var target = new ActivationForViewFetcher();
            var formActivator = target.GetActivationForView(form);

            Assert.NotNull(formActivator);
            Assert.NotNull(formActivator.Item1);
            Assert.NotNull(formActivator.Item2);
        }
      

        [Fact]
        public void CanFetchActivatorForControl()
        {
            var control = new TestControl();
            var target = new ActivationForViewFetcher();
            var activator = target.GetActivationForView(control);

            Assert.NotNull(activator);
            Assert.NotNull(activator.Item1);
            Assert.NotNull(activator.Item2);
        }

        [Fact]
        public void SmokeTestWindowsForm()
        {
            var target = new ActivationForViewFetcher();
            using (var form = new TestForm()) {
                var formActivator = target.GetActivationForView(form);

                int formActivateCount = 0, formDeActivateCount = 0;
                formActivator.Item1.Subscribe(_ => formActivateCount++);
                formActivator.Item2.Subscribe(_ => formDeActivateCount++);

                Assert.Equal(0, formActivateCount);
                Assert.Equal(0, formDeActivateCount);

                form.Visible = true;
                Assert.Equal(1, formActivateCount);

                form.Visible = false;
                Assert.Equal(1, formActivateCount);
                Assert.Equal(1, formDeActivateCount);

                form.Visible = true;
                Assert.Equal(2, formActivateCount);

                form.Close();
                Assert.Equal(2, formDeActivateCount);
            }
        }

        [Fact]
        public void SmokeTestUserControl()
        {
            var target = new ActivationForViewFetcher();
            using(var userControl = new TestControl())
            using (var parent = new TestForm()) {
                var userControlActivator = target.GetActivationForView(userControl);

                int userControlActivateCount = 0, userControlDeActivateCount = 0;
                userControlActivator.Item1.Subscribe(_ => userControlActivateCount++);
                userControlActivator.Item2.Subscribe(_ => userControlDeActivateCount++);


                parent.Visible = true;
                parent.Controls.Add(userControl);

                userControl.Visible = true;
                Assert.Equal(1, userControlActivateCount);
                userControl.Visible = false;
                Assert.Equal(1, userControlDeActivateCount);

                userControl.Visible = true;
                Assert.Equal(2, userControlActivateCount);

                //closing the form deactivated the usercontrol
                parent.Close();
                Assert.Equal(2, userControlDeActivateCount);
            }
        }

        class TestControl : System.Windows.Forms.Control, IActivatable { }

        class TestForm : System.Windows.Forms.Form, IActivatable { }
    }
}
