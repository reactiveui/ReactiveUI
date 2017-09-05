// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disconnected
{
    public class DisconnectedErrorException : Exception
    {
        public DisconnectedErrorException()
        {
        }

        public DisconnectedErrorException(string message) : base(message)
        {
        }

        public DisconnectedErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
