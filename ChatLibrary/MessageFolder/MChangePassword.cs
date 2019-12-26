using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MChangePassword
    {
        private int id;
        private string newPassword;

        public MChangePassword(int id, string message)
        {
            Id = id;
            NewPassowrd = message;
        }

        public int Id { get => id; set => id = value; }
        public string NewPassowrd { get => newPassword; set => newPassword = value; }
    }
}
