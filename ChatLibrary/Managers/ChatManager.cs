using ChatLibrary.ChatFolder;
using ChatLibrary.UserFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.Managers
{
    public class ChatManager : Manager<int, FullChat>
    {
        public FullChat newChat(string name, User user)
        {
            List.Add(List.Count + 1, new FullChat(List.Count + 1, name, user));
            return List.Last().Value;
        }

        public void rename(int id, string name)
        {
            List[id].rename(name);
        }

        public void userJoin(int id, User user)
        {
            List[id].userJoin(user);
        }

        public void userLeave(int id, User user)
        {
            List[id].userLeave(user);
        }

        public void changeRights(int id, int idUser, int right)
        {
            List[id].changeRights(idUser, right);
        }
    }
}
