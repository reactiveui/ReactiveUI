using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveUI.Subjects
{
    public struct NoneType
    { }

    /// <summary>
    /// Minimal Option Class. Really should be using 
    /// https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Option_A.htm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Option<T>
    {
        public readonly bool IsSome;
        private readonly T value;

        private Option(T value, bool isSome)
        {
            this.value = value;
            this.IsSome = isSome;
        }

        public static implicit operator Option<T>(NoneType none) => None;
        public static Option<T> Some(T value) => new Option<T>(value, true);
        public static Option<T> None => new Option<T>(default(T), false);



        public U Match<U>(Func<U> noneFunc, Func<T, U> someFunc) => IsSome ? someFunc(value) : noneFunc();
        public void Match(Action noneFunc, Action<T> someFunc)
        {
            if (IsSome)
                someFunc(value);
            else
                noneFunc();
        }
        /// <summary>
        /// Monadic bind
        /// </summary>
        public Option<U> Bind<U>(Func<T, Option<U>> fn) => IsSome ? fn( value ) : Option.None;
    };

    public static class Option
    {
        public static Option<T> Some<T>(this T value) => Option<T>.Some(value);
        public static NoneType None => new NoneType();
        public static Option<TU> Select<T, TU>(this Option<T> source, Func<T, TU> fn ) => source.Bind( v=>Some(fn(v)) );
        public static void IfSome<T>( this Option<T> source, Action<T> act ) => source.Match ( () => { } , act );
        public static T IfNone<T>( this Option<T> source, T v) => source.Match ( () => v , q=>q);
        public static T IfNone<T>( this Option<T> source, Func<T> fn) => source.Match ( fn , q=>q);
    }

}
