﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MJoinChat
    {
        private int idChat;
        private int idUser;

        public MJoinChat(int idChat, int idUser)
        {
            IdChat = idChat;
            IdUser = idUser;
        }

        public int IdChat { get => idChat; set => idChat = value; }
        public int IdUser { get => idUser; set => idUser = value; }
    }
}
