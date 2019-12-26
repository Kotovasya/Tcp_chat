using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatLibrary.Exceptions;
using ChatLibrary.UserFolder;
using ChatLibrary.ChatFolder;
using ChatLibrary.Managers;
using ChatLibrary.MessageFolder;

namespace UnitTests.TestServer
{
    [TestClass]
    public class CreateChat
    {
        [TestMethod]
        public void CreateValidChat()
        {
            Server.Server server = new Server.Server();
            server.startServer(2020);
            TcpClient client = new TcpClient("127.0.0.1", 2020);

            User user = new User(0, "Kotovasya");
            FullUser fullUser = new FullUser(user, "228");
            server.BaseManager.add(0, fullUser);
            server.OnlineManager.add(client, user);

            server.acceptCreateChat(client, new MCreateChat("Test"));

            ChatManager testChatManager = new ChatManager();
            testChatManager.add(1, new FullChat(1, "Test", user));

            Assert.IsTrue(server.ChatManager.Equals(testChatManager), "Созданный чат не соответствует ожидаемому");
        }
    }
}
