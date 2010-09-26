using System;

#if DOTNETISOLDANDSAD || WINDOWS_PHONE

namespace System.Diagnostics.Contracts
{
    internal class ContractInvariantMethodAttribute : Attribute {}
    
    internal class Contract
    {
        public static void Requires(bool b, string s = null) {}
        public static void Ensures(bool b, string s = null) {}
        public static void Invariant(bool b, string s = null) {}
        public static T Result<T>() { return default(T); }
    }
}

#endif

#if WINDOWS_PHONE
namespace System.Concurrency {}

public class Lazy<T>
{
    public Lazy(Func<T> ValueFetcher) 
    {
        _Value = ValueFetcher();
    }

    T _Value;
    T Value {
        get { return _Value; }
    }
}
#endif