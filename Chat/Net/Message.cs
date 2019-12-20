using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net
{
    [Serializable]
    public class Message
    {
        public enum Header { Registration, Login, Disconnect, SendMessage, GetUsers, NewUser, CreateCR, JoinCR, LeaveCR, DeleteCR, GetCR }
        private Header head;
        private List<string> messageList;

        public Message(Header head)
        {
            Head = head;
            MessageList = null;
        }

        public Header Head { get => head; set => head = value; }
        public List<string> MessageList { get => messageList; set => messageList = value; }

        public void addData(string message)
        {
            messageList.Add(message);
        }
    }
}
