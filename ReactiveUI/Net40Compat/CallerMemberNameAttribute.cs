/// WARNING: This will only work if the project is compiled with VS2012 or higher

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }

}