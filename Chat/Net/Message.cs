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
        public enum Header { Registration, Login, Disconnect, SendMessage, GetUsers, NewUser }
        private Header head;
        private List<string> messageList;

        public Message(Header head, string message)
        {
            this.head = head;
            this.messageList = new List<string>();
            messageList.Add(message);
        }

        public Message(Header head, List<string> messages)
        {
            this.head = head;
            this.messageList = messages;
        }

        public Message(Header head)
        {
            this.head = head;
            this.messageList = new List<string>();
        }

        public Header Head
        {
            get { return head; }
            set { head = value; }
        }

        public List<string> MessageList
        {
            get { return messageList; }
            set { messageList = value; }
        }

        public void addData(string message)
        {
            messageList.Add(message);
        }
    }
}
