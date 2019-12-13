using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Auth
{
    [Serializable()]
    public class User : IComparable<User>
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

        /// <summary>
        /// Проверка входа
        /// </summary>
        /// <param name="other">Введенные параметры</param>
        /// <returns></returns>
        public int CompareTo(User other)
        {
            if (this.login == other.login)
            {
                return 0;
            }

            return -1;
        }
    }
}
