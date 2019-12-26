using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Exceptions
{
    public class SessionAlreadyExistException : Exception
    {
        public SessionAlreadyExistException(string message) : base(message)
        {

        }
    }
}
