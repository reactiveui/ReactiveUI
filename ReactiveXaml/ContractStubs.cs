using System;

#if DOTNETISOLDANDSAD

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