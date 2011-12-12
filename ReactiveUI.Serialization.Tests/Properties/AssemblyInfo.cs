// <copyright file="AssemblyInfo.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// System.Security
#if !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif

// System.Reflection
[assembly: AssemblyTitle("ReactiveUI.Serialization Pex Tests")]

// System.Runtime.InteropServices
[assembly: ComVisible(false)]
[assembly: Guid("670bd3eb-de63-42fa-92c2-010dc280b0dc")]

// System
[assembly: CLSCompliant(false)]

// System.Runtime.CompilerServices
[assembly: InternalsVisibleTo("Microsoft.Pex, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]

