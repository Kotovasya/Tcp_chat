using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MRenameUser
    {
        private int idUser;
        private string name;

        public MRenameUser(int idUser, string name)
        {
            IdUser = idUser;
            Name = name;
        }

        public int IdUser { get => idUser; set => idUser = value; }
        public string Name { get => name; set => name = value; }
    }
}
