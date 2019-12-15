using Chat.Exceptions;
using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Auth
{
    /// <summary>
    /// Класс для управления сессиями пользователей
    /// </summary>
    public class SessionManager
    {
        List<Session> sessionList;

        /// <summary>
        /// Хранилище сессий пользователей
        /// </summary>
        public List<Session> SessionList
        {
            get { return sessionList; }
            set { sessionList = value; }
        }

        public SessionManager()
        {
            SessionList = new List<Session>();
        }

        /// <summary>
        /// Добавление новой сессии в менеджер сессий
        /// </summary>
        /// <param name="other">Добавляемая сессия</param>
        public void addSession(Session other)
        {
            Session session = sessionList.FirstOrDefault(m => m.Token == other.Token);
            if (session == null)
                SessionList.Add(other);
            else
                throw new SessionAlreadyExistException($"Сессия с токеном {other.Token} уже существует");
        }

        /// <summary>
        /// Удаление сессии по ее токену
        /// </summary>
        /// <param name="token">Токен удаляемой сессии</param>
        public void removeSession(Guid token)
        {
            Session sessionToDelete = sessionList.FirstOrDefault(m => m.Token == token);
            if (sessionToDelete == null)
                throw new SessionUnknownException($"Сессия с токеном {token} не существует");
            else
                SessionList.Remove(sessionToDelete);
            
        }

        public void setUser(Guid token, string login, string password)
        {
            int index = sessionList.FindIndex(m => m.Token == token);
            if (index == -1)
                throw new SessionUnknownException($"Сессия с токеном {token} не существует");
            else
                SessionList[index].User = new User(login, password);
        }

        public User getUser(Guid token)
        {
            User user = sessionList.FirstOrDefault(m => m.Token == token).User;
            if (user == null)
                throw new SessionUnknownException($"Сессия с токеном {token} не существует");
            else
                return user;
        }
    }
}
