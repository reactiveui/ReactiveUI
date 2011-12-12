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
        //
        // Here are some properties that are immutable (i.e once we set them
        // in the Constructor, we won't change them). We make them Properties so
        // that WPF/Silverlight can bind to them
        //

        public AppModel Model { get; protected set; }
        public ReactiveCollection<BlockItemTileViewModel> CompletedBlocks { get; protected set; }


        //
        // It's usually a good idea to declare your Commands in a separate
        // section to make your code more readable. Since we'll never change
        // them once they are set up, they are also immutable.
        //

        public ReactiveCommand StartNewBlock { get; protected set; }


        /* COOLSTUFF: The Constructor is where we describe our Interactions
         *
         * In ReactiveUI, the Constructor is the place where we will set up the
         * connections between our objects via Observables. Instead of executing
         * code immediately, we're creating Observables and Subscriptions -
         * we're describing what *will* happen when certain events or property
         * changes happen. 
         *
         * This means that the Constructor will often be the most important
         * method in all of your classes, since it is describing the top-level
         * interactions of the entire ViewModel (i.e. the Behavior of your
         * Application)
         */

        public MainWindowViewModel(AppModel model)
        {
            Model = model;

            CompletedBlocks = Model.CompletedBlocks
                .CreateDerivedCollection(x => new BlockItemTileViewModel(x));

            StartNewBlock = new ReactiveCommand();

            StartNewBlock.Subscribe(dontcare => {
                var window = new BlockTimerWindow();
                window.ShowDialog();

                if (window.ViewModel.Model.IsObjectValid()) {
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
