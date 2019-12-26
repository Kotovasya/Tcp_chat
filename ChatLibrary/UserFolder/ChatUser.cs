using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.UserFolder
{
    [Serializable]
    #pragma warning disable CS0659
    public class ChatUser : IEquatable<ChatUser>
    {
        private User user;
        private int righst; // 2 - пользователь, 4 - модератор, 8 - администратор
        private bool notification;

        public ChatUser(User user)
        {
            User = user;
            Righst = 2;
            Notification = true;
        }

        public ChatUser(User user, int right)
        {
            User = user;
            Righst = right;
            Notification = true;
        }

        public bool Notification { get => notification; set => notification = value; }
        public int Righst { get => righst; set => righst = value; }
        public User User { get => user; set => user = value; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            return this.Equals(obj as ChatUser);
        }

        public bool Equals(ChatUser other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            return this.User.Equals(other.User);
        }
    }
}
