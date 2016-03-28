About this source repository
----------------------------

This is a partial import of rx.codeplex.com for mono.

Since rx.codeplex.com is massive and we only need partial source tree of it
(and due to some checkout failure on Linux [*1]), we set up another
repository for mono submodule.

This tree is hence manually imported. Though it is somewhat easy to maintain:
we try to "cherry-pick" [*2] changes that are applied only to Rx.NET in the
rx.codeplex.com.
Mostly we would not need to copy sources from the original tree manually,
but sometimes we will do so when a checkout involves other directories
than Rx/NET.

(Actually we had to sort of revamp the cherry-pick model when there was
folder structural change in the Microsoft repository. But the structure
is simpler than before and we will likely need less manual imports.)

For every original release, we should import the updates and commit to
this master, then create a branch for each release and *then* apply our
local changes (which is minimum but required) to the branch.

[*1] http://codeplex.codeplex.com/workitem/26133
[*2] not really meaning git cherry-pick which won't apply, but rather
     applying diffs to the tree.

Source changes
--------------

As of OSS release 1.0, there are only two steps are required to make it
possible to build with mono in the source repo:

- apply mono.patch
- cd Rx/NET/Source/Tests.System.Reactive and run "csharp ../../../../replacer.sh"

Actually ObservableExTest.cs cannot be compiled due to insufficient
type inference for lambdas, so I skipped it in Mono.Reactive.Testing_test.dll.

Note that the actual class library build is done in mono/mcs/class and
there is a build script that generates required source list etc.

