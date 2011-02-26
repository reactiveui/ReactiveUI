using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Sample.Models;
using ReactiveUI.Xaml;
using ReactiveUI.Sample.Views;

namespace ReactiveUI.Sample.ViewModels
{
    public class MainWindowViewModel : ReactiveValidatedObject
    {
        public AppModel Model { get; protected set; }
        public ReactiveCollection<BlockItemTileViewModel> CompletedBlocks { get; protected set; }

        public ReactiveCommand StartNewBlock { get; protected set; }

        public MainWindowViewModel(AppModel model)
        {
            Model = model;

            CompletedBlocks = Model.CompletedBlocks
                .CreateDerivedCollection(x => new BlockItemTileViewModel(x));

            StartNewBlock = new ReactiveCommand();
            StartNewBlock.Subscribe(dontcare => {
                var window = new BlockTimerWindow();
                window.ShowDialog();

                if (!window.ViewModel.UserPressedCancel) {
                    Model.CompletedBlocks.Add(window.ViewModel.Model);
                }
            });

        }
    }

    public class BlockItemTileViewModel : ReactiveObject
    {
        public BlockItem Model { get; protected set; }

        public BlockItemTileViewModel(BlockItem model)
        {
            Model = model;
        }

        /* COOLSTUFF: Hey, why don't these use RaiseAndSetIfChanged? 
         * 
         * Since BlockItem is a model type who doesn't change once it's added
         * to the ViewModel, we don't need to treat this as a read/write 
         * property. This doesn't stop other parts of the code from updating
         * Model behind our backs, but we are making an explicit decision here
         * to *not* update the value once we fetch it initially */

        public string TimespanAsString {
            get {
                return String.Format("From {0:t} to {1:t}, with {2} pauses",
                    Model.StartedAt, Model.EndedAt, Model.PauseList.Count);
            }
        }
    }
}