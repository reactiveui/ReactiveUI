### Getting log messages to Visual Studio's IntelliTrace

1. Modify Visual Studio's IntelliTrace settings via Debug>IntelliTrace>Open
   IntelliTrace Settings>IntelliTrace Events>Tracing> Check everything except
   Assertion under tracing.  If you leave the default settings, you won't pick
   up Debug level tracing - by checking all of the Trace events, you will pick
   up Debug level messages.

1. Make sure you have the `reactiveui-nlog` package installed to your unit test
   assembly (unfortunately, you are out of luck if you are using a Windows
   Store Test Library, but a "normal" unit test library works fine)

1. Add a file called nlog.config to your unit test project.  __Make sure you
   set the "copy to output directory" property to "Copy if newer" or "Copy
   always"__  If you leave the default action of "Do not copy" then the
   nlog.config file won't be copied to your bin directory and nlog won't be
   able to find its config file, which means it won't know to write to the
   trace listener.

1. Here is the `nlog.config` file

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >
  <targets>
    <target name="trace" xsi:type="trace"  layout="RxUI:${message}"/>
  </targets>
  <rules>
    <logger name="ReactiveUI.*"  writeTo="trace" />
  </rules>
</nlog>
```

1. Register NLogger at the start of your unit test with:

``` cs
var logManager = Locator.Current.GetService<ILogManager>();
Locator.CurrentMutable.RegisterConstant(logManager.GetLogger<NLogLogger>(),typeof(IFullLogger));   
```

*Hint: An easy way to filter the IntelliTrace view to only show ReactiveUI
events is to type RxUI into the IntelliTrace window search box*

