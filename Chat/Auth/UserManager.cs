using Chat.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Auth
{
    /// <summary>
    /// Класс для управления данными пользователей
    /// </summary>
    public class UserManager
    {
        List<User> userList;

        /// <summary>
        /// Хранилище данных пользователей
        /// </summary>
        public List<User> UserList
        {
            get { return userList; }
            set { userList = value; }
        }

        public UserManager()
        {
            UserList = new List<User>();
        }

        /// <summary>
        /// Добавляет пользователя в хранилище
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        public void addUser(string login, string password)
        {
            User user = userList.FirstOrDefault(m => m.Login == login);
            if (user != null)
                throw new UserAlreadyExistException($"Пользователь с именем {login} уже существует");
            else
                UserList.Add(new User(login));
        }

        /// <summary>
        /// Удаляет пользователя из хранилища по его имени
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        public void removeUser(string login)
        {
            User userToDelete = userList.FirstOrDefault(m => m.Login == login);

            if (userToDelete != null)
                throw new UserUnknownException($"Пользователя с именем {login} не существует");
            else
                UserList.Remove(userToDelete);
        }

        /// <summary>
        /// Поиск пользователя в хранилище
        /// </summary>
        /// <param name="other">Искомый пользователь</param>
        /// <returns></returns>
        public User getUser(User other)
        {
            User getUser = userList.FirstOrDefault(m => m.Login == other.Login);
            if (getUser != null)
                throw new UserUnknownException($"Пользователя с именем {other.Login} не существует");
            else
                return getUser;
        }

        /// <summary>
        /// Метод атуентификации пользователя
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Вводимый пароль</param>
        public void authentify(string login, string password)
        {
            User getUser = userList.FirstOrDefault(m => m.Login == login);
            if (getUser != null)
            {
                if (getUser.Password != password)
                    throw new WrongPasswordException("Неверный пароль");
            }
            else
                throw new WrongLoginException($"Пользователь с именем {login} не зарегистрирован");
        }
    }
}
