# IReactiveCommand

    Don't use IReactiveCommand for anything, always declare the concrete type. 
    Should've removed that in RxUI 6.0 :-/

See https://github.com/reactiveui/ReactiveUI/issues/710

    I think use interfaces is good idea when we create unit test and other benefits ... Tell me if I'm wrong
    
    In most cases I agree, but ReactiveCommand itself is already designed around testability. Also, the
    likelihood that you will correctly mock IReactiveCommand's semantics via Moq is pretty low, it's a
    pretty complicated class (and if you did, you would end up doing a ton of unnecessary work).
