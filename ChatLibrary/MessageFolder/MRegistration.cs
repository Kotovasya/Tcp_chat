using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MRegistration
    {
        private string login;
        private string password;

        public MRegistration(string password, string login)
        {
            Password = password;
            Login = login;
        }

        public string Password { get => password; set => password = value; }
        public string Login { get => login; set => login = value; }
    }
}
