using ChatLibrary.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using ChatLibrary.Exceptions;
using ChatLibrary.UserFolder;
using ChatLibrary.ChatFolder;
using ChatLibrary.MessageFolder;
using ChatLibrary.Managers;

namespace Server
{
    public class Server : TCPServer
    {
        private UserManager onlineManager;
        private ChatManager chatManager;
        private BaseManager baseManager;
        private int countIdUsers;

        public Server()
        {
            CountIdUsers = 0;
            OnlineManager = new UserManager();
            ChatManager = new ChatManager();
            BaseManager = new BaseManager();
        }

        public UserManager OnlineManager { get => onlineManager; set => onlineManager = value; }
        public ChatManager ChatManager { get => chatManager; set => chatManager = value; }
        public BaseManager BaseManager { get => baseManager; set => baseManager = value; }
        public int CountIdUsers { get => countIdUsers; set => countIdUsers = value; }

        public void run()
        {
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
                    if (OnlineManager.List.Count > 0)
                    {
                        foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
                        {
                            if (user.Key.GetStream().DataAvailable) //Если клиент присылает данные, то
                            {
                                Message message = getMessage(user.Key.Client);
                                if (message != null)
                                {
                                    Thread processData = new Thread(() => this.processData(user.Key, message));
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
                    OnlineManager.add(client, null);
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
        /// <param name="fromUser">Сессия, отуда пришло сообщение</param>
        /// <param name="message">Пришедшее сообщение</param>
        private void processData(TcpClient fromClient, Message message)
        {           
            switch (message.Head)
            {
                case Message.Header.Registration:
                    acceptRegistration(fromClient, message.Content as MRegistration);
                    break;
                case Message.Header.Login:
                    acceptLogin(fromClient, message.Content as MLogin);
                    break;
                case Message.Header.Disconnect:
                    acceptDisconnect(fromClient, message.Content as MDisconnect);
                    break;
                case Message.Header.GetListChat:
                    acceptGetListChat(fromClient);
                    break;
                case Message.Header.CreateChat:
                    acceptCreateChat(fromClient, message.Content as MCreateChat);
                    break;
                case Message.Header.DeleteChat:
                    acceptDeleteChat(message);
                    break;
                case Message.Header.JoinChat:
                    acceptJoinChat(fromClient, message.Content as MJoinChat);
                    break;
                case Message.Header.LeaveChat:
                    acceptLeaveChat(fromClient, message.Content as MLeaveChat);
                    break;
                case Message.Header.SendChatMessage:
                    acceptSendChatMessage(message.Content as MSendChatMessage);
                    break;
                case Message.Header.EditChatMessage:
                    acceptEditChatMessage(message.Content as MEditChatMessage);
                    break;
                case Message.Header.DeleteChatMessage:
                    acceptDeleteChatMessage(message.Content as MDeleteChatMessage);
                    break;
                case Message.Header.ChangeRights:
                    acceptChangeRights(message.Content as MChangeRights);
                    break;
                case Message.Header.RenameChat:
                    acceptRenameChat(message.Content as MRenameChat);
                    break;
                case Message.Header.RenameUser:
                    acceptReameUser(fromClient, message.Content as MRenameUser);
                    break;
                case Message.Header.ChangePassword:
                    acceptChangePassword(message.Content as MChangePassword);
                    break;
            }
        }

        public void acceptRegistration(TcpClient client, MRegistration message)
        {
            try
            {
                FullUser newUser = new FullUser(new User(CountIdUsers++, message.Login), message.Password);
                BaseManager.register(newUser);
                OnlineManager.set(client, newUser.UserInfo);
                sendMessage(new Message(Message.Header.Registration, newUser), client.Client);
                foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
                {
                    if (user.Value != null)
                        if (user.Value.Id != newUser.UserInfo.Id)
                            sendMessage(new Message(Message.Header.Connect, newUser.UserInfo), user.Key.Client);
                }
            }
            catch (UserAlreadyExistException)
            {
                sendMessage(new Message(Message.Header.Registration, "Пользователь с таким именем уже существует"), client.Client);
                throw new UserAlreadyExistException("Пользователь с таким именем уже существует"); //Unit Тест
            }
            catch { }
        }

        public void acceptLogin(TcpClient client, MLogin message)
        {
            try
            {
                FullUser authUser = BaseManager.authentify(message.Login, message.Password);
                OnlineManager.set(client, authUser.UserInfo);
                foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
                {
                    if (user.Value != null)
                        if (user.Value.Id != authUser.UserInfo.Id)
                            sendMessage(new Message(Message.Header.Connect, authUser.UserInfo), user.Key.Client);
                }
                sendMessage(new Message(Message.Header.Login, authUser), client.Client);
            }
            catch (WrongLoginException)
            {
                sendMessage(new Message(Message.Header.Login, "Пользователь с таким именем не зарегистрирован"), client.Client);
                throw new WrongLoginException("Пользователь с таким именем не зарегистрирован"); // Unit Test
            }
            catch (WrongPasswordException)
            {
                sendMessage(new Message(Message.Header.Login, "Неверный пароль"), client.Client);
                throw new WrongPasswordException("Неверный пароль"); // Unit Test
            }
        }

        public void acceptDisconnect(TcpClient client, MDisconnect message)
        {
            try
            {
                OnlineManager.remove(client);
                BaseManager.disconnect(message.Id);
                foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
                    if (user.Value != null)
                        sendMessage(new Message(Message.Header.Disconnect, message), user.Key.Client);
            }
            catch (UserUnknownException) { }
        }

        public void acceptGetListChat(TcpClient client)
        {
            List<Chat> chats = new List<Chat>();
            foreach (KeyValuePair<int, FullChat> chat in ChatManager.List)
            {
                chats.Add(chat.Value.ToChat());
            }
            sendMessage(new Message(Message.Header.GetListChat, new MGetListChat(chats)), client.Client);
        }

        public void acceptCreateChat(TcpClient client, MCreateChat message)
        {
            FullChat newChat = ChatManager.newChat(message.Name, OnlineManager.get(client));
            sendMessage(new Message(Message.Header.CreateChat, newChat), client.Client);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.CreateChat, newChat.ToChat()), user.Key.Client);
            }
        }

        public void acceptDeleteChat(Message message)
        {
            ChatManager.remove((message.Content as MDeleteChat).IdChat);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                {
                    sendMessage(message, user.Key.Client);
                }
            }
        }

        public void acceptJoinChat(TcpClient client, MJoinChat message)
        {
            ChatManager.userJoin(message.IdChat, OnlineManager.get(client));
            sendMessage(new Message(Message.Header.JoinChat, ChatManager.get(message.IdChat)), client.Client);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.JoinChat, message), user.Key.Client);
            }
        }

        public void acceptLeaveChat(TcpClient client, MLeaveChat message)
        {
            ChatManager.userLeave(message.IdChat, OnlineManager.get(client));
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.LeaveChat, message), user.Key.Client);
            }
        }

        public void acceptSendChatMessage(MSendChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.addMessage(message.Message);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (chat.Users.FirstOrDefault(m => m.User.Id == user.Value.Id) != null)
                        sendMessage(new Message(Message.Header.SendChatMessage, message), user.Key.Client);
            }
        }

        public void acceptEditChatMessage(MEditChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.editMessage(message.Message);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (chat.Users.FirstOrDefault(m => m.User.Id == user.Value.Id) != null)
                        sendMessage(new Message(Message.Header.EditChatMessage, message), user.Key.Client);
            }
        }

        public void acceptDeleteChatMessage(MDeleteChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.deleteMessage(message.IdMessage);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (chat.Users.FirstOrDefault(m => m.User.Id == user.Value.Id) != null)
                        sendMessage(new Message(Message.Header.DeleteChatMessage, message), user.Key.Client);
            }
        }

        public void acceptChangeRights(MChangeRights message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.changeRights(message.IdUser, message.Rights);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (chat.Users.FirstOrDefault(m => m.User.Id == user.Value.Id) != null)
                        sendMessage(new Message(Message.Header.ChangeRights, message), user.Key.Client);
            }
        }

        public void acceptRenameChat(MRenameChat message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.rename(message.Name);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    if (chat.Users.FirstOrDefault(m => m.User.Id == user.Value.Id) != null)
                        sendMessage(new Message(Message.Header.RenameChat, message), user.Key.Client);
            }
        }

        public void acceptReameUser(TcpClient client, MRenameUser message)
        {
            BaseManager.rename(message.IdUser, message.Name);
            OnlineManager.changeUsername(client, message.Name);
            foreach (KeyValuePair<TcpClient, User> user in OnlineManager.List)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.RenameUser, message), user.Key.Client);
            }
        }

        public void acceptChangePassword(MChangePassword message)
        {
            BaseManager.changePassword(message.Id, message.NewPassowrd);
        }
    }
}
