using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MChangeRights
    {
        private int idChat;
        private int idUser;
        private int rights;

        public MChangeRights(int idChat, int idUser, int rights)
        {
            IdChat = idChat;
            IdUser = idUser;
            Rights = rights;
        }

        public int IdChat { get => idChat; set => idChat = value; }
        public int IdUser { get => idUser; set => idUser = value; }
        public int Rights { get => rights; set => rights = value; }
    }
}
