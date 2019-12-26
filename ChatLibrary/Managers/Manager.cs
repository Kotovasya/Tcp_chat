using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLibrary.Managers
{
#pragma warning disable CS0659
    public abstract class Manager<T, K> : IEquatable<Manager<T, K>>
    {
        public Dictionary<T, K> List { get; set; }

        public Manager()
        {
            List = new Dictionary<T, K>();
        }

        public Manager(Dictionary<T, K> list)
        {
            this.List = list;
        }

        public void add(T t, K k)
        {
            List.Add(t, k);
        }

        public virtual void remove(T t)
        {
            List.Remove(t);
        }

        public virtual K get(T t)
        {
            return List[t];
        }

        public virtual void set(T t, K k)
        {
            List[t] = k;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            return this.Equals(obj as Manager<T, K>);
        }

        public bool Equals(Manager<T, K> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (this.List.Count != other.List.Count)
                return false;
            for (int i = 0; i < this.List.Count; i++)
            {
                if (!this.List.ElementAtOrDefault(i).Value.Equals(other.List.ElementAtOrDefault(i).Value))
                    return false;
            }
            return true;
        }
    }
}
