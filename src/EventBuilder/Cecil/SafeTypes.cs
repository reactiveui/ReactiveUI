using Mono.Cecil;
using System.Linq;

namespace EventBuilder.Cecil
{
    public static class SafeTypes
    {
        public static TypeDefinition[] GetSafeTypes(AssemblyDefinition a)
        {
            return a.Modules.SelectMany(x => x.GetTypes()).ToArray();
        }
    }
}