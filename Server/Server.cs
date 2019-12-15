using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Chat.Auth;
using Chat.Exceptions;

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
        /// Поток, проверяющий входящие сообщения пользователей
        /// </summary>
        public void checkData()
        {
            while (this.Running)
            {
                try
                {
                    if (sessionManager.SessionList.Count > 0)
                    {
                        foreach (Session session in sessionManager.SessionList)
                        {
                            if (session != null && session.Client.GetStream().DataAvailable) //Если клиент присылает данные, то
                            {
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
                    if (ex.SocketErrorCode != SocketError.Interrupted)
                        Console.WriteLine("## При подключении клиента возникло исключение: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для обработки сообщений
        /// </summary>
        /// <param name="session">Сессия, отуда пришло сообщение</param>
        /// <param name="message">Пришедшее сообщение</param>
        public void processData(Session session, Message message)
        {            
            if (session.User != null) //Если пользователь в сессии еще не был создан, значит это либо авторизация, либо регистрация
            {
                switch (message.Head)
                {
                    case Message.Header.Disconnect:
                        shortSend(Message.Header.Disconnect, "success", session.Client.Client);
                        session.Client.Close();
                        sessionManager.removeSession(session.Token);
                        Console.WriteLine($"Клиент отключился ({session.Token})");
                        break;
                    case Message.Header.SendMessage:
                        foreach (Session localSession in sessionManager.SessionList)
                            sendMessage(message, localSession.Client.Client);
                        Console.WriteLine($"Клиент ({session.User.Login}) отправил сообщение");
                        break;
                    case Message.Header.GetUsers:
                        Message reply = new Message(Message.Header.GetUsers);
                        foreach (Session localSession in sessionManager.SessionList)
                            reply.addData(localSession.User?.Login);
                        sendMessage(reply, session.Client.Client);
                        Console.WriteLine($"Клиенту ({session.User.Login}) отправлен список пользователей");
                        break;
                }
            }
            else
            {
                switch (message.Head)
                {
                    case Message.Header.Registration:
                        try
                        {
                            userManager.addUser(message.MessageList[0], message.MessageList[1]);
                            sessionManager.setUser(session.Token, message.MessageList[0], message.MessageList[1]);
                            shortSend(Message.Header.Registration, "success", session.Client.Client);
                            Console.WriteLine($"Пользователь ({message.MessageList[0]}) зарегистрировался");
                            foreach (Session localsession in sessionManager.SessionList)
                            {
                                if (localsession.Token != session.Token)
                                    shortSend(Message.Header.NewUser, message.MessageList[0], localsession.Client.Client);
                            }
                        }
                        catch (Exception ex)
                        {
                            shortSend(Message.Header.Registration, ex.Message, session.Client.Client);
                        }
                        break;
                    case Message.Header.Login:
                        try
                        {
                            userManager.authentify(message.MessageList[0], message.MessageList[1]);
                            shortSend(Message.Header.Login, "success", session.Client.Client);
                            Console.WriteLine($"Пользователь ({message.MessageList[0]}) авторизовался");
                            foreach (Session localsession in sessionManager.SessionList)
                                if (localsession.Token != session.Token)
                                    shortSend(Message.Header.NewUser, message.MessageList[0], localsession.Client.Client);
                        }
                        catch (Exception ex)
                        {
                            shortSend(Message.Header.Login, ex.Message, session.Client.Client);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Метод для отправки ответа от сервера клиенту
        /// </summary>
        /// <param name="head">Тип сообщения</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="socket">Сокет клиента</param>
        private void shortSend(Message.Header head, string text, Socket socket)
        {
            try
            {
                Message message = new Message(head);
                message.addData(text);
                sendMessage(message, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("При отправке сообщения возникло исключение: " + ex.Message);
            }
        }
    }
}
