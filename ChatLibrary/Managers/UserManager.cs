using ChatLibrary.UserFolder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Managers
{
    public class UserManager : Manager<TcpClient, User>
    {
        public void changeUsername(TcpClient client, string login)
        {
            List[client].Login = login;
        }

        public void changeAvatar(TcpClient client, Image avatar)
        {
            List[client].Avatar = avatar;
        }

        public override User get(TcpClient t)
        {
            return List.FirstOrDefault(m => m.Key.Client == t.Client).Value;
        }

        public override void remove(TcpClient t)
        {
            List.Remove(List.FirstOrDefault(m => m.Key.Client == t.Client).Key);
        }

        public override void set(TcpClient t, User k)
        {
            List[List.FirstOrDefault(m => m.Key.Client == t.Client).Key] = k;
        }
    }
}
