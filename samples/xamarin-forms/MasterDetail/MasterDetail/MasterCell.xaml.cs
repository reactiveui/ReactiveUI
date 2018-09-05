using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace MasterDetail
{
    public partial class MasterCell : ReactiveViewCell<MasterCellViewModel>
    {
        public MasterCell()
        {
            InitializeComponent();

            // Disposal of this subsciption is *not* necessary because we're simply monitoring
            // the property (ViewModel) on the view itself, so the subscription is attaching to
            // PropertyChanged on this view. This means the view has a reference to itself and
            // thus, doesn't prevent the it from being garbage collected.
            // Note: WPF & UWP *do* require disposal in this scenario, thanks to the way
            // dependency properties work.
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Do(PopulateFromViewModel)
                .Subscribe();
        }

        private void PopulateFromViewModel(MasterCellViewModel viewModel)
        {
            // Because menu items usually don't change for the lifetime of an app (for most use cases),
            // set the values directly instead of binding, for better performance.
            // If your ViewModel properties don't change over time, definitely use this pattern.
            TitleLabel.Text = viewModel.Title;
            IconImage.Source = viewModel.IconSource;
        }
    }
}
