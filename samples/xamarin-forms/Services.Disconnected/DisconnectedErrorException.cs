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
