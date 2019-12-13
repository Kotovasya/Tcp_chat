using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Chat.Auth;

namespace Server
{
    class Server : TCPServer
    {
        SessionManager sessionManager;
        UserManager userManager;

        public void run()
        {
            sessionManager = new SessionManager();
            userManager = new UserManager();

            checkDataThread = new Thread(new ThreadStart(this.checkData));
            checkDataThread.Start();

            checkListenerThread = new Thread(new ThreadStart(this.listen));
            checkListenerThread.Start();
        }

        /// <summary>
        /// Поток, проверяющий входящие сообщения пользователей каждые 30 мс
        /// </summary>
        public void checkData()
        {
            while (this.Running)
            {
                try
                {
                    if (sessionManager.SessionList.Count > 0)
                    {
                        foreach (Session session in sessionManager.SessionList.ToList())
                        {
                            if (session != null && session.Client.GetStream().DataAvailable)
                            {
                                Thread.Sleep(25);
                                Message message = getMessage(session.Client.Client);

                                if (message != null)
                                {
                                    Thread processData = new Thread(() => this.processData(session, message));
                                    processData.Start();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("При принятии сообщения произошло исключение: " + ex.Message);
                }

                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Создание отдельной сессии для каждого подключаемого клиента
        /// </summary>
        public void listen()
        {
            while (this.Running)
            {
                try
                {
                    Console.WriteLine("Ожидание подключения...");
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    Session session = new Session();
                    session.Client = client;
                    sessionManager.addSession(session);
                    Console.WriteLine($"Новый клиент подключен ({session.Token})");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("## При подключении клиента возникло исключение: " + ex.Message);
                }
            }
        }

        //Как идея, создать собственные исключения и обрабатывать их в этом методе
        /// <summary>
        /// Метод для обработки сообщений
        /// </summary>
        /// <param name="session">Сессия, отуда пришло сообщение</param>
        /// <param name="message">Пришедшее сообщение</param>
        public void processData(Session session, Message message)
        {
            if (session.User != null)
            {
                switch (message.Head)
                {
                    case Message.Header.CreateR:
                        break;
                    case Message.Header.JoinR:
                        break;
                    case Message.Header.LeaveR:
                        break;
                    case Message.Header.Disconnect:
                        Message messageSuccess = new Message(Message.Header.Disconnect);
                        messageSuccess.addData("success");
                        sendMessage(messageSuccess, session.Client.Client);
                        session.Client.Close();
                        sessionManager.removeSession(session.Token);
                        Console.WriteLine($"Клиент отключился ({session.Token})");
                        break;
                    case Message.Header.SendMessage:
                        try
                        {
                            foreach (Session localSession in sessionManager.SessionList)
                                sendMessage(message, localSession.Client.Client);
                            Console.WriteLine($"Клиент ({session.User.Login}) отправил сообщение");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("При отправки сообщения возникло исключение: " + ex.Message);
                        }
                        break;
                }
            }
            else
            {
                switch (message.Head)
                {
                    case Message.Header.Connect:
                        userManager.addUser(message.MessageList[0], message.MessageList[1]);
                        Message messageSuccess = new Message(Message.Header.Connect);
                        messageSuccess.MessageList.Add("success");
                        sendMessage(messageSuccess, session.Client.Client);
                        break;
                }
            }
        }
    }
}
