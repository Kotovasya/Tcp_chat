using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Exceptions
{
    public class SuccessfullyLoginException : Exception
    {
        public SuccessfullyLoginException(string message) : base(message)
        {
        }
    }
}
