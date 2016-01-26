# Here be dragons (tm)
This section of EventBuilder is a hairball of complexity, it has served us well but it was written in a fit of rage and there now exist better ways which that at the time did not exist. If you have the skills and time, we would _really_ appreciate a pull request that contains a refactor of this away from `Mono.Cecil` towards the `System.Reflection.Metadata` nuget package.

Deliverables:

* Unit tests.
* Proper OO modeling instead of static everywhere.
* Refactored to use System.Reflection.Metadata.
* Inline comments explaining what it is doing, how it works and why it's doing what it does.

Here's an example of how to use `System.Reflection.Metadata` we hope it sets you on the right path forward.

```csharp
//string path = @"C:\windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll";
string path = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Xamarin.iOS\v1.0\Xamarin.iOS.dll";
//string path = @"C:\PROGRA~2\Windows Phone Kits\8.1\References\CommonConfiguration\Neutral\Windows.winmd";
using (
    var peReader = new PEReader(new FileStream(path, FileMode.Open, FileAccess.Read),
        PEStreamOptions.PrefetchMetadata))
{
    var contractReader = peReader.GetMetadataReader();

    MetadataReader metadataReader = peReader.GetMetadataReader();
    foreach (var type in metadataReader.TypeDefinitions)
    {
        TypeDefinition typeDefinition = metadataReader.GetTypeDefinition(type);

        foreach (var eventDefinitionHandle in typeDefinition.GetEvents())
        {
            var event = metadataReader.GetEvent(eventDefinitionHandle);
            var eventName = metadataReader.GetString(test.Name);

            Console.WriteLine(eventName);
        }

    }
}
```

