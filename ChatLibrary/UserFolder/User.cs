using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.UserFolder
{
    [Serializable]
    #pragma warning disable CS0659
    public class User : IEquatable<User>
    {
        private int id;
        private string login;
        private DateTime lastOnline;
        private Image avatar;
        
        public User(int id, string login)
        {
            this.Id = id;
            this.Login = login;
            this.LastOnline = DateTime.MinValue;
            this.Avatar = null;
        }

        public int Id { get => id; set => id = value; }
        public string Login { get => login; set => login = value; }
        public DateTime LastOnline { get => lastOnline; set => lastOnline = value; }
        public Image Avatar { get => avatar; set => avatar = value; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            return this.Equals(obj as User);
        }

        public bool Equals(User other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (this.Id == other.Id && this.Login == other.Login)
                return true;
            else
                return false;
        }
    }
}
