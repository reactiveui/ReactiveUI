# Reactive.Input - Handle inputs the easy way

This document is focused primarily on dealing with keyboard shortcuts, but 
it could be expanded to various other gestures such as touch gestures, etc.

## Scenarios

First, let's list the types of scenarios we run into with real apps. Some of
these are taken from GitHub for Windows.

### Specific scenarios

1. `F1` to refresh. Could have different meaning on different views.
1. `~` to launch a shell from any view. But not when Text input has focus.
1. Arrow keys: Navigate current focused list. Navigate to next list.
1. `Esc` and `ALT+LeftArrow` closes current modal or popup screen.
1. `Enter` navigates to current selected repository.

### Scenario Categories

1. __Key bound to a command__: This is straightforward. You want a specific key
combination to be bound to a specific `ICommand` instance.

1. __Key bound to a navigation command__: In GitHub for Windows, we have the
concept that `Esc` and `ALT+Back` will navigate back from whatever view you
happen to be looking at. This applies to Menus and Modal views. Ideally, we
wouldn't have to bind this to ever view's back command but have a way to have
commands of a certain "class" respond to this key combination.

1. __Shortcut Popover__: We'd like the "?" to launch a popover sheet that's 
automatically generated for the current view.

1. __Global Shortcuts:__ We have `~` mapped to launch a command shell no matter
where you are. This could simply be a command on the `MainWindowViewModel` 
(or `ShellViewModel` depending on what you call it). But it also has to be
smart to ignore cases when `~` is typed within a text input.
