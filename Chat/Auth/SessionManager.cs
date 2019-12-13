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
            {
                SessionList.Add(other);
                Console.WriteLine($"Сессия с токеном {other.Token} успешно добавлена");
            }
            else
                Console.WriteLine($"## При добавлении сессии возникла ошибка. Токен {other.Token} уже существует");

        }

        /// <summary>
        /// Удаление сессии по ее токену
        /// </summary>
        /// <param name="token">Токен удаляемой сессии</param>
        public void removeSession(Guid token)
        {
            Session sessionToDelete = sessionList.FirstOrDefault(m => m.Token == token);

            if (sessionToDelete == null)
                Console.WriteLine($"## При удалении сессии возникла ошибка. Токен {token} не существует");
            else
            {
                SessionList.Remove(sessionToDelete);
                Console.WriteLine($"Сессия с токеном {token} успешно удалена.");
            }
        }
    }
}
