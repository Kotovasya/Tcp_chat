using ChatLibrary.Managers;
using ChatLibrary.MessageFolder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary.Exceptions;
using ChatLibrary.UserFolder;
using ChatLibrary.ChatFolder;

namespace UnitTests.TestServer
{
    [TestClass]
    public class LoginUser
    {
        [TestMethod]
        public void LoginValid()
        {
            Server.Server server = new Server.Server();
            server.startServer(2020);
            TcpClient client = new TcpClient("127.0.0.1", 2020);

            server.BaseManager.add(0, new FullUser(new User(0, "Kotovasya"), "228"));
            server.OnlineManager.add(client, null);

            UserManager testOnlineManager = new UserManager();
            testOnlineManager.add(client, null);
            testOnlineManager.set(client, new User(0, "Kotovasya"));

            MLogin messageLogin = new MLogin("Kotovasya", "228");
            server.acceptLogin(client, messageLogin);

            Assert.IsTrue(server.OnlineManager.Equals(testOnlineManager), "Авторизация не была произведена");
        }

        [TestMethod]
        public void LoginWrong()
        {
        
            Server.Server server = new Server.Server();
            server.startServer(2020);
            TcpClient client = new TcpClient("127.0.0.1", 2020);

            server.BaseManager.add(0, new FullUser(new User(0, "Kotovasya"), "228"));
            server.OnlineManager.add(client, null);

            MLogin messageLogin = new MLogin("gagag", "228");
            try
            {
                server.acceptLogin(client, messageLogin);
            }
            catch (WrongLoginException ex)
            {
                StringAssert.Contains(ex.Message, "Пользователь с таким именем не зарегистрирован");
                return;
            }
            Assert.Fail("Исключение WrongLoginException не было вызвано");
        }

        [TestMethod]
        public void PasswordWrong()
        {
            
            Server.Server server = new Server.Server();
            server.startServer(2020);
            TcpClient client = new TcpClient("127.0.0.1", 2020);

            server.BaseManager.add(0, new FullUser(new User(0, "Kotovasya"), "228"));
            server.OnlineManager.add(client, null);

            MLogin messageLogin = new MLogin("Kotovasya", "111");
            try
            {
                server.acceptLogin(client, messageLogin);
            }
            catch (WrongPasswordException ex)
            {
                StringAssert.Contains(ex.Message, "Неверный пароль");
                return;
            }
            Assert.Fail("Исключение WrongPasswordException не было вызвано");
        }
    }
}
