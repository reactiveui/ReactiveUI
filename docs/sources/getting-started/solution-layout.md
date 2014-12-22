We suggest the following logical naming pattern when laying out your solution:


* image of visual studio layout video or animated gif of the side-waffle
* extension that creates everything automaticlaly


All application logic is stored within a core portable class library which is
shared between and referenced in each specific platform application:

# Core Application Library

The portable class library will be the heart of your application and where you
will spend most of your time. `Profile78` or `Profile259` is the recommended
profile which should be used, you will need to select it when you create the
project. Alternatively you can adjust the profile after creation by editing
the `.csproj` but you will need to run some nuget commands to reinstall most
of your packages.

EndlessCatsApp.Core.csproj:

```xml
<TargetFrameworkProfile>Profile78</TargetFrameworkProfile>
<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
```

EndlessCatsApp.Core.csproj:

```xml
<TargetFrameworkProfile>Profile259</TargetFrameworkProfile>
<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
```
# Core Application Tests

A standard class library that contains unit tests that confirm functionality
of .Core.

# Target Platform Application

An application of the platform that you are targeting that contains purely of
user interface rendering code and platform specific application logic. Create
your views here and sew your views to your view model using data binding.
Register your platform specific concrete implemtnation of your services ie.
iPhoneNetworkConnectivityService : INetworkConnectiviyService.

Please note Android namespace is reserved by Google for Android internals and
must not be used. This is a Google limitation. Failure to obey this will
result in heaps of pain.


# Target Platform Tests

How you handle this is purely up to you, due to the way xamarin works this is
a good opporunity to write higher level acceptance tests using Xamarin Test
Cloud instead of lower level library tests. Reaosn behind this is code behaves
differently on physical hardware vs emulated hardware and that the linker can
sometimes be too greedy resulting in code being linked out. Only way to pick
up on that is to actually run on physical hardware.
