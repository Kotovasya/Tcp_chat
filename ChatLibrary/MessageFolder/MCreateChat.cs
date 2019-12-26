using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class MCreateChat
    {
        private string name;

        public MCreateChat(string name)
        {
            Name = name;
        }

        public string Name { get => name; set => name = value; }
    }
}
