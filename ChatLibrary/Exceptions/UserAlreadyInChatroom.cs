﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Exceptions
{
    public class UserAlreadyInChatroom : Exception
    {
        public UserAlreadyInChatroom(string message) : base(message)
        {
        }
    }
}
