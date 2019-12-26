using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Net
{
    [Serializable]
    public class Message
    {
        public enum Header {
            Registration, Login, Connect, Disconnect, RenameUser, GetListChat, ChangePassword,
            JoinChat, LeaveChat, SendChatMessage, CreateChat, DeleteChat, RenameChat, ChangeRights, EditChatMessage, DeleteChatMessage }
        private Header head;
        private object content;

        public Message(Header head, object content)
        {
            Head = head;
            Content = content;
        }

        public Header Head { get => head; set => head = value; }
        public object Content { get => content; set => content = value; }
    }
}
