using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MDeleteChatMessage
    {
        private int idMessage;
        private int idChat;

        public MDeleteChatMessage(int idMessage, int idChat)
        {
            IdMessage = idMessage;
            IdChat = idChat;
        }

        public int IdMessage { get => idMessage; set => idMessage = value; }
        public int IdChat { get => idChat; set => idChat = value; }
    }
}
