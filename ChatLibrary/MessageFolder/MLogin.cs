using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MLogin
    {
        private string login;
        private string password;

        public MLogin(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
    }
}
