using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using ChatLibrary.MessageFolder;
using ChatLibrary.Managers;
using ChatLibrary.UserFolder;
using ChatLibrary.Exceptions;

namespace UnitTests.TestServer
{
    [TestClass]
    public class RegistrationUser
    {

        [TestMethod]
        public void RegistrationValidUser()
        {
            Server.Server server = new Server.Server();
            server.startServer(2020);

            TcpClient client = new TcpClient("127.0.0.1", 2020);

            server.OnlineManager.add(client, null);
            MRegistration message = new MRegistration("228", "Kotovasya");

            server.acceptRegistration(client, message);

            BaseManager testBaseManager = new BaseManager();
            testBaseManager.register(new FullUser(new User(0, "Kotovasya"), "228"));

            Assert.IsTrue(server.BaseManager.Equals(testBaseManager), "BaseManager сервера не соответсвует предполагаемому значению");
        }

        [TestMethod]
        public void RegistrationAlreadyExistUser()
        {
            Server.Server server = new Server.Server();
        
            server.startServer(2020);

            TcpClient client = new TcpClient("127.0.0.1", 2020);

            server.OnlineManager.add(client, null);
            MRegistration message = new MRegistration("228", "Kotovasya");
        
            server.acceptRegistration(client, message);
        
            server.acceptDisconnect(client, new MDisconnect(0));
        
            try
            {
                server.acceptRegistration(client, message);
            }
            catch (UserAlreadyExistException ex)
            {
                StringAssert.Contains(ex.Message, "Пользователь с таким именем уже существует");
                return;
            }
            Assert.Fail("Исключение UserAlreadyExistException не было вызвано");
        }
    }
}
