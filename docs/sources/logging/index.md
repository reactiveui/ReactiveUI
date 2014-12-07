# Conversations from Slack  

    haacked [8:56 AM] So does rxui6 get rid of this.Log()?

    paulcbetts [8:57 AM] No, it's part of Splat

    paulcbetts [8:57 AM] It just got moved

    haacked [8:59 AM] But there's no implementation for nlog yet.

    paulcbetts [9:00 AM] Correct - you should be able to copy-paste the
    reactiveui-nlog version though

    paulcbetts [9:01 AM] Like, the code is exactly the same, it's just in a
    different assembly

    haacked [9:01 AM] BTW, this.Log() is a static method. So it's effectively the
    same thing. :wink:

    haacked [9:04 AM] I guess the benefit is you don't have to define a static
    variable in every class, which is nice.

    haacked [9:04 AM] Does it somehow use the class defined by `this` to create the
    scope of the logger? So each class still gets its own?

    paulcbetts [9:04 AM] Yeah

    paulcbetts [9:05 AM] That's the scam, is that the `this` is used to set the
    class name for the logger

    haacked [9:07 AM] But I assume every call to `this.Log()` doesn't create a new
    logger. Instead, you look it up based on the class name in some concurrent
    dictionary?

    paulcbetts [9:08 AM] It's stored in a MemoizedMRUCache as I recall

    paulcbetts [9:09 AM] Can't remember the details

    haacked [9:10 AM] :cool: thanks!
