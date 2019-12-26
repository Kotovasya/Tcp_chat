using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.ChatFolder
{
    [Serializable]
    #pragma warning disable CS0659
    public class Chat : IEquatable<Chat>
    {
        private int id;
        private string name;

        public Chat(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            return this.Equals(obj as Chat);
        }

        public bool Equals(Chat other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (this.Id == other.Id && this.Name == other.Name)
                return true;
            else
                return false;
        }
    }
}
