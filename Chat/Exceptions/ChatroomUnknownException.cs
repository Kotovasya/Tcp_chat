﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Exceptions
{
    public class ChatroomUnknownException : Exception
    {
        public ChatroomUnknownException(string message) : base(message)
        {

        }
    }
}
