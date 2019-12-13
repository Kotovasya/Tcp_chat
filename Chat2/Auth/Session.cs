using Chat.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net
{
    public class Session
    {
        Guid token;
        User user;
        TcpClient client;

        public Guid Token
        {
            get { return token; }
            set { token = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public TcpClient Client
        {
            get { return client; }
            set { client = value; }
        }

        /// <summary>
        /// Создает сессию со сгенерированным токеном
        /// </summary>
        public Session()
        {
            Token = Guid.NewGuid();
            User = null;
            Client = null;
        }
    }
}
