We suggest the following logical naming pattern when laying out your solution:

<img>


All application logic is stored within a core portable class library which is shared between and referenced in each specific platform application:

.Core
-----

A portable class library that will be the heart of the application and where you will spend most of your time. Profile259 is the recommended profile which should be used, you will need to select it when you create the project.

.Core.Tests
-----------

A standard class library that contains unit tests that confirm functionality of .Core.

.Droid
-------

A monodroid (Xamarin.Android) application which contains Android user interface code for both phone and tablet. Please note .Android namespace prefix is reserved by Google for Android internals and must not be used.

.iOS
-----

A monotouch (Xamarin.iOS) application which contains iOS user interface code for both iPhone and iPad.

.Mac
----

A monomac (Xamarin.Mac) application which contains OSX user interface code.

.WindowsPhone
-------------

A Windows Phone application which contains the user interface code.

.WindowsStore
-------------

Windows 8 Store application with contains the user interface code.

.Wpf
----

Windows Presentation Foundation applications with contains the user interface code.
