.assembly extern mscorlib
{
  .ver 2:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly extern System.CoreEx
{
  .ver 1:0:2787:102
}
.assembly extern System.Core
{
  .ver 3:5:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly extern System.Observable
{
  .ver 1:0:0:102
  .publickeytoken = (31 BF 38 56 AD 36 4E 35 ) // 1.8V.6N5
}
.assembly extern System
{
  .ver 2:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly 'System.Interactive'
{
  .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::'.ctor'(int32) =  (01 00 08 00 00 00 00 00 ) // ........

  .custom instance void class [mscorlib]System.Reflection.AssemblyTitleAttribute::'.ctor'(string) =  (
		01 00 12 53 79 73 74 65 6D 2E 49 6E 74 65 72 61   // ...System.Intera
		63 74 69 76 65 00 00                            ) // ctive..

  .custom instance void class [mscorlib]System.Diagnostics.DebuggableAttribute::'.ctor'(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) =  (01 00 03 00 00 00 00 00 ) // ........

  .custom instance void class [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::'.ctor'() =  (
		01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
		63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01       ) // ceptionThrows.

  .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

  .custom instance void class [mscorlib]System.Reflection.AssemblyCompanyAttribute::'.ctor'(string) =  (
		01 00 15 4D 69 63 72 6F 73 6F 66 74 20 43 6F 72   // ...Microsoft Cor
		70 6F 72 61 74 69 6F 6E 00 00                   ) // poration..

  .custom instance void class [mscorlib]System.Reflection.AssemblyProductAttribute::'.ctor'(string) =  (
		01 00 27 52 65 61 63 74 69 76 65 20 45 78 74 65   // ..'Reactive Exte
		6E 73 69 6F 6E 73 20 49 6E 74 65 72 61 63 74 69   // nsions Interacti
		76 65 20 4C 69 62 72 61 72 79 00 00             ) // ve Library..

  .custom instance void class [mscorlib]System.Reflection.AssemblyCopyrightAttribute::'.ctor'(string) =  (
		01 00 27 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ..'Copyright .. 
		4D 69 63 72 6F 73 6F 66 74 20 43 6F 72 70 6F 72   // Microsoft Corpor
		61 74 69 6F 6E 20 32 30 31 30 00 00             ) // ation 2010..

  .custom instance void class [mscorlib]System.CLSCompliantAttribute::'.ctor'(bool) =  (01 00 01 00 00 ) // .....

  .custom instance void class [mscorlib]System.Reflection.AssemblyFileVersionAttribute::'.ctor'(string) =  (01 00 0A 31 2E 30 2E 32 37 38 37 2E 30 00 00 ) // ...1.0.2787.0..

  .hash algorithm 0x00008004
  .ver  1:0:2787:102
  .publickey = (
		00 24 00 00 04 80 00 00 94 00 00 00 06 02 00 00   // .$..............
		00 24 00 00 52 53 41 31 00 04 00 00 01 00 01 00   // .$..RSA1........
		B5 FC 90 E7 02 7F 67 87 1E 77 3A 8F DE 89 38 C8   // ......g..w:...8.
		1D D4 02 BA 65 B9 20 1D 60 59 3E 96 C4 92 65 1E   // ....e. .`Y>...e.
		88 9C C1 3F 14 15 EB B5 3F AC 11 31 AE 0B D3 33   // ...?....?..1...3
		C5 EE 60 21 67 2D 97 18 EA 31 A8 AE BD 0D A0 07   // ..`!g-...1......
		2F 25 D8 7D BA 6F C9 0F FD 59 8E D4 DA 35 E4 4C   // /%.}.o...Y...5.L
		39 8C 45 43 07 E8 E3 3B 84 26 14 3D AE C9 F5 96   // 9.EC...;.&.=....
		83 6F 97 C8 F7 47 50 E5 97 5C 64 E2 18 9F 45 DE   // .o...GP..\d...E.
		F4 6B 2A 2B 12 47 AD C3 65 2B F5 C3 08 05 5D A9 ) // .k*+.G..e+....].
}
.module System.Interactive.dll // GUID = {85EE248F-5A71-451A-A237-53FAD62A4CA9}


.namespace System.Linq
{
  .class public auto ansi abstract sealed beforefieldinit EnumerableEx
  	extends [mscorlib]System.Object
  {
    .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<float64,float64,float64> 'CS$<>9__CachedAnonymousMethodDelegatee6'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<float32,float32,float32> 'CS$<>9__CachedAnonymousMethodDelegatee8'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegateea'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegateec'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<int64,int64,int64> 'CS$<>9__CachedAnonymousMethodDelegateee'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<float64,valuetype [mscorlib]System.Nullable`1<float64>,float64> 'CS$<>9__CachedAnonymousMethodDelegatef1'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<float64,valuetype [mscorlib]System.Nullable`1<float64>> 'CS$<>9__CachedAnonymousMethodDelegatef2'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<float32,valuetype [mscorlib]System.Nullable`1<float32>,float32> 'CS$<>9__CachedAnonymousMethodDelegatef5'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<float32,valuetype [mscorlib]System.Nullable`1<float32>> 'CS$<>9__CachedAnonymousMethodDelegatef6'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegatef9'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegatefa'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<int32,valuetype [mscorlib]System.Nullable`1<int32>,int32> 'CS$<>9__CachedAnonymousMethodDelegatefd'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<int32,valuetype [mscorlib]System.Nullable`1<int32>> 'CS$<>9__CachedAnonymousMethodDelegatefe'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<int64,valuetype [mscorlib]System.Nullable`1<int64>,int64> 'CS$<>9__CachedAnonymousMethodDelegate101'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<int64,valuetype [mscorlib]System.Nullable`1<int64>> 'CS$<>9__CachedAnonymousMethodDelegate102'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float64,int64>,float64,class '<>f__AnonymousType0`2'<float64,int64>> 'CS$<>9__CachedAnonymousMethodDelegate116'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float64,int64>,float64> 'CS$<>9__CachedAnonymousMethodDelegate117'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float32,int64>,float32,class '<>f__AnonymousType0`2'<float32,int64>> 'CS$<>9__CachedAnonymousMethodDelegate11a'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float32,int64>,float32> 'CS$<>9__CachedAnonymousMethodDelegate11b'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> 'CS$<>9__CachedAnonymousMethodDelegate11e'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegate11f'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int32,class '<>f__AnonymousType0`2'<int64,int64>> 'CS$<>9__CachedAnonymousMethodDelegate122'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> 'CS$<>9__CachedAnonymousMethodDelegate123'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int64,class '<>f__AnonymousType0`2'<int64,int64>> 'CS$<>9__CachedAnonymousMethodDelegate126'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> 'CS$<>9__CachedAnonymousMethodDelegate127'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>> 'CS$<>9__CachedAnonymousMethodDelegate12a'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> 'CS$<>9__CachedAnonymousMethodDelegate12b'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>> 'CS$<>9__CachedAnonymousMethodDelegate12e'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>> 'CS$<>9__CachedAnonymousMethodDelegate12f'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>> 'CS$<>9__CachedAnonymousMethodDelegate132'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate133'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> 'CS$<>9__CachedAnonymousMethodDelegate136'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> 'CS$<>9__CachedAnonymousMethodDelegate137'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> 'CS$<>9__CachedAnonymousMethodDelegate13a'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private static  class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> 'CS$<>9__CachedAnonymousMethodDelegate13b'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 1
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Synchronize<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x20d0
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldstr "Synchronized Enumerable"
	IL_0006:  newobj instance void class [System.CoreEx]System.Concurrency.EventLoopScheduler::'.ctor'(string)
	IL_000b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::ObserveOn<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.CoreEx]System.Concurrency.IScheduler)
	IL_0010:  ret 
    } // end of method EnumerableEx::Synchronize

    // method line 2
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> ObserveOn<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.CoreEx]System.Concurrency.IScheduler scheduler)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x20e2
	// Code size 36 (0x24)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "scheduler"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, class [System.CoreEx]System.Concurrency.IScheduler)
	IL_0023:  ret 
    } // end of method EnumerableEx::ObserveOn

    // method line 3
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> StartWith<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TSource first)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2107
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Return<!!0> (!!0)
	IL_0014:  ldarg.0 
	IL_0015:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Concat<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_001a:  ret 
    } // end of method EnumerableEx::StartWith

    // method line 4
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> StartWith<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TSource[] first)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

	.param [2]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2123
	// Code size 36 (0x24)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "first"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Concat<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  ret 
    } // end of method EnumerableEx::StartWith

    // method line 5
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Do<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x214c
	// Code size 60 (0x3c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldnull 
	IL_001f:  ldftn void class System.Linq.EnumerableEx::'<Do>b__0'<!!0> (class [mscorlib]System.Exception)
	IL_0025:  newobj instance void class [mscorlib]System.Action`1<class [mscorlib]System.Exception>::'.ctor'(object, native int)
	IL_002a:  ldnull 
	IL_002b:  ldftn void class System.Linq.EnumerableEx::'<Do>b__1'<!!0> ()
	IL_0031:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0036:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DoHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_003b:  ret 
    } // end of method EnumerableEx::Do

    // method line 6
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Do<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [System.Core]System.Action onCompleted)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x218b
	// Code size 63 (0x3f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onCompleted"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldnull 
	IL_002d:  ldftn void class System.Linq.EnumerableEx::'<Do>b__2'<!!0> (class [mscorlib]System.Exception)
	IL_0033:  newobj instance void class [mscorlib]System.Action`1<class [mscorlib]System.Exception>::'.ctor'(object, native int)
	IL_0038:  ldarg.2 
	IL_0039:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DoHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_003e:  ret 
    } // end of method EnumerableEx::Do

    // method line 7
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Do<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x21cd
	// Code size 63 (0x3f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onError"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  ldnull 
	IL_002e:  ldftn void class System.Linq.EnumerableEx::'<Do>b__3'<!!0> ()
	IL_0034:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0039:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DoHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_003e:  ret 
    } // end of method EnumerableEx::Do

    // method line 8
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Do<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Observable]System.IObserver`1<!!TSource> observer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2210
	// Code size 74 (0x4a)
	.maxstack 6
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "observer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  dup 
	IL_001f:  ldvirtftn instance void class [System.Observable]System.IObserver`1<!!TSource>::OnNext(!0)
	IL_0025:  newobj instance void class [mscorlib]System.Action`1<!!TSource>::'.ctor'(object, native int)
	IL_002a:  ldarg.1 
	IL_002b:  dup 
	IL_002c:  ldvirtftn instance void class [System.Observable]System.IObserver`1<!!TSource>::OnError(class [mscorlib]System.Exception)
	IL_0032:  newobj instance void class [mscorlib]System.Action`1<class [mscorlib]System.Exception>::'.ctor'(object, native int)
	IL_0037:  ldarg.1 
	IL_0038:  dup 
	IL_0039:  ldvirtftn instance void class [System.Observable]System.IObserver`1<!!TSource>::OnCompleted()
	IL_003f:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0044:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DoHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_0049:  ret 
    } // end of method EnumerableEx::Do

    // method line 9
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Do<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2268
	// Code size 66 (0x42)
	.maxstack 4
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onError"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.3 
	IL_002b:  brtrue.s IL_0038

	IL_002d:  ldstr "onCompleted"
	IL_0032:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0037:  throw 
	IL_0038:  ldarg.0 
	IL_0039:  ldarg.1 
	IL_003a:  ldarg.2 
	IL_003b:  ldarg.3 
	IL_003c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DoHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_0041:  ret 
    } // end of method EnumerableEx::Do

    // method line 10
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DoHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x24b8
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [mscorlib]System.Action`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!0>::'<>3__onNext'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [mscorlib]System.Action`1<class [mscorlib]System.Exception> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!0>::'<>3__onError'
	IL_001d:  ldloc.0 
	IL_001e:  ldarg.3 
	IL_001f:  stfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!!0>::'<>3__onCompleted'
	IL_0024:  ldloc.0 
	IL_0025:  ret 
    } // end of method EnumerableEx::DoHelper

    // method line 11
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Finally<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Action finallyAction)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x24ea
	// Code size 36 (0x24)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "finallyAction"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::FinallyHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Action)
	IL_0023:  ret 
    } // end of method EnumerableEx::Finally

    // method line 12
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> FinallyHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Action finallyAction)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x26f4
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!!0>::'<>3__finallyAction'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::FinallyHelper

    // method line 13
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> BufferWithCount<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count, int32 skip)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2718
	// Code size 53 (0x35)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  ldc.i4.0 
	IL_0010:  bgt.s IL_001d

	IL_0012:  ldstr "count"
	IL_0017:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_001c:  throw 
	IL_001d:  ldarg.2 
	IL_001e:  ldc.i4.0 
	IL_001f:  bgt.s IL_002c

	IL_0021:  ldstr "skip"
	IL_0026:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_002b:  throw 
	IL_002c:  ldarg.0 
	IL_002d:  ldarg.1 
	IL_002e:  ldarg.2 
	IL_002f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::BufferHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32, int32)
	IL_0034:  ret 
    } // end of method EnumerableEx::BufferWithCount

    // method line 14
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> BufferHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count, int32 skip)  cil managed 
    {
        // Method begins at RVA 0x2a30
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!!0>::'<>3__count'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!!0>::'<>3__skip'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::BufferHelper

    // method line 15
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> BufferWithCount<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a5b
	// Code size 38 (0x26)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  ldc.i4.0 
	IL_0010:  bgt.s IL_001d

	IL_0012:  ldstr "count"
	IL_0017:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_001c:  throw 
	IL_001d:  ldarg.0 
	IL_001e:  ldarg.1 
	IL_001f:  ldarg.1 
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::BufferWithCount<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32, int32)
	IL_0025:  ret 
    } // end of method EnumerableEx::BufferWithCount

    // method line 16
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Defer<TSource> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> enumerableFactory)  cil managed 
    {
        // Method begins at RVA 0x2a82
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "enumerableFactory"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.EnumerableEx/DeferredEnumerable`1<!!TSource>::'.ctor'(class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Defer

    // method line 17
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TSource>> Materialize<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a98
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>> class System.Linq.EnumerableEx::MaterializeHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Materialize

    // method line 18
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TSource>> MaterializeHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2cfc
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::MaterializeHelper

    // method line 19
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Dematerialize<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TSource>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2d19
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DematerializeHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Dematerialize

    // method line 20
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DematerializeHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TSource>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2f38
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::DematerializeHelper

    // method line 21
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TAccumulate> Scan<TSource,TAccumulate> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TAccumulate seed, class [System.Core]System.Func`3<!!TAccumulate, !!TSource, !!TAccumulate> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2fd8
	// Code size 99 (0x63)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!TSource, !!TAccumulate>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!TSource, !!TAccumulate>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::source
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::seed
	IL_0014:  ldloc.0 
	IL_0015:  ldarg.2 
	IL_0016:  stfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::accumulator
	IL_001b:  ldloc.0 
	IL_001c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::source
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldstr "source"
	IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_002d:  throw 
	IL_002e:  ldloc.0 
	IL_002f:  ldfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::accumulator
	IL_0034:  brtrue.s IL_0041

	IL_0036:  ldstr "accumulator"
	IL_003b:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0040:  throw 
	IL_0041:  ldloc.0 
	IL_0042:  ldfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!0,!!1>::seed
	IL_0047:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Return<!!1> (!!0)
	IL_004c:  ldloc.0 
	IL_004d:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!!TSource, !!TAccumulate>::'<Scan>b__2b'()
	IL_0053:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TAccumulate>>::'.ctor'(object, native int)
	IL_0058:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!1> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_005d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Concat<!!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0062:  ret 
    } // end of method EnumerableEx::Scan

    // method line 22
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Scan<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`3<!!TSource, !!TSource, !!TSource> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x30d8
	// Code size 76 (0x4c)
	.maxstack 3
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!TSource>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!TSource>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!0>::source
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [System.Core]System.Func`3<!0,!0,!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!0>::accumulator
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!0>::source
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "source"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldfld class [System.Core]System.Func`3<!0,!0,!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!0>::accumulator
	IL_002d:  brtrue.s IL_003a

	IL_002f:  ldstr "accumulator"
	IL_0034:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0039:  throw 
	IL_003a:  ldloc.0 
	IL_003b:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!!TSource>::'<Scan>b__31'()
	IL_0041:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>>::'.ctor'(object, native int)
	IL_0046:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!0> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_004b:  ret 
    } // end of method EnumerableEx::Scan

    // method line 23
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TAccumulate> Scan0<TSource,TAccumulate> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TAccumulate seed, class [System.Core]System.Func`3<!!TAccumulate, !!TSource, !!TAccumulate> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3130
	// Code size 37 (0x25)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "accumulator"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldarg.2 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan0Helper<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0024:  ret 
    } // end of method EnumerableEx::Scan0

    // method line 24
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TAccumulate> Scan0Helper<TSource,TAccumulate> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TAccumulate seed, class [System.Core]System.Func`3<!!TAccumulate, !!TSource, !!TAccumulate> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x33a4
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!!TSource, !!TAccumulate>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!!TSource, !!TAccumulate>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!!0,!!1>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!!0,!!1>::'<>3__seed'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!!0,!!1>::'<>3__accumulator'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::Scan0Helper

    // method line 25
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DistinctUntilChanged<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x33cf
	// Code size 51 (0x33)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "comparer"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DistinctUntilChangedHelper<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!1>)
	IL_0032:  ret 
    } // end of method EnumerableEx::DistinctUntilChanged

    // method line 26
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DistinctUntilChangedHelper<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3648
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!!TSource, !!TKey>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!!TSource, !!TKey>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!!0,!!1>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!!0,!!1>::'<>3__keySelector'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!!0,!!1>::'<>3__comparer'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::DistinctUntilChangedHelper

    // method line 27
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DistinctUntilChanged<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3676
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "comparer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn !!0 class System.Linq.EnumerableEx::'<DistinctUntilChanged>b__48'<!!0> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DistinctUntilChanged<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!1>)
	IL_002f:  ret 
    } // end of method EnumerableEx::DistinctUntilChanged

    // method line 28
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DistinctUntilChanged<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x36a7
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!!TKey>::get_Default()
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DistinctUntilChanged<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::DistinctUntilChanged

    // method line 29
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DistinctUntilChanged<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x36d4
	// Code size 38 (0x26)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldnull 
	IL_0010:  ldftn !!0 class System.Linq.EnumerableEx::'<DistinctUntilChanged>b__49'<!!0> (!!0)
	IL_0016:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_001b:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!!TSource>::get_Default()
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::DistinctUntilChanged<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!1>)
	IL_0025:  ret 
    } // end of method EnumerableEx::DistinctUntilChanged

    // method line 30
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> WhileCore<TSource> (class [System.Core]System.Func`1<bool> condition, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        // Method begins at RVA 0x37e8
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!!0>::'<>3__condition'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!!0>::'<>3__source'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::WhileCore

    // method line 31
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> While<TSource> (class [System.Core]System.Func`1<bool> condition, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        // Method begins at RVA 0x380c
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "condition"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "source"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>> class System.Linq.EnumerableEx::WhileCore<!!0> (class [System.Core]System.Func`1<bool>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Concat<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0028:  ret 
    } // end of method EnumerableEx::While

    // method line 32
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> If<TResult> (class [System.Core]System.Func`1<bool> condition, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> thenSource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> elseSource)  cil managed 
    {
        // Method begins at RVA 0x385c
	// Code size 102 (0x66)
	.maxstack 3
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!TResult>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!TResult>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::condition
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::thenSource
	IL_0014:  ldloc.0 
	IL_0015:  ldarg.2 
	IL_0016:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::elseSource
	IL_001b:  ldloc.0 
	IL_001c:  ldfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::condition
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldstr "condition"
	IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_002d:  throw 
	IL_002e:  ldloc.0 
	IL_002f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::thenSource
	IL_0034:  brtrue.s IL_0041

	IL_0036:  ldstr "thenSource"
	IL_003b:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0040:  throw 
	IL_0041:  ldloc.0 
	IL_0042:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!0>::elseSource
	IL_0047:  brtrue.s IL_0054

	IL_0049:  ldstr "elseSource"
	IL_004e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0053:  throw 
	IL_0054:  ldloc.0 
	IL_0055:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!!TResult>::'<If>b__4d'()
	IL_005b:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::'.ctor'(object, native int)
	IL_0060:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!0> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0065:  ret 
    } // end of method EnumerableEx::If

    // method line 33
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> DoWhile<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`1<bool> condition)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x38ce
	// Code size 42 (0x2a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "condition"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldarg.0 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::While<!!0> (class [System.Core]System.Func`1<bool>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Concat<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0029:  ret 
    } // end of method EnumerableEx::DoWhile

    // method line 34
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Case<TValue,TResult> (class [System.Core]System.Func`1<!!TValue> selector, class [mscorlib]System.Collections.Generic.IDictionary`2<!!TValue, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> sources, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> defaultSource)  cil managed 
    {
        // Method begins at RVA 0x3934
	// Code size 102 (0x66)
	.maxstack 3
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!TValue, !!TResult>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!TValue, !!TResult>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class [System.Core]System.Func`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::selector
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IDictionary`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::sources
	IL_0014:  ldloc.0 
	IL_0015:  ldarg.2 
	IL_0016:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::defaultSource
	IL_001b:  ldloc.0 
	IL_001c:  ldfld class [System.Core]System.Func`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::selector
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldstr "selector"
	IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_002d:  throw 
	IL_002e:  ldloc.0 
	IL_002f:  ldfld class [mscorlib]System.Collections.Generic.IDictionary`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::sources
	IL_0034:  brtrue.s IL_0041

	IL_0036:  ldstr "sources"
	IL_003b:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0040:  throw 
	IL_0041:  ldloc.0 
	IL_0042:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!0,!!1>::defaultSource
	IL_0047:  brtrue.s IL_0054

	IL_0049:  ldstr "defaultSource"
	IL_004e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0053:  throw 
	IL_0054:  ldloc.0 
	IL_0055:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!!TValue, !!TResult>::'<Case>b__50'()
	IL_005b:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::'.ctor'(object, native int)
	IL_0060:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!1> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0065:  ret 
    } // end of method EnumerableEx::Case

    // method line 35
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Case<TValue,TResult> (class [System.Core]System.Func`1<!!TValue> selector, class [mscorlib]System.Collections.Generic.IDictionary`2<!!TValue, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> sources)  cil managed 
    {
        // Method begins at RVA 0x39a6
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "selector"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "sources"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Empty<!!1> ()
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Case<!!0, !!1> (class [System.Core]System.Func`1<!!0>, class [mscorlib]System.Collections.Generic.IDictionary`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::Case

    // method line 36
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> ForCore<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> resultSelector)  cil managed 
    {
        // Method begins at RVA 0x39d0
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<!!0, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0007:  ret 
    } // end of method EnumerableEx::ForCore

    // method line 37
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> For<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> resultSelector)  cil managed 
    {
        // Method begins at RVA 0x39d9
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "resultSelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>> class System.Linq.EnumerableEx::ForCore<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>)
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Concat<!!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0028:  ret 
    } // end of method EnumerableEx::For

    // method line 38
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Let<TValue,TResult> (!!TValue 'value', class [System.Core]System.Func`2<!!TValue, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> selector)  cil managed 
    {
        // Method begins at RVA 0x3a20
	// Code size 57 (0x39)
	.maxstack 3
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!TValue, !!TResult>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!TValue, !!TResult>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!0,!!1>::'value'
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [System.Core]System.Func`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!0,!!1>::selector
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class [System.Core]System.Func`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!0,!!1>::selector
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "selector"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!!TValue, !!TResult>::'<Let>b__53'()
	IL_002e:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::'.ctor'(object, native int)
	IL_0033:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!1> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0038:  ret 
    } // end of method EnumerableEx::Let

    // method line 39
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> SkipLast<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3a65
	// Code size 37 (0x25)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  ldc.i4.0 
	IL_0010:  bge.s IL_001d

	IL_0012:  ldstr "count"
	IL_0017:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_001c:  throw 
	IL_001d:  ldarg.0 
	IL_001e:  ldarg.1 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::SkipLastHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32)
	IL_0024:  ret 
    } // end of method EnumerableEx::SkipLast

    // method line 40
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> SkipLastHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3c60
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!!0>::'<>3__count'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::SkipLastHelper

    // method line 41
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> TakeLast<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3c84
	// Code size 37 (0x25)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  ldc.i4.0 
	IL_0010:  bge.s IL_001d

	IL_0012:  ldstr "count"
	IL_0017:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_001c:  throw 
	IL_001d:  ldarg.0 
	IL_001e:  ldarg.1 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::TakeLastHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32)
	IL_0024:  ret 
    } // end of method EnumerableEx::TakeLast

    // method line 42
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> TakeLastHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 count)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3e28
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!!0>::'<>3__count'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::TakeLastHelper

    // method line 43
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Remotable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3e4c
	// Code size 26 (0x1a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.EnumerableEx/RemotableEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_0014:  newobj instance void class System.Linq.EnumerableEx/SerializableEnumerable`1<!!TSource>::'.ctor'(class System.Linq.EnumerableEx/RemotableEnumerable`1<!0>)
	IL_0019:  ret 
    } // end of method EnumerableEx::Remotable

    // method line 44
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TOther> SelectMany<TSource,TOther> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TOther> other)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3e78
	// Code size 65 (0x41)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!!TSource, !!TOther>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!!TSource, !!TOther>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!!0,!!1>::other
	IL_000d:  ldarg.0 
	IL_000e:  brtrue.s IL_001b

	IL_0010:  ldstr "source"
	IL_0015:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001a:  throw 
	IL_001b:  ldloc.0 
	IL_001c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!!0,!!1>::other
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldstr "other"
	IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_002d:  throw 
	IL_002e:  ldarg.0 
	IL_002f:  ldloc.0 
	IL_0030:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!!TSource, !!TOther>::'<SelectMany>b__61'(!0)
	IL_0036:  newobj instance void class [System.Core]System.Func`2<!!TSource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TOther>>::'.ctor'(object, native int)
	IL_003b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::SelectMany<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>)
	IL_0040:  ret 
    } // end of method EnumerableEx::SelectMany

    // method line 45
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> OnErrorResumeNext<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> first, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> second)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3ec8
	// Code size 50 (0x32)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>[]	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "first"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "second"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldc.i4.2 
	IL_001d:  newarr class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>
	IL_0022:  stloc.0 
	IL_0023:  ldloc.0 
	IL_0024:  ldc.i4.0 
	IL_0025:  ldarg.0 
	IL_0026:  stelem.ref 
	IL_0027:  ldloc.0 
	IL_0028:  ldc.i4.1 
	IL_0029:  ldarg.1 
	IL_002a:  stelem.ref 
	IL_002b:  ldloc.0 
	IL_002c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::OnErrorResumeNext<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>[])
	IL_0031:  ret 
    } // end of method EnumerableEx::OnErrorResumeNext

    // method line 46
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> OnErrorResumeNext<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>[] sources)  cil managed 
    {
	.param [1]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3f06
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::OnErrorResumeNext<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0006:  ret 
    } // end of method EnumerableEx::OnErrorResumeNext

    // method line 47
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> OnErrorResumeNext<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3f0e
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::OnErrorResumeNextHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::OnErrorResumeNext

    // method line 48
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> OnErrorResumeNextHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        // Method begins at RVA 0x4154
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!!0>::'<>3__sources'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::OnErrorResumeNextHelper

    // method line 49
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Concat<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>[] sources)  cil managed 
    {
	.param [1]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4171
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::ConcatHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Concat

    // method line 50
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Concat<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4187
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::ConcatHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Concat

    // method line 51
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> ConcatHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        // Method begins at RVA 0x43b4
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!!0>::'<>3__sources'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::ConcatHelper

    // method line 52
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Catch<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> first, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> second)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x43d4
	// Code size 50 (0x32)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>[]	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "first"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "second"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldc.i4.2 
	IL_001d:  newarr class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>
	IL_0022:  stloc.0 
	IL_0023:  ldloc.0 
	IL_0024:  ldc.i4.0 
	IL_0025:  ldarg.0 
	IL_0026:  stelem.ref 
	IL_0027:  ldloc.0 
	IL_0028:  ldc.i4.1 
	IL_0029:  ldarg.1 
	IL_002a:  stelem.ref 
	IL_002b:  ldloc.0 
	IL_002c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::CatchHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0031:  ret 
    } // end of method EnumerableEx::Catch

    // method line 53
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Catch<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>[] sources)  cil managed 
    {
	.param [1]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4412
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::CatchHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Catch

    // method line 54
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Catch<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        // Method begins at RVA 0x4428
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::CatchHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Catch

    // method line 55
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> CatchHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> sources)  cil managed 
    {
        // Method begins at RVA 0x4674
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!!0>::'<>3__sources'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::CatchHelper

    // method line 56
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Catch<TSource,(class [mscorlib]System.Exception) TException> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TException, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> 'handler')  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4691
	// Code size 36 (0x24)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "handler"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::CatchHelper<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0023:  ret 
    } // end of method EnumerableEx::Catch

    // method line 57
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> CatchHelper<TSource,(class [mscorlib]System.Exception) TException> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TException, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> 'handler')  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x49d0
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!!TSource, !!TException>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!!TSource, !!TException>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!!0,!!1>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!!0,!!1>::'<>3__handler'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::CatchHelper

    // method line 58
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Zip<TFirst,TSecond,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TFirst> first, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSecond> second, class [System.Core]System.Func`3<!!TFirst, !!TSecond, !!TResult> resultSelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x49f4
	// Code size 51 (0x33)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "first"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "second"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "resultSelector"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> class System.Linq.EnumerableEx::ZipIterator<!!0, !!1, !!2> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>, class [System.Core]System.Func`3<!!0,!!1,!!2>)
	IL_0032:  ret 
    } // end of method EnumerableEx::Zip

    // method line 59
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> ZipIterator<TFirst,TSecond,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TFirst> first, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSecond> second, class [System.Core]System.Func`3<!!TFirst, !!TSecond, !!TResult> resultSelector)  cil managed 
    {
        // Method begins at RVA 0x4c4c
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!!TFirst, !!TSecond, !!TResult>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!!TFirst, !!TSecond, !!TResult>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!!0,!!1,!!2>::'<>3__first'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!!0,!!1,!!2>::'<>3__second'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [System.Core]System.Func`3<!0,!1,!2> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!!0,!!1,!!2>::'<>3__resultSelector'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::ZipIterator

    // method line 60
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T[]> ForkJoin<T> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T>> sources)  cil managed 
    {
        // Method begins at RVA 0x4c77
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0[]> class System.Linq.EnumerableEx::ForkJoinHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::ForkJoin

    // method line 61
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T[]> ForkJoin<T> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T>[] sources)  cil managed 
    {
	.param [1]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4c8d
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "sources"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0[]> class System.Linq.EnumerableEx::ForkJoinHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::ForkJoin

    // method line 62
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T[]> ForkJoinHelper<T> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T>> sources)  cil managed 
    {
        // Method begins at RVA 0x4e2c
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!!T>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!!T>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!!0>::'<>3__sources'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::ForkJoinHelper

    // method line 63
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Return<TSource> (!!TSource 'value')  cil managed 
    {
        // Method begins at RVA 0x4f24
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Return>d__96`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Return>d__96`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!!0>::'<>3__value'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::Return

    // method line 64
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Throw<TSource> (class [mscorlib]System.Exception exception)  cil managed 
    {
        // Method begins at RVA 0x4f41
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "exception"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::ThrowHelper<!!0> (class [mscorlib]System.Exception)
	IL_0014:  ret 
    } // end of method EnumerableEx::Throw

    // method line 65
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> ThrowHelper<TSource> (class [mscorlib]System.Exception exception)  cil managed 
    {
        // Method begins at RVA 0x500c
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!!0>::'<>3__exception'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::ThrowHelper

    // method line 66
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Never<TSource> ()  cil managed 
    {
        // Method begins at RVA 0x50d4
	// Code size 10 (0xa)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Never>d__9c`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Never>d__9c`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ret 
    } // end of method EnumerableEx::Never

    // method line 67
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Generate<TState,TResult> (!!TState initialState, class [System.Core]System.Func`2<!!TState, bool> condition, class [System.Core]System.Func`2<!!TState, !!TResult> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x50ea
	// Code size 52 (0x34)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "condition"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.3 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "iterate"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "resultSelector"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  ldarg.3 
	IL_002e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::GenerateHelper<!!0, !!1> (!!0, class [System.Core]System.Func`2<!!0,bool>, class [System.Core]System.Func`2<!!0,!!1>, class [System.Core]System.Func`2<!!0,!!0>)
	IL_0033:  ret 
    } // end of method EnumerableEx::Generate

    // method line 68
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> GenerateHelper<TState,TResult> (!!TState initialState, class [System.Core]System.Func`2<!!TState, bool> condition, class [System.Core]System.Func`2<!!TState, !!TResult> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x525c
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!TState, !!TResult>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!TState, !!TResult>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!0,!!1>::'<>3__initialState'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!0,!!1>::'<>3__condition'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!0,!!1>::'<>3__resultSelector'
	IL_001d:  ldloc.0 
	IL_001e:  ldarg.3 
	IL_001f:  stfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!!0,!!1>::'<>3__iterate'
	IL_0024:  ldloc.0 
	IL_0025:  ret 
    } // end of method EnumerableEx::GenerateHelper

    // method line 69
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> Generate<TValue> (class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TValue>> function)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x528e
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "function"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::GenerateHelper<!!0> (class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Generate

    // method line 70
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> GenerateHelper<TValue> (class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!TValue>> function)  cil managed 
    {
        // Method begins at RVA 0x53a0
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!!TValue>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!!TValue>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!!0>::'<>3__function'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::GenerateHelper

    // method line 71
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Generate<TState,TResult> (!!TState initialState, class [System.Core]System.Func`2<!!TState, class [System.CoreEx]System.Collections.Generic.Notification`1<!!TResult>> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x53bd
	// Code size 37 (0x25)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "resultSelector"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "iterate"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldarg.2 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::GenerateHelper<!!0, !!1> (!!0, class [System.Core]System.Func`2<!!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!!1>>, class [System.Core]System.Func`2<!!0,!!0>)
	IL_0024:  ret 
    } // end of method EnumerableEx::Generate

    // method line 72
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> GenerateHelper<TState,TResult> (!!TState initialState, class [System.Core]System.Func`2<!!TState, class [System.CoreEx]System.Collections.Generic.Notification`1<!!TResult>> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x5520
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!!TState, !!TResult>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!!TState, !!TResult>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!!0,!!1>::'<>3__initialState'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!!0,!!1>::'<>3__resultSelector'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!!0,!!1>::'<>3__iterate'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::GenerateHelper

    // method line 73
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Generate<TState,TResult> (!!TState initial, class [System.Core]System.Func`2<!!TState, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x554e
	// Code size 49 (0x31)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "resultSelector"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "iterate"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn bool class System.Linq.EnumerableEx::'<Generate>b__ac'<!!0, !!1> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TState, bool>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  ldarg.2 
	IL_002b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Generate<!!0, !!1> (!!0, class [System.Core]System.Func`2<!!0,bool>, class [System.Core]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>, class [System.Core]System.Func`2<!!0,!!0>)
	IL_0030:  ret 
    } // end of method EnumerableEx::Generate

    // method line 74
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Generate<TState,TResult> (!!TState initial, class [System.Core]System.Func`2<!!TState, bool> condition, class [System.Core]System.Func`2<!!TState, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> resultSelector, class [System.Core]System.Func`2<!!TState, !!TState> iterate)  cil managed 
    {
        // Method begins at RVA 0x5580
	// Code size 57 (0x39)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "condition"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "resultSelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.3 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "iterate"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  ldarg.3 
	IL_002e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Generate<!!0, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>> (!!0, class [System.Core]System.Func`2<!!0,bool>, class [System.Core]System.Func`2<!!0,!!1>, class [System.Core]System.Func`2<!!0,!!0>)
	IL_0033:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Concat<!!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_0038:  ret 
    } // end of method EnumerableEx::Generate

    // method line 75
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Using<TSource,(class [mscorlib]System.IDisposable) TResource> (class [System.Core]System.Func`1<!!TResource> resourceSelector, class [System.Core]System.Func`2<!!TResource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>> resourceUsage)  cil managed 
    {
        // Method begins at RVA 0x5600
	// Code size 76 (0x4c)
	.maxstack 3
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!TSource, !!TResource>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!TSource, !!TResource>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class [System.Core]System.Func`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!0,!!1>::resourceSelector
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!0,!!1>::resourceUsage
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class [System.Core]System.Func`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!0,!!1>::resourceSelector
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "resourceSelector"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!0,!!1>::resourceUsage
	IL_002d:  brtrue.s IL_003a

	IL_002f:  ldstr "resourceUsage"
	IL_0034:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0039:  throw 
	IL_003a:  ldloc.0 
	IL_003b:  ldftn instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!!TSource, !!TResource>::'<Using>b__ad'()
	IL_0041:  newobj instance void class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>>::'.ctor'(object, native int)
	IL_0046:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Defer<!!0> (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
	IL_004b:  ret 
    } // end of method EnumerableEx::Using

    // method line 76
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Repeat<TSource> (!!TSource 'value')  cil managed 
    {
        // Method begins at RVA 0x5730
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!!0>::'<>3__value'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::Repeat

    // method line 77
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Repeat<TSource> (!!TSource 'value', int32 repeatCount)  cil managed 
    {
        // Method begins at RVA 0x5858
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!!0>::'<>3__value'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!!0>::'<>3__repeatCount'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::Repeat

    // method line 78
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Repeat<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x587c
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::RepeatHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Repeat

    // method line 79
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> RepeatHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        // Method begins at RVA 0x5a28
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::RepeatHelper

    // method line 80
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Repeat<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 repeatCount)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5a45
	// Code size 22 (0x16)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::RepeatHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32)
	IL_0015:  ret 
    } // end of method EnumerableEx::Repeat

    // method line 81
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> RepeatHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 repeatCount)  cil managed 
    {
        // Method begins at RVA 0x5c20
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!!0>::'<>3__repeatCount'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::RepeatHelper

    // method line 82
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> Retry<TValue> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5df0
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!!TValue>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!!TValue>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::Retry

    // method line 83
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> Retry<TValue> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TValue> source, int32 retryCount)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5ff0
	// Code size 24 (0x18)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!!TValue>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!!TValue>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!!0>::'<>3__retryCount'
	IL_0016:  ldloc.0 
	IL_0017:  ret 
    } // end of method EnumerableEx::Retry

    // method line 84
    .method public static hidebysig 
           default !!TSource Min<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6017
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "comparer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn !!0 class System.Linq.EnumerableEx::'<Min>b__cf'<!!0> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  call !!0 class System.Linq.EnumerableEx::MinBy<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_002f:  ret 
    } // end of method EnumerableEx::Min

    // method line 85
    .method public static hidebysig 
           default !!TSource MinBy<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6048
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.Comparer`1<!0> class [mscorlib]System.Collections.Generic.Comparer`1<!!TKey>::get_Default()
	IL_0023:  call !!0 class System.Linq.EnumerableEx::MinBy<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::MinBy

    // method line 86
    .method public static hidebysig 
           default !!TSource MinBy<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x608c
	// Code size 80 (0x50)
	.maxstack 5
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!!TSource, !!TKey>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!!TSource, !!TKey>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.2 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!!0,!!1>::comparer
	IL_000d:  ldarg.0 
	IL_000e:  brtrue.s IL_001b

	IL_0010:  ldstr "source"
	IL_0015:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001a:  throw 
	IL_001b:  ldarg.1 
	IL_001c:  brtrue.s IL_0029

	IL_001e:  ldstr "keySelector"
	IL_0023:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0028:  throw 
	IL_0029:  ldloc.0 
	IL_002a:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!!0,!!1>::comparer
	IL_002f:  brtrue.s IL_003c

	IL_0031:  ldstr "comparer"
	IL_0036:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_003b:  throw 
	IL_003c:  ldarg.0 
	IL_003d:  ldarg.1 
	IL_003e:  ldloc.0 
	IL_003f:  ldftn instance bool class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!!TSource, !!TKey>::'<MinBy>b__d0'(!1, !1)
	IL_0045:  newobj instance void class [System.Core]System.Func`3<!!TKey, !!TKey, bool>::'.ctor'(object, native int)
	IL_004a:  call !!0 class System.Linq.EnumerableEx::ExtremaBy<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [System.Core]System.Func`3<!!1,!!1,bool>)
	IL_004f:  ret 
    } // end of method EnumerableEx::MinBy

    // method line 87
    .method public static hidebysig 
           default !!TSource Max<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x60eb
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "comparer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn !!0 class System.Linq.EnumerableEx::'<Max>b__d3'<!!0> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  call !!0 class System.Linq.EnumerableEx::MaxBy<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_002f:  ret 
    } // end of method EnumerableEx::Max

    // method line 88
    .method public static hidebysig 
           default !!TSource MaxBy<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x611c
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.Comparer`1<!0> class [mscorlib]System.Collections.Generic.Comparer`1<!!TKey>::get_Default()
	IL_0023:  call !!0 class System.Linq.EnumerableEx::MaxBy<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::MaxBy

    // method line 89
    .method public static hidebysig 
           default !!TSource MaxBy<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6160
	// Code size 80 (0x50)
	.maxstack 5
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!!TSource, !!TKey>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!!TSource, !!TKey>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.2 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!!0,!!1>::comparer
	IL_000d:  ldarg.0 
	IL_000e:  brtrue.s IL_001b

	IL_0010:  ldstr "source"
	IL_0015:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001a:  throw 
	IL_001b:  ldarg.1 
	IL_001c:  brtrue.s IL_0029

	IL_001e:  ldstr "keySelector"
	IL_0023:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0028:  throw 
	IL_0029:  ldloc.0 
	IL_002a:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!!0,!!1>::comparer
	IL_002f:  brtrue.s IL_003c

	IL_0031:  ldstr "comparer"
	IL_0036:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_003b:  throw 
	IL_003c:  ldarg.0 
	IL_003d:  ldarg.1 
	IL_003e:  ldloc.0 
	IL_003f:  ldftn instance bool class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!!TSource, !!TKey>::'<MaxBy>b__d4'(!1, !1)
	IL_0045:  newobj instance void class [System.Core]System.Func`3<!!TKey, !!TKey, bool>::'.ctor'(object, native int)
	IL_004a:  call !!0 class System.Linq.EnumerableEx::ExtremaBy<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [System.Core]System.Func`3<!!1,!!1,bool>)
	IL_004f:  ret 
    } // end of method EnumerableEx::MaxBy

    // method line 90
    .method private static hidebysig 
           default !!TSource ExtremaBy<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [System.Core]System.Func`3<!!TKey, !!TKey, bool> compare)  cil managed 
    {
        // Method begins at RVA 0x61bc
	// Code size 101 (0x65)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource>	V_0,
		!!TSource	V_1,
		!!TKey	V_2,
		!!TSource	V_3,
		!!TKey	V_4,
		!!TSource	V_5)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>::GetEnumerator()
	IL_0006:  stloc.0 
	.try { // 0
	  IL_0007:  ldloc.0 
	  IL_0008:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_000d:  brtrue.s IL_001a

	  IL_000f:  ldstr "Source sequence doesn't contain any elements."
	  IL_0014:  newobj instance void class [mscorlib]System.InvalidOperationException::'.ctor'(string)
	  IL_0019:  throw 
	  IL_001a:  ldloc.0 
	  IL_001b:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource>::get_Current()
	  IL_0020:  stloc.1 
	  IL_0021:  ldarg.1 
	  IL_0022:  ldloc.1 
	  IL_0023:  callvirt instance !1 class [System.Core]System.Func`2<!!TSource, !!TKey>::Invoke(!0)
	  IL_0028:  stloc.2 
	  IL_0029:  br.s IL_004b

	  IL_002b:  ldloc.0 
	  IL_002c:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource>::get_Current()
	  IL_0031:  stloc.3 
	  IL_0032:  ldarg.1 
	  IL_0033:  ldloc.3 
	  IL_0034:  callvirt instance !1 class [System.Core]System.Func`2<!!TSource, !!TKey>::Invoke(!0)
	  IL_0039:  stloc.s 4
	  IL_003b:  ldarg.2 
	  IL_003c:  ldloc.s 4
	  IL_003e:  ldloc.2 
	  IL_003f:  callvirt instance !2 class [System.Core]System.Func`3<!!TKey, !!TKey, bool>::Invoke(!0, !1)
	  IL_0044:  brfalse.s IL_004b

	  IL_0046:  ldloc.3 
	  IL_0047:  stloc.1 
	  IL_0048:  ldloc.s 4
	  IL_004a:  stloc.2 
	  IL_004b:  ldloc.0 
	  IL_004c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0051:  brtrue.s IL_002b

	  IL_0053:  ldloc.1 
	  IL_0054:  stloc.s 5
	  IL_0056:  leave.s IL_0062

	} // end .try 0
	finally  { // 0
	  IL_0058:  ldloc.0 
	  IL_0059:  brfalse.s IL_0061

	  IL_005b:  ldloc.0 
	  IL_005c:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0061:  endfinally 
	} // end handler 0
	IL_0062:  ldloc.s 5
	IL_0064:  ret 
    } // end of method EnumerableEx::ExtremaBy

    // method line 91
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6240
	// Code size 51 (0x33)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>::GetEnumerator()
	IL_0014:  stloc.0 
	.try { // 0
	  IL_0015:  br.s IL_001e

	  IL_0017:  ldloc.0 
	  IL_0018:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource>::get_Current()
	  IL_001d:  pop 
	  IL_001e:  ldloc.0 
	  IL_001f:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0024:  brtrue.s IL_0017

	  IL_0026:  leave.s IL_0032

	} // end .try 0
	finally  { // 0
	  IL_0028:  ldloc.0 
	  IL_0029:  brfalse.s IL_0031

	  IL_002b:  ldloc.0 
	  IL_002c:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0031:  endfinally 
	} // end handler 0
	IL_0032:  ret 
    } // end of method EnumerableEx::Run

    // method line 92
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6290
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Do<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>)
	IL_0023:  call void class System.Linq.EnumerableEx::Run<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0028:  ret 
    } // end of method EnumerableEx::Run

    // method line 93
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x62ba
	// Code size 56 (0x38)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onError"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Do<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>)
	IL_0032:  call void class System.Linq.EnumerableEx::Run<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0037:  ret 
    } // end of method EnumerableEx::Run

    // method line 94
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [System.Core]System.Action onCompleted)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x62f3
	// Code size 56 (0x38)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onCompleted"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Do<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [System.Core]System.Action)
	IL_0032:  call void class System.Linq.EnumerableEx::Run<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0037:  ret 
    } // end of method EnumerableEx::Run

    // method line 95
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Action`1<!!TSource> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x632c
	// Code size 71 (0x47)
	.maxstack 4
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "onNext"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "onError"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.3 
	IL_002b:  brtrue.s IL_0038

	IL_002d:  ldstr "onCompleted"
	IL_0032:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0037:  throw 
	IL_0038:  ldarg.0 
	IL_0039:  ldarg.1 
	IL_003a:  ldarg.2 
	IL_003b:  ldarg.3 
	IL_003c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Do<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Action`1<!!0>, class [mscorlib]System.Action`1<class [mscorlib]System.Exception>, class [System.Core]System.Action)
	IL_0041:  call void class System.Linq.EnumerableEx::Run<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0046:  ret 
    } // end of method EnumerableEx::Run

    // method line 96
    .method public static hidebysig 
           default void Run<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Observable]System.IObserver`1<!!TSource> observer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x637f
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "observer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Do<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Observable]System.IObserver`1<!!0>)
	IL_0023:  call void class System.Linq.EnumerableEx::Run<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0028:  ret 
    } // end of method EnumerableEx::Run

    // method line 97
    .method public static hidebysig 
           default bool IsEmpty<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x63a9
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call bool class [System.Core]System.Linq.Enumerable::Any<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0006:  ldc.i4.0 
	IL_0007:  ceq 
	IL_0009:  ret 
    } // end of method EnumerableEx::IsEmpty

    // method line 98
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Let<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x63b4
	// Code size 36 (0x24)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_0023:  ret 
    } // end of method EnumerableEx::Let

    // method line 99
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> MemoizeAll<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x63d9
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::MemoizeAll

    // method line 100
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Memoize<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x63ef
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Memoize

    // method line 101
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Publish<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6405
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::MemoizeAll<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_0028:  ret 
    } // end of method EnumerableEx::Publish

    // method line 102
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Publish<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function, !!TSource initialValue)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x642f
	// Code size 47 (0x2f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::MemoizeAll<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  ldarg.2 
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::StartWith<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!0)
	IL_0029:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_002e:  ret 
    } // end of method EnumerableEx::Publish

    // method line 103
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Replay<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x645f
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Memoize<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_0028:  ret 
    } // end of method EnumerableEx::Replay

    // method line 104
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Replay<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function, int32 bufferSize)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6489
	// Code size 42 (0x2a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  ldarg.2 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Memoize<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32)
	IL_0024:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_0029:  ret 
    } // end of method EnumerableEx::Replay

    // method line 105
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult> Prune<TSource,TResult> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>> function)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x64b4
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "function"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.1 
	IL_001d:  ldarg.0 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Share<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0023:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource>, class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TResult>>::Invoke(!0)
	IL_0028:  ret 
    } // end of method EnumerableEx::Prune

    // method line 106
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Share<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x64de
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.SharedEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Share

    // method line 107
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Memoize<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, int32 bufferSize)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x64f4
	// Code size 22 (0x16)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  newobj instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, int32)
	IL_0015:  ret 
    } // end of method EnumerableEx::Memoize

    // method line 108
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TAccumulate> AggregateEnumerable<TSource,TAccumulate> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TAccumulate seed, class [System.Core]System.Func`3<!!TAccumulate, !!TSource, !!TAccumulate> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x650b
	// Code size 42 (0x2a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "accumulator"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldarg.2 
	IL_001f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan0<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<!!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0029:  ret 
    } // end of method EnumerableEx::AggregateEnumerable

    // method line 109
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> AggregateEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`3<!!TSource, !!TSource, !!TSource> accumulator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6536
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "accumulator"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Scan<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`3<!!0,!!0,!!0>)
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0028:  ret 
    } // end of method EnumerableEx::AggregateEnumerable

    // method line 110
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> IsEmptyEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6560
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::IsEmptyEnumerableHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::IsEmptyEnumerable

    // method line 111
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> IsEmptyEnumerableHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        // Method begins at RVA 0x6724
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::IsEmptyEnumerableHelper

    // method line 112
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> AnyEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6747
	// Code size 38 (0x26)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::IsEmptyEnumerable<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0014:  ldnull 
	IL_0015:  ldftn bool class System.Linq.EnumerableEx::'<AnyEnumerable>b__dc'<!!0> (bool)
	IL_001b:  newobj instance void class [System.Core]System.Func`2<bool, bool>::'.ctor'(object, native int)
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<bool, bool> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0025:  ret 
    } // end of method EnumerableEx::AnyEnumerable

    // method line 113
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> AnyEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, bool> predicate)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x676e
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "predicate"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Where<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,bool>)
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::AnyEnumerable<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0028:  ret 
    } // end of method EnumerableEx::AnyEnumerable

    // method line 114
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> AllEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, bool> predicate)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x67b4
	// Code size 70 (0x46)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!!TSource>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!!TSource>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!!0>::predicate
	IL_000d:  ldarg.0 
	IL_000e:  brtrue.s IL_001b

	IL_0010:  ldstr "source"
	IL_0015:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001a:  throw 
	IL_001b:  ldloc.0 
	IL_001c:  ldfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!!0>::predicate
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldstr "predicate"
	IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_002d:  throw 
	IL_002e:  ldarg.0 
	IL_002f:  ldloc.0 
	IL_0030:  ldftn instance bool class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!!TSource>::'<AllEnumerable>b__dd'(!0)
	IL_0036:  newobj instance void class [System.Core]System.Func`2<!!TSource, bool>::'.ctor'(object, native int)
	IL_003b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Where<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,bool>)
	IL_0040:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::IsEmptyEnumerable<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0045:  ret 
    } // end of method EnumerableEx::AllEnumerable

    // method line 115
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> ContainsEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TSource 'value', class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6824
	// Code size 77 (0x4d)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!TSource>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!TSource>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!0>::'value'
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.2 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!0>::comparer
	IL_0014:  ldarg.0 
	IL_0015:  brtrue.s IL_0022

	IL_0017:  ldstr "source"
	IL_001c:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0021:  throw 
	IL_0022:  ldloc.0 
	IL_0023:  ldfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!0>::comparer
	IL_0028:  brtrue.s IL_0035

	IL_002a:  ldstr "comparer"
	IL_002f:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0034:  throw 
	IL_0035:  ldarg.0 
	IL_0036:  ldloc.0 
	IL_0037:  ldftn instance bool class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!!TSource>::'<ContainsEnumerable>b__e0'(!0)
	IL_003d:  newobj instance void class [System.Core]System.Func`2<!!TSource, bool>::'.ctor'(object, native int)
	IL_0042:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class [System.Core]System.Linq.Enumerable::Where<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,bool>)
	IL_0047:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::AnyEnumerable<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_004c:  ret 
    } // end of method EnumerableEx::ContainsEnumerable

    // method line 116
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> ContainsEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, !!TSource 'value')  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x687d
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!!TSource>::get_Default()
	IL_0015:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<bool> class System.Linq.EnumerableEx::ContainsEnumerable<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!0, class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!!0>)
	IL_001a:  ret 
    } // end of method EnumerableEx::ContainsEnumerable

    // method line 117
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> CountEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x689e
	// Code size 34 (0x22)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  ldnull 
	IL_0011:  ldftn int32 class System.Linq.EnumerableEx::'<CountEnumerable>b__e3'<!!0> (int32, !!0)
	IL_0017:  newobj instance void class [System.Core]System.Func`3<int32, !!TSource, int32>::'.ctor'(object, native int)
	IL_001c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<!!0, int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0021:  ret 
    } // end of method EnumerableEx::CountEnumerable

    // method line 118
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> LongCountEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x68c7
	// Code size 40 (0x28)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  ldnull 
	IL_0012:  ldftn int64 class System.Linq.EnumerableEx::'<LongCountEnumerable>b__e4'<!!0> (int64, !!0)
	IL_0018:  newobj instance void class [System.Core]System.Func`3<int64, !!TSource, int64>::'.ctor'(object, native int)
	IL_001d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<!!0, int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0022:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0027:  ret 
    } // end of method EnumerableEx::LongCountEnumerable

    // method line 119
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x68f5
	// Code size 59 (0x3b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 0.
	IL_0018:  ldsfld class [System.Core]System.Func`3<float64,float64,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee6'
	IL_001d:  brtrue.s IL_0030

	IL_001f:  ldnull 
	IL_0020:  ldftn float64 class System.Linq.EnumerableEx::'<SumEnumerable>b__e5'(float64, float64)
	IL_0026:  newobj instance void class [System.Core]System.Func`3<float64, float64, float64>::'.ctor'(object, native int)
	IL_002b:  stsfld class [System.Core]System.Func`3<float64,float64,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee6'
	IL_0030:  ldsfld class [System.Core]System.Func`3<float64,float64,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee6'
	IL_0035:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<float64, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003a:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 120
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6936
	// Code size 55 (0x37)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 0.
	IL_0014:  ldsfld class [System.Core]System.Func`3<float32,float32,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee8'
	IL_0019:  brtrue.s IL_002c

	IL_001b:  ldnull 
	IL_001c:  ldftn float32 class System.Linq.EnumerableEx::'<SumEnumerable>b__e7'(float32, float32)
	IL_0022:  newobj instance void class [System.Core]System.Func`3<float32, float32, float32>::'.ctor'(object, native int)
	IL_0027:  stsfld class [System.Core]System.Func`3<float32,float32,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee8'
	IL_002c:  ldsfld class [System.Core]System.Func`3<float32,float32,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatee8'
	IL_0031:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<float32, float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0036:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 121
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6977
	// Code size 56 (0x38)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32)
	IL_0015:  ldsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateea'
	IL_001a:  brtrue.s IL_002d

	IL_001c:  ldnull 
	IL_001d:  ldftn valuetype [mscorlib]System.Decimal class System.Linq.EnumerableEx::'<SumEnumerable>b__e9'(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_0023:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal>::'.ctor'(object, native int)
	IL_0028:  stsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateea'
	IL_002d:  ldsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateea'
	IL_0032:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0037:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 122
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x69b5
	// Code size 51 (0x33)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  ldsfld class [System.Core]System.Func`3<int32,int32,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateec'
	IL_0015:  brtrue.s IL_0028

	IL_0017:  ldnull 
	IL_0018:  ldftn int32 class System.Linq.EnumerableEx::'<SumEnumerable>b__eb'(int32, int32)
	IL_001e:  newobj instance void class [System.Core]System.Func`3<int32, int32, int32>::'.ctor'(object, native int)
	IL_0023:  stsfld class [System.Core]System.Func`3<int32,int32,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateec'
	IL_0028:  ldsfld class [System.Core]System.Func`3<int32,int32,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateec'
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<int32, int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0032:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 123
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x69ee
	// Code size 52 (0x34)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  ldsfld class [System.Core]System.Func`3<int64,int64,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateee'
	IL_0016:  brtrue.s IL_0029

	IL_0018:  ldnull 
	IL_0019:  ldftn int64 class System.Linq.EnumerableEx::'<SumEnumerable>b__ed'(int64, int64)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<int64, int64, int64>::'.ctor'(object, native int)
	IL_0024:  stsfld class [System.Core]System.Func`3<int64,int64,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateee'
	IL_0029:  ldsfld class [System.Core]System.Func`3<int64,int64,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegateee'
	IL_002e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<int64, int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0033:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 124
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6a38
	// Code size 93 (0x5d)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 0.
	IL_0018:  ldsfld class [System.Core]System.Func`3<float64,valuetype [mscorlib]System.Nullable`1<float64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef1'
	IL_001d:  brtrue.s IL_0030

	IL_001f:  ldnull 
	IL_0020:  ldftn float64 class System.Linq.EnumerableEx::'<SumEnumerable>b__ef'(float64, valuetype [mscorlib]System.Nullable`1<float64>)
	IL_0026:  newobj instance void class [System.Core]System.Func`3<float64, valuetype [mscorlib]System.Nullable`1<float64>, float64>::'.ctor'(object, native int)
	IL_002b:  stsfld class [System.Core]System.Func`3<float64,valuetype [mscorlib]System.Nullable`1<float64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef1'
	IL_0030:  ldsfld class [System.Core]System.Func`3<float64,valuetype [mscorlib]System.Nullable`1<float64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef1'
	IL_0035:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float64>, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003a:  ldsfld class [System.Core]System.Func`2<float64,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef2'
	IL_003f:  brtrue.s IL_0052

	IL_0041:  ldnull 
	IL_0042:  ldftn valuetype [mscorlib]System.Nullable`1<float64> class System.Linq.EnumerableEx::'<SumEnumerable>b__f0'(float64)
	IL_0048:  newobj instance void class [System.Core]System.Func`2<float64, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_004d:  stsfld class [System.Core]System.Func`2<float64,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef2'
	IL_0052:  ldsfld class [System.Core]System.Func`2<float64,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef2'
	IL_0057:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<float64, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_005c:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 125
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6ab4
	// Code size 89 (0x59)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 0.
	IL_0014:  ldsfld class [System.Core]System.Func`3<float32,valuetype [mscorlib]System.Nullable`1<float32>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef5'
	IL_0019:  brtrue.s IL_002c

	IL_001b:  ldnull 
	IL_001c:  ldftn float32 class System.Linq.EnumerableEx::'<SumEnumerable>b__f3'(float32, valuetype [mscorlib]System.Nullable`1<float32>)
	IL_0022:  newobj instance void class [System.Core]System.Func`3<float32, valuetype [mscorlib]System.Nullable`1<float32>, float32>::'.ctor'(object, native int)
	IL_0027:  stsfld class [System.Core]System.Func`3<float32,valuetype [mscorlib]System.Nullable`1<float32>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef5'
	IL_002c:  ldsfld class [System.Core]System.Func`3<float32,valuetype [mscorlib]System.Nullable`1<float32>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef5'
	IL_0031:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float32>, float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0036:  ldsfld class [System.Core]System.Func`2<float32,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef6'
	IL_003b:  brtrue.s IL_004e

	IL_003d:  ldnull 
	IL_003e:  ldftn valuetype [mscorlib]System.Nullable`1<float32> class System.Linq.EnumerableEx::'<SumEnumerable>b__f4'(float32)
	IL_0044:  newobj instance void class [System.Core]System.Func`2<float32, valuetype [mscorlib]System.Nullable`1<float32>>::'.ctor'(object, native int)
	IL_0049:  stsfld class [System.Core]System.Func`2<float32,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef6'
	IL_004e:  ldsfld class [System.Core]System.Func`2<float32,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef6'
	IL_0053:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<float32, valuetype [mscorlib]System.Nullable`1<float32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0058:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 126
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6b30
	// Code size 90 (0x5a)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32)
	IL_0015:  ldsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef9'
	IL_001a:  brtrue.s IL_002d

	IL_001c:  ldnull 
	IL_001d:  ldftn valuetype [mscorlib]System.Decimal class System.Linq.EnumerableEx::'<SumEnumerable>b__f7'(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
	IL_0023:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Decimal>::'.ctor'(object, native int)
	IL_0028:  stsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef9'
	IL_002d:  ldsfld class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatef9'
	IL_0032:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0037:  ldsfld class [System.Core]System.Func`2<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefa'
	IL_003c:  brtrue.s IL_004f

	IL_003e:  ldnull 
	IL_003f:  ldftn valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> class System.Linq.EnumerableEx::'<SumEnumerable>b__f8'(valuetype [mscorlib]System.Decimal)
	IL_0045:  newobj instance void class [System.Core]System.Func`2<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>>::'.ctor'(object, native int)
	IL_004a:  stsfld class [System.Core]System.Func`2<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefa'
	IL_004f:  ldsfld class [System.Core]System.Func`2<valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefa'
	IL_0054:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0059:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 127
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6bac
	// Code size 85 (0x55)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  ldsfld class [System.Core]System.Func`3<int32,valuetype [mscorlib]System.Nullable`1<int32>,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefd'
	IL_0015:  brtrue.s IL_0028

	IL_0017:  ldnull 
	IL_0018:  ldftn int32 class System.Linq.EnumerableEx::'<SumEnumerable>b__fb'(int32, valuetype [mscorlib]System.Nullable`1<int32>)
	IL_001e:  newobj instance void class [System.Core]System.Func`3<int32, valuetype [mscorlib]System.Nullable`1<int32>, int32>::'.ctor'(object, native int)
	IL_0023:  stsfld class [System.Core]System.Func`3<int32,valuetype [mscorlib]System.Nullable`1<int32>,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefd'
	IL_0028:  ldsfld class [System.Core]System.Func`3<int32,valuetype [mscorlib]System.Nullable`1<int32>,int32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefd'
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int32>, int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0032:  ldsfld class [System.Core]System.Func`2<int32,valuetype [mscorlib]System.Nullable`1<int32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefe'
	IL_0037:  brtrue.s IL_004a

	IL_0039:  ldnull 
	IL_003a:  ldftn valuetype [mscorlib]System.Nullable`1<int32> class System.Linq.EnumerableEx::'<SumEnumerable>b__fc'(int32)
	IL_0040:  newobj instance void class [System.Core]System.Func`2<int32, valuetype [mscorlib]System.Nullable`1<int32>>::'.ctor'(object, native int)
	IL_0045:  stsfld class [System.Core]System.Func`2<int32,valuetype [mscorlib]System.Nullable`1<int32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefe'
	IL_004a:  ldsfld class [System.Core]System.Func`2<int32,valuetype [mscorlib]System.Nullable`1<int32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegatefe'
	IL_004f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<int32, valuetype [mscorlib]System.Nullable`1<int32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0054:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 128
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> SumEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6c20
	// Code size 86 (0x56)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  ldsfld class [System.Core]System.Func`3<int64,valuetype [mscorlib]System.Nullable`1<int64>,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate101'
	IL_0016:  brtrue.s IL_0029

	IL_0018:  ldnull 
	IL_0019:  ldftn int64 class System.Linq.EnumerableEx::'<SumEnumerable>b__ff'(int64, valuetype [mscorlib]System.Nullable`1<int64>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<int64, valuetype [mscorlib]System.Nullable`1<int64>, int64>::'.ctor'(object, native int)
	IL_0024:  stsfld class [System.Core]System.Func`3<int64,valuetype [mscorlib]System.Nullable`1<int64>,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate101'
	IL_0029:  ldsfld class [System.Core]System.Func`3<int64,valuetype [mscorlib]System.Nullable`1<int64>,int64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate101'
	IL_002e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int64>, int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0033:  ldsfld class [System.Core]System.Func`2<int64,valuetype [mscorlib]System.Nullable`1<int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate102'
	IL_0038:  brtrue.s IL_004b

	IL_003a:  ldnull 
	IL_003b:  ldftn valuetype [mscorlib]System.Nullable`1<int64> class System.Linq.EnumerableEx::'<SumEnumerable>b__100'(int64)
	IL_0041:  newobj instance void class [System.Core]System.Func`2<int64, valuetype [mscorlib]System.Nullable`1<int64>>::'.ctor'(object, native int)
	IL_0046:  stsfld class [System.Core]System.Func`2<int64,valuetype [mscorlib]System.Nullable`1<int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate102'
	IL_004b:  ldsfld class [System.Core]System.Func`2<int64,valuetype [mscorlib]System.Nullable`1<int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate102'
	IL_0050:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<int64, valuetype [mscorlib]System.Nullable`1<int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0055:  ret 
    } // end of method EnumerableEx::SumEnumerable

    // method line 129
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> MinByEnumerable<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6c82
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.Comparer`1<!0> class [mscorlib]System.Collections.Generic.Comparer`1<!!TKey>::get_Default()
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MinByEnumerable<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::MinByEnumerable

    // method line 130
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> MinByEnumerable<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6cc8
	// Code size 85 (0x55)
	.maxstack 5
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!!TSource, !!TKey>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!!TSource, !!TKey>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.2 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!!0,!!1>::comparer
	IL_000d:  ldarg.0 
	IL_000e:  brtrue.s IL_001b

	IL_0010:  ldstr "source"
	IL_0015:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001a:  throw 
	IL_001b:  ldarg.1 
	IL_001c:  brtrue.s IL_0029

	IL_001e:  ldstr "keySelector"
	IL_0023:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0028:  throw 
	IL_0029:  ldloc.0 
	IL_002a:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!!0,!!1>::comparer
	IL_002f:  brtrue.s IL_003c

	IL_0031:  ldstr "comparer"
	IL_0036:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_003b:  throw 
	IL_003c:  ldarg.0 
	IL_003d:  ldarg.1 
	IL_003e:  ldloc.0 
	IL_003f:  ldftn instance int32 class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!!TSource, !!TKey>::'<MinByEnumerable>b__103'(!1, !1)
	IL_0045:  newobj instance void class [System.Core]System.Func`3<!!TKey, !!TKey, int32>::'.ctor'(object, native int)
	IL_004a:  newobj instance void class [System.CoreEx]System.Collections.Generic.AnonymousComparer`1<!!TKey>::'.ctor'(class [System.Core]System.Func`3<!0,!0,int32>)
	IL_004f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::ExtremaByEnumerable<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0054:  ret 
    } // end of method EnumerableEx::MinByEnumerable

    // method line 131
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> MinEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d34
	// Code size 50 (0x32)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldnull 
	IL_0010:  ldftn !!0 class System.Linq.EnumerableEx::'<MinEnumerable>b__106'<!!0> (!!0)
	IL_0016:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_001b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MinByEnumerable<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0020:  ldnull 
	IL_0021:  ldftn !!0 class System.Linq.EnumerableEx::'<MinEnumerable>b__107'<!!0> (class [mscorlib]System.Collections.Generic.IList`1<!!0>)
	IL_0027:  newobj instance void class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>, !!TSource>::'.ctor'(object, native int)
	IL_002c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class [mscorlib]System.Collections.Generic.IList`1<!!0>, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0031:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 132
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> MinEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d74
	// Code size 65 (0x41)
	.maxstack 4
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "comparer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn !!0 class System.Linq.EnumerableEx::'<MinEnumerable>b__108'<!!0> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MinByEnumerable<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_002f:  ldnull 
	IL_0030:  ldftn !!0 class System.Linq.EnumerableEx::'<MinEnumerable>b__109'<!!0> (class [mscorlib]System.Collections.Generic.IList`1<!!0>)
	IL_0036:  newobj instance void class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>, !!TSource>::'.ctor'(object, native int)
	IL_003b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class [mscorlib]System.Collections.Generic.IList`1<!!0>, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0040:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 133
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6dc1
	// Code size 47 (0x2f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 1.7976931348623157e+308
	IL_0018:  ldnull 
	IL_0019:  ldftn float64 class [mscorlib]System.Math::Min(float64, float64)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<float64, float64, float64>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float64, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002e:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 134
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6df1
	// Code size 43 (0x2b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 3.4028234663852886e+38
	IL_0014:  ldnull 
	IL_0015:  ldftn float32 class [mscorlib]System.Math::Min(float32, float32)
	IL_001b:  newobj instance void class [System.Core]System.Func`3<float32, float32, float32>::'.ctor'(object, native int)
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float32, float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0025:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002a:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 135
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6e1d
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.m1 
	IL_0010:  ldc.i4.m1 
	IL_0011:  ldc.i4.m1 
	IL_0012:  ldc.i4.0 
	IL_0013:  ldc.i4.0 
	IL_0014:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32, int32, int32, bool, unsigned int8)
	IL_0019:  ldnull 
	IL_001a:  ldftn valuetype [mscorlib]System.Decimal class [mscorlib]System.Math::Min(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_0020:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal>::'.ctor'(object, native int)
	IL_0025:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_002a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002f:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 136
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6e4e
	// Code size 43 (0x2b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4 2147483647
	IL_0014:  ldnull 
	IL_0015:  ldftn int32 class [mscorlib]System.Math::Min(int32, int32)
	IL_001b:  newobj instance void class [System.Core]System.Func`3<int32, int32, int32>::'.ctor'(object, native int)
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int32, int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0025:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002a:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 137
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6e7a
	// Code size 47 (0x2f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i8 0x7fffffffffffffff
	IL_0018:  ldnull 
	IL_0019:  ldftn int64 class [mscorlib]System.Math::Min(int64, int64)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<int64, int64, int64>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int64, int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002e:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 138
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6eac
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float64>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMinEnumerable<float64> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 139
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6ee4
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float32>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<float32>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMinEnumerable<float32> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 140
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6f1c
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMinEnumerable<valuetype [mscorlib]System.Decimal> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 141
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6f54
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int32>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<int32>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMinEnumerable<int32> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 142
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> MinEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6f8c
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<int64>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMinEnumerable<int64> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MinEnumerable

    // method line 143
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> MaxByEnumerable<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6fc2
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  call class [mscorlib]System.Collections.Generic.Comparer`1<!0> class [mscorlib]System.Collections.Generic.Comparer`1<!!TKey>::get_Default()
	IL_0023:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MaxByEnumerable<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0028:  ret 
    } // end of method EnumerableEx::MaxByEnumerable

    // method line 144
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> MaxByEnumerable<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6fec
	// Code size 51 (0x33)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "comparer"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::ExtremaByEnumerable<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0032:  ret 
    } // end of method EnumerableEx::MaxByEnumerable

    // method line 145
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> MaxEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x702b
	// Code size 50 (0x32)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldnull 
	IL_0010:  ldftn !!0 class System.Linq.EnumerableEx::'<MaxEnumerable>b__10a'<!!0> (!!0)
	IL_0016:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_001b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MaxByEnumerable<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0020:  ldnull 
	IL_0021:  ldftn !!0 class System.Linq.EnumerableEx::'<MaxEnumerable>b__10b'<!!0> (class [mscorlib]System.Collections.Generic.IList`1<!!0>)
	IL_0027:  newobj instance void class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>, !!TSource>::'.ctor'(object, native int)
	IL_002c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class [mscorlib]System.Collections.Generic.IList`1<!!0>, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0031:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 146
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> MaxEnumerable<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [mscorlib]System.Collections.Generic.IComparer`1<!!TSource> comparer)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x706c
	// Code size 65 (0x41)
	.maxstack 4
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "comparer"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldnull 
	IL_001e:  ldftn !!0 class System.Linq.EnumerableEx::'<MaxEnumerable>b__10c'<!!0> (!!0)
	IL_0024:  newobj instance void class [System.Core]System.Func`2<!!TSource, !!TSource>::'.ctor'(object, native int)
	IL_0029:  ldarg.1 
	IL_002a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::MaxByEnumerable<!!0, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_002f:  ldnull 
	IL_0030:  ldftn !!0 class System.Linq.EnumerableEx::'<MaxEnumerable>b__10d'<!!0> (class [mscorlib]System.Collections.Generic.IList`1<!!0>)
	IL_0036:  newobj instance void class [System.Core]System.Func`2<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>, !!TSource>::'.ctor'(object, native int)
	IL_003b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class [mscorlib]System.Collections.Generic.IList`1<!!0>, !!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0040:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 147
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x70b9
	// Code size 47 (0x2f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 -1.7976931348623157e+308
	IL_0018:  ldnull 
	IL_0019:  ldftn float64 class [mscorlib]System.Math::Max(float64, float64)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<float64, float64, float64>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float64, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002e:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 148
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x70e9
	// Code size 43 (0x2b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 -3.4028234663852886e+38
	IL_0014:  ldnull 
	IL_0015:  ldftn float32 class [mscorlib]System.Math::Max(float32, float32)
	IL_001b:  newobj instance void class [System.Core]System.Func`3<float32, float32, float32>::'.ctor'(object, native int)
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float32, float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0025:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002a:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 149
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7115
	// Code size 52 (0x34)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.m1 
	IL_0010:  ldc.i4.m1 
	IL_0011:  ldc.i4.m1 
	IL_0012:  ldc.i4 128
	IL_0017:  ldc.i4.0 
	IL_0018:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32, int32, int32, bool, unsigned int8)
	IL_001d:  ldnull 
	IL_001e:  ldftn valuetype [mscorlib]System.Decimal class [mscorlib]System.Math::Max(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_0024:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal>::'.ctor'(object, native int)
	IL_0029:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_002e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0033:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 150
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x714a
	// Code size 43 (0x2b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4 -2147483648
	IL_0014:  ldnull 
	IL_0015:  ldftn int32 class [mscorlib]System.Math::Max(int32, int32)
	IL_001b:  newobj instance void class [System.Core]System.Func`3<int32, int32, int32>::'.ctor'(object, native int)
	IL_0020:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int32, int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0025:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<int32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002a:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 151
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7176
	// Code size 47 (0x2f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i8 0x8000000000000000
	IL_0018:  ldnull 
	IL_0019:  ldftn int64 class [mscorlib]System.Math::Max(int64, int64)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<int64, int64, int64>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int64, int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<int64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_002e:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 152
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x71a8
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float64>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMaxEnumerable<float64> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float64>, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 153
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x71e0
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float32>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<float32>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMaxEnumerable<float32> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float32>, valuetype [mscorlib]System.Nullable`1<float32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 154
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7218
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMaxEnumerable<valuetype [mscorlib]System.Decimal> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 155
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7250
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int32>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<int32>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMaxEnumerable<int32> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int32>, valuetype [mscorlib]System.Nullable`1<int32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 156
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> MaxEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7288
	// Code size 42 (0x2a)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldloca.s 0
	IL_0011:  initobj valuetype [mscorlib]System.Nullable`1<int64>
	IL_0017:  ldloc.0 
	IL_0018:  ldnull 
	IL_0019:  ldftn valuetype [mscorlib]System.Nullable`1<!!0> class System.Linq.EnumerableEx::NullableMaxEnumerable<int64> (valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_001f:  newobj instance void class [System.Core]System.Func`3<valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>>::'.ctor'(object, native int)
	IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int64>, valuetype [mscorlib]System.Nullable`1<int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0029:  ret 
    } // end of method EnumerableEx::MaxEnumerable

    // method line 157
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> ExtremaByEnumerable<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        // Method begins at RVA 0x72be
	// Code size 51 (0x33)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "keySelector"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.2 
	IL_001d:  brtrue.s IL_002a

	IL_001f:  ldstr "comparer"
	IL_0024:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0029:  throw 
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.1 
	IL_002c:  ldarg.2 
	IL_002d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!0>> class System.Linq.EnumerableEx::ExtremaByEnumerableHelper<!!0, !!1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>, class [mscorlib]System.Collections.Generic.IComparer`1<!!1>)
	IL_0032:  ret 
    } // end of method EnumerableEx::ExtremaByEnumerable

    // method line 158
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!!TSource>> ExtremaByEnumerableHelper<TSource,TKey> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source, class [System.Core]System.Func`2<!!TSource, !!TKey> keySelector, class [mscorlib]System.Collections.Generic.IComparer`1<!!TKey> comparer)  cil managed 
    {
        // Method begins at RVA 0x74ac
	// Code size 31 (0x1f)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!!TSource, !!TKey>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!!TSource, !!TKey>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!!0,!!1>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ldarg.1 
	IL_0011:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!!0,!!1>::'<>3__keySelector'
	IL_0016:  ldloc.0 
	IL_0017:  ldarg.2 
	IL_0018:  stfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!!0,!!1>::'<>3__comparer'
	IL_001d:  ldloc.0 
	IL_001e:  ret 
    } // end of method EnumerableEx::ExtremaByEnumerableHelper

    // method line 159
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x761c
	// Code size 105 (0x69)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 0.
	IL_0018:  ldc.i4.0 
	IL_0019:  conv.i8 
	IL_001a:  newobj instance void class '<>f__AnonymousType0`2'<float64, int64>::'.ctor'(!0, !1)
	IL_001f:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float64,int64>,float64,class '<>f__AnonymousType0`2'<float64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate116'
	IL_0024:  brtrue.s IL_0037

	IL_0026:  ldnull 
	IL_0027:  ldftn class '<>f__AnonymousType0`2'<float64,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__114'(class '<>f__AnonymousType0`2'<float64,int64>, float64)
	IL_002d:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float64, int64>, float64, class '<>f__AnonymousType0`2'<float64, int64>>::'.ctor'(object, native int)
	IL_0032:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float64,int64>,float64,class '<>f__AnonymousType0`2'<float64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate116'
	IL_0037:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float64,int64>,float64,class '<>f__AnonymousType0`2'<float64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate116'
	IL_003c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float64, class '<>f__AnonymousType0`2'<float64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0041:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<class '<>f__AnonymousType0`2'<float64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0046:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate117'
	IL_004b:  brtrue.s IL_005e

	IL_004d:  ldnull 
	IL_004e:  ldftn float64 class System.Linq.EnumerableEx::'<AverageEnumerable>b__115'(class '<>f__AnonymousType0`2'<float64,int64>)
	IL_0054:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float64, int64>, float64>::'.ctor'(object, native int)
	IL_0059:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate117'
	IL_005e:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate117'
	IL_0063:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<float64,int64>, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0068:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 160
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<float32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x76bc
	// Code size 101 (0x65)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 0.
	IL_0014:  ldc.i4.0 
	IL_0015:  conv.i8 
	IL_0016:  newobj instance void class '<>f__AnonymousType0`2'<float32, int64>::'.ctor'(!0, !1)
	IL_001b:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float32,int64>,float32,class '<>f__AnonymousType0`2'<float32,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11a'
	IL_0020:  brtrue.s IL_0033

	IL_0022:  ldnull 
	IL_0023:  ldftn class '<>f__AnonymousType0`2'<float32,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__118'(class '<>f__AnonymousType0`2'<float32,int64>, float32)
	IL_0029:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float32, int64>, float32, class '<>f__AnonymousType0`2'<float32, int64>>::'.ctor'(object, native int)
	IL_002e:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float32,int64>,float32,class '<>f__AnonymousType0`2'<float32,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11a'
	IL_0033:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<float32,int64>,float32,class '<>f__AnonymousType0`2'<float32,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11a'
	IL_0038:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<float32, class '<>f__AnonymousType0`2'<float32,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<class '<>f__AnonymousType0`2'<float32,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0042:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float32,int64>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11b'
	IL_0047:  brtrue.s IL_005a

	IL_0049:  ldnull 
	IL_004a:  ldftn float32 class System.Linq.EnumerableEx::'<AverageEnumerable>b__119'(class '<>f__AnonymousType0`2'<float32,int64>)
	IL_0050:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float32, int64>, float32>::'.ctor'(object, native int)
	IL_0055:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float32,int64>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11b'
	IL_005a:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<float32,int64>,float32> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11b'
	IL_005f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<float32,int64>, float32> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0064:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 161
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Decimal> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7764
	// Code size 102 (0x66)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32)
	IL_0015:  ldc.i4.0 
	IL_0016:  conv.i8 
	IL_0017:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::'.ctor'(!0, !1)
	IL_001c:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11e'
	IL_0021:  brtrue.s IL_0034

	IL_0023:  ldnull 
	IL_0024:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__11c'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>, valuetype [mscorlib]System.Decimal)
	IL_002a:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>, valuetype [mscorlib]System.Decimal, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>>::'.ctor'(object, native int)
	IL_002f:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11e'
	IL_0034:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11e'
	IL_0039:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<valuetype [mscorlib]System.Decimal, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0043:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11f'
	IL_0048:  brtrue.s IL_005b

	IL_004a:  ldnull 
	IL_004b:  ldftn valuetype [mscorlib]System.Decimal class System.Linq.EnumerableEx::'<AverageEnumerable>b__11d'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>)
	IL_0051:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>, valuetype [mscorlib]System.Decimal>::'.ctor'(object, native int)
	IL_0056:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11f'
	IL_005b:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>,valuetype [mscorlib]System.Decimal> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate11f'
	IL_0060:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal,int64>, valuetype [mscorlib]System.Decimal> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0065:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 162
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7800
	// Code size 98 (0x62)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  ldc.i4.0 
	IL_0012:  conv.i8 
	IL_0013:  newobj instance void class '<>f__AnonymousType0`2'<int64, int64>::'.ctor'(!0, !1)
	IL_0018:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int32,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate122'
	IL_001d:  brtrue.s IL_0030

	IL_001f:  ldnull 
	IL_0020:  ldftn class '<>f__AnonymousType0`2'<int64,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__120'(class '<>f__AnonymousType0`2'<int64,int64>, int32)
	IL_0026:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64, int64>, int32, class '<>f__AnonymousType0`2'<int64, int64>>::'.ctor'(object, native int)
	IL_002b:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int32,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate122'
	IL_0030:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int32,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate122'
	IL_0035:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int32, class '<>f__AnonymousType0`2'<int64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<class '<>f__AnonymousType0`2'<int64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_003f:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate123'
	IL_0044:  brtrue.s IL_0057

	IL_0046:  ldnull 
	IL_0047:  ldftn float64 class System.Linq.EnumerableEx::'<AverageEnumerable>b__121'(class '<>f__AnonymousType0`2'<int64,int64>)
	IL_004d:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64, int64>, float64>::'.ctor'(object, native int)
	IL_0052:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate123'
	IL_0057:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate123'
	IL_005c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<int64,int64>, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0061:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 163
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<float64> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<int64> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7898
	// Code size 98 (0x62)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  ldc.i4.0 
	IL_0012:  conv.i8 
	IL_0013:  newobj instance void class '<>f__AnonymousType0`2'<int64, int64>::'.ctor'(!0, !1)
	IL_0018:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int64,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate126'
	IL_001d:  brtrue.s IL_0030

	IL_001f:  ldnull 
	IL_0020:  ldftn class '<>f__AnonymousType0`2'<int64,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__124'(class '<>f__AnonymousType0`2'<int64,int64>, int64)
	IL_0026:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64, int64>, int64, class '<>f__AnonymousType0`2'<int64, int64>>::'.ctor'(object, native int)
	IL_002b:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int64,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate126'
	IL_0030:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<int64,int64>,int64,class '<>f__AnonymousType0`2'<int64,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate126'
	IL_0035:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::Scan<int64, class '<>f__AnonymousType0`2'<int64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Final<class '<>f__AnonymousType0`2'<int64,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_003f:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate127'
	IL_0044:  brtrue.s IL_0057

	IL_0046:  ldnull 
	IL_0047:  ldftn float64 class System.Linq.EnumerableEx::'<AverageEnumerable>b__125'(class '<>f__AnonymousType0`2'<int64,int64>)
	IL_004d:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64, int64>, float64>::'.ctor'(object, native int)
	IL_0052:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate127'
	IL_0057:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<int64,int64>,float64> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate127'
	IL_005c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<int64,int64>, float64> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0061:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 164
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x79b4
	// Code size 105 (0x69)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r8 0.
	IL_0018:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_001d:  ldc.i4.0 
	IL_001e:  conv.i8 
	IL_001f:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::'.ctor'(!0, !1)
	IL_0024:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12a'
	IL_0029:  brtrue.s IL_003c

	IL_002b:  ldnull 
	IL_002c:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__128'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>, valuetype [mscorlib]System.Nullable`1<float64>)
	IL_0032:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>, valuetype [mscorlib]System.Nullable`1<float64>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>>::'.ctor'(object, native int)
	IL_0037:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12a'
	IL_003c:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12a'
	IL_0041:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float64>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0046:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12b'
	IL_004b:  brtrue.s IL_005e

	IL_004d:  ldnull 
	IL_004e:  ldftn valuetype [mscorlib]System.Nullable`1<float64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__129'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>)
	IL_0054:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_0059:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12b'
	IL_005e:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12b'
	IL_0063:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>,int64>, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0068:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 165
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7ad8
	// Code size 101 (0x65)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.r4 0.
	IL_0014:  newobj instance void valuetype [mscorlib]System.Nullable`1<float32>::'.ctor'(!0)
	IL_0019:  ldc.i4.0 
	IL_001a:  conv.i8 
	IL_001b:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::'.ctor'(!0, !1)
	IL_0020:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12e'
	IL_0025:  brtrue.s IL_0038

	IL_0027:  ldnull 
	IL_0028:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__12c'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>, valuetype [mscorlib]System.Nullable`1<float32>)
	IL_002e:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>, valuetype [mscorlib]System.Nullable`1<float32>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>>::'.ctor'(object, native int)
	IL_0033:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12e'
	IL_0038:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12e'
	IL_003d:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<float32>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0042:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12f'
	IL_0047:  brtrue.s IL_005a

	IL_0049:  ldnull 
	IL_004a:  ldftn valuetype [mscorlib]System.Nullable`1<float32> class System.Linq.EnumerableEx::'<AverageEnumerable>b__12d'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>)
	IL_0050:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>, valuetype [mscorlib]System.Nullable`1<float32>>::'.ctor'(object, native int)
	IL_0055:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12f'
	IL_005a:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>,valuetype [mscorlib]System.Nullable`1<float32>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate12f'
	IL_005f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>,int64>, valuetype [mscorlib]System.Nullable`1<float32>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0064:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 166
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7c04
	// Code size 102 (0x66)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  newobj instance void valuetype [mscorlib]System.Decimal::'.ctor'(int32)
	IL_0015:  newobj instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::'.ctor'(!0)
	IL_001a:  ldc.i4.0 
	IL_001b:  conv.i8 
	IL_001c:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::'.ctor'(!0, !1)
	IL_0021:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate132'
	IL_0026:  brtrue.s IL_0039

	IL_0028:  ldnull 
	IL_0029:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__130'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
	IL_002f:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>>::'.ctor'(object, native int)
	IL_0034:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate132'
	IL_0039:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate132'
	IL_003e:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_0043:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate133'
	IL_0048:  brtrue.s IL_005b

	IL_004a:  ldnull 
	IL_004b:  ldftn valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> class System.Linq.EnumerableEx::'<AverageEnumerable>b__131'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>)
	IL_0051:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>>::'.ctor'(object, native int)
	IL_0056:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate133'
	IL_005b:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>,valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate133'
	IL_0060:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>,int64>, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0065:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 167
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int32>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7d48
	// Code size 98 (0x62)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::'.ctor'(!0)
	IL_0016:  ldc.i4.0 
	IL_0017:  conv.i8 
	IL_0018:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::'.ctor'(!0, !1)
	IL_001d:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate136'
	IL_0022:  brtrue.s IL_0035

	IL_0024:  ldnull 
	IL_0025:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__134'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>, valuetype [mscorlib]System.Nullable`1<int32>)
	IL_002b:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>, valuetype [mscorlib]System.Nullable`1<int32>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>>::'.ctor'(object, native int)
	IL_0030:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate136'
	IL_0035:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int32>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate136'
	IL_003a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int32>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003f:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate137'
	IL_0044:  brtrue.s IL_0057

	IL_0046:  ldnull 
	IL_0047:  ldftn valuetype [mscorlib]System.Nullable`1<float64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__135'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>)
	IL_004d:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_0052:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate137'
	IL_0057:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate137'
	IL_005c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0061:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 168
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<float64>> AverageEnumerable (class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.Nullable`1<int64>> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7e88
	// Code size 98 (0x62)
	.maxstack 5
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldc.i4.0 
	IL_0010:  conv.i8 
	IL_0011:  newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::'.ctor'(!0)
	IL_0016:  ldc.i4.0 
	IL_0017:  conv.i8 
	IL_0018:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::'.ctor'(!0, !1)
	IL_001d:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13a'
	IL_0022:  brtrue.s IL_0035

	IL_0024:  ldnull 
	IL_0025:  ldftn class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__138'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>, valuetype [mscorlib]System.Nullable`1<int64>)
	IL_002b:  newobj instance void class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>, valuetype [mscorlib]System.Nullable`1<int64>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>>::'.ctor'(object, native int)
	IL_0030:  stsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13a'
	IL_0035:  ldsfld class [System.Core]System.Func`3<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<int64>,class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13a'
	IL_003a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class System.Linq.EnumerableEx::AggregateEnumerable<valuetype [mscorlib]System.Nullable`1<int64>, class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!1, class [System.Core]System.Func`3<!!1,!!0,!!1>)
	IL_003f:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13b'
	IL_0044:  brtrue.s IL_0057

	IL_0046:  ldnull 
	IL_0047:  ldftn valuetype [mscorlib]System.Nullable`1<float64> class System.Linq.EnumerableEx::'<AverageEnumerable>b__139'(class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>)
	IL_004d:  newobj instance void class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>, valuetype [mscorlib]System.Nullable`1<float64>>::'.ctor'(object, native int)
	IL_0052:  stsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13b'
	IL_0057:  ldsfld class [System.Core]System.Func`2<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>,valuetype [mscorlib]System.Nullable`1<float64>> System.Linq.EnumerableEx::'CS$<>9__CachedAnonymousMethodDelegate13b'
	IL_005c:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>,int64>, valuetype [mscorlib]System.Nullable`1<float64>> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0061:  ret 
    } // end of method EnumerableEx::AverageEnumerable

    // method line 169
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> Final<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7ef6
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "source"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::FinalHelper<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0014:  ret 
    } // end of method EnumerableEx::Final

    // method line 170
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> FinalHelper<TSource> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!TSource> source)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8064
	// Code size 17 (0x11)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!!TSource>	V_0)
	IL_0000:  ldc.i4.s 0xfffffffe
	IL_0002:  newobj instance void class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!!TSource>::'.ctor'(int32)
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldarg.0 
	IL_000a:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!!0>::'<>3__source'
	IL_000f:  ldloc.0 
	IL_0010:  ret 
    } // end of method EnumerableEx::FinalHelper

    // method line 171
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<!!T> NullableMinEnumerable<valuetype .ctor (class [mscorlib]System.ValueType, class [mscorlib]System.IComparable`1<!!T>) T> (valuetype [mscorlib]System.Nullable`1<!!T> x, valuetype [mscorlib]System.Nullable`1<!!T> y)  cil managed 
    {
        // Method begins at RVA 0x8084
	// Code size 57 (0x39)
	.maxstack 2
	.locals init (
		!!T	V_0)
	IL_0000:  ldarga.s 0
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.1 
	IL_000a:  ret 
	IL_000b:  ldarga.s 1
	IL_000d:  call instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
	IL_0012:  brtrue.s IL_0016

	IL_0014:  ldarg.0 
	IL_0015:  ret 
	IL_0016:  ldarga.s 0
	IL_0018:  call instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
	IL_001d:  stloc.0 
	IL_001e:  ldloca.s 0
	IL_0020:  ldarga.s 1
	IL_0022:  call instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
	IL_0027:  constrained. !!0
	IL_002d:  callvirt instance int32 class [mscorlib]System.IComparable`1<!!T>::CompareTo(!0)
	IL_0032:  ldc.i4.0 
	IL_0033:  bgt.s IL_0037

	IL_0035:  ldarg.0 
	IL_0036:  ret 
	IL_0037:  ldarg.1 
	IL_0038:  ret 
    } // end of method EnumerableEx::NullableMinEnumerable

    // method line 172
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<!!T> NullableMaxEnumerable<valuetype .ctor (class [mscorlib]System.ValueType, class [mscorlib]System.IComparable`1<!!T>) T> (valuetype [mscorlib]System.Nullable`1<!!T> x, valuetype [mscorlib]System.Nullable`1<!!T> y)  cil managed 
    {
        // Method begins at RVA 0x80cc
	// Code size 57 (0x39)
	.maxstack 2
	.locals init (
		!!T	V_0)
	IL_0000:  ldarga.s 0
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.1 
	IL_000a:  ret 
	IL_000b:  ldarga.s 1
	IL_000d:  call instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
	IL_0012:  brtrue.s IL_0016

	IL_0014:  ldarg.0 
	IL_0015:  ret 
	IL_0016:  ldarga.s 0
	IL_0018:  call instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
	IL_001d:  stloc.0 
	IL_001e:  ldloca.s 0
	IL_0020:  ldarga.s 1
	IL_0022:  call instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
	IL_0027:  constrained. !!0
	IL_002d:  callvirt instance int32 class [mscorlib]System.IComparable`1<!!T>::CompareTo(!0)
	IL_0032:  ldc.i4.0 
	IL_0033:  blt.s IL_0037

	IL_0035:  ldarg.0 
	IL_0036:  ret 
	IL_0037:  ldarg.1 
	IL_0038:  ret 
    } // end of method EnumerableEx::NullableMaxEnumerable

    // method line 173
    .method private static hidebysig 
           default void '<Do>b__0'<TSource> (class [mscorlib]System.Exception _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2148
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method EnumerableEx::<Do>b__0

    // method line 174
    .method private static hidebysig 
           default void '<Do>b__1'<TSource> ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x214a
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method EnumerableEx::<Do>b__1

    // method line 175
    .method private static hidebysig 
           default void '<Do>b__2'<TSource> (class [mscorlib]System.Exception _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2189
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method EnumerableEx::<Do>b__2

    // method line 176
    .method private static hidebysig 
           default void '<Do>b__3'<TSource> ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x21cb
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method EnumerableEx::<Do>b__3

    // method line 177
    .method private static hidebysig 
           default !!TSource '<DistinctUntilChanged>b__48'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3673
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<DistinctUntilChanged>b__48

    // method line 178
    .method private static hidebysig 
           default !!TSource '<DistinctUntilChanged>b__49'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x36d1
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<DistinctUntilChanged>b__49

    // method line 179
    .method private static hidebysig 
           default bool '<Generate>b__ac'<TState,TResult> (!!TState _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x554b
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.1 
	IL_0001:  ret 
    } // end of method EnumerableEx::<Generate>b__ac

    // method line 180
    .method private static hidebysig 
           default !!TSource '<Min>b__cf'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6014
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<Min>b__cf

    // method line 181
    .method private static hidebysig 
           default !!TSource '<Max>b__d3'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x60e8
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<Max>b__d3

    // method line 182
    .method private static hidebysig 
           default bool '<AnyEnumerable>b__dc'<TSource> (bool b)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6741
	// Code size 5 (0x5)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.0 
	IL_0002:  ceq 
	IL_0004:  ret 
    } // end of method EnumerableEx::<AnyEnumerable>b__dc

    // method line 183
    .method private static hidebysig 
           default int32 '<CountEnumerable>b__e3'<TSource> (int32 count, !!TSource _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6899
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  add.ovf 
	IL_0003:  ret 
    } // end of method EnumerableEx::<CountEnumerable>b__e3

    // method line 184
    .method private static hidebysig 
           default int64 '<LongCountEnumerable>b__e4'<TSource> (int64 count, !!TSource _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x68c1
	// Code size 5 (0x5)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  conv.i8 
	IL_0003:  add.ovf 
	IL_0004:  ret 
    } // end of method EnumerableEx::<LongCountEnumerable>b__e4

    // method line 185
    .method private static hidebysig 
           default float64 '<SumEnumerable>b__e5' (float64 prev, float64 curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x68f0
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  add 
	IL_0003:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__e5

    // method line 186
    .method private static hidebysig 
           default float32 '<SumEnumerable>b__e7' (float32 prev, float32 curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6931
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  add 
	IL_0003:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__e7

    // method line 187
    .method private static hidebysig 
           default valuetype [mscorlib]System.Decimal '<SumEnumerable>b__e9' (valuetype [mscorlib]System.Decimal prev, valuetype [mscorlib]System.Decimal curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x696e
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_0007:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__e9

    // method line 188
    .method private static hidebysig 
           default int32 '<SumEnumerable>b__eb' (int32 prev, int32 curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x69b0
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  add.ovf 
	IL_0003:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__eb

    // method line 189
    .method private static hidebysig 
           default int64 '<SumEnumerable>b__ed' (int64 prev, int64 curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x69e9
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  add.ovf 
	IL_0003:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__ed

    // method line 190
    .method private static hidebysig 
           default float64 '<SumEnumerable>b__ef' (float64 prev, valuetype [mscorlib]System.Nullable`1<float64> curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6a23
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarga.s 1
	IL_0003:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0008:  add 
	IL_0009:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__ef

    // method line 191
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float64> '<SumEnumerable>b__f0' (float64 x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6a2e
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_0006:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__f0

    // method line 192
    .method private static hidebysig 
           default float32 '<SumEnumerable>b__f3' (float32 prev, valuetype [mscorlib]System.Nullable`1<float32> curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6aa1
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarga.s 1
	IL_0003:  call instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
	IL_0008:  add 
	IL_0009:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__f3

    // method line 193
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float32> '<SumEnumerable>b__f4' (float32 x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6aac
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void valuetype [mscorlib]System.Nullable`1<float32>::'.ctor'(!0)
	IL_0006:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__f4

    // method line 194
    .method private static hidebysig 
           default valuetype [mscorlib]System.Decimal '<SumEnumerable>b__f7' (valuetype [mscorlib]System.Decimal prev, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6b19
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarga.s 1
	IL_0003:  call instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
	IL_0008:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_000d:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__f7

    // method line 195
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<SumEnumerable>b__f8' (valuetype [mscorlib]System.Decimal x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6b28
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::'.ctor'(!0)
	IL_0006:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__f8

    // method line 196
    .method private static hidebysig 
           default int32 '<SumEnumerable>b__fb' (int32 prev, valuetype [mscorlib]System.Nullable`1<int32> curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6b96
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarga.s 1
	IL_0003:  call instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
	IL_0008:  add.ovf 
	IL_0009:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__fb

    // method line 197
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<int32> '<SumEnumerable>b__fc' (int32 x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6ba1
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void valuetype [mscorlib]System.Nullable`1<int32>::'.ctor'(!0)
	IL_0006:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__fc

    // method line 198
    .method private static hidebysig 
           default int64 '<SumEnumerable>b__ff' (int64 prev, valuetype [mscorlib]System.Nullable`1<int64> curr)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6c0d
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarga.s 1
	IL_0003:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_0008:  add.ovf 
	IL_0009:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__ff

    // method line 199
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<int64> '<SumEnumerable>b__100' (int64 x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6c18
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::'.ctor'(!0)
	IL_0006:  ret 
    } // end of method EnumerableEx::<SumEnumerable>b__100

    // method line 200
    .method private static hidebysig 
           default !!TSource '<MinEnumerable>b__106'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d29
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<MinEnumerable>b__106

    // method line 201
    .method private static hidebysig 
           default !!TSource '<MinEnumerable>b__107'<TSource> (class [mscorlib]System.Collections.Generic.IList`1<!!TSource> x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d2c
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call !!0 class [System.Core]System.Linq.Enumerable::First<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0006:  ret 
    } // end of method EnumerableEx::<MinEnumerable>b__107

    // method line 202
    .method private static hidebysig 
           default !!TSource '<MinEnumerable>b__108'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d67
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<MinEnumerable>b__108

    // method line 203
    .method private static hidebysig 
           default !!TSource '<MinEnumerable>b__109'<TSource> (class [mscorlib]System.Collections.Generic.IList`1<!!TSource> x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6d6a
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call !!0 class [System.Core]System.Linq.Enumerable::First<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0006:  ret 
    } // end of method EnumerableEx::<MinEnumerable>b__109

    // method line 204
    .method private static hidebysig 
           default !!TSource '<MaxEnumerable>b__10a'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7020
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<MaxEnumerable>b__10a

    // method line 205
    .method private static hidebysig 
           default !!TSource '<MaxEnumerable>b__10b'<TSource> (class [mscorlib]System.Collections.Generic.IList`1<!!TSource> x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7023
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call !!0 class [System.Core]System.Linq.Enumerable::First<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0006:  ret 
    } // end of method EnumerableEx::<MaxEnumerable>b__10b

    // method line 206
    .method private static hidebysig 
           default !!TSource '<MaxEnumerable>b__10c'<TSource> (!!TSource x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x705e
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ret 
    } // end of method EnumerableEx::<MaxEnumerable>b__10c

    // method line 207
    .method private static hidebysig 
           default !!TSource '<MaxEnumerable>b__10d'<TSource> (class [mscorlib]System.Collections.Generic.IList`1<!!TSource> x)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7061
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call !!0 class [System.Core]System.Linq.Enumerable::First<!!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_0006:  ret 
    } // end of method EnumerableEx::<MaxEnumerable>b__10d

    // method line 208
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<float64, int64> '<AverageEnumerable>b__114' (class '<>f__AnonymousType0`2'<float64, int64> prev, float64 cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x75f2
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<float64, int64>::get_sum()
	IL_0006:  ldarg.1 
	IL_0007:  add 
	IL_0008:  ldarg.0 
	IL_0009:  callvirt instance !1 class '<>f__AnonymousType0`2'<float64, int64>::get_count()
	IL_000e:  ldc.i4.1 
	IL_000f:  conv.i8 
	IL_0010:  add.ovf 
	IL_0011:  newobj instance void class '<>f__AnonymousType0`2'<float64, int64>::'.ctor'(!0, !1)
	IL_0016:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__114

    // method line 209
    .method private static hidebysig 
           default float64 '<AverageEnumerable>b__115' (class '<>f__AnonymousType0`2'<float64, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x760a
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<float64, int64>::get_sum()
	IL_0006:  ldarg.0 
	IL_0007:  callvirt instance !1 class '<>f__AnonymousType0`2'<float64, int64>::get_count()
	IL_000c:  conv.r8 
	IL_000d:  div 
	IL_000e:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__115

    // method line 210
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<float32, int64> '<AverageEnumerable>b__118' (class '<>f__AnonymousType0`2'<float32, int64> prev, float32 cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7691
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<float32, int64>::get_sum()
	IL_0006:  ldarg.1 
	IL_0007:  add 
	IL_0008:  ldarg.0 
	IL_0009:  callvirt instance !1 class '<>f__AnonymousType0`2'<float32, int64>::get_count()
	IL_000e:  ldc.i4.1 
	IL_000f:  conv.i8 
	IL_0010:  add.ovf 
	IL_0011:  newobj instance void class '<>f__AnonymousType0`2'<float32, int64>::'.ctor'(!0, !1)
	IL_0016:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__118

    // method line 211
    .method private static hidebysig 
           default float32 '<AverageEnumerable>b__119' (class '<>f__AnonymousType0`2'<float32, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x76a9
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<float32, int64>::get_sum()
	IL_0006:  ldarg.0 
	IL_0007:  callvirt instance !1 class '<>f__AnonymousType0`2'<float32, int64>::get_count()
	IL_000c:  conv.r4 
	IL_000d:  div 
	IL_000e:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__119

    // method line 212
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64> '<AverageEnumerable>b__11c' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64> prev, valuetype [mscorlib]System.Decimal cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x772d
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::get_sum()
	IL_0006:  ldarg.1 
	IL_0007:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_000c:  ldarg.0 
	IL_000d:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::get_count()
	IL_0012:  ldc.i4.1 
	IL_0013:  conv.i8 
	IL_0014:  add.ovf 
	IL_0015:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::'.ctor'(!0, !1)
	IL_001a:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__11c

    // method line 213
    .method private static hidebysig 
           default valuetype [mscorlib]System.Decimal '<AverageEnumerable>b__11d' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7749
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::get_sum()
	IL_0006:  ldarg.0 
	IL_0007:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Decimal, int64>::get_count()
	IL_000c:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Implicit(int64)
	IL_0011:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_0016:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__11d

    // method line 214
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<int64, int64> '<AverageEnumerable>b__120' (class '<>f__AnonymousType0`2'<int64, int64> prev, int32 cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x77d6
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<int64, int64>::get_sum()
	IL_0006:  ldarg.1 
	IL_0007:  conv.i8 
	IL_0008:  add.ovf 
	IL_0009:  ldarg.0 
	IL_000a:  callvirt instance !1 class '<>f__AnonymousType0`2'<int64, int64>::get_count()
	IL_000f:  ldc.i4.1 
	IL_0010:  conv.i8 
	IL_0011:  add.ovf 
	IL_0012:  newobj instance void class '<>f__AnonymousType0`2'<int64, int64>::'.ctor'(!0, !1)
	IL_0017:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__120

    // method line 215
    .method private static hidebysig 
           default float64 '<AverageEnumerable>b__121' (class '<>f__AnonymousType0`2'<int64, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x77ef
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<int64, int64>::get_sum()
	IL_0006:  conv.r8 
	IL_0007:  ldarg.0 
	IL_0008:  callvirt instance !1 class '<>f__AnonymousType0`2'<int64, int64>::get_count()
	IL_000d:  conv.r8 
	IL_000e:  div 
	IL_000f:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__121

    // method line 216
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<int64, int64> '<AverageEnumerable>b__124' (class '<>f__AnonymousType0`2'<int64, int64> prev, int64 cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x786e
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<int64, int64>::get_sum()
	IL_0006:  ldarg.1 
	IL_0007:  add.ovf 
	IL_0008:  ldarg.0 
	IL_0009:  callvirt instance !1 class '<>f__AnonymousType0`2'<int64, int64>::get_count()
	IL_000e:  ldc.i4.1 
	IL_000f:  conv.i8 
	IL_0010:  add.ovf 
	IL_0011:  newobj instance void class '<>f__AnonymousType0`2'<int64, int64>::'.ctor'(!0, !1)
	IL_0016:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__124

    // method line 217
    .method private static hidebysig 
           default float64 '<AverageEnumerable>b__125' (class '<>f__AnonymousType0`2'<int64, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7886
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !0 class '<>f__AnonymousType0`2'<int64, int64>::get_sum()
	IL_0006:  conv.r8 
	IL_0007:  ldarg.0 
	IL_0008:  callvirt instance !1 class '<>f__AnonymousType0`2'<int64, int64>::get_count()
	IL_000d:  conv.r8 
	IL_000e:  div 
	IL_000f:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__125

    // method line 218
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64> '<AverageEnumerable>b__128' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64> prev, valuetype [mscorlib]System.Nullable`1<float64> cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7908
	// Code size 76 (0x4c)
	.maxstack 4
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float64>	V_0,
		float64	V_1,
		valuetype [mscorlib]System.Nullable`1<float64>	V_2)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<float64>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.0 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::get_sum()
	IL_0011:  stloc.0 
	IL_0012:  ldarga.s 1
	IL_0014:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0019:  stloc.1 
	IL_001a:  ldloca.s 0
	IL_001c:  call instance bool valuetype [mscorlib]System.Nullable`1<float64>::get_HasValue()
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldloca.s 2
	IL_0025:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_002b:  ldloc.2 
	IL_002c:  br.s IL_003d

	IL_002e:  ldloca.s 0
	IL_0030:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0035:  conv.r8 
	IL_0036:  ldloc.1 
	IL_0037:  add 
	IL_0038:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_003d:  ldarg.0 
	IL_003e:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::get_count()
	IL_0043:  ldc.i4.1 
	IL_0044:  conv.i8 
	IL_0045:  add.ovf 
	IL_0046:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::'.ctor'(!0, !1)
	IL_004b:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__128

    // method line 219
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float64> '<AverageEnumerable>b__129' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7960
	// Code size 70 (0x46)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float64>	V_0,
		float64	V_1,
		valuetype [mscorlib]System.Nullable`1<float64>	V_2,
		valuetype [mscorlib]System.Nullable`1<float64>	V_3)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::get_count()
	IL_0006:  ldc.i4.0 
	IL_0007:  conv.i8 
	IL_0008:  beq.s IL_003c

	IL_000a:  ldarg.0 
	IL_000b:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::get_sum()
	IL_0010:  stloc.0 
	IL_0011:  ldarg.0 
	IL_0012:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float64>, int64>::get_count()
	IL_0017:  conv.r8 
	IL_0018:  stloc.1 
	IL_0019:  ldloca.s 0
	IL_001b:  call instance bool valuetype [mscorlib]System.Nullable`1<float64>::get_HasValue()
	IL_0020:  brtrue.s IL_002c

	IL_0022:  ldloca.s 2
	IL_0024:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_002a:  ldloc.2 
	IL_002b:  ret 
	IL_002c:  ldloca.s 0
	IL_002e:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0033:  conv.r8 
	IL_0034:  ldloc.1 
	IL_0035:  div 
	IL_0036:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_003b:  ret 
	IL_003c:  ldloca.s 3
	IL_003e:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0044:  ldloc.3 
	IL_0045:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__129

    // method line 220
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64> '<AverageEnumerable>b__12c' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64> prev, valuetype [mscorlib]System.Nullable`1<float32> cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7a2c
	// Code size 76 (0x4c)
	.maxstack 4
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float32>	V_0,
		float32	V_1,
		valuetype [mscorlib]System.Nullable`1<float32>	V_2)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.0 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::get_sum()
	IL_0011:  stloc.0 
	IL_0012:  ldarga.s 1
	IL_0014:  call instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
	IL_0019:  stloc.1 
	IL_001a:  ldloca.s 0
	IL_001c:  call instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldloca.s 2
	IL_0025:  initobj valuetype [mscorlib]System.Nullable`1<float32>
	IL_002b:  ldloc.2 
	IL_002c:  br.s IL_003d

	IL_002e:  ldloca.s 0
	IL_0030:  call instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
	IL_0035:  conv.r4 
	IL_0036:  ldloc.1 
	IL_0037:  add 
	IL_0038:  newobj instance void valuetype [mscorlib]System.Nullable`1<float32>::'.ctor'(!0)
	IL_003d:  ldarg.0 
	IL_003e:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::get_count()
	IL_0043:  ldc.i4.1 
	IL_0044:  conv.i8 
	IL_0045:  add.ovf 
	IL_0046:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::'.ctor'(!0, !1)
	IL_004b:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__12c

    // method line 221
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float32> '<AverageEnumerable>b__12d' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7a84
	// Code size 70 (0x46)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.Nullable`1<float32>	V_0,
		float32	V_1,
		valuetype [mscorlib]System.Nullable`1<float32>	V_2,
		valuetype [mscorlib]System.Nullable`1<float32>	V_3)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::get_count()
	IL_0006:  ldc.i4.0 
	IL_0007:  conv.i8 
	IL_0008:  beq.s IL_003c

	IL_000a:  ldarg.0 
	IL_000b:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::get_sum()
	IL_0010:  stloc.0 
	IL_0011:  ldarg.0 
	IL_0012:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<float32>, int64>::get_count()
	IL_0017:  conv.r4 
	IL_0018:  stloc.1 
	IL_0019:  ldloca.s 0
	IL_001b:  call instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
	IL_0020:  brtrue.s IL_002c

	IL_0022:  ldloca.s 2
	IL_0024:  initobj valuetype [mscorlib]System.Nullable`1<float32>
	IL_002a:  ldloc.2 
	IL_002b:  ret 
	IL_002c:  ldloca.s 0
	IL_002e:  call instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
	IL_0033:  conv.r4 
	IL_0034:  ldloc.1 
	IL_0035:  div 
	IL_0036:  newobj instance void valuetype [mscorlib]System.Nullable`1<float32>::'.ctor'(!0)
	IL_003b:  ret 
	IL_003c:  ldloca.s 3
	IL_003e:  initobj valuetype [mscorlib]System.Nullable`1<float32>
	IL_0044:  ldloc.3 
	IL_0045:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__12d

    // method line 222
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64> '<AverageEnumerable>b__130' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64> prev, valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7b4c
	// Code size 79 (0x4f)
	.maxstack 4
	.locals init (
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_0,
		valuetype [mscorlib]System.Decimal	V_1,
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_2)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.0 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::get_sum()
	IL_0011:  stloc.0 
	IL_0012:  ldarga.s 1
	IL_0014:  call instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
	IL_0019:  stloc.1 
	IL_001a:  ldloca.s 0
	IL_001c:  call instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldloca.s 2
	IL_0025:  initobj valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
	IL_002b:  ldloc.2 
	IL_002c:  br.s IL_0040

	IL_002e:  ldloca.s 0
	IL_0030:  call instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
	IL_0035:  ldloc.1 
	IL_0036:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_003b:  newobj instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::'.ctor'(!0)
	IL_0040:  ldarg.0 
	IL_0041:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::get_count()
	IL_0046:  ldc.i4.1 
	IL_0047:  conv.i8 
	IL_0048:  add.ovf 
	IL_0049:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::'.ctor'(!0, !1)
	IL_004e:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__130

    // method line 223
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<AverageEnumerable>b__131' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7ba8
	// Code size 77 (0x4d)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_0,
		valuetype [mscorlib]System.Decimal	V_1,
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_2,
		valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>	V_3)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::get_count()
	IL_0006:  ldc.i4.0 
	IL_0007:  conv.i8 
	IL_0008:  beq.s IL_0043

	IL_000a:  ldarg.0 
	IL_000b:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::get_sum()
	IL_0010:  stloc.0 
	IL_0011:  ldarg.0 
	IL_0012:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>, int64>::get_count()
	IL_0017:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Implicit(int64)
	IL_001c:  stloc.1 
	IL_001d:  ldloca.s 0
	IL_001f:  call instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
	IL_0024:  brtrue.s IL_0030

	IL_0026:  ldloca.s 2
	IL_0028:  initobj valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
	IL_002e:  ldloc.2 
	IL_002f:  ret 
	IL_0030:  ldloca.s 0
	IL_0032:  call instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
	IL_0037:  ldloc.1 
	IL_0038:  call valuetype [mscorlib]System.Decimal valuetype [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	IL_003d:  newobj instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::'.ctor'(!0)
	IL_0042:  ret 
	IL_0043:  ldloca.s 3
	IL_0045:  initobj valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
	IL_004b:  ldloc.3 
	IL_004c:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__131

    // method line 224
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> '<AverageEnumerable>b__134' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> prev, valuetype [mscorlib]System.Nullable`1<int32> cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7c78
	// Code size 76 (0x4c)
	.maxstack 4
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0,
		int64	V_1,
		valuetype [mscorlib]System.Nullable`1<int64>	V_2)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.0 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_sum()
	IL_0011:  stloc.0 
	IL_0012:  ldarga.s 1
	IL_0014:  call instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
	IL_0019:  conv.i8 
	IL_001a:  stloc.1 
	IL_001b:  ldloca.s 0
	IL_001d:  call instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
	IL_0022:  brtrue.s IL_002f

	IL_0024:  ldloca.s 2
	IL_0026:  initobj valuetype [mscorlib]System.Nullable`1<int64>
	IL_002c:  ldloc.2 
	IL_002d:  br.s IL_003d

	IL_002f:  ldloca.s 0
	IL_0031:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_0036:  ldloc.1 
	IL_0037:  add.ovf 
	IL_0038:  newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::'.ctor'(!0)
	IL_003d:  ldarg.0 
	IL_003e:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0043:  ldc.i4.1 
	IL_0044:  conv.i8 
	IL_0045:  add.ovf 
	IL_0046:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::'.ctor'(!0, !1)
	IL_004b:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__134

    // method line 225
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float64> '<AverageEnumerable>b__135' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7cd0
	// Code size 106 (0x6a)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0,
		valuetype [mscorlib]System.Nullable`1<float64>	V_1,
		valuetype [mscorlib]System.Nullable`1<float64>	V_2,
		float64	V_3,
		valuetype [mscorlib]System.Nullable`1<float64>	V_4,
		valuetype [mscorlib]System.Nullable`1<float64>	V_5)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0006:  ldc.i4.0 
	IL_0007:  conv.i8 
	IL_0008:  beq.s IL_005f

	IL_000a:  ldarg.0 
	IL_000b:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_sum()
	IL_0010:  stloc.0 
	IL_0011:  ldloca.s 0
	IL_0013:  call instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
	IL_0018:  brtrue.s IL_0025

	IL_001a:  ldloca.s 1
	IL_001c:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0022:  ldloc.1 
	IL_0023:  br.s IL_0032

	IL_0025:  ldloca.s 0
	IL_0027:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_002c:  conv.r8 
	IL_002d:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_0032:  stloc.2 
	IL_0033:  ldarg.0 
	IL_0034:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0039:  conv.r8 
	IL_003a:  stloc.3 
	IL_003b:  ldloca.s 2
	IL_003d:  call instance bool valuetype [mscorlib]System.Nullable`1<float64>::get_HasValue()
	IL_0042:  brtrue.s IL_004f

	IL_0044:  ldloca.s 4
	IL_0046:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_004c:  ldloc.s 4
	IL_004e:  ret 
	IL_004f:  ldloca.s 2
	IL_0051:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0056:  conv.r8 
	IL_0057:  ldloc.3 
	IL_0058:  div 
	IL_0059:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_005e:  ret 
	IL_005f:  ldloca.s 5
	IL_0061:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0067:  ldloc.s 5
	IL_0069:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__135

    // method line 226
    .method private static hidebysig 
           default class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> '<AverageEnumerable>b__138' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> prev, valuetype [mscorlib]System.Nullable`1<int64> cur)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7db8
	// Code size 75 (0x4b)
	.maxstack 4
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0,
		int64	V_1,
		valuetype [mscorlib]System.Nullable`1<int64>	V_2)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
	IL_0007:  brtrue.s IL_000b

	IL_0009:  ldarg.0 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_sum()
	IL_0011:  stloc.0 
	IL_0012:  ldarga.s 1
	IL_0014:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_0019:  stloc.1 
	IL_001a:  ldloca.s 0
	IL_001c:  call instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
	IL_0021:  brtrue.s IL_002e

	IL_0023:  ldloca.s 2
	IL_0025:  initobj valuetype [mscorlib]System.Nullable`1<int64>
	IL_002b:  ldloc.2 
	IL_002c:  br.s IL_003c

	IL_002e:  ldloca.s 0
	IL_0030:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_0035:  ldloc.1 
	IL_0036:  add.ovf 
	IL_0037:  newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::'.ctor'(!0)
	IL_003c:  ldarg.0 
	IL_003d:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0042:  ldc.i4.1 
	IL_0043:  conv.i8 
	IL_0044:  add.ovf 
	IL_0045:  newobj instance void class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::'.ctor'(!0, !1)
	IL_004a:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__138

    // method line 227
    .method private static hidebysig 
           default valuetype [mscorlib]System.Nullable`1<float64> '<AverageEnumerable>b__139' (class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64> s)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7e10
	// Code size 106 (0x6a)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.Nullable`1<int64>	V_0,
		valuetype [mscorlib]System.Nullable`1<float64>	V_1,
		valuetype [mscorlib]System.Nullable`1<float64>	V_2,
		float64	V_3,
		valuetype [mscorlib]System.Nullable`1<float64>	V_4,
		valuetype [mscorlib]System.Nullable`1<float64>	V_5)
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0006:  ldc.i4.0 
	IL_0007:  conv.i8 
	IL_0008:  beq.s IL_005f

	IL_000a:  ldarg.0 
	IL_000b:  callvirt instance !0 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_sum()
	IL_0010:  stloc.0 
	IL_0011:  ldloca.s 0
	IL_0013:  call instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
	IL_0018:  brtrue.s IL_0025

	IL_001a:  ldloca.s 1
	IL_001c:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0022:  ldloc.1 
	IL_0023:  br.s IL_0032

	IL_0025:  ldloca.s 0
	IL_0027:  call instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
	IL_002c:  conv.r8 
	IL_002d:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_0032:  stloc.2 
	IL_0033:  ldarg.0 
	IL_0034:  callvirt instance !1 class '<>f__AnonymousType0`2'<valuetype [mscorlib]System.Nullable`1<int64>, int64>::get_count()
	IL_0039:  conv.r8 
	IL_003a:  stloc.3 
	IL_003b:  ldloca.s 2
	IL_003d:  call instance bool valuetype [mscorlib]System.Nullable`1<float64>::get_HasValue()
	IL_0042:  brtrue.s IL_004f

	IL_0044:  ldloca.s 4
	IL_0046:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_004c:  ldloc.s 4
	IL_004e:  ret 
	IL_004f:  ldloca.s 2
	IL_0051:  call instance !0 valuetype [mscorlib]System.Nullable`1<float64>::GetValueOrDefault()
	IL_0056:  conv.r8 
	IL_0057:  ldloc.3 
	IL_0058:  div 
	IL_0059:  newobj instance void valuetype [mscorlib]System.Nullable`1<float64>::'.ctor'(!0)
	IL_005e:  ret 
	IL_005f:  ldloca.s 5
	IL_0061:  initobj valuetype [mscorlib]System.Nullable`1<float64>
	IL_0067:  ldloc.s 5
	IL_0069:  ret 
    } // end of method EnumerableEx::<AverageEnumerable>b__139

  .class nested private auto ansi beforefieldinit AsynchronousEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private initonly  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> m_source
    .field  private initonly  class [System.CoreEx]System.Concurrency.IScheduler m_scheduler

    // method line 228
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source, class [System.CoreEx]System.Concurrency.IScheduler scheduler)  cil managed 
    {
        // Method begins at RVA 0x8111
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0>::m_source
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  stfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0>::m_scheduler
	IL_0014:  ret 
    } // end of method AsynchronousEnumerable`1::.ctor

    // method line 229
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8168
	// Code size 129 (0x81)
	.maxstack 4
	.locals init (
		class [System.Core]System.Action	V_0,
		class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!T>	V_1,
		class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!T>	V_2)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!T>::'.ctor'()
	IL_0005:  stloc.2 
	IL_0006:  ldloc.2 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0>::'<>4__this'
	IL_000d:  ldloc.2 
	IL_000e:  ldnull 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0>::enumerator
	IL_0014:  ldnull 
	IL_0015:  stloc.0 
	IL_0016:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!T>::'.ctor'()
	IL_001b:  stloc.1 
	IL_001c:  ldloc.1 
	IL_001d:  ldloc.2 
	IL_001e:  stfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::'CS$<>8__locals143'
	IL_0023:  ldloc.1 
	IL_0024:  ldc.i4.0 
	IL_0025:  ldc.i4.1 
	IL_0026:  newobj instance void class [System]System.Threading.Semaphore::'.ctor'(int32, int32)
	IL_002b:  stfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::waiter
	.try { // 0
	  IL_0030:  ldarg.0 
	  IL_0031:  ldfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0>::m_scheduler
	  IL_0036:  ldloc.0 
	  IL_0037:  brtrue.s IL_0046

	  IL_0039:  ldloc.1 
	  IL_003a:  ldftn instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!T>::'<GetEnumerator>b__141'()
	  IL_0040:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	  IL_0045:  stloc.0 
	  IL_0046:  ldloc.0 
	  IL_0047:  callvirt instance class [mscorlib]System.IDisposable class [System.CoreEx]System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	  IL_004c:  pop 
	  IL_004d:  ldloc.1 
	  IL_004e:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::waiter
	  IL_0053:  callvirt instance bool class [mscorlib]System.Threading.WaitHandle::WaitOne()
	  IL_0058:  pop 
	  IL_0059:  leave.s IL_006f

	} // end .try 0
	finally  { // 0
	  IL_005b:  ldloc.1 
	  IL_005c:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::waiter
	  IL_0061:  brfalse.s IL_006e

	  IL_0063:  ldloc.1 
	  IL_0064:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::waiter
	  IL_0069:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_006e:  endfinally 
	} // end handler 0
	IL_006f:  ldloc.2 
	IL_0070:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0>::enumerator
	IL_0075:  ldarg.0 
	IL_0076:  ldfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0>::m_scheduler
	IL_007b:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, class [System.CoreEx]System.Concurrency.IScheduler)
	IL_0080:  ret 
    } // end of method AsynchronousEnumerable`1::GetEnumerator

    // method line 230
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8208
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method AsynchronousEnumerable`1::System.Collections.IEnumerable.GetEnumerator

  .class nested private auto ansi sealed beforefieldinit AsynchronousEnumerator<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.IDisposable, [mscorlib]System.Collections.IEnumerator  {
    .field  private initonly  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> m_source
    .field  private initonly  class [System.CoreEx]System.Concurrency.IScheduler m_scheduler

    // method line 231
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> source, class [System.CoreEx]System.Concurrency.IScheduler scheduler)  cil managed 
    {
        // Method begins at RVA 0x8210
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_source
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  stfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_scheduler
	IL_0014:  ret 
    } // end of method AsynchronousEnumerator::.ctor

    // method line 232
    .method public final virtual hidebysig newslot specialname 
           instance default !T get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8254
	// Code size 81 (0x51)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!T>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!T>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::'<>4__this'
	IL_000d:  ldloc.0 
	IL_000e:  ldflda !0 class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::result
	IL_0013:  initobj !0
	IL_0019:  ldloc.0 
	IL_001a:  ldc.i4.0 
	IL_001b:  ldc.i4.1 
	IL_001c:  newobj instance void class [System]System.Threading.Semaphore::'.ctor'(int32, int32)
	IL_0021:  stfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::waiter
	IL_0026:  ldarg.0 
	IL_0027:  ldfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_scheduler
	IL_002c:  ldloc.0 
	IL_002d:  ldftn instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!T>::'<get_Current>b__147'()
	IL_0033:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0038:  callvirt instance class [mscorlib]System.IDisposable class [System.CoreEx]System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_003d:  pop 
	IL_003e:  ldloc.0 
	IL_003f:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::waiter
	IL_0044:  callvirt instance bool class [mscorlib]System.Threading.WaitHandle::WaitOne()
	IL_0049:  pop 
	IL_004a:  ldloc.0 
	IL_004b:  ldfld !0 class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::result
	IL_0050:  ret 
    } // end of method AsynchronousEnumerator::get_Current

    // method line 233
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x82be
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_scheduler
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!T>::'<Dispose>b__14a'()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance class [mscorlib]System.IDisposable class [System.CoreEx]System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_0017:  pop 
	IL_0018:  ret 
    } // end of method AsynchronousEnumerator::Dispose

    // method line 234
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        // Method begins at RVA 0x82d8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!T>::get_Current()
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method AsynchronousEnumerator::System.Collections.IEnumerator.get_Current

    // method line 235
    .method public final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x833c
	// Code size 103 (0x67)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!T>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!T>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::'<>4__this'
	IL_000d:  ldloc.0 
	IL_000e:  ldc.i4.0 
	IL_000f:  stfld bool class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::result
	IL_0014:  ldloc.0 
	IL_0015:  ldnull 
	IL_0016:  stfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::exception
	IL_001b:  ldloc.0 
	IL_001c:  ldc.i4.0 
	IL_001d:  ldc.i4.1 
	IL_001e:  newobj instance void class [System]System.Threading.Semaphore::'.ctor'(int32, int32)
	IL_0023:  stfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::waiter
	IL_0028:  ldarg.0 
	IL_0029:  ldfld class [System.CoreEx]System.Concurrency.IScheduler class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_scheduler
	IL_002e:  ldloc.0 
	IL_002f:  ldftn instance void class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!T>::'<MoveNext>b__14b'()
	IL_0035:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_003a:  callvirt instance class [mscorlib]System.IDisposable class [System.CoreEx]System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_003f:  pop 
	IL_0040:  ldloc.0 
	IL_0041:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::waiter
	IL_0046:  callvirt instance bool class [mscorlib]System.Threading.WaitHandle::WaitOne()
	IL_004b:  pop 
	IL_004c:  ldloc.0 
	IL_004d:  ldfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::exception
	IL_0052:  brfalse.s IL_0060

	IL_0054:  ldloc.0 
	IL_0055:  ldfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::exception
	IL_005a:  call class [mscorlib]System.Exception class [System.CoreEx]System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	IL_005f:  throw 
	IL_0060:  ldloc.0 
	IL_0061:  ldfld bool class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::result
	IL_0066:  ret 
    } // end of method AsynchronousEnumerator::MoveNext

    // method line 236
    .method public final virtual hidebysig newslot 
           instance default void Reset ()  cil managed 
    {
        // Method begins at RVA 0x83af
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method AsynchronousEnumerator::Reset

    // method line 237
    .method private hidebysig 
           instance default void '<Dispose>b__14a' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x82b1
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_source
	IL_0006:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_000b:  ret 
    } // end of method AsynchronousEnumerator::<Dispose>b__14a

	.property instance !T Current ()
	{
		.get instance default !0 System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator::get_Current () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator::System.Collections.IEnumerator.get_Current () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass148'<T>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  !0 result
    .field  public  class [System]System.Threading.Semaphore waiter
    .field  public  class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> '<>4__this'

    // method line 613
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x8226
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass148::.ctor

    // method line 614
    .method public hidebysig 
           instance default void '<get_Current>b__147' ()  cil managed 
    {
        // Method begins at RVA 0x822e
	// Code size 35 (0x23)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.0 
	IL_0002:  ldfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::'<>4__this'
	IL_0007:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_source
	IL_000c:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	IL_0011:  stfld !0 class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::result
	IL_0016:  ldarg.0 
	IL_0017:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass148'<!0>::waiter
	IL_001c:  callvirt instance int32 class [System]System.Threading.Semaphore::Release()
	IL_0021:  pop 
	IL_0022:  ret 
    } // end of method <>c__DisplayClass148::<get_Current>b__147

  } // end of class <>c__DisplayClass148

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass14c'<T>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  bool result
    .field  public  class [mscorlib]System.Exception exception
    .field  public  class [System]System.Threading.Semaphore waiter
    .field  public  class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> '<>4__this'

    // method line 615
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x82e5
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass14c::.ctor

    // method line 616
    .method public hidebysig 
           instance default void '<MoveNext>b__14b' ()  cil managed 
    {
        // Method begins at RVA 0x82f0
	// Code size 47 (0x2f)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Exception	V_0)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldarg.0 
	  IL_0002:  ldfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::'<>4__this'
	  IL_0007:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator<!0>::m_source
	  IL_000c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0011:  stfld bool class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::result
	  IL_0016:  leave.s IL_0022

	} // end .try 0
	catch class [mscorlib]System.Exception { // 0
	  IL_0018:  stloc.0 
	  IL_0019:  ldarg.0 
	  IL_001a:  ldloc.0 
	  IL_001b:  stfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::exception
	  IL_0020:  leave.s IL_0022

	} // end handler 0
	IL_0022:  ldarg.0 
	IL_0023:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/AsynchronousEnumerator/'<>c__DisplayClass14c'<!0>::waiter
	IL_0028:  callvirt instance int32 class [System]System.Threading.Semaphore::Release()
	IL_002d:  pop 
	IL_002e:  ret 
    } // end of method <>c__DisplayClass14c::<MoveNext>b__14b

  } // end of class <>c__DisplayClass14c

  } // end of class AsynchronousEnumerator

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass142'<T>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> enumerator
    .field  public  class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0> '<>4__this'

    // method line 610
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x8127
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass142::.ctor

  } // end of class <>c__DisplayClass142

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass145'<T>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0> 'CS$<>8__locals143'
    .field  public  class [System]System.Threading.Semaphore waiter

    // method line 611
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x812f
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass145::.ctor

    // method line 612
    .method public hidebysig 
           instance default void '<GetEnumerator>b__141' ()  cil managed 
    {
        // Method begins at RVA 0x8137
	// Code size 45 (0x2d)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::'CS$<>8__locals143'
	IL_0006:  ldarg.0 
	IL_0007:  ldfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::'CS$<>8__locals143'
	IL_000c:  ldfld class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0>::'<>4__this'
	IL_0011:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1<!0>::m_source
	IL_0016:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
	IL_001b:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass142'<!0>::enumerator
	IL_0020:  ldarg.0 
	IL_0021:  ldfld class [System]System.Threading.Semaphore class System.Linq.EnumerableEx/AsynchronousEnumerable`1/'<>c__DisplayClass145'<!0>::waiter
	IL_0026:  callvirt instance int32 class [System]System.Threading.Semaphore::Release()
	IL_002b:  pop 
	IL_002c:  ret 
    } // end of method <>c__DisplayClass145::<GetEnumerator>b__141

  } // end of class <>c__DisplayClass145

  } // end of class AsynchronousEnumerable`1

  .class nested private auto ansi beforefieldinit DeferredEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private  class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> m_deferredEnumerable

    // method line 238
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>> deferredEnumerable)  cil managed 
    {
        // Method begins at RVA 0x83b6
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/DeferredEnumerable`1<!0>::m_deferredEnumerable
	IL_000d:  ret 
    } // end of method DeferredEnumerable`1::.ctor

    // method line 239
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x83c5
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/DeferredEnumerable`1<!0>::m_deferredEnumerable
	IL_0006:  callvirt instance !0 class [System.Core]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>>::Invoke()
	IL_000b:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
	IL_0010:  ret 
    } // end of method DeferredEnumerable`1::GetEnumerator

    // method line 240
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x83d7
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/DeferredEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method DeferredEnumerable`1::System.Collections.IEnumerable.GetEnumerator

  } // end of class DeferredEnumerable`1

  .class nested private auto ansi sealed serializable beforefieldinit SerializableEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private  class System.Linq.EnumerableEx/RemotableEnumerable`1<!0> remotableEnumerable

    // method line 241
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class System.Linq.EnumerableEx/RemotableEnumerable`1<!T> remotableEnumerable)  cil managed 
    {
        // Method begins at RVA 0x83df
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class System.Linq.EnumerableEx/RemotableEnumerable`1<!0> class System.Linq.EnumerableEx/SerializableEnumerable`1<!0>::remotableEnumerable
	IL_000d:  ret 
    } // end of method SerializableEnumerable`1::.ctor

    // method line 242
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x83ee
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/RemotableEnumerable`1<!0> class System.Linq.EnumerableEx/SerializableEnumerable`1<!0>::remotableEnumerable
	IL_0006:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerable`1<!T>::GetEnumerator()
	IL_000b:  ret 
    } // end of method SerializableEnumerable`1::GetEnumerator

    // method line 243
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x83fb
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/SerializableEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method SerializableEnumerable`1::System.Collections.IEnumerable.GetEnumerator

  } // end of class SerializableEnumerable`1

  .class nested private auto ansi sealed beforefieldinit RemotableEnumerator`1<T>
  	extends [mscorlib]System.MarshalByRefObject
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.IDisposable, [mscorlib]System.Collections.IEnumerator  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> underlyingEnumerator

    // method line 244
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> underlyingEnumerator)  cil managed 
    {
        // Method begins at RVA 0x8403
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class [mscorlib]System.MarshalByRefObject::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerator`1<!0>::underlyingEnumerator
	IL_000d:  ret 
    } // end of method RemotableEnumerator`1::.ctor

    // method line 245
    .method public final virtual hidebysig newslot specialname 
           instance default !T get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8412
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerator`1<!0>::underlyingEnumerator
	IL_0006:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	IL_000b:  ret 
    } // end of method RemotableEnumerator`1::get_Current

    // method line 246
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x841f
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerator`1<!0>::underlyingEnumerator
	IL_0006:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_000b:  ret 
    } // end of method RemotableEnumerator`1::Dispose

    // method line 247
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        // Method begins at RVA 0x842c
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 class System.Linq.EnumerableEx/RemotableEnumerator`1<!T>::get_Current()
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method RemotableEnumerator`1::System.Collections.IEnumerator.get_Current

    // method line 248
    .method public final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x8439
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerator`1<!0>::underlyingEnumerator
	IL_0006:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	IL_000b:  ret 
    } // end of method RemotableEnumerator`1::MoveNext

    // method line 249
    .method public final virtual hidebysig newslot 
           instance default void Reset ()  cil managed 
    {
        // Method begins at RVA 0x8446
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerator`1<!0>::underlyingEnumerator
	IL_0006:  callvirt instance void class [mscorlib]System.Collections.IEnumerator::Reset()
	IL_000b:  ret 
    } // end of method RemotableEnumerator`1::Reset

	.property instance !T Current ()
	{
		.get instance default !0 System.Linq.EnumerableEx/RemotableEnumerator`1::get_Current () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/RemotableEnumerator`1::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class RemotableEnumerator`1

  .class nested private auto ansi sealed beforefieldinit RemotableEnumerable`1<T>
  	extends [mscorlib]System.MarshalByRefObject
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> underlyingEnumerable

    // method line 250
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> underlyingEnumerable)  cil managed 
    {
        // Method begins at RVA 0x8453
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class [mscorlib]System.MarshalByRefObject::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/RemotableEnumerable`1<!0>::underlyingEnumerable
	IL_000d:  ret 
    } // end of method RemotableEnumerable`1::.ctor

    // method line 251
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8462
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/RemotableEnumerable`1<!0>::underlyingEnumerable
	IL_0006:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
	IL_000b:  newobj instance void class System.Linq.EnumerableEx/RemotableEnumerator`1<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>)
	IL_0010:  ret 
    } // end of method RemotableEnumerable`1::GetEnumerator

    // method line 252
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8474
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/RemotableEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method RemotableEnumerable`1::System.Collections.IEnumerable.GetEnumerator

  } // end of class RemotableEnumerable`1

  .class nested private auto ansi beforefieldinit MemoizeAllEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private  class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> m_list
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> m_source
    .field  private  object m_gate

    // method line 253
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source)  cil managed 
    {
        // Method begins at RVA 0x847c
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void object::'.ctor'()
	IL_0006:  stfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_gate
	IL_000b:  ldarg.0 
	IL_000c:  call instance void object::'.ctor'()
	IL_0011:  ldarg.0 
	IL_0012:  ldarg.1 
	IL_0013:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_source
	IL_0018:  ret 
    } // end of method MemoizeAllEnumerable`1::.ctor

    // method line 254
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x850c
	// Code size 18 (0x12)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldftn instance class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!T>::'<GetEnumerator>b__14e'()
	IL_0007:  newobj instance void class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>>::'.ctor'(object, native int)
	IL_000c:  call class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!T>::WalkList(class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>>)
	IL_0011:  ret 
    } // end of method MemoizeAllEnumerable`1::GetEnumerator

    // method line 255
    .method private static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> WalkList (class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>> listFactory)  cil managed 
    {
        // Method begins at RVA 0x85e8
	// Code size 16 (0x10)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!T>	V_0)
	IL_0000:  ldc.i4.0 
	IL_0001:  newobj instance void class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!T>::'.ctor'(int32)
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldarg.0 
	IL_0009:  stfld class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::listFactory
	IL_000e:  ldloc.0 
	IL_000f:  ret 
    } // end of method MemoizeAllEnumerable`1::WalkList

    // method line 256
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8604
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method MemoizeAllEnumerable`1::System.Collections.IEnumerable.GetEnumerator

    // method line 257
    .method private hidebysig 
           instance default class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T> '<GetEnumerator>b__14e' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8498
	// Code size 85 (0x55)
	.maxstack 3
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_source
	IL_0006:  brfalse.s IL_004e

	IL_0008:  ldarg.0 
	IL_0009:  ldfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_gate
	IL_000e:  dup 
	IL_000f:  stloc.0 
	IL_0010:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0015:  ldarg.0 
	  IL_0016:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_source
	  IL_001b:  brfalse.s IL_0045

	  IL_001d:  ldarg.0 
	  IL_001e:  ldarg.0 
	  IL_001f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_source
	  IL_0024:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>> class System.Linq.EnumerableEx::Materialize<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	  IL_0029:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>>::GetEnumerator()
	  IL_002e:  ldarg.0 
	  IL_002f:  ldfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_gate
	  IL_0034:  call class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::Create(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, object)
	  IL_0039:  stfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_list
	  IL_003e:  ldarg.0 
	  IL_003f:  ldnull 
	  IL_0040:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_source
	  IL_0045:  leave.s IL_004e

	} // end .try 0
	finally  { // 0
	  IL_0047:  ldloc.0 
	  IL_0048:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_004d:  endfinally 
	} // end handler 0
	IL_004e:  ldarg.0 
	IL_004f:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1<!0>::m_list
	IL_0054:  ret 
    } // end of method MemoizeAllEnumerable`1::<GetEnumerator>b__14e

  .class nested private auto ansi beforefieldinit LinkedList<T>
  	extends [mscorlib]System.Object
  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> m_enumerator
    .field  private  class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> m_next
    .field  private  object m_gate
    .field  private  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<Current>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 258
    .method public hidebysig specialname 
           instance default class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T> get_Next ()  cil managed 
    {
        // Method begins at RVA 0x860c
	// Code size 75 (0x4b)
	.maxstack 3
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_enumerator
	IL_0006:  brfalse.s IL_0044

	IL_0008:  ldarg.0 
	IL_0009:  ldfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_gate
	IL_000e:  dup 
	IL_000f:  stloc.0 
	IL_0010:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0015:  ldarg.0 
	  IL_0016:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_001b:  brfalse.s IL_003b

	  IL_001d:  ldarg.0 
	  IL_001e:  ldarg.0 
	  IL_001f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_0024:  ldarg.0 
	  IL_0025:  ldfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_gate
	  IL_002a:  call class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::Create(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, object)
	  IL_002f:  stfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_next
	  IL_0034:  ldarg.0 
	  IL_0035:  ldnull 
	  IL_0036:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_003b:  leave.s IL_0044

	} // end .try 0
	finally  { // 0
	  IL_003d:  ldloc.0 
	  IL_003e:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0043:  endfinally 
	} // end handler 0
	IL_0044:  ldarg.0 
	IL_0045:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_next
	IL_004a:  ret 
    } // end of method LinkedList::get_Next

    // method line 259
    .method public hidebysig specialname 
           instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!T> get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8674
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::'<Current>k__BackingField'
	IL_0006:  ret 
    } // end of method LinkedList::get_Current

    // method line 260
    .method private hidebysig specialname 
           instance default void set_Current (class [System.CoreEx]System.Collections.Generic.Notification`1<!T> 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x867c
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::'<Current>k__BackingField'
	IL_0007:  ret 
    } // end of method LinkedList::set_Current

    // method line 261
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>> enumerator, class [System.CoreEx]System.Collections.Generic.Notification`1<!T> current, object gate)  cil managed 
    {
        // Method begins at RVA 0x8685
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.2 
	IL_0008:  call instance void class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::set_Current(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_enumerator
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.3 
	IL_0016:  stfld object class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>::m_gate
	IL_001b:  ret 
    } // end of method LinkedList::.ctor

    // method line 262
    .method public static hidebysig 
           default class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T> Create (class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>> enumerator, object gate)  cil managed 
    {
        // Method begins at RVA 0x86a4
	// Code size 51 (0x33)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>	V_0,
		object	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  dup 
	IL_0002:  stloc.1 
	IL_0003:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0008:  ldarg.0 
	  IL_0009:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_000e:  brtrue.s IL_001a

	  IL_0010:  ldarg.0 
	  IL_0011:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0016:  ldnull 
	  IL_0017:  stloc.0 
	  IL_0018:  leave.s IL_0031

	  IL_001a:  ldarg.0 
	  IL_001b:  ldarg.0 
	  IL_001c:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>>::get_Current()
	  IL_0021:  ldarg.1 
	  IL_0022:  newobj instance void class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, object)
	  IL_0027:  stloc.0 
	  IL_0028:  leave.s IL_0031

	} // end .try 0
	finally  { // 0
	  IL_002a:  ldloc.1 
	  IL_002b:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0030:  endfinally 
	} // end handler 0
	IL_0031:  ldloc.0 
	IL_0032:  ret 
    } // end of method LinkedList::Create

	.property instance class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T> Next ()
	{
		.get instance default class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList::get_Next () 
	}
	.property instance class [System.CoreEx]System.Collections.Generic.Notification`1<!T> Current ()
	{
		.get instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!0> System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList::get_Current () 
		.set instance default void System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList::set_Current (class [System.CoreEx]System.Collections.Generic.Notification`1<!0> 'value') 
	}
  } // end of class LinkedList

  .class nested private auto ansi sealed beforefieldinit '<WalkList>d__14f'<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  public  class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>> listFactory
    .field  public  class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> '<current>5__150'

    // method line 617
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x8520
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 140 (0x8c)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_006a)
	IL_0015:  br.s IL_008a

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::listFactory
	IL_0025:  callvirt instance !0 class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>>::Invoke()
	IL_002a:  stfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_002f:  ldarg.0 
	IL_0030:  ldnull 
	IL_0031:  stfld class [System.Core]System.Func`1<class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0>> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::listFactory
	IL_0036:  br.s IL_0082

	IL_0038:  ldarg.0 
	IL_0039:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_003e:  callvirt instance class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::get_Current()
	IL_0043:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!T>::get_Kind()
	IL_0048:  ldc.i4.2 
	IL_0049:  beq.s IL_008a

	IL_004b:  ldarg.0 
	IL_004c:  ldarg.0 
	IL_004d:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_0052:  callvirt instance class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::get_Current()
	IL_0057:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_005c:  stfld !0 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>2__current'
	IL_0061:  ldarg.0 
	IL_0062:  ldc.i4.1 
	IL_0063:  stfld int32 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>1__state'
	IL_0068:  ldc.i4.1 
	IL_0069:  ret 
	IL_006a:  ldarg.0 
	IL_006b:  ldc.i4.m1 
	IL_006c:  stfld int32 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>1__state'
	IL_0071:  ldarg.0 
	IL_0072:  ldarg.0 
	IL_0073:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_0078:  callvirt instance class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!T>::get_Next()
	IL_007d:  stfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_0082:  ldarg.0 
	IL_0083:  ldfld class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<current>5__150'
	IL_0088:  brtrue.s IL_0038

	IL_008a:  ldc.i4.0 
	IL_008b:  ret 
    } // end of method <WalkList>d__14f::MoveNext

    // method line 618
    .method private final virtual hidebysig newslot specialname 
           instance default !T 'System.Collections.Generic.IEnumerator<T>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x85b8
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <WalkList>d__14f::System.Collections.Generic.IEnumerator<T>.get_Current

    // method line 619
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x85c0
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <WalkList>d__14f::System.Collections.IEnumerator.Reset

    // method line 620
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x85c7
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <WalkList>d__14f::System.IDisposable.Dispose

    // method line 621
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x85c9
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <WalkList>d__14f::System.Collections.IEnumerator.get_Current

    // method line 622
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x85d6
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'<!0>::'<>1__state'
	IL_000d:  ret 
    } // end of method <WalkList>d__14f::.ctor

	.property instance !T 'System.Collections.Generic.IEnumerator<T>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'::'System.Collections.Generic.IEnumerator<T>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/MemoizeAllEnumerable`1/'<WalkList>d__14f'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <WalkList>d__14f

  } // end of class MemoizeAllEnumerable`1

  .class nested private auto ansi sealed beforefieldinit MemoizeEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable  {
    .field  private  class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> m_starter
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> m_source
    .field  private  object m_gate
    .field  private  int32 m_bufferSize

    // method line 263
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source)  cil managed 
    {
        // Method begins at RVA 0x86f4
	// Code size 9 (0x9)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  ldc.i4.0 
	IL_0003:  call instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, int32)
	IL_0008:  ret 
    } // end of method MemoizeEnumerable`1::.ctor

    // method line 264
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source, int32 bufferSize)  cil managed 
    {
        // Method begins at RVA 0x86fe
	// Code size 32 (0x20)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  newobj instance void object::'.ctor'()
	IL_000c:  stfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_gate
	IL_0011:  ldarg.0 
	IL_0012:  ldarg.1 
	IL_0013:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_source
	IL_0018:  ldarg.0 
	IL_0019:  ldarg.2 
	IL_001a:  stfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_bufferSize
	IL_001f:  ret 
    } // end of method MemoizeEnumerable`1::.ctor

    // method line 265
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8720
	// Code size 86 (0x56)
	.maxstack 5
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_source
	IL_0006:  brfalse.s IL_004a

	IL_0008:  ldarg.0 
	IL_0009:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_gate
	IL_000e:  dup 
	IL_000f:  stloc.0 
	IL_0010:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0015:  ldarg.0 
	  IL_0016:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_source
	  IL_001b:  brfalse.s IL_0041

	  IL_001d:  ldarg.0 
	  IL_001e:  ldarg.0 
	  IL_001f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_source
	  IL_0024:  ldarg.0 
	  IL_0025:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_gate
	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_bufferSize
	  IL_0030:  newobj instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, object, int32)
	  IL_0035:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_starter
	  IL_003a:  ldarg.0 
	  IL_003b:  ldnull 
	  IL_003c:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_source
	  IL_0041:  leave.s IL_004a

	} // end .try 0
	finally  { // 0
	  IL_0043:  ldloc.0 
	  IL_0044:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0049:  endfinally 
	} // end handler 0
	IL_004a:  ldarg.0 
	IL_004b:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!0>::m_starter
	IL_0050:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::WalkList()
	IL_0055:  ret 
    } // end of method MemoizeEnumerable`1::GetEnumerator

    // method line 266
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8794
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method MemoizeEnumerable`1::System.Collections.IEnumerable.GetEnumerator

  .class nested private auto ansi sealed beforefieldinit Starter<T>
  	extends [mscorlib]System.Object
  {
    .field  private  class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> m_positions
    .field  private  int32 m_position
    .field  private  int32 m_bufferSize
    .field  private  object m_gate
    .field  private  class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> '<Current>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 267
    .method public hidebysig specialname 
           instance default class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x879c
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::'<Current>k__BackingField'
	IL_0006:  ret 
    } // end of method Starter::get_Current

    // method line 268
    .method public hidebysig specialname 
           instance default void set_Current (class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x87a4
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::'<Current>k__BackingField'
	IL_0007:  ret 
    } // end of method Starter::set_Current

    // method line 269
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source, object gate, int32 bufferSize)  cil managed 
    {
        // Method begins at RVA 0x87ad
	// Code size 63 (0x3f)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>> class System.Linq.EnumerableEx::Materialize<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	IL_000d:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>>::GetEnumerator()
	IL_0012:  ldnull 
	IL_0013:  ldarg.2 
	IL_0014:  call class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::Create(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, object)
	IL_0019:  call instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::set_Current(class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>)
	IL_001e:  ldarg.0 
	IL_001f:  ldc.i4.0 
	IL_0020:  stfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_position
	IL_0025:  ldarg.0 
	IL_0026:  ldarg.3 
	IL_0027:  stfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_bufferSize
	IL_002c:  ldarg.0 
	IL_002d:  ldarg.2 
	IL_002e:  stfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_gate
	IL_0033:  ldarg.0 
	IL_0034:  newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::'.ctor'()
	IL_0039:  stfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	IL_003e:  ret 
    } // end of method Starter::.ctor

    // method line 270
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> WalkList ()  cil managed 
    {
        // Method begins at RVA 0x87ed
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!T>::'.ctor'(class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>)
	IL_0006:  ret 
    } // end of method Starter::WalkList

    // method line 271
    .method assembly hidebysig 
           instance default void Add (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> enumerator)  cil managed 
    {
        // Method begins at RVA 0x87f8
	// Code size 41 (0x29)
	.maxstack 3
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_gate
	IL_0006:  dup 
	IL_0007:  stloc.0 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	  IL_0013:  ldarg.1 
	  IL_0014:  ldarg.0 
	  IL_0015:  ldfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_position
	  IL_001a:  callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::set_Item(!0, !1)
	  IL_001f:  leave.s IL_0028

	} // end .try 0
	finally  { // 0
	  IL_0021:  ldloc.0 
	  IL_0022:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0027:  endfinally 
	} // end handler 0
	IL_0028:  ret 
    } // end of method Starter::Add

    // method line 272
    .method assembly hidebysig 
           instance default void Remove (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> enumerator)  cil managed 
    {
        // Method begins at RVA 0x8840
	// Code size 36 (0x24)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_gate
	IL_0006:  dup 
	IL_0007:  stloc.0 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	  IL_0013:  ldarg.1 
	  IL_0014:  callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::Remove(!0)
	  IL_0019:  pop 
	  IL_001a:  leave.s IL_0023

	} // end .try 0
	finally  { // 0
	  IL_001c:  ldloc.0 
	  IL_001d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0022:  endfinally 
	} // end handler 0
	IL_0023:  ret 
    } // end of method Starter::Remove

    // method line 273
    .method private hidebysig 
           instance default void MoveForward (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> enumerator)  cil managed 
    {
        // Method begins at RVA 0x8880
	// Code size 161 (0xa1)
	.maxstack 4
	.locals init (
		int32	V_0,
		int32	V_1,
		int32	V_2,
		object	V_3,
		class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>	V_4,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>	V_5)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_gate
	IL_0006:  dup 
	IL_0007:  stloc.3 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	  IL_0013:  ldarg.1 
	  IL_0014:  callvirt instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::get_Item(!0)
	  IL_0019:  ldc.i4 2147483647
	  IL_001e:  bne.un.s IL_002b

	  IL_0020:  ldstr "MemoizedEnumerable can only be used for sequences holding less than Int32.MaxValue values."
	  IL_0025:  newobj instance void class [mscorlib]System.InvalidOperationException::'.ctor'(string)
	  IL_002a:  throw 
	  IL_002b:  ldarg.0 
	  IL_002c:  ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	  IL_0031:  dup 
	  IL_0032:  stloc.s 4
	  IL_0034:  ldarg.1 
	  IL_0035:  dup 
	  IL_0036:  stloc.s 5
	  IL_0038:  ldloc.s 4
	  IL_003a:  ldloc.s 5
	  IL_003c:  callvirt instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::get_Item(!0)
	  IL_0041:  ldc.i4.1 
	  IL_0042:  add 
	  IL_0043:  callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::set_Item(!0, !1)
	  IL_0048:  ldc.i4.0 
	  IL_0049:  ldarg.0 
	  IL_004a:  ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>,int32> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_positions
	  IL_004f:  callvirt instance class [mscorlib]System.Collections.Generic.Dictionary`2/ValueCollection<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>, int32>::get_Values()
	  IL_0054:  call int32 class [System.Core]System.Linq.Enumerable::Max(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>)
	  IL_0059:  ldarg.0 
	  IL_005a:  ldfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_bufferSize
	  IL_005f:  sub 
	  IL_0060:  call int32 class [mscorlib]System.Math::Max(int32, int32)
	  IL_0065:  stloc.0 
	  IL_0066:  ldloc.0 
	  IL_0067:  ldarg.0 
	  IL_0068:  ldfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_position
	  IL_006d:  sub 
	  IL_006e:  stloc.1 
	  IL_006f:  ldloc.1 
	  IL_0070:  ldc.i4.0 
	  IL_0071:  ble.s IL_0090

	  IL_0073:  ldc.i4.0 
	  IL_0074:  stloc.2 
	  IL_0075:  br.s IL_008c

	  IL_0077:  ldarg.0 
	  IL_0078:  ldarg.0 
	  IL_0079:  call instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::get_Current()
	  IL_007e:  callvirt instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::get_Next()
	  IL_0083:  call instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::set_Current(class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>)
	  IL_0088:  ldloc.2 
	  IL_0089:  ldc.i4.1 
	  IL_008a:  add 
	  IL_008b:  stloc.2 
	  IL_008c:  ldloc.2 
	  IL_008d:  ldloc.1 
	  IL_008e:  blt.s IL_0077

	  IL_0090:  ldarg.0 
	  IL_0091:  ldloc.0 
	  IL_0092:  stfld int32 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0>::m_position
	  IL_0097:  leave.s IL_00a0

	} // end .try 0
	finally  { // 0
	  IL_0099:  ldloc.3 
	  IL_009a:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_009f:  endfinally 
	} // end handler 0
	IL_00a0:  ret 
    } // end of method Starter::MoveForward

	.property instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> Current ()
	{
		.get instance default class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter::get_Current () 
		.set instance default void System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter::set_Current (class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> 'value') 
	}
  .class nested private auto ansi sealed beforefieldinit Enumerator<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.IDisposable, [mscorlib]System.Collections.IEnumerator  {
    .field  private  class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> m_current
    .field  private  class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> m_starter

    // method line 274
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T> starter)  cil managed 
    {
        // Method begins at RVA 0x8940
	// Code size 38 (0x26)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_starter
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.1 
	IL_000f:  callvirt instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::get_Current()
	IL_0014:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_0019:  ldarg.0 
	IL_001a:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_starter
	IL_001f:  ldarg.0 
	IL_0020:  callvirt instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::Add(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>)
	IL_0025:  ret 
    } // end of method Enumerator::.ctor

    // method line 275
    .method public final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x8968
	// Code size 54 (0x36)
	.maxstack 2
	.locals init (
		bool	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_0006:  brtrue.s IL_000a

	IL_0008:  ldc.i4.0 
	IL_0009:  ret 
	IL_000a:  ldarg.0 
	IL_000b:  ldarg.0 
	IL_000c:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_0011:  callvirt instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::get_Next()
	IL_0016:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_001b:  ldarg.0 
	IL_001c:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_0021:  ldnull 
	IL_0022:  ceq 
	IL_0024:  ldc.i4.0 
	IL_0025:  ceq 
	IL_0027:  stloc.0 
	IL_0028:  ldarg.0 
	IL_0029:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_starter
	IL_002e:  ldarg.0 
	IL_002f:  callvirt instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::MoveForward(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>)
	IL_0034:  ldloc.0 
	IL_0035:  ret 
    } // end of method Enumerator::MoveNext

    // method line 276
    .method public final virtual hidebysig newslot specialname 
           instance default !T get_Current ()  cil managed 
    {
        // Method begins at RVA 0x89aa
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_0006:  callvirt instance class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::get_Current()
	IL_000b:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0010:  ret 
    } // end of method Enumerator::get_Current

    // method line 277
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x89bc
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_starter
	IL_0006:  ldarg.0 
	IL_0007:  callvirt instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!T>::Remove(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>)
	IL_000c:  ldarg.0 
	IL_000d:  ldnull 
	IL_000e:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_starter
	IL_0013:  ldarg.0 
	IL_0014:  ldnull 
	IL_0015:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!0>::m_current
	IL_001a:  ret 
    } // end of method Enumerator::Dispose

    // method line 278
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        // Method begins at RVA 0x89d8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 class System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator<!T>::get_Current()
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method Enumerator::System.Collections.IEnumerator.get_Current

    // method line 279
    .method public final virtual hidebysig newslot 
           instance default void Reset ()  cil managed 
    {
        // Method begins at RVA 0x89e5
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method Enumerator::Reset

	.property instance !T Current ()
	{
		.get instance default !0 System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator::get_Current () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/MemoizeEnumerable`1/Starter/Enumerator::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class Enumerator

  } // end of class Starter

  .class nested private auto ansi beforefieldinit LinkedList<T>
  	extends [mscorlib]System.Object
  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> m_enumerator
    .field  private  class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> m_next
    .field  private  object m_gate
    .field  private  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<Current>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 280
    .method public hidebysig specialname 
           instance default class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> get_Next ()  cil managed 
    {
        // Method begins at RVA 0x89ec
	// Code size 136 (0x88)
	.maxstack 4
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	IL_0006:  brfalse.s IL_0081

	IL_0008:  ldarg.0 
	IL_0009:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_gate
	IL_000e:  dup 
	IL_000f:  stloc.0 
	IL_0010:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0015:  ldarg.0 
	  IL_0016:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_001b:  brfalse.s IL_0078

	  IL_001d:  ldarg.0 
	  IL_001e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_0023:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0028:  brtrue.s IL_0033

	  IL_002a:  ldarg.0 
	  IL_002b:  ldnull 
	  IL_002c:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_next
	  IL_0031:  br.s IL_0071

	  IL_0033:  ldarg.0 
	  IL_0034:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_0039:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>>::get_Current()
	  IL_003e:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!T>::get_Kind()
	  IL_0043:  ldc.i4.2 
	  IL_0044:  bne.un.s IL_004f

	  IL_0046:  ldarg.0 
	  IL_0047:  ldnull 
	  IL_0048:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_next
	  IL_004d:  br.s IL_0071

	  IL_004f:  ldarg.0 
	  IL_0050:  ldarg.0 
	  IL_0051:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_0056:  ldarg.0 
	  IL_0057:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_005c:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>>::get_Current()
	  IL_0061:  ldarg.0 
	  IL_0062:  ldfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_gate
	  IL_0067:  call class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::Create(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, object)
	  IL_006c:  stfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_next
	  IL_0071:  ldarg.0 
	  IL_0072:  ldnull 
	  IL_0073:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	  IL_0078:  leave.s IL_0081

	} // end .try 0
	finally  { // 0
	  IL_007a:  ldloc.0 
	  IL_007b:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0080:  endfinally 
	} // end handler 0
	IL_0081:  ldarg.0 
	IL_0082:  ldfld class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_next
	IL_0087:  ret 
    } // end of method LinkedList::get_Next

    // method line 281
    .method public hidebysig specialname 
           instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!T> get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8a90
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::'<Current>k__BackingField'
	IL_0006:  ret 
    } // end of method LinkedList::get_Current

    // method line 282
    .method private hidebysig specialname 
           instance default void set_Current (class [System.CoreEx]System.Collections.Generic.Notification`1<!T> 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8a98
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::'<Current>k__BackingField'
	IL_0007:  ret 
    } // end of method LinkedList::set_Current

    // method line 283
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>> enumerator, class [System.CoreEx]System.Collections.Generic.Notification`1<!T> current, object gate)  cil managed 
    {
        // Method begins at RVA 0x8aa1
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.2 
	IL_0008:  call instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::set_Current(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_enumerator
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.3 
	IL_0016:  stfld object class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0>::m_gate
	IL_001b:  ret 
    } // end of method LinkedList::.ctor

    // method line 284
    .method public static hidebysig 
           default class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> Create (class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!T>> enumerator, class [System.CoreEx]System.Collections.Generic.Notification`1<!T> current, object gate)  cil managed 
    {
        // Method begins at RVA 0x8abe
	// Code size 9 (0x9)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  ldarg.2 
	IL_0003:  newobj instance void class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, object)
	IL_0008:  ret 
    } // end of method LinkedList::Create

	.property instance class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!T> Next ()
	{
		.get instance default class System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList<!0> System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList::get_Next () 
	}
	.property instance class [System.CoreEx]System.Collections.Generic.Notification`1<!T> Current ()
	{
		.get instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!0> System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList::get_Current () 
		.set instance default void System.Linq.EnumerableEx/MemoizeEnumerable`1/LinkedList::set_Current (class [System.CoreEx]System.Collections.Generic.Notification`1<!0> 'value') 
	}
  } // end of class LinkedList

  } // end of class MemoizeEnumerable`1

  .class nested private auto ansi sealed beforefieldinit '<DoHelper>d__4`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [mscorlib]System.Action`1<!0> onNext
    .field  public  class [mscorlib]System.Action`1<!0> '<>3__onNext'
    .field  public  class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError
    .field  public  class [mscorlib]System.Action`1<class [mscorlib]System.Exception> '<>3__onError'
    .field  public  class [System.Core]System.Action onCompleted
    .field  public  class [System.Core]System.Action '<>3__onCompleted'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<e>5__5'
    .field  public  !0 '<current>5__6'

    // method line 304
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x22b8
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 96 (0x60)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [mscorlib]System.Action`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>3__onNext'
	IL_0041:  stfld class [mscorlib]System.Action`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onNext
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Exception> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>3__onError'
	IL_004d:  stfld class [mscorlib]System.Action`1<class [mscorlib]System.Exception> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onError
	IL_0052:  ldloc.0 
	IL_0053:  ldarg.0 
	IL_0054:  ldfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>3__onCompleted'
	IL_0059:  stfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onCompleted
	IL_005e:  ldloc.0 
	IL_005f:  ret 
    } // end of method <DoHelper>d__4`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 305
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2324
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <DoHelper>d__4`1::System.Collections.IEnumerable.GetEnumerator

    // method line 306
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x232c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 199 (0xc7)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Exception	V_0,
		bool	V_1,
		int32	V_2)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	  IL_0006:  stloc.2 
	  IL_0007:  ldloc.2 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0017

	  IL_000b:  ldloc.2 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq IL_00a0

	  IL_0012:  br IL_00ba

	  IL_0017:  ldarg.0 
	  IL_0018:  ldc.i4.m1 
	  IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	  IL_001e:  ldarg.0 
	  IL_001f:  ldarg.0 
	  IL_0020:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::source
	  IL_0025:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_002a:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<e>5__5'
	  IL_002f:  ldarg.0 
	  IL_0030:  ldc.i4.1 
	  IL_0031:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	  IL_0036:  ldarg.0 
	  IL_0037:  ldflda !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<current>5__6'
	  IL_003c:  initobj !0
	  .try { // 0
	    IL_0042:  ldarg.0 
	    IL_0043:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<e>5__5'
	    IL_0048:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_004d:  brtrue.s IL_0051

	    IL_004f:  leave.s IL_00a9

	    IL_0051:  ldarg.0 
	    IL_0052:  ldarg.0 
	    IL_0053:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<e>5__5'
	    IL_0058:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	    IL_005d:  stfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<current>5__6'
	    IL_0062:  leave.s IL_0078

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_0064:  stloc.0 
	    IL_0065:  ldarg.0 
	    IL_0066:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Exception> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onError
	    IL_006b:  ldloc.0 
	    IL_006c:  callvirt instance void class [mscorlib]System.Action`1<class [mscorlib]System.Exception>::Invoke(!0)
	    IL_0071:  ldloc.0 
	    IL_0072:  call class [mscorlib]System.Exception class [System.CoreEx]System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	    IL_0077:  throw 
	  } // end handler 0
	  IL_0078:  ldarg.0 
	  IL_0079:  ldfld class [mscorlib]System.Action`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onNext
	  IL_007e:  ldarg.0 
	  IL_007f:  ldfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<current>5__6'
	  IL_0084:  callvirt instance void class [mscorlib]System.Action`1<!TSource>::Invoke(!0)
	  IL_0089:  ldarg.0 
	  IL_008a:  ldarg.0 
	  IL_008b:  ldfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<current>5__6'
	  IL_0090:  stfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>2__current'
	  IL_0095:  ldarg.0 
	  IL_0096:  ldc.i4.3 
	  IL_0097:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	  IL_009c:  ldc.i4.1 
	  IL_009d:  stloc.1 
	  IL_009e:  leave.s IL_00c5

	  IL_00a0:  ldarg.0 
	  IL_00a1:  ldc.i4.1 
	  IL_00a2:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	  IL_00a7:  br.s IL_0036

	  IL_00a9:  ldarg.0 
	  IL_00aa:  ldfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::onCompleted
	  IL_00af:  callvirt instance void class [System.Core]System.Action::Invoke()
	  IL_00b4:  ldarg.0 
	  IL_00b5:  call instance void class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>::'<>m__Finally7'()
	  IL_00ba:  ldc.i4.0 
	  IL_00bb:  stloc.1 
	  IL_00bc:  leave.s IL_00c5

	} // end .try 1
	fault  { // 1
	  IL_00be:  ldarg.0 
	  IL_00bf:  call instance void class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00c4:  endfault
	} // end handler 1
	IL_00c5:  ldloc.1 
	IL_00c6:  ret 
    } // end of method <DoHelper>d__4`1::MoveNext

    // method line 307
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x241c
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <DoHelper>d__4`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 308
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2424
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <DoHelper>d__4`1::System.Collections.IEnumerator.Reset

    // method line 309
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x242c
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Exception	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	IL_0006:  stloc.1 
	IL_0007:  ldloc.1 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_0025,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!TSource>::'<>m__Finally7'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <DoHelper>d__4`1::System.IDisposable.Dispose

    // method line 310
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2470
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <DoHelper>d__4`1::System.Collections.IEnumerator.get_Current

    // method line 311
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x247d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <DoHelper>d__4`1::.ctor

    // method line 312
    .method private hidebysig 
           instance default void '<>m__Finally7' ()  cil managed 
    {
        // Method begins at RVA 0x249c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<e>5__5'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DoHelper>d__4`1'<!0>::'<e>5__5'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <DoHelper>d__4`1::<>m__Finally7

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<DoHelper>d__4`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<DoHelper>d__4`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <DoHelper>d__4`1

  .class nested private auto ansi sealed beforefieldinit '<FinallyHelper>d__a`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [System.Core]System.Action finallyAction
    .field  public  class [System.Core]System.Action '<>3__finallyAction'
    .field  public  !0 '<item>5__b'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrapd'

    // method line 313
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2510
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>3__finallyAction'
	IL_0041:  stfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::finallyAction
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <FinallyHelper>d__a`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 314
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2564
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <FinallyHelper>d__a`1::System.Collections.IEnumerable.GetEnumerator

    // method line 315
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x256c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 142 (0x8e)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0011

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq.s IL_0061

	  IL_000f:  br.s IL_0081

	  IL_0011:  ldarg.0 
	  IL_0012:  ldc.i4.m1 
	  IL_0013:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_0018:  ldarg.0 
	  IL_0019:  ldc.i4.1 
	  IL_001a:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_001f:  ldarg.0 
	  IL_0020:  ldarg.0 
	  IL_0021:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::source
	  IL_0026:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_002b:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>7__wrapd'
	  IL_0030:  ldarg.0 
	  IL_0031:  ldc.i4.2 
	  IL_0032:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_0037:  br.s IL_0068

	  IL_0039:  ldarg.0 
	  IL_003a:  ldarg.0 
	  IL_003b:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>7__wrapd'
	  IL_0040:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0045:  stfld !0 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<item>5__b'
	  IL_004a:  ldarg.0 
	  IL_004b:  ldarg.0 
	  IL_004c:  ldfld !0 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<item>5__b'
	  IL_0051:  stfld !0 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>2__current'
	  IL_0056:  ldarg.0 
	  IL_0057:  ldc.i4.3 
	  IL_0058:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_005d:  ldc.i4.1 
	  IL_005e:  stloc.0 
	  IL_005f:  leave.s IL_008c

	  IL_0061:  ldarg.0 
	  IL_0062:  ldc.i4.2 
	  IL_0063:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_0068:  ldarg.0 
	  IL_0069:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>7__wrapd'
	  IL_006e:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0073:  brtrue.s IL_0039

	  IL_0075:  ldarg.0 
	  IL_0076:  call instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'<>m__Finallye'()
	  IL_007b:  ldarg.0 
	  IL_007c:  call instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'<>m__Finallyc'()
	  IL_0081:  ldc.i4.0 
	  IL_0082:  stloc.0 
	  IL_0083:  leave.s IL_008c

	} // end .try 0
	fault  { // 0
	  IL_0085:  ldarg.0 
	  IL_0086:  call instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::System.IDisposable.Dispose()
	  IL_008b:  endfault
	} // end handler 0
	IL_008c:  ldloc.0 
	IL_008d:  ret 
    } // end of method <FinallyHelper>d__a`1::MoveNext

    // method line 316
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2618
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <FinallyHelper>d__a`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 317
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2620
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <FinallyHelper>d__a`1::System.Collections.IEnumerator.Reset

    // method line 318
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2628
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 1
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	  IL_0022:  stloc.1 
	  IL_0023:  ldloc.1 
	  IL_0024:  ldc.i4.2 
	  IL_0025:  sub 
	  IL_0026:  switch (
	    IL_0035,
	    IL_0035)
	  IL_0033:  br.s IL_003e

	  .try { // 0
	    IL_0035:  leave.s IL_003e

	  } // end .try 0
	  finally  { // 0
	    IL_0037:  ldarg.0 
	    IL_0038:  call instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'<>m__Finallye'()
	    IL_003d:  endfinally 
	  } // end handler 0
	  IL_003e:  leave.s IL_0047

	} // end .try 1
	finally  { // 1
	  IL_0040:  ldarg.0 
	  IL_0041:  call instance void class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!TSource>::'<>m__Finallyc'()
	  IL_0046:  endfinally 
	} // end handler 1
	IL_0047:  ret 
    } // end of method <FinallyHelper>d__a`1::System.IDisposable.Dispose

    // method line 319
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2698
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <FinallyHelper>d__a`1::System.Collections.IEnumerator.get_Current

    // method line 320
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x26a5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <FinallyHelper>d__a`1::.ctor

    // method line 321
    .method private hidebysig 
           instance default void '<>m__Finallyc' ()  cil managed 
    {
        // Method begins at RVA 0x26c4
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [System.Core]System.Action class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::finallyAction
	IL_000d:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0012:  ret 
    } // end of method <FinallyHelper>d__a`1::<>m__Finallyc

    // method line 322
    .method private hidebysig 
           instance default void '<>m__Finallye' ()  cil managed 
    {
        // Method begins at RVA 0x26d8
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>7__wrapd'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'<!0>::'<>7__wrapd'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <FinallyHelper>d__a`1::<>m__Finallye

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<FinallyHelper>d__a`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <FinallyHelper>d__a`1

  .class nested private auto ansi sealed beforefieldinit '<BufferHelper>d__11`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!0>>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!0>>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  class [mscorlib]System.Collections.Generic.IList`1<!0> '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  int32 count
    .field  public  int32 '<>3__count'
    .field  public  int32 skip
    .field  public  int32 '<>3__skip'
    .field  public  class [mscorlib]System.Collections.Generic.List`1<!0> '<list>5__12'
    .field  public  int32 '<n>5__13'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<x>5__14'
    .field  public  class [mscorlib]System.Collections.Generic.List`1<!0> '<result>5__15'
    .field  public  class [mscorlib]System.Collections.Generic.List`1<!0> '<result>5__16'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<>7__wrap17'

    // method line 323
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>> 'System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2750
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>3__count'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::count
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>3__skip'
	IL_004d:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::skip
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <BufferHelper>d__11`1::System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator

    // method line 324
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x27b0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::'System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <BufferHelper>d__11`1::System.Collections.IEnumerable.GetEnumerator

    // method line 325
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x27b8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 440 (0x1b8)
	.maxstack 4
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_0022,
	    IL_01ab,
	    IL_0128,
	    IL_0167)
	  IL_001d:  br IL_01ab

	  IL_0022:  ldarg.0 
	  IL_0023:  ldc.i4.m1 
	  IL_0024:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_0029:  ldarg.0 
	  IL_002a:  newobj instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::'.ctor'()
	  IL_002f:  stfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_0034:  ldarg.0 
	  IL_0035:  ldc.i4.0 
	  IL_0036:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<n>5__13'
	  IL_003b:  ldarg.0 
	  IL_003c:  ldarg.0 
	  IL_003d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::source
	  IL_0042:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>> class System.Linq.EnumerableEx::Materialize<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	  IL_0047:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::GetEnumerator()
	  IL_004c:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>7__wrap17'
	  IL_0051:  ldarg.0 
	  IL_0052:  ldc.i4.1 
	  IL_0053:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_0058:  br IL_0195

	  IL_005d:  ldarg.0 
	  IL_005e:  ldarg.0 
	  IL_005f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>7__wrap17'
	  IL_0064:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::get_Current()
	  IL_0069:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<x>5__14'
	  IL_006e:  ldarg.0 
	  IL_006f:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<x>5__14'
	  IL_0074:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Kind()
	  IL_0079:  brtrue IL_0131

	  IL_007e:  ldarg.0 
	  IL_007f:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<n>5__13'
	  IL_0084:  brtrue.s IL_009e

	  IL_0086:  ldarg.0 
	  IL_0087:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_008c:  ldarg.0 
	  IL_008d:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<x>5__14'
	  IL_0092:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Value()
	  IL_0097:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::Add(!0)
	  IL_009c:  br.s IL_00ac

	  IL_009e:  ldarg.0 
	  IL_009f:  dup 
	  IL_00a0:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<n>5__13'
	  IL_00a5:  ldc.i4.1 
	  IL_00a6:  sub 
	  IL_00a7:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<n>5__13'
	  IL_00ac:  ldarg.0 
	  IL_00ad:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_00b2:  callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<!TSource>::get_Count()
	  IL_00b7:  ldarg.0 
	  IL_00b8:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::count
	  IL_00bd:  bne.un IL_0195

	  IL_00c2:  ldarg.0 
	  IL_00c3:  ldarg.0 
	  IL_00c4:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_00c9:  call class [mscorlib]System.Collections.Generic.List`1<!!0> class [System.Core]System.Linq.Enumerable::ToList<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	  IL_00ce:  stfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<result>5__15'
	  IL_00d3:  ldarg.0 
	  IL_00d4:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_00d9:  ldc.i4.0 
	  IL_00da:  ldarg.0 
	  IL_00db:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::skip
	  IL_00e0:  ldarg.0 
	  IL_00e1:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_00e6:  callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<!TSource>::get_Count()
	  IL_00eb:  call int32 class [mscorlib]System.Math::Min(int32, int32)
	  IL_00f0:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::RemoveRange(int32, int32)
	  IL_00f5:  ldarg.0 
	  IL_00f6:  ldc.i4.0 
	  IL_00f7:  ldarg.0 
	  IL_00f8:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::skip
	  IL_00fd:  ldarg.0 
	  IL_00fe:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::count
	  IL_0103:  sub 
	  IL_0104:  call int32 class [mscorlib]System.Math::Max(int32, int32)
	  IL_0109:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<n>5__13'
	  IL_010e:  ldarg.0 
	  IL_010f:  ldarg.0 
	  IL_0110:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<result>5__15'
	  IL_0115:  stfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>2__current'
	  IL_011a:  ldarg.0 
	  IL_011b:  ldc.i4.2 
	  IL_011c:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_0121:  ldc.i4.1 
	  IL_0122:  stloc.0 
	  IL_0123:  leave IL_01b6

	  IL_0128:  ldarg.0 
	  IL_0129:  ldc.i4.1 
	  IL_012a:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_012f:  br.s IL_0195

	  IL_0131:  ldarg.0 
	  IL_0132:  ldarg.0 
	  IL_0133:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<list>5__12'
	  IL_0138:  call class [mscorlib]System.Collections.Generic.List`1<!!0> class [System.Core]System.Linq.Enumerable::ToList<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	  IL_013d:  stfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<result>5__16'
	  IL_0142:  ldarg.0 
	  IL_0143:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<result>5__16'
	  IL_0148:  callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<!TSource>::get_Count()
	  IL_014d:  ldc.i4.0 
	  IL_014e:  ble.s IL_016e

	  IL_0150:  ldarg.0 
	  IL_0151:  ldarg.0 
	  IL_0152:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<result>5__16'
	  IL_0157:  stfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>2__current'
	  IL_015c:  ldarg.0 
	  IL_015d:  ldc.i4.3 
	  IL_015e:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_0163:  ldc.i4.1 
	  IL_0164:  stloc.0 
	  IL_0165:  leave.s IL_01b6

	  IL_0167:  ldarg.0 
	  IL_0168:  ldc.i4.1 
	  IL_0169:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	  IL_016e:  ldarg.0 
	  IL_016f:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<x>5__14'
	  IL_0174:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Kind()
	  IL_0179:  ldc.i4.1 
	  IL_017a:  bne.un.s IL_018d

	  IL_017c:  ldarg.0 
	  IL_017d:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<x>5__14'
	  IL_0182:  castclass class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0>
	  IL_0187:  callvirt instance class [mscorlib]System.Exception class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Exception()
	  IL_018c:  throw 
	  IL_018d:  ldarg.0 
	  IL_018e:  call instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0193:  leave.s IL_01ab

	  IL_0195:  ldarg.0 
	  IL_0196:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>7__wrap17'
	  IL_019b:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_01a0:  brtrue IL_005d

	  IL_01a5:  ldarg.0 
	  IL_01a6:  call instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::'<>m__Finally18'()
	  IL_01ab:  ldc.i4.0 
	  IL_01ac:  stloc.0 
	  IL_01ad:  leave.s IL_01b6

	} // end .try 0
	fault  { // 0
	  IL_01af:  ldarg.0 
	  IL_01b0:  call instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::System.IDisposable.Dispose()
	  IL_01b5:  endfault
	} // end handler 0
	IL_01b6:  ldloc.0 
	IL_01b7:  ret 
    } // end of method <BufferHelper>d__11`1::MoveNext

    // method line 326
    .method private final virtual hidebysig newslot specialname 
           instance default class [mscorlib]System.Collections.Generic.IList`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2998
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <BufferHelper>d__11`1::System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current

    // method line 327
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x29a0
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <BufferHelper>d__11`1::System.Collections.IEnumerator.Reset

    // method line 328
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x29a8
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!TSource>::'<>m__Finally18'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <BufferHelper>d__11`1::System.IDisposable.Dispose

    // method line 329
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x29ec
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <BufferHelper>d__11`1::System.Collections.IEnumerator.get_Current

    // method line 330
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x29f4
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <BufferHelper>d__11`1::.ctor

    // method line 331
    .method private hidebysig 
           instance default void '<>m__Finally18' ()  cil managed 
    {
        // Method begins at RVA 0x2a13
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>7__wrap17'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<BufferHelper>d__11`1'<!0>::'<>7__wrap17'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <BufferHelper>d__11`1::<>m__Finally18

	.property instance class [mscorlib]System.Collections.Generic.IList`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.Current' ()
	{
		.get instance default class [mscorlib]System.Collections.Generic.IList`1<!0> System.Linq.EnumerableEx/'<BufferHelper>d__11`1'::'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<BufferHelper>d__11`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <BufferHelper>d__11`1

  .class nested private auto ansi sealed beforefieldinit '<MaterializeHelper>d__1b`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<enumerator>5__1c'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<result>5__1d'
    .field  public  bool '<hasNext>5__1e'
    .field  public  !0 '<value>5__1f'

    // method line 332
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>> 'System.Collections.Generic.IEnumerable<System.Collections.Generic.Notification<TSource>>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2ab0
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <MaterializeHelper>d__1b`1::System.Collections.Generic.IEnumerable<System.Collections.Generic.Notification<TSource>>.GetEnumerator

    // method line 333
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2af8
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::'System.Collections.Generic.IEnumerable<System.Collections.Generic.Notification<TSource>>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <MaterializeHelper>d__1b`1::System.Collections.IEnumerable.GetEnumerator

    // method line 334
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x2b00
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 291 (0x123)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Exception	V_0,
		bool	V_1,
		int32	V_2)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_0006:  stloc.2 
	  IL_0007:  ldloc.2 
	  IL_0008:  switch (
	    IL_002a,
	    IL_0116,
	    IL_0116,
	    IL_00a1,
	    IL_00e5,
	    IL_0107)
	  IL_0025:  br IL_0116

	  IL_002a:  ldarg.0 
	  IL_002b:  ldc.i4.m1 
	  IL_002c:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_0031:  ldarg.0 
	  IL_0032:  ldarg.0 
	  IL_0033:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::source
	  IL_0038:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_003d:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<enumerator>5__1c'
	  IL_0042:  ldarg.0 
	  IL_0043:  ldc.i4.1 
	  IL_0044:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_0049:  ldarg.0 
	  IL_004a:  ldnull 
	  IL_004b:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<result>5__1d'
	  IL_0050:  ldarg.0 
	  IL_0051:  ldc.i4.0 
	  IL_0052:  stfld bool class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<hasNext>5__1e'
	  .try { // 0
	    IL_0057:  ldarg.0 
	    IL_0058:  ldarg.0 
	    IL_0059:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<enumerator>5__1c'
	    IL_005e:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_0063:  stfld bool class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<hasNext>5__1e'
	    IL_0068:  leave.s IL_0079

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_006a:  stloc.0 
	    IL_006b:  ldarg.0 
	    IL_006c:  ldloc.0 
	    IL_006d:  newobj instance void class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!TSource>::'.ctor'(class [mscorlib]System.Exception)
	    IL_0072:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<result>5__1d'
	    IL_0077:  leave.s IL_0079

	  } // end handler 0
	  IL_0079:  ldarg.0 
	  IL_007a:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<result>5__1d'
	  IL_007f:  ldnull 
	  IL_0080:  call bool class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::op_Inequality(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	  IL_0085:  brfalse.s IL_00b0

	  IL_0087:  ldarg.0 
	  IL_0088:  ldarg.0 
	  IL_0089:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<result>5__1d'
	  IL_008e:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>2__current'
	  IL_0093:  ldarg.0 
	  IL_0094:  ldc.i4.3 
	  IL_0095:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_009a:  ldc.i4.1 
	  IL_009b:  stloc.1 
	  IL_009c:  leave IL_0121

	  IL_00a1:  ldarg.0 
	  IL_00a2:  ldc.i4.1 
	  IL_00a3:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_00a8:  ldarg.0 
	  IL_00a9:  call instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00ae:  leave.s IL_0116

	  IL_00b0:  ldarg.0 
	  IL_00b1:  ldfld bool class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<hasNext>5__1e'
	  IL_00b6:  brfalse.s IL_00f1

	  IL_00b8:  ldarg.0 
	  IL_00b9:  ldarg.0 
	  IL_00ba:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<enumerator>5__1c'
	  IL_00bf:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_00c4:  stfld !0 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<value>5__1f'
	  IL_00c9:  ldarg.0 
	  IL_00ca:  ldarg.0 
	  IL_00cb:  ldfld !0 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<value>5__1f'
	  IL_00d0:  newobj instance void class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!TSource>::'.ctor'(!0)
	  IL_00d5:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>2__current'
	  IL_00da:  ldarg.0 
	  IL_00db:  ldc.i4.4 
	  IL_00dc:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_00e1:  ldc.i4.1 
	  IL_00e2:  stloc.1 
	  IL_00e3:  leave.s IL_0121

	  IL_00e5:  ldarg.0 
	  IL_00e6:  ldc.i4.1 
	  IL_00e7:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_00ec:  br IL_0049

	  IL_00f1:  ldarg.0 
	  IL_00f2:  newobj instance void class [System.CoreEx]System.Collections.Generic.Notification`1/OnCompleted<!TSource>::'.ctor'()
	  IL_00f7:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>2__current'
	  IL_00fc:  ldarg.0 
	  IL_00fd:  ldc.i4.5 
	  IL_00fe:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_0103:  ldc.i4.1 
	  IL_0104:  stloc.1 
	  IL_0105:  leave.s IL_0121

	  IL_0107:  ldarg.0 
	  IL_0108:  ldc.i4.1 
	  IL_0109:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	  IL_010e:  ldarg.0 
	  IL_010f:  call instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0114:  leave.s IL_0116

	  IL_0116:  ldc.i4.0 
	  IL_0117:  stloc.1 
	  IL_0118:  leave.s IL_0121

	} // end .try 1
	fault  { // 1
	  IL_011a:  ldarg.0 
	  IL_011b:  call instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0120:  endfault
	} // end handler 1
	IL_0121:  ldloc.1 
	IL_0122:  ret 
    } // end of method <MaterializeHelper>d__1b`1::MoveNext

    // method line 335
    .method private final virtual hidebysig newslot specialname 
           instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.Notification<TSource>>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2c64
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <MaterializeHelper>d__1b`1::System.Collections.Generic.IEnumerator<System.Collections.Generic.Notification<TSource>>.get_Current

    // method line 336
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2c6c
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <MaterializeHelper>d__1b`1::System.Collections.IEnumerator.Reset

    // method line 337
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2c74
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 46 (0x2e)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Exception	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	IL_0006:  stloc.1 
	IL_0007:  ldloc.1 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0024,
	  IL_002d,
	  IL_0024,
	  IL_0024,
	  IL_0024)
	IL_0023:  ret 
	.try { // 0
	  IL_0024:  leave.s IL_002d

	} // end .try 0
	finally  { // 0
	  IL_0026:  ldarg.0 
	  IL_0027:  call instance void class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!TSource>::'<>m__Finally20'()
	  IL_002c:  endfinally 
	} // end handler 0
	IL_002d:  ret 
    } // end of method <MaterializeHelper>d__1b`1::System.IDisposable.Dispose

    // method line 338
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2cc0
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <MaterializeHelper>d__1b`1::System.Collections.IEnumerator.get_Current

    // method line 339
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2cc8
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <MaterializeHelper>d__1b`1::.ctor

    // method line 340
    .method private hidebysig 
           instance default void '<>m__Finally20' ()  cil managed 
    {
        // Method begins at RVA 0x2ce7
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'<!0>::'<enumerator>5__1c'
	IL_000d:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0012:  ret 
    } // end of method <MaterializeHelper>d__1b`1::<>m__Finally20

	.property instance class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.Notification<TSource>>.Current' ()
	{
		.get instance default class [System.CoreEx]System.Collections.Generic.Notification`1<!0> System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'::'System.Collections.Generic.IEnumerator<System.Collections.Generic.Notification<TSource>>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<MaterializeHelper>d__1b`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <MaterializeHelper>d__1b`1

  .class nested private auto ansi sealed beforefieldinit '<DematerializeHelper>d__23`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<>3__source'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<item>5__24'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> '<errorNotification>5__25'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> '<nextNotification>5__26'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<>7__wrap27'

    // method line 341
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2d30
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <DematerializeHelper>d__23`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 342
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2d78
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <DematerializeHelper>d__23`1::System.Collections.IEnumerable.GetEnumerator

    // method line 343
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x2d80
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 259 (0x103)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_001e,
	    IL_00f6,
	    IL_00c3)
	  IL_0019:  br IL_00f6

	  IL_001e:  ldarg.0 
	  IL_001f:  ldc.i4.m1 
	  IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	  IL_0025:  ldarg.0 
	  IL_0026:  ldarg.0 
	  IL_0027:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::source
	  IL_002c:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::GetEnumerator()
	  IL_0031:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>7__wrap27'
	  IL_0036:  ldarg.0 
	  IL_0037:  ldc.i4.1 
	  IL_0038:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	  IL_003d:  br IL_00e0

	  IL_0042:  ldarg.0 
	  IL_0043:  ldarg.0 
	  IL_0044:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>7__wrap27'
	  IL_0049:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::get_Current()
	  IL_004e:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<item>5__24'
	  IL_0053:  ldarg.0 
	  IL_0054:  ldarg.0 
	  IL_0055:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<item>5__24'
	  IL_005a:  isinst class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0>
	  IL_005f:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<errorNotification>5__25'
	  IL_0064:  ldarg.0 
	  IL_0065:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<errorNotification>5__25'
	  IL_006a:  ldnull 
	  IL_006b:  call bool class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::op_Inequality(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	  IL_0070:  brfalse.s IL_0088

	  IL_0072:  ldarg.0 
	  IL_0073:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<errorNotification>5__25'
	  IL_0078:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Value()
	  IL_007d:  box !0
	  IL_0082:  castclass [mscorlib]System.Exception
	  IL_0087:  throw 
	  IL_0088:  ldarg.0 
	  IL_0089:  ldarg.0 
	  IL_008a:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<item>5__24'
	  IL_008f:  isinst class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0>
	  IL_0094:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<nextNotification>5__26'
	  IL_0099:  ldarg.0 
	  IL_009a:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<nextNotification>5__26'
	  IL_009f:  ldnull 
	  IL_00a0:  call bool class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::op_Inequality(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	  IL_00a5:  brfalse.s IL_00cc

	  IL_00a7:  ldarg.0 
	  IL_00a8:  ldarg.0 
	  IL_00a9:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<nextNotification>5__26'
	  IL_00ae:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Value()
	  IL_00b3:  stfld !0 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>2__current'
	  IL_00b8:  ldarg.0 
	  IL_00b9:  ldc.i4.2 
	  IL_00ba:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	  IL_00bf:  ldc.i4.1 
	  IL_00c0:  stloc.0 
	  IL_00c1:  leave.s IL_0101

	  IL_00c3:  ldarg.0 
	  IL_00c4:  ldc.i4.1 
	  IL_00c5:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	  IL_00ca:  br.s IL_00e0

	  IL_00cc:  ldarg.0 
	  IL_00cd:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<item>5__24'
	  IL_00d2:  castclass class [System.CoreEx]System.Collections.Generic.Notification`1/OnCompleted<!0>
	  IL_00d7:  pop 
	  IL_00d8:  ldarg.0 
	  IL_00d9:  call instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00de:  leave.s IL_00f6

	  IL_00e0:  ldarg.0 
	  IL_00e1:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>7__wrap27'
	  IL_00e6:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00eb:  brtrue IL_0042

	  IL_00f0:  ldarg.0 
	  IL_00f1:  call instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::'<>m__Finally28'()
	  IL_00f6:  ldc.i4.0 
	  IL_00f7:  stloc.0 
	  IL_00f8:  leave.s IL_0101

	} // end .try 0
	fault  { // 0
	  IL_00fa:  ldarg.0 
	  IL_00fb:  call instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0100:  endfault
	} // end handler 0
	IL_0101:  ldloc.0 
	IL_0102:  ret 
    } // end of method <DematerializeHelper>d__23`1::MoveNext

    // method line 344
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2ea0
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <DematerializeHelper>d__23`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 345
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2ea8
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <DematerializeHelper>d__23`1::System.Collections.IEnumerator.Reset

    // method line 346
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2eb0
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 34 (0x22)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0018,
	  IL_0018)
	IL_0017:  ret 
	.try { // 0
	  IL_0018:  leave.s IL_0021

	} // end .try 0
	finally  { // 0
	  IL_001a:  ldarg.0 
	  IL_001b:  call instance void class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!TSource>::'<>m__Finally28'()
	  IL_0020:  endfinally 
	} // end handler 0
	IL_0021:  ret 
    } // end of method <DematerializeHelper>d__23`1::System.IDisposable.Dispose

    // method line 347
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2ef0
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <DematerializeHelper>d__23`1::System.Collections.IEnumerator.get_Current

    // method line 348
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2efd
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <DematerializeHelper>d__23`1::.ctor

    // method line 349
    .method private hidebysig 
           instance default void '<>m__Finally28' ()  cil managed 
    {
        // Method begins at RVA 0x2f1c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>7__wrap27'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'<!0>::'<>7__wrap27'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <DematerializeHelper>d__23`1::<>m__Finally28

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<DematerializeHelper>d__23`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <DematerializeHelper>d__23`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass2d`2'<TSource,TAccumulate>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  !1 seed
    .field  public  class [System.Core]System.Func`3<!1,!0,!1> accumulator

    // method line 350
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2f55
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass2d`2::.ctor

    // method line 351
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TAccumulate> '<Scan>b__2b' ()  cil managed 
    {
        // Method begins at RVA 0x2f98
	// Code size 49 (0x31)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!TSource, !TAccumulate>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!TSource, !TAccumulate>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!0,!1>::'CS$<>8__locals2e'
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.0 
	IL_000f:  ldfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1>::seed
	IL_0014:  stfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!0,!1>::accumulation
	IL_0019:  ldarg.0 
	IL_001a:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1>::source
	IL_001f:  ldloc.0 
	IL_0020:  ldftn instance !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!TSource, !TAccumulate>::'<Scan>b__2c'(!0)
	IL_0026:  newobj instance void class [System.Core]System.Func`2<!TSource, !TAccumulate>::'.ctor'(object, native int)
	IL_002b:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<!0, !1> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0030:  ret 
    } // end of method <>c__DisplayClass2d`2::<Scan>b__2b

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass2f'<TSource,TAccumulate>
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1> 'CS$<>8__locals2e'
    .field  public  !1 accumulation

    // method line 352
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2f5d
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass2f::.ctor

    // method line 353
    .method public hidebysig 
           instance default !TAccumulate '<Scan>b__2c' (!TSource x)  cil managed 
    {
        // Method begins at RVA 0x2f68
	// Code size 33 (0x21)
	.maxstack 4
	.locals init (
		!TAccumulate	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.0 
	IL_0002:  ldfld class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!0,!1>::'CS$<>8__locals2e'
	IL_0007:  ldfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'<!0,!1>::accumulator
	IL_000c:  ldarg.0 
	IL_000d:  ldfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!0,!1>::accumulation
	IL_0012:  ldarg.1 
	IL_0013:  callvirt instance !2 class [System.Core]System.Func`3<!TAccumulate, !TSource, !TAccumulate>::Invoke(!0, !1)
	IL_0018:  dup 
	IL_0019:  stloc.0 
	IL_001a:  stfld !1 class System.Linq.EnumerableEx/'<>c__DisplayClass2d`2'/'<>c__DisplayClass2f'<!0,!1>::accumulation
	IL_001f:  ldloc.0 
	IL_0020:  ret 
    } // end of method <>c__DisplayClass2f::<Scan>b__2c

  } // end of class <>c__DisplayClass2f

  } // end of class <>c__DisplayClass2d`2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass33`1'<TSource>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [System.Core]System.Func`3<!0,!0,!0> accumulator

    // method line 354
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3047
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass33`1::.ctor

    // method line 355
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource> '<Scan>b__31' ()  cil managed 
    {
        // Method begins at RVA 0x3094
	// Code size 56 (0x38)
	.maxstack 4
	.locals init (
		class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!TSource>	V_0)
	IL_0000:  newobj instance void class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!TSource>::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::'CS$<>8__locals34'
	IL_000d:  ldloc.0 
	IL_000e:  ldflda !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::accumulation
	IL_0013:  initobj !0
	IL_0019:  ldloc.0 
	IL_001a:  ldc.i4.0 
	IL_001b:  stfld bool class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::hasAccumulation
	IL_0020:  ldarg.0 
	IL_0021:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!0>::source
	IL_0026:  ldloc.0 
	IL_0027:  ldftn instance !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!TSource>::'<Scan>b__32'(!0)
	IL_002d:  newobj instance void class [System.Core]System.Func`2<!TSource, !TSource>::'.ctor'(object, native int)
	IL_0032:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> class [System.Core]System.Linq.Enumerable::Select<!0, !0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Func`2<!!0,!!1>)
	IL_0037:  ret 
    } // end of method <>c__DisplayClass33`1::<Scan>b__31

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass35'<TSource>
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!0> 'CS$<>8__locals34'
    .field  public  !0 accumulation
    .field  public  bool hasAccumulation

    // method line 356
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x304f
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass35::.ctor

    // method line 357
    .method public hidebysig 
           instance default !TSource '<Scan>b__32' (!TSource x)  cil managed 
    {
        // Method begins at RVA 0x3057
	// Code size 60 (0x3c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld bool class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::hasAccumulation
	IL_0006:  brfalse.s IL_0027

	IL_0008:  ldarg.0 
	IL_0009:  ldarg.0 
	IL_000a:  ldfld class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::'CS$<>8__locals34'
	IL_000f:  ldfld class [System.Core]System.Func`3<!0,!0,!0> class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'<!0>::accumulator
	IL_0014:  ldarg.0 
	IL_0015:  ldfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::accumulation
	IL_001a:  ldarg.1 
	IL_001b:  callvirt instance !2 class [System.Core]System.Func`3<!TSource, !TSource, !TSource>::Invoke(!0, !1)
	IL_0020:  stfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::accumulation
	IL_0025:  br.s IL_0035

	IL_0027:  ldarg.0 
	IL_0028:  ldarg.1 
	IL_0029:  stfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::accumulation
	IL_002e:  ldarg.0 
	IL_002f:  ldc.i4.1 
	IL_0030:  stfld bool class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::hasAccumulation
	IL_0035:  ldarg.0 
	IL_0036:  ldfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass33`1'/'<>c__DisplayClass35'<!0>::accumulation
	IL_003b:  ret 
    } // end of method <>c__DisplayClass35::<Scan>b__32

  } // end of class <>c__DisplayClass35

  } // end of class <>c__DisplayClass33`1

  .class nested private auto ansi sealed beforefieldinit '<Scan0Helper>d__37`2'<TSource,TAccumulate>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!1>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !1 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  !1 seed
    .field  public  !1 '<>3__seed'
    .field  public  class [System.Core]System.Func`3<!1,!0,!1> accumulator
    .field  public  class [System.Core]System.Func`3<!1,!0,!1> '<>3__accumulator'
    .field  public  !1 '<currentValue>5__38'
    .field  public  bool '<atBegin>5__39'
    .field  public  !0 '<value>5__3a'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrap3b'

    // method line 358
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TAccumulate> 'System.Collections.Generic.IEnumerable<TAccumulate>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3158
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TAccumulate>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>3__seed'
	IL_0041:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::seed
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>3__accumulator'
	IL_004d:  stfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::accumulator
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <Scan0Helper>d__37`2::System.Collections.Generic.IEnumerable<TAccumulate>.GetEnumerator

    // method line 359
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x31b8
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>::'System.Collections.Generic.IEnumerable<TAccumulate>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Scan0Helper>d__37`2::System.Collections.IEnumerable.GetEnumerator

    // method line 360
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x31c0
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 287 (0x11f)
	.maxstack 4
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_0026,
	    IL_0112,
	    IL_008d,
	    IL_00cf,
	    IL_010b)
	  IL_0021:  br IL_0112

	  IL_0026:  ldarg.0 
	  IL_0027:  ldc.i4.m1 
	  IL_0028:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_002d:  ldarg.0 
	  IL_002e:  ldarg.0 
	  IL_002f:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::seed
	  IL_0034:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_0039:  ldarg.0 
	  IL_003a:  ldc.i4.1 
	  IL_003b:  stfld bool class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<atBegin>5__39'
	  IL_0040:  ldarg.0 
	  IL_0041:  ldarg.0 
	  IL_0042:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::source
	  IL_0047:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_004c:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>7__wrap3b'
	  IL_0051:  ldarg.0 
	  IL_0052:  ldc.i4.1 
	  IL_0053:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0058:  br.s IL_00d6

	  IL_005a:  ldarg.0 
	  IL_005b:  ldarg.0 
	  IL_005c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>7__wrap3b'
	  IL_0061:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0066:  stfld !0 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<value>5__3a'
	  IL_006b:  ldarg.0 
	  IL_006c:  ldfld bool class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<atBegin>5__39'
	  IL_0071:  brfalse.s IL_009b

	  IL_0073:  ldarg.0 
	  IL_0074:  ldarg.0 
	  IL_0075:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_007a:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>2__current'
	  IL_007f:  ldarg.0 
	  IL_0080:  ldc.i4.2 
	  IL_0081:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0086:  ldc.i4.1 
	  IL_0087:  stloc.0 
	  IL_0088:  leave IL_011d

	  IL_008d:  ldarg.0 
	  IL_008e:  ldc.i4.1 
	  IL_008f:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0094:  ldarg.0 
	  IL_0095:  ldc.i4.0 
	  IL_0096:  stfld bool class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<atBegin>5__39'
	  IL_009b:  ldarg.0 
	  IL_009c:  ldarg.0 
	  IL_009d:  ldfld class [System.Core]System.Func`3<!1,!0,!1> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::accumulator
	  IL_00a2:  ldarg.0 
	  IL_00a3:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_00a8:  ldarg.0 
	  IL_00a9:  ldfld !0 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<value>5__3a'
	  IL_00ae:  callvirt instance !2 class [System.Core]System.Func`3<!TAccumulate, !TSource, !TAccumulate>::Invoke(!0, !1)
	  IL_00b3:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_00b8:  ldarg.0 
	  IL_00b9:  ldarg.0 
	  IL_00ba:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_00bf:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>2__current'
	  IL_00c4:  ldarg.0 
	  IL_00c5:  ldc.i4.3 
	  IL_00c6:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_00cb:  ldc.i4.1 
	  IL_00cc:  stloc.0 
	  IL_00cd:  leave.s IL_011d

	  IL_00cf:  ldarg.0 
	  IL_00d0:  ldc.i4.1 
	  IL_00d1:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_00d6:  ldarg.0 
	  IL_00d7:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>7__wrap3b'
	  IL_00dc:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00e1:  brtrue IL_005a

	  IL_00e6:  ldarg.0 
	  IL_00e7:  call instance void class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>::'<>m__Finally3c'()
	  IL_00ec:  ldarg.0 
	  IL_00ed:  ldfld bool class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<atBegin>5__39'
	  IL_00f2:  brfalse.s IL_0112

	  IL_00f4:  ldarg.0 
	  IL_00f5:  ldarg.0 
	  IL_00f6:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<currentValue>5__38'
	  IL_00fb:  stfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>2__current'
	  IL_0100:  ldarg.0 
	  IL_0101:  ldc.i4.4 
	  IL_0102:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0107:  ldc.i4.1 
	  IL_0108:  stloc.0 
	  IL_0109:  leave.s IL_011d

	  IL_010b:  ldarg.0 
	  IL_010c:  ldc.i4.m1 
	  IL_010d:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	  IL_0112:  ldc.i4.0 
	  IL_0113:  stloc.0 
	  IL_0114:  leave.s IL_011d

	} // end .try 0
	fault  { // 0
	  IL_0116:  ldarg.0 
	  IL_0117:  call instance void class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>::System.IDisposable.Dispose()
	  IL_011c:  endfault
	} // end handler 0
	IL_011d:  ldloc.0 
	IL_011e:  ret 
    } // end of method <Scan0Helper>d__37`2::MoveNext

    // method line 361
    .method private final virtual hidebysig newslot specialname 
           instance default !TAccumulate 'System.Collections.Generic.IEnumerator<TAccumulate>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3308
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TAccumulate>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Scan0Helper>d__37`2::System.Collections.Generic.IEnumerator<TAccumulate>.get_Current

    // method line 362
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3310
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Scan0Helper>d__37`2::System.Collections.IEnumerator.Reset

    // method line 363
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3318
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!TSource, !TAccumulate>::'<>m__Finally3c'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <Scan0Helper>d__37`2::System.IDisposable.Dispose

    // method line 364
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x335c
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>2__current'
	IL_0006:  box !1
	IL_000b:  ret 
    } // end of method <Scan0Helper>d__37`2::System.Collections.IEnumerator.get_Current

    // method line 365
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3369
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Scan0Helper>d__37`2::.ctor

    // method line 366
    .method private hidebysig 
           instance default void '<>m__Finally3c' ()  cil managed 
    {
        // Method begins at RVA 0x3388
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>7__wrap3b'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'<!0,!1>::'<>7__wrap3b'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <Scan0Helper>d__37`2::<>m__Finally3c

	.property instance !TAccumulate 'System.Collections.Generic.IEnumerator<TAccumulate>.Current' ()
	{
		.get instance default !1 System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'::'System.Collections.Generic.IEnumerator<TAccumulate>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Scan0Helper>d__37`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Scan0Helper>d__37`2

  .class nested private auto ansi sealed beforefieldinit '<DistinctUntilChangedHelper>d__3f`2'<TSource,TKey>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [System.Core]System.Func`2<!0,!1> keySelector
    .field  public  class [System.Core]System.Func`2<!0,!1> '<>3__keySelector'
    .field  public  class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> comparer
    .field  public  class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> '<>3__comparer'
    .field  public  bool '<hasValue>5__40'
    .field  public  !1 '<lastKey>5__41'
    .field  public  !0 '<value>5__42'
    .field  public  !1 '<key>5__43'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrap44'

    // method line 367
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3404
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>3__keySelector'
	IL_0041:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::keySelector
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>3__comparer'
	IL_004d:  stfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::comparer
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 368
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3464
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.Collections.IEnumerable.GetEnumerator

    // method line 369
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x346c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 278 (0x116)
	.maxstack 3
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_0022,
	    IL_0109,
	    IL_00a7,
	    IL_00e0)
	  IL_001d:  br IL_0109

	  IL_0022:  ldarg.0 
	  IL_0023:  ldc.i4.m1 
	  IL_0024:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_0029:  ldarg.0 
	  IL_002a:  ldc.i4.0 
	  IL_002b:  stfld bool class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<hasValue>5__40'
	  IL_0030:  ldarg.0 
	  IL_0031:  ldflda !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<lastKey>5__41'
	  IL_0036:  initobj !1
	  IL_003c:  ldarg.0 
	  IL_003d:  ldarg.0 
	  IL_003e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::source
	  IL_0043:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_0048:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>7__wrap44'
	  IL_004d:  ldarg.0 
	  IL_004e:  ldc.i4.1 
	  IL_004f:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_0054:  br IL_00f3

	  IL_0059:  ldarg.0 
	  IL_005a:  ldarg.0 
	  IL_005b:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>7__wrap44'
	  IL_0060:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0065:  stfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<value>5__42'
	  IL_006a:  ldarg.0 
	  IL_006b:  ldarg.0 
	  IL_006c:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::keySelector
	  IL_0071:  ldarg.0 
	  IL_0072:  ldfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<value>5__42'
	  IL_0077:  callvirt instance !1 class [System.Core]System.Func`2<!TSource, !TKey>::Invoke(!0)
	  IL_007c:  stfld !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<key>5__43'
	  IL_0081:  ldarg.0 
	  IL_0082:  ldfld bool class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<hasValue>5__40'
	  IL_0087:  brtrue.s IL_00b0

	  IL_0089:  ldarg.0 
	  IL_008a:  ldc.i4.1 
	  IL_008b:  stfld bool class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<hasValue>5__40'
	  IL_0090:  ldarg.0 
	  IL_0091:  ldarg.0 
	  IL_0092:  ldfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<value>5__42'
	  IL_0097:  stfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>2__current'
	  IL_009c:  ldarg.0 
	  IL_009d:  ldc.i4.2 
	  IL_009e:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_00a3:  ldc.i4.1 
	  IL_00a4:  stloc.0 
	  IL_00a5:  leave.s IL_0114

	  IL_00a7:  ldarg.0 
	  IL_00a8:  ldc.i4.1 
	  IL_00a9:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_00ae:  br.s IL_00e7

	  IL_00b0:  ldarg.0 
	  IL_00b1:  ldfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!1> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::comparer
	  IL_00b6:  ldarg.0 
	  IL_00b7:  ldfld !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<lastKey>5__41'
	  IL_00bc:  ldarg.0 
	  IL_00bd:  ldfld !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<key>5__43'
	  IL_00c2:  callvirt instance bool class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!TKey>::Equals(!0, !0)
	  IL_00c7:  brtrue.s IL_00e7

	  IL_00c9:  ldarg.0 
	  IL_00ca:  ldarg.0 
	  IL_00cb:  ldfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<value>5__42'
	  IL_00d0:  stfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>2__current'
	  IL_00d5:  ldarg.0 
	  IL_00d6:  ldc.i4.3 
	  IL_00d7:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_00dc:  ldc.i4.1 
	  IL_00dd:  stloc.0 
	  IL_00de:  leave.s IL_0114

	  IL_00e0:  ldarg.0 
	  IL_00e1:  ldc.i4.1 
	  IL_00e2:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	  IL_00e7:  ldarg.0 
	  IL_00e8:  ldarg.0 
	  IL_00e9:  ldfld !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<key>5__43'
	  IL_00ee:  stfld !1 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<lastKey>5__41'
	  IL_00f3:  ldarg.0 
	  IL_00f4:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>7__wrap44'
	  IL_00f9:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00fe:  brtrue IL_0059

	  IL_0103:  ldarg.0 
	  IL_0104:  call instance void class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>::'<>m__Finally45'()
	  IL_0109:  ldc.i4.0 
	  IL_010a:  stloc.0 
	  IL_010b:  leave.s IL_0114

	} // end .try 0
	fault  { // 0
	  IL_010d:  ldarg.0 
	  IL_010e:  call instance void class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>::System.IDisposable.Dispose()
	  IL_0113:  endfault
	} // end handler 0
	IL_0114:  ldloc.0 
	IL_0115:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::MoveNext

    // method line 370
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x35ac
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 371
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x35b4
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.Collections.IEnumerator.Reset

    // method line 372
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x35bc
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!TSource, !TKey>::'<>m__Finally45'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.IDisposable.Dispose

    // method line 373
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3600
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::System.Collections.IEnumerator.get_Current

    // method line 374
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x360d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::.ctor

    // method line 375
    .method private hidebysig 
           instance default void '<>m__Finally45' ()  cil managed 
    {
        // Method begins at RVA 0x362c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>7__wrap44'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'<!0,!1>::'<>7__wrap44'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <DistinctUntilChangedHelper>d__3f`2::<>m__Finally45

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<DistinctUntilChangedHelper>d__3f`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <DistinctUntilChangedHelper>d__3f`2

  .class nested private auto ansi sealed beforefieldinit '<WhileCore>d__4a`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [System.Core]System.Func`1<bool> condition
    .field  public  class [System.Core]System.Func`1<bool> '<>3__condition'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'

    // method line 376
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>> 'System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<TSource>>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x36fc
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>3__condition'
	IL_0035:  stfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::condition
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>3__source'
	IL_0041:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::source
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <WhileCore>d__4a`1::System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<TSource>>.GetEnumerator

    // method line 377
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3750
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!TSource>::'System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<TSource>>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <WhileCore>d__4a`1::System.Collections.IEnumerable.GetEnumerator

    // method line 378
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x3758
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 75 (0x4b)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_0035)
	IL_0015:  br.s IL_0049

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_001e:  br.s IL_003c

	IL_0020:  ldarg.0 
	IL_0021:  ldarg.0 
	IL_0022:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::source
	IL_0027:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>2__current'
	IL_002c:  ldarg.0 
	IL_002d:  ldc.i4.1 
	IL_002e:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_0033:  ldc.i4.1 
	IL_0034:  ret 
	IL_0035:  ldarg.0 
	IL_0036:  ldc.i4.m1 
	IL_0037:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_003c:  ldarg.0 
	IL_003d:  ldfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::condition
	IL_0042:  callvirt instance !0 class [System.Core]System.Func`1<bool>::Invoke()
	IL_0047:  brtrue.s IL_0020

	IL_0049:  ldc.i4.0 
	IL_004a:  ret 
    } // end of method <WhileCore>d__4a`1::MoveNext

    // method line 379
    .method private final virtual hidebysig newslot specialname 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<TSource>>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x37af
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <WhileCore>d__4a`1::System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<TSource>>.get_Current

    // method line 380
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x37b7
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <WhileCore>d__4a`1::System.Collections.IEnumerator.Reset

    // method line 381
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x37be
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <WhileCore>d__4a`1::System.IDisposable.Dispose

    // method line 382
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x37c0
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <WhileCore>d__4a`1::System.Collections.IEnumerator.get_Current

    // method line 383
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x37c8
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<WhileCore>d__4a`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <WhileCore>d__4a`1::.ctor

	.property instance class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<TSource>>.Current' ()
	{
		.get instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> System.Linq.EnumerableEx/'<WhileCore>d__4a`1'::'System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<TSource>>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<WhileCore>d__4a`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <WhileCore>d__4a`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass4e`1'<TResult>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [System.Core]System.Func`1<bool> condition
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> thenSource
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> elseSource

    // method line 384
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3836
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass4e`1::.ctor

    // method line 385
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult> '<If>b__4d' ()  cil managed 
    {
        // Method begins at RVA 0x383e
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Func`1<bool> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!0>::condition
	IL_0006:  callvirt instance !0 class [System.Core]System.Func`1<bool>::Invoke()
	IL_000b:  brtrue.s IL_0014

	IL_000d:  ldarg.0 
	IL_000e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!0>::elseSource
	IL_0013:  ret 
	IL_0014:  ldarg.0 
	IL_0015:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass4e`1'<!0>::thenSource
	IL_001a:  ret 
    } // end of method <>c__DisplayClass4e`1::<If>b__4d

  } // end of class <>c__DisplayClass4e`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass51`2'<TValue,TResult>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [System.Core]System.Func`1<!0> selector
    .field  public  class [mscorlib]System.Collections.Generic.IDictionary`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> sources
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> defaultSource

    // method line 386
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x38f9
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass51`2::.ctor

    // method line 387
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult> '<Case>b__50' ()  cil managed 
    {
        // Method begins at RVA 0x3904
	// Code size 35 (0x23)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IDictionary`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!0,!1>::sources
	IL_0006:  ldarg.0 
	IL_0007:  ldfld class [System.Core]System.Func`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!0,!1>::selector
	IL_000c:  callvirt instance !0 class [System.Core]System.Func`1<!TValue>::Invoke()
	IL_0011:  ldloca.s 0
	IL_0013:  callvirt instance bool class [mscorlib]System.Collections.Generic.IDictionary`2<!TValue, class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>>::TryGetValue(!0, [out] !1&)
	IL_0018:  brtrue.s IL_0021

	IL_001a:  ldarg.0 
	IL_001b:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass51`2'<!0,!1>::defaultSource
	IL_0020:  stloc.0 
	IL_0021:  ldloc.0 
	IL_0022:  ret 
    } // end of method <>c__DisplayClass51`2::<Case>b__50

  } // end of class <>c__DisplayClass51`2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass54`2'<TValue,TResult>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  !0 'value'
    .field  public  class [System.Core]System.Func`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> selector

    // method line 388
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3a03
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass54`2::.ctor

    // method line 389
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult> '<Let>b__53' ()  cil managed 
    {
        // Method begins at RVA 0x3a0b
	// Code size 18 (0x12)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Func`2<!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>> class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!0,!1>::selector
	IL_0006:  ldarg.0 
	IL_0007:  ldfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClass54`2'<!0,!1>::'value'
	IL_000c:  callvirt instance !1 class [System.Core]System.Func`2<!TValue, class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>>::Invoke(!0)
	IL_0011:  ret 
    } // end of method <>c__DisplayClass54`2::<Let>b__53

  } // end of class <>c__DisplayClass54`2

  .class nested private auto ansi sealed beforefieldinit '<SkipLastHelper>d__56`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  int32 count
    .field  public  int32 '<>3__count'
    .field  public  class [System]System.Collections.Generic.Queue`1<!0> '<q>5__57'
    .field  public  !0 '<x>5__58'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrap59'

    // method line 390
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3a8c
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>3__count'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::count
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <SkipLastHelper>d__56`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 391
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3ae0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <SkipLastHelper>d__56`1::System.Collections.IEnumerable.GetEnumerator

    // method line 392
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x3ae8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 194 (0xc2)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_001e,
	    IL_00b5,
	    IL_009b)
	  IL_0019:  br IL_00b5

	  IL_001e:  ldarg.0 
	  IL_001f:  ldc.i4.m1 
	  IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	  IL_0025:  ldarg.0 
	  IL_0026:  newobj instance void class [System]System.Collections.Generic.Queue`1<!TSource>::'.ctor'()
	  IL_002b:  stfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<q>5__57'
	  IL_0030:  ldarg.0 
	  IL_0031:  ldarg.0 
	  IL_0032:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::source
	  IL_0037:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_003c:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>7__wrap59'
	  IL_0041:  ldarg.0 
	  IL_0042:  ldc.i4.1 
	  IL_0043:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	  IL_0048:  br.s IL_00a2

	  IL_004a:  ldarg.0 
	  IL_004b:  ldarg.0 
	  IL_004c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>7__wrap59'
	  IL_0051:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0056:  stfld !0 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<x>5__58'
	  IL_005b:  ldarg.0 
	  IL_005c:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<q>5__57'
	  IL_0061:  ldarg.0 
	  IL_0062:  ldfld !0 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<x>5__58'
	  IL_0067:  callvirt instance void class [System]System.Collections.Generic.Queue`1<!TSource>::Enqueue(!0)
	  IL_006c:  ldarg.0 
	  IL_006d:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<q>5__57'
	  IL_0072:  callvirt instance int32 class [System]System.Collections.Generic.Queue`1<!TSource>::get_Count()
	  IL_0077:  ldarg.0 
	  IL_0078:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::count
	  IL_007d:  ble.s IL_00a2

	  IL_007f:  ldarg.0 
	  IL_0080:  ldarg.0 
	  IL_0081:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<q>5__57'
	  IL_0086:  callvirt instance !0 class [System]System.Collections.Generic.Queue`1<!TSource>::Dequeue()
	  IL_008b:  stfld !0 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>2__current'
	  IL_0090:  ldarg.0 
	  IL_0091:  ldc.i4.2 
	  IL_0092:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	  IL_0097:  ldc.i4.1 
	  IL_0098:  stloc.0 
	  IL_0099:  leave.s IL_00c0

	  IL_009b:  ldarg.0 
	  IL_009c:  ldc.i4.1 
	  IL_009d:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	  IL_00a2:  ldarg.0 
	  IL_00a3:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>7__wrap59'
	  IL_00a8:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00ad:  brtrue.s IL_004a

	  IL_00af:  ldarg.0 
	  IL_00b0:  call instance void class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>::'<>m__Finally5a'()
	  IL_00b5:  ldc.i4.0 
	  IL_00b6:  stloc.0 
	  IL_00b7:  leave.s IL_00c0

	} // end .try 0
	fault  { // 0
	  IL_00b9:  ldarg.0 
	  IL_00ba:  call instance void class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00bf:  endfault
	} // end handler 0
	IL_00c0:  ldloc.0 
	IL_00c1:  ret 
    } // end of method <SkipLastHelper>d__56`1::MoveNext

    // method line 393
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3bc8
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <SkipLastHelper>d__56`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 394
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3bd0
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <SkipLastHelper>d__56`1::System.Collections.IEnumerator.Reset

    // method line 395
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3bd8
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 34 (0x22)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0018,
	  IL_0018)
	IL_0017:  ret 
	.try { // 0
	  IL_0018:  leave.s IL_0021

	} // end .try 0
	finally  { // 0
	  IL_001a:  ldarg.0 
	  IL_001b:  call instance void class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!TSource>::'<>m__Finally5a'()
	  IL_0020:  endfinally 
	} // end handler 0
	IL_0021:  ret 
    } // end of method <SkipLastHelper>d__56`1::System.IDisposable.Dispose

    // method line 396
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3c18
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <SkipLastHelper>d__56`1::System.Collections.IEnumerator.get_Current

    // method line 397
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3c25
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <SkipLastHelper>d__56`1::.ctor

    // method line 398
    .method private hidebysig 
           instance default void '<>m__Finally5a' ()  cil managed 
    {
        // Method begins at RVA 0x3c44
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>7__wrap59'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'<!0>::'<>7__wrap59'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <SkipLastHelper>d__56`1::<>m__Finally5a

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<SkipLastHelper>d__56`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <SkipLastHelper>d__56`1

  .class nested private auto ansi sealed beforefieldinit '<TakeLastHelper>d__5d`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  int32 count
    .field  public  int32 '<>3__count'
    .field  public  class [System]System.Collections.Generic.Queue`1<!0> '<q>5__5e'

    // method line 399
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3cac
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>3__count'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::count
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <TakeLastHelper>d__5d`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 400
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3d00
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <TakeLastHelper>d__5d`1::System.Collections.IEnumerable.GetEnumerator

    // method line 401
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x3d08
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 181 (0xb5)
	.maxstack 2
	.locals init (
		!TSource	V_0,
		int32	V_1,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_0006:  stloc.1 
	IL_0007:  ldloc.1 
	IL_0008:  switch (
	  IL_001e,
	  IL_00b3,
	  IL_009e)
	IL_0019:  br IL_00b3

	IL_001e:  ldarg.0 
	IL_001f:  ldc.i4.m1 
	IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_0025:  ldarg.0 
	IL_0026:  newobj instance void class [System]System.Collections.Generic.Queue`1<!TSource>::'.ctor'()
	IL_002b:  stfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	IL_0030:  ldarg.0 
	IL_0031:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::source
	IL_0036:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	IL_003b:  stloc.2 
	.try { // 0
	  IL_003c:  br.s IL_0070

	  IL_003e:  ldloc.2 
	  IL_003f:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0044:  stloc.0 
	  IL_0045:  ldarg.0 
	  IL_0046:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	  IL_004b:  ldloc.0 
	  IL_004c:  callvirt instance void class [System]System.Collections.Generic.Queue`1<!TSource>::Enqueue(!0)
	  IL_0051:  ldarg.0 
	  IL_0052:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	  IL_0057:  callvirt instance int32 class [System]System.Collections.Generic.Queue`1<!TSource>::get_Count()
	  IL_005c:  ldarg.0 
	  IL_005d:  ldfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::count
	  IL_0062:  ble.s IL_0070

	  IL_0064:  ldarg.0 
	  IL_0065:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	  IL_006a:  callvirt instance !0 class [System]System.Collections.Generic.Queue`1<!TSource>::Dequeue()
	  IL_006f:  pop 
	  IL_0070:  ldloc.2 
	  IL_0071:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0076:  brtrue.s IL_003e

	  IL_0078:  leave.s IL_00a5

	} // end .try 0
	finally  { // 0
	  IL_007a:  ldloc.2 
	  IL_007b:  brfalse.s IL_0083

	  IL_007d:  ldloc.2 
	  IL_007e:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0083:  endfinally 
	} // end handler 0
	IL_0084:  ldarg.0 
	IL_0085:  ldarg.0 
	IL_0086:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	IL_008b:  callvirt instance !0 class [System]System.Collections.Generic.Queue`1<!TSource>::Dequeue()
	IL_0090:  stfld !0 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>2__current'
	IL_0095:  ldarg.0 
	IL_0096:  ldc.i4.2 
	IL_0097:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_009c:  ldc.i4.1 
	IL_009d:  ret 
	IL_009e:  ldarg.0 
	IL_009f:  ldc.i4.m1 
	IL_00a0:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_00a5:  ldarg.0 
	IL_00a6:  ldfld class [System]System.Collections.Generic.Queue`1<!0> class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<q>5__5e'
	IL_00ab:  callvirt instance int32 class [System]System.Collections.Generic.Queue`1<!TSource>::get_Count()
	IL_00b0:  ldc.i4.0 
	IL_00b1:  bgt.s IL_0084

	IL_00b3:  ldc.i4.0 
	IL_00b4:  ret 
    } // end of method <TakeLastHelper>d__5d`1::MoveNext

    // method line 402
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3ddc
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <TakeLastHelper>d__5d`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 403
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3de4
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <TakeLastHelper>d__5d`1::System.Collections.IEnumerator.Reset

    // method line 404
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3dec
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 0
	.locals init (
		!TSource	V_0)
	IL_0000:  ret 
    } // end of method <TakeLastHelper>d__5d`1::System.IDisposable.Dispose

    // method line 405
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3df9
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <TakeLastHelper>d__5d`1::System.Collections.IEnumerator.get_Current

    // method line 406
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3e06
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <TakeLastHelper>d__5d`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<TakeLastHelper>d__5d`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <TakeLastHelper>d__5d`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass62`2'<TSource,TOther>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> other

    // method line 407
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3e67
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass62`2::.ctor

    // method line 408
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TOther> '<SelectMany>b__61' (!TSource _)  cil managed 
    {
        // Method begins at RVA 0x3e6f
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass62`2'<!0,!1>::other
	IL_0006:  ret 
    } // end of method <>c__DisplayClass62`2::<SelectMany>b__61

  } // end of class <>c__DisplayClass62`2

  .class nested private auto ansi sealed beforefieldinit '<OnErrorResumeNextHelper>d__64`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> sources
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>3__sources'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<outerEnumerator>5__65'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<innerEnumerator>5__66'
    .field  public  !0 '<value>5__67'

    // method line 409
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3f24
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>3__sources'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::sources
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 410
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3f6c
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.Collections.IEnumerable.GetEnumerator

    // method line 411
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x3f74
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 209 (0xd1)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0017

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.4 
	  IL_000d:  beq IL_00b0

	  IL_0012:  br IL_00c4

	  IL_0017:  ldarg.0 
	  IL_0018:  ldc.i4.m1 
	  IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_001e:  ldarg.0 
	  IL_001f:  ldarg.0 
	  IL_0020:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::sources
	  IL_0025:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::GetEnumerator()
	  IL_002a:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<outerEnumerator>5__65'
	  IL_002f:  ldarg.0 
	  IL_0030:  ldc.i4.1 
	  IL_0031:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_0036:  ldarg.0 
	  IL_0037:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<outerEnumerator>5__65'
	  IL_003c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0041:  brtrue.s IL_004b

	  IL_0043:  ldarg.0 
	  IL_0044:  call instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0049:  leave.s IL_00c4

	  IL_004b:  ldarg.0 
	  IL_004c:  ldarg.0 
	  IL_004d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<outerEnumerator>5__65'
	  IL_0052:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::get_Current()
	  IL_0057:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_005c:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<innerEnumerator>5__66'
	  IL_0061:  ldarg.0 
	  IL_0062:  ldc.i4.2 
	  IL_0063:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_0068:  ldarg.0 
	  IL_0069:  ldflda !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<value>5__67'
	  IL_006e:  initobj !0
	  .try { // 0
	    IL_0074:  ldarg.0 
	    IL_0075:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<innerEnumerator>5__66'
	    IL_007a:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_007f:  brtrue.s IL_0083

	    IL_0081:  leave.s IL_00b9

	    IL_0083:  ldarg.0 
	    IL_0084:  ldarg.0 
	    IL_0085:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<innerEnumerator>5__66'
	    IL_008a:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	    IL_008f:  stfld !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<value>5__67'
	    IL_0094:  leave.s IL_0099

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_0096:  pop 
	    IL_0097:  leave.s IL_00b9

	  } // end handler 0
	  IL_0099:  ldarg.0 
	  IL_009a:  ldarg.0 
	  IL_009b:  ldfld !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<value>5__67'
	  IL_00a0:  stfld !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>2__current'
	  IL_00a5:  ldarg.0 
	  IL_00a6:  ldc.i4.4 
	  IL_00a7:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_00ac:  ldc.i4.1 
	  IL_00ad:  stloc.0 
	  IL_00ae:  leave.s IL_00cf

	  IL_00b0:  ldarg.0 
	  IL_00b1:  ldc.i4.2 
	  IL_00b2:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_00b7:  br.s IL_0068

	  IL_00b9:  ldarg.0 
	  IL_00ba:  call instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::'<>m__Finally69'()
	  IL_00bf:  br IL_0036

	  IL_00c4:  ldc.i4.0 
	  IL_00c5:  stloc.0 
	  IL_00c6:  leave.s IL_00cf

	} // end .try 1
	fault  { // 1
	  IL_00c8:  ldarg.0 
	  IL_00c9:  call instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00ce:  endfault
	} // end handler 1
	IL_00cf:  ldloc.0 
	IL_00d0:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::MoveNext

    // method line 412
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4070
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 413
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4078
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.Collections.IEnumerator.Reset

    // method line 414
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x4080
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 80 (0x50)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0020,
	  IL_0020,
	  IL_004f,
	  IL_0020)
	IL_001f:  ret 
	.try { // 1
	  IL_0020:  ldarg.0 
	  IL_0021:  ldfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	  IL_0026:  stloc.1 
	  IL_0027:  ldloc.1 
	  IL_0028:  ldc.i4.2 
	  IL_0029:  sub 
	  IL_002a:  switch (
	    IL_003d,
	    IL_0046,
	    IL_003d)
	  IL_003b:  br.s IL_0046

	  .try { // 0
	    IL_003d:  leave.s IL_0046

	  } // end .try 0
	  finally  { // 0
	    IL_003f:  ldarg.0 
	    IL_0040:  call instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::'<>m__Finally69'()
	    IL_0045:  endfinally 
	  } // end handler 0
	  IL_0046:  leave.s IL_004f

	} // end .try 1
	finally  { // 1
	  IL_0048:  ldarg.0 
	  IL_0049:  call instance void class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!TSource>::'<>m__Finally68'()
	  IL_004e:  endfinally 
	} // end handler 1
	IL_004f:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.IDisposable.Dispose

    // method line 415
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x40f8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::System.Collections.IEnumerator.get_Current

    // method line 416
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4105
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::.ctor

    // method line 417
    .method private hidebysig 
           instance default void '<>m__Finally68' ()  cil managed 
    {
        // Method begins at RVA 0x4124
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<outerEnumerator>5__65'
	IL_000d:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0012:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::<>m__Finally68

    // method line 418
    .method private hidebysig 
           instance default void '<>m__Finally69' ()  cil managed 
    {
        // Method begins at RVA 0x4138
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<innerEnumerator>5__66'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'<!0>::'<innerEnumerator>5__66'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <OnErrorResumeNextHelper>d__64`1::<>m__Finally69

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<OnErrorResumeNextHelper>d__64`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <OnErrorResumeNextHelper>d__64`1

  .class nested private auto ansi sealed beforefieldinit '<ConcatHelper>d__6c`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> sources
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>3__sources'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<source>5__6d'
    .field  public  !0 '<item>5__6e'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>7__wrap6f'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrap71'

    // method line 419
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x41a0
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>3__sources'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::sources
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <ConcatHelper>d__6c`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 420
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x41e8
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <ConcatHelper>d__6c`1::System.Collections.IEnumerable.GetEnumerator

    // method line 421
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x41f0
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 194 (0xc2)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0014

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq.s IL_0088

	  IL_000f:  br IL_00b5

	  IL_0014:  ldarg.0 
	  IL_0015:  ldc.i4.m1 
	  IL_0016:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_001b:  ldarg.0 
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::sources
	  IL_0022:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::GetEnumerator()
	  IL_0027:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap6f'
	  IL_002c:  ldarg.0 
	  IL_002d:  ldc.i4.1 
	  IL_002e:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_0033:  br.s IL_00a2

	  IL_0035:  ldarg.0 
	  IL_0036:  ldarg.0 
	  IL_0037:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap6f'
	  IL_003c:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::get_Current()
	  IL_0041:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<source>5__6d'
	  IL_0046:  ldarg.0 
	  IL_0047:  ldarg.0 
	  IL_0048:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<source>5__6d'
	  IL_004d:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_0052:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap71'
	  IL_0057:  ldarg.0 
	  IL_0058:  ldc.i4.2 
	  IL_0059:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_005e:  br.s IL_008f

	  IL_0060:  ldarg.0 
	  IL_0061:  ldarg.0 
	  IL_0062:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap71'
	  IL_0067:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_006c:  stfld !0 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<item>5__6e'
	  IL_0071:  ldarg.0 
	  IL_0072:  ldarg.0 
	  IL_0073:  ldfld !0 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<item>5__6e'
	  IL_0078:  stfld !0 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>2__current'
	  IL_007d:  ldarg.0 
	  IL_007e:  ldc.i4.3 
	  IL_007f:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_0084:  ldc.i4.1 
	  IL_0085:  stloc.0 
	  IL_0086:  leave.s IL_00c0

	  IL_0088:  ldarg.0 
	  IL_0089:  ldc.i4.2 
	  IL_008a:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_008f:  ldarg.0 
	  IL_0090:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap71'
	  IL_0095:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_009a:  brtrue.s IL_0060

	  IL_009c:  ldarg.0 
	  IL_009d:  call instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'<>m__Finally72'()
	  IL_00a2:  ldarg.0 
	  IL_00a3:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap6f'
	  IL_00a8:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00ad:  brtrue.s IL_0035

	  IL_00af:  ldarg.0 
	  IL_00b0:  call instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'<>m__Finally70'()
	  IL_00b5:  ldc.i4.0 
	  IL_00b6:  stloc.0 
	  IL_00b7:  leave.s IL_00c0

	} // end .try 0
	fault  { // 0
	  IL_00b9:  ldarg.0 
	  IL_00ba:  call instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00bf:  endfault
	} // end handler 0
	IL_00c0:  ldloc.0 
	IL_00c1:  ret 
    } // end of method <ConcatHelper>d__6c`1::MoveNext

    // method line 422
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x42d0
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ConcatHelper>d__6c`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 423
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x42d8
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <ConcatHelper>d__6c`1::System.Collections.IEnumerator.Reset

    // method line 424
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x42e0
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 1
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	  IL_0022:  stloc.1 
	  IL_0023:  ldloc.1 
	  IL_0024:  ldc.i4.2 
	  IL_0025:  sub 
	  IL_0026:  switch (
	    IL_0035,
	    IL_0035)
	  IL_0033:  br.s IL_003e

	  .try { // 0
	    IL_0035:  leave.s IL_003e

	  } // end .try 0
	  finally  { // 0
	    IL_0037:  ldarg.0 
	    IL_0038:  call instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'<>m__Finally72'()
	    IL_003d:  endfinally 
	  } // end handler 0
	  IL_003e:  leave.s IL_0047

	} // end .try 1
	finally  { // 1
	  IL_0040:  ldarg.0 
	  IL_0041:  call instance void class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!TSource>::'<>m__Finally70'()
	  IL_0046:  endfinally 
	} // end handler 1
	IL_0047:  ret 
    } // end of method <ConcatHelper>d__6c`1::System.IDisposable.Dispose

    // method line 425
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4350
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <ConcatHelper>d__6c`1::System.Collections.IEnumerator.get_Current

    // method line 426
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x435d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <ConcatHelper>d__6c`1::.ctor

    // method line 427
    .method private hidebysig 
           instance default void '<>m__Finally70' ()  cil managed 
    {
        // Method begins at RVA 0x437c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap6f'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap6f'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <ConcatHelper>d__6c`1::<>m__Finally70

    // method line 428
    .method private hidebysig 
           instance default void '<>m__Finally72' ()  cil managed 
    {
        // Method begins at RVA 0x4398
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap71'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'<!0>::'<>7__wrap71'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <ConcatHelper>d__6c`1::<>m__Finally72

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<ConcatHelper>d__6c`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <ConcatHelper>d__6c`1

  .class nested private auto ansi sealed beforefieldinit '<CatchHelper>d__75`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> sources
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>3__sources'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<outerEnumerator>5__76'
    .field  public  !0 '<value>5__77'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<innerEnumerator>5__78'

    // method line 429
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4440
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>3__sources'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::sources
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <CatchHelper>d__75`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 430
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4488
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <CatchHelper>d__75`1::System.Collections.IEnumerable.GetEnumerator

    // method line 431
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4490
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 215 (0xd7)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0017

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.4 
	  IL_000d:  beq IL_00b6

	  IL_0012:  br IL_00ca

	  IL_0017:  ldarg.0 
	  IL_0018:  ldc.i4.m1 
	  IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_001e:  ldarg.0 
	  IL_001f:  ldarg.0 
	  IL_0020:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::sources
	  IL_0025:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::GetEnumerator()
	  IL_002a:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<outerEnumerator>5__76'
	  IL_002f:  ldarg.0 
	  IL_0030:  ldc.i4.1 
	  IL_0031:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_0036:  ldarg.0 
	  IL_0037:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<outerEnumerator>5__76'
	  IL_003c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0041:  brtrue.s IL_004b

	  IL_0043:  ldarg.0 
	  IL_0044:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::System.IDisposable.Dispose()
	  IL_0049:  leave.s IL_00ca

	  IL_004b:  ldarg.0 
	  IL_004c:  ldflda !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<value>5__77'
	  IL_0051:  initobj !0
	  IL_0057:  ldarg.0 
	  IL_0058:  ldarg.0 
	  IL_0059:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<outerEnumerator>5__76'
	  IL_005e:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::get_Current()
	  IL_0063:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_0068:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<innerEnumerator>5__78'
	  IL_006d:  ldarg.0 
	  IL_006e:  ldc.i4.2 
	  IL_006f:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  .try { // 0
	    IL_0074:  ldarg.0 
	    IL_0075:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<innerEnumerator>5__78'
	    IL_007a:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_007f:  brtrue.s IL_0089

	    IL_0081:  ldarg.0 
	    IL_0082:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::System.IDisposable.Dispose()
	    IL_0087:  leave.s IL_00ca

	    IL_0089:  ldarg.0 
	    IL_008a:  ldarg.0 
	    IL_008b:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<innerEnumerator>5__78'
	    IL_0090:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	    IL_0095:  stfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<value>5__77'
	    IL_009a:  leave.s IL_009f

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_009c:  pop 
	    IL_009d:  leave.s IL_00bf

	  } // end handler 0
	  IL_009f:  ldarg.0 
	  IL_00a0:  ldarg.0 
	  IL_00a1:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<value>5__77'
	  IL_00a6:  stfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>2__current'
	  IL_00ab:  ldarg.0 
	  IL_00ac:  ldc.i4.4 
	  IL_00ad:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_00b2:  ldc.i4.1 
	  IL_00b3:  stloc.0 
	  IL_00b4:  leave.s IL_00d5

	  IL_00b6:  ldarg.0 
	  IL_00b7:  ldc.i4.2 
	  IL_00b8:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_00bd:  br.s IL_0074

	  IL_00bf:  ldarg.0 
	  IL_00c0:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::'<>m__Finally7a'()
	  IL_00c5:  br IL_0036

	  IL_00ca:  ldc.i4.0 
	  IL_00cb:  stloc.0 
	  IL_00cc:  leave.s IL_00d5

	} // end .try 1
	fault  { // 1
	  IL_00ce:  ldarg.0 
	  IL_00cf:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00d4:  endfault
	} // end handler 1
	IL_00d5:  ldloc.0 
	IL_00d6:  ret 
    } // end of method <CatchHelper>d__75`1::MoveNext

    // method line 432
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4590
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <CatchHelper>d__75`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 433
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4598
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <CatchHelper>d__75`1::System.Collections.IEnumerator.Reset

    // method line 434
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x45a0
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 80 (0x50)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0020,
	  IL_0020,
	  IL_004f,
	  IL_0020)
	IL_001f:  ret 
	.try { // 1
	  IL_0020:  ldarg.0 
	  IL_0021:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	  IL_0026:  stloc.1 
	  IL_0027:  ldloc.1 
	  IL_0028:  ldc.i4.2 
	  IL_0029:  sub 
	  IL_002a:  switch (
	    IL_003d,
	    IL_0046,
	    IL_003d)
	  IL_003b:  br.s IL_0046

	  .try { // 0
	    IL_003d:  leave.s IL_0046

	  } // end .try 0
	  finally  { // 0
	    IL_003f:  ldarg.0 
	    IL_0040:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::'<>m__Finally7a'()
	    IL_0045:  endfinally 
	  } // end handler 0
	  IL_0046:  leave.s IL_004f

	} // end .try 1
	finally  { // 1
	  IL_0048:  ldarg.0 
	  IL_0049:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!TSource>::'<>m__Finally79'()
	  IL_004e:  endfinally 
	} // end handler 1
	IL_004f:  ret 
    } // end of method <CatchHelper>d__75`1::System.IDisposable.Dispose

    // method line 435
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4618
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <CatchHelper>d__75`1::System.Collections.IEnumerator.get_Current

    // method line 436
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4625
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <CatchHelper>d__75`1::.ctor

    // method line 437
    .method private hidebysig 
           instance default void '<>m__Finally79' ()  cil managed 
    {
        // Method begins at RVA 0x4644
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<outerEnumerator>5__76'
	IL_000d:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0012:  ret 
    } // end of method <CatchHelper>d__75`1::<>m__Finally79

    // method line 438
    .method private hidebysig 
           instance default void '<>m__Finally7a' ()  cil managed 
    {
        // Method begins at RVA 0x4658
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<innerEnumerator>5__78'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__75`1'<!0>::'<innerEnumerator>5__78'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <CatchHelper>d__75`1::<>m__Finally7a

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<CatchHelper>d__75`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<CatchHelper>d__75`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <CatchHelper>d__75`1

  .class nested private auto ansi sealed beforefieldinit '<CatchHelper>d__7d`2'<TSource,(class [mscorlib]System.Exception) TException>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> 'handler'
    .field  public  class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>3__handler'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<reified>5__7e'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<item>5__7f'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> '<errorNotification>5__80'
    .field  public  !1 '<typedException>5__81'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<newEnumerator>5__82'
    .field  public  !0 '<newItem>5__83'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> '<nextNotification>5__84'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<>7__wrap85'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrap87'

    // method line 439
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x46b8
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>3__handler'
	IL_0041:  stfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'handler'
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <CatchHelper>d__7d`2::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 440
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x470c
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <CatchHelper>d__7d`2::System.Collections.IEnumerable.GetEnumerator

    // method line 441
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4714
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 428 (0x1ac)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Exception	V_0,
		bool	V_1,
		int32	V_2)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_0006:  stloc.2 
	  IL_0007:  ldloc.2 
	  IL_0008:  switch (
	    IL_0026,
	    IL_019f,
	    IL_019f,
	    IL_0112,
	    IL_0182)
	  IL_0021:  br IL_019f

	  IL_0026:  ldarg.0 
	  IL_0027:  ldc.i4.m1 
	  IL_0028:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_002d:  ldarg.0 
	  IL_002e:  ldarg.0 
	  IL_002f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::source
	  IL_0034:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!!0>> class System.Linq.EnumerableEx::Materialize<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
	  IL_0039:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<reified>5__7e'
	  IL_003e:  ldarg.0 
	  IL_003f:  ldarg.0 
	  IL_0040:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<reified>5__7e'
	  IL_0045:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::GetEnumerator()
	  IL_004a:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap85'
	  IL_004f:  ldarg.0 
	  IL_0050:  ldc.i4.1 
	  IL_0051:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_0056:  br IL_0189

	  IL_005b:  ldarg.0 
	  IL_005c:  ldarg.0 
	  IL_005d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap85'
	  IL_0062:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>>::get_Current()
	  IL_0067:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<item>5__7f'
	  IL_006c:  ldarg.0 
	  IL_006d:  ldarg.0 
	  IL_006e:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<item>5__7f'
	  IL_0073:  isinst class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0>
	  IL_0078:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<errorNotification>5__80'
	  IL_007d:  ldarg.0 
	  IL_007e:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<errorNotification>5__80'
	  IL_0083:  ldnull 
	  IL_0084:  call bool class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::op_Inequality(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	  IL_0089:  brfalse IL_0147

	  IL_008e:  ldarg.0 
	  IL_008f:  ldarg.0 
	  IL_0090:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<errorNotification>5__80'
	  IL_0095:  callvirt instance class [mscorlib]System.Exception class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Exception()
	  IL_009a:  isinst !1
	  IL_009f:  unbox.any !1
	  IL_00a4:  stfld !1 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<typedException>5__81'
	  IL_00a9:  ldarg.0 
	  IL_00aa:  ldfld !1 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<typedException>5__81'
	  IL_00af:  box !1
	  IL_00b4:  brfalse.s IL_0134

	  IL_00b6:  ldarg.0 
	  IL_00b7:  ldarg.0 
	  IL_00b8:  ldfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'handler'
	  IL_00bd:  ldarg.0 
	  IL_00be:  ldfld !1 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<typedException>5__81'
	  IL_00c3:  callvirt instance !1 class [System.Core]System.Func`2<!TException, class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::Invoke(!0)
	  IL_00c8:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<newEnumerator>5__82'
	  IL_00cd:  ldarg.0 
	  IL_00ce:  ldarg.0 
	  IL_00cf:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<newEnumerator>5__82'
	  IL_00d4:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_00d9:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap87'
	  IL_00de:  ldarg.0 
	  IL_00df:  ldc.i4.2 
	  IL_00e0:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_00e5:  br.s IL_0119

	  IL_00e7:  ldarg.0 
	  IL_00e8:  ldarg.0 
	  IL_00e9:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap87'
	  IL_00ee:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_00f3:  stfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<newItem>5__83'
	  IL_00f8:  ldarg.0 
	  IL_00f9:  ldarg.0 
	  IL_00fa:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<newItem>5__83'
	  IL_00ff:  stfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>2__current'
	  IL_0104:  ldarg.0 
	  IL_0105:  ldc.i4.3 
	  IL_0106:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_010b:  ldc.i4.1 
	  IL_010c:  stloc.1 
	  IL_010d:  leave IL_01aa

	  IL_0112:  ldarg.0 
	  IL_0113:  ldc.i4.2 
	  IL_0114:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_0119:  ldarg.0 
	  IL_011a:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap87'
	  IL_011f:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0124:  brtrue.s IL_00e7

	  IL_0126:  ldarg.0 
	  IL_0127:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'<>m__Finally88'()
	  IL_012c:  ldarg.0 
	  IL_012d:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::System.IDisposable.Dispose()
	  IL_0132:  leave.s IL_019f

	  IL_0134:  ldarg.0 
	  IL_0135:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnError<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<errorNotification>5__80'
	  IL_013a:  callvirt instance class [mscorlib]System.Exception class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Exception()
	  IL_013f:  stloc.0 
	  IL_0140:  ldloc.0 
	  IL_0141:  call class [mscorlib]System.Exception class [System.CoreEx]System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	  IL_0146:  throw 
	  IL_0147:  ldarg.0 
	  IL_0148:  ldarg.0 
	  IL_0149:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<item>5__7f'
	  IL_014e:  isinst class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0>
	  IL_0153:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<nextNotification>5__84'
	  IL_0158:  ldarg.0 
	  IL_0159:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<nextNotification>5__84'
	  IL_015e:  ldnull 
	  IL_015f:  call bool class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::op_Inequality(class [System.CoreEx]System.Collections.Generic.Notification`1<!0>, class [System.CoreEx]System.Collections.Generic.Notification`1<!0>)
	  IL_0164:  brfalse.s IL_0189

	  IL_0166:  ldarg.0 
	  IL_0167:  ldarg.0 
	  IL_0168:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1/OnNext<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<nextNotification>5__84'
	  IL_016d:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TSource>::get_Value()
	  IL_0172:  stfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>2__current'
	  IL_0177:  ldarg.0 
	  IL_0178:  ldc.i4.4 
	  IL_0179:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_017e:  ldc.i4.1 
	  IL_017f:  stloc.1 
	  IL_0180:  leave.s IL_01aa

	  IL_0182:  ldarg.0 
	  IL_0183:  ldc.i4.1 
	  IL_0184:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_0189:  ldarg.0 
	  IL_018a:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap85'
	  IL_018f:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0194:  brtrue IL_005b

	  IL_0199:  ldarg.0 
	  IL_019a:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'<>m__Finally86'()
	  IL_019f:  ldc.i4.0 
	  IL_01a0:  stloc.1 
	  IL_01a1:  leave.s IL_01aa

	} // end .try 0
	fault  { // 0
	  IL_01a3:  ldarg.0 
	  IL_01a4:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::System.IDisposable.Dispose()
	  IL_01a9:  endfault
	} // end handler 0
	IL_01aa:  ldloc.1 
	IL_01ab:  ret 
    } // end of method <CatchHelper>d__7d`2::MoveNext

    // method line 442
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x48e8
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <CatchHelper>d__7d`2::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 443
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x48f0
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <CatchHelper>d__7d`2::System.Collections.IEnumerator.Reset

    // method line 444
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x48f8
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 76 (0x4c)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Exception	V_0,
		int32	V_1,
		int32	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.1 
	IL_0007:  ldloc.1 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0020,
	  IL_0020,
	  IL_0020,
	  IL_0020)
	IL_001f:  ret 
	.try { // 1
	  IL_0020:  ldarg.0 
	  IL_0021:  ldfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	  IL_0026:  stloc.2 
	  IL_0027:  ldloc.2 
	  IL_0028:  ldc.i4.2 
	  IL_0029:  sub 
	  IL_002a:  switch (
	    IL_0039,
	    IL_0039)
	  IL_0037:  br.s IL_0042

	  .try { // 0
	    IL_0039:  leave.s IL_0042

	  } // end .try 0
	  finally  { // 0
	    IL_003b:  ldarg.0 
	    IL_003c:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'<>m__Finally88'()
	    IL_0041:  endfinally 
	  } // end handler 0
	  IL_0042:  leave.s IL_004b

	} // end .try 1
	finally  { // 1
	  IL_0044:  ldarg.0 
	  IL_0045:  call instance void class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!TSource, !TException>::'<>m__Finally86'()
	  IL_004a:  endfinally 
	} // end handler 1
	IL_004b:  ret 
    } // end of method <CatchHelper>d__7d`2::System.IDisposable.Dispose

    // method line 445
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x496c
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <CatchHelper>d__7d`2::System.Collections.IEnumerator.get_Current

    // method line 446
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4979
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <CatchHelper>d__7d`2::.ctor

    // method line 447
    .method private hidebysig 
           instance default void '<>m__Finally86' ()  cil managed 
    {
        // Method begins at RVA 0x4998
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap85'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap85'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <CatchHelper>d__7d`2::<>m__Finally86

    // method line 448
    .method private hidebysig 
           instance default void '<>m__Finally88' ()  cil managed 
    {
        // Method begins at RVA 0x49b4
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap87'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'<!0,!1>::'<>7__wrap87'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <CatchHelper>d__7d`2::<>m__Finally88

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<CatchHelper>d__7d`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <CatchHelper>d__7d`2

  .class nested private auto ansi sealed beforefieldinit '<ZipIterator>d__8b`3'<TFirst,TSecond,TResult>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!2>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!2>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !2 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> first
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__first'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> second
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> '<>3__second'
    .field  public  class [System.Core]System.Func`3<!0,!1,!2> resultSelector
    .field  public  class [System.Core]System.Func`3<!0,!1,!2> '<>3__resultSelector'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<e1>5__8c'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> '<e2>5__8d'

    // method line 449
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult> 'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4a28
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>3__first'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::first
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>3__second'
	IL_0041:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::second
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [System.Core]System.Func`3<!0,!1,!2> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>3__resultSelector'
	IL_004d:  stfld class [System.Core]System.Func`3<!0,!1,!2> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::resultSelector
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <ZipIterator>d__8b`3::System.Collections.Generic.IEnumerable<TResult>.GetEnumerator

    // method line 450
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4a88
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!2> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <ZipIterator>d__8b`3::System.Collections.IEnumerable.GetEnumerator

    // method line 451
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4a90
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 185 (0xb9)
	.maxstack 4
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0014

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq.s IL_007f

	  IL_000f:  br IL_00ac

	  IL_0014:  ldarg.0 
	  IL_0015:  ldc.i4.m1 
	  IL_0016:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_001b:  ldarg.0 
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::first
	  IL_0022:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TFirst>::GetEnumerator()
	  IL_0027:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e1>5__8c'
	  IL_002c:  ldarg.0 
	  IL_002d:  ldc.i4.1 
	  IL_002e:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_0033:  ldarg.0 
	  IL_0034:  ldarg.0 
	  IL_0035:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::second
	  IL_003a:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSecond>::GetEnumerator()
	  IL_003f:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e2>5__8d'
	  IL_0044:  ldarg.0 
	  IL_0045:  ldc.i4.2 
	  IL_0046:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_004b:  br.s IL_0086

	  IL_004d:  ldarg.0 
	  IL_004e:  ldarg.0 
	  IL_004f:  ldfld class [System.Core]System.Func`3<!0,!1,!2> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::resultSelector
	  IL_0054:  ldarg.0 
	  IL_0055:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e1>5__8c'
	  IL_005a:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TFirst>::get_Current()
	  IL_005f:  ldarg.0 
	  IL_0060:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e2>5__8d'
	  IL_0065:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSecond>::get_Current()
	  IL_006a:  callvirt instance !2 class [System.Core]System.Func`3<!TFirst, !TSecond, !TResult>::Invoke(!0, !1)
	  IL_006f:  stfld !2 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>2__current'
	  IL_0074:  ldarg.0 
	  IL_0075:  ldc.i4.3 
	  IL_0076:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_007b:  ldc.i4.1 
	  IL_007c:  stloc.0 
	  IL_007d:  leave.s IL_00b7

	  IL_007f:  ldarg.0 
	  IL_0080:  ldc.i4.2 
	  IL_0081:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_0086:  ldarg.0 
	  IL_0087:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e1>5__8c'
	  IL_008c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0091:  brfalse.s IL_00a0

	  IL_0093:  ldarg.0 
	  IL_0094:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e2>5__8d'
	  IL_0099:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_009e:  brtrue.s IL_004d

	  IL_00a0:  ldarg.0 
	  IL_00a1:  call instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'<>m__Finally8f'()
	  IL_00a6:  ldarg.0 
	  IL_00a7:  call instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'<>m__Finally8e'()
	  IL_00ac:  ldc.i4.0 
	  IL_00ad:  stloc.0 
	  IL_00ae:  leave.s IL_00b7

	} // end .try 0
	fault  { // 0
	  IL_00b0:  ldarg.0 
	  IL_00b1:  call instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::System.IDisposable.Dispose()
	  IL_00b6:  endfault
	} // end handler 0
	IL_00b7:  ldloc.0 
	IL_00b8:  ret 
    } // end of method <ZipIterator>d__8b`3::MoveNext

    // method line 452
    .method private final virtual hidebysig newslot specialname 
           instance default !TResult 'System.Collections.Generic.IEnumerator<TResult>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4b68
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !2 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ZipIterator>d__8b`3::System.Collections.Generic.IEnumerator<TResult>.get_Current

    // method line 453
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4b70
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <ZipIterator>d__8b`3::System.Collections.IEnumerator.Reset

    // method line 454
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x4b78
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_001c,
	  IL_001c)
	IL_001b:  ret 
	.try { // 1
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	  IL_0022:  stloc.1 
	  IL_0023:  ldloc.1 
	  IL_0024:  ldc.i4.2 
	  IL_0025:  sub 
	  IL_0026:  switch (
	    IL_0035,
	    IL_0035)
	  IL_0033:  br.s IL_003e

	  .try { // 0
	    IL_0035:  leave.s IL_003e

	  } // end .try 0
	  finally  { // 0
	    IL_0037:  ldarg.0 
	    IL_0038:  call instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'<>m__Finally8f'()
	    IL_003d:  endfinally 
	  } // end handler 0
	  IL_003e:  leave.s IL_0047

	} // end .try 1
	finally  { // 1
	  IL_0040:  ldarg.0 
	  IL_0041:  call instance void class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!TFirst, !TSecond, !TResult>::'<>m__Finally8e'()
	  IL_0046:  endfinally 
	} // end handler 1
	IL_0047:  ret 
    } // end of method <ZipIterator>d__8b`3::System.IDisposable.Dispose

    // method line 455
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4be8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !2 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>2__current'
	IL_0006:  box !2
	IL_000b:  ret 
    } // end of method <ZipIterator>d__8b`3::System.Collections.IEnumerator.get_Current

    // method line 456
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4bf5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <ZipIterator>d__8b`3::.ctor

    // method line 457
    .method private hidebysig 
           instance default void '<>m__Finally8e' ()  cil managed 
    {
        // Method begins at RVA 0x4c14
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e1>5__8c'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e1>5__8c'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <ZipIterator>d__8b`3::<>m__Finally8e

    // method line 458
    .method private hidebysig 
           instance default void '<>m__Finally8f' ()  cil managed 
    {
        // Method begins at RVA 0x4c30
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e2>5__8d'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'<!0,!1,!2>::'<e2>5__8d'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <ZipIterator>d__8b`3::<>m__Finally8f

	.property instance !TResult 'System.Collections.Generic.IEnumerator<TResult>.Current' ()
	{
		.get instance default !2 System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'::'System.Collections.Generic.IEnumerator<TResult>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<ZipIterator>d__8b`3'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <ZipIterator>d__8b`3

  .class nested private auto ansi sealed beforefieldinit '<ForkJoinHelper>d__92`1'<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0[]>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0[]>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0[] '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> sources
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> '<>3__sources'
    .field  public  class [mscorlib]System.Collections.Generic.List`1<!0> '<results>5__93'

    // method line 459
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T[]> 'System.Collections.Generic.IEnumerable<T[]>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4ca4
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T[]>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!T>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!T>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>3__sources'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::sources
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <ForkJoinHelper>d__92`1::System.Collections.Generic.IEnumerable<T[]>.GetEnumerator

    // method line 460
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4cec
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0[]> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!T>::'System.Collections.Generic.IEnumerable<T[]>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <ForkJoinHelper>d__92`1::System.Collections.IEnumerable.GetEnumerator

    // method line 461
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4cf4
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 203 (0xcb)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>	V_0,
		bool	V_1,
		!T	V_2,
		!T	V_3,
		int32	V_4,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>>	V_5,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>	V_6)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_0006:  stloc.s 4
	IL_0008:  ldloc.s 4
	IL_000a:  ldc.i4.0 
	IL_000b:  beq.s IL_001a

	IL_000d:  ldloc.s 4
	IL_000f:  ldc.i4.3 
	IL_0010:  beq IL_00c2

	IL_0015:  br IL_00c9

	IL_001a:  ldarg.0 
	IL_001b:  ldc.i4.m1 
	IL_001c:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_0021:  ldarg.0 
	IL_0022:  newobj instance void class [mscorlib]System.Collections.Generic.List`1<!T>::'.ctor'()
	IL_0027:  stfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<results>5__93'
	IL_002c:  ldarg.0 
	IL_002d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::sources
	IL_0032:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>>::GetEnumerator()
	IL_0037:  stloc.s 5
	.try { // 1
	  IL_0039:  br.s IL_0091

	  IL_003b:  ldloc.s 5
	  IL_003d:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>>::get_Current()
	  IL_0042:  stloc.0 
	  IL_0043:  ldc.i4.0 
	  IL_0044:  stloc.1 
	  IL_0045:  ldloca.s 2
	  IL_0047:  initobj !0
	  IL_004d:  ldloc.0 
	  IL_004e:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
	  IL_0053:  stloc.s 6
	  .try { // 0
	    IL_0055:  br.s IL_0063

	    IL_0057:  ldloc.s 6
	    IL_0059:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	    IL_005e:  stloc.3 
	    IL_005f:  ldc.i4.1 
	    IL_0060:  stloc.1 
	    IL_0061:  ldloc.3 
	    IL_0062:  stloc.2 
	    IL_0063:  ldloc.s 6
	    IL_0065:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_006a:  brtrue.s IL_0057

	    IL_006c:  leave.s IL_007a

	  } // end .try 0
	  finally  { // 0
	    IL_006e:  ldloc.s 6
	    IL_0070:  brfalse.s IL_0079

	    IL_0072:  ldloc.s 6
	    IL_0074:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	    IL_0079:  endfinally 
	  } // end handler 0
	  IL_007a:  ldloc.1 
	  IL_007b:  brtrue.s IL_0085

	  IL_007d:  ldarg.0 
	  IL_007e:  call instance void class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!T>::System.IDisposable.Dispose()
	  IL_0083:  leave.s IL_00c9

	  IL_0085:  ldarg.0 
	  IL_0086:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<results>5__93'
	  IL_008b:  ldloc.2 
	  IL_008c:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<!T>::Add(!0)
	  IL_0091:  ldloc.s 5
	  IL_0093:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0098:  brtrue.s IL_003b

	  IL_009a:  leave.s IL_00a8

	} // end .try 1
	finally  { // 1
	  IL_009c:  ldloc.s 5
	  IL_009e:  brfalse.s IL_00a7

	  IL_00a0:  ldloc.s 5
	  IL_00a2:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_00a7:  endfinally 
	} // end handler 1
	IL_00a8:  ldarg.0 
	IL_00a9:  ldarg.0 
	IL_00aa:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<results>5__93'
	IL_00af:  callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<!T>::ToArray()
	IL_00b4:  stfld !0[] class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>2__current'
	IL_00b9:  ldarg.0 
	IL_00ba:  ldc.i4.3 
	IL_00bb:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_00c0:  ldc.i4.1 
	IL_00c1:  ret 
	IL_00c2:  ldarg.0 
	IL_00c3:  ldc.i4.m1 
	IL_00c4:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_00c9:  ldc.i4.0 
	IL_00ca:  ret 
    } // end of method <ForkJoinHelper>d__92`1::MoveNext

    // method line 462
    .method private final virtual hidebysig newslot specialname 
           instance default !T[] 'System.Collections.Generic.IEnumerator<T[]>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4de8
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T[]>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0[] class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ForkJoinHelper>d__92`1::System.Collections.Generic.IEnumerator<T[]>.get_Current

    // method line 463
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4df0
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <ForkJoinHelper>d__92`1::System.Collections.IEnumerator.Reset

    // method line 464
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x4df8
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 0
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>	V_0,
		bool	V_1,
		!T	V_2,
		!T	V_3)
	IL_0000:  ret 
    } // end of method <ForkJoinHelper>d__92`1::System.IDisposable.Dispose

    // method line 465
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4e05
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0[] class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ForkJoinHelper>d__92`1::System.Collections.IEnumerator.get_Current

    // method line 466
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4e0d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <ForkJoinHelper>d__92`1::.ctor

	.property instance !T[] 'System.Collections.Generic.IEnumerator<T[]>.Current' ()
	{
		.get instance default !0[] System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'::'System.Collections.Generic.IEnumerator<T[]>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<ForkJoinHelper>d__92`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <ForkJoinHelper>d__92`1

  .class nested private auto ansi sealed beforefieldinit '<Return>d__96`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  !0 'value'
    .field  public  !0 '<>3__value'

    // method line 467
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4e4c
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Return>d__96`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Return>d__96`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>3__value'
	IL_0035:  stfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'value'
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <Return>d__96`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 468
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4e94
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Return>d__96`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Return>d__96`1::System.Collections.IEnumerable.GetEnumerator

    // method line 469
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4e9c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_0033)
	IL_0015:  br.s IL_003a

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'value'
	IL_0025:  stfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>2__current'
	IL_002a:  ldarg.0 
	IL_002b:  ldc.i4.1 
	IL_002c:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_0031:  ldc.i4.1 
	IL_0032:  ret 
	IL_0033:  ldarg.0 
	IL_0034:  ldc.i4.m1 
	IL_0035:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_003a:  ldc.i4.0 
	IL_003b:  ret 
    } // end of method <Return>d__96`1::MoveNext

    // method line 470
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4ee4
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Return>d__96`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 471
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4eec
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Return>d__96`1::System.Collections.IEnumerator.Reset

    // method line 472
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x4ef3
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <Return>d__96`1::System.IDisposable.Dispose

    // method line 473
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4ef5
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Return>d__96`1::System.Collections.IEnumerator.get_Current

    // method line 474
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4f02
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Return>d__96`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Return>d__96`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Return>d__96`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Return>d__96`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Return>d__96`1

  .class nested private auto ansi sealed beforefieldinit '<ThrowHelper>d__99`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Exception exception
    .field  public  class [mscorlib]System.Exception '<>3__exception'

    // method line 475
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4f58
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>3__exception'
	IL_0035:  stfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::exception
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <ThrowHelper>d__99`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 476
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4fa0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <ThrowHelper>d__99`1::System.Collections.IEnumerable.GetEnumerator

    // method line 477
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x4fa8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 27 (0x1b)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.0 
	IL_0009:  bne.un.s IL_0019

	IL_000b:  ldarg.0 
	IL_000c:  ldc.i4.m1 
	IL_000d:  stfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>1__state'
	IL_0012:  ldarg.0 
	IL_0013:  ldfld class [mscorlib]System.Exception class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::exception
	IL_0018:  throw 
	IL_0019:  ldc.i4.0 
	IL_001a:  ret 
    } // end of method <ThrowHelper>d__99`1::MoveNext

    // method line 478
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4fcf
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ThrowHelper>d__99`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 479
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4fd7
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <ThrowHelper>d__99`1::System.Collections.IEnumerator.Reset

    // method line 480
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x4fde
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <ThrowHelper>d__99`1::System.IDisposable.Dispose

    // method line 481
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4fe0
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <ThrowHelper>d__99`1::System.Collections.IEnumerator.get_Current

    // method line 482
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4fed
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <ThrowHelper>d__99`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<ThrowHelper>d__99`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <ThrowHelper>d__99`1

  .class nested private auto ansi sealed beforefieldinit '<Never>d__9c`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'

    // method line 483
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x502c
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 48 (0x30)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Never>d__9c`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Never>d__9c`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ret 
    } // end of method <Never>d__9c`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 484
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5068
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Never>d__9c`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Never>d__9c`1::System.Collections.IEnumerable.GetEnumerator

    // method line 485
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x5070
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 26 (0x1a)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.0 
	IL_0009:  bne.un.s IL_0018

	IL_000b:  ldarg.0 
	IL_000c:  ldc.i4.m1 
	IL_000d:  stfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>1__state'
	IL_0012:  ldc.i4.m1 
	IL_0013:  call void class [mscorlib]System.Threading.Thread::Sleep(int32)
	IL_0018:  ldc.i4.0 
	IL_0019:  ret 
    } // end of method <Never>d__9c`1::MoveNext

    // method line 486
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5096
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Never>d__9c`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 487
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x509e
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Never>d__9c`1::System.Collections.IEnumerator.Reset

    // method line 488
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x50a5
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <Never>d__9c`1::System.IDisposable.Dispose

    // method line 489
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x50a7
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Never>d__9c`1::System.Collections.IEnumerator.get_Current

    // method line 490
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x50b4
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Never>d__9c`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Never>d__9c`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Never>d__9c`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Never>d__9c`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Never>d__9c`1

  .class nested private auto ansi sealed beforefieldinit '<GenerateHelper>d__9f`2'<TState,TResult>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!1>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !1 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  !0 initialState
    .field  public  !0 '<>3__initialState'
    .field  public  class [System.Core]System.Func`2<!0,bool> condition
    .field  public  class [System.Core]System.Func`2<!0,bool> '<>3__condition'
    .field  public  class [System.Core]System.Func`2<!0,!1> resultSelector
    .field  public  class [System.Core]System.Func`2<!0,!1> '<>3__resultSelector'
    .field  public  class [System.Core]System.Func`2<!0,!0> iterate
    .field  public  class [System.Core]System.Func`2<!0,!0> '<>3__iterate'
    .field  public  !0 '<currentState>5__a0'

    // method line 491
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult> 'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5120
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>::GetEnumerator()
	// Code size 96 (0x60)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!TState, !TResult>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!TState, !TResult>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>3__initialState'
	IL_0035:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::initialState
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>3__condition'
	IL_0041:  stfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::condition
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>3__resultSelector'
	IL_004d:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::resultSelector
	IL_0052:  ldloc.0 
	IL_0053:  ldarg.0 
	IL_0054:  ldfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>3__iterate'
	IL_0059:  stfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::iterate
	IL_005e:  ldloc.0 
	IL_005f:  ret 
    } // end of method <GenerateHelper>d__9f`2::System.Collections.Generic.IEnumerable<TResult>.GetEnumerator

    // method line 492
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x518c
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!TState, !TResult>::'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__9f`2::System.Collections.IEnumerable.GetEnumerator

    // method line 493
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x5194
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 127 (0x7f)
	.maxstack 3
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_004c)
	IL_0015:  br.s IL_007d

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::initialState
	IL_0025:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<currentState>5__a0'
	IL_002a:  br.s IL_006a

	IL_002c:  ldarg.0 
	IL_002d:  ldarg.0 
	IL_002e:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::resultSelector
	IL_0033:  ldarg.0 
	IL_0034:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<currentState>5__a0'
	IL_0039:  callvirt instance !1 class [System.Core]System.Func`2<!TState, !TResult>::Invoke(!0)
	IL_003e:  stfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>2__current'
	IL_0043:  ldarg.0 
	IL_0044:  ldc.i4.1 
	IL_0045:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_004a:  ldc.i4.1 
	IL_004b:  ret 
	IL_004c:  ldarg.0 
	IL_004d:  ldc.i4.m1 
	IL_004e:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_0053:  ldarg.0 
	IL_0054:  ldarg.0 
	IL_0055:  ldfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::iterate
	IL_005a:  ldarg.0 
	IL_005b:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<currentState>5__a0'
	IL_0060:  callvirt instance !1 class [System.Core]System.Func`2<!TState, !TState>::Invoke(!0)
	IL_0065:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<currentState>5__a0'
	IL_006a:  ldarg.0 
	IL_006b:  ldfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::condition
	IL_0070:  ldarg.0 
	IL_0071:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<currentState>5__a0'
	IL_0076:  callvirt instance !1 class [System.Core]System.Func`2<!TState, bool>::Invoke(!0)
	IL_007b:  brtrue.s IL_002c

	IL_007d:  ldc.i4.0 
	IL_007e:  ret 
    } // end of method <GenerateHelper>d__9f`2::MoveNext

    // method line 494
    .method private final virtual hidebysig newslot specialname 
           instance default !TResult 'System.Collections.Generic.IEnumerator<TResult>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x521f
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__9f`2::System.Collections.Generic.IEnumerator<TResult>.get_Current

    // method line 495
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5227
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <GenerateHelper>d__9f`2::System.Collections.IEnumerator.Reset

    // method line 496
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x522e
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <GenerateHelper>d__9f`2::System.IDisposable.Dispose

    // method line 497
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5230
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>2__current'
	IL_0006:  box !1
	IL_000b:  ret 
    } // end of method <GenerateHelper>d__9f`2::System.Collections.IEnumerator.get_Current

    // method line 498
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x523d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <GenerateHelper>d__9f`2::.ctor

	.property instance !TResult 'System.Collections.Generic.IEnumerator<TResult>.Current' ()
	{
		.get instance default !1 System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'::'System.Collections.Generic.IEnumerator<TResult>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<GenerateHelper>d__9f`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <GenerateHelper>d__9f`2

  .class nested private auto ansi sealed beforefieldinit '<GenerateHelper>d__a3`1'<TValue>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> function
    .field  public  class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> '<>3__function'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!0> '<n>5__a4'

    // method line 499
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue> 'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x52a4
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TValue>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!TValue>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!TValue>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>3__function'
	IL_0035:  stfld class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::function
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <GenerateHelper>d__a3`1::System.Collections.Generic.IEnumerable<TValue>.GetEnumerator

    // method line 500
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x52ec
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!TValue>::'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__a3`1::System.Collections.IEnumerable.GetEnumerator

    // method line 501
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x52f4
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 98 (0x62)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_0057)
	IL_0015:  br.s IL_0060

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!0>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::function
	IL_0025:  callvirt instance !0 class [System.Core]System.Func`1<class [System.CoreEx]System.Collections.Generic.Notification`1<!TValue>>::Invoke()
	IL_002a:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<n>5__a4'
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<n>5__a4'
	IL_0035:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!TValue>::get_Kind()
	IL_003a:  ldc.i4.2 
	IL_003b:  beq.s IL_0060

	IL_003d:  ldarg.0 
	IL_003e:  ldarg.0 
	IL_003f:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<n>5__a4'
	IL_0044:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TValue>::get_Value()
	IL_0049:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>2__current'
	IL_004e:  ldarg.0 
	IL_004f:  ldc.i4.1 
	IL_0050:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_0055:  ldc.i4.1 
	IL_0056:  ret 
	IL_0057:  ldarg.0 
	IL_0058:  ldc.i4.m1 
	IL_0059:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_005e:  br.s IL_001e

	IL_0060:  ldc.i4.0 
	IL_0061:  ret 
    } // end of method <GenerateHelper>d__a3`1::MoveNext

    // method line 502
    .method private final virtual hidebysig newslot specialname 
           instance default !TValue 'System.Collections.Generic.IEnumerator<TValue>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5362
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__a3`1::System.Collections.Generic.IEnumerator<TValue>.get_Current

    // method line 503
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x536a
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <GenerateHelper>d__a3`1::System.Collections.IEnumerator.Reset

    // method line 504
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5371
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <GenerateHelper>d__a3`1::System.IDisposable.Dispose

    // method line 505
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5373
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <GenerateHelper>d__a3`1::System.Collections.IEnumerator.get_Current

    // method line 506
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5380
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <GenerateHelper>d__a3`1::.ctor

	.property instance !TValue 'System.Collections.Generic.IEnumerator<TValue>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'::'System.Collections.Generic.IEnumerator<TValue>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<GenerateHelper>d__a3`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <GenerateHelper>d__a3`1

  .class nested private auto ansi sealed beforefieldinit '<GenerateHelper>d__a7`2'<TState,TResult>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!1>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!1>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !1 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  !0 initialState
    .field  public  !0 '<>3__initialState'
    .field  public  class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> resultSelector
    .field  public  class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> '<>3__resultSelector'
    .field  public  class [System.Core]System.Func`2<!0,!0> iterate
    .field  public  class [System.Core]System.Func`2<!0,!0> '<>3__iterate'
    .field  public  !0 '<state>5__a8'
    .field  public  class [System.CoreEx]System.Collections.Generic.Notification`1<!1> '<n>5__a9'

    // method line 507
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult> 'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x53e4
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!TState, !TResult>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!TState, !TResult>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>3__initialState'
	IL_0035:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::initialState
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>3__resultSelector'
	IL_0041:  stfld class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::resultSelector
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>3__iterate'
	IL_004d:  stfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::iterate
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <GenerateHelper>d__a7`2::System.Collections.Generic.IEnumerable<TResult>.GetEnumerator

    // method line 508
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5444
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!TState, !TResult>::'System.Collections.Generic.IEnumerable<TResult>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__a7`2::System.Collections.IEnumerable.GetEnumerator

    // method line 509
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x544c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 139 (0x8b)
	.maxstack 3
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_0069)
	IL_0015:  br.s IL_0089

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::initialState
	IL_0025:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<state>5__a8'
	IL_002a:  ldarg.0 
	IL_002b:  ldarg.0 
	IL_002c:  ldfld class [System.Core]System.Func`2<!0,class [System.CoreEx]System.Collections.Generic.Notification`1<!1>> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::resultSelector
	IL_0031:  ldarg.0 
	IL_0032:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<state>5__a8'
	IL_0037:  callvirt instance !1 class [System.Core]System.Func`2<!TState, class [System.CoreEx]System.Collections.Generic.Notification`1<!TResult>>::Invoke(!0)
	IL_003c:  stfld class [System.CoreEx]System.Collections.Generic.Notification`1<!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<n>5__a9'
	IL_0041:  ldarg.0 
	IL_0042:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<n>5__a9'
	IL_0047:  callvirt instance valuetype [System.CoreEx]System.Collections.Generic.NotificationKind class [System.CoreEx]System.Collections.Generic.Notification`1<!TResult>::get_Kind()
	IL_004c:  ldc.i4.2 
	IL_004d:  beq.s IL_0089

	IL_004f:  ldarg.0 
	IL_0050:  ldarg.0 
	IL_0051:  ldfld class [System.CoreEx]System.Collections.Generic.Notification`1<!1> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<n>5__a9'
	IL_0056:  callvirt instance !0 class [System.CoreEx]System.Collections.Generic.Notification`1<!TResult>::get_Value()
	IL_005b:  stfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>2__current'
	IL_0060:  ldarg.0 
	IL_0061:  ldc.i4.1 
	IL_0062:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_0067:  ldc.i4.1 
	IL_0068:  ret 
	IL_0069:  ldarg.0 
	IL_006a:  ldc.i4.m1 
	IL_006b:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_0070:  ldarg.0 
	IL_0071:  ldarg.0 
	IL_0072:  ldfld class [System.Core]System.Func`2<!0,!0> class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::iterate
	IL_0077:  ldarg.0 
	IL_0078:  ldfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<state>5__a8'
	IL_007d:  callvirt instance !1 class [System.Core]System.Func`2<!TState, !TState>::Invoke(!0)
	IL_0082:  stfld !0 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<state>5__a8'
	IL_0087:  br.s IL_002a

	IL_0089:  ldc.i4.0 
	IL_008a:  ret 
    } // end of method <GenerateHelper>d__a7`2::MoveNext

    // method line 510
    .method private final virtual hidebysig newslot specialname 
           instance default !TResult 'System.Collections.Generic.IEnumerator<TResult>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x54e3
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TResult>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <GenerateHelper>d__a7`2::System.Collections.Generic.IEnumerator<TResult>.get_Current

    // method line 511
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x54eb
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <GenerateHelper>d__a7`2::System.Collections.IEnumerator.Reset

    // method line 512
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x54f2
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <GenerateHelper>d__a7`2::System.IDisposable.Dispose

    // method line 513
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x54f4
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>2__current'
	IL_0006:  box !1
	IL_000b:  ret 
    } // end of method <GenerateHelper>d__a7`2::System.Collections.IEnumerator.get_Current

    // method line 514
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5501
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <GenerateHelper>d__a7`2::.ctor

	.property instance !TResult 'System.Collections.Generic.IEnumerator<TResult>.Current' ()
	{
		.get instance default !1 System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'::'System.Collections.Generic.IEnumerator<TResult>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<GenerateHelper>d__a7`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <GenerateHelper>d__a7`2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClassae`2'<TSource,(class [mscorlib]System.IDisposable) TResource>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [System.Core]System.Func`1<!1> resourceSelector
    .field  public  class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> resourceUsage

    // method line 515
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x55ba
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClassae`2::.ctor

    // method line 516
    .method public hidebysig 
           instance default class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource> '<Using>b__ad' ()  cil managed 
    {
        // Method begins at RVA 0x55c4
	// Code size 48 (0x30)
	.maxstack 4
	.locals init (
		!TResource	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Func`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!0,!1>::resourceSelector
	IL_0006:  callvirt instance !0 class [System.Core]System.Func`1<!TResource>::Invoke()
	IL_000b:  stloc.0 
	IL_000c:  ldarg.0 
	IL_000d:  ldfld class [System.Core]System.Func`2<!1,class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>> class System.Linq.EnumerableEx/'<>c__DisplayClassae`2'<!0,!1>::resourceUsage
	IL_0012:  ldloc.0 
	IL_0013:  callvirt instance !1 class [System.Core]System.Func`2<!TResource, class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>>::Invoke(!0)
	IL_0018:  ldloc.0 
	IL_0019:  box !1
	IL_001e:  dup 
	IL_001f:  ldvirtftn instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0025:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_002a:  call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> class System.Linq.EnumerableEx::Finally<!0> (class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [System.Core]System.Action)
	IL_002f:  ret 
    } // end of method <>c__DisplayClassae`2::<Using>b__ad

  } // end of class <>c__DisplayClassae`2

  .class nested private auto ansi sealed beforefieldinit '<Repeat>d__b0`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  !0 'value'
    .field  public  !0 '<>3__value'

    // method line 517
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5658
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>3__value'
	IL_0035:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'value'
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <Repeat>d__b0`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 518
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x56a0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Repeat>d__b0`1::System.Collections.IEnumerable.GetEnumerator

    // method line 519
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x56a8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 62 (0x3e)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_0033)
	IL_0015:  br.s IL_003c

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldarg.0 
	IL_0020:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'value'
	IL_0025:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>2__current'
	IL_002a:  ldarg.0 
	IL_002b:  ldc.i4.1 
	IL_002c:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_0031:  ldc.i4.1 
	IL_0032:  ret 
	IL_0033:  ldarg.0 
	IL_0034:  ldc.i4.m1 
	IL_0035:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_003a:  br.s IL_001e

	IL_003c:  ldc.i4.0 
	IL_003d:  ret 
    } // end of method <Repeat>d__b0`1::MoveNext

    // method line 520
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x56f2
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Repeat>d__b0`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 521
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x56fa
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Repeat>d__b0`1::System.Collections.IEnumerator.Reset

    // method line 522
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5701
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <Repeat>d__b0`1::System.IDisposable.Dispose

    // method line 523
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5703
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Repeat>d__b0`1::System.Collections.IEnumerator.get_Current

    // method line 524
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5710
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b0`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Repeat>d__b0`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Repeat>d__b0`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Repeat>d__b0`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Repeat>d__b0`1

  .class nested private auto ansi sealed beforefieldinit '<Repeat>d__b3`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  !0 'value'
    .field  public  !0 '<>3__value'
    .field  public  int32 repeatCount
    .field  public  int32 '<>3__repeatCount'
    .field  public  int32 '<i>5__b4'

    // method line 525
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5750
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>3__value'
	IL_0035:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'value'
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>3__repeatCount'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::repeatCount
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <Repeat>d__b3`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 526
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x57a4
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Repeat>d__b3`1::System.Collections.IEnumerable.GetEnumerator

    // method line 527
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x57ac
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 97 (0x61)
	.maxstack 3
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  switch (
	  IL_0017,
	  IL_003c)
	IL_0015:  br.s IL_005f

	IL_0017:  ldarg.0 
	IL_0018:  ldc.i4.m1 
	IL_0019:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_001e:  ldarg.0 
	IL_001f:  ldc.i4.0 
	IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<i>5__b4'
	IL_0025:  br.s IL_0051

	IL_0027:  ldarg.0 
	IL_0028:  ldarg.0 
	IL_0029:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'value'
	IL_002e:  stfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>2__current'
	IL_0033:  ldarg.0 
	IL_0034:  ldc.i4.1 
	IL_0035:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_003a:  ldc.i4.1 
	IL_003b:  ret 
	IL_003c:  ldarg.0 
	IL_003d:  ldc.i4.m1 
	IL_003e:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_0043:  ldarg.0 
	IL_0044:  dup 
	IL_0045:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<i>5__b4'
	IL_004a:  ldc.i4.1 
	IL_004b:  add 
	IL_004c:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<i>5__b4'
	IL_0051:  ldarg.0 
	IL_0052:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<i>5__b4'
	IL_0057:  ldarg.0 
	IL_0058:  ldfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::repeatCount
	IL_005d:  blt.s IL_0027

	IL_005f:  ldc.i4.0 
	IL_0060:  ret 
    } // end of method <Repeat>d__b3`1::MoveNext

    // method line 528
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5819
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Repeat>d__b3`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 529
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5821
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Repeat>d__b3`1::System.Collections.IEnumerator.Reset

    // method line 530
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5828
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method <Repeat>d__b3`1::System.IDisposable.Dispose

    // method line 531
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x582a
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Repeat>d__b3`1::System.Collections.IEnumerator.get_Current

    // method line 532
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5837
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Repeat>d__b3`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Repeat>d__b3`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Repeat>d__b3`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Repeat>d__b3`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Repeat>d__b3`1

  .class nested private auto ansi sealed beforefieldinit '<RepeatHelper>d__b7`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  !0 '<item>5__b8'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrapb9'

    // method line 533
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5894
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <RepeatHelper>d__b7`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 534
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x58dc
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <RepeatHelper>d__b7`1::System.Collections.IEnumerable.GetEnumerator

    // method line 535
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x58e4
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 141 (0x8d)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_001b,
	    IL_0080,
	    IL_0064)
	  IL_0019:  br.s IL_0080

	  IL_001b:  ldarg.0 
	  IL_001c:  ldc.i4.m1 
	  IL_001d:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	  IL_0022:  ldarg.0 
	  IL_0023:  ldarg.0 
	  IL_0024:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::source
	  IL_0029:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_002e:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>7__wrapb9'
	  IL_0033:  ldarg.0 
	  IL_0034:  ldc.i4.1 
	  IL_0035:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	  IL_003a:  br.s IL_006b

	  IL_003c:  ldarg.0 
	  IL_003d:  ldarg.0 
	  IL_003e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>7__wrapb9'
	  IL_0043:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0048:  stfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<item>5__b8'
	  IL_004d:  ldarg.0 
	  IL_004e:  ldarg.0 
	  IL_004f:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<item>5__b8'
	  IL_0054:  stfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>2__current'
	  IL_0059:  ldarg.0 
	  IL_005a:  ldc.i4.2 
	  IL_005b:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	  IL_0060:  ldc.i4.1 
	  IL_0061:  stloc.0 
	  IL_0062:  leave.s IL_008b

	  IL_0064:  ldarg.0 
	  IL_0065:  ldc.i4.1 
	  IL_0066:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	  IL_006b:  ldarg.0 
	  IL_006c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>7__wrapb9'
	  IL_0071:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0076:  brtrue.s IL_003c

	  IL_0078:  ldarg.0 
	  IL_0079:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>::'<>m__Finallyba'()
	  IL_007e:  br.s IL_0022

	  IL_0080:  ldc.i4.0 
	  IL_0081:  stloc.0 
	  IL_0082:  leave.s IL_008b

	} // end .try 0
	fault  { // 0
	  IL_0084:  ldarg.0 
	  IL_0085:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>::System.IDisposable.Dispose()
	  IL_008a:  endfault
	} // end handler 0
	IL_008b:  ldloc.0 
	IL_008c:  ret 
    } // end of method <RepeatHelper>d__b7`1::MoveNext

    // method line 536
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5990
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <RepeatHelper>d__b7`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 537
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5998
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <RepeatHelper>d__b7`1::System.Collections.IEnumerator.Reset

    // method line 538
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x59a0
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 34 (0x22)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0018,
	  IL_0018)
	IL_0017:  ret 
	.try { // 0
	  IL_0018:  leave.s IL_0021

	} // end .try 0
	finally  { // 0
	  IL_001a:  ldarg.0 
	  IL_001b:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!TSource>::'<>m__Finallyba'()
	  IL_0020:  endfinally 
	} // end handler 0
	IL_0021:  ret 
    } // end of method <RepeatHelper>d__b7`1::System.IDisposable.Dispose

    // method line 539
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x59e0
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <RepeatHelper>d__b7`1::System.Collections.IEnumerator.get_Current

    // method line 540
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x59ed
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <RepeatHelper>d__b7`1::.ctor

    // method line 541
    .method private hidebysig 
           instance default void '<>m__Finallyba' ()  cil managed 
    {
        // Method begins at RVA 0x5a0c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>7__wrapb9'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'<!0>::'<>7__wrapb9'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <RepeatHelper>d__b7`1::<>m__Finallyba

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<RepeatHelper>d__b7`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <RepeatHelper>d__b7`1

  .class nested private auto ansi sealed beforefieldinit '<RepeatHelper>d__bd`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  int32 repeatCount
    .field  public  int32 '<>3__repeatCount'
    .field  public  int32 '<i>5__be'
    .field  public  !0 '<item>5__bf'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrapc0'

    // method line 542
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5a5c
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>3__repeatCount'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::repeatCount
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <RepeatHelper>d__bd`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 543
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5ab0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <RepeatHelper>d__bd`1::System.Collections.IEnumerable.GetEnumerator

    // method line 544
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x5ab8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 179 (0xb3)
	.maxstack 3
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_001e,
	    IL_00a6,
	    IL_0070)
	  IL_0019:  br IL_00a6

	  IL_001e:  ldarg.0 
	  IL_001f:  ldc.i4.m1 
	  IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	  IL_0025:  ldarg.0 
	  IL_0026:  ldc.i4.0 
	  IL_0027:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<i>5__be'
	  IL_002c:  br.s IL_0098

	  IL_002e:  ldarg.0 
	  IL_002f:  ldarg.0 
	  IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::source
	  IL_0035:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_003a:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>7__wrapc0'
	  IL_003f:  ldarg.0 
	  IL_0040:  ldc.i4.1 
	  IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	  IL_0046:  br.s IL_0077

	  IL_0048:  ldarg.0 
	  IL_0049:  ldarg.0 
	  IL_004a:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>7__wrapc0'
	  IL_004f:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0054:  stfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<item>5__bf'
	  IL_0059:  ldarg.0 
	  IL_005a:  ldarg.0 
	  IL_005b:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<item>5__bf'
	  IL_0060:  stfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>2__current'
	  IL_0065:  ldarg.0 
	  IL_0066:  ldc.i4.2 
	  IL_0067:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	  IL_006c:  ldc.i4.1 
	  IL_006d:  stloc.0 
	  IL_006e:  leave.s IL_00b1

	  IL_0070:  ldarg.0 
	  IL_0071:  ldc.i4.1 
	  IL_0072:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	  IL_0077:  ldarg.0 
	  IL_0078:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>7__wrapc0'
	  IL_007d:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0082:  brtrue.s IL_0048

	  IL_0084:  ldarg.0 
	  IL_0085:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>::'<>m__Finallyc1'()
	  IL_008a:  ldarg.0 
	  IL_008b:  dup 
	  IL_008c:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<i>5__be'
	  IL_0091:  ldc.i4.1 
	  IL_0092:  add 
	  IL_0093:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<i>5__be'
	  IL_0098:  ldarg.0 
	  IL_0099:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<i>5__be'
	  IL_009e:  ldarg.0 
	  IL_009f:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::repeatCount
	  IL_00a4:  blt.s IL_002e

	  IL_00a6:  ldc.i4.0 
	  IL_00a7:  stloc.0 
	  IL_00a8:  leave.s IL_00b1

	} // end .try 0
	fault  { // 0
	  IL_00aa:  ldarg.0 
	  IL_00ab:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00b0:  endfault
	} // end handler 0
	IL_00b1:  ldloc.0 
	IL_00b2:  ret 
    } // end of method <RepeatHelper>d__bd`1::MoveNext

    // method line 545
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5b88
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <RepeatHelper>d__bd`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 546
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5b90
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <RepeatHelper>d__bd`1::System.Collections.IEnumerator.Reset

    // method line 547
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5b98
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 34 (0x22)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0018,
	  IL_0018)
	IL_0017:  ret 
	.try { // 0
	  IL_0018:  leave.s IL_0021

	} // end .try 0
	finally  { // 0
	  IL_001a:  ldarg.0 
	  IL_001b:  call instance void class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!TSource>::'<>m__Finallyc1'()
	  IL_0020:  endfinally 
	} // end handler 0
	IL_0021:  ret 
    } // end of method <RepeatHelper>d__bd`1::System.IDisposable.Dispose

    // method line 548
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5bd8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <RepeatHelper>d__bd`1::System.Collections.IEnumerator.get_Current

    // method line 549
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5be5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <RepeatHelper>d__bd`1::.ctor

    // method line 550
    .method private hidebysig 
           instance default void '<>m__Finallyc1' ()  cil managed 
    {
        // Method begins at RVA 0x5c04
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>7__wrapc0'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'<!0>::'<>7__wrapc0'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <RepeatHelper>d__bd`1::<>m__Finallyc1

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<RepeatHelper>d__bd`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <RepeatHelper>d__bd`1

  .class nested private auto ansi sealed beforefieldinit '<Retry>d__c4`1'<TValue>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<enumerator>5__c5'

    // method line 551
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue> 'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5c44
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TValue>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <Retry>d__c4`1::System.Collections.Generic.IEnumerable<TValue>.GetEnumerator

    // method line 552
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5c8c
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Retry>d__c4`1::System.Collections.IEnumerable.GetEnumerator

    // method line 553
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x5c94
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 151 (0x97)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0011

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq.s IL_0079

	  IL_000f:  br.s IL_008a

	  IL_0011:  ldarg.0 
	  IL_0012:  ldc.i4.m1 
	  IL_0013:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	  IL_0018:  ldarg.0 
	  IL_0019:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::source
	  IL_001e:  brtrue.s IL_002b

	  IL_0020:  ldstr "source"
	  IL_0025:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	  IL_002a:  throw 
	  IL_002b:  ldarg.0 
	  IL_002c:  ldarg.0 
	  IL_002d:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::source
	  IL_0032:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TValue>::GetEnumerator()
	  IL_0037:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<enumerator>5__c5'
	  IL_003c:  ldarg.0 
	  IL_003d:  ldc.i4.1 
	  IL_003e:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	  .try { // 0
	    IL_0043:  ldarg.0 
	    IL_0044:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<enumerator>5__c5'
	    IL_0049:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_004e:  brtrue.s IL_0058

	    IL_0050:  ldarg.0 
	    IL_0051:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::System.IDisposable.Dispose()
	    IL_0056:  leave.s IL_008a

	    IL_0058:  leave.s IL_005d

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_005a:  pop 
	    IL_005b:  leave.s IL_0082

	  } // end handler 0
	  IL_005d:  ldarg.0 
	  IL_005e:  ldarg.0 
	  IL_005f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<enumerator>5__c5'
	  IL_0064:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue>::get_Current()
	  IL_0069:  stfld !0 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>2__current'
	  IL_006e:  ldarg.0 
	  IL_006f:  ldc.i4.3 
	  IL_0070:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	  IL_0075:  ldc.i4.1 
	  IL_0076:  stloc.0 
	  IL_0077:  leave.s IL_0095

	  IL_0079:  ldarg.0 
	  IL_007a:  ldc.i4.1 
	  IL_007b:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	  IL_0080:  br.s IL_0043

	  IL_0082:  ldarg.0 
	  IL_0083:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::'<>m__Finallyc6'()
	  IL_0088:  br.s IL_002b

	  IL_008a:  ldc.i4.0 
	  IL_008b:  stloc.0 
	  IL_008c:  leave.s IL_0095

	} // end .try 1
	fault  { // 1
	  IL_008e:  ldarg.0 
	  IL_008f:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::System.IDisposable.Dispose()
	  IL_0094:  endfault
	} // end handler 1
	IL_0095:  ldloc.0 
	IL_0096:  ret 
    } // end of method <Retry>d__c4`1::MoveNext

    // method line 554
    .method private final virtual hidebysig newslot specialname 
           instance default !TValue 'System.Collections.Generic.IEnumerator<TValue>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5d54
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Retry>d__c4`1::System.Collections.Generic.IEnumerator<TValue>.get_Current

    // method line 555
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5d5c
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Retry>d__c4`1::System.Collections.IEnumerator.Reset

    // method line 556
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5d64
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_0025,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!TValue>::'<>m__Finallyc6'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <Retry>d__c4`1::System.IDisposable.Dispose

    // method line 557
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5da8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Retry>d__c4`1::System.Collections.IEnumerator.get_Current

    // method line 558
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5db5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Retry>d__c4`1::.ctor

    // method line 559
    .method private hidebysig 
           instance default void '<>m__Finallyc6' ()  cil managed 
    {
        // Method begins at RVA 0x5dd4
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<enumerator>5__c5'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c4`1'<!0>::'<enumerator>5__c5'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <Retry>d__c4`1::<>m__Finallyc6

	.property instance !TValue 'System.Collections.Generic.IEnumerator<TValue>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Retry>d__c4`1'::'System.Collections.Generic.IEnumerator<TValue>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Retry>d__c4`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Retry>d__c4`1

  .class nested private auto ansi sealed beforefieldinit '<Retry>d__c9`1'<TValue>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  int32 retryCount
    .field  public  int32 '<>3__retryCount'
    .field  public  int32 '<i>5__ca'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<enumerator>5__cb'

    // method line 560
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue> 'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5e10
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TValue>::GetEnumerator()
	// Code size 72 (0x48)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>3__retryCount'
	IL_0041:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::retryCount
	IL_0046:  ldloc.0 
	IL_0047:  ret 
    } // end of method <Retry>d__c9`1::System.Collections.Generic.IEnumerable<TValue>.GetEnumerator

    // method line 561
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5e64
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::'System.Collections.Generic.IEnumerable<TValue>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <Retry>d__c9`1::System.Collections.IEnumerable.GetEnumerator

    // method line 562
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x5e6c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 189 (0xbd)
	.maxstack 3
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 1
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  ldc.i4.0 
	  IL_0009:  beq.s IL_0014

	  IL_000b:  ldloc.1 
	  IL_000c:  ldc.i4.3 
	  IL_000d:  beq.s IL_0085

	  IL_000f:  br IL_00b0

	  IL_0014:  ldarg.0 
	  IL_0015:  ldc.i4.m1 
	  IL_0016:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	  IL_001b:  ldarg.0 
	  IL_001c:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::source
	  IL_0021:  brtrue.s IL_002e

	  IL_0023:  ldstr "source"
	  IL_0028:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	  IL_002d:  throw 
	  IL_002e:  ldarg.0 
	  IL_002f:  ldc.i4.0 
	  IL_0030:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<i>5__ca'
	  IL_0035:  br.s IL_00a2

	  IL_0037:  ldarg.0 
	  IL_0038:  ldarg.0 
	  IL_0039:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::source
	  IL_003e:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TValue>::GetEnumerator()
	  IL_0043:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<enumerator>5__cb'
	  IL_0048:  ldarg.0 
	  IL_0049:  ldc.i4.1 
	  IL_004a:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	  .try { // 0
	    IL_004f:  ldarg.0 
	    IL_0050:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<enumerator>5__cb'
	    IL_0055:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	    IL_005a:  brtrue.s IL_0064

	    IL_005c:  ldarg.0 
	    IL_005d:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::System.IDisposable.Dispose()
	    IL_0062:  leave.s IL_00b0

	    IL_0064:  leave.s IL_0069

	  } // end .try 0
	  catch class [mscorlib]System.Exception { // 0
	    IL_0066:  pop 
	    IL_0067:  leave.s IL_008e

	  } // end handler 0
	  IL_0069:  ldarg.0 
	  IL_006a:  ldarg.0 
	  IL_006b:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<enumerator>5__cb'
	  IL_0070:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue>::get_Current()
	  IL_0075:  stfld !0 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>2__current'
	  IL_007a:  ldarg.0 
	  IL_007b:  ldc.i4.3 
	  IL_007c:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	  IL_0081:  ldc.i4.1 
	  IL_0082:  stloc.0 
	  IL_0083:  leave.s IL_00bb

	  IL_0085:  ldarg.0 
	  IL_0086:  ldc.i4.1 
	  IL_0087:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	  IL_008c:  br.s IL_004f

	  IL_008e:  ldarg.0 
	  IL_008f:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::'<>m__Finallycc'()
	  IL_0094:  ldarg.0 
	  IL_0095:  dup 
	  IL_0096:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<i>5__ca'
	  IL_009b:  ldc.i4.1 
	  IL_009c:  add 
	  IL_009d:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<i>5__ca'
	  IL_00a2:  ldarg.0 
	  IL_00a3:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<i>5__ca'
	  IL_00a8:  ldarg.0 
	  IL_00a9:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::retryCount
	  IL_00ae:  blt.s IL_0037

	  IL_00b0:  ldc.i4.0 
	  IL_00b1:  stloc.0 
	  IL_00b2:  leave.s IL_00bb

	} // end .try 1
	fault  { // 1
	  IL_00b4:  ldarg.0 
	  IL_00b5:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::System.IDisposable.Dispose()
	  IL_00ba:  endfault
	} // end handler 1
	IL_00bb:  ldloc.0 
	IL_00bc:  ret 
    } // end of method <Retry>d__c9`1::MoveNext

    // method line 563
    .method private final virtual hidebysig newslot specialname 
           instance default !TValue 'System.Collections.Generic.IEnumerator<TValue>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5f54
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TValue>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <Retry>d__c9`1::System.Collections.Generic.IEnumerator<TValue>.get_Current

    // method line 564
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5f5c
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <Retry>d__c9`1::System.Collections.IEnumerator.Reset

    // method line 565
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x5f64
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 38 (0x26)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_001c,
	  IL_0025,
	  IL_001c)
	IL_001b:  ret 
	.try { // 0
	  IL_001c:  leave.s IL_0025

	} // end .try 0
	finally  { // 0
	  IL_001e:  ldarg.0 
	  IL_001f:  call instance void class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!TValue>::'<>m__Finallycc'()
	  IL_0024:  endfinally 
	} // end handler 0
	IL_0025:  ret 
    } // end of method <Retry>d__c9`1::System.IDisposable.Dispose

    // method line 566
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5fa8
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <Retry>d__c9`1::System.Collections.IEnumerator.get_Current

    // method line 567
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x5fb5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <Retry>d__c9`1::.ctor

    // method line 568
    .method private hidebysig 
           instance default void '<>m__Finallycc' ()  cil managed 
    {
        // Method begins at RVA 0x5fd4
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<enumerator>5__cb'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<Retry>d__c9`1'<!0>::'<enumerator>5__cb'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <Retry>d__c9`1::<>m__Finallycc

	.property instance !TValue 'System.Collections.Generic.IEnumerator<TValue>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<Retry>d__c9`1'::'System.Collections.Generic.IEnumerator<TValue>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<Retry>d__c9`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <Retry>d__c9`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClassd1`2'<TSource,TKey>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IComparer`1<!1> comparer

    // method line 569
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x6072
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClassd1`2::.ctor

    // method line 570
    .method public hidebysig 
           instance default bool '<MinBy>b__d0' (!TKey key, !TKey minValue)  cil managed 
    {
        // Method begins at RVA 0x607a
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd1`2'<!0,!1>::comparer
	IL_0006:  ldarg.1 
	IL_0007:  ldarg.2 
	IL_0008:  callvirt instance int32 class [mscorlib]System.Collections.Generic.IComparer`1<!TKey>::Compare(!0, !0)
	IL_000d:  ldc.i4.0 
	IL_000e:  clt 
	IL_0010:  ret 
    } // end of method <>c__DisplayClassd1`2::<MinBy>b__d0

  } // end of class <>c__DisplayClassd1`2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClassd5`2'<TSource,TKey>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IComparer`1<!1> comparer

    // method line 571
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x6146
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClassd5`2::.ctor

    // method line 572
    .method public hidebysig 
           instance default bool '<MaxBy>b__d4' (!TKey key, !TKey minValue)  cil managed 
    {
        // Method begins at RVA 0x614e
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClassd5`2'<!0,!1>::comparer
	IL_0006:  ldarg.1 
	IL_0007:  ldarg.2 
	IL_0008:  callvirt instance int32 class [mscorlib]System.Collections.Generic.IComparer`1<!TKey>::Compare(!0, !0)
	IL_000d:  ldc.i4.0 
	IL_000e:  cgt 
	IL_0010:  ret 
    } // end of method <>c__DisplayClassd5`2::<MaxBy>b__d4

  } // end of class <>c__DisplayClassd5`2

  .class nested private auto ansi sealed beforefieldinit '<IsEmptyEnumerableHelper>d__d7`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<bool>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<bool>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  bool '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> '<>7__wrapd8'

    // method line 573
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<bool> 'System.Collections.Generic.IEnumerable<System.Boolean>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6578
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<bool>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.Collections.Generic.IEnumerable<System.Boolean>.GetEnumerator

    // method line 574
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x65c0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<bool> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::'System.Collections.Generic.IEnumerable<System.Boolean>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.Collections.IEnumerable.GetEnumerator

    // method line 575
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x65c8
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 166 (0xa6)
	.maxstack 2
	.locals init (
		bool	V_0,
		int32	V_1)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_0006:  stloc.1 
	  IL_0007:  ldloc.1 
	  IL_0008:  switch (
	    IL_001f,
	    IL_0099,
	    IL_005e,
	    IL_0092)
	  IL_001d:  br.s IL_0099

	  IL_001f:  ldarg.0 
	  IL_0020:  ldc.i4.m1 
	  IL_0021:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_0026:  ldarg.0 
	  IL_0027:  ldarg.0 
	  IL_0028:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::source
	  IL_002d:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	  IL_0032:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>7__wrapd8'
	  IL_0037:  ldarg.0 
	  IL_0038:  ldc.i4.1 
	  IL_0039:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_003e:  br.s IL_006d

	  IL_0040:  ldarg.0 
	  IL_0041:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>7__wrapd8'
	  IL_0046:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_004b:  pop 
	  IL_004c:  ldarg.0 
	  IL_004d:  ldc.i4.0 
	  IL_004e:  stfld bool class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>2__current'
	  IL_0053:  ldarg.0 
	  IL_0054:  ldc.i4.2 
	  IL_0055:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_005a:  ldc.i4.1 
	  IL_005b:  stloc.0 
	  IL_005c:  leave.s IL_00a4

	  IL_005e:  ldarg.0 
	  IL_005f:  ldc.i4.1 
	  IL_0060:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_0065:  ldarg.0 
	  IL_0066:  call instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::System.IDisposable.Dispose()
	  IL_006b:  leave.s IL_0099

	  IL_006d:  ldarg.0 
	  IL_006e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>7__wrapd8'
	  IL_0073:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0078:  brtrue.s IL_0040

	  IL_007a:  ldarg.0 
	  IL_007b:  call instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::'<>m__Finallyd9'()
	  IL_0080:  ldarg.0 
	  IL_0081:  ldc.i4.1 
	  IL_0082:  stfld bool class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>2__current'
	  IL_0087:  ldarg.0 
	  IL_0088:  ldc.i4.3 
	  IL_0089:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_008e:  ldc.i4.1 
	  IL_008f:  stloc.0 
	  IL_0090:  leave.s IL_00a4

	  IL_0092:  ldarg.0 
	  IL_0093:  ldc.i4.m1 
	  IL_0094:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	  IL_0099:  ldc.i4.0 
	  IL_009a:  stloc.0 
	  IL_009b:  leave.s IL_00a4

	} // end .try 0
	fault  { // 0
	  IL_009d:  ldarg.0 
	  IL_009e:  call instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::System.IDisposable.Dispose()
	  IL_00a3:  endfault
	} // end handler 0
	IL_00a4:  ldloc.0 
	IL_00a5:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::MoveNext

    // method line 576
    .method private final virtual hidebysig newslot specialname 
           instance default bool 'System.Collections.Generic.IEnumerator<System.Boolean>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x668c
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<bool>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld bool class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.Collections.Generic.IEnumerator<System.Boolean>.get_Current

    // method line 577
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x6694
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.Collections.IEnumerator.Reset

    // method line 578
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x669c
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 34 (0x22)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldc.i4.1 
	IL_0009:  sub 
	IL_000a:  switch (
	  IL_0018,
	  IL_0018)
	IL_0017:  ret 
	.try { // 0
	  IL_0018:  leave.s IL_0021

	} // end .try 0
	finally  { // 0
	  IL_001a:  ldarg.0 
	  IL_001b:  call instance void class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!TSource>::'<>m__Finallyd9'()
	  IL_0020:  endfinally 
	} // end handler 0
	IL_0021:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.IDisposable.Dispose

    // method line 579
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x66dc
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld bool class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>2__current'
	IL_0006:  box [mscorlib]System.Boolean
	IL_000b:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::System.Collections.IEnumerator.get_Current

    // method line 580
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x66e9
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::.ctor

    // method line 581
    .method private hidebysig 
           instance default void '<>m__Finallyd9' ()  cil managed 
    {
        // Method begins at RVA 0x6708
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.m1 
	IL_0002:  stfld int32 class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>1__state'
	IL_0007:  ldarg.0 
	IL_0008:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>7__wrapd8'
	IL_000d:  brfalse.s IL_001a

	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'<!0>::'<>7__wrapd8'
	IL_0015:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_001a:  ret 
    } // end of method <IsEmptyEnumerableHelper>d__d7`1::<>m__Finallyd9

	.property instance bool 'System.Collections.Generic.IEnumerator<System.Boolean>.Current' ()
	{
		.get instance default bool System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'::'System.Collections.Generic.IEnumerator<System.Boolean>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<IsEmptyEnumerableHelper>d__d7`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <IsEmptyEnumerableHelper>d__d7`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClassde`1'<TSource>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [System.Core]System.Func`2<!0,bool> predicate

    // method line 582
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x6798
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClassde`1::.ctor

    // method line 583
    .method public hidebysig 
           instance default bool '<AllEnumerable>b__dd' (!TSource v)  cil managed 
    {
        // Method begins at RVA 0x67a0
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Func`2<!0,bool> class System.Linq.EnumerableEx/'<>c__DisplayClassde`1'<!0>::predicate
	IL_0006:  ldarg.1 
	IL_0007:  callvirt instance !1 class [System.Core]System.Func`2<!TSource, bool>::Invoke(!0)
	IL_000c:  ldc.i4.0 
	IL_000d:  ceq 
	IL_000f:  ret 
    } // end of method <>c__DisplayClassde`1::<AllEnumerable>b__dd

  } // end of class <>c__DisplayClassde`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClasse1`1'<TSource>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  !0 'value'
    .field  public  class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!0> comparer

    // method line 584
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x6806
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClasse1`1::.ctor

    // method line 585
    .method public hidebysig 
           instance default bool '<ContainsEnumerable>b__e0' (!TSource v)  cil managed 
    {
        // Method begins at RVA 0x680e
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!0> class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!0>::comparer
	IL_0006:  ldarg.1 
	IL_0007:  ldarg.0 
	IL_0008:  ldfld !0 class System.Linq.EnumerableEx/'<>c__DisplayClasse1`1'<!0>::'value'
	IL_000d:  callvirt instance bool class [mscorlib]System.Collections.Generic.IEqualityComparer`1<!TSource>::Equals(!0, !0)
	IL_0012:  ret 
    } // end of method <>c__DisplayClasse1`1::<ContainsEnumerable>b__e0

  } // end of class <>c__DisplayClasse1`1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass104`2'<TSource,TKey>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [mscorlib]System.Collections.Generic.IComparer`1<!1> comparer

    // method line 586
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x6cac
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass104`2::.ctor

    // method line 587
    .method public hidebysig 
           instance default int32 '<MinByEnumerable>b__103' (!TKey x, !TKey y)  cil managed 
    {
        // Method begins at RVA 0x6cb4
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<>c__DisplayClass104`2'<!0,!1>::comparer
	IL_0006:  ldarg.1 
	IL_0007:  ldarg.2 
	IL_0008:  callvirt instance int32 class [mscorlib]System.Collections.Generic.IComparer`1<!TKey>::Compare(!0, !0)
	IL_000d:  ldc.i4.m1 
	IL_000e:  mul 
	IL_000f:  ret 
    } // end of method <>c__DisplayClass104`2::<MinByEnumerable>b__103

  } // end of class <>c__DisplayClass104`2

  .class nested private auto ansi sealed beforefieldinit '<ExtremaByEnumerableHelper>d__10e`2'<TSource,TKey>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!0>>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!0>>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  class [mscorlib]System.Collections.Generic.IList`1<!0> '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  class [System.Core]System.Func`2<!0,!1> keySelector
    .field  public  class [System.Core]System.Func`2<!0,!1> '<>3__keySelector'
    .field  public  class [mscorlib]System.Collections.Generic.IComparer`1<!1> comparer
    .field  public  class [mscorlib]System.Collections.Generic.IComparer`1<!1> '<>3__comparer'
    .field  public  bool '<hasValue>5__10f'
    .field  public  !1 '<lastKey>5__110'
    .field  public  class [mscorlib]System.Collections.Generic.List`1<!0> '<list>5__111'

    // method line 588
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>> 'System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x72f4
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>>::GetEnumerator()
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!TSource, !TKey>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!TSource, !TKey>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::source
	IL_003a:  ldloc.0 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>3__keySelector'
	IL_0041:  stfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::keySelector
	IL_0046:  ldloc.0 
	IL_0047:  ldarg.0 
	IL_0048:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>3__comparer'
	IL_004d:  stfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::comparer
	IL_0052:  ldloc.0 
	IL_0053:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator

    // method line 589
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7354
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!0>> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!TSource, !TKey>::'System.Collections.Generic.IEnumerable<System.Collections.Generic.IList<TSource>>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.Collections.IEnumerable.GetEnumerator

    // method line 590
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x735c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 239 (0xef)
	.maxstack 3
	.locals init (
		!TSource	V_0,
		!TKey	V_1,
		int32	V_2,
		int32	V_3,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>	V_4)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_0006:  stloc.3 
	IL_0007:  ldloc.3 
	IL_0008:  switch (
	  IL_001e,
	  IL_00ed,
	  IL_00e6)
	IL_0019:  br IL_00ed

	IL_001e:  ldarg.0 
	IL_001f:  ldc.i4.m1 
	IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_0025:  ldarg.0 
	IL_0026:  ldc.i4.0 
	IL_0027:  stfld bool class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<hasValue>5__10f'
	IL_002c:  ldarg.0 
	IL_002d:  ldflda !1 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<lastKey>5__110'
	IL_0032:  initobj !1
	IL_0038:  ldarg.0 
	IL_0039:  newobj instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::'.ctor'()
	IL_003e:  stfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<list>5__111'
	IL_0043:  ldarg.0 
	IL_0044:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::source
	IL_0049:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	IL_004e:  stloc.s 4
	.try { // 0
	  IL_0050:  br.s IL_00ba

	  IL_0052:  ldloc.s 4
	  IL_0054:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_0059:  stloc.0 
	  IL_005a:  ldarg.0 
	  IL_005b:  ldfld class [System.Core]System.Func`2<!0,!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::keySelector
	  IL_0060:  ldloc.0 
	  IL_0061:  callvirt instance !1 class [System.Core]System.Func`2<!TSource, !TKey>::Invoke(!0)
	  IL_0066:  stloc.1 
	  IL_0067:  ldc.i4.0 
	  IL_0068:  stloc.2 
	  IL_0069:  ldarg.0 
	  IL_006a:  ldfld bool class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<hasValue>5__10f'
	  IL_006f:  brtrue.s IL_0081

	  IL_0071:  ldarg.0 
	  IL_0072:  ldc.i4.1 
	  IL_0073:  stfld bool class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<hasValue>5__10f'
	  IL_0078:  ldarg.0 
	  IL_0079:  ldloc.1 
	  IL_007a:  stfld !1 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<lastKey>5__110'
	  IL_007f:  br.s IL_0094

	  IL_0081:  ldarg.0 
	  IL_0082:  ldfld class [mscorlib]System.Collections.Generic.IComparer`1<!1> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::comparer
	  IL_0087:  ldloc.1 
	  IL_0088:  ldarg.0 
	  IL_0089:  ldfld !1 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<lastKey>5__110'
	  IL_008e:  callvirt instance int32 class [mscorlib]System.Collections.Generic.IComparer`1<!TKey>::Compare(!0, !0)
	  IL_0093:  stloc.2 
	  IL_0094:  ldloc.2 
	  IL_0095:  ldc.i4.0 
	  IL_0096:  ble.s IL_00aa

	  IL_0098:  ldarg.0 
	  IL_0099:  ldloc.1 
	  IL_009a:  stfld !1 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<lastKey>5__110'
	  IL_009f:  ldarg.0 
	  IL_00a0:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<list>5__111'
	  IL_00a5:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::Clear()
	  IL_00aa:  ldloc.2 
	  IL_00ab:  ldc.i4.0 
	  IL_00ac:  blt.s IL_00ba

	  IL_00ae:  ldarg.0 
	  IL_00af:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<list>5__111'
	  IL_00b4:  ldloc.0 
	  IL_00b5:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<!TSource>::Add(!0)
	  IL_00ba:  ldloc.s 4
	  IL_00bc:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_00c1:  brtrue.s IL_0052

	  IL_00c3:  leave.s IL_00d1

	} // end .try 0
	finally  { // 0
	  IL_00c5:  ldloc.s 4
	  IL_00c7:  brfalse.s IL_00d0

	  IL_00c9:  ldloc.s 4
	  IL_00cb:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_00d0:  endfinally 
	} // end handler 0
	IL_00d1:  ldarg.0 
	IL_00d2:  ldarg.0 
	IL_00d3:  ldfld class [mscorlib]System.Collections.Generic.List`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<list>5__111'
	IL_00d8:  stfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>2__current'
	IL_00dd:  ldarg.0 
	IL_00de:  ldc.i4.2 
	IL_00df:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_00e4:  ldc.i4.1 
	IL_00e5:  ret 
	IL_00e6:  ldarg.0 
	IL_00e7:  ldc.i4.m1 
	IL_00e8:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_00ed:  ldc.i4.0 
	IL_00ee:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::MoveNext

    // method line 591
    .method private final virtual hidebysig newslot specialname 
           instance default class [mscorlib]System.Collections.Generic.IList`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7468
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Collections.Generic.IList`1<!TSource>>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current

    // method line 592
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7470
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.Collections.IEnumerator.Reset

    // method line 593
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x7478
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 0
	.locals init (
		!TSource	V_0,
		!TKey	V_1,
		int32	V_2)
	IL_0000:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.IDisposable.Dispose

    // method line 594
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7485
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IList`1<!0> class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>2__current'
	IL_0006:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::System.Collections.IEnumerator.get_Current

    // method line 595
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x748d
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'<!0,!1>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <ExtremaByEnumerableHelper>d__10e`2::.ctor

	.property instance class [mscorlib]System.Collections.Generic.IList`1<!TSource> 'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.Current' ()
	{
		.get instance default class [mscorlib]System.Collections.Generic.IList`1<!0> System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'::'System.Collections.Generic.IEnumerator<System.Collections.Generic.IList<TSource>>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<ExtremaByEnumerableHelper>d__10e`2'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <ExtremaByEnumerableHelper>d__10e`2

  .class nested private auto ansi sealed beforefieldinit '<FinalHelper>d__13c`1'<TSource>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.Collections.IEnumerator, [mscorlib]System.IDisposable  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<>2__current'
    .field  private  int32 '<>1__state'
    .field  private  int32 '<>l__initialThreadId'
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> source
    .field  public  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> '<>3__source'
    .field  public  !0 '<value>5__13d'
    .field  public  bool '<hasValue>5__13e'

    // method line 602
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource> 'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7f0c
	.override method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	// Code size 60 (0x3c)
	.maxstack 2
	.locals init (
		class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!TSource>	V_0)
	IL_0000:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0005:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_000a:  ldarg.0 
	IL_000b:  ldfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>l__initialThreadId'
	IL_0010:  bne.un.s IL_0027

	IL_0012:  ldarg.0 
	IL_0013:  ldfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_0018:  ldc.i4.s 0xfffffffe
	IL_001a:  bne.un.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldc.i4.0 
	IL_001e:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_0023:  ldarg.0 
	IL_0024:  stloc.0 
	IL_0025:  br.s IL_002e

	IL_0027:  ldc.i4.0 
	IL_0028:  newobj instance void class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!TSource>::'.ctor'(int32)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.0 
	IL_002f:  ldarg.0 
	IL_0030:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>3__source'
	IL_0035:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::source
	IL_003a:  ldloc.0 
	IL_003b:  ret 
    } // end of method <FinalHelper>d__13c`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator

    // method line 603
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7f54
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!TSource>::'System.Collections.Generic.IEnumerable<TSource>.GetEnumerator'()
	IL_0006:  ret 
    } // end of method <FinalHelper>d__13c`1::System.Collections.IEnumerable.GetEnumerator

    // method line 604
    .method private final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x7f5c
	.override class [mscorlib]System.Collections.IEnumerator::MoveNext
	// Code size 160 (0xa0)
	.maxstack 2
	.locals init (
		!TSource	V_0,
		int32	V_1,
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_0006:  stloc.1 
	IL_0007:  ldloc.1 
	IL_0008:  switch (
	  IL_001e,
	  IL_009e,
	  IL_0097)
	IL_0019:  br IL_009e

	IL_001e:  ldarg.0 
	IL_001f:  ldc.i4.m1 
	IL_0020:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_0025:  ldarg.0 
	IL_0026:  ldflda !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<value>5__13d'
	IL_002b:  initobj !0
	IL_0031:  ldarg.0 
	IL_0032:  ldc.i4.0 
	IL_0033:  stfld bool class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<hasValue>5__13e'
	IL_0038:  ldarg.0 
	IL_0039:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::source
	IL_003e:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TSource>::GetEnumerator()
	IL_0043:  stloc.2 
	.try { // 0
	  IL_0044:  br.s IL_005b

	  IL_0046:  ldloc.2 
	  IL_0047:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	  IL_004c:  stloc.0 
	  IL_004d:  ldarg.0 
	  IL_004e:  ldc.i4.1 
	  IL_004f:  stfld bool class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<hasValue>5__13e'
	  IL_0054:  ldarg.0 
	  IL_0055:  ldloc.0 
	  IL_0056:  stfld !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<value>5__13d'
	  IL_005b:  ldloc.2 
	  IL_005c:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0061:  brtrue.s IL_0046

	  IL_0063:  leave.s IL_006f

	} // end .try 0
	finally  { // 0
	  IL_0065:  ldloc.2 
	  IL_0066:  brfalse.s IL_006e

	  IL_0068:  ldloc.2 
	  IL_0069:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_006e:  endfinally 
	} // end handler 0
	IL_006f:  ldarg.0 
	IL_0070:  ldfld bool class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<hasValue>5__13e'
	IL_0075:  brtrue.s IL_0082

	IL_0077:  ldstr "Sequence contains no elements."
	IL_007c:  newobj instance void class [mscorlib]System.InvalidOperationException::'.ctor'(string)
	IL_0081:  throw 
	IL_0082:  ldarg.0 
	IL_0083:  ldarg.0 
	IL_0084:  ldfld !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<value>5__13d'
	IL_0089:  stfld !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>2__current'
	IL_008e:  ldarg.0 
	IL_008f:  ldc.i4.2 
	IL_0090:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_0095:  ldc.i4.1 
	IL_0096:  ret 
	IL_0097:  ldarg.0 
	IL_0098:  ldc.i4.m1 
	IL_0099:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_009e:  ldc.i4.0 
	IL_009f:  ret 
    } // end of method <FinalHelper>d__13c`1::MoveNext

    // method line 605
    .method private final virtual hidebysig newslot specialname 
           instance default !TSource 'System.Collections.Generic.IEnumerator<TSource>.get_Current' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8018
	.override method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!TSource>::get_Current()
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>2__current'
	IL_0006:  ret 
    } // end of method <FinalHelper>d__13c`1::System.Collections.Generic.IEnumerator<TSource>.get_Current

    // method line 606
    .method private final virtual hidebysig newslot 
           instance default void System.Collections.IEnumerator.Reset ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8020
	.override class [mscorlib]System.Collections.IEnumerator::Reset
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'()
	IL_0005:  throw 
    } // end of method <FinalHelper>d__13c`1::System.Collections.IEnumerator.Reset

    // method line 607
    .method private final virtual hidebysig newslot 
           instance default void System.IDisposable.Dispose ()  cil managed 
    {
        // Method begins at RVA 0x8028
	.override class [mscorlib]System.IDisposable::Dispose
	// Code size 1 (0x1)
	.maxstack 0
	.locals init (
		!TSource	V_0)
	IL_0000:  ret 
    } // end of method <FinalHelper>d__13c`1::System.IDisposable.Dispose

    // method line 608
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8035
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>2__current'
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method <FinalHelper>d__13c`1::System.Collections.IEnumerator.get_Current

    // method line 609
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (int32 '<>1__state')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8042
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>1__state'
	IL_000d:  ldarg.0 
	IL_000e:  call class [mscorlib]System.Threading.Thread class [mscorlib]System.Threading.Thread::get_CurrentThread()
	IL_0013:  callvirt instance int32 class [mscorlib]System.Threading.Thread::get_ManagedThreadId()
	IL_0018:  stfld int32 class System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'<!0>::'<>l__initialThreadId'
	IL_001d:  ret 
    } // end of method <FinalHelper>d__13c`1::.ctor

	.property instance !TSource 'System.Collections.Generic.IEnumerator<TSource>.Current' ()
	{
		.get instance default !0 System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'::'System.Collections.Generic.IEnumerator<TSource>.get_Current' () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.EnumerableEx/'<FinalHelper>d__13c`1'::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class <FinalHelper>d__13c`1

  } // end of class System.Linq.EnumerableEx
}

.namespace System.Linq
{
  .class public auto ansi abstract sealed beforefieldinit Enumerator
  	extends [mscorlib]System.Object
  {
    .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 285
    .method public static hidebysig 
           default class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource> AsEnumerator<TSource> (class [mscorlib]System.Collections.Generic.IEnumerator`1<!!TSource> enumerator)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8ac8
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "enumerator"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Linq.Enumerator/HiddenEnumerator`1<!!TSource>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>)
	IL_0014:  ret 
    } // end of method Enumerator::AsEnumerator

  .class nested private auto ansi sealed beforefieldinit HiddenEnumerator`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.IDisposable, [mscorlib]System.Collections.IEnumerator  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> enumerator

    // method line 286
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> enumerator)  cil managed 
    {
        // Method begins at RVA 0x8ade
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_000d:  ret 
    } // end of method HiddenEnumerator`1::.ctor

    // method line 287
    .method public final virtual hidebysig newslot specialname 
           instance default !T get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8aed
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_0006:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	IL_000b:  ret 
    } // end of method HiddenEnumerator`1::get_Current

    // method line 288
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x8afa
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_0006:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_000b:  ret 
    } // end of method HiddenEnumerator`1::Dispose

    // method line 289
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8b07
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_0006:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	IL_000b:  box !0
	IL_0010:  ret 
    } // end of method HiddenEnumerator`1::System.Collections.IEnumerator.get_Current

    // method line 290
    .method public final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x8b19
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_0006:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	IL_000b:  ret 
    } // end of method HiddenEnumerator`1::MoveNext

    // method line 291
    .method public final virtual hidebysig newslot 
           instance default void Reset ()  cil managed 
    {
        // Method begins at RVA 0x8b26
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.Enumerator/HiddenEnumerator`1<!0>::enumerator
	IL_0006:  callvirt instance void class [mscorlib]System.Collections.IEnumerator::Reset()
	IL_000b:  ret 
    } // end of method HiddenEnumerator`1::Reset

	.property instance !T Current ()
	{
		.get instance default !0 System.Linq.Enumerator/HiddenEnumerator`1::get_Current () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.Enumerator/HiddenEnumerator`1::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class HiddenEnumerator`1

  } // end of class System.Linq.Enumerator
}

.namespace System.Linq
{
  .class private auto ansi sealed beforefieldinit SharedEnumerable`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>, [mscorlib]System.Collections.IEnumerable, [mscorlib]System.IDisposable  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> _source
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> _sharedEnumerator
    .field  private  object _lockObject
    .field  private  int32 _enumeratorCount
    .field  private  bool '<Disposed>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 292
    .method assembly hidebysig specialname 
           instance default bool get_Disposed ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8b33
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld bool class System.Linq.SharedEnumerable`1<!0>::'<Disposed>k__BackingField'
	IL_0006:  ret 
    } // end of method SharedEnumerable`1::get_Disposed

    // method line 293
    .method assembly hidebysig specialname 
           instance default void set_Disposed (bool 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x8b3b
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld bool class System.Linq.SharedEnumerable`1<!0>::'<Disposed>k__BackingField'
	IL_0007:  ret 
    } // end of method SharedEnumerable`1::set_Disposed

    // method line 294
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Collections.Generic.IEnumerable`1<!T> source)  cil managed 
    {
        // Method begins at RVA 0x8b44
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.SharedEnumerable`1<!0>::_source
	IL_000d:  ldarg.0 
	IL_000e:  newobj instance void object::'.ctor'()
	IL_0013:  stfld object class System.Linq.SharedEnumerable`1<!0>::_lockObject
	IL_0018:  ret 
    } // end of method SharedEnumerable`1::.ctor

    // method line 295
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8b60
	// Code size 82 (0x52)
	.maxstack 4
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>	V_0,
		object	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.SharedEnumerable`1<!0>::_lockObject
	IL_0006:  dup 
	IL_0007:  stloc.1 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1<!0>::_sharedEnumerator
	  IL_0013:  brtrue.s IL_0026

	  IL_0015:  ldarg.0 
	  IL_0016:  ldarg.0 
	  IL_0017:  ldfld class [mscorlib]System.Collections.Generic.IEnumerable`1<!0> class System.Linq.SharedEnumerable`1<!0>::_source
	  IL_001c:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
	  IL_0021:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1<!0>::_sharedEnumerator
	  IL_0026:  ldarg.0 
	  IL_0027:  dup 
	  IL_0028:  ldfld int32 class System.Linq.SharedEnumerable`1<!0>::_enumeratorCount
	  IL_002d:  ldc.i4.1 
	  IL_002e:  add 
	  IL_002f:  stfld int32 class System.Linq.SharedEnumerable`1<!0>::_enumeratorCount
	  IL_0034:  ldarg.0 
	  IL_0035:  ldarg.0 
	  IL_0036:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1<!0>::_sharedEnumerator
	  IL_003b:  ldarg.0 
	  IL_003c:  ldfld object class System.Linq.SharedEnumerable`1<!0>::_lockObject
	  IL_0041:  newobj instance void class System.Linq.SharedEnumerable`1/SharedEnumerator<!T>::'.ctor'(class System.Linq.SharedEnumerable`1<!0>, class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, object)
	  IL_0046:  stloc.0 
	  IL_0047:  leave.s IL_0050

	} // end .try 0
	finally  { // 0
	  IL_0049:  ldloc.1 
	  IL_004a:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_004f:  endfinally 
	} // end handler 0
	IL_0050:  ldloc.0 
	IL_0051:  ret 
    } // end of method SharedEnumerable`1::GetEnumerator

    // method line 296
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x8bd0
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1<!T>::GetEnumerator()
	IL_0006:  ret 
    } // end of method SharedEnumerable`1::System.Collections.IEnumerable.GetEnumerator

    // method line 297
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x8bd8
	// Code size 49 (0x31)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.SharedEnumerable`1<!0>::_lockObject
	IL_0006:  dup 
	IL_0007:  stloc.0 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  call instance bool class System.Linq.SharedEnumerable`1<!T>::get_Disposed()
	  IL_0013:  brtrue.s IL_0027

	  IL_0015:  ldarg.0 
	  IL_0016:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1<!0>::_sharedEnumerator
	  IL_001b:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0020:  ldarg.0 
	  IL_0021:  ldc.i4.1 
	  IL_0022:  call instance void class System.Linq.SharedEnumerable`1<!T>::set_Disposed(bool)
	  IL_0027:  leave.s IL_0030

	} // end .try 0
	finally  { // 0
	  IL_0029:  ldloc.0 
	  IL_002a:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_002f:  endfinally 
	} // end handler 0
	IL_0030:  ret 
    } // end of method SharedEnumerable`1::Dispose

	.property instance bool Disposed ()
	{
		.set instance default void System.Linq.SharedEnumerable`1::set_Disposed (bool 'value') 
		.get instance default bool System.Linq.SharedEnumerable`1::get_Disposed () 
	}
  .class nested private auto ansi sealed beforefieldinit SharedEnumerator<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IEnumerator`1<!0>, [mscorlib]System.IDisposable, [mscorlib]System.Collections.IEnumerator  {
    .field  private  class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> _sharedEnumerator
    .field  private  object _lockObject
    .field  private  class System.Linq.SharedEnumerable`1<!0> _enumerable
    .field  private  !0 _current

    // method line 298
    .method assembly hidebysig specialname rtspecialname 
           instance default void '.ctor' (class System.Linq.SharedEnumerable`1<!T> enumerable, class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> sharedEnumerator, object lockObject)  cil managed 
    {
        // Method begins at RVA 0x8c28
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class System.Linq.SharedEnumerable`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_enumerable
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_sharedEnumerator
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.3 
	IL_0016:  stfld object class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_lockObject
	IL_001b:  ret 
    } // end of method SharedEnumerator::.ctor

    // method line 299
    .method public final virtual hidebysig newslot specialname 
           instance default !T get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8c45
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_current
	IL_0006:  ret 
    } // end of method SharedEnumerator::get_Current

    // method line 300
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x8c4d
	// Code size 22 (0x16)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldnull 
	IL_0002:  stfld class System.Linq.SharedEnumerable`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_enumerable
	IL_0007:  ldarg.0 
	IL_0008:  ldnull 
	IL_0009:  stfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_sharedEnumerator
	IL_000e:  ldarg.0 
	IL_000f:  ldnull 
	IL_0010:  stfld object class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_lockObject
	IL_0015:  ret 
    } // end of method SharedEnumerator::Dispose

    // method line 301
    .method private final virtual hidebysig newslot specialname 
           instance default object System.Collections.IEnumerator.get_Current ()  cil managed 
    {
        // Method begins at RVA 0x8c64
	.override class [mscorlib]System.Collections.IEnumerator::get_Current
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 class System.Linq.SharedEnumerable`1/SharedEnumerator<!T>::get_Current()
	IL_0006:  box !0
	IL_000b:  ret 
    } // end of method SharedEnumerator::System.Collections.IEnumerator.get_Current

    // method line 302
    .method public final virtual hidebysig newslot 
           instance default bool MoveNext ()  cil managed 
    {
        // Method begins at RVA 0x8c74
	// Code size 101 (0x65)
	.maxstack 2
	.locals init (
		bool	V_0,
		bool	V_1,
		object	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_lockObject
	IL_0006:  dup 
	IL_0007:  stloc.2 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class System.Linq.SharedEnumerable`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_enumerable
	  IL_0013:  callvirt instance bool class System.Linq.SharedEnumerable`1<!T>::get_Disposed()
	  IL_0018:  brfalse.s IL_001e

	  IL_001a:  ldc.i4.0 
	  IL_001b:  stloc.1 
	  IL_001c:  leave.s IL_0063

	  IL_001e:  ldarg.0 
	  IL_001f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_sharedEnumerator
	  IL_0024:  callvirt instance bool class [mscorlib]System.Collections.IEnumerator::MoveNext()
	  IL_0029:  stloc.0 
	  IL_002a:  ldloc.0 
	  IL_002b:  brfalse.s IL_003e

	  IL_002d:  ldarg.0 
	  IL_002e:  ldarg.0 
	  IL_002f:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_sharedEnumerator
	  IL_0034:  callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
	  IL_0039:  stfld !0 class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_current
	  IL_003e:  ldloc.0 
	  IL_003f:  brtrue.s IL_0058

	  IL_0041:  ldarg.0 
	  IL_0042:  ldfld class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_sharedEnumerator
	  IL_0047:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_004c:  ldarg.0 
	  IL_004d:  ldfld class System.Linq.SharedEnumerable`1<!0> class System.Linq.SharedEnumerable`1/SharedEnumerator<!0>::_enumerable
	  IL_0052:  ldc.i4.1 
	  IL_0053:  callvirt instance void class System.Linq.SharedEnumerable`1<!T>::set_Disposed(bool)
	  IL_0058:  ldloc.0 
	  IL_0059:  stloc.1 
	  IL_005a:  leave.s IL_0063

	} // end .try 0
	finally  { // 0
	  IL_005c:  ldloc.2 
	  IL_005d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0062:  endfinally 
	} // end handler 0
	IL_0063:  ldloc.1 
	IL_0064:  ret 
    } // end of method SharedEnumerator::MoveNext

    // method line 303
    .method public final virtual hidebysig newslot 
           instance default void Reset ()  cil managed 
    {
        // Method begins at RVA 0x8cf8
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  ldstr "SharedEnumerators cannot be Reset."
	IL_0005:  newobj instance void class [mscorlib]System.NotSupportedException::'.ctor'(string)
	IL_000a:  throw 
    } // end of method SharedEnumerator::Reset

	.property instance !T Current ()
	{
		.get instance default !0 System.Linq.SharedEnumerable`1/SharedEnumerator::get_Current () 
	}
	.property instance object System.Collections.IEnumerator.Current ()
	{
		.get instance default object System.Linq.SharedEnumerable`1/SharedEnumerator::System.Collections.IEnumerator.get_Current () 
	}
  } // end of class SharedEnumerator

  } // end of class System.Linq.SharedEnumerable`1
}

  .class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<sum>j__TPar','<count>j__TPar'>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .custom instance void class [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::'.ctor'(string) =  (
		01 00 21 5C 7B 20 73 75 6D 20 3D 20 7B 73 75 6D   // ..!\{ sum = {sum
		7D 2C 20 63 6F 75 6E 74 20 3D 20 7B 63 6F 75 6E   // }, count = {coun
		74 7D 20 7D 01 00 54 0E 04 54 79 70 65 10 3C 41   // t} }..T..Type.<A
		6E 6F 6E 79 6D 6F 75 73 20 54 79 70 65 3E       ) // nonymous Type>

    .field  private initonly  !0 '<sum>i__Field'
    .custom instance void class [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::'.ctor'(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) =  (01 00 00 00 00 00 00 00 ) // ........

    .field  private initonly  !1 '<count>i__Field'
    .custom instance void class [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::'.ctor'(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) =  (01 00 00 00 00 00 00 00 ) // ........


    // method line 596
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (!'<sum>j__TPar' sum, !'<count>j__TPar' count)  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x74d7
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  stfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_0014:  ret 
    } // end of method <>f__AnonymousType0`2::.ctor

    // method line 597
    .method public hidebysig specialname 
           instance default !'<sum>j__TPar' get_sum ()  cil managed 
    {
        // Method begins at RVA 0x74ed
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_0006:  ret 
    } // end of method <>f__AnonymousType0`2::get_sum

    // method line 598
    .method public hidebysig specialname 
           instance default !'<count>j__TPar' get_count ()  cil managed 
    {
        // Method begins at RVA 0x74f5
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_0006:  ret 
    } // end of method <>f__AnonymousType0`2::get_count

    // method line 599
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7500
	// Code size 85 (0x55)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Text.StringBuilder	V_0)
	IL_0000:  newobj instance void class [mscorlib]System.Text.StringBuilder::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldstr "{ sum = "
	IL_000c:  callvirt instance class [mscorlib]System.Text.StringBuilder class [mscorlib]System.Text.StringBuilder::Append(string)
	IL_0011:  pop 
	IL_0012:  ldloc.0 
	IL_0013:  ldarg.0 
	IL_0014:  ldfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_0019:  box !0
	IL_001e:  callvirt instance class [mscorlib]System.Text.StringBuilder class [mscorlib]System.Text.StringBuilder::Append(object)
	IL_0023:  pop 
	IL_0024:  ldloc.0 
	IL_0025:  ldstr ", count = "
	IL_002a:  callvirt instance class [mscorlib]System.Text.StringBuilder class [mscorlib]System.Text.StringBuilder::Append(string)
	IL_002f:  pop 
	IL_0030:  ldloc.0 
	IL_0031:  ldarg.0 
	IL_0032:  ldfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_0037:  box !1
	IL_003c:  callvirt instance class [mscorlib]System.Text.StringBuilder class [mscorlib]System.Text.StringBuilder::Append(object)
	IL_0041:  pop 
	IL_0042:  ldloc.0 
	IL_0043:  ldstr " }"
	IL_0048:  callvirt instance class [mscorlib]System.Text.StringBuilder class [mscorlib]System.Text.StringBuilder::Append(string)
	IL_004d:  pop 
	IL_004e:  ldloc.0 
	IL_004f:  callvirt instance string object::ToString()
	IL_0054:  ret 
    } // end of method <>f__AnonymousType0`2::ToString

    // method line 600
    .method public virtual hidebysig 
           instance default bool Equals (object 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x7564
	// Code size 59 (0x3b)
	.maxstack 3
	.locals init (
		class '<>f__AnonymousType0`2'<!'<sum>j__TPar', !'<count>j__TPar'>	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  isinst class '<>f__AnonymousType0`2'<!0,!1>
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  brfalse.s IL_0039

	IL_000a:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<sum>j__TPar'>::get_Default()
	IL_000f:  ldarg.0 
	IL_0010:  ldfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_0015:  ldloc.0 
	IL_0016:  ldfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_001b:  callvirt instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<sum>j__TPar'>::Equals(!0, !0)
	IL_0020:  brfalse.s IL_0039

	IL_0022:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<count>j__TPar'>::get_Default()
	IL_0027:  ldarg.0 
	IL_0028:  ldfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_002d:  ldloc.0 
	IL_002e:  ldfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_0033:  callvirt instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<count>j__TPar'>::Equals(!0, !0)
	IL_0038:  ret 
	IL_0039:  ldc.i4.0 
	IL_003a:  ret 
    } // end of method <>f__AnonymousType0`2::Equals

    // method line 601
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x75ac
	// Code size 58 (0x3a)
	.maxstack 3
	.locals init (
		int32	V_0)
	IL_0000:  ldc.i4 976303866
	IL_0005:  stloc.0 
	IL_0006:  ldc.i4 -1521134295
	IL_000b:  ldloc.0 
	IL_000c:  mul 
	IL_000d:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<sum>j__TPar'>::get_Default()
	IL_0012:  ldarg.0 
	IL_0013:  ldfld !0 class '<>f__AnonymousType0`2'<!0,!1>::'<sum>i__Field'
	IL_0018:  callvirt instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<sum>j__TPar'>::GetHashCode(!0)
	IL_001d:  add 
	IL_001e:  stloc.0 
	IL_001f:  ldc.i4 -1521134295
	IL_0024:  ldloc.0 
	IL_0025:  mul 
	IL_0026:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<count>j__TPar'>::get_Default()
	IL_002b:  ldarg.0 
	IL_002c:  ldfld !1 class '<>f__AnonymousType0`2'<!0,!1>::'<count>i__Field'
	IL_0031:  callvirt instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<count>j__TPar'>::GetHashCode(!0)
	IL_0036:  add 
	IL_0037:  stloc.0 
	IL_0038:  ldloc.0 
	IL_0039:  ret 
    } // end of method <>f__AnonymousType0`2::GetHashCode

	.property instance !'<sum>j__TPar' sum ()
	{
		.get instance default !0 '<>f__AnonymousType0`2'::get_sum () 
	}
	.property instance !'<count>j__TPar' count ()
	{
		.get instance default !1 '<>f__AnonymousType0`2'::get_count () 
	}
  } // end of class <>f__AnonymousType0`2

