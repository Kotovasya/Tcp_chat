using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary.ChatFolder;

namespace ChatLibrary.UserFolder
{
    [Serializable]
    #pragma warning disable CS0659
    public class FullUser : IEquatable<FullUser>
    {
        private User userInfo;
        private string password;
        private List<Chat> chats;
        public delegate void disconnectDelegate();
        public event disconnectDelegate disconnected;

        public FullUser(User user, string password)
        {
            UserInfo = user;
            Password = password;
        }

        public string Password { get => password; set => password = value; }
        public List<Chat> Chats { get => chats; set => chats = value; }
        public User UserInfo { get => userInfo; set => userInfo = value; }

        public void rename(string username)
        {
            this.UserInfo.Login = username;
        }

        public void changePassword(string password)
        {
            this.Password = password;
        }

        public void disconnect()
        {
            disconnected?.Invoke();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            return this.Equals(obj as FullUser);
        }

        public bool Equals(FullUser other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (this.UserInfo.Equals(other.UserInfo) && this.Password == other.Password)
                return true;
            else
                return false;
        }
    }
}
