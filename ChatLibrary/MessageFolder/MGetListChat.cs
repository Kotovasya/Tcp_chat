using ChatLibrary.ChatFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MGetListChat
    {
        private List<ChatFolder.Chat> chatList;

        public MGetListChat(List<Chat> chatList)
        {
            ChatList = chatList;
        }

        public List<Chat> ChatList { get => chatList; set => chatList = value; }
    }
}
