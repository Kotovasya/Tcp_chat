using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.ChatRoom
{
    public class Chatroom
    {
        private int id;
        private string name;
        private List<Session> users;

        public Chatroom(int id, string name)
        {
            Id = id;
            Name = name;
            Users = new List<Session>();
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public List<Session> Users { get => users; set => users = value; }

        public void addUser(Session session)
        {
            if (Users.Exists(m => m.Token == session.Token))
                throw new Exceptions.UserAlreadyInChatroom($"Пользователь с именем {session.User.Login} уже есть в комнате");
            Users.Add(session);
        }
    }
}
