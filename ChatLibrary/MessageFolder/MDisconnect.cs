using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MDisconnect
    {
        private int id;

        public MDisconnect(int id)
        {
            Id = id;
        }

        public int Id { get => id; set => id = value; }
    }
}
