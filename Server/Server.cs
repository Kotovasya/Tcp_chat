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
using Chat.ChatRoom;

namespace Server
{
    class Server : TCPServer
    {
        private SessionManager sessionManager;
        private UserManager userManager;
        private ChatroomManager chatroomManager;

        public SessionManager SessionManager { get => sessionManager; set => sessionManager = value; }
        public UserManager UserManager { get => userManager; set => userManager = value; }
        public ChatroomManager ChatroomManager { get => chatroomManager; set => chatroomManager = value; }

        public void run()
        {
            SessionManager = new SessionManager();
            UserManager = new UserManager();
            chatroomManager = new ChatroomManager();

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
                    if (SessionManager.SessionList.Count > 0)
                    {
                        foreach (Session session in SessionManager.SessionList)
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
                    Console.WriteLine("## При принятии сообщения произошло исключение: " + ex.Message);
                }
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Создание отдельной сессии для каждого подключаемого клиента
        /// </summary>
        private void listen()
        {
            while (this.Running)
            {
                try
                {
                    Console.WriteLine("Ожидание подключения...");
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    Session session = new Session();
                    session.Client = client;
                    SessionManager.addSession(session);
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
        private void processData(Session session, Message message)
        {           
            if (session.User != null) //Если пользователь в сессии еще не был создан, значит это либо авторизация, либо регистрация
            {
                Message reply;
                Chatroom chatroom;
                switch (message.Head)
                {
                    case Message.Header.Disconnect:
                        shortSend(Message.Header.Disconnect, "success", session);
                        session.Client.Close();
                        SessionManager.removeSession(session.Token);
                        Console.WriteLine($"Клиент отключился ({session.Token})");
                        break;
                    case Message.Header.SendMessage:
                        foreach (Session localSession in SessionManager.SessionList)
                            sendMessage(message, localSession.Client.Client);
                        Console.WriteLine($"Клиент ({session.User.Login}) отправил сообщение");
                        break;
                    case Message.Header.GetUsers:
                        reply = new Message(Message.Header.GetUsers);
                        foreach (Session localSession in SessionManager.SessionList)
                            if (localSession.User.IdChatroom == int.Parse(message.MessageList[0]))
                                reply.addData(localSession.User?.Login);
                        sendMessage(reply, session.Client.Client);
                        Console.WriteLine($"Клиенту ({session.User.Login}) отправлен список пользователей");
                        break;
                    case Message.Header.CreateCR:
                        try
                        {
                            ChatroomManager.addChatroom(message.MessageList[0]);
                            ChatroomManager.Chatrooms.Last().addUser(session);
                            UserManager.setUserCR(ChatroomManager.Chatrooms.Last().Id, session.User.Login);
                            reply = new Message(Message.Header.CreateCR);
                            reply.addData("success");
                            reply.addData(ChatroomManager.Count.ToString());
                            reply.addData(message.MessageList[0]);
                            sendMessage(reply, session.Client.Client);
                            foreach (Session localsession in SessionManager.SessionList)
                                shortSend(Message.Header.CreateCR, message.MessageList[0], localsession);
                            Console.WriteLine($"Клиент ({session.User.Login}) создал комнату {message.MessageList[0]}");
                        }
                        catch (ChatroomAlreadyExistException ex)
                        {
                            shortSend(Message.Header.CreateCR, ex.Message, session);
                        }
                        break;
                    case Message.Header.DeleteCR:
                        try
                        {
                            chatroom = ChatroomManager.getChatroom(int.Parse(message.MessageList[0]));
                            foreach (Session localsession in chatroom.Users)
                                sendMessage(message, localsession.Client.Client);
                            chatroomManager.removeChatroom(chatroom.Id);
                            Console.WriteLine($"Клиент ({session.User.Login}) удалил комнату {chatroom.Name}");
                        }
                        catch (Exception ex)
                        {
                            shortSend(Message.Header.DeleteCR, ex.Message, session);
                        }
                        break;
                    case Message.Header.JoinCR: // 0 - имя комнаты, 1 - имя пользователя, 2 - название комнаты
                        try
                        {   //При получении сообщения, у клиента проверяем, находится ли он в комнате с таким же ID
                            chatroom = ChatroomManager.getChatroom(message.MessageList[0]);
                            ChatroomManager.userJoin(chatroom.Id, session);
                            UserManager.setUserCR(chatroom.Id, session.User.Login);
                            reply = new Message(Message.Header.JoinCR);
                            reply.addData(chatroom.Id.ToString());
                            reply.addData(session.User.Login);
                            reply.addData(chatroom.Name);
                            foreach (Session localsession in chatroom.Users)
                                sendMessage(reply, localsession.Client.Client);
                            Console.WriteLine($"Клиент ({session.User.Login}) зашел в комнату {chatroom.Name}");
                        }
                        catch (ChatroomUnknownException ex)
                        {
                            shortSend(Message.Header.JoinCR, ex.Message, session);
                        }
                        catch (UserAlreadyInChatroom ex)
                        {
                            shortSend(Message.Header.JoinCR, ex.Message, session);
                        }
                        catch { }
                        break;
                    case Message.Header.LeaveCR: // 0 - id комнаты, 1 - имя пользователя. При возникновении ошибки, у клиента нужно проверять на int
                        try
                        {
                            chatroom = ChatroomManager.getChatroom(int.Parse(message.MessageList[0]));
                            ChatroomManager.userLeave(chatroom.Id, session);
                            UserManager.setUserCR(chatroom.Id, session.User.Login);
                            shortSend(Message.Header.LeaveCR, "success", session);
                            foreach (Session localsession in chatroom.Users)
                                sendMessage(message, localsession.Client.Client);
                            Console.WriteLine($"Клиент ({session.User.Login}) вышел из комнаты {chatroom.Name}");
                        }
                        catch (ChatroomUnknownException ex)
                        {
                            shortSend(Message.Header.LeaveCR, ex.Message, session);
                        }
                        break;
                    case Message.Header.GetCR:
                        reply = new Message(Message.Header.GetCR);
                        foreach (Chatroom localchatroom in ChatroomManager.Chatrooms)
                            reply.addData(localchatroom.Name);
                        sendMessage(reply, session.Client.Client);
                        Console.WriteLine($"Клиенту ({session.User.Login}) отправлен список комнат");
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
                            UserManager.addUser(message.MessageList[0], message.MessageList[1]);
                            SessionManager.setUser(session.Token, message.MessageList[0], message.MessageList[1]);
                            shortSend(Message.Header.Registration, "success", session);
                            Console.WriteLine($"Пользователь ({message.MessageList[0]}) зарегистрировался");
                            foreach (Session localsession in SessionManager.SessionList)
                            {
                                if (localsession.Token != session.Token)
                                    shortSend(Message.Header.NewUser, message.MessageList[0], localsession);
                            }
                        }
                        catch (Exception ex)
                        {
                            shortSend(Message.Header.Registration, ex.Message, session);
                        }
                        break;
                    case Message.Header.Login:
                        try
                        {
                            UserManager.authentify(message.MessageList[0], message.MessageList[1]);
                            SessionManager.setUser(session.Token, message.MessageList[0], message.MessageList[1]);
                            shortSend(Message.Header.Login, "success", session);
                            Console.WriteLine($"Пользователь ({message.MessageList[0]}) авторизовался");
                            foreach (Session localsession in SessionManager.SessionList)
                                if (localsession.Token != session.Token)
                                    shortSend(Message.Header.NewUser, message.MessageList[0], localsession);
                        }
                        catch (Exception ex)
                        {
                            shortSend(Message.Header.Login, ex.Message, session);
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
        private void shortSend(Message.Header head, string text, Session session)
        {
            Message message = new Message(head);
            message.addData(text);
            sendMessage(message, session.Client.Client);
        }
    }
}
