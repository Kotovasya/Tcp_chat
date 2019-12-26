using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatLibrary.Exceptions;
using ChatLibrary.UserFolder;
using ChatLibrary.ChatFolder;
using ChatLibrary.Managers;
using ChatLibrary.MessageFolder;
using System.Net.Sockets;

namespace UnitTests.TestServer
{
    /// <summary>
    /// Сводное описание для GetListChat
    /// </summary>
    [TestClass]
    public class GetListChat
    {        
        [TestMethod]
        public void GetTwoChats()
        {
            Server.Server server = new Server.Server();
            server.startServer(2020);
            TcpClient client = new TcpClient("127.0.0.1", 2020);

            User user = new User(0, "Kotovasya");
            FullUser fullUser = new FullUser(user, "228");

            server.BaseManager.add(0, fullUser);
            server.OnlineManager.add(client, user);

            server.ChatManager.newChat("Test1", user);
            server.ChatManager.newChat("Test2", user);

            List<Chat> testChats = new List<Chat>() { new Chat(1, "Test1"), new Chat(2, "Test2") };
            List<Chat> chats = new List<Chat>();
            foreach (KeyValuePair<int, FullChat> chat in server.ChatManager.List)
            {
                chats.Add(chat.Value.ToChat());
            }

            Assert.IsTrue(chats.Equals(testChats), "Полученные чаты не совпадают с ожидаемыми");
        }
    }
}
