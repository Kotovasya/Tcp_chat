﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Exceptions
{
    public class SessionUnknownException : Exception
    {
        public SessionUnknownException(string message) : base(message)
        {

        }
    }
}
