using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Exceptions;
using Chat.Net;

namespace Chat.ChatRoom
{
    public class ChatroomManager
    {
        private List<Chatroom> chatrooms;
        private int count;

        public ChatroomManager()
        {
            Chatrooms = new List<Chatroom>();
            Count = 0;
        }

        public List<Chatroom> Chatrooms { get => chatrooms; set => chatrooms = value; }
        public int Count { get => count; set => count = value; }

        /// <summary>
        /// Создает новую комнату
        /// </summary>
        /// <param name="name"></param>
        public void addChatroom(string name)
        {
            if (Chatrooms.Exists(m => m.Name == name))
                throw new ChatroomAlreadyExistException($"Комната с именем {name} уже существует");
            Chatrooms.Add(new Chatroom(count, name));
            count++;
        }

        /// <summary>
        /// Удаляет комнату по ее ID
        /// </summary>
        /// <param name="id"></param>
        public void removeChatroom(int id)
        {
            Chatrooms.RemoveAll(m => m.Id == id);
        }

        /// <summary>
        /// Вход пользователя в комнату по ее ID
        /// </summary>
        /// <param name="id">ID комнаты</param>
        /// <param name="session">Сессия пользователя</param>
        public void userJoin(int id, Session session)
        {
            int index = Chatrooms.FindIndex(m => m.Id == id);
            if (index == -1)
                throw new ChatroomUnknownException("Комната, в которую вы пытаетесь войти, не существует");
            Chatrooms[index].addUser(session);
        }

        /// <summary>
        /// Выход пользователя из комнаты по ее ID
        /// </summary>
        /// <param name="id">ID комнаты</param>
        /// <param name="session">Сессия пользователя</param>
        public void userLeave(int id, Session session)
        {
            // Если комнату удалили до того, как пользователь решил выйти, просто отправим клиенту ответ об успешном выходе
            int index = Chatrooms.FindIndex(m => m.Id == id);   
            if (index != -1)                                    
                Chatrooms[index].Users.RemoveAll(m => m.Token == session.Token);
        }

        public Chatroom getChatroom(int id)
        {
            Chatroom chatroom = Chatrooms.FirstOrDefault(m => m.Id == id);
            if (chatroom == null)
                throw new ChatroomUnknownException("Комната, с которой вы патаетесь взаимодействовать, не существует");
            return chatroom;
        }

        public Chatroom getChatroom(string name)
        {
            Chatroom chatroom = Chatrooms.FirstOrDefault(m => m.Name == name);
            if (chatroom == null)
                throw new ChatroomUnknownException("Комната, с которой вы патаетесь взаимодействовать, не существует");
            return chatroom;
        }
    }
}
