using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MEditChatMessage
    {
        private int idChat;
        private ChatMessage message;

        public MEditChatMessage(int idChat, ChatMessage message)
        {
            IdChat = idChat;
            Message = message;
        }

        public int IdChat { get => idChat; set => idChat = value; }
        public ChatMessage Message { get => message; set => message = value; }
    }
}
