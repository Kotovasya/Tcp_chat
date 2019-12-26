using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary.UserFolder;

namespace ChatLibrary.MessageFolder
{
    public class MConnect
    {
        private User user;

        public MConnect(User user)
        {
            User = user;
        }

        public User User { get => user; set => user = value; }
    }
}
