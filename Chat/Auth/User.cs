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

        public User(string login)
        {
            this.login = login;
            this.password = "";
        }

        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        public User()
        {
            this.login = "";
            this.password = "";
        }
    }
}
