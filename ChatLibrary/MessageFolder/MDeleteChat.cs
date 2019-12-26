using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MDeleteChat
    {
        private int idChat;

        public MDeleteChat(int idChat)
        {
            IdChat = idChat;
        }

        public int IdChat { get => idChat; set => idChat = value; }
    }
}
