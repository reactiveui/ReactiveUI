# Navigation

    phil.cleveland [8:16 AM] 
    @moswald: I see how that could work.  I give up the default binding goodness of the command

    phil.cleveland [8:18 AM]
    @michaelteper: Not sure I follow where you are indicating to put those subjects

    phil.cleveland [8:19 AM]
    My original design had each page impl the Next and Back and also the xaml for the buttons.  I didn't like it because each page then had to know about all the other pages it could possibly go to or go back to.  So I moved all the logic to the shell, but that forces me to recreate the command each time a page changes. (Which I think is legit, but doesn't work)

    moswald [8:20 AM] 
    well, my solution is a little bit wrong, combine it with @michaelteper's

    moswald [8:20 AM]
    pass two `Subject<bool>`s into your page VMs

    moswald [8:20 AM]
    those subjects are your `CanExecute`s

    moswald [8:21 AM]
    that way you keep your `ReactiveCommand` binding goodness

    phil.cleveland [8:21 AM] 
    I see. So set up the cmd with those and then the pages OnNext to define the enabled

    phil.cleveland [8:21 AM]
    Ok.  I like that

    phil.cleveland [8:22 AM]
    Thanks :simple_smile:

    michaelteper [8:23 AM] 
    ```var canGoBack = new Subject<bool>();
    var canGoNext = new Subject<bool>();
    BackButton = ReactiveCommand.Create(canGoBack);
    NextButton = ReactiveCommand.Create(canGoNext);
    this.WhenAnyValue(x => x.Router.CurrentViewModel)
                    .Subscribe(cvm =>
                    {
                        var page = Router.GetCurrentViewModel() as IWizardPage;
                        canGoBack.OnNext(page.CanMoveBack);
                        canGoNext.OnNext(page.CanMoveNext);

              ...
