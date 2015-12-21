# Automatic Execution
Don't do it from the ViewModel, invoke from the View instead!

See -> https://codereview.stackexchange.com/questions/74642/a-viewmodel-using-reactiveui-6-that-loads-and-sends-data

The only thing I would change, is to not immediately call LoadItems.ExecuteAsyncTask in the ViewModel constructor. Invoking this in the VM constructor means that your VM class becomes more difficult to test, because you always have to mock out the effects of calling LoadItems, even if the thing you are testing is unrelated.

Instead, I always call these commands in the View constructor, something like:

    this.WhenAnyValue(x => x.ViewModel.LoadItems)
        .SelectMany(x => x.ExecuteAsync())
        .Subscribe();
        
This means that any time we get a new ViewModel, we execute LoadItems, which is what we want when the app is running, but not what we want in every unit test.
