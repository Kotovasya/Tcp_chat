using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary.UserFolder;
using ChatLibrary.MessageFolder;


namespace ChatLibrary.ChatFolder
{
    [Serializable]
    public class FullChat : Chat
    {
        private List<ChatUser> users;
        private List<ChatMessage> messages;

        public FullChat(int id, string name, User user) : base(id, name)
        {
            this.Id = id;
            this.Name = name;
            this.Users = null;
            this.Messages = null;
            Users = new List<ChatUser> { new ChatUser(user, 8) };
        }

        public List<ChatUser> Users { get => users; set => users = value; }
        public List<ChatMessage> Messages { get => messages; set => messages = value; }

        public Chat ToChat()
        {
            return this;
        }

        public void userJoin(User user)
        {
            Users.Add(new ChatUser(user));
            //userJoined?.Invoke(user);
        }

        public void userLeave(User user)
        {
            Users.RemoveAll(m => m.User.Id == user.Id);
            //userLeaves?.Invoke(user);
        }

        public void changeRights(int id, int right)
        {
            Users.FirstOrDefault(m => m.User.Id == id).Righst = right;
        }

        public List<ChatMessage> findMessage(string text)
        {
            List<ChatMessage> returnList = new List<ChatMessage>();
            foreach (ChatMessage message in Messages)
                if (message.Text.IndexOf(text) != -1)
                    returnList.Add(message);
            return returnList;
        }

        public void addMessage(ChatMessage message)
        {
            Messages.Add(message);
        }

        public void deleteMessage(int id)
        {
            Messages.RemoveAll(m => m.Id == id);
        }

        public void editMessage(ChatMessage message)
        {
            ChatMessage editMessage = Messages.FirstOrDefault(m => m.Id == message.Id);
            if (editMessage != null)
            {
                editMessage.Attachments = message.Attachments;
                editMessage.Text = message.Text;
            }
        }

        public void rename(string name)
        {
            this.Name = name;
        }
    }
}
