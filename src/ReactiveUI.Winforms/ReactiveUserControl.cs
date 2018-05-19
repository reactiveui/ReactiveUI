using System.ComponentModel;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    public partial class ReactiveUserControl<TViewModel>: UserControl, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        public ReactiveUserControl()
        {
            components = new Container();
            AutoScaleMode = AutoScaleMode.Font;
        }

        [Category("ReactiveUI")]
        [Description("The ViewModel.")]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
