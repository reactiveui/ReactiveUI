.assembly extern mscorlib
{
  .ver 2:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly extern System.Core
{
  .ver 3:5:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly extern System.Threading
{
  .ver 1:0:2787:102
  .publickeytoken = (31 BF 38 56 AD 36 4E 35 ) // 1.8V.6N5
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
.assembly 'System.CoreEx'
{
  .custom instance void class [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::'.ctor'() =  (
		01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
		63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01       ) // ceptionThrows.

  .custom instance void class [mscorlib]System.Reflection.AssemblyCompanyAttribute::'.ctor'(string) =  (
		01 00 15 4D 69 63 72 6F 73 6F 66 74 20 43 6F 72   // ...Microsoft Cor
		70 6F 72 61 74 69 6F 6E 00 00                   ) // poration..

  .custom instance void class [mscorlib]System.Reflection.AssemblyProductAttribute::'.ctor'(string) =  (
		01 00 20 52 65 61 63 74 69 76 65 20 45 78 74 65   // .. Reactive Exte
		6E 73 69 6F 6E 73 20 43 6F 72 65 20 4C 69 62 72   // nsions Core Libr
		61 72 79 00 00                                  ) // ary..

  .custom instance void class [mscorlib]System.Diagnostics.DebuggableAttribute::'.ctor'(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) =  (01 00 03 00 00 00 00 00 ) // ........

  .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

  .custom instance void class [mscorlib]System.Reflection.AssemblyTitleAttribute::'.ctor'(string) =  (
		01 00 0D 53 79 73 74 65 6D 2E 43 6F 72 65 45 78   // ...System.CoreEx
		00 00                                           ) // ..

  .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::'.ctor'(int32) =  (01 00 08 00 00 00 00 00 ) // ........

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
.module System.CoreEx.dll // GUID = {F2F8B0D6-36FA-467B-8885-5243928E4752}


.namespace System
{
  .class public auto ansi sealed Action`5<T1,T2,T3,T4,T5>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 1
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`5::.ctor

    // method line 2
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`5::Invoke

    // method line 3
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`5::BeginInvoke

    // method line 4
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`5::EndInvoke

  } // end of class System.Action`5
}

.namespace System
{
  .class public auto ansi sealed Action`6<T1,T2,T3,T4,T5,T6>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 5
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`6::.ctor

    // method line 6
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`6::Invoke

    // method line 7
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`6::BeginInvoke

    // method line 8
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`6::EndInvoke

  } // end of class System.Action`6
}

.namespace System
{
  .class public auto ansi sealed Action`7<T1,T2,T3,T4,T5,T6,T7>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 9
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`7::.ctor

    // method line 10
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`7::Invoke

    // method line 11
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`7::BeginInvoke

    // method line 12
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`7::EndInvoke

  } // end of class System.Action`7
}

.namespace System
{
  .class public auto ansi sealed Action`8<T1,T2,T3,T4,T5,T6,T7,T8>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 13
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`8::.ctor

    // method line 14
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`8::Invoke

    // method line 15
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`8::BeginInvoke

    // method line 16
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`8::EndInvoke

  } // end of class System.Action`8
}

.namespace System
{
  .class public auto ansi sealed Action`9<T1,T2,T3,T4,T5,T6,T7,T8,T9>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 17
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`9::.ctor

    // method line 18
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`9::Invoke

    // method line 19
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`9::BeginInvoke

    // method line 20
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`9::EndInvoke

  } // end of class System.Action`9
}

.namespace System
{
  .class public auto ansi sealed Action`10<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 21
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`10::.ctor

    // method line 22
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`10::Invoke

    // method line 23
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`10::BeginInvoke

    // method line 24
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`10::EndInvoke

  } // end of class System.Action`10
}

.namespace System
{
  .class public auto ansi sealed Action`11<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 25
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`11::.ctor

    // method line 26
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`11::Invoke

    // method line 27
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`11::BeginInvoke

    // method line 28
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`11::EndInvoke

  } // end of class System.Action`11
}

.namespace System
{
  .class public auto ansi sealed Action`12<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 29
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`12::.ctor

    // method line 30
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`12::Invoke

    // method line 31
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`12::BeginInvoke

    // method line 32
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`12::EndInvoke

  } // end of class System.Action`12
}

.namespace System
{
  .class public auto ansi sealed Action`13<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 33
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`13::.ctor

    // method line 34
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`13::Invoke

    // method line 35
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`13::BeginInvoke

    // method line 36
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`13::EndInvoke

  } // end of class System.Action`13
}

.namespace System
{
  .class public auto ansi sealed Action`14<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 37
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`14::.ctor

    // method line 38
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`14::Invoke

    // method line 39
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`14::BeginInvoke

    // method line 40
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`14::EndInvoke

  } // end of class System.Action`14
}

.namespace System
{
  .class public auto ansi sealed Action`15<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 41
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`15::.ctor

    // method line 42
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`15::Invoke

    // method line 43
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`15::BeginInvoke

    // method line 44
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`15::EndInvoke

  } // end of class System.Action`15
}

.namespace System
{
  .class public auto ansi sealed Action`16<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 45
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`16::.ctor

    // method line 46
    .method public virtual hidebysig newslot 
           instance default void Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, !T16 arg16)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`16::Invoke

    // method line 47
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, !T16 arg16, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`16::BeginInvoke

    // method line 48
    .method public virtual hidebysig newslot 
           instance default void EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Action`16::EndInvoke

  } // end of class System.Action`16
}

.namespace System.Collections.Generic
{
  .class public auto ansi sealed beforefieldinit AnonymousComparer`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.IComparer`1<!0>  {
    .field  private initonly  class [System.Core]System.Func`3<!0,!0,int32> comparer

    // method line 49
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Core]System.Func`3<!T, !T, int32> comparer)  cil managed 
    {
        // Method begins at RVA 0x20d0
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "comparer"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.1 
	IL_0016:  stfld class [System.Core]System.Func`3<!0,!0,int32> class System.Collections.Generic.AnonymousComparer`1<!0>::comparer
	IL_001b:  ret 
    } // end of method AnonymousComparer`1::.ctor

    // method line 50
    .method public final virtual hidebysig newslot 
           instance default int32 Compare (!T x, !T y)  cil managed 
    {
        // Method begins at RVA 0x20ed
	// Code size 52 (0x34)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  box !0
	IL_0006:  brtrue.s IL_0013

	IL_0008:  ldstr "x"
	IL_000d:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0012:  throw 
	IL_0013:  ldarg.2 
	IL_0014:  box !0
	IL_0019:  brtrue.s IL_0026

	IL_001b:  ldstr "y"
	IL_0020:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0025:  throw 
	IL_0026:  ldarg.0 
	IL_0027:  ldfld class [System.Core]System.Func`3<!0,!0,int32> class System.Collections.Generic.AnonymousComparer`1<!0>::comparer
	IL_002c:  ldarg.1 
	IL_002d:  ldarg.2 
	IL_002e:  callvirt instance !2 class [System.Core]System.Func`3<!T, !T, int32>::Invoke(!0, !1)
	IL_0033:  ret 
    } // end of method AnonymousComparer`1::Compare

  } // end of class System.Collections.Generic.AnonymousComparer`1
}

.namespace System.Concurrency
{
  .class interface public auto ansi abstract IScheduler
  {

    // method line 51
    .method public virtual hidebysig newslot abstract specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IScheduler::get_Now

    // method line 52
    .method public virtual hidebysig newslot abstract 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IScheduler::Schedule

    // method line 53
    .method public virtual hidebysig newslot abstract 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IScheduler::Schedule

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.IScheduler::get_Now () 
	}
  } // end of class System.Concurrency.IScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi abstract beforefieldinit VirtualScheduler`2<(class [mscorlib]System.IComparable`1<!TAbsolute>) TAbsolute,TRelative>
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  private initonly  class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> queue
    .field  private  !0 '<Ticks>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 54
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x2122
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0007:  call instance class [mscorlib]System.IDisposable class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_000c:  ret 
    } // end of method VirtualScheduler`2::Schedule

    // method line 55
    .method public virtual hidebysig newslot abstract 
           instance default !TAbsolute Increment (!TAbsolute absolute, !TRelative relative)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method VirtualScheduler`2::Increment

    // method line 56
    .method public virtual hidebysig newslot abstract 
           instance default valuetype [mscorlib]System.DateTimeOffset ToDateTimeOffset (!TAbsolute absolute)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method VirtualScheduler`2::ToDateTimeOffset

    // method line 57
    .method public virtual hidebysig newslot abstract 
           instance default !TRelative FromTimeSpan (valuetype [mscorlib]System.TimeSpan timeSpan)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method VirtualScheduler`2::FromTimeSpan

    // method line 58
    .method public hidebysig 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, !TRelative dueTime)  cil managed 
    {
        // Method begins at RVA 0x2154
	// Code size 78 (0x4e)
	.maxstack 3
	.locals init (
		!TAbsolute	V_0,
		class [System.Core]System.Action	V_1,
		class System.Concurrency.ScheduledItem`1<!TAbsolute>	V_2,
		class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!TAbsolute, !TRelative>	V_3)
	IL_0000:  newobj instance void class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!TAbsolute, !TRelative>::'.ctor'()
	IL_0005:  stloc.3 
	IL_0006:  ldloc.3 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!0,!1>::action
	IL_000d:  ldloc.3 
	IL_000e:  newobj instance void class System.Disposables.BooleanDisposable::'.ctor'()
	IL_0013:  stfld class System.Disposables.BooleanDisposable class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!0,!1>::disposable
	IL_0018:  ldarg.0 
	IL_0019:  ldarg.0 
	IL_001a:  call instance !0 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::get_Ticks()
	IL_001f:  ldarg.2 
	IL_0020:  callvirt instance !0 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::Increment(!0, !1)
	IL_0025:  stloc.0 
	IL_0026:  ldloc.3 
	IL_0027:  ldftn instance void class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!TAbsolute, !TRelative>::'<Schedule>b__0'()
	IL_002d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0032:  stloc.1 
	IL_0033:  ldloc.1 
	IL_0034:  ldloc.0 
	IL_0035:  newobj instance void class System.Concurrency.ScheduledItem`1<!TAbsolute>::'.ctor'(class [System.Core]System.Action, !0)
	IL_003a:  stloc.2 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_0041:  ldloc.2 
	IL_0042:  callvirt instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::Enqueue(!0)
	IL_0047:  ldloc.3 
	IL_0048:  ldfld class System.Disposables.BooleanDisposable class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!0,!1>::disposable
	IL_004d:  ret 
    } // end of method VirtualScheduler`2::Schedule

    // method line 59
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x21ae
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  ldarg.0 
	IL_0003:  ldarg.2 
	IL_0004:  callvirt instance !1 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::FromTimeSpan(valuetype [mscorlib]System.TimeSpan)
	IL_0009:  call instance class [mscorlib]System.IDisposable class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::Schedule(class [System.Core]System.Action, !1)
	IL_000e:  ret 
    } // end of method VirtualScheduler`2::Schedule

    // method line 60
    .method public hidebysig 
           instance default void Run ()  cil managed 
    {
        // Method begins at RVA 0x21c0
	// Code size 52 (0x34)
	.maxstack 2
	.locals init (
		class System.Concurrency.ScheduledItem`1<!TAbsolute>	V_0)
	IL_0000:  br.s IL_0025

	IL_0002:  ldarg.0 
	IL_0003:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_0008:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::Dequeue()
	IL_000d:  stloc.0 
	IL_000e:  ldarg.0 
	IL_000f:  ldloc.0 
	IL_0010:  callvirt instance !0 class System.Concurrency.ScheduledItem`1<!TAbsolute>::get_DueTime()
	IL_0015:  call instance void class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::set_Ticks(!0)
	IL_001a:  ldloc.0 
	IL_001b:  callvirt instance class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<!TAbsolute>::get_Action()
	IL_0020:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0025:  ldarg.0 
	IL_0026:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_002b:  callvirt instance int32 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::get_Count()
	IL_0030:  ldc.i4.0 
	IL_0031:  bgt.s IL_0002

	IL_0033:  ret 
    } // end of method VirtualScheduler`2::Run

    // method line 61
    .method public hidebysig 
           instance default void RunTo (!TAbsolute time)  cil managed 
    {
        // Method begins at RVA 0x2200
	// Code size 86 (0x56)
	.maxstack 2
	.locals init (
		class System.Concurrency.ScheduledItem`1<!TAbsolute>	V_0,
		!TAbsolute	V_1)
	IL_0000:  br.s IL_0025

	IL_0002:  ldarg.0 
	IL_0003:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_0008:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::Dequeue()
	IL_000d:  stloc.0 
	IL_000e:  ldarg.0 
	IL_000f:  ldloc.0 
	IL_0010:  callvirt instance !0 class System.Concurrency.ScheduledItem`1<!TAbsolute>::get_DueTime()
	IL_0015:  call instance void class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::set_Ticks(!0)
	IL_001a:  ldloc.0 
	IL_001b:  callvirt instance class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<!TAbsolute>::get_Action()
	IL_0020:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0025:  ldarg.0 
	IL_0026:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_002b:  callvirt instance int32 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::get_Count()
	IL_0030:  ldc.i4.0 
	IL_0031:  ble.s IL_0055

	IL_0033:  ldarg.0 
	IL_0034:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_0039:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::Peek()
	IL_003e:  callvirt instance !0 class System.Concurrency.ScheduledItem`1<!TAbsolute>::get_DueTime()
	IL_0043:  stloc.1 
	IL_0044:  ldloca.s 1
	IL_0046:  ldarg.1 
	IL_0047:  constrained. !0
	IL_004d:  callvirt instance int32 class [mscorlib]System.IComparable`1<!TAbsolute>::CompareTo(!0)
	IL_0052:  ldc.i4.0 
	IL_0053:  ble.s IL_0002

	IL_0055:  ret 
    } // end of method VirtualScheduler`2::RunTo

    // method line 62
    .method public hidebysig 
           instance default void Sleep (!TRelative ticks)  cil managed 
    {
        // Method begins at RVA 0x2262
	// Code size 20 (0x14)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.0 
	IL_0002:  ldarg.0 
	IL_0003:  call instance !0 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::get_Ticks()
	IL_0008:  ldarg.1 
	IL_0009:  callvirt instance !0 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::Increment(!0, !1)
	IL_000e:  call instance void class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::set_Ticks(!0)
	IL_0013:  ret 
    } // end of method VirtualScheduler`2::Sleep

    // method line 63
    .method public hidebysig specialname 
           instance default !TAbsolute get_Ticks ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2277
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Concurrency.VirtualScheduler`2<!0,!1>::'<Ticks>k__BackingField'
	IL_0006:  ret 
    } // end of method VirtualScheduler`2::get_Ticks

    // method line 64
    .method private hidebysig specialname 
           instance default void set_Ticks (!TAbsolute 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x227f
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld !0 class System.Concurrency.VirtualScheduler`2<!0,!1>::'<Ticks>k__BackingField'
	IL_0007:  ret 
    } // end of method VirtualScheduler`2::set_Ticks

    // method line 65
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x2288
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.0 
	IL_0002:  call instance !0 class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::get_Ticks()
	IL_0007:  callvirt instance valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.VirtualScheduler`2<!TAbsolute, !TRelative>::ToDateTimeOffset(!0)
	IL_000c:  ret 
    } // end of method VirtualScheduler`2::get_Now

    // method line 66
    .method family hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2296
	// Code size 18 (0x12)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!TAbsolute>>::'.ctor'()
	IL_0006:  stfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem`1<!0>> class System.Concurrency.VirtualScheduler`2<!0,!1>::queue
	IL_000b:  ldarg.0 
	IL_000c:  call instance void object::'.ctor'()
	IL_0011:  ret 
    } // end of method VirtualScheduler`2::.ctor

	.property instance !TAbsolute Ticks ()
	{
		.set instance default void System.Concurrency.VirtualScheduler`2::set_Ticks (!0 'value') 
		.get instance default !0 System.Concurrency.VirtualScheduler`2::get_Ticks () 
	}
	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.VirtualScheduler`2::get_Now () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass1'<(class [mscorlib]System.IComparable`1<!TAbsolute>) TAbsolute,TRelative>
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.BooleanDisposable disposable
    .field  public  class [System.Core]System.Action action

    // method line 336
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2130
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass1::.ctor

    // method line 337
    .method public hidebysig 
           instance default void '<Schedule>b__0' ()  cil managed 
    {
        // Method begins at RVA 0x2138
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!0,!1>::disposable
	IL_0006:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_000b:  brtrue.s IL_0018

	IL_000d:  ldarg.0 
	IL_000e:  ldfld class [System.Core]System.Action class System.Concurrency.VirtualScheduler`2/'<>c__DisplayClass1'<!0,!1>::action
	IL_0013:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0018:  ret 
    } // end of method <>c__DisplayClass1::<Schedule>b__0

  } // end of class <>c__DisplayClass1

  } // end of class System.Concurrency.VirtualScheduler`2
}

.namespace System.Concurrency
{
  .class public auto ansi beforefieldinit EventLoopScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler, [mscorlib]System.IDisposable  {
    .field  private  class [mscorlib]System.Threading.Thread thread
    .field  private  class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> queue
    .field  private  class [mscorlib]System.Threading.ManualResetEvent gate
    .field  private  bool disposed
    .field  private static  int32 counter

    // method line 67
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x22a9
	// Code size 32 (0x20)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldstr "Event Loop "
	IL_0006:  ldsflda int32 System.Concurrency.EventLoopScheduler::counter
	IL_000b:  call int32 class [mscorlib]System.Threading.Interlocked::Increment([out] int32&)
	IL_0010:  box [mscorlib]System.Int32
	IL_0015:  call string string::Concat(object, object)
	IL_001a:  call instance void class System.Concurrency.EventLoopScheduler::'.ctor'(string)
	IL_001f:  ret 
    } // end of method EventLoopScheduler::.ctor

    // method line 68
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (string name)  cil managed 
    {
        // Method begins at RVA 0x2404
	// Code size 101 (0x65)
	.maxstack 4
	.locals init (
		class [mscorlib]System.Threading.Thread	V_0,
		class [mscorlib]System.Threading.ThreadStart	V_1)
	IL_0000:  ldnull 
	IL_0001:  stloc.1 
	IL_0002:  ldarg.0 
	IL_0003:  call instance void object::'.ctor'()
	IL_0008:  ldarg.1 
	IL_0009:  brtrue.s IL_0016

	IL_000b:  ldstr "name"
	IL_0010:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0015:  throw 
	IL_0016:  ldarg.0 
	IL_0017:  ldc.i4.0 
	IL_0018:  newobj instance void class [mscorlib]System.Threading.ManualResetEvent::'.ctor'(bool)
	IL_001d:  stfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0022:  ldarg.0 
	IL_0023:  newobj instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::'.ctor'()
	IL_0028:  stfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.EventLoopScheduler::queue
	IL_002d:  ldarg.0 
	IL_002e:  ldloc.1 
	IL_002f:  brtrue.s IL_003e

	IL_0031:  ldarg.0 
	IL_0032:  ldftn instance void class System.Concurrency.EventLoopScheduler::'<.ctor>b__1'()
	IL_0038:  newobj instance void class [mscorlib]System.Threading.ThreadStart::'.ctor'(object, native int)
	IL_003d:  stloc.1 
	IL_003e:  ldloc.1 
	IL_003f:  newobj instance void class [mscorlib]System.Threading.Thread::'.ctor'(class [mscorlib]System.Threading.ThreadStart)
	IL_0044:  stloc.0 
	IL_0045:  ldloc.0 
	IL_0046:  ldc.i4.1 
	IL_0047:  callvirt instance void class [mscorlib]System.Threading.Thread::set_IsBackground(bool)
	IL_004c:  ldloc.0 
	IL_004d:  ldarg.1 
	IL_004e:  callvirt instance void class [mscorlib]System.Threading.Thread::set_Name(string)
	IL_0053:  ldloc.0 
	IL_0054:  stfld class [mscorlib]System.Threading.Thread System.Concurrency.EventLoopScheduler::thread
	IL_0059:  ldarg.0 
	IL_005a:  ldfld class [mscorlib]System.Threading.Thread System.Concurrency.EventLoopScheduler::thread
	IL_005f:  callvirt instance void class [mscorlib]System.Threading.Thread::Start()
	IL_0064:  ret 
    } // end of method EventLoopScheduler::.ctor

    // method line 69
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x2475
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset valuetype [mscorlib]System.DateTimeOffset::get_Now()
	IL_0005:  ret 
    } // end of method EventLoopScheduler::get_Now

    // method line 70
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x247c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0015:  call instance class [mscorlib]System.IDisposable class System.Concurrency.EventLoopScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_001a:  ret 
    } // end of method EventLoopScheduler::Schedule

    // method line 71
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x2498
	// Code size 88 (0x58)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.DateTimeOffset	V_0,
		class System.Concurrency.ScheduledItem	V_1,
		class [mscorlib]System.Threading.ManualResetEvent	V_2)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call instance valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.EventLoopScheduler::get_Now()
	IL_0014:  ldarg.2 
	IL_0015:  call valuetype [mscorlib]System.DateTimeOffset valuetype [mscorlib]System.DateTimeOffset::op_Addition(valuetype [mscorlib]System.DateTimeOffset, valuetype [mscorlib]System.TimeSpan)
	IL_001a:  stloc.0 
	IL_001b:  ldarg.1 
	IL_001c:  ldloc.0 
	IL_001d:  newobj instance void class System.Concurrency.ScheduledItem::'.ctor'(class [System.Core]System.Action, valuetype [mscorlib]System.DateTimeOffset)
	IL_0022:  stloc.1 
	IL_0023:  ldarg.0 
	IL_0024:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0029:  dup 
	IL_002a:  stloc.2 
	IL_002b:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0030:  ldarg.0 
	  IL_0031:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.EventLoopScheduler::queue
	  IL_0036:  ldloc.1 
	  IL_0037:  callvirt instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Enqueue(!0)
	  IL_003c:  ldarg.0 
	  IL_003d:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	  IL_0042:  callvirt instance bool class [mscorlib]System.Threading.EventWaitHandle::Set()
	  IL_0047:  pop 
	  IL_0048:  leave.s IL_0051

	} // end .try 0
	finally  { // 0
	  IL_004a:  ldloc.2 
	  IL_004b:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0050:  endfinally 
	} // end handler 0
	IL_0051:  ldloc.1 
	IL_0052:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::GetDisposable()
	IL_0057:  ret 
    } // end of method EventLoopScheduler::Schedule

    // method line 72
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x250c
	// Code size 42 (0x2a)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Threading.ManualResetEvent	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0006:  dup 
	IL_0007:  stloc.0 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldc.i4.1 
	  IL_000f:  stfld bool System.Concurrency.EventLoopScheduler::disposed
	  IL_0014:  ldarg.0 
	  IL_0015:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	  IL_001a:  callvirt instance bool class [mscorlib]System.Threading.EventWaitHandle::Set()
	  IL_001f:  pop 
	  IL_0020:  leave.s IL_0029

	} // end .try 0
	finally  { // 0
	  IL_0022:  ldloc.0 
	  IL_0023:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0028:  endfinally 
	} // end handler 0
	IL_0029:  ret 
    } // end of method EventLoopScheduler::Dispose

    // method line 73
    .method private hidebysig 
           instance default void '<.ctor>b__1' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x22d8
	// Code size 260 (0x104)
	.maxstack 4
	.locals init (
		class System.Concurrency.ScheduledItem	V_0,
		valuetype [mscorlib]System.DateTimeOffset	V_1,
		valuetype [mscorlib]System.TimeSpan	V_2,
		class System.Concurrency.ScheduledItem	V_3,
		class [System.Core]System.Action	V_4,
		class [mscorlib]System.Threading.ManualResetEvent	V_5,
		class [mscorlib]System.Threading.ManualResetEvent	V_6)
	IL_0000:  ldnull 
	IL_0001:  stloc.s 4
	IL_0003:  ldarg.0 
	IL_0004:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0009:  callvirt instance bool class [mscorlib]System.Threading.WaitHandle::WaitOne()
	IL_000e:  pop 
	IL_000f:  ldarg.0 
	IL_0010:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0015:  dup 
	IL_0016:  stloc.s 5
	IL_0018:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_001d:  ldarg.0 
	  IL_001e:  ldfld bool System.Concurrency.EventLoopScheduler::disposed
	  IL_0023:  brfalse.s IL_002a

	  IL_0025:  leave IL_0103

	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.EventLoopScheduler::queue
	  IL_0030:  callvirt instance int32 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::get_Count()
	  IL_0035:  brtrue.s IL_0045

	  IL_0037:  ldarg.0 
	  IL_0038:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	  IL_003d:  callvirt instance bool class [mscorlib]System.Threading.EventWaitHandle::Reset()
	  IL_0042:  pop 
	  IL_0043:  leave.s IL_0003

	  IL_0045:  leave.s IL_004f

	} // end .try 0
	finally  { // 0
	  IL_0047:  ldloc.s 5
	  IL_0049:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_004e:  endfinally 
	} // end handler 0
	IL_004f:  ldnull 
	IL_0050:  stloc.0 
	IL_0051:  ldarg.0 
	IL_0052:  call instance valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.EventLoopScheduler::get_Now()
	IL_0057:  stloc.1 
	IL_0058:  ldloca.s 2
	IL_005a:  initobj [mscorlib]System.TimeSpan
	IL_0060:  ldarg.0 
	IL_0061:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0066:  dup 
	IL_0067:  stloc.s 6
	IL_0069:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 1
	  IL_006e:  ldarg.0 
	  IL_006f:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.EventLoopScheduler::queue
	  IL_0074:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Peek()
	  IL_0079:  stloc.3 
	  IL_007a:  ldloc.3 
	  IL_007b:  callvirt instance !0 class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_DueTime()
	  IL_0080:  ldloc.1 
	  IL_0081:  call valuetype [mscorlib]System.TimeSpan valuetype [mscorlib]System.DateTimeOffset::op_Subtraction(valuetype [mscorlib]System.DateTimeOffset, valuetype [mscorlib]System.DateTimeOffset)
	  IL_0086:  stloc.2 
	  IL_0087:  ldloc.3 
	  IL_0088:  callvirt instance bool class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_IsCanceled()
	  IL_008d:  brtrue.s IL_009e

	  IL_008f:  ldloca.s 2
	  IL_0091:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	  IL_0096:  call instance int32 valuetype [mscorlib]System.TimeSpan::CompareTo(valuetype [mscorlib]System.TimeSpan)
	  IL_009b:  ldc.i4.0 
	  IL_009c:  bgt.s IL_00aa

	  IL_009e:  ldarg.0 
	  IL_009f:  ldfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.EventLoopScheduler::queue
	  IL_00a4:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Dequeue()
	  IL_00a9:  stloc.0 
	  IL_00aa:  leave.s IL_00b4

	} // end .try 1
	finally  { // 1
	  IL_00ac:  ldloc.s 6
	  IL_00ae:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_00b3:  endfinally 
	} // end handler 1
	IL_00b4:  ldloc.0 
	IL_00b5:  brtrue.s IL_00e8

	IL_00b7:  ldarg.0 
	IL_00b8:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_00bd:  callvirt instance bool class [mscorlib]System.Threading.EventWaitHandle::Reset()
	IL_00c2:  pop 
	IL_00c3:  call class System.Concurrency.ThreadPoolScheduler class System.Concurrency.Scheduler::get_ThreadPool()
	IL_00c8:  ldloc.s 4
	IL_00ca:  brtrue.s IL_00da

	IL_00cc:  ldarg.0 
	IL_00cd:  ldftn instance void class System.Concurrency.EventLoopScheduler::'<.ctor>b__2'()
	IL_00d3:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_00d8:  stloc.s 4
	IL_00da:  ldloc.s 4
	IL_00dc:  ldloc.2 
	IL_00dd:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.ThreadPoolScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_00e2:  pop 
	IL_00e3:  br IL_0003

	IL_00e8:  ldloc.0 
	IL_00e9:  callvirt instance bool class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_IsCanceled()
	IL_00ee:  brtrue IL_0003

	IL_00f3:  ldloc.0 
	IL_00f4:  callvirt instance class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_Action()
	IL_00f9:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_00fe:  br IL_0003

	IL_0103:  ret 
    } // end of method EventLoopScheduler::<.ctor>b__1

    // method line 74
    .method private hidebysig 
           instance default void '<.ctor>b__2' ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x22ca
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Threading.ManualResetEvent System.Concurrency.EventLoopScheduler::gate
	IL_0006:  callvirt instance bool class [mscorlib]System.Threading.EventWaitHandle::Set()
	IL_000b:  pop 
	IL_000c:  ret 
    } // end of method EventLoopScheduler::<.ctor>b__2

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.EventLoopScheduler::get_Now () 
	}
  } // end of class System.Concurrency.EventLoopScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit NewThreadScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  assembly static initonly  class System.Concurrency.NewThreadScheduler Instance

    // method line 75
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x2554
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method NewThreadScheduler::get_Now

    // method line 76
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x255b
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0015:  call instance class [mscorlib]System.IDisposable class System.Concurrency.NewThreadScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_001a:  ret 
    } // end of method NewThreadScheduler::Schedule

    // method line 77
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x25cc
	// Code size 81 (0x51)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Threading.Thread	V_0,
		class System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'	V_1)
	IL_0000:  newobj instance void class System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::'.ctor'()
	IL_0005:  stloc.1 
	IL_0006:  ldloc.1 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::action
	IL_000d:  ldloc.1 
	IL_000e:  ldarg.2 
	IL_000f:  stfld valuetype [mscorlib]System.TimeSpan System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::dueTime
	IL_0014:  ldloc.1 
	IL_0015:  ldfld class [System.Core]System.Action System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::action
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "action"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.1 
	IL_0028:  newobj instance void class System.Disposables.BooleanDisposable::'.ctor'()
	IL_002d:  stfld class System.Disposables.BooleanDisposable System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::d
	IL_0032:  ldloc.1 
	IL_0033:  ldftn instance void class System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::'<Schedule>b__0'()
	IL_0039:  newobj instance void class [mscorlib]System.Threading.ThreadStart::'.ctor'(object, native int)
	IL_003e:  newobj instance void class [mscorlib]System.Threading.Thread::'.ctor'(class [mscorlib]System.Threading.ThreadStart)
	IL_0043:  stloc.0 
	IL_0044:  ldloc.0 
	IL_0045:  callvirt instance void class [mscorlib]System.Threading.Thread::Start()
	IL_004a:  ldloc.1 
	IL_004b:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::d
	IL_0050:  ret 
    } // end of method NewThreadScheduler::Schedule

    // method line 78
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2635
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method NewThreadScheduler::.ctor

    // method line 79
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x2629
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  newobj instance void class System.Concurrency.NewThreadScheduler::'.ctor'()
	IL_0005:  stsfld class System.Concurrency.NewThreadScheduler System.Concurrency.NewThreadScheduler::Instance
	IL_000a:  ret 
    } // end of method NewThreadScheduler::.cctor

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.NewThreadScheduler::get_Now () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass1'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.BooleanDisposable d
    .field  public  class [System.Core]System.Action action
    .field  public  valuetype [mscorlib]System.TimeSpan dueTime

    // method line 338
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2577
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass1::.ctor

    // method line 339
    .method public hidebysig 
           instance default void '<Schedule>b__0' ()  cil managed 
    {
        // Method begins at RVA 0x2580
	// Code size 64 (0x40)
	.maxstack 2
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::d
	IL_0006:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_000b:  brtrue.s IL_003f

	IL_000d:  ldarg.0 
	IL_000e:  ldflda valuetype [mscorlib]System.TimeSpan System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::dueTime
	IL_0013:  call instance int64 valuetype [mscorlib]System.TimeSpan::get_Ticks()
	IL_0018:  ldc.i4.0 
	IL_0019:  conv.i8 
	IL_001a:  ble.s IL_0027

	IL_001c:  ldarg.0 
	IL_001d:  ldfld valuetype [mscorlib]System.TimeSpan System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::dueTime
	IL_0022:  call void class [mscorlib]System.Threading.Thread::Sleep(valuetype [mscorlib]System.TimeSpan)
	IL_0027:  ldarg.0 
	IL_0028:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::d
	IL_002d:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_0032:  brtrue.s IL_003f

	IL_0034:  ldarg.0 
	IL_0035:  ldfld class [System.Core]System.Action System.Concurrency.NewThreadScheduler/'<>c__DisplayClass1'::action
	IL_003a:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_003f:  ret 
    } // end of method <>c__DisplayClass1::<Schedule>b__0

  } // end of class <>c__DisplayClass1

  } // end of class System.Concurrency.NewThreadScheduler
}

.namespace System.Concurrency
{
  .class private auto ansi beforefieldinit ScheduledItem`1<(class [mscorlib]System.IComparable`1<!TAbsolute>) TAbsolute>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.IComparable`1<class System.Concurrency.ScheduledItem`1<!0>>  {
    .field  private static  int64 count
    .field  private  class [System.Core]System.Action action
    .field  private  !0 dueTime
    .field  private  int64 id
    .field  private  class System.Disposables.BooleanDisposable d

    // method line 80
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Core]System.Action action, !TAbsolute dueTime)  cil managed 
    {
        // Method begins at RVA 0x263d
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void class System.Disposables.BooleanDisposable::'.ctor'()
	IL_0006:  stfld class System.Disposables.BooleanDisposable class System.Concurrency.ScheduledItem`1<!0>::d
	IL_000b:  ldarg.0 
	IL_000c:  call instance void object::'.ctor'()
	IL_0011:  ldarg.0 
	IL_0012:  ldsflda int64 class System.Concurrency.ScheduledItem`1<!0>::count
	IL_0017:  call int64 class [mscorlib]System.Threading.Interlocked::Increment([out] int64&)
	IL_001c:  stfld int64 class System.Concurrency.ScheduledItem`1<!0>::id
	IL_0021:  ldarg.0 
	IL_0022:  ldarg.1 
	IL_0023:  stfld class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<!0>::action
	IL_0028:  ldarg.0 
	IL_0029:  ldarg.2 
	IL_002a:  stfld !0 class System.Concurrency.ScheduledItem`1<!0>::dueTime
	IL_002f:  ret 
    } // end of method ScheduledItem`1::.ctor

    // method line 81
    .method public hidebysig specialname 
           instance default class [System.Core]System.Action get_Action ()  cil managed 
    {
        // Method begins at RVA 0x266e
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<!0>::action
	IL_0006:  ret 
    } // end of method ScheduledItem`1::get_Action

    // method line 82
    .method public hidebysig specialname 
           instance default !TAbsolute get_DueTime ()  cil managed 
    {
        // Method begins at RVA 0x2676
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Concurrency.ScheduledItem`1<!0>::dueTime
	IL_0006:  ret 
    } // end of method ScheduledItem`1::get_DueTime

    // method line 83
    .method public final virtual hidebysig newslot 
           instance default int32 CompareTo (class System.Concurrency.ScheduledItem`1<!TAbsolute> other)  cil managed 
    {
        // Method begins at RVA 0x2680
	// Code size 47 (0x2f)
	.maxstack 2
	.locals init (
		int32	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldflda !0 class System.Concurrency.ScheduledItem`1<!0>::dueTime
	IL_0006:  ldarg.1 
	IL_0007:  ldfld !0 class System.Concurrency.ScheduledItem`1<!0>::dueTime
	IL_000c:  constrained. !0
	IL_0012:  callvirt instance int32 class [mscorlib]System.IComparable`1<!TAbsolute>::CompareTo(!0)
	IL_0017:  stloc.0 
	IL_0018:  ldloc.0 
	IL_0019:  brfalse.s IL_001d

	IL_001b:  ldloc.0 
	IL_001c:  ret 
	IL_001d:  ldarg.0 
	IL_001e:  ldflda int64 class System.Concurrency.ScheduledItem`1<!0>::id
	IL_0023:  ldarg.1 
	IL_0024:  ldfld int64 class System.Concurrency.ScheduledItem`1<!0>::id
	IL_0029:  call instance int32 int64::CompareTo(int64)
	IL_002e:  ret 
    } // end of method ScheduledItem`1::CompareTo

    // method line 84
    .method public hidebysig specialname 
           instance default bool get_IsCanceled ()  cil managed 
    {
        // Method begins at RVA 0x26bb
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable class System.Concurrency.ScheduledItem`1<!0>::d
	IL_0006:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_000b:  ret 
    } // end of method ScheduledItem`1::get_IsCanceled

    // method line 85
    .method public hidebysig 
           instance default class [mscorlib]System.IDisposable GetDisposable ()  cil managed 
    {
        // Method begins at RVA 0x26c8
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable class System.Concurrency.ScheduledItem`1<!0>::d
	IL_0006:  ret 
    } // end of method ScheduledItem`1::GetDisposable

    // method line 86
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x26d0
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldc.i8 0x8000000000000000
	IL_0009:  stsfld int64 class System.Concurrency.ScheduledItem`1<!0>::count
	IL_000e:  ret 
    } // end of method ScheduledItem`1::.cctor

	.property instance class [System.Core]System.Action Action ()
	{
		.get instance default class [System.Core]System.Action System.Concurrency.ScheduledItem`1::get_Action () 
	}
	.property instance !TAbsolute DueTime ()
	{
		.get instance default !0 System.Concurrency.ScheduledItem`1::get_DueTime () 
	}
	.property instance bool IsCanceled ()
	{
		.get instance default bool System.Concurrency.ScheduledItem`1::get_IsCanceled () 
	}
  } // end of class System.Concurrency.ScheduledItem`1
}

.namespace System.Concurrency
{
  .class private auto ansi beforefieldinit ScheduledItem
  	extends class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>
  	implements class [mscorlib]System.IComparable`1<class System.Concurrency.ScheduledItem>  {

    // method line 87
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Core]System.Action action, valuetype [mscorlib]System.DateTimeOffset dueTime)  cil managed 
    {
        // Method begins at RVA 0x26e0
	// Code size 9 (0x9)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  ldarg.2 
	IL_0003:  call instance void class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::'.ctor'(class [System.Core]System.Action, !0)
	IL_0008:  ret 
    } // end of method ScheduledItem::.ctor

    // method line 88
    .method private final virtual hidebysig newslot 
           instance default int32 'System.IComparable<System.Concurrency.ScheduledItem>.CompareTo' (class System.Concurrency.ScheduledItem other)  cil managed 
    {
        // Method begins at RVA 0x26ea
	.override method instance int32 class [mscorlib]System.IComparable`1<class System.Concurrency.ScheduledItem>::CompareTo(!0)
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  call instance int32 class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::CompareTo(class System.Concurrency.ScheduledItem`1<!0>)
	IL_0007:  ret 
    } // end of method ScheduledItem::System.IComparable<System.Concurrency.ScheduledItem>.CompareTo

  } // end of class System.Concurrency.ScheduledItem
}

.namespace System.Concurrency
{
  .class public auto ansi beforefieldinit SynchronizationContextScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  private  class [mscorlib]System.Threading.SynchronizationContext context

    // method line 89
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Threading.SynchronizationContext context)  cil managed 
    {
        // Method begins at RVA 0x26f3
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [mscorlib]System.Threading.SynchronizationContext System.Concurrency.SynchronizationContextScheduler::context
	IL_000d:  ret 
    } // end of method SynchronizationContextScheduler::.ctor

    // method line 90
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x2702
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method SynchronizationContextScheduler::get_Now

    // method line 91
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x272c
	// Code size 74 (0x4a)
	.maxstack 4
	.locals init (
		class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'	V_0)
	IL_0000:  newobj instance void class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::action
	IL_000d:  ldloc.0 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::action
	IL_0013:  brtrue.s IL_0020

	IL_0015:  ldstr "action"
	IL_001a:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001f:  throw 
	IL_0020:  ldloc.0 
	IL_0021:  newobj instance void class System.Disposables.BooleanDisposable::'.ctor'()
	IL_0026:  stfld class System.Disposables.BooleanDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::d
	IL_002b:  ldarg.0 
	IL_002c:  ldfld class [mscorlib]System.Threading.SynchronizationContext System.Concurrency.SynchronizationContextScheduler::context
	IL_0031:  ldloc.0 
	IL_0032:  ldftn instance void class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::'<Schedule>b__0'(object)
	IL_0038:  newobj instance void class [mscorlib]System.Threading.SendOrPostCallback::'.ctor'(object, native int)
	IL_003d:  ldnull 
	IL_003e:  callvirt instance void class [mscorlib]System.Threading.SynchronizationContext::Post(class [mscorlib]System.Threading.SendOrPostCallback, object)
	IL_0043:  ldloc.0 
	IL_0044:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::d
	IL_0049:  ret 
    } // end of method SynchronizationContextScheduler::Schedule

    // method line 92
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x27a8
	// Code size 104 (0x68)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.TimeSpan	V_0,
		class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'	V_1)
	IL_0000:  newobj instance void class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::'.ctor'()
	IL_0005:  stloc.1 
	IL_0006:  ldloc.1 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::action
	IL_000d:  ldloc.1 
	IL_000e:  ldarg.0 
	IL_000f:  stfld class System.Concurrency.SynchronizationContextScheduler System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::'<>4__this'
	IL_0014:  ldloc.1 
	IL_0015:  ldfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::action
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "action"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldarg.2 
	IL_0028:  call valuetype [mscorlib]System.TimeSpan class System.Concurrency.Scheduler::Normalize(valuetype [mscorlib]System.TimeSpan)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.1 
	IL_002f:  ldc.i4.0 
	IL_0030:  newarr [mscorlib]System.IDisposable
	IL_0035:  newobj instance void class System.Disposables.CompositeDisposable::'.ctor'(class [mscorlib]System.IDisposable[])
	IL_003a:  stfld class System.Disposables.CompositeDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::g
	IL_003f:  ldloc.1 
	IL_0040:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::g
	IL_0045:  ldsfld class System.Concurrency.ThreadPoolScheduler System.Concurrency.ThreadPoolScheduler::Instance
	IL_004a:  ldloc.1 
	IL_004b:  ldftn instance void class System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::'<Schedule>b__3'()
	IL_0051:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0056:  ldloc.0 
	IL_0057:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.ThreadPoolScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_005c:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_0061:  ldloc.1 
	IL_0062:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::g
	IL_0067:  ret 
    } // end of method SynchronizationContextScheduler::Schedule

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.SynchronizationContextScheduler::get_Now () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass1'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.BooleanDisposable d
    .field  public  class [System.Core]System.Action action

    // method line 340
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2709
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass1::.ctor

    // method line 341
    .method public hidebysig 
           instance default void '<Schedule>b__0' (object _)  cil managed 
    {
        // Method begins at RVA 0x2711
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::d
	IL_0006:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_000b:  brtrue.s IL_0018

	IL_000d:  ldarg.0 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass1'::action
	IL_0013:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0018:  ret 
    } // end of method <>c__DisplayClass1::<Schedule>b__0

  } // end of class <>c__DisplayClass1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass4'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.CompositeDisposable g
    .field  public  class System.Concurrency.SynchronizationContextScheduler '<>4__this'
    .field  public  class [System.Core]System.Action action

    // method line 342
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2782
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass4::.ctor

    // method line 343
    .method public hidebysig 
           instance default void '<Schedule>b__3' ()  cil managed 
    {
        // Method begins at RVA 0x278a
	// Code size 29 (0x1d)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::g
	IL_0006:  ldarg.0 
	IL_0007:  ldfld class System.Concurrency.SynchronizationContextScheduler System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::'<>4__this'
	IL_000c:  ldarg.0 
	IL_000d:  ldfld class [System.Core]System.Action System.Concurrency.SynchronizationContextScheduler/'<>c__DisplayClass4'::action
	IL_0012:  call instance class [mscorlib]System.IDisposable class System.Concurrency.SynchronizationContextScheduler::Schedule(class [System.Core]System.Action)
	IL_0017:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_001c:  ret 
    } // end of method <>c__DisplayClass4::<Schedule>b__3

  } // end of class <>c__DisplayClass4

  } // end of class System.Concurrency.SynchronizationContextScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit TaskPoolScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  assembly static initonly  class System.Concurrency.TaskPoolScheduler Instance
    .field  private  class [System.Threading]System.Threading.Tasks.TaskFactory taskFactory

    // method line 93
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Threading]System.Threading.Tasks.TaskFactory taskFactory)  cil managed 
    {
        // Method begins at RVA 0x281c
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Threading]System.Threading.Tasks.TaskFactory System.Concurrency.TaskPoolScheduler::taskFactory
	IL_000d:  ret 
    } // end of method TaskPoolScheduler::.ctor

    // method line 94
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x282b
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method TaskPoolScheduler::get_Now

    // method line 95
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x28b0
	// Code size 70 (0x46)
	.maxstack 4
	.locals init (
		class System.Disposables.CancellationDisposable	V_0,
		class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'	V_1)
	IL_0000:  newobj instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'::'.ctor'()
	IL_0005:  stloc.1 
	IL_0006:  ldloc.1 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'::action
	IL_000d:  ldloc.1 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'::action
	IL_0013:  brtrue.s IL_0020

	IL_0015:  ldstr "action"
	IL_001a:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001f:  throw 
	IL_0020:  newobj instance void class System.Disposables.CancellationDisposable::'.ctor'()
	IL_0025:  stloc.0 
	IL_0026:  ldarg.0 
	IL_0027:  ldfld class [System.Threading]System.Threading.Tasks.TaskFactory System.Concurrency.TaskPoolScheduler::taskFactory
	IL_002c:  ldloc.1 
	IL_002d:  ldftn instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'::'<Schedule>b__0'()
	IL_0033:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0038:  ldloc.0 
	IL_0039:  callvirt instance valuetype [System.Threading]System.Threading.CancellationToken class System.Disposables.CancellationDisposable::get_Token()
	IL_003e:  callvirt instance class [System.Threading]System.Threading.Tasks.Task class [System.Threading]System.Threading.Tasks.TaskFactory::StartNew(class [System.Core]System.Action, valuetype [System.Threading]System.Threading.CancellationToken)
	IL_0043:  pop 
	IL_0044:  ldloc.0 
	IL_0045:  ret 
    } // end of method TaskPoolScheduler::Schedule

    // method line 96
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x2928
	// Code size 104 (0x68)
	.maxstack 5
	.locals init (
		valuetype [mscorlib]System.TimeSpan	V_0,
		class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'	V_1)
	IL_0000:  newobj instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::'.ctor'()
	IL_0005:  stloc.1 
	IL_0006:  ldloc.1 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::action
	IL_000d:  ldloc.1 
	IL_000e:  ldarg.0 
	IL_000f:  stfld class System.Concurrency.TaskPoolScheduler System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::'<>4__this'
	IL_0014:  ldloc.1 
	IL_0015:  ldfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::action
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "action"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldarg.2 
	IL_0028:  call valuetype [mscorlib]System.TimeSpan class System.Concurrency.Scheduler::Normalize(valuetype [mscorlib]System.TimeSpan)
	IL_002d:  stloc.0 
	IL_002e:  ldloc.1 
	IL_002f:  ldc.i4.0 
	IL_0030:  newarr [mscorlib]System.IDisposable
	IL_0035:  newobj instance void class System.Disposables.CompositeDisposable::'.ctor'(class [mscorlib]System.IDisposable[])
	IL_003a:  stfld class System.Disposables.CompositeDisposable System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::g
	IL_003f:  ldloc.1 
	IL_0040:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::g
	IL_0045:  ldsfld class System.Concurrency.ThreadPoolScheduler System.Concurrency.ThreadPoolScheduler::Instance
	IL_004a:  ldloc.1 
	IL_004b:  ldftn instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::'<Schedule>b__6'()
	IL_0051:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0056:  ldloc.0 
	IL_0057:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.ThreadPoolScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_005c:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_0061:  ldloc.1 
	IL_0062:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::g
	IL_0067:  ret 
    } // end of method TaskPoolScheduler::Schedule

    // method line 97
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x299c
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  call class [System.Threading]System.Threading.Tasks.TaskFactory class [System.Threading]System.Threading.Tasks.Task::get_Factory()
	IL_0005:  newobj instance void class System.Concurrency.TaskPoolScheduler::'.ctor'(class [System.Threading]System.Threading.Tasks.TaskFactory)
	IL_000a:  stsfld class System.Concurrency.TaskPoolScheduler System.Concurrency.TaskPoolScheduler::Instance
	IL_000f:  ret 
    } // end of method TaskPoolScheduler::.cctor

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.TaskPoolScheduler::get_Now () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass2'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class [System.Core]System.Action action

    // method line 344
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2832
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass2::.ctor

    // method line 345
    .method public hidebysig 
           instance default void '<Schedule>b__0' ()  cil managed 
    {
        // Method begins at RVA 0x2850
	// Code size 67 (0x43)
	.maxstack 3
	.locals init (
		class [mscorlib]System.Exception	V_0,
		class [mscorlib]System.Threading.Thread	V_1,
		class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'	V_2)
	.try { // 0
	  IL_0000:  ldarg.0 
	  IL_0001:  ldfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'::action
	  IL_0006:  callvirt instance void class [System.Core]System.Action::Invoke()
	  IL_000b:  leave.s IL_0042

	} // end .try 0
	catch class [mscorlib]System.Exception { // 0
	  IL_000d:  stloc.0 
	  IL_000e:  newobj instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'::'.ctor'()
	  IL_0013:  stloc.2 
	  IL_0014:  ldloc.2 
	  IL_0015:  ldarg.0 
	  IL_0016:  stfld class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2' System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'::'CS$<>8__locals3'
	  IL_001b:  ldloc.2 
	  IL_001c:  ldloc.0 
	  IL_001d:  stfld class [mscorlib]System.Exception System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'::ex
	  IL_0022:  ldloc.2 
	  IL_0023:  ldftn instance void class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'::'<Schedule>b__1'()
	  IL_0029:  newobj instance void class [mscorlib]System.Threading.ThreadStart::'.ctor'(object, native int)
	  IL_002e:  newobj instance void class [mscorlib]System.Threading.Thread::'.ctor'(class [mscorlib]System.Threading.ThreadStart)
	  IL_0033:  stloc.1 
	  IL_0034:  ldloc.1 
	  IL_0035:  callvirt instance void class [mscorlib]System.Threading.Thread::Start()
	  IL_003a:  ldloc.1 
	  IL_003b:  callvirt instance void class [mscorlib]System.Threading.Thread::Join()
	  IL_0040:  leave.s IL_0042

	} // end handler 0
	IL_0042:  ret 
    } // end of method <>c__DisplayClass2::<Schedule>b__0

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass4'
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2' 'CS$<>8__locals3'
    .field  public  class [mscorlib]System.Exception ex

    // method line 346
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x283a
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass4::.ctor

    // method line 347
    .method public hidebysig 
           instance default void '<Schedule>b__1' ()  cil managed 
    {
        // Method begins at RVA 0x2842
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Exception System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass2'/'<>c__DisplayClass4'::ex
	IL_0006:  call class [mscorlib]System.Exception class System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	IL_000b:  throw 
    } // end of method <>c__DisplayClass4::<Schedule>b__1

  } // end of class <>c__DisplayClass4

  } // end of class <>c__DisplayClass2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass7'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.CompositeDisposable g
    .field  public  class System.Concurrency.TaskPoolScheduler '<>4__this'
    .field  public  class [System.Core]System.Action action

    // method line 348
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2902
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass7::.ctor

    // method line 349
    .method public hidebysig 
           instance default void '<Schedule>b__6' ()  cil managed 
    {
        // Method begins at RVA 0x290a
	// Code size 29 (0x1d)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::g
	IL_0006:  ldarg.0 
	IL_0007:  ldfld class System.Concurrency.TaskPoolScheduler System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::'<>4__this'
	IL_000c:  ldarg.0 
	IL_000d:  ldfld class [System.Core]System.Action System.Concurrency.TaskPoolScheduler/'<>c__DisplayClass7'::action
	IL_0012:  call instance class [mscorlib]System.IDisposable class System.Concurrency.TaskPoolScheduler::Schedule(class [System.Core]System.Action)
	IL_0017:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_001c:  ret 
    } // end of method <>c__DisplayClass7::<Schedule>b__6

  } // end of class <>c__DisplayClass7

  } // end of class System.Concurrency.TaskPoolScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi beforefieldinit TestScheduler
  	extends class System.Concurrency.VirtualScheduler`2<int64,int64>
  {

    // method line 98
    .method public virtual hidebysig 
           instance default int64 Increment (int64 absolute, int64 relative)  cil managed 
    {
        // Method begins at RVA 0x29ad
	// Code size 4 (0x4)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.2 
	IL_0002:  add 
	IL_0003:  ret 
    } // end of method TestScheduler::Increment

    // method line 99
    .method public virtual hidebysig 
           instance default valuetype [mscorlib]System.DateTimeOffset ToDateTimeOffset (int64 absolute)  cil managed 
    {
        // Method begins at RVA 0x29b2
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0006:  newobj instance void valuetype [mscorlib]System.DateTimeOffset::'.ctor'(int64, valuetype [mscorlib]System.TimeSpan)
	IL_000b:  ret 
    } // end of method TestScheduler::ToDateTimeOffset

    // method line 100
    .method public virtual hidebysig 
           instance default int64 FromTimeSpan (valuetype [mscorlib]System.TimeSpan timeSpan)  cil managed 
    {
        // Method begins at RVA 0x29c0
	// Code size 18 (0x12)
	.maxstack 2
	.locals init (
		int64	V_0)
	IL_0000:  ldarga.s 1
	IL_0002:  call instance int64 valuetype [mscorlib]System.TimeSpan::get_Ticks()
	IL_0007:  stloc.0 
	IL_0008:  ldloc.0 
	IL_0009:  ldc.i4.0 
	IL_000a:  conv.i8 
	IL_000b:  bgt.s IL_0010

	IL_000d:  ldc.i4.1 
	IL_000e:  conv.i8 
	IL_000f:  ret 
	IL_0010:  ldloc.0 
	IL_0011:  ret 
    } // end of method TestScheduler::FromTimeSpan

    // method line 101
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x29de
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class System.Concurrency.VirtualScheduler`2<int64, int64>::'.ctor'()
	IL_0006:  ret 
    } // end of method TestScheduler::.ctor

  } // end of class System.Concurrency.TestScheduler
}

.namespace System.Disposables
{
  .class private auto ansi sealed beforefieldinit AnonymousDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private initonly  class [System.Core]System.Action dispose
    .field  private  int32 isDisposed

    // method line 102
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Core]System.Action dispose)  cil managed 
    {
        // Method begins at RVA 0x29e6
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Disposables.AnonymousDisposable::dispose
	IL_000d:  ret 
    } // end of method AnonymousDisposable::.ctor

    // method line 103
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x29f5
	// Code size 26 (0x1a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldflda int32 System.Disposables.AnonymousDisposable::isDisposed
	IL_0006:  ldc.i4.1 
	IL_0007:  call int32 class [mscorlib]System.Threading.Interlocked::Exchange([out] int32&, int32)
	IL_000c:  brtrue.s IL_0019

	IL_000e:  ldarg.0 
	IL_000f:  ldfld class [System.Core]System.Action System.Disposables.AnonymousDisposable::dispose
	IL_0014:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0019:  ret 
    } // end of method AnonymousDisposable::Dispose

  } // end of class System.Disposables.AnonymousDisposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit BooleanDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  bool '<IsDisposed>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 104
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2a10
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method BooleanDisposable::.ctor

    // method line 105
    .method public hidebysig specialname 
           instance default bool get_IsDisposed ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a18
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld bool System.Disposables.BooleanDisposable::'<IsDisposed>k__BackingField'
	IL_0006:  ret 
    } // end of method BooleanDisposable::get_IsDisposed

    // method line 106
    .method private hidebysig specialname 
           instance default void set_IsDisposed (bool 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a20
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld bool System.Disposables.BooleanDisposable::'<IsDisposed>k__BackingField'
	IL_0007:  ret 
    } // end of method BooleanDisposable::set_IsDisposed

    // method line 107
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2a29
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.1 
	IL_0002:  call instance void class System.Disposables.BooleanDisposable::set_IsDisposed(bool)
	IL_0007:  ret 
    } // end of method BooleanDisposable::Dispose

	.property instance bool IsDisposed ()
	{
		.get instance default bool System.Disposables.BooleanDisposable::get_IsDisposed () 
		.set instance default void System.Disposables.BooleanDisposable::set_IsDisposed (bool 'value') 
	}
  } // end of class System.Disposables.BooleanDisposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit CancellationDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  class [System.Threading]System.Threading.CancellationTokenSource cts

    // method line 108
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [System.Threading]System.Threading.CancellationTokenSource cts)  cil managed 
    {
        // Method begins at RVA 0x2a32
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "cts"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.1 
	IL_0016:  stfld class [System.Threading]System.Threading.CancellationTokenSource System.Disposables.CancellationDisposable::cts
	IL_001b:  ret 
    } // end of method CancellationDisposable::.ctor

    // method line 109
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2a4f
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void class [System.Threading]System.Threading.CancellationTokenSource::'.ctor'()
	IL_0006:  call instance void class System.Disposables.CancellationDisposable::'.ctor'(class [System.Threading]System.Threading.CancellationTokenSource)
	IL_000b:  ret 
    } // end of method CancellationDisposable::.ctor

    // method line 110
    .method public hidebysig specialname 
           instance default valuetype [System.Threading]System.Threading.CancellationToken get_Token ()  cil managed 
    {
        // Method begins at RVA 0x2a5c
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Threading]System.Threading.CancellationTokenSource System.Disposables.CancellationDisposable::cts
	IL_0006:  callvirt instance valuetype [System.Threading]System.Threading.CancellationToken class [System.Threading]System.Threading.CancellationTokenSource::get_Token()
	IL_000b:  ret 
    } // end of method CancellationDisposable::get_Token

    // method line 111
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2a69
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [System.Threading]System.Threading.CancellationTokenSource System.Disposables.CancellationDisposable::cts
	IL_0006:  callvirt instance void class [System.Threading]System.Threading.CancellationTokenSource::Cancel()
	IL_000b:  ret 
    } // end of method CancellationDisposable::Dispose

	.property instance valuetype [System.Threading]System.Threading.CancellationToken Token ()
	{
		.get instance default valuetype [System.Threading]System.Threading.CancellationToken System.Disposables.CancellationDisposable::get_Token () 
	}
  } // end of class System.Disposables.CancellationDisposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit ContextDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  class [mscorlib]System.IDisposable disposable
    .field  private  int32 disposed
    .field  private  class [mscorlib]System.Threading.SynchronizationContext '<Context>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 112
    .method public hidebysig specialname 
           instance default class [mscorlib]System.Threading.SynchronizationContext get_Context ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a76
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Threading.SynchronizationContext System.Disposables.ContextDisposable::'<Context>k__BackingField'
	IL_0006:  ret 
    } // end of method ContextDisposable::get_Context

    // method line 113
    .method private hidebysig specialname 
           instance default void set_Context (class [mscorlib]System.Threading.SynchronizationContext 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2a7e
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class [mscorlib]System.Threading.SynchronizationContext System.Disposables.ContextDisposable::'<Context>k__BackingField'
	IL_0007:  ret 
    } // end of method ContextDisposable::set_Context

    // method line 114
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Threading.SynchronizationContext context, class [mscorlib]System.IDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x2a87
	// Code size 49 (0x31)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "context"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.2 
	IL_0015:  brtrue.s IL_0022

	IL_0017:  ldstr "disposable"
	IL_001c:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0021:  throw 
	IL_0022:  ldarg.0 
	IL_0023:  ldarg.1 
	IL_0024:  call instance void class System.Disposables.ContextDisposable::set_Context(class [mscorlib]System.Threading.SynchronizationContext)
	IL_0029:  ldarg.0 
	IL_002a:  ldarg.2 
	IL_002b:  stfld class [mscorlib]System.IDisposable System.Disposables.ContextDisposable::disposable
	IL_0030:  ret 
    } // end of method ContextDisposable::.ctor

    // method line 115
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2ac6
	// Code size 39 (0x27)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldflda int32 System.Disposables.ContextDisposable::disposed
	IL_0006:  ldc.i4.1 
	IL_0007:  call int32 class [mscorlib]System.Threading.Interlocked::Exchange([out] int32&, int32)
	IL_000c:  brtrue.s IL_0026

	IL_000e:  ldarg.0 
	IL_000f:  call instance class [mscorlib]System.Threading.SynchronizationContext class System.Disposables.ContextDisposable::get_Context()
	IL_0014:  ldarg.0 
	IL_0015:  ldftn instance void class System.Disposables.ContextDisposable::'<Dispose>b__0'(object)
	IL_001b:  newobj instance void class [mscorlib]System.Threading.SendOrPostCallback::'.ctor'(object, native int)
	IL_0020:  ldnull 
	IL_0021:  callvirt instance void class [mscorlib]System.Threading.SynchronizationContext::Post(class [mscorlib]System.Threading.SendOrPostCallback, object)
	IL_0026:  ret 
    } // end of method ContextDisposable::Dispose

    // method line 116
    .method private hidebysig 
           instance default void '<Dispose>b__0' (object _)  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2ab9
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.IDisposable System.Disposables.ContextDisposable::disposable
	IL_0006:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_000b:  ret 
    } // end of method ContextDisposable::<Dispose>b__0

	.property instance class [mscorlib]System.Threading.SynchronizationContext Context ()
	{
		.set instance default void System.Disposables.ContextDisposable::set_Context (class [mscorlib]System.Threading.SynchronizationContext 'value') 
		.get instance default class [mscorlib]System.Threading.SynchronizationContext System.Disposables.ContextDisposable::get_Context () 
	}
  } // end of class System.Disposables.ContextDisposable
}

.namespace System.Disposables
{
  .class private auto ansi sealed beforefieldinit DefaultDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  public static initonly  class System.Disposables.DefaultDisposable Instance

    // method line 117
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2aee
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method DefaultDisposable::.ctor

    // method line 118
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2af6
	// Code size 1 (0x1)
	.maxstack 8
	IL_0000:  ret 
    } // end of method DefaultDisposable::Dispose

    // method line 119
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x2af8
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  newobj instance void class System.Disposables.DefaultDisposable::'.ctor'()
	IL_0005:  stsfld class System.Disposables.DefaultDisposable System.Disposables.DefaultDisposable::Instance
	IL_000a:  ret 
    } // end of method DefaultDisposable::.cctor

  } // end of class System.Disposables.DefaultDisposable
}

.namespace System.Disposables
{
  .class public auto ansi abstract sealed beforefieldinit Disposable
  	extends [mscorlib]System.Object
  {

    // method line 120
    .method public static hidebysig specialname 
           default class [mscorlib]System.IDisposable get_Empty ()  cil managed 
    {
        // Method begins at RVA 0x2b04
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Disposables.DefaultDisposable System.Disposables.DefaultDisposable::Instance
	IL_0005:  ret 
    } // end of method Disposable::get_Empty

    // method line 121
    .method public static hidebysig 
           default class [mscorlib]System.IDisposable Create (class [System.Core]System.Action dispose)  cil managed 
    {
        // Method begins at RVA 0x2b0b
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "dispose"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  newobj instance void class System.Disposables.AnonymousDisposable::'.ctor'(class [System.Core]System.Action)
	IL_0014:  ret 
    } // end of method Disposable::Create

	.property class [mscorlib]System.IDisposable Empty ()
	{
		.get default class [mscorlib]System.IDisposable System.Disposables.Disposable::get_Empty () 
	}
  } // end of class System.Disposables.Disposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit CompositeDisposable
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.Collections.Generic.ICollection`1<class [mscorlib]System.IDisposable>, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.IDisposable>, [mscorlib]System.Collections.IEnumerable, [mscorlib]System.IDisposable  {
    .field  private  bool disposed
    .field  private  class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> disposables

    // method line 122
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.IDisposable[] disposables)  cil managed 
    {
	.param [1]
	.custom instance void class [mscorlib]System.ParamArrayAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x2b24
	// Code size 67 (0x43)
	.maxstack 3
	.locals init (
		class [mscorlib]System.IDisposable	V_0,
		class [mscorlib]System.IDisposable[]	V_1,
		int32	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "disposables"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.1 
	IL_0015:  stloc.1 
	IL_0016:  ldc.i4.0 
	IL_0017:  stloc.2 
	IL_0018:  br.s IL_0030

	IL_001a:  ldloc.1 
	IL_001b:  ldloc.2 
	IL_001c:  ldelem.ref 
	IL_001d:  stloc.0 
	IL_001e:  ldloc.0 
	IL_001f:  brtrue.s IL_002c

	IL_0021:  ldstr "disposables"
	IL_0026:  newobj instance void class [mscorlib]System.ArgumentOutOfRangeException::'.ctor'(string)
	IL_002b:  throw 
	IL_002c:  ldloc.2 
	IL_002d:  ldc.i4.1 
	IL_002e:  add 
	IL_002f:  stloc.2 
	IL_0030:  ldloc.2 
	IL_0031:  ldloc.1 
	IL_0032:  ldlen 
	IL_0033:  conv.i4 
	IL_0034:  blt.s IL_001a

	IL_0036:  ldarg.0 
	IL_0037:  ldarg.1 
	IL_0038:  newobj instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::'.ctor'(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_003d:  stfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0042:  ret 
    } // end of method CompositeDisposable::.ctor

    // method line 123
    .method public final virtual hidebysig newslot specialname 
           instance default int32 get_Count ()  cil managed 
    {
        // Method begins at RVA 0x2b73
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0006:  callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::get_Count()
	IL_000b:  ret 
    } // end of method CompositeDisposable::get_Count

    // method line 124
    .method public final virtual hidebysig newslot 
           instance default void Add (class [mscorlib]System.IDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x2b80
	// Code size 75 (0x4b)
	.maxstack 2
	.locals init (
		bool	V_0,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "disposable"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldc.i4.0 
	IL_000f:  stloc.0 
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0016:  dup 
	IL_0017:  stloc.1 
	IL_0018:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_001d:  ldarg.0 
	  IL_001e:  ldfld bool System.Disposables.CompositeDisposable::disposed
	  IL_0023:  stloc.0 
	  IL_0024:  ldarg.0 
	  IL_0025:  ldfld bool System.Disposables.CompositeDisposable::disposed
	  IL_002a:  brtrue.s IL_0038

	  IL_002c:  ldarg.0 
	  IL_002d:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0032:  ldarg.1 
	  IL_0033:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::Add(!0)
	  IL_0038:  leave.s IL_0041

	} // end .try 0
	finally  { // 0
	  IL_003a:  ldloc.1 
	  IL_003b:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0040:  endfinally 
	} // end handler 0
	IL_0041:  ldloc.0 
	IL_0042:  brfalse.s IL_004a

	IL_0044:  ldarg.1 
	IL_0045:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_004a:  ret 
    } // end of method CompositeDisposable::Add

    // method line 125
    .method public final virtual hidebysig newslot 
           instance default bool Remove (class [mscorlib]System.IDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x2be8
	// Code size 70 (0x46)
	.maxstack 2
	.locals init (
		bool	V_0,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "disposable"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldc.i4.0 
	IL_000f:  stloc.0 
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0016:  dup 
	IL_0017:  stloc.1 
	IL_0018:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_001d:  ldarg.0 
	  IL_001e:  ldfld bool System.Disposables.CompositeDisposable::disposed
	  IL_0023:  brtrue.s IL_0032

	  IL_0025:  ldarg.0 
	  IL_0026:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_002b:  ldarg.1 
	  IL_002c:  callvirt instance bool class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::Remove(!0)
	  IL_0031:  stloc.0 
	  IL_0032:  leave.s IL_003b

	} // end .try 0
	finally  { // 0
	  IL_0034:  ldloc.1 
	  IL_0035:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003a:  endfinally 
	} // end handler 0
	IL_003b:  ldloc.0 
	IL_003c:  brfalse.s IL_0044

	IL_003e:  ldarg.1 
	IL_003f:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0044:  ldloc.0 
	IL_0045:  ret 
    } // end of method CompositeDisposable::Remove

    // method line 126
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2c4c
	// Code size 97 (0x61)
	.maxstack 2
	.locals init (
		class [mscorlib]System.IDisposable[]	V_0,
		class [mscorlib]System.IDisposable	V_1,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_2,
		class [mscorlib]System.IDisposable[]	V_3,
		int32	V_4)
	IL_0000:  ldnull 
	IL_0001:  stloc.0 
	IL_0002:  ldarg.0 
	IL_0003:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0008:  dup 
	IL_0009:  stloc.2 
	IL_000a:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000f:  ldarg.0 
	  IL_0010:  ldfld bool System.Disposables.CompositeDisposable::disposed
	  IL_0015:  brtrue.s IL_0035

	  IL_0017:  ldarg.0 
	  IL_0018:  ldc.i4.1 
	  IL_0019:  stfld bool System.Disposables.CompositeDisposable::disposed
	  IL_001e:  ldarg.0 
	  IL_001f:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0024:  callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::ToArray()
	  IL_0029:  stloc.0 
	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0030:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::Clear()
	  IL_0035:  leave.s IL_003e

	} // end .try 0
	finally  { // 0
	  IL_0037:  ldloc.2 
	  IL_0038:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003d:  endfinally 
	} // end handler 0
	IL_003e:  ldloc.0 
	IL_003f:  brfalse.s IL_0060

	IL_0041:  ldloc.0 
	IL_0042:  stloc.3 
	IL_0043:  ldc.i4.0 
	IL_0044:  stloc.s 4
	IL_0046:  br.s IL_0059

	IL_0048:  ldloc.3 
	IL_0049:  ldloc.s 4
	IL_004b:  ldelem.ref 
	IL_004c:  stloc.1 
	IL_004d:  ldloc.1 
	IL_004e:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0053:  ldloc.s 4
	IL_0055:  ldc.i4.1 
	IL_0056:  add 
	IL_0057:  stloc.s 4
	IL_0059:  ldloc.s 4
	IL_005b:  ldloc.3 
	IL_005c:  ldlen 
	IL_005d:  conv.i4 
	IL_005e:  blt.s IL_0048

	IL_0060:  ret 
    } // end of method CompositeDisposable::Dispose

    // method line 127
    .method public final virtual hidebysig newslot 
           instance default void Clear ()  cil managed 
    {
        // Method begins at RVA 0x2ccc
	// Code size 79 (0x4f)
	.maxstack 2
	.locals init (
		class [mscorlib]System.IDisposable[]	V_0,
		class [mscorlib]System.IDisposable	V_1,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_2,
		class [mscorlib]System.IDisposable[]	V_3,
		int32	V_4)
	IL_0000:  ldnull 
	IL_0001:  stloc.0 
	IL_0002:  ldarg.0 
	IL_0003:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0008:  dup 
	IL_0009:  stloc.2 
	IL_000a:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000f:  ldarg.0 
	  IL_0010:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0015:  callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::ToArray()
	  IL_001a:  stloc.0 
	  IL_001b:  ldarg.0 
	  IL_001c:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0021:  callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::Clear()
	  IL_0026:  leave.s IL_002f

	} // end .try 0
	finally  { // 0
	  IL_0028:  ldloc.2 
	  IL_0029:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_002e:  endfinally 
	} // end handler 0
	IL_002f:  ldloc.0 
	IL_0030:  stloc.3 
	IL_0031:  ldc.i4.0 
	IL_0032:  stloc.s 4
	IL_0034:  br.s IL_0047

	IL_0036:  ldloc.3 
	IL_0037:  ldloc.s 4
	IL_0039:  ldelem.ref 
	IL_003a:  stloc.1 
	IL_003b:  ldloc.1 
	IL_003c:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0041:  ldloc.s 4
	IL_0043:  ldc.i4.1 
	IL_0044:  add 
	IL_0045:  stloc.s 4
	IL_0047:  ldloc.s 4
	IL_0049:  ldloc.3 
	IL_004a:  ldlen 
	IL_004b:  conv.i4 
	IL_004c:  blt.s IL_0036

	IL_004e:  ret 
    } // end of method CompositeDisposable::Clear

    // method line 128
    .method public final virtual hidebysig newslot 
           instance default bool Contains (class [mscorlib]System.IDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x2d38
	// Code size 37 (0x25)
	.maxstack 2
	.locals init (
		bool	V_0,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0006:  dup 
	IL_0007:  stloc.1 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0013:  ldarg.1 
	  IL_0014:  callvirt instance bool class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::Contains(!0)
	  IL_0019:  stloc.0 
	  IL_001a:  leave.s IL_0023

	} // end .try 0
	finally  { // 0
	  IL_001c:  ldloc.1 
	  IL_001d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0022:  endfinally 
	} // end handler 0
	IL_0023:  ldloc.0 
	IL_0024:  ret 
    } // end of method CompositeDisposable::Contains

    // method line 129
    .method public final virtual hidebysig newslot 
           instance default void CopyTo (class [mscorlib]System.IDisposable[] 'array', int32 arrayIndex)  cil managed 
    {
        // Method begins at RVA 0x2d7c
	// Code size 77 (0x4d)
	.maxstack 6
	.locals init (
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "array"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  ldc.i4.0 
	IL_0010:  blt.s IL_0018

	IL_0012:  ldarg.2 
	IL_0013:  ldarg.1 
	IL_0014:  ldlen 
	IL_0015:  conv.i4 
	IL_0016:  blt.s IL_001e

	IL_0018:  newobj instance void class [mscorlib]System.IndexOutOfRangeException::'.ctor'()
	IL_001d:  throw 
	IL_001e:  ldarg.0 
	IL_001f:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0024:  dup 
	IL_0025:  stloc.0 
	IL_0026:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_002b:  ldarg.0 
	  IL_002c:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0031:  callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::ToArray()
	  IL_0036:  ldc.i4.0 
	  IL_0037:  ldarg.1 
	  IL_0038:  ldarg.2 
	  IL_0039:  ldarg.1 
	  IL_003a:  ldlen 
	  IL_003b:  conv.i4 
	  IL_003c:  ldarg.2 
	  IL_003d:  sub 
	  IL_003e:  call void class [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int32, class [mscorlib]System.Array, int32, int32)
	  IL_0043:  leave.s IL_004c

	} // end .try 0
	finally  { // 0
	  IL_0045:  ldloc.0 
	  IL_0046:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_004b:  endfinally 
	} // end handler 0
	IL_004c:  ret 
    } // end of method CompositeDisposable::CopyTo

    // method line 130
    .method public final virtual hidebysig newslot specialname 
           instance default bool get_IsReadOnly ()  cil managed 
    {
        // Method begins at RVA 0x2de8
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method CompositeDisposable::get_IsReadOnly

    // method line 131
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.IDisposable> GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x2dec
	// Code size 46 (0x2e)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.IDisposable>	V_0,
		class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	IL_0006:  dup 
	IL_0007:  stloc.1 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable> System.Disposables.CompositeDisposable::disposables
	  IL_0013:  callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.IDisposable>::ToArray()
	  IL_0018:  castclass class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.IDisposable>
	  IL_001d:  callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.IDisposable>::GetEnumerator()
	  IL_0022:  stloc.0 
	  IL_0023:  leave.s IL_002c

	} // end .try 0
	finally  { // 0
	  IL_0025:  ldloc.1 
	  IL_0026:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_002b:  endfinally 
	} // end handler 0
	IL_002c:  ldloc.0 
	IL_002d:  ret 
    } // end of method CompositeDisposable::GetEnumerator

    // method line 132
    .method private final virtual hidebysig newslot 
           instance default class [mscorlib]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()  cil managed 
    {
        // Method begins at RVA 0x2e38
	.override class [mscorlib]System.Collections.IEnumerable::GetEnumerator
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.IDisposable> class System.Disposables.CompositeDisposable::GetEnumerator()
	IL_0006:  ret 
    } // end of method CompositeDisposable::System.Collections.IEnumerable.GetEnumerator

	.property instance int32 Count ()
	{
		.get instance default int32 System.Disposables.CompositeDisposable::get_Count () 
	}
	.property instance bool IsReadOnly ()
	{
		.get instance default bool System.Disposables.CompositeDisposable::get_IsReadOnly () 
	}
  } // end of class System.Disposables.CompositeDisposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit RefCountDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  bool isPrimaryDisposed
    .field  private  bool isUnderlyingDisposed
    .field  private  class [mscorlib]System.IDisposable underlyingDisposable
    .field  private  int32 count
    .field  private  object gate

    // method line 133
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.IDisposable underlyingDisposable)  cil managed 
    {
        // Method begins at RVA 0x2e40
	// Code size 60 (0x3c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "underlyingDisposable"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.0 
	IL_0015:  ldc.i4.0 
	IL_0016:  stfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	IL_001b:  ldarg.0 
	IL_001c:  ldc.i4.0 
	IL_001d:  stfld bool System.Disposables.RefCountDisposable::isPrimaryDisposed
	IL_0022:  ldarg.0 
	IL_0023:  newobj instance void object::'.ctor'()
	IL_0028:  stfld object System.Disposables.RefCountDisposable::gate
	IL_002d:  ldarg.0 
	IL_002e:  ldc.i4.0 
	IL_002f:  stfld int32 System.Disposables.RefCountDisposable::count
	IL_0034:  ldarg.0 
	IL_0035:  ldarg.1 
	IL_0036:  stfld class [mscorlib]System.IDisposable System.Disposables.RefCountDisposable::underlyingDisposable
	IL_003b:  ret 
    } // end of method RefCountDisposable::.ctor

    // method line 134
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2e80
	// Code size 79 (0x4f)
	.maxstack 2
	.locals init (
		bool	V_0,
		object	V_1)
	IL_0000:  ldc.i4.0 
	IL_0001:  stloc.0 
	IL_0002:  ldarg.0 
	IL_0003:  ldfld object System.Disposables.RefCountDisposable::gate
	IL_0008:  dup 
	IL_0009:  stloc.1 
	IL_000a:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000f:  ldarg.0 
	  IL_0010:  ldfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	  IL_0015:  brtrue.s IL_0037

	  IL_0017:  ldarg.0 
	  IL_0018:  ldfld bool System.Disposables.RefCountDisposable::isPrimaryDisposed
	  IL_001d:  brtrue.s IL_0037

	  IL_001f:  ldarg.0 
	  IL_0020:  ldc.i4.1 
	  IL_0021:  stfld bool System.Disposables.RefCountDisposable::isPrimaryDisposed
	  IL_0026:  ldarg.0 
	  IL_0027:  ldfld int32 System.Disposables.RefCountDisposable::count
	  IL_002c:  brtrue.s IL_0037

	  IL_002e:  ldarg.0 
	  IL_002f:  ldc.i4.1 
	  IL_0030:  stfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	  IL_0035:  ldc.i4.1 
	  IL_0036:  stloc.0 
	  IL_0037:  leave.s IL_0040

	} // end .try 0
	finally  { // 0
	  IL_0039:  ldloc.1 
	  IL_003a:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003f:  endfinally 
	} // end handler 0
	IL_0040:  ldloc.0 
	IL_0041:  brfalse.s IL_004e

	IL_0043:  ldarg.0 
	IL_0044:  ldfld class [mscorlib]System.IDisposable System.Disposables.RefCountDisposable::underlyingDisposable
	IL_0049:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_004e:  ret 
    } // end of method RefCountDisposable::Dispose

    // method line 135
    .method public hidebysig 
           instance default class [mscorlib]System.IDisposable GetDisposable ()  cil managed 
    {
        // Method begins at RVA 0x2eec
	// Code size 47 (0x2f)
	.maxstack 2
	.locals init (
		class [mscorlib]System.IDisposable	V_0,
		object	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object System.Disposables.RefCountDisposable::gate
	IL_0006:  dup 
	IL_0007:  stloc.1 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	  IL_0013:  brfalse.s IL_001d

	  IL_0015:  call class [mscorlib]System.IDisposable class System.Disposables.Disposable::get_Empty()
	  IL_001a:  stloc.0 
	  IL_001b:  leave.s IL_002d

	  IL_001d:  ldarg.0 
	  IL_001e:  newobj instance void class System.Disposables.RefCountDisposable/InnerDisposable::'.ctor'(class System.Disposables.RefCountDisposable)
	  IL_0023:  stloc.0 
	  IL_0024:  leave.s IL_002d

	} // end .try 0
	finally  { // 0
	  IL_0026:  ldloc.1 
	  IL_0027:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_002c:  endfinally 
	} // end handler 0
	IL_002d:  ldloc.0 
	IL_002e:  ret 
    } // end of method RefCountDisposable::GetDisposable

  .class nested private auto ansi sealed beforefieldinit InnerDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  bool isInnerDisposed
    .field  private  class System.Disposables.RefCountDisposable disposable

    // method line 136
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class System.Disposables.RefCountDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x2f38
	// Code size 33 (0x21)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	IL_000d:  ldarg.0 
	IL_000e:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	IL_0013:  dup 
	IL_0014:  ldfld int32 System.Disposables.RefCountDisposable::count
	IL_0019:  ldc.i4.1 
	IL_001a:  add 
	IL_001b:  stfld int32 System.Disposables.RefCountDisposable::count
	IL_0020:  ret 
    } // end of method InnerDisposable::.ctor

    // method line 137
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x2f5c
	// Code size 136 (0x88)
	.maxstack 3
	.locals init (
		bool	V_0,
		object	V_1)
	IL_0000:  ldc.i4.0 
	IL_0001:  stloc.0 
	IL_0002:  ldarg.0 
	IL_0003:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	IL_0008:  ldfld object System.Disposables.RefCountDisposable::gate
	IL_000d:  dup 
	IL_000e:  stloc.1 
	IL_000f:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0014:  ldarg.0 
	  IL_0015:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	  IL_001a:  ldfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	  IL_001f:  brtrue.s IL_006b

	  IL_0021:  ldarg.0 
	  IL_0022:  ldfld bool System.Disposables.RefCountDisposable/InnerDisposable::isInnerDisposed
	  IL_0027:  brtrue.s IL_006b

	  IL_0029:  ldarg.0 
	  IL_002a:  ldc.i4.1 
	  IL_002b:  stfld bool System.Disposables.RefCountDisposable/InnerDisposable::isInnerDisposed
	  IL_0030:  ldarg.0 
	  IL_0031:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	  IL_0036:  dup 
	  IL_0037:  ldfld int32 System.Disposables.RefCountDisposable::count
	  IL_003c:  ldc.i4.1 
	  IL_003d:  sub 
	  IL_003e:  stfld int32 System.Disposables.RefCountDisposable::count
	  IL_0043:  ldarg.0 
	  IL_0044:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	  IL_0049:  ldfld int32 System.Disposables.RefCountDisposable::count
	  IL_004e:  brtrue.s IL_006b

	  IL_0050:  ldarg.0 
	  IL_0051:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	  IL_0056:  ldfld bool System.Disposables.RefCountDisposable::isPrimaryDisposed
	  IL_005b:  brfalse.s IL_006b

	  IL_005d:  ldarg.0 
	  IL_005e:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	  IL_0063:  ldc.i4.1 
	  IL_0064:  stfld bool System.Disposables.RefCountDisposable::isUnderlyingDisposed
	  IL_0069:  ldc.i4.1 
	  IL_006a:  stloc.0 
	  IL_006b:  leave.s IL_0074

	} // end .try 0
	finally  { // 0
	  IL_006d:  ldloc.1 
	  IL_006e:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0073:  endfinally 
	} // end handler 0
	IL_0074:  ldloc.0 
	IL_0075:  brfalse.s IL_0087

	IL_0077:  ldarg.0 
	IL_0078:  ldfld class System.Disposables.RefCountDisposable System.Disposables.RefCountDisposable/InnerDisposable::disposable
	IL_007d:  ldfld class [mscorlib]System.IDisposable System.Disposables.RefCountDisposable::underlyingDisposable
	IL_0082:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0087:  ret 
    } // end of method InnerDisposable::Dispose

  } // end of class InnerDisposable

  } // end of class System.Disposables.RefCountDisposable
}

.namespace System.Disposables
{
  .class public auto ansi sealed beforefieldinit MutableDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  object gate
    .field  private  class [mscorlib]System.IDisposable current
    .field  private  bool disposed

    // method line 138
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3000
	// Code size 18 (0x12)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void object::'.ctor'()
	IL_0006:  stfld object System.Disposables.MutableDisposable::gate
	IL_000b:  ldarg.0 
	IL_000c:  call instance void object::'.ctor'()
	IL_0011:  ret 
    } // end of method MutableDisposable::.ctor

    // method line 139
    .method public hidebysig specialname 
           instance default class [mscorlib]System.IDisposable get_Disposable ()  cil managed 
    {
        // Method begins at RVA 0x3013
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	IL_0006:  ret 
    } // end of method MutableDisposable::get_Disposable

    // method line 140
    .method public hidebysig specialname 
           instance default void set_Disposable (class [mscorlib]System.IDisposable 'value')  cil managed 
    {
        // Method begins at RVA 0x301c
	// Code size 73 (0x49)
	.maxstack 2
	.locals init (
		bool	V_0,
		object	V_1)
	IL_0000:  ldc.i4.0 
	IL_0001:  stloc.0 
	IL_0002:  ldarg.0 
	IL_0003:  ldfld object System.Disposables.MutableDisposable::gate
	IL_0008:  dup 
	IL_0009:  stloc.1 
	IL_000a:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000f:  ldarg.0 
	  IL_0010:  ldfld bool System.Disposables.MutableDisposable::disposed
	  IL_0015:  stloc.0 
	  IL_0016:  ldloc.0 
	  IL_0017:  brtrue.s IL_0033

	  IL_0019:  ldarg.0 
	  IL_001a:  ldfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_001f:  brfalse.s IL_002c

	  IL_0021:  ldarg.0 
	  IL_0022:  ldfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_0027:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_002c:  ldarg.0 
	  IL_002d:  ldarg.1 
	  IL_002e:  stfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_0033:  leave.s IL_003c

	} // end .try 0
	finally  { // 0
	  IL_0035:  ldloc.1 
	  IL_0036:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003b:  endfinally 
	} // end handler 0
	IL_003c:  ldloc.0 
	IL_003d:  brfalse.s IL_0048

	IL_003f:  ldarg.1 
	IL_0040:  brfalse.s IL_0048

	IL_0042:  ldarg.1 
	IL_0043:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0048:  ret 
    } // end of method MutableDisposable::set_Disposable

    // method line 141
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3084
	// Code size 64 (0x40)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object System.Disposables.MutableDisposable::gate
	IL_0006:  dup 
	IL_0007:  stloc.0 
	IL_0008:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000d:  ldarg.0 
	  IL_000e:  ldfld bool System.Disposables.MutableDisposable::disposed
	  IL_0013:  brtrue.s IL_0036

	  IL_0015:  ldarg.0 
	  IL_0016:  ldc.i4.1 
	  IL_0017:  stfld bool System.Disposables.MutableDisposable::disposed
	  IL_001c:  ldarg.0 
	  IL_001d:  ldfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_0022:  brfalse.s IL_0036

	  IL_0024:  ldarg.0 
	  IL_0025:  ldfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_002a:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_002f:  ldarg.0 
	  IL_0030:  ldnull 
	  IL_0031:  stfld class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::current
	  IL_0036:  leave.s IL_003f

	} // end .try 0
	finally  { // 0
	  IL_0038:  ldloc.0 
	  IL_0039:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003e:  endfinally 
	} // end handler 0
	IL_003f:  ret 
    } // end of method MutableDisposable::Dispose

	.property instance class [mscorlib]System.IDisposable Disposable ()
	{
		.get instance default class [mscorlib]System.IDisposable System.Disposables.MutableDisposable::get_Disposable () 
		.set instance default void System.Disposables.MutableDisposable::set_Disposable (class [mscorlib]System.IDisposable 'value') 
	}
  } // end of class System.Disposables.MutableDisposable
}

.namespace System.Disposables
{
  .class public auto ansi beforefieldinit ScheduledDisposable
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {
    .field  private  int32 disposed
    .field  private  class [mscorlib]System.IDisposable '<Disposable>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  class System.Concurrency.IScheduler '<Scheduler>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 142
    .method public hidebysig specialname 
           instance default class [mscorlib]System.IDisposable get_Disposable ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x30e0
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.IDisposable System.Disposables.ScheduledDisposable::'<Disposable>k__BackingField'
	IL_0006:  ret 
    } // end of method ScheduledDisposable::get_Disposable

    // method line 143
    .method private hidebysig specialname 
           instance default void set_Disposable (class [mscorlib]System.IDisposable 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x30e8
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class [mscorlib]System.IDisposable System.Disposables.ScheduledDisposable::'<Disposable>k__BackingField'
	IL_0007:  ret 
    } // end of method ScheduledDisposable::set_Disposable

    // method line 144
    .method public hidebysig specialname 
           instance default class System.Concurrency.IScheduler get_Scheduler ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x30f1
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.IScheduler System.Disposables.ScheduledDisposable::'<Scheduler>k__BackingField'
	IL_0006:  ret 
    } // end of method ScheduledDisposable::get_Scheduler

    // method line 145
    .method private hidebysig specialname 
           instance default void set_Scheduler (class System.Concurrency.IScheduler 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x30f9
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld class System.Concurrency.IScheduler System.Disposables.ScheduledDisposable::'<Scheduler>k__BackingField'
	IL_0007:  ret 
    } // end of method ScheduledDisposable::set_Scheduler

    // method line 146
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class System.Concurrency.IScheduler scheduler, class [mscorlib]System.IDisposable disposable)  cil managed 
    {
        // Method begins at RVA 0x3102
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  call instance void class System.Disposables.ScheduledDisposable::set_Scheduler(class System.Concurrency.IScheduler)
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  call instance void class System.Disposables.ScheduledDisposable::set_Disposable(class [mscorlib]System.IDisposable)
	IL_0014:  ret 
    } // end of method ScheduledDisposable::.ctor

    // method line 147
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3118
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance class System.Concurrency.IScheduler class System.Disposables.ScheduledDisposable::get_Scheduler()
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Disposables.ScheduledDisposable::DisposeInner()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_0017:  pop 
	IL_0018:  ret 
    } // end of method ScheduledDisposable::Dispose

    // method line 148
    .method private hidebysig 
           instance default void DisposeInner ()  cil managed 
    {
        // Method begins at RVA 0x3132
	// Code size 26 (0x1a)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldflda int32 System.Disposables.ScheduledDisposable::disposed
	IL_0006:  ldc.i4.1 
	IL_0007:  call int32 class [mscorlib]System.Threading.Interlocked::Exchange([out] int32&, int32)
	IL_000c:  brtrue.s IL_0019

	IL_000e:  ldarg.0 
	IL_000f:  call instance class [mscorlib]System.IDisposable class System.Disposables.ScheduledDisposable::get_Disposable()
	IL_0014:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	IL_0019:  ret 
    } // end of method ScheduledDisposable::DisposeInner

	.property instance class [mscorlib]System.IDisposable Disposable ()
	{
		.get instance default class [mscorlib]System.IDisposable System.Disposables.ScheduledDisposable::get_Disposable () 
		.set instance default void System.Disposables.ScheduledDisposable::set_Disposable (class [mscorlib]System.IDisposable 'value') 
	}
	.property instance class System.Concurrency.IScheduler Scheduler ()
	{
		.get instance default class System.Concurrency.IScheduler System.Disposables.ScheduledDisposable::get_Scheduler () 
		.set instance default void System.Disposables.ScheduledDisposable::set_Scheduler (class System.Concurrency.IScheduler 'value') 
	}
  } // end of class System.Disposables.ScheduledDisposable
}

.namespace System.Diagnostics
{
  .class public auto ansi abstract sealed beforefieldinit ExceptionExtensions
  	extends [mscorlib]System.Object
  {
    .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field private static literal  string ExceptionPrepForRemotingMethodName = "PrepForRemoting"
    .field  private static initonly  class [mscorlib]System.Reflection.MethodInfo prepForRemoting

    // method line 149
    .method public static hidebysig 
           default class [mscorlib]System.Exception PrepareForRethrow (class [mscorlib]System.Exception exception)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x314d
	// Code size 34 (0x22)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "exception"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldsfld class [mscorlib]System.Reflection.MethodInfo System.Diagnostics.ExceptionExtensions::prepForRemoting
	IL_0013:  ldarg.0 
	IL_0014:  ldc.i4.0 
	IL_0015:  newarr [mscorlib]System.Object
	IL_001a:  callvirt instance object class [mscorlib]System.Reflection.MethodBase::Invoke(object, object[])
	IL_001f:  pop 
	IL_0020:  ldarg.0 
	IL_0021:  ret 
    } // end of method ExceptionExtensions::PrepareForRethrow

    // method line 150
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x3170
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldtoken [mscorlib]System.Exception
	IL_0005:  call class [mscorlib]System.Type class [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
	IL_000a:  ldstr "PrepForRemoting"
	IL_000f:  ldc.i4.s 0x24
	IL_0011:  call instance class [mscorlib]System.Reflection.MethodInfo class [mscorlib]System.Type::GetMethod(string, valuetype [mscorlib]System.Reflection.BindingFlags)
	IL_0016:  stsfld class [mscorlib]System.Reflection.MethodInfo System.Diagnostics.ExceptionExtensions::prepForRemoting
	IL_001b:  ret 
    } // end of method ExceptionExtensions::.cctor

  } // end of class System.Diagnostics.ExceptionExtensions
}

.namespace System
{
  .class public auto ansi sealed Func`6<T1,T2,T3,T4,T5,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 151
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`6::.ctor

    // method line 152
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`6::Invoke

    // method line 153
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`6::BeginInvoke

    // method line 154
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`6::EndInvoke

  } // end of class System.Func`6
}

.namespace System
{
  .class public auto ansi sealed Func`7<T1,T2,T3,T4,T5,T6,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 155
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`7::.ctor

    // method line 156
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`7::Invoke

    // method line 157
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`7::BeginInvoke

    // method line 158
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`7::EndInvoke

  } // end of class System.Func`7
}

.namespace System
{
  .class public auto ansi sealed Func`8<T1,T2,T3,T4,T5,T6,T7,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 159
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`8::.ctor

    // method line 160
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`8::Invoke

    // method line 161
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`8::BeginInvoke

    // method line 162
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`8::EndInvoke

  } // end of class System.Func`8
}

.namespace System
{
  .class public auto ansi sealed Func`9<T1,T2,T3,T4,T5,T6,T7,T8,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 163
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`9::.ctor

    // method line 164
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`9::Invoke

    // method line 165
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`9::BeginInvoke

    // method line 166
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`9::EndInvoke

  } // end of class System.Func`9
}

.namespace System
{
  .class public auto ansi sealed Func`10<T1,T2,T3,T4,T5,T6,T7,T8,T9,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 167
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`10::.ctor

    // method line 168
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`10::Invoke

    // method line 169
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`10::BeginInvoke

    // method line 170
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`10::EndInvoke

  } // end of class System.Func`10
}

.namespace System
{
  .class public auto ansi sealed Func`11<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 171
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`11::.ctor

    // method line 172
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`11::Invoke

    // method line 173
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`11::BeginInvoke

    // method line 174
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`11::EndInvoke

  } // end of class System.Func`11
}

.namespace System
{
  .class public auto ansi sealed Func`12<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 175
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`12::.ctor

    // method line 176
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`12::Invoke

    // method line 177
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`12::BeginInvoke

    // method line 178
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`12::EndInvoke

  } // end of class System.Func`12
}

.namespace System
{
  .class public auto ansi sealed Func`13<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 179
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`13::.ctor

    // method line 180
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`13::Invoke

    // method line 181
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`13::BeginInvoke

    // method line 182
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`13::EndInvoke

  } // end of class System.Func`13
}

.namespace System
{
  .class public auto ansi sealed Func`14<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 183
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`14::.ctor

    // method line 184
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`14::Invoke

    // method line 185
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`14::BeginInvoke

    // method line 186
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`14::EndInvoke

  } // end of class System.Func`14
}

.namespace System
{
  .class public auto ansi sealed Func`15<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 187
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`15::.ctor

    // method line 188
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`15::Invoke

    // method line 189
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`15::BeginInvoke

    // method line 190
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`15::EndInvoke

  } // end of class System.Func`15
}

.namespace System
{
  .class public auto ansi sealed Func`16<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 191
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`16::.ctor

    // method line 192
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`16::Invoke

    // method line 193
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`16::BeginInvoke

    // method line 194
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`16::EndInvoke

  } // end of class System.Func`16
}

.namespace System
{
  .class public auto ansi sealed Func`17<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TResult>
  	extends [mscorlib]System.MulticastDelegate
  {

    // method line 195
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object 'object', native int 'method')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`17::.ctor

    // method line 196
    .method public virtual hidebysig newslot 
           instance default !TResult Invoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, !T16 arg16)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`17::Invoke

    // method line 197
    .method public virtual hidebysig newslot 
           instance default class [mscorlib]System.IAsyncResult BeginInvoke (!T1 arg1, !T2 arg2, !T3 arg3, !T4 arg4, !T5 arg5, !T6 arg6, !T7 arg7, !T8 arg8, !T9 arg9, !T10 arg10, !T11 arg11, !T12 arg12, !T13 arg13, !T14 arg14, !T15 arg15, !T16 arg16, class [mscorlib]System.AsyncCallback callback, object 'object')  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`17::BeginInvoke

    // method line 198
    .method public virtual hidebysig newslot 
           instance default !TResult EndInvoke (class [mscorlib]System.IAsyncResult result)  runtime managed 
    {
        // Method begins at RVA 0x0
          // Disassembly of native methods is not supported
    } // end of method Func`17::EndInvoke

  } // end of class System.Func`17
}

.namespace System
{
  .class interface public auto ansi abstract IObserver`2<TValue,TResult>
  {

    // method line 199
    .method public virtual hidebysig newslot abstract 
           instance default !TResult OnNext (!TValue 'value')  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IObserver`2::OnNext

    // method line 200
    .method public virtual hidebysig newslot abstract 
           instance default !TResult OnError (class [mscorlib]System.Exception exception)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IObserver`2::OnError

    // method line 201
    .method public virtual hidebysig newslot abstract 
           instance default !TResult OnCompleted ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IObserver`2::OnCompleted

  } // end of class System.IObserver`2
}

.namespace System.Collections.Generic
{
  .class public auto ansi sealed NotificationKind
  	extends [mscorlib]System.Enum
  {
    .field  public specialname rtspecialname  int32 value__
    .field public static literal  valuetype System.Collections.Generic.NotificationKind OnNext = int32(0x00000000)
    .field public static literal  valuetype System.Collections.Generic.NotificationKind OnError = int32(0x00000001)
    .field public static literal  valuetype System.Collections.Generic.NotificationKind OnCompleted = int32(0x00000002)

  } // end of class System.Collections.Generic.NotificationKind
}

.namespace System.Collections.Generic
{
  .class public auto ansi abstract beforefieldinit Notification`1<T>
  	extends [mscorlib]System.Object
  	implements class [mscorlib]System.IEquatable`1<class System.Collections.Generic.Notification`1<!0>>  {

    // method line 202
    .method assembly hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x318d
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method Notification`1::.ctor

    // method line 203
    .method public virtual hidebysig newslot abstract specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::get_Value

    // method line 204
    .method public virtual hidebysig newslot abstract specialname 
           instance default bool get_HasValue ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::get_HasValue

    // method line 205
    .method public virtual hidebysig newslot abstract specialname 
           instance default class [mscorlib]System.Exception get_Exception ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::get_Exception

    // method line 206
    .method public virtual hidebysig newslot abstract specialname 
           instance default valuetype System.Collections.Generic.NotificationKind get_Kind ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::get_Kind

    // method line 207
    .method public virtual hidebysig newslot abstract 
           instance default bool Equals (class System.Collections.Generic.Notification`1<!T> other)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::Equals

    // method line 208
    .method public virtual hidebysig 
           instance default bool Equals (object obj)  cil managed 
    {
        // Method begins at RVA 0x3195
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  isinst class System.Collections.Generic.Notification`1<!0>
	IL_0007:  callvirt instance bool class System.Collections.Generic.Notification`1<!T>::Equals(class System.Collections.Generic.Notification`1<!0>)
	IL_000c:  ret 
    } // end of method Notification`1::Equals

    // method line 209
    .method public static hidebysig specialname 
           default bool op_Equality (class System.Collections.Generic.Notification`1<!T> left, class System.Collections.Generic.Notification`1<!T> right)  cil managed 
    {
        // Method begins at RVA 0x31a3
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  call bool object::ReferenceEquals(object, object)
	IL_0007:  brfalse.s IL_000b

	IL_0009:  ldc.i4.1 
	IL_000a:  ret 
	IL_000b:  ldarg.0 
	IL_000c:  brfalse.s IL_0011

	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_0013

	IL_0011:  ldc.i4.0 
	IL_0012:  ret 
	IL_0013:  ldarg.0 
	IL_0014:  ldarg.1 
	IL_0015:  callvirt instance bool class System.Collections.Generic.Notification`1<!T>::Equals(class System.Collections.Generic.Notification`1<!0>)
	IL_001a:  ret 
    } // end of method Notification`1::op_Equality

    // method line 210
    .method public static hidebysig specialname 
           default bool op_Inequality (class System.Collections.Generic.Notification`1<!T> left, class System.Collections.Generic.Notification`1<!T> right)  cil managed 
    {
        // Method begins at RVA 0x31bf
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  call bool class System.Collections.Generic.Notification`1<!T>::op_Equality(class System.Collections.Generic.Notification`1<!0>, class System.Collections.Generic.Notification`1<!0>)
	IL_0007:  ldc.i4.0 
	IL_0008:  ceq 
	IL_000a:  ret 
    } // end of method Notification`1::op_Inequality

    // method line 211
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x31cb
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance int32 object::GetHashCode()
	IL_0006:  ret 
    } // end of method Notification`1::GetHashCode

    // method line 212
    .method public virtual hidebysig newslot abstract 
           instance default void Accept (class [System.Observable]System.IObserver`1<!T> observer)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::Accept

    // method line 213
    .method public virtual hidebysig newslot abstract 
           instance default !!TResult Accept<TResult> (class System.IObserver`2<!T, !!TResult> observer)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::Accept

    // method line 214
    .method public virtual hidebysig newslot abstract 
           instance default void Accept (class [mscorlib]System.Action`1<!T> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::Accept

    // method line 215
    .method public virtual hidebysig newslot abstract 
           instance default !!TResult Accept<TResult> (class [System.Core]System.Func`2<!T, !!TResult> onNext, class [System.Core]System.Func`2<class [mscorlib]System.Exception, !!TResult> onError, class [System.Core]System.Func`1<!!TResult> onCompleted)  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method Notification`1::Accept

	.property instance !T Value ()
	{
		.get instance default !0 System.Collections.Generic.Notification`1::get_Value () 
	}
	.property instance bool HasValue ()
	{
		.get instance default bool System.Collections.Generic.Notification`1::get_HasValue () 
	}
	.property instance class [mscorlib]System.Exception Exception ()
	{
		.get instance default class [mscorlib]System.Exception System.Collections.Generic.Notification`1::get_Exception () 
	}
	.property instance valuetype System.Collections.Generic.NotificationKind Kind ()
	{
		.get instance default valuetype System.Collections.Generic.NotificationKind System.Collections.Generic.Notification`1::get_Kind () 
	}
  .class nested public auto ansi sealed beforefieldinit OnNext<T>
  	extends class System.Collections.Generic.Notification`1<!0>
  {
    .custom instance void class [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::'.ctor'(string) =  (
		01 00 0F 4F 6E 4E 65 78 74 28 7B 56 61 6C 75 65   // ...OnNext({Value
		7D 29 00 00                                     ) // })..

    .field  private  !0 'value'

    // method line 216
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (!T 'value')  cil managed 
    {
        // Method begins at RVA 0x31d3
	// Code size 14 (0xe)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class System.Collections.Generic.Notification`1<!T>::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld !0 class System.Collections.Generic.Notification`1/OnNext<!0>::'value'
	IL_000d:  ret 
    } // end of method OnNext::.ctor

    // method line 217
    .method public virtual hidebysig specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x31e2
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Collections.Generic.Notification`1/OnNext<!0>::'value'
	IL_0006:  ret 
    } // end of method OnNext::get_Value

    // method line 218
    .method public virtual hidebysig specialname 
           instance default class [mscorlib]System.Exception get_Exception ()  cil managed 
    {
        // Method begins at RVA 0x31ea
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldnull 
	IL_0001:  ret 
    } // end of method OnNext::get_Exception

    // method line 219
    .method public virtual hidebysig specialname 
           instance default bool get_HasValue ()  cil managed 
    {
        // Method begins at RVA 0x31ed
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.1 
	IL_0001:  ret 
    } // end of method OnNext::get_HasValue

    // method line 220
    .method public virtual hidebysig specialname 
           instance default valuetype System.Collections.Generic.NotificationKind get_Kind ()  cil managed 
    {
        // Method begins at RVA 0x31f0
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method OnNext::get_Kind

    // method line 221
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x31f3
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::get_Default()
	IL_0005:  ldarg.0 
	IL_0006:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_000b:  callvirt instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::GetHashCode(!0)
	IL_0010:  ret 
    } // end of method OnNext::GetHashCode

    // method line 222
    .method public virtual hidebysig 
           instance default bool Equals (class System.Collections.Generic.Notification`1<!T> other)  cil managed 
    {
        // Method begins at RVA 0x3208
	// Code size 41 (0x29)
	.maxstack 3
	.locals init (
		class System.Collections.Generic.Notification`1/OnNext<!T>	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  isinst class System.Collections.Generic.Notification`1/OnNext<!0>
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldnull 
	IL_0009:  call bool class System.Collections.Generic.Notification`1<!T>::op_Equality(class System.Collections.Generic.Notification`1<!0>, class System.Collections.Generic.Notification`1<!0>)
	IL_000e:  brfalse.s IL_0012

	IL_0010:  ldc.i4.0 
	IL_0011:  ret 
	IL_0012:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::get_Default()
	IL_0017:  ldarg.0 
	IL_0018:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_001d:  ldloc.0 
	IL_001e:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0023:  callvirt instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::Equals(!0, !0)
	IL_0028:  ret 
    } // end of method OnNext::Equals

    // method line 223
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        // Method begins at RVA 0x3240
	// Code size 38 (0x26)
	.maxstack 5
	.locals init (
		object[]	V_0)
	IL_0000:  call class [mscorlib]System.Globalization.CultureInfo class [mscorlib]System.Globalization.CultureInfo::get_CurrentCulture()
	IL_0005:  ldstr "OnNext({0})"
	IL_000a:  ldc.i4.1 
	IL_000b:  newarr [mscorlib]System.Object
	IL_0010:  stloc.0 
	IL_0011:  ldloc.0 
	IL_0012:  ldc.i4.0 
	IL_0013:  ldarg.0 
	IL_0014:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0019:  box !0
	IL_001e:  stelem.ref 
	IL_001f:  ldloc.0 
	IL_0020:  call string string::Format(class [mscorlib]System.IFormatProvider, string, object[])
	IL_0025:  ret 
    } // end of method OnNext::ToString

    // method line 224
    .method public virtual hidebysig 
           instance default void Accept (class [System.Observable]System.IObserver`1<!T> observer)  cil managed 
    {
        // Method begins at RVA 0x3272
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0007:  callvirt instance void class [System.Observable]System.IObserver`1<!T>::OnNext(!0)
	IL_000c:  ret 
    } // end of method OnNext::Accept

    // method line 225
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class System.IObserver`2<!T, !!TResult> observer)  cil managed 
    {
        // Method begins at RVA 0x3280
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0007:  callvirt instance !1 class System.IObserver`2<!T, !!TResult>::OnNext(!0)
	IL_000c:  ret 
    } // end of method OnNext::Accept

    // method line 226
    .method public virtual hidebysig 
           instance default void Accept (class [mscorlib]System.Action`1<!T> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        // Method begins at RVA 0x328e
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0007:  callvirt instance void class [mscorlib]System.Action`1<!T>::Invoke(!0)
	IL_000c:  ret 
    } // end of method OnNext::Accept

    // method line 227
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class [System.Core]System.Func`2<!T, !!TResult> onNext, class [System.Core]System.Func`2<class [mscorlib]System.Exception, !!TResult> onError, class [System.Core]System.Func`1<!!TResult> onCompleted)  cil managed 
    {
        // Method begins at RVA 0x329c
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance !0 class System.Collections.Generic.Notification`1<!T>::get_Value()
	IL_0007:  callvirt instance !1 class [System.Core]System.Func`2<!T, !!TResult>::Invoke(!0)
	IL_000c:  ret 
    } // end of method OnNext::Accept

	.property instance !T Value ()
	{
		.get instance default !0 System.Collections.Generic.Notification`1/OnNext::get_Value () 
	}
	.property instance class [mscorlib]System.Exception Exception ()
	{
		.get instance default class [mscorlib]System.Exception System.Collections.Generic.Notification`1/OnNext::get_Exception () 
	}
	.property instance bool HasValue ()
	{
		.get instance default bool System.Collections.Generic.Notification`1/OnNext::get_HasValue () 
	}
	.property instance valuetype System.Collections.Generic.NotificationKind Kind ()
	{
		.get instance default valuetype System.Collections.Generic.NotificationKind System.Collections.Generic.Notification`1/OnNext::get_Kind () 
	}
  } // end of class OnNext

  .class nested public auto ansi sealed beforefieldinit OnError<T>
  	extends class System.Collections.Generic.Notification`1<!0>
  {
    .custom instance void class [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::'.ctor'(string) =  (
		01 00 14 4F 6E 45 72 72 6F 72 28 7B 45 78 63 65   // ...OnError({Exce
		70 74 69 6F 6E 7D 29 00 00                      ) // ption})..

    .field  private  class [mscorlib]System.Exception exception

    // method line 228
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (class [mscorlib]System.Exception exception)  cil managed 
    {
        // Method begins at RVA 0x32aa
	// Code size 28 (0x1c)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class System.Collections.Generic.Notification`1<!T>::'.ctor'()
	IL_0006:  ldarg.1 
	IL_0007:  brtrue.s IL_0014

	IL_0009:  ldstr "exception"
	IL_000e:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0013:  throw 
	IL_0014:  ldarg.0 
	IL_0015:  ldarg.1 
	IL_0016:  stfld class [mscorlib]System.Exception class System.Collections.Generic.Notification`1/OnError<!0>::exception
	IL_001b:  ret 
    } // end of method OnError::.ctor

    // method line 229
    .method public virtual hidebysig specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x32c7
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Exception class System.Collections.Generic.Notification`1/OnError<!0>::exception
	IL_0006:  call class [mscorlib]System.Exception class System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	IL_000b:  throw 
    } // end of method OnError::get_Value

    // method line 230
    .method public virtual hidebysig specialname 
           instance default class [mscorlib]System.Exception get_Exception ()  cil managed 
    {
        // Method begins at RVA 0x32d4
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Exception class System.Collections.Generic.Notification`1/OnError<!0>::exception
	IL_0006:  ret 
    } // end of method OnError::get_Exception

    // method line 231
    .method public virtual hidebysig specialname 
           instance default bool get_HasValue ()  cil managed 
    {
        // Method begins at RVA 0x32dc
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method OnError::get_HasValue

    // method line 232
    .method public virtual hidebysig specialname 
           instance default valuetype System.Collections.Generic.NotificationKind get_Kind ()  cil managed 
    {
        // Method begins at RVA 0x32df
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.1 
	IL_0001:  ret 
    } // end of method OnError::get_Kind

    // method line 233
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x32e2
	// Code size 12 (0xc)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0006:  callvirt instance int32 object::GetHashCode()
	IL_000b:  ret 
    } // end of method OnError::GetHashCode

    // method line 234
    .method public virtual hidebysig 
           instance default bool Equals (class System.Collections.Generic.Notification`1<!T> other)  cil managed 
    {
        // Method begins at RVA 0x32f0
	// Code size 36 (0x24)
	.maxstack 2
	.locals init (
		class System.Collections.Generic.Notification`1/OnError<!T>	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  isinst class System.Collections.Generic.Notification`1/OnError<!0>
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  ldnull 
	IL_0009:  call bool class System.Collections.Generic.Notification`1<!T>::op_Equality(class System.Collections.Generic.Notification`1<!0>, class System.Collections.Generic.Notification`1<!0>)
	IL_000e:  brfalse.s IL_0012

	IL_0010:  ldc.i4.0 
	IL_0011:  ret 
	IL_0012:  ldarg.0 
	IL_0013:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0018:  ldloc.0 
	IL_0019:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_001e:  call bool object::Equals(object, object)
	IL_0023:  ret 
    } // end of method OnError::Equals

    // method line 235
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        // Method begins at RVA 0x3320
	// Code size 43 (0x2b)
	.maxstack 5
	.locals init (
		object[]	V_0)
	IL_0000:  call class [mscorlib]System.Globalization.CultureInfo class [mscorlib]System.Globalization.CultureInfo::get_CurrentCulture()
	IL_0005:  ldstr "OnError({0})"
	IL_000a:  ldc.i4.1 
	IL_000b:  newarr [mscorlib]System.Object
	IL_0010:  stloc.0 
	IL_0011:  ldloc.0 
	IL_0012:  ldc.i4.0 
	IL_0013:  ldarg.0 
	IL_0014:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0019:  callvirt instance class [mscorlib]System.Type class [mscorlib]System.Exception::GetType()
	IL_001e:  callvirt instance string class [mscorlib]System.Type::get_FullName()
	IL_0023:  stelem.ref 
	IL_0024:  ldloc.0 
	IL_0025:  call string string::Format(class [mscorlib]System.IFormatProvider, string, object[])
	IL_002a:  ret 
    } // end of method OnError::ToString

    // method line 236
    .method public virtual hidebysig 
           instance default void Accept (class [System.Observable]System.IObserver`1<!T> observer)  cil managed 
    {
        // Method begins at RVA 0x3357
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0007:  callvirt instance void class [System.Observable]System.IObserver`1<!T>::OnError(class [mscorlib]System.Exception)
	IL_000c:  ret 
    } // end of method OnError::Accept

    // method line 237
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class System.IObserver`2<!T, !!TResult> observer)  cil managed 
    {
        // Method begins at RVA 0x3365
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0007:  callvirt instance !1 class System.IObserver`2<!T, !!TResult>::OnError(class [mscorlib]System.Exception)
	IL_000c:  ret 
    } // end of method OnError::Accept

    // method line 238
    .method public virtual hidebysig 
           instance default void Accept (class [mscorlib]System.Action`1<!T> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        // Method begins at RVA 0x3373
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.2 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0007:  callvirt instance void class [mscorlib]System.Action`1<class [mscorlib]System.Exception>::Invoke(!0)
	IL_000c:  ret 
    } // end of method OnError::Accept

    // method line 239
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class [System.Core]System.Func`2<!T, !!TResult> onNext, class [System.Core]System.Func`2<class [mscorlib]System.Exception, !!TResult> onError, class [System.Core]System.Func`1<!!TResult> onCompleted)  cil managed 
    {
        // Method begins at RVA 0x3381
	// Code size 13 (0xd)
	.maxstack 8
	IL_0000:  ldarg.2 
	IL_0001:  ldarg.0 
	IL_0002:  callvirt instance class [mscorlib]System.Exception class System.Collections.Generic.Notification`1<!T>::get_Exception()
	IL_0007:  callvirt instance !1 class [System.Core]System.Func`2<class [mscorlib]System.Exception, !!TResult>::Invoke(!0)
	IL_000c:  ret 
    } // end of method OnError::Accept

	.property instance !T Value ()
	{
		.get instance default !0 System.Collections.Generic.Notification`1/OnError::get_Value () 
	}
	.property instance class [mscorlib]System.Exception Exception ()
	{
		.get instance default class [mscorlib]System.Exception System.Collections.Generic.Notification`1/OnError::get_Exception () 
	}
	.property instance bool HasValue ()
	{
		.get instance default bool System.Collections.Generic.Notification`1/OnError::get_HasValue () 
	}
	.property instance valuetype System.Collections.Generic.NotificationKind Kind ()
	{
		.get instance default valuetype System.Collections.Generic.NotificationKind System.Collections.Generic.Notification`1/OnError::get_Kind () 
	}
  } // end of class OnError

  .class nested public auto ansi sealed beforefieldinit OnCompleted<T>
  	extends class System.Collections.Generic.Notification`1<!0>
  {
    .custom instance void class [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::'.ctor'(string) =  (
		01 00 0D 4F 6E 43 6F 6D 70 6C 65 74 65 64 28 29   // ...OnCompleted()
		00 00                                           ) // ..


    // method line 240
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x338f
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void class System.Collections.Generic.Notification`1<!T>::'.ctor'()
	IL_0006:  ret 
    } // end of method OnCompleted::.ctor

    // method line 241
    .method public virtual hidebysig specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x3397
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  ldstr "Operation has been canceled."
	IL_0005:  newobj instance void class [mscorlib]System.InvalidOperationException::'.ctor'(string)
	IL_000a:  throw 
    } // end of method OnCompleted::get_Value

    // method line 242
    .method public virtual hidebysig specialname 
           instance default class [mscorlib]System.Exception get_Exception ()  cil managed 
    {
        // Method begins at RVA 0x33a3
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldnull 
	IL_0001:  ret 
    } // end of method OnCompleted::get_Exception

    // method line 243
    .method public virtual hidebysig specialname 
           instance default bool get_HasValue ()  cil managed 
    {
        // Method begins at RVA 0x33a6
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method OnCompleted::get_HasValue

    // method line 244
    .method public virtual hidebysig specialname 
           instance default valuetype System.Collections.Generic.NotificationKind get_Kind ()  cil managed 
    {
        // Method begins at RVA 0x33a9
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.2 
	IL_0001:  ret 
    } // end of method OnCompleted::get_Kind

    // method line 245
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x33ac
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldc.i4 8510
	IL_0005:  ret 
    } // end of method OnCompleted::GetHashCode

    // method line 246
    .method public virtual hidebysig 
           instance default bool Equals (class System.Collections.Generic.Notification`1<!T> other)  cil managed 
    {
        // Method begins at RVA 0x33b3
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  isinst class System.Collections.Generic.Notification`1/OnCompleted<!0>
	IL_0006:  ldnull 
	IL_0007:  cgt.un 
	IL_0009:  ret 
    } // end of method OnCompleted::Equals

    // method line 247
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        // Method begins at RVA 0x33be
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldstr "OnCompleted()"
	IL_0005:  ret 
    } // end of method OnCompleted::ToString

    // method line 248
    .method public virtual hidebysig 
           instance default void Accept (class [System.Observable]System.IObserver`1<!T> observer)  cil managed 
    {
        // Method begins at RVA 0x33c5
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  callvirt instance void class [System.Observable]System.IObserver`1<!T>::OnCompleted()
	IL_0006:  ret 
    } // end of method OnCompleted::Accept

    // method line 249
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class System.IObserver`2<!T, !!TResult> observer)  cil managed 
    {
        // Method begins at RVA 0x33cd
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  callvirt instance !1 class System.IObserver`2<!T, !!TResult>::OnCompleted()
	IL_0006:  ret 
    } // end of method OnCompleted::Accept

    // method line 250
    .method public virtual hidebysig 
           instance default void Accept (class [mscorlib]System.Action`1<!T> onNext, class [mscorlib]System.Action`1<class [mscorlib]System.Exception> onError, class [System.Core]System.Action onCompleted)  cil managed 
    {
        // Method begins at RVA 0x33d5
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.3 
	IL_0001:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0006:  ret 
    } // end of method OnCompleted::Accept

    // method line 251
    .method public virtual hidebysig 
           instance default !!TResult Accept<TResult> (class [System.Core]System.Func`2<!T, !!TResult> onNext, class [System.Core]System.Func`2<class [mscorlib]System.Exception, !!TResult> onError, class [System.Core]System.Func`1<!!TResult> onCompleted)  cil managed 
    {
        // Method begins at RVA 0x33dd
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.3 
	IL_0001:  callvirt instance !0 class [System.Core]System.Func`1<!!TResult>::Invoke()
	IL_0006:  ret 
    } // end of method OnCompleted::Accept

	.property instance !T Value ()
	{
		.get instance default !0 System.Collections.Generic.Notification`1/OnCompleted::get_Value () 
	}
	.property instance class [mscorlib]System.Exception Exception ()
	{
		.get instance default class [mscorlib]System.Exception System.Collections.Generic.Notification`1/OnCompleted::get_Exception () 
	}
	.property instance bool HasValue ()
	{
		.get instance default bool System.Collections.Generic.Notification`1/OnCompleted::get_HasValue () 
	}
	.property instance valuetype System.Collections.Generic.NotificationKind Kind ()
	{
		.get instance default valuetype System.Collections.Generic.NotificationKind System.Collections.Generic.Notification`1/OnCompleted::get_Kind () 
	}
  } // end of class OnCompleted

  } // end of class System.Collections.Generic.Notification`1
}

.namespace System.Collections.Generic
{
  .class private auto ansi beforefieldinit PriorityQueue`1<(class [mscorlib]System.IComparable`1<!T>) T>
  	extends [mscorlib]System.Object
  {
    .field private static literal  int32 defaultSize = int32(0x00000400)
    .field  private  !0[] items
    .field  private  int32 size

    // method line 252
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x33e5
	// Code size 30 (0x1e)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldc.i4 1024
	IL_000c:  newarr !0
	IL_0011:  stfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0016:  ldarg.0 
	IL_0017:  ldc.i4.0 
	IL_0018:  stfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_001d:  ret 
    } // end of method PriorityQueue`1::.ctor

    // method line 253
    .method private hidebysig 
           instance default bool IsHigherPriority (int32 left, int32 right)  cil managed 
    {
        // Method begins at RVA 0x3404
	// Code size 41 (0x29)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0006:  ldarg.1 
	IL_0007:  readonly. 
	IL_0009:  ldelema !0
	IL_000e:  ldarg.0 
	IL_000f:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0014:  ldarg.2 
	IL_0015:  ldelem !0
	IL_001a:  constrained. !0
	IL_0020:  callvirt instance int32 class [mscorlib]System.IComparable`1<!T>::CompareTo(!0)
	IL_0025:  ldc.i4.0 
	IL_0026:  clt 
	IL_0028:  ret 
    } // end of method PriorityQueue`1::IsHigherPriority

    // method line 254
    .method private hidebysig 
           instance default void Percolate (int32 index)  cil managed 
    {
        // Method begins at RVA 0x3430
	// Code size 97 (0x61)
	.maxstack 4
	.locals init (
		int32	V_0,
		!T	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0007:  bge.s IL_000d

	IL_0009:  ldarg.1 
	IL_000a:  ldc.i4.0 
	IL_000b:  bge.s IL_000e

	IL_000d:  ret 
	IL_000e:  ldarg.1 
	IL_000f:  ldc.i4.1 
	IL_0010:  sub 
	IL_0011:  ldc.i4.2 
	IL_0012:  div 
	IL_0013:  stloc.0 
	IL_0014:  ldloc.0 
	IL_0015:  ldc.i4.0 
	IL_0016:  blt.s IL_001c

	IL_0018:  ldloc.0 
	IL_0019:  ldarg.1 
	IL_001a:  bne.un.s IL_001d

	IL_001c:  ret 
	IL_001d:  ldarg.0 
	IL_001e:  ldarg.1 
	IL_001f:  ldloc.0 
	IL_0020:  call instance bool class System.Collections.Generic.PriorityQueue`1<!T>::IsHigherPriority(int32, int32)
	IL_0025:  brfalse.s IL_0060

	IL_0027:  ldarg.0 
	IL_0028:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_002d:  ldarg.1 
	IL_002e:  ldelem !0
	IL_0033:  stloc.1 
	IL_0034:  ldarg.0 
	IL_0035:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_003a:  ldarg.1 
	IL_003b:  ldarg.0 
	IL_003c:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0041:  ldloc.0 
	IL_0042:  ldelem !0
	IL_0047:  stelem !0
	IL_004c:  ldarg.0 
	IL_004d:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0052:  ldloc.0 
	IL_0053:  ldloc.1 
	IL_0054:  stelem !0
	IL_0059:  ldarg.0 
	IL_005a:  ldloc.0 
	IL_005b:  call instance void class System.Collections.Generic.PriorityQueue`1<!T>::Percolate(int32)
	IL_0060:  ret 
    } // end of method PriorityQueue`1::Percolate

    // method line 255
    .method private hidebysig 
           instance default void Heapify ()  cil managed 
    {
        // Method begins at RVA 0x349d
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldc.i4.0 
	IL_0002:  call instance void class System.Collections.Generic.PriorityQueue`1<!T>::Heapify(int32)
	IL_0007:  ret 
    } // end of method PriorityQueue`1::Heapify

    // method line 256
    .method private hidebysig 
           instance default void Heapify (int32 index)  cil managed 
    {
        // Method begins at RVA 0x34a8
	// Code size 132 (0x84)
	.maxstack 4
	.locals init (
		int32	V_0,
		int32	V_1,
		int32	V_2,
		!T	V_3)
	IL_0000:  ldarg.1 
	IL_0001:  ldarg.0 
	IL_0002:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0007:  bge.s IL_000d

	IL_0009:  ldarg.1 
	IL_000a:  ldc.i4.0 
	IL_000b:  bge.s IL_000e

	IL_000d:  ret 
	IL_000e:  ldc.i4.2 
	IL_000f:  ldarg.1 
	IL_0010:  mul 
	IL_0011:  ldc.i4.1 
	IL_0012:  add 
	IL_0013:  stloc.0 
	IL_0014:  ldc.i4.2 
	IL_0015:  ldarg.1 
	IL_0016:  mul 
	IL_0017:  ldc.i4.2 
	IL_0018:  add 
	IL_0019:  stloc.1 
	IL_001a:  ldarg.1 
	IL_001b:  stloc.2 
	IL_001c:  ldloc.0 
	IL_001d:  ldarg.0 
	IL_001e:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0023:  bge.s IL_0031

	IL_0025:  ldarg.0 
	IL_0026:  ldloc.0 
	IL_0027:  ldloc.2 
	IL_0028:  call instance bool class System.Collections.Generic.PriorityQueue`1<!T>::IsHigherPriority(int32, int32)
	IL_002d:  brfalse.s IL_0031

	IL_002f:  ldloc.0 
	IL_0030:  stloc.2 
	IL_0031:  ldloc.1 
	IL_0032:  ldarg.0 
	IL_0033:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0038:  bge.s IL_0046

	IL_003a:  ldarg.0 
	IL_003b:  ldloc.1 
	IL_003c:  ldloc.2 
	IL_003d:  call instance bool class System.Collections.Generic.PriorityQueue`1<!T>::IsHigherPriority(int32, int32)
	IL_0042:  brfalse.s IL_0046

	IL_0044:  ldloc.1 
	IL_0045:  stloc.2 
	IL_0046:  ldloc.2 
	IL_0047:  ldarg.1 
	IL_0048:  beq.s IL_0083

	IL_004a:  ldarg.0 
	IL_004b:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0050:  ldarg.1 
	IL_0051:  ldelem !0
	IL_0056:  stloc.3 
	IL_0057:  ldarg.0 
	IL_0058:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_005d:  ldarg.1 
	IL_005e:  ldarg.0 
	IL_005f:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0064:  ldloc.2 
	IL_0065:  ldelem !0
	IL_006a:  stelem !0
	IL_006f:  ldarg.0 
	IL_0070:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0075:  ldloc.2 
	IL_0076:  ldloc.3 
	IL_0077:  stelem !0
	IL_007c:  ldarg.0 
	IL_007d:  ldloc.2 
	IL_007e:  call instance void class System.Collections.Generic.PriorityQueue`1<!T>::Heapify(int32)
	IL_0083:  ret 
    } // end of method PriorityQueue`1::Heapify

    // method line 257
    .method public hidebysig specialname 
           instance default int32 get_Count ()  cil managed 
    {
        // Method begins at RVA 0x3538
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0006:  ret 
    } // end of method PriorityQueue`1::get_Count

    // method line 258
    .method public hidebysig 
           instance default !T Peek ()  cil managed 
    {
        // Method begins at RVA 0x3540
	// Code size 32 (0x20)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0006:  brtrue.s IL_0013

	IL_0008:  ldstr "Heap is empty."
	IL_000d:  newobj instance void class [mscorlib]System.InvalidOperationException::'.ctor'(string)
	IL_0012:  throw 
	IL_0013:  ldarg.0 
	IL_0014:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0019:  ldc.i4.0 
	IL_001a:  ldelem !0
	IL_001f:  ret 
    } // end of method PriorityQueue`1::Peek

    // method line 259
    .method public hidebysig 
           instance default !T Dequeue ()  cil managed 
    {
        // Method begins at RVA 0x3564
	// Code size 147 (0x93)
	.maxstack 6
	.locals init (
		!T	V_0,
		!T[]	V_1,
		int32	V_2,
		!T	V_3)
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 class System.Collections.Generic.PriorityQueue`1<!T>::Peek()
	IL_0006:  stloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_000d:  ldc.i4.0 
	IL_000e:  ldarg.0 
	IL_000f:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0014:  ldarg.0 
	IL_0015:  dup 
	IL_0016:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_001b:  ldc.i4.1 
	IL_001c:  sub 
	IL_001d:  dup 
	IL_001e:  stloc.2 
	IL_001f:  stfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0024:  ldloc.2 
	IL_0025:  ldelem !0
	IL_002a:  stelem !0
	IL_002f:  ldarg.0 
	IL_0030:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0035:  ldarg.0 
	IL_0036:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_003b:  ldloca.s 3
	IL_003d:  initobj !0
	IL_0043:  ldloc.3 
	IL_0044:  stelem !0
	IL_0049:  ldarg.0 
	IL_004a:  call instance void class System.Collections.Generic.PriorityQueue`1<!T>::Heapify()
	IL_004f:  ldarg.0 
	IL_0050:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0055:  ldarg.0 
	IL_0056:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_005b:  ldlen 
	IL_005c:  conv.i4 
	IL_005d:  ldc.i4.4 
	IL_005e:  div 
	IL_005f:  bge.s IL_0091

	IL_0061:  ldarg.0 
	IL_0062:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0067:  stloc.1 
	IL_0068:  ldarg.0 
	IL_0069:  ldarg.0 
	IL_006a:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_006f:  ldlen 
	IL_0070:  conv.i4 
	IL_0071:  ldc.i4.2 
	IL_0072:  div 
	IL_0073:  newarr !0
	IL_0078:  stfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_007d:  ldloc.1 
	IL_007e:  ldc.i4.0 
	IL_007f:  ldarg.0 
	IL_0080:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0085:  ldc.i4.0 
	IL_0086:  ldarg.0 
	IL_0087:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_008c:  call void class [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int32, class [mscorlib]System.Array, int32, int32)
	IL_0091:  ldloc.0 
	IL_0092:  ret 
    } // end of method PriorityQueue`1::Dequeue

    // method line 260
    .method public hidebysig 
           instance default void Enqueue (!T item)  cil managed 
    {
        // Method begins at RVA 0x3604
	// Code size 96 (0x60)
	.maxstack 3
	.locals init (
		!T[]	V_0,
		int32	V_1,
		int32	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0006:  ldarg.0 
	IL_0007:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_000c:  ldlen 
	IL_000d:  conv.i4 
	IL_000e:  blt.s IL_0039

	IL_0010:  ldarg.0 
	IL_0011:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0016:  stloc.0 
	IL_0017:  ldarg.0 
	IL_0018:  ldarg.0 
	IL_0019:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_001e:  ldlen 
	IL_001f:  conv.i4 
	IL_0020:  ldc.i4.2 
	IL_0021:  mul 
	IL_0022:  newarr !0
	IL_0027:  stfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_002c:  ldloc.0 
	IL_002d:  ldarg.0 
	IL_002e:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0033:  ldc.i4.0 
	IL_0034:  callvirt instance void class [mscorlib]System.Array::CopyTo(class [mscorlib]System.Array, int32)
	IL_0039:  ldarg.0 
	IL_003a:  dup 
	IL_003b:  ldfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0040:  dup 
	IL_0041:  stloc.2 
	IL_0042:  ldc.i4.1 
	IL_0043:  add 
	IL_0044:  stfld int32 class System.Collections.Generic.PriorityQueue`1<!0>::size
	IL_0049:  ldloc.2 
	IL_004a:  stloc.1 
	IL_004b:  ldarg.0 
	IL_004c:  ldfld !0[] class System.Collections.Generic.PriorityQueue`1<!0>::items
	IL_0051:  ldloc.1 
	IL_0052:  ldarg.1 
	IL_0053:  stelem !0
	IL_0058:  ldarg.0 
	IL_0059:  ldloc.1 
	IL_005a:  call instance void class System.Collections.Generic.PriorityQueue`1<!T>::Percolate(int32)
	IL_005f:  ret 
    } // end of method PriorityQueue`1::Enqueue

	.property instance int32 Count ()
	{
		.get instance default int32 System.Collections.Generic.PriorityQueue`1::get_Count () 
	}
  } // end of class System.Collections.Generic.PriorityQueue`1
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit AsyncLock
  	extends [mscorlib]System.Object
  {
    .field  private  class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> queue
    .field  private  bool isAcquired
    .field  private  bool hasFaulted

    // method line 261
    .method public hidebysig 
           instance default void Wait (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x3670
	// Code size 200 (0xc8)
	.maxstack 2
	.locals init (
		bool	V_0,
		class [System.Core]System.Action	V_1,
		class [mscorlib]System.Exception	V_2,
		class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>	V_3,
		class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>	V_4,
		class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>	V_5)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldc.i4.0 
	IL_000f:  stloc.0 
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	IL_0016:  dup 
	IL_0017:  stloc.3 
	IL_0018:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_001d:  ldarg.0 
	  IL_001e:  ldfld bool System.Concurrency.AsyncLock::hasFaulted
	  IL_0023:  brtrue.s IL_0042

	  IL_0025:  ldarg.0 
	  IL_0026:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	  IL_002b:  ldarg.1 
	  IL_002c:  callvirt instance void class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>::Enqueue(!0)
	  IL_0031:  ldarg.0 
	  IL_0032:  ldfld bool System.Concurrency.AsyncLock::isAcquired
	  IL_0037:  ldc.i4.0 
	  IL_0038:  ceq 
	  IL_003a:  stloc.0 
	  IL_003b:  ldarg.0 
	  IL_003c:  ldc.i4.1 
	  IL_003d:  stfld bool System.Concurrency.AsyncLock::isAcquired
	  IL_0042:  leave.s IL_004b

	} // end .try 0
	finally  { // 0
	  IL_0044:  ldloc.3 
	  IL_0045:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_004a:  endfinally 
	} // end handler 0
	IL_004b:  ldloc.0 
	IL_004c:  brfalse.s IL_00c7

	IL_004e:  ldnull 
	IL_004f:  stloc.1 
	IL_0050:  ldarg.0 
	IL_0051:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	IL_0056:  dup 
	IL_0057:  stloc.s 4
	IL_0059:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 1
	  IL_005e:  ldarg.0 
	  IL_005f:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	  IL_0064:  callvirt instance int32 class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>::get_Count()
	  IL_0069:  ldc.i4.0 
	  IL_006a:  ble.s IL_007a

	  IL_006c:  ldarg.0 
	  IL_006d:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	  IL_0072:  callvirt instance !0 class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>::Dequeue()
	  IL_0077:  stloc.1 
	  IL_0078:  br.s IL_0083

	  IL_007a:  ldarg.0 
	  IL_007b:  ldc.i4.0 
	  IL_007c:  stfld bool System.Concurrency.AsyncLock::isAcquired
	  IL_0081:  leave.s IL_00c7

	  IL_0083:  leave.s IL_008d

	} // end .try 1
	finally  { // 1
	  IL_0085:  ldloc.s 4
	  IL_0087:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_008c:  endfinally 
	} // end handler 1
	.try { // 3
	  IL_008d:  ldloc.1 
	  IL_008e:  callvirt instance void class [System.Core]System.Action::Invoke()
	  IL_0093:  leave.s IL_004e

	} // end .try 3
	catch class [mscorlib]System.Exception { // 3
	  IL_0095:  stloc.2 
	  IL_0096:  ldarg.0 
	  IL_0097:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	  IL_009c:  dup 
	  IL_009d:  stloc.s 5
	  IL_009f:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	  .try { // 2
	    IL_00a4:  ldarg.0 
	    IL_00a5:  ldfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	    IL_00aa:  callvirt instance void class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>::Clear()
	    IL_00af:  ldarg.0 
	    IL_00b0:  ldc.i4.1 
	    IL_00b1:  stfld bool System.Concurrency.AsyncLock::hasFaulted
	    IL_00b6:  leave.s IL_00c0

	  } // end .try 2
	  finally  { // 2
	    IL_00b8:  ldloc.s 5
	    IL_00ba:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	    IL_00bf:  endfinally 
	  } // end handler 2
	  IL_00c0:  ldloc.2 
	  IL_00c1:  call class [mscorlib]System.Exception class System.Diagnostics.ExceptionExtensions::PrepareForRethrow(class [mscorlib]System.Exception)
	  IL_00c6:  throw 
	} // end handler 3
	IL_00c7:  ret 
    } // end of method AsyncLock::Wait

    // method line 262
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3778
	// Code size 18 (0x12)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  newobj instance void class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action>::'.ctor'()
	IL_0006:  stfld class [System]System.Collections.Generic.Queue`1<class [System.Core]System.Action> System.Concurrency.AsyncLock::queue
	IL_000b:  ldarg.0 
	IL_000c:  call instance void object::'.ctor'()
	IL_0011:  ret 
    } // end of method AsyncLock::.ctor

  } // end of class System.Concurrency.AsyncLock
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit ImmediateScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  assembly static initonly  class System.Concurrency.ImmediateScheduler Instance

    // method line 273
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3af5
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method ImmediateScheduler::.ctor

    // method line 274
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x3afd
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method ImmediateScheduler::get_Now

    // method line 275
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x3b04
	// Code size 26 (0x1a)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0014:  call class [mscorlib]System.IDisposable class System.Disposables.Disposable::get_Empty()
	IL_0019:  ret 
    } // end of method ImmediateScheduler::Schedule

    // method line 276
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x3b20
	// Code size 39 (0x27)
	.maxstack 2
	.locals init (
		valuetype [mscorlib]System.TimeSpan	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.2 
	IL_000f:  call valuetype [mscorlib]System.TimeSpan class System.Concurrency.Scheduler::Normalize(valuetype [mscorlib]System.TimeSpan)
	IL_0014:  stloc.0 
	IL_0015:  ldloc.0 
	IL_0016:  call void class [mscorlib]System.Threading.Thread::Sleep(valuetype [mscorlib]System.TimeSpan)
	IL_001b:  ldarg.1 
	IL_001c:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0021:  call class [mscorlib]System.IDisposable class System.Disposables.Disposable::get_Empty()
	IL_0026:  ret 
    } // end of method ImmediateScheduler::Schedule

    // method line 277
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x3b53
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  newobj instance void class System.Concurrency.ImmediateScheduler::'.ctor'()
	IL_0005:  stsfld class System.Concurrency.ImmediateScheduler System.Concurrency.ImmediateScheduler::Instance
	IL_000a:  ret 
    } // end of method ImmediateScheduler::.cctor

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.ImmediateScheduler::get_Now () 
	}
  } // end of class System.Concurrency.ImmediateScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit ThreadPoolScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  assembly static initonly  class System.Concurrency.ThreadPoolScheduler Instance
    .field  assembly static initonly  object gate
    .field  assembly static initonly  class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer,object> timers

    // method line 278
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3b5f
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method ThreadPoolScheduler::.ctor

    // method line 279
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x3b67
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method ThreadPoolScheduler::get_Now

    // method line 280
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x3b90
	// Code size 69 (0x45)
	.maxstack 3
	.locals init (
		class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'	V_0)
	IL_0000:  newobj instance void class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::action
	IL_000d:  ldloc.0 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::action
	IL_0013:  brtrue.s IL_0020

	IL_0015:  ldstr "action"
	IL_001a:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001f:  throw 
	IL_0020:  ldloc.0 
	IL_0021:  newobj instance void class System.Disposables.BooleanDisposable::'.ctor'()
	IL_0026:  stfld class System.Disposables.BooleanDisposable System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::cancelable
	IL_002b:  ldloc.0 
	IL_002c:  ldftn instance void class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::'<Schedule>b__0'(object)
	IL_0032:  newobj instance void class [mscorlib]System.Threading.WaitCallback::'.ctor'(object, native int)
	IL_0037:  ldnull 
	IL_0038:  call bool class [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback, object)
	IL_003d:  pop 
	IL_003e:  ldloc.0 
	IL_003f:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::cancelable
	IL_0044:  ret 
    } // end of method ThreadPoolScheduler::Schedule

    // method line 281
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x3cdc
	// Code size 170 (0xaa)
	.maxstack 6
	.locals init (
		valuetype [mscorlib]System.TimeSpan	V_0,
		class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'	V_1,
		object	V_2)
	IL_0000:  newobj instance void class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::'.ctor'()
	IL_0005:  stloc.1 
	IL_0006:  ldloc.1 
	IL_0007:  ldarg.1 
	IL_0008:  stfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::action
	IL_000d:  ldloc.1 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::action
	IL_0013:  brtrue.s IL_0020

	IL_0015:  ldstr "action"
	IL_001a:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001f:  throw 
	IL_0020:  ldarg.2 
	IL_0021:  call valuetype [mscorlib]System.TimeSpan class System.Concurrency.Scheduler::Normalize(valuetype [mscorlib]System.TimeSpan)
	IL_0026:  stloc.0 
	IL_0027:  ldloc.1 
	IL_0028:  ldc.i4.0 
	IL_0029:  stfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasAdded
	IL_002e:  ldloc.1 
	IL_002f:  ldc.i4.0 
	IL_0030:  stfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasRemoved
	IL_0035:  ldloc.1 
	IL_0036:  ldnull 
	IL_0037:  stfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_003c:  ldloc.1 
	IL_003d:  ldloc.1 
	IL_003e:  ldftn instance void class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::'<Schedule>b__3'(object)
	IL_0044:  newobj instance void class [mscorlib]System.Threading.TimerCallback::'.ctor'(object, native int)
	IL_0049:  ldnull 
	IL_004a:  ldloc.0 
	IL_004b:  ldc.r8 -1.
	IL_0054:  call valuetype [mscorlib]System.TimeSpan valuetype [mscorlib]System.TimeSpan::FromMilliseconds(float64)
	IL_0059:  newobj instance void class [mscorlib]System.Threading.Timer::'.ctor'(class [mscorlib]System.Threading.TimerCallback, object, valuetype [mscorlib]System.TimeSpan, valuetype [mscorlib]System.TimeSpan)
	IL_005e:  stfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_0063:  ldsfld object System.Concurrency.ThreadPoolScheduler::gate
	IL_0068:  dup 
	IL_0069:  stloc.2 
	IL_006a:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_006f:  ldloc.1 
	  IL_0070:  ldfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasRemoved
	  IL_0075:  brtrue.s IL_008f

	  IL_0077:  ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer,object> System.Concurrency.ThreadPoolScheduler::timers
	  IL_007c:  ldloc.1 
	  IL_007d:  ldfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	  IL_0082:  ldnull 
	  IL_0083:  callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer, object>::Add(!0, !1)
	  IL_0088:  ldloc.1 
	  IL_0089:  ldc.i4.1 
	  IL_008a:  stfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasAdded
	  IL_008f:  leave.s IL_0098

	} // end .try 0
	finally  { // 0
	  IL_0091:  ldloc.2 
	  IL_0092:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0097:  endfinally 
	} // end handler 0
	IL_0098:  ldloc.1 
	IL_0099:  ldftn instance void class System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::'<Schedule>b__4'()
	IL_009f:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_00a4:  newobj instance void class System.Disposables.AnonymousDisposable::'.ctor'(class [System.Core]System.Action)
	IL_00a9:  ret 
    } // end of method ThreadPoolScheduler::Schedule

    // method line 282
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x3da4
	// Code size 31 (0x1f)
	.maxstack 8
	IL_0000:  newobj instance void class System.Concurrency.ThreadPoolScheduler::'.ctor'()
	IL_0005:  stsfld class System.Concurrency.ThreadPoolScheduler System.Concurrency.ThreadPoolScheduler::Instance
	IL_000a:  newobj instance void object::'.ctor'()
	IL_000f:  stsfld object System.Concurrency.ThreadPoolScheduler::gate
	IL_0014:  newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer, object>::'.ctor'()
	IL_0019:  stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer,object> System.Concurrency.ThreadPoolScheduler::timers
	IL_001e:  ret 
    } // end of method ThreadPoolScheduler::.cctor

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.ThreadPoolScheduler::get_Now () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass1'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.BooleanDisposable cancelable
    .field  public  class [System.Core]System.Action action

    // method line 361
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3b6e
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass1::.ctor

    // method line 362
    .method public hidebysig 
           instance default void '<Schedule>b__0' (object _)  cil managed 
    {
        // Method begins at RVA 0x3b76
	// Code size 25 (0x19)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Disposables.BooleanDisposable System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::cancelable
	IL_0006:  callvirt instance bool class System.Disposables.BooleanDisposable::get_IsDisposed()
	IL_000b:  brtrue.s IL_0018

	IL_000d:  ldarg.0 
	IL_000e:  ldfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass1'::action
	IL_0013:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0018:  ret 
    } // end of method <>c__DisplayClass1::<Schedule>b__0

  } // end of class <>c__DisplayClass1

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass5'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  bool hasAdded
    .field  public  bool hasRemoved
    .field  public  class [mscorlib]System.Threading.Timer timer
    .field  public  class [System.Core]System.Action action

    // method line 363
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3be1
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass5::.ctor

    // method line 364
    .method public hidebysig 
           instance default void '<Schedule>b__3' (object _)  cil managed 
    {
        // Method begins at RVA 0x3bec
	// Code size 110 (0x6e)
	.maxstack 2
	.locals init (
		class [System.Core]System.Action	V_0,
		class [mscorlib]System.Threading.Timer	V_1,
		object	V_2)
	IL_0000:  ldnull 
	IL_0001:  stloc.0 
	IL_0002:  ldsfld object System.Concurrency.ThreadPoolScheduler::gate
	IL_0007:  dup 
	IL_0008:  stloc.2 
	IL_0009:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_000e:  ldarg.0 
	  IL_000f:  ldfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasAdded
	  IL_0014:  brfalse.s IL_002f

	  IL_0016:  ldarg.0 
	  IL_0017:  ldfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	  IL_001c:  brfalse.s IL_002f

	  IL_001e:  ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer,object> System.Concurrency.ThreadPoolScheduler::timers
	  IL_0023:  ldarg.0 
	  IL_0024:  ldfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	  IL_0029:  callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer, object>::Remove(!0)
	  IL_002e:  pop 
	  IL_002f:  ldarg.0 
	  IL_0030:  ldc.i4.1 
	  IL_0031:  stfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasRemoved
	  IL_0036:  ldarg.0 
	  IL_0037:  ldfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::action
	  IL_003c:  stloc.0 
	  IL_003d:  leave.s IL_0046

	} // end .try 0
	finally  { // 0
	  IL_003f:  ldloc.2 
	  IL_0040:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0045:  endfinally 
	} // end handler 0
	IL_0046:  ldarg.0 
	IL_0047:  ldfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_004c:  stloc.1 
	IL_004d:  ldloc.1 
	IL_004e:  brfalse.s IL_0056

	IL_0050:  ldloc.1 
	IL_0051:  callvirt instance void class [mscorlib]System.Threading.Timer::Dispose()
	IL_0056:  ldarg.0 
	IL_0057:  ldnull 
	IL_0058:  stfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_005d:  ldloc.0 
	IL_005e:  brfalse.s IL_0066

	IL_0060:  ldloc.0 
	IL_0061:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0066:  ldarg.0 
	IL_0067:  ldnull 
	IL_0068:  stfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::action
	IL_006d:  ret 
    } // end of method <>c__DisplayClass5::<Schedule>b__3

    // method line 365
    .method public hidebysig 
           instance default void '<Schedule>b__4' ()  cil managed 
    {
        // Method begins at RVA 0x3c78
	// Code size 71 (0x47)
	.maxstack 2
	.locals init (
		class [mscorlib]System.Threading.Timer	V_0,
		object	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_0006:  stloc.0 
	IL_0007:  ldloc.0 
	IL_0008:  brfalse.s IL_003f

	IL_000a:  ldloc.0 
	IL_000b:  callvirt instance void class [mscorlib]System.Threading.Timer::Dispose()
	IL_0010:  ldsfld object System.Concurrency.ThreadPoolScheduler::gate
	IL_0015:  dup 
	IL_0016:  stloc.1 
	IL_0017:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_001c:  ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer,object> System.Concurrency.ThreadPoolScheduler::timers
	  IL_0021:  ldloc.0 
	  IL_0022:  callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Threading.Timer, object>::Remove(!0)
	  IL_0027:  pop 
	  IL_0028:  ldarg.0 
	  IL_0029:  ldc.i4.1 
	  IL_002a:  stfld bool System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::hasRemoved
	  IL_002f:  ldarg.0 
	  IL_0030:  ldnull 
	  IL_0031:  stfld class [System.Core]System.Action System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::action
	  IL_0036:  leave.s IL_003f

	} // end .try 0
	finally  { // 0
	  IL_0038:  ldloc.1 
	  IL_0039:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_003e:  endfinally 
	} // end handler 0
	IL_003f:  ldarg.0 
	IL_0040:  ldnull 
	IL_0041:  stfld class [mscorlib]System.Threading.Timer System.Concurrency.ThreadPoolScheduler/'<>c__DisplayClass5'::timer
	IL_0046:  ret 
    } // end of method <>c__DisplayClass5::<Schedule>b__4

  } // end of class <>c__DisplayClass5

  } // end of class System.Concurrency.ThreadPoolScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi sealed beforefieldinit CurrentThreadScheduler
  	extends [mscorlib]System.Object
  	implements System.Concurrency.IScheduler  {
    .field  assembly static initonly  class System.Concurrency.CurrentThreadScheduler Instance
    .field  private static  class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> threadLocalQueue
    .custom instance void class [mscorlib]System.ThreadStaticAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 283
    .method private hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3dc4
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method CurrentThreadScheduler::.ctor

    // method line 284
    .method private static hidebysig 
           default class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> GetQueue ()  cil managed 
    {
        // Method begins at RVA 0x3dcc
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.CurrentThreadScheduler::threadLocalQueue
	IL_0005:  ret 
    } // end of method CurrentThreadScheduler::GetQueue

    // method line 285
    .method private static hidebysig 
           default void SetQueue (class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> newQueue)  cil managed 
    {
        // Method begins at RVA 0x3dd3
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  stsfld class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> System.Concurrency.CurrentThreadScheduler::threadLocalQueue
	IL_0006:  ret 
    } // end of method CurrentThreadScheduler::SetQueue

    // method line 286
    .method public final virtual hidebysig newslot specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x3ddb
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0005:  ret 
    } // end of method CurrentThreadScheduler::get_Now

    // method line 287
    .method public hidebysig 
           instance default void EnsureTrampoline (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x3de4
	// Code size 58 (0x3a)
	.maxstack 2
	.locals init (
		class System.Concurrency.CurrentThreadScheduler/Trampoline	V_0)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  call class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> class System.Concurrency.CurrentThreadScheduler::GetQueue()
	IL_0013:  brtrue.s IL_0033

	IL_0015:  newobj instance void class System.Concurrency.CurrentThreadScheduler/Trampoline::'.ctor'()
	IL_001a:  stloc.0 
	.try { // 0
	  IL_001b:  ldarg.1 
	  IL_001c:  callvirt instance void class [System.Core]System.Action::Invoke()
	  IL_0021:  ldloc.0 
	  IL_0022:  callvirt instance void class System.Concurrency.CurrentThreadScheduler/Trampoline::Run()
	  IL_0027:  leave.s IL_0039

	} // end .try 0
	finally  { // 0
	  IL_0029:  ldloc.0 
	  IL_002a:  brfalse.s IL_0032

	  IL_002c:  ldloc.0 
	  IL_002d:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_0032:  endfinally 
	} // end handler 0
	IL_0033:  ldarg.1 
	IL_0034:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0039:  ret 
    } // end of method CurrentThreadScheduler::EnsureTrampoline

    // method line 288
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action)  cil managed 
    {
        // Method begins at RVA 0x3e3c
	// Code size 27 (0x1b)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  ldarg.1 
	IL_0010:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0015:  call instance class [mscorlib]System.IDisposable class System.Concurrency.CurrentThreadScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_001a:  ret 
    } // end of method CurrentThreadScheduler::Schedule

    // method line 289
    .method public final virtual hidebysig newslot 
           instance default class [mscorlib]System.IDisposable Schedule (class [System.Core]System.Action action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        // Method begins at RVA 0x3e58
	// Code size 93 (0x5d)
	.maxstack 3
	.locals init (
		valuetype [mscorlib]System.DateTimeOffset	V_0,
		class System.Concurrency.ScheduledItem	V_1,
		class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>	V_2,
		class System.Concurrency.CurrentThreadScheduler/Trampoline	V_3)
	IL_0000:  ldarg.1 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "action"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.0 
	IL_000f:  call instance valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.CurrentThreadScheduler::get_Now()
	IL_0014:  ldarg.2 
	IL_0015:  call valuetype [mscorlib]System.DateTimeOffset valuetype [mscorlib]System.DateTimeOffset::op_Addition(valuetype [mscorlib]System.DateTimeOffset, valuetype [mscorlib]System.TimeSpan)
	IL_001a:  stloc.0 
	IL_001b:  ldarg.1 
	IL_001c:  ldloc.0 
	IL_001d:  newobj instance void class System.Concurrency.ScheduledItem::'.ctor'(class [System.Core]System.Action, valuetype [mscorlib]System.DateTimeOffset)
	IL_0022:  stloc.1 
	IL_0023:  call class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> class System.Concurrency.CurrentThreadScheduler::GetQueue()
	IL_0028:  stloc.2 
	IL_0029:  ldloc.2 
	IL_002a:  brtrue.s IL_004f

	IL_002c:  newobj instance void class System.Concurrency.CurrentThreadScheduler/Trampoline::'.ctor'()
	IL_0031:  stloc.3 
	.try { // 0
	  IL_0032:  call class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> class System.Concurrency.CurrentThreadScheduler::GetQueue()
	  IL_0037:  ldloc.1 
	  IL_0038:  callvirt instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Enqueue(!0)
	  IL_003d:  ldloc.3 
	  IL_003e:  callvirt instance void class System.Concurrency.CurrentThreadScheduler/Trampoline::Run()
	  IL_0043:  leave.s IL_0056

	} // end .try 0
	finally  { // 0
	  IL_0045:  ldloc.3 
	  IL_0046:  brfalse.s IL_004e

	  IL_0048:  ldloc.3 
	  IL_0049:  callvirt instance void class [mscorlib]System.IDisposable::Dispose()
	  IL_004e:  endfinally 
	} // end handler 0
	IL_004f:  ldloc.2 
	IL_0050:  ldloc.1 
	IL_0051:  callvirt instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Enqueue(!0)
	IL_0056:  ldloc.1 
	IL_0057:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::GetDisposable()
	IL_005c:  ret 
    } // end of method CurrentThreadScheduler::Schedule

    // method line 290
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x3ed4
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  newobj instance void class System.Concurrency.CurrentThreadScheduler::'.ctor'()
	IL_0005:  stsfld class System.Concurrency.CurrentThreadScheduler System.Concurrency.CurrentThreadScheduler::Instance
	IL_000a:  ret 
    } // end of method CurrentThreadScheduler::.cctor

	.property instance valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.CurrentThreadScheduler::get_Now () 
	}
  .class nested private auto ansi beforefieldinit Trampoline
  	extends [mscorlib]System.Object
  	implements [mscorlib]System.IDisposable  {

    // method line 291
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3ee0
	// Code size 17 (0x11)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  newobj instance void class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::'.ctor'()
	IL_000b:  call void class System.Concurrency.CurrentThreadScheduler::SetQueue(class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>)
	IL_0010:  ret 
    } // end of method Trampoline::.ctor

    // method line 292
    .method public final virtual hidebysig newslot 
           instance default void Dispose ()  cil managed 
    {
        // Method begins at RVA 0x3ef2
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldnull 
	IL_0001:  call void class System.Concurrency.CurrentThreadScheduler::SetQueue(class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>)
	IL_0006:  ret 
    } // end of method Trampoline::Dispose

    // method line 293
    .method public hidebysig 
           instance default void Run ()  cil managed 
    {
        // Method begins at RVA 0x3efc
	// Code size 90 (0x5a)
	.maxstack 2
	.locals init (
		class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>	V_0,
		class System.Concurrency.ScheduledItem	V_1,
		valuetype [mscorlib]System.TimeSpan	V_2)
	IL_0000:  call class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem> class System.Concurrency.CurrentThreadScheduler::GetQueue()
	IL_0005:  stloc.0 
	IL_0006:  br.s IL_0050

	IL_0008:  ldloc.0 
	IL_0009:  callvirt instance !0 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::Dequeue()
	IL_000e:  stloc.1 
	IL_000f:  ldloc.1 
	IL_0010:  callvirt instance bool class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_IsCanceled()
	IL_0015:  brtrue.s IL_0050

	IL_0017:  ldloc.1 
	IL_0018:  callvirt instance !0 class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_DueTime()
	IL_001d:  call valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.Scheduler::get_Now()
	IL_0022:  call valuetype [mscorlib]System.TimeSpan valuetype [mscorlib]System.DateTimeOffset::op_Subtraction(valuetype [mscorlib]System.DateTimeOffset, valuetype [mscorlib]System.DateTimeOffset)
	IL_0027:  stloc.2 
	IL_0028:  ldloca.s 2
	IL_002a:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_002f:  call instance int32 valuetype [mscorlib]System.TimeSpan::CompareTo(valuetype [mscorlib]System.TimeSpan)
	IL_0034:  ldc.i4.0 
	IL_0035:  ble.s IL_003d

	IL_0037:  ldloc.2 
	IL_0038:  call void class [mscorlib]System.Threading.Thread::Sleep(valuetype [mscorlib]System.TimeSpan)
	IL_003d:  ldloc.1 
	IL_003e:  callvirt instance bool class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_IsCanceled()
	IL_0043:  brtrue.s IL_0050

	IL_0045:  ldloc.1 
	IL_0046:  callvirt instance class [System.Core]System.Action class System.Concurrency.ScheduledItem`1<valuetype [mscorlib]System.DateTimeOffset>::get_Action()
	IL_004b:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0050:  ldloc.0 
	IL_0051:  callvirt instance int32 class System.Collections.Generic.PriorityQueue`1<class System.Concurrency.ScheduledItem>::get_Count()
	IL_0056:  ldc.i4.0 
	IL_0057:  bgt.s IL_0008

	IL_0059:  ret 
    } // end of method Trampoline::Run

  } // end of class Trampoline

  } // end of class System.Concurrency.CurrentThreadScheduler
}

.namespace System.Concurrency
{
  .class public auto ansi abstract sealed beforefieldinit Scheduler
  	extends [mscorlib]System.Object
  {
    .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    // method line 294
    .method assembly static hidebysig specialname 
           default valuetype [mscorlib]System.DateTimeOffset get_Now ()  cil managed 
    {
        // Method begins at RVA 0x3f62
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  call valuetype [mscorlib]System.DateTimeOffset valuetype [mscorlib]System.DateTimeOffset::get_Now()
	IL_0005:  ret 
    } // end of method Scheduler::get_Now

    // method line 295
    .method assembly static hidebysig 
           default valuetype [mscorlib]System.TimeSpan Normalize (valuetype [mscorlib]System.TimeSpan timeSpan)  cil managed 
    {
        // Method begins at RVA 0x3f69
	// Code size 19 (0x13)
	.maxstack 8
	IL_0000:  ldarga.s 0
	IL_0002:  call instance int64 valuetype [mscorlib]System.TimeSpan::get_Ticks()
	IL_0007:  ldc.i4.0 
	IL_0008:  conv.i8 
	IL_0009:  bge.s IL_0011

	IL_000b:  ldsfld valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::Zero
	IL_0010:  ret 
	IL_0011:  ldarg.0 
	IL_0012:  ret 
    } // end of method Scheduler::Normalize

    // method line 296
    .method public static hidebysig specialname 
           default class System.Concurrency.ImmediateScheduler get_Immediate ()  cil managed 
    {
        // Method begins at RVA 0x3f7d
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Concurrency.ImmediateScheduler System.Concurrency.ImmediateScheduler::Instance
	IL_0005:  ret 
    } // end of method Scheduler::get_Immediate

    // method line 297
    .method public static hidebysig specialname 
           default class System.Concurrency.CurrentThreadScheduler get_CurrentThread ()  cil managed 
    {
        // Method begins at RVA 0x3f84
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Concurrency.CurrentThreadScheduler System.Concurrency.CurrentThreadScheduler::Instance
	IL_0005:  ret 
    } // end of method Scheduler::get_CurrentThread

    // method line 298
    .method public static hidebysig specialname 
           default class System.Concurrency.ThreadPoolScheduler get_ThreadPool ()  cil managed 
    {
        // Method begins at RVA 0x3f8b
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Concurrency.ThreadPoolScheduler System.Concurrency.ThreadPoolScheduler::Instance
	IL_0005:  ret 
    } // end of method Scheduler::get_ThreadPool

    // method line 299
    .method public static hidebysig specialname 
           default class System.Concurrency.NewThreadScheduler get_NewThread ()  cil managed 
    {
        // Method begins at RVA 0x3f92
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Concurrency.NewThreadScheduler System.Concurrency.NewThreadScheduler::Instance
	IL_0005:  ret 
    } // end of method Scheduler::get_NewThread

    // method line 300
    .method public static hidebysig specialname 
           default class System.Concurrency.TaskPoolScheduler get_TaskPool ()  cil managed 
    {
        // Method begins at RVA 0x3f99
	// Code size 6 (0x6)
	.maxstack 8
	IL_0000:  ldsfld class System.Concurrency.TaskPoolScheduler System.Concurrency.TaskPoolScheduler::Instance
	IL_0005:  ret 
    } // end of method Scheduler::get_TaskPool

    // method line 302
    .method public static hidebysig 
           default class [mscorlib]System.IDisposable Schedule (class System.Concurrency.IScheduler scheduler, class [System.Core]System.Action action, valuetype [mscorlib]System.DateTimeOffset dueTime)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x3fa7
	// Code size 48 (0x30)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  brtrue.s IL_000e

	IL_0003:  ldstr "scheduler"
	IL_0008:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_000d:  throw 
	IL_000e:  ldarg.1 
	IL_000f:  brtrue.s IL_001c

	IL_0011:  ldstr "action"
	IL_0016:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_001b:  throw 
	IL_001c:  ldarg.0 
	IL_001d:  ldarg.1 
	IL_001e:  ldarg.2 
	IL_001f:  ldarg.0 
	IL_0020:  callvirt instance valuetype [mscorlib]System.DateTimeOffset class System.Concurrency.IScheduler::get_Now()
	IL_0025:  call valuetype [mscorlib]System.TimeSpan valuetype [mscorlib]System.DateTimeOffset::op_Subtraction(valuetype [mscorlib]System.DateTimeOffset, valuetype [mscorlib]System.DateTimeOffset)
	IL_002a:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_002f:  ret 
    } // end of method Scheduler::Schedule

    // method line 303
    .method public static hidebysig 
           default class [mscorlib]System.IDisposable Schedule (class System.Concurrency.IScheduler scheduler, class [mscorlib]System.Action`1<class [System.Core]System.Action> action)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4120
	// Code size 157 (0x9d)
	.maxstack 4
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClass4'	V_0)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass4'::scheduler
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Action`1<class [System.Core]System.Action> System.Concurrency.Scheduler/'<>c__DisplayClass4'::action
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass4'::scheduler
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "scheduler"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldfld class [mscorlib]System.Action`1<class [System.Core]System.Action> System.Concurrency.Scheduler/'<>c__DisplayClass4'::action
	IL_002d:  brtrue.s IL_003a

	IL_002f:  ldstr "action"
	IL_0034:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0039:  throw 
	IL_003a:  ldloc.0 
	IL_003b:  ldc.i4.0 
	IL_003c:  newarr [mscorlib]System.IDisposable
	IL_0041:  newobj instance void class System.Disposables.CompositeDisposable::'.ctor'(class [mscorlib]System.IDisposable[])
	IL_0046:  stfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'::group
	IL_004b:  ldloc.0 
	IL_004c:  ldnull 
	IL_004d:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass4'::recursiveAction
	IL_0052:  ldloc.0 
	IL_0053:  newobj instance void object::'.ctor'()
	IL_0058:  stfld object System.Concurrency.Scheduler/'<>c__DisplayClass4'::gate
	IL_005d:  ldloc.0 
	IL_005e:  newobj instance void class System.Concurrency.AsyncLock::'.ctor'()
	IL_0063:  stfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClass4'::asyncLock
	IL_0068:  ldloc.0 
	IL_0069:  ldloc.0 
	IL_006a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'::'<Schedule>b__0'()
	IL_0070:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0075:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass4'::recursiveAction
	IL_007a:  ldloc.0 
	IL_007b:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'::group
	IL_0080:  ldloc.0 
	IL_0081:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass4'::scheduler
	IL_0086:  ldloc.0 
	IL_0087:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass4'::recursiveAction
	IL_008c:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_0091:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_0096:  ldloc.0 
	IL_0097:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'::group
	IL_009c:  ret 
    } // end of method Scheduler::Schedule

    // method line 304
    .method public static hidebysig 
           default class [mscorlib]System.IDisposable Schedule (class System.Concurrency.IScheduler scheduler, class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>> action, valuetype [mscorlib]System.TimeSpan dueTime)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4314
	// Code size 158 (0x9e)
	.maxstack 4
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClassc'	V_0)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClassc'::scheduler
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>> System.Concurrency.Scheduler/'<>c__DisplayClassc'::action
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClassc'::scheduler
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "scheduler"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>> System.Concurrency.Scheduler/'<>c__DisplayClassc'::action
	IL_002d:  brtrue.s IL_003a

	IL_002f:  ldstr "action"
	IL_0034:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0039:  throw 
	IL_003a:  ldloc.0 
	IL_003b:  ldc.i4.0 
	IL_003c:  newarr [mscorlib]System.IDisposable
	IL_0041:  newobj instance void class System.Disposables.CompositeDisposable::'.ctor'(class [mscorlib]System.IDisposable[])
	IL_0046:  stfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'::group
	IL_004b:  ldloc.0 
	IL_004c:  ldnull 
	IL_004d:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClassc'::recursiveAction
	IL_0052:  ldloc.0 
	IL_0053:  newobj instance void object::'.ctor'()
	IL_0058:  stfld object System.Concurrency.Scheduler/'<>c__DisplayClassc'::gate
	IL_005d:  ldloc.0 
	IL_005e:  newobj instance void class System.Concurrency.AsyncLock::'.ctor'()
	IL_0063:  stfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClassc'::asyncLock
	IL_0068:  ldloc.0 
	IL_0069:  ldloc.0 
	IL_006a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'::'<Schedule>b__8'()
	IL_0070:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0075:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClassc'::recursiveAction
	IL_007a:  ldloc.0 
	IL_007b:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'::group
	IL_0080:  ldloc.0 
	IL_0081:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClassc'::scheduler
	IL_0086:  ldloc.0 
	IL_0087:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClassc'::recursiveAction
	IL_008c:  ldarg.2 
	IL_008d:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_0092:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_0097:  ldloc.0 
	IL_0098:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'::group
	IL_009d:  ret 
    } // end of method Scheduler::Schedule

    // method line 305
    .method public static hidebysig 
           default class [mscorlib]System.IDisposable Schedule (class System.Concurrency.IScheduler scheduler, class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>> action, valuetype [mscorlib]System.DateTimeOffset dueTime)  cil managed 
    {
        .custom instance void class [System.Core]System.Runtime.CompilerServices.ExtensionAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4508
	// Code size 158 (0x9e)
	.maxstack 4
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClass14'	V_0)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass14'::scheduler
	IL_000d:  ldloc.0 
	IL_000e:  ldarg.1 
	IL_000f:  stfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>> System.Concurrency.Scheduler/'<>c__DisplayClass14'::action
	IL_0014:  ldloc.0 
	IL_0015:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass14'::scheduler
	IL_001a:  brtrue.s IL_0027

	IL_001c:  ldstr "scheduler"
	IL_0021:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0026:  throw 
	IL_0027:  ldloc.0 
	IL_0028:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>> System.Concurrency.Scheduler/'<>c__DisplayClass14'::action
	IL_002d:  brtrue.s IL_003a

	IL_002f:  ldstr "action"
	IL_0034:  newobj instance void class [mscorlib]System.ArgumentNullException::'.ctor'(string)
	IL_0039:  throw 
	IL_003a:  ldloc.0 
	IL_003b:  ldc.i4.0 
	IL_003c:  newarr [mscorlib]System.IDisposable
	IL_0041:  newobj instance void class System.Disposables.CompositeDisposable::'.ctor'(class [mscorlib]System.IDisposable[])
	IL_0046:  stfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'::group
	IL_004b:  ldloc.0 
	IL_004c:  ldnull 
	IL_004d:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass14'::recursiveAction
	IL_0052:  ldloc.0 
	IL_0053:  newobj instance void object::'.ctor'()
	IL_0058:  stfld object System.Concurrency.Scheduler/'<>c__DisplayClass14'::gate
	IL_005d:  ldloc.0 
	IL_005e:  newobj instance void class System.Concurrency.AsyncLock::'.ctor'()
	IL_0063:  stfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClass14'::asyncLock
	IL_0068:  ldloc.0 
	IL_0069:  ldloc.0 
	IL_006a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'::'<Schedule>b__10'()
	IL_0070:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0075:  stfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass14'::recursiveAction
	IL_007a:  ldloc.0 
	IL_007b:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'::group
	IL_0080:  ldloc.0 
	IL_0081:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass14'::scheduler
	IL_0086:  ldloc.0 
	IL_0087:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass14'::recursiveAction
	IL_008c:  ldarg.2 
	IL_008d:  call class [mscorlib]System.IDisposable class System.Concurrency.Scheduler::Schedule(class System.Concurrency.IScheduler, class [System.Core]System.Action, valuetype [mscorlib]System.DateTimeOffset)
	IL_0092:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	IL_0097:  ldloc.0 
	IL_0098:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'::group
	IL_009d:  ret 
    } // end of method Scheduler::Schedule

    // method line 306
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x45b2
	// Code size 16 (0x10)
	.maxstack 8
	IL_0000:  ret 
    } // end of method Scheduler::.cctor

	.property valuetype [mscorlib]System.DateTimeOffset Now ()
	{
		.get default valuetype [mscorlib]System.DateTimeOffset System.Concurrency.Scheduler::get_Now () 
	}
	.property class System.Concurrency.ImmediateScheduler Immediate ()
	{
		.get default class System.Concurrency.ImmediateScheduler System.Concurrency.Scheduler::get_Immediate () 
	}
	.property class System.Concurrency.CurrentThreadScheduler CurrentThread ()
	{
		.get default class System.Concurrency.CurrentThreadScheduler System.Concurrency.Scheduler::get_CurrentThread () 
	}
	.property class System.Concurrency.ThreadPoolScheduler ThreadPool ()
	{
		.get default class System.Concurrency.ThreadPoolScheduler System.Concurrency.Scheduler::get_ThreadPool () 
	}
	.property class System.Concurrency.NewThreadScheduler NewThread ()
	{
		.get default class System.Concurrency.NewThreadScheduler System.Concurrency.Scheduler::get_NewThread () 
	}
	.property class System.Concurrency.TaskPoolScheduler TaskPool ()
	{
		.get default class System.Concurrency.TaskPoolScheduler System.Concurrency.Scheduler::get_TaskPool () 
	}
  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass4'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.CompositeDisposable group
    .field  public  class [System.Core]System.Action recursiveAction
    .field  public  object gate
    .field  public  class System.Concurrency.AsyncLock asyncLock
    .field  public  class System.Concurrency.IScheduler scheduler
    .field  public  class [mscorlib]System.Action`1<class [System.Core]System.Action> action

    // method line 366
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3fd8
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass4::.ctor

    // method line 367
    .method public hidebysig 
           instance default void '<Schedule>b__0' ()  cil managed 
    {
        // Method begins at RVA 0x4105
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClass4'::asyncLock
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'::'<Schedule>b__1'()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class System.Concurrency.AsyncLock::Wait(class [System.Core]System.Action)
	IL_0017:  ret 
    } // end of method <>c__DisplayClass4::<Schedule>b__0

    // method line 368
    .method public hidebysig 
           instance default void '<Schedule>b__1' ()  cil managed 
    {
        // Method begins at RVA 0x40ec
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Action`1<class [System.Core]System.Action> System.Concurrency.Scheduler/'<>c__DisplayClass4'::action
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'::'<Schedule>b__2'()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class [mscorlib]System.Action`1<class [System.Core]System.Action>::Invoke(!0)
	IL_0017:  ret 
    } // end of method <>c__DisplayClass4::<Schedule>b__1

    // method line 369
    .method public hidebysig 
           instance default void '<Schedule>b__2' ()  cil managed 
    {
        // Method begins at RVA 0x4058
	// Code size 118 (0x76)
	.maxstack 5
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'	V_0,
		object	V_1)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.Scheduler/'<>c__DisplayClass4' System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'CS$<>8__locals5'
	IL_000d:  ldloc.0 
	IL_000e:  ldc.i4.0 
	IL_000f:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isAdded
	IL_0014:  ldloc.0 
	IL_0015:  ldc.i4.0 
	IL_0016:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isDone
	IL_001b:  ldloc.0 
	IL_001c:  ldnull 
	IL_001d:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::d
	IL_0022:  ldloc.0 
	IL_0023:  ldarg.0 
	IL_0024:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass4'::scheduler
	IL_0029:  ldloc.0 
	IL_002a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'<Schedule>b__3'()
	IL_0030:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0035:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action)
	IL_003a:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::d
	IL_003f:  ldarg.0 
	IL_0040:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClass4'::gate
	IL_0045:  dup 
	IL_0046:  stloc.1 
	IL_0047:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_004c:  ldloc.0 
	  IL_004d:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isDone
	  IL_0052:  brtrue.s IL_006c

	  IL_0054:  ldarg.0 
	  IL_0055:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'::group
	  IL_005a:  ldloc.0 
	  IL_005b:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::d
	  IL_0060:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	  IL_0065:  ldloc.0 
	  IL_0066:  ldc.i4.1 
	  IL_0067:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isAdded
	  IL_006c:  leave.s IL_0075

	} // end .try 0
	finally  { // 0
	  IL_006e:  ldloc.1 
	  IL_006f:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0074:  endfinally 
	} // end handler 0
	IL_0075:  ret 
    } // end of method <>c__DisplayClass4::<Schedule>b__2

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass6'
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Concurrency.Scheduler/'<>c__DisplayClass4' 'CS$<>8__locals5'
    .field  public  bool isAdded
    .field  public  bool isDone
    .field  public  class [mscorlib]System.IDisposable d

    // method line 370
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x3fe0
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass6::.ctor

    // method line 371
    .method public hidebysig 
           instance default void '<Schedule>b__3' ()  cil managed 
    {
        // Method begins at RVA 0x3fe8
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass4' System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'CS$<>8__locals5'
	IL_0006:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass4'::recursiveAction
	IL_000b:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass4' System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'CS$<>8__locals5'
	IL_0016:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClass4'::gate
	IL_001b:  dup 
	IL_001c:  stloc.0 
	IL_001d:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0022:  ldarg.0 
	  IL_0023:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isAdded
	  IL_0028:  brfalse.s IL_0043

	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass4' System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::'CS$<>8__locals5'
	  IL_0030:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'::group
	  IL_0035:  ldarg.0 
	  IL_0036:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::d
	  IL_003b:  callvirt instance bool class System.Disposables.CompositeDisposable::Remove(class [mscorlib]System.IDisposable)
	  IL_0040:  pop 
	  IL_0041:  br.s IL_004a

	  IL_0043:  ldarg.0 
	  IL_0044:  ldc.i4.1 
	  IL_0045:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass4'/'<>c__DisplayClass6'::isDone
	  IL_004a:  leave.s IL_0053

	} // end .try 0
	finally  { // 0
	  IL_004c:  ldloc.0 
	  IL_004d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0052:  endfinally 
	} // end handler 0
	IL_0053:  ret 
    } // end of method <>c__DisplayClass6::<Schedule>b__3

  } // end of class <>c__DisplayClass6

  } // end of class <>c__DisplayClass4

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClassc'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.CompositeDisposable group
    .field  public  class [System.Core]System.Action recursiveAction
    .field  public  object gate
    .field  public  class System.Concurrency.AsyncLock asyncLock
    .field  public  class System.Concurrency.IScheduler scheduler
    .field  public  class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>> action

    // method line 372
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x41c9
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClassc::.ctor

    // method line 373
    .method public hidebysig 
           instance default void '<Schedule>b__8' ()  cil managed 
    {
        // Method begins at RVA 0x42f9
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClassc'::asyncLock
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'::'<Schedule>b__9'()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class System.Concurrency.AsyncLock::Wait(class [System.Core]System.Action)
	IL_0017:  ret 
    } // end of method <>c__DisplayClassc::<Schedule>b__8

    // method line 374
    .method public hidebysig 
           instance default void '<Schedule>b__9' ()  cil managed 
    {
        // Method begins at RVA 0x42e0
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>> System.Concurrency.Scheduler/'<>c__DisplayClassc'::action
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'::'<Schedule>b__a'(valuetype [mscorlib]System.TimeSpan)
	IL_000d:  newobj instance void class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.TimeSpan>>::Invoke(!0)
	IL_0017:  ret 
    } // end of method <>c__DisplayClassc::<Schedule>b__9

    // method line 375
    .method public hidebysig 
           instance default void '<Schedule>b__a' (valuetype [mscorlib]System.TimeSpan dt)  cil managed 
    {
        // Method begins at RVA 0x424c
	// Code size 119 (0x77)
	.maxstack 5
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'	V_0,
		object	V_1)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.Scheduler/'<>c__DisplayClassc' System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'CS$<>8__localsd'
	IL_000d:  ldloc.0 
	IL_000e:  ldc.i4.0 
	IL_000f:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isAdded
	IL_0014:  ldloc.0 
	IL_0015:  ldc.i4.0 
	IL_0016:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isDone
	IL_001b:  ldloc.0 
	IL_001c:  ldnull 
	IL_001d:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::d
	IL_0022:  ldloc.0 
	IL_0023:  ldarg.0 
	IL_0024:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClassc'::scheduler
	IL_0029:  ldloc.0 
	IL_002a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'<Schedule>b__b'()
	IL_0030:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0035:  ldarg.1 
	IL_0036:  callvirt instance class [mscorlib]System.IDisposable class System.Concurrency.IScheduler::Schedule(class [System.Core]System.Action, valuetype [mscorlib]System.TimeSpan)
	IL_003b:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::d
	IL_0040:  ldarg.0 
	IL_0041:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClassc'::gate
	IL_0046:  dup 
	IL_0047:  stloc.1 
	IL_0048:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_004d:  ldloc.0 
	  IL_004e:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isDone
	  IL_0053:  brtrue.s IL_006d

	  IL_0055:  ldarg.0 
	  IL_0056:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'::group
	  IL_005b:  ldloc.0 
	  IL_005c:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::d
	  IL_0061:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	  IL_0066:  ldloc.0 
	  IL_0067:  ldc.i4.1 
	  IL_0068:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isAdded
	  IL_006d:  leave.s IL_0076

	} // end .try 0
	finally  { // 0
	  IL_006f:  ldloc.1 
	  IL_0070:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0075:  endfinally 
	} // end handler 0
	IL_0076:  ret 
    } // end of method <>c__DisplayClassc::<Schedule>b__a

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClasse'
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Concurrency.Scheduler/'<>c__DisplayClassc' 'CS$<>8__localsd'
    .field  public  bool isAdded
    .field  public  bool isDone
    .field  public  class [mscorlib]System.IDisposable d

    // method line 376
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x41d1
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClasse::.ctor

    // method line 377
    .method public hidebysig 
           instance default void '<Schedule>b__b' ()  cil managed 
    {
        // Method begins at RVA 0x41dc
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClassc' System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'CS$<>8__localsd'
	IL_0006:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClassc'::recursiveAction
	IL_000b:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClassc' System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'CS$<>8__localsd'
	IL_0016:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClassc'::gate
	IL_001b:  dup 
	IL_001c:  stloc.0 
	IL_001d:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0022:  ldarg.0 
	  IL_0023:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isAdded
	  IL_0028:  brfalse.s IL_0043

	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClassc' System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::'CS$<>8__localsd'
	  IL_0030:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'::group
	  IL_0035:  ldarg.0 
	  IL_0036:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::d
	  IL_003b:  callvirt instance bool class System.Disposables.CompositeDisposable::Remove(class [mscorlib]System.IDisposable)
	  IL_0040:  pop 
	  IL_0041:  br.s IL_004a

	  IL_0043:  ldarg.0 
	  IL_0044:  ldc.i4.1 
	  IL_0045:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClassc'/'<>c__DisplayClasse'::isDone
	  IL_004a:  leave.s IL_0053

	} // end .try 0
	finally  { // 0
	  IL_004c:  ldloc.0 
	  IL_004d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0052:  endfinally 
	} // end handler 0
	IL_0053:  ret 
    } // end of method <>c__DisplayClasse::<Schedule>b__b

  } // end of class <>c__DisplayClasse

  } // end of class <>c__DisplayClassc

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass14'
  	extends [mscorlib]System.Object
  {
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  public  class System.Disposables.CompositeDisposable group
    .field  public  class [System.Core]System.Action recursiveAction
    .field  public  object gate
    .field  public  class System.Concurrency.AsyncLock asyncLock
    .field  public  class System.Concurrency.IScheduler scheduler
    .field  public  class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>> action

    // method line 378
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x43be
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass14::.ctor

    // method line 379
    .method public hidebysig 
           instance default void '<Schedule>b__10' ()  cil managed 
    {
        // Method begins at RVA 0x44ed
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.AsyncLock System.Concurrency.Scheduler/'<>c__DisplayClass14'::asyncLock
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'::'<Schedule>b__11'()
	IL_000d:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class System.Concurrency.AsyncLock::Wait(class [System.Core]System.Action)
	IL_0017:  ret 
    } // end of method <>c__DisplayClass14::<Schedule>b__10

    // method line 380
    .method public hidebysig 
           instance default void '<Schedule>b__11' ()  cil managed 
    {
        // Method begins at RVA 0x44d4
	// Code size 24 (0x18)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>> System.Concurrency.Scheduler/'<>c__DisplayClass14'::action
	IL_0006:  ldarg.0 
	IL_0007:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'::'<Schedule>b__12'(valuetype [mscorlib]System.DateTimeOffset)
	IL_000d:  newobj instance void class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>::'.ctor'(object, native int)
	IL_0012:  callvirt instance void class [mscorlib]System.Action`1<class [mscorlib]System.Action`1<valuetype [mscorlib]System.DateTimeOffset>>::Invoke(!0)
	IL_0017:  ret 
    } // end of method <>c__DisplayClass14::<Schedule>b__11

    // method line 381
    .method public hidebysig 
           instance default void '<Schedule>b__12' (valuetype [mscorlib]System.DateTimeOffset dt)  cil managed 
    {
        // Method begins at RVA 0x4440
	// Code size 119 (0x77)
	.maxstack 5
	.locals init (
		class System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'	V_0,
		object	V_1)
	IL_0000:  newobj instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'.ctor'()
	IL_0005:  stloc.0 
	IL_0006:  ldloc.0 
	IL_0007:  ldarg.0 
	IL_0008:  stfld class System.Concurrency.Scheduler/'<>c__DisplayClass14' System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'CS$<>8__locals15'
	IL_000d:  ldloc.0 
	IL_000e:  ldc.i4.0 
	IL_000f:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isAdded
	IL_0014:  ldloc.0 
	IL_0015:  ldc.i4.0 
	IL_0016:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isDone
	IL_001b:  ldloc.0 
	IL_001c:  ldnull 
	IL_001d:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::d
	IL_0022:  ldloc.0 
	IL_0023:  ldarg.0 
	IL_0024:  ldfld class System.Concurrency.IScheduler System.Concurrency.Scheduler/'<>c__DisplayClass14'::scheduler
	IL_0029:  ldloc.0 
	IL_002a:  ldftn instance void class System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'<Schedule>b__13'()
	IL_0030:  newobj instance void class [System.Core]System.Action::'.ctor'(object, native int)
	IL_0035:  ldarg.1 
	IL_0036:  call class [mscorlib]System.IDisposable class System.Concurrency.Scheduler::Schedule(class System.Concurrency.IScheduler, class [System.Core]System.Action, valuetype [mscorlib]System.DateTimeOffset)
	IL_003b:  stfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::d
	IL_0040:  ldarg.0 
	IL_0041:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClass14'::gate
	IL_0046:  dup 
	IL_0047:  stloc.1 
	IL_0048:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_004d:  ldloc.0 
	  IL_004e:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isDone
	  IL_0053:  brtrue.s IL_006d

	  IL_0055:  ldarg.0 
	  IL_0056:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'::group
	  IL_005b:  ldloc.0 
	  IL_005c:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::d
	  IL_0061:  callvirt instance void class System.Disposables.CompositeDisposable::Add(class [mscorlib]System.IDisposable)
	  IL_0066:  ldloc.0 
	  IL_0067:  ldc.i4.1 
	  IL_0068:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isAdded
	  IL_006d:  leave.s IL_0076

	} // end .try 0
	finally  { // 0
	  IL_006f:  ldloc.1 
	  IL_0070:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0075:  endfinally 
	} // end handler 0
	IL_0076:  ret 
    } // end of method <>c__DisplayClass14::<Schedule>b__12

  .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass16'
  	extends [mscorlib]System.Object
  {
    .field  public  class System.Concurrency.Scheduler/'<>c__DisplayClass14' 'CS$<>8__locals15'
    .field  public  bool isAdded
    .field  public  bool isDone
    .field  public  class [mscorlib]System.IDisposable d

    // method line 382
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x43c6
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method <>c__DisplayClass16::.ctor

    // method line 383
    .method public hidebysig 
           instance default void '<Schedule>b__13' ()  cil managed 
    {
        // Method begins at RVA 0x43d0
	// Code size 84 (0x54)
	.maxstack 2
	.locals init (
		object	V_0)
	IL_0000:  ldarg.0 
	IL_0001:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass14' System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'CS$<>8__locals15'
	IL_0006:  ldfld class [System.Core]System.Action System.Concurrency.Scheduler/'<>c__DisplayClass14'::recursiveAction
	IL_000b:  callvirt instance void class [System.Core]System.Action::Invoke()
	IL_0010:  ldarg.0 
	IL_0011:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass14' System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'CS$<>8__locals15'
	IL_0016:  ldfld object System.Concurrency.Scheduler/'<>c__DisplayClass14'::gate
	IL_001b:  dup 
	IL_001c:  stloc.0 
	IL_001d:  call void class [mscorlib]System.Threading.Monitor::Enter(object)
	.try { // 0
	  IL_0022:  ldarg.0 
	  IL_0023:  ldfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isAdded
	  IL_0028:  brfalse.s IL_0043

	  IL_002a:  ldarg.0 
	  IL_002b:  ldfld class System.Concurrency.Scheduler/'<>c__DisplayClass14' System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::'CS$<>8__locals15'
	  IL_0030:  ldfld class System.Disposables.CompositeDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'::group
	  IL_0035:  ldarg.0 
	  IL_0036:  ldfld class [mscorlib]System.IDisposable System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::d
	  IL_003b:  callvirt instance bool class System.Disposables.CompositeDisposable::Remove(class [mscorlib]System.IDisposable)
	  IL_0040:  pop 
	  IL_0041:  br.s IL_004a

	  IL_0043:  ldarg.0 
	  IL_0044:  ldc.i4.1 
	  IL_0045:  stfld bool System.Concurrency.Scheduler/'<>c__DisplayClass14'/'<>c__DisplayClass16'::isDone
	  IL_004a:  leave.s IL_0053

	} // end .try 0
	finally  { // 0
	  IL_004c:  ldloc.0 
	  IL_004d:  call void class [mscorlib]System.Threading.Monitor::Exit(object)
	  IL_0052:  endfinally 
	} // end handler 0
	IL_0053:  ret 
    } // end of method <>c__DisplayClass16::<Schedule>b__13

  } // end of class <>c__DisplayClass16

  } // end of class <>c__DisplayClass14

  } // end of class System.Concurrency.Scheduler
}

.namespace System.Linq
{
  .class public sequential ansi sealed beforefieldinit TimeInterval`1<T>
  	extends [mscorlib]System.ValueType
  {
    .field  private  valuetype [mscorlib]System.TimeSpan interval
    .field  private  !0 'value'

    // method line 307
    .method public hidebysig specialname 
           instance default valuetype [mscorlib]System.TimeSpan get_Interval ()  cil managed 
    {
        // Method begins at RVA 0x45c3
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!0>::interval
	IL_0006:  ret 
    } // end of method TimeInterval`1::get_Interval

    // method line 308
    .method public hidebysig specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x45cb
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 valuetype System.Linq.TimeInterval`1<!0>::'value'
	IL_0006:  ret 
    } // end of method TimeInterval`1::get_Value

    // method line 309
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (!T 'value', valuetype [mscorlib]System.TimeSpan interval)  cil managed 
    {
        // Method begins at RVA 0x45d3
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.2 
	IL_0002:  stfld valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!0>::interval
	IL_0007:  ldarg.0 
	IL_0008:  ldarg.1 
	IL_0009:  stfld !0 valuetype System.Linq.TimeInterval`1<!0>::'value'
	IL_000e:  ret 
    } // end of method TimeInterval`1::.ctor

    // method line 310
    .method public virtual hidebysig 
           instance default bool Equals (object obj)  cil managed 
    {
        // Method begins at RVA 0x45e4
	// Code size 71 (0x47)
	.maxstack 2
	.locals init (
		valuetype System.Linq.TimeInterval`1<!T>	V_0,
		valuetype [mscorlib]System.TimeSpan	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  isinst valuetype System.Linq.TimeInterval`1<!0>
	IL_0006:  brtrue.s IL_000a

	IL_0008:  ldc.i4.0 
	IL_0009:  ret 
	IL_000a:  ldarg.1 
	IL_000b:  unbox.any valuetype System.Linq.TimeInterval`1<!0>
	IL_0010:  stloc.0 
	IL_0011:  ldloca.s 0
	IL_0013:  call instance valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!T>::get_Interval()
	IL_0018:  stloc.1 
	IL_0019:  ldloca.s 1
	IL_001b:  ldarg.0 
	IL_001c:  call instance valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!T>::get_Interval()
	IL_0021:  call instance bool valuetype [mscorlib]System.TimeSpan::Equals(valuetype [mscorlib]System.TimeSpan)
	IL_0026:  brfalse.s IL_0045

	IL_0028:  ldarg.0 
	IL_0029:  call instance !0 valuetype System.Linq.TimeInterval`1<!T>::get_Value()
	IL_002e:  box !0
	IL_0033:  ldloca.s 0
	IL_0035:  call instance !0 valuetype System.Linq.TimeInterval`1<!T>::get_Value()
	IL_003a:  box !0
	IL_003f:  call bool object::Equals(object, object)
	IL_0044:  ret 
	IL_0045:  ldc.i4.0 
	IL_0046:  ret 
    } // end of method TimeInterval`1::Equals

    // method line 311
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x4638
	// Code size 64 (0x40)
	.maxstack 2
	.locals init (
		int32	V_0,
		!T	V_1,
		valuetype [mscorlib]System.TimeSpan	V_2)
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 valuetype System.Linq.TimeInterval`1<!T>::get_Value()
	IL_0006:  box !0
	IL_000b:  brfalse.s IL_0023

	IL_000d:  ldarg.0 
	IL_000e:  call instance !0 valuetype System.Linq.TimeInterval`1<!T>::get_Value()
	IL_0013:  stloc.1 
	IL_0014:  ldloca.s 1
	IL_0016:  constrained. !0
	IL_001c:  callvirt instance int32 object::GetHashCode()
	IL_0021:  br.s IL_0028

	IL_0023:  ldc.i4 1963
	IL_0028:  stloc.0 
	IL_0029:  ldarg.0 
	IL_002a:  call instance valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!T>::get_Interval()
	IL_002f:  stloc.2 
	IL_0030:  ldloca.s 2
	IL_0032:  constrained. [mscorlib]System.TimeSpan
	IL_0038:  callvirt instance int32 object::GetHashCode()
	IL_003d:  ldloc.0 
	IL_003e:  xor 
	IL_003f:  ret 
    } // end of method TimeInterval`1::GetHashCode

    // method line 312
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        // Method begins at RVA 0x4684
	// Code size 52 (0x34)
	.maxstack 5
	.locals init (
		object[]	V_0)
	IL_0000:  call class [mscorlib]System.Globalization.CultureInfo class [mscorlib]System.Globalization.CultureInfo::get_CurrentCulture()
	IL_0005:  ldstr "{0}@{1}"
	IL_000a:  ldc.i4.2 
	IL_000b:  newarr [mscorlib]System.Object
	IL_0010:  stloc.0 
	IL_0011:  ldloc.0 
	IL_0012:  ldc.i4.0 
	IL_0013:  ldarg.0 
	IL_0014:  call instance !0 valuetype System.Linq.TimeInterval`1<!T>::get_Value()
	IL_0019:  box !0
	IL_001e:  stelem.ref 
	IL_001f:  ldloc.0 
	IL_0020:  ldc.i4.1 
	IL_0021:  ldarg.0 
	IL_0022:  call instance valuetype [mscorlib]System.TimeSpan valuetype System.Linq.TimeInterval`1<!T>::get_Interval()
	IL_0027:  box [mscorlib]System.TimeSpan
	IL_002c:  stelem.ref 
	IL_002d:  ldloc.0 
	IL_002e:  call string string::Format(class [mscorlib]System.IFormatProvider, string, object[])
	IL_0033:  ret 
    } // end of method TimeInterval`1::ToString

    // method line 313
    .method public static hidebysig specialname 
           default bool op_Equality (valuetype System.Linq.TimeInterval`1<!T> first, valuetype System.Linq.TimeInterval`1<!T> second)  cil managed 
    {
        // Method begins at RVA 0x46c4
	// Code size 20 (0x14)
	.maxstack 8
	IL_0000:  ldarga.s 0
	IL_0002:  ldarg.1 
	IL_0003:  box valuetype System.Linq.TimeInterval`1<!0>
	IL_0008:  constrained. valuetype System.Linq.TimeInterval`1<!0>
	IL_000e:  callvirt instance bool object::Equals(object)
	IL_0013:  ret 
    } // end of method TimeInterval`1::op_Equality

    // method line 314
    .method public static hidebysig specialname 
           default bool op_Inequality (valuetype System.Linq.TimeInterval`1<!T> first, valuetype System.Linq.TimeInterval`1<!T> second)  cil managed 
    {
        // Method begins at RVA 0x46d9
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarga.s 0
	IL_0002:  ldarg.1 
	IL_0003:  box valuetype System.Linq.TimeInterval`1<!0>
	IL_0008:  constrained. valuetype System.Linq.TimeInterval`1<!0>
	IL_000e:  callvirt instance bool object::Equals(object)
	IL_0013:  ldc.i4.0 
	IL_0014:  ceq 
	IL_0016:  ret 
    } // end of method TimeInterval`1::op_Inequality

	.property instance valuetype [mscorlib]System.TimeSpan Interval ()
	{
		.get instance default valuetype [mscorlib]System.TimeSpan System.Linq.TimeInterval`1::get_Interval () 
	}
	.property instance !T Value ()
	{
		.get instance default !0 System.Linq.TimeInterval`1::get_Value () 
	}
  } // end of class System.Linq.TimeInterval`1
}

.namespace System.Linq
{
  .class public sequential ansi sealed beforefieldinit Timestamped`1<T>
  	extends [mscorlib]System.ValueType
  {
    .field  private  valuetype [mscorlib]System.DateTimeOffset timestamp
    .field  private  !0 'value'

    // method line 315
    .method public hidebysig specialname 
           instance default valuetype [mscorlib]System.DateTimeOffset get_Timestamp ()  cil managed 
    {
        // Method begins at RVA 0x46f1
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!0>::timestamp
	IL_0006:  ret 
    } // end of method Timestamped`1::get_Timestamp

    // method line 316
    .method public hidebysig specialname 
           instance default !T get_Value ()  cil managed 
    {
        // Method begins at RVA 0x46f9
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 valuetype System.Linq.Timestamped`1<!0>::'value'
	IL_0006:  ret 
    } // end of method Timestamped`1::get_Value

    // method line 317
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (!T 'value', valuetype [mscorlib]System.DateTimeOffset timestamp)  cil managed 
    {
        // Method begins at RVA 0x4701
	// Code size 15 (0xf)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.2 
	IL_0002:  stfld valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!0>::timestamp
	IL_0007:  ldarg.0 
	IL_0008:  ldarg.1 
	IL_0009:  stfld !0 valuetype System.Linq.Timestamped`1<!0>::'value'
	IL_000e:  ret 
    } // end of method Timestamped`1::.ctor

    // method line 318
    .method public virtual hidebysig 
           instance default bool Equals (object obj)  cil managed 
    {
        // Method begins at RVA 0x4714
	// Code size 66 (0x42)
	.maxstack 3
	.locals init (
		valuetype System.Linq.Timestamped`1<!T>	V_0,
		valuetype [mscorlib]System.DateTimeOffset	V_1)
	IL_0000:  ldarg.1 
	IL_0001:  isinst valuetype System.Linq.Timestamped`1<!0>
	IL_0006:  brtrue.s IL_000a

	IL_0008:  ldc.i4.0 
	IL_0009:  ret 
	IL_000a:  ldarg.1 
	IL_000b:  unbox.any valuetype System.Linq.Timestamped`1<!0>
	IL_0010:  stloc.0 
	IL_0011:  ldloca.s 0
	IL_0013:  call instance valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!T>::get_Timestamp()
	IL_0018:  stloc.1 
	IL_0019:  ldloca.s 1
	IL_001b:  ldarg.0 
	IL_001c:  call instance valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!T>::get_Timestamp()
	IL_0021:  call instance bool valuetype [mscorlib]System.DateTimeOffset::Equals(valuetype [mscorlib]System.DateTimeOffset)
	IL_0026:  brfalse.s IL_0040

	IL_0028:  call class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::get_Default()
	IL_002d:  ldarg.0 
	IL_002e:  call instance !0 valuetype System.Linq.Timestamped`1<!T>::get_Value()
	IL_0033:  ldloca.s 0
	IL_0035:  call instance !0 valuetype System.Linq.Timestamped`1<!T>::get_Value()
	IL_003a:  callvirt instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!T>::Equals(!0, !0)
	IL_003f:  ret 
	IL_0040:  ldc.i4.0 
	IL_0041:  ret 
    } // end of method Timestamped`1::Equals

    // method line 319
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x4764
	// Code size 61 (0x3d)
	.maxstack 2
	.locals init (
		int32	V_0,
		!T	V_1)
	IL_0000:  ldarg.0 
	IL_0001:  call instance !0 valuetype System.Linq.Timestamped`1<!T>::get_Value()
	IL_0006:  box !0
	IL_000b:  brfalse.s IL_0023

	IL_000d:  ldarg.0 
	IL_000e:  call instance !0 valuetype System.Linq.Timestamped`1<!T>::get_Value()
	IL_0013:  stloc.1 
	IL_0014:  ldloca.s 1
	IL_0016:  constrained. !0
	IL_001c:  callvirt instance int32 object::GetHashCode()
	IL_0021:  br.s IL_0028

	IL_0023:  ldc.i4 1979
	IL_0028:  stloc.0 
	IL_0029:  ldarg.0 
	IL_002a:  ldflda valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!0>::timestamp
	IL_002f:  constrained. [mscorlib]System.DateTimeOffset
	IL_0035:  callvirt instance int32 object::GetHashCode()
	IL_003a:  ldloc.0 
	IL_003b:  xor 
	IL_003c:  ret 
    } // end of method Timestamped`1::GetHashCode

    // method line 320
    .method public virtual hidebysig 
           instance default string ToString ()  cil managed 
    {
        // Method begins at RVA 0x47b0
	// Code size 52 (0x34)
	.maxstack 5
	.locals init (
		object[]	V_0)
	IL_0000:  call class [mscorlib]System.Globalization.CultureInfo class [mscorlib]System.Globalization.CultureInfo::get_CurrentCulture()
	IL_0005:  ldstr "{0}@{1}"
	IL_000a:  ldc.i4.2 
	IL_000b:  newarr [mscorlib]System.Object
	IL_0010:  stloc.0 
	IL_0011:  ldloc.0 
	IL_0012:  ldc.i4.0 
	IL_0013:  ldarg.0 
	IL_0014:  call instance !0 valuetype System.Linq.Timestamped`1<!T>::get_Value()
	IL_0019:  box !0
	IL_001e:  stelem.ref 
	IL_001f:  ldloc.0 
	IL_0020:  ldc.i4.1 
	IL_0021:  ldarg.0 
	IL_0022:  call instance valuetype [mscorlib]System.DateTimeOffset valuetype System.Linq.Timestamped`1<!T>::get_Timestamp()
	IL_0027:  box [mscorlib]System.DateTimeOffset
	IL_002c:  stelem.ref 
	IL_002d:  ldloc.0 
	IL_002e:  call string string::Format(class [mscorlib]System.IFormatProvider, string, object[])
	IL_0033:  ret 
    } // end of method Timestamped`1::ToString

    // method line 321
    .method public static hidebysig specialname 
           default bool op_Equality (valuetype System.Linq.Timestamped`1<!T> first, valuetype System.Linq.Timestamped`1<!T> second)  cil managed 
    {
        // Method begins at RVA 0x47f0
	// Code size 20 (0x14)
	.maxstack 8
	IL_0000:  ldarga.s 0
	IL_0002:  ldarg.1 
	IL_0003:  box valuetype System.Linq.Timestamped`1<!0>
	IL_0008:  constrained. valuetype System.Linq.Timestamped`1<!0>
	IL_000e:  callvirt instance bool object::Equals(object)
	IL_0013:  ret 
    } // end of method Timestamped`1::op_Equality

    // method line 322
    .method public static hidebysig specialname 
           default bool op_Inequality (valuetype System.Linq.Timestamped`1<!T> first, valuetype System.Linq.Timestamped`1<!T> second)  cil managed 
    {
        // Method begins at RVA 0x4805
	// Code size 23 (0x17)
	.maxstack 8
	IL_0000:  ldarga.s 0
	IL_0002:  ldarg.1 
	IL_0003:  box valuetype System.Linq.Timestamped`1<!0>
	IL_0008:  constrained. valuetype System.Linq.Timestamped`1<!0>
	IL_000e:  callvirt instance bool object::Equals(object)
	IL_0013:  ldc.i4.0 
	IL_0014:  ceq 
	IL_0016:  ret 
    } // end of method Timestamped`1::op_Inequality

	.property instance valuetype [mscorlib]System.DateTimeOffset Timestamp ()
	{
		.get instance default valuetype [mscorlib]System.DateTimeOffset System.Linq.Timestamped`1::get_Timestamp () 
	}
	.property instance !T Value ()
	{
		.get instance default !0 System.Linq.Timestamped`1::get_Value () 
	}
  } // end of class System.Linq.Timestamped`1
}

.namespace System
{
  .class public sequential ansi sealed beforefieldinit Unit
  	extends [mscorlib]System.ValueType
  	implements class [mscorlib]System.IEquatable`1<valuetype System.Unit>  {
    .pack 0
    .size 1

    // method line 323
    .method public final virtual hidebysig newslot 
           instance default bool Equals (valuetype System.Unit other)  cil managed 
    {
        // Method begins at RVA 0x481d
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.1 
	IL_0001:  ret 
    } // end of method Unit::Equals

    // method line 324
    .method public virtual hidebysig 
           instance default bool Equals (object obj)  cil managed 
    {
        // Method begins at RVA 0x4820
	// Code size 10 (0xa)
	.maxstack 8
	IL_0000:  ldarg.1 
	IL_0001:  isinst System.Unit
	IL_0006:  ldnull 
	IL_0007:  cgt.un 
	IL_0009:  ret 
    } // end of method Unit::Equals

    // method line 325
    .method public virtual hidebysig 
           instance default int32 GetHashCode ()  cil managed 
    {
        // Method begins at RVA 0x482b
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method Unit::GetHashCode

    // method line 326
    .method public static hidebysig specialname 
           default bool op_Equality (valuetype System.Unit unit1, valuetype System.Unit unit2)  cil managed 
    {
        // Method begins at RVA 0x482e
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.1 
	IL_0001:  ret 
    } // end of method Unit::op_Equality

    // method line 327
    .method public static hidebysig specialname 
           default bool op_Inequality (valuetype System.Unit unit1, valuetype System.Unit unit2)  cil managed 
    {
        // Method begins at RVA 0x4831
	// Code size 2 (0x2)
	.maxstack 8
	IL_0000:  ldc.i4.0 
	IL_0001:  ret 
    } // end of method Unit::op_Inequality

  } // end of class System.Unit
}

.namespace System.Collections.Generic
{
  .class interface public auto ansi abstract IEvent`1<TEventArgs>
  {

    // method line 328
    .method public virtual hidebysig newslot abstract specialname 
           instance default object get_Sender ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IEvent`1::get_Sender

    // method line 329
    .method public virtual hidebysig newslot abstract specialname 
           instance default !TEventArgs get_EventArgs ()  cil managed 
    {
        // Method begins at RVA 0x0
    } // end of method IEvent`1::get_EventArgs

	.property instance object Sender ()
	{
		.get instance default object System.Collections.Generic.IEvent`1::get_Sender () 
	}
	.property instance !TEventArgs EventArgs ()
	{
		.get instance default !0 System.Collections.Generic.IEvent`1::get_EventArgs () 
	}
  } // end of class System.Collections.Generic.IEvent`1
}

.namespace System.Collections.Generic
{
  .class private auto ansi beforefieldinit Event`1<TEventArgs>
  	extends [mscorlib]System.Object
  	implements class System.Collections.Generic.IEvent`1<!0>  {
    .field  private  object '<Sender>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

    .field  private  !0 '<EventArgs>k__BackingField'
    .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....


    // method line 330
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' (object sender, !TEventArgs eventArgs)  cil managed 
    {
        // Method begins at RVA 0x4834
	// Code size 21 (0x15)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ldarg.0 
	IL_0007:  ldarg.1 
	IL_0008:  call instance void class System.Collections.Generic.Event`1<!TEventArgs>::set_Sender(object)
	IL_000d:  ldarg.0 
	IL_000e:  ldarg.2 
	IL_000f:  call instance void class System.Collections.Generic.Event`1<!TEventArgs>::set_EventArgs(!0)
	IL_0014:  ret 
    } // end of method Event`1::.ctor

    // method line 331
    .method public final virtual hidebysig newslot specialname 
           instance default object get_Sender ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x484a
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld object class System.Collections.Generic.Event`1<!0>::'<Sender>k__BackingField'
	IL_0006:  ret 
    } // end of method Event`1::get_Sender

    // method line 332
    .method private hidebysig specialname 
           instance default void set_Sender (object 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4852
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld object class System.Collections.Generic.Event`1<!0>::'<Sender>k__BackingField'
	IL_0007:  ret 
    } // end of method Event`1::set_Sender

    // method line 333
    .method public final virtual hidebysig newslot specialname 
           instance default !TEventArgs get_EventArgs ()  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x485b
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldfld !0 class System.Collections.Generic.Event`1<!0>::'<EventArgs>k__BackingField'
	IL_0006:  ret 
    } // end of method Event`1::get_EventArgs

    // method line 334
    .method private hidebysig specialname 
           instance default void set_EventArgs (!TEventArgs 'value')  cil managed 
    {
        .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::'.ctor'() =  (01 00 00 00 ) // ....

        // Method begins at RVA 0x4863
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  stfld !0 class System.Collections.Generic.Event`1<!0>::'<EventArgs>k__BackingField'
	IL_0007:  ret 
    } // end of method Event`1::set_EventArgs

	.property instance object Sender ()
	{
		.get instance default object System.Collections.Generic.Event`1::get_Sender () 
		.set instance default void System.Collections.Generic.Event`1::set_Sender (object 'value') 
	}
	.property instance !TEventArgs EventArgs ()
	{
		.get instance default !0 System.Collections.Generic.Event`1::get_EventArgs () 
		.set instance default void System.Collections.Generic.Event`1::set_EventArgs (!0 'value') 
	}
  } // end of class System.Collections.Generic.Event`1
}

.namespace System.Collections.Generic
{
  .class public auto ansi abstract sealed beforefieldinit Event
  	extends [mscorlib]System.Object
  {

    // method line 335
    .method public static hidebysig 
           default class System.Collections.Generic.IEvent`1<!!TEventArgs> Create<TEventArgs> (object sender, !!TEventArgs eventArgs)  cil managed 
    {
        // Method begins at RVA 0x486c
	// Code size 8 (0x8)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  ldarg.1 
	IL_0002:  newobj instance void class System.Collections.Generic.Event`1<!!TEventArgs>::'.ctor'(object, !0)
	IL_0007:  ret 
    } // end of method Event::Create

  } // end of class System.Collections.Generic.Event
}

