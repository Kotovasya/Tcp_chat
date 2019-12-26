using ChatLibrary.UserFolder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary.ChatFolder;
using ChatLibrary.Exceptions;

namespace ChatLibrary.Managers
{
    public class BaseManager : Manager<int, FullUser>
    {
        public void rename(int id, string login)
        {
            if (List.FirstOrDefault(m => m.Value.UserInfo.Login == login).Value != null)
                throw new UserAlreadyExistException("");
            List[id].UserInfo.Login = login;
        }

        public void changePassword(int id, string password)
        {
            List[id].Password = password;
        }

        public void changeAvatar(int id, Image avatar)
        {
            List[id].UserInfo.Avatar = avatar;
        }

        public void joinChat(int id, Chat chat)
        {
            List[id].Chats.Add(chat);
        }

        public void leaveChat(int id, int idChat)
        {
            List[id].Chats.RemoveAll(m => m.Id == idChat);
        }

        public void renameChat(int id, int idChat, string name)
        {
            List[id].Chats.Find(m => m.Id == idChat).Name = name;
        }

        public void disconnect(int id)
        {
            List[id].UserInfo.LastOnline = DateTime.UtcNow;
        }

        public void register(FullUser user)
        {
            if (List.FirstOrDefault(m => m.Value.UserInfo.Login == user.UserInfo.Login).Value != null)
                throw new UserAlreadyExistException("");
            add(user.UserInfo.Id, user);
        }

        public FullUser authentify(string login, string password)
        {
            FullUser findUser = List.FirstOrDefault(m => m.Value.UserInfo.Login == login).Value;
            if (findUser == null)
                throw new WrongLoginException("");
            if (findUser.Password != password)
                throw new WrongPasswordException("");
            return findUser;
        }
    }
}
