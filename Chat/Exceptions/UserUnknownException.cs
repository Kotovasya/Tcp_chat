using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Exceptions
{
    public class UserUnknownException : Exception
    {
        public UserUnknownException(string message) : base(message)
        {

        }
    }
}
