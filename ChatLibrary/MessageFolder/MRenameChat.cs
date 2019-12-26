using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MRenameChat
    {
        private int idChat;
        private string name;

        public MRenameChat(int idChat, string name)
        {
            IdChat = idChat;
            Name = name;
        }

        public int IdChat { get => idChat; set => idChat = value; }
        public string Name { get => name; set => name = value; }
    }
}
