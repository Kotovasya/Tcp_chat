using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Auth
{
    [Serializable()]
    public class User
    {
        string login;
        string password;
        int idChatroom;

        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int IdChatroom { get => idChatroom; set => idChatroom = value; }

        public User(string login)
        {
            this.login = login;
            this.password = "";
            this.IdChatroom = -1;
        }

        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
            this.IdChatroom = -1;
        }

        public User()
        {
            this.login = "";
            this.password = "";
            this.IdChatroom = -1;
        }
    }
}
