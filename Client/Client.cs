using ChatLibrary.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatLibrary.UserFolder;
using ChatLibrary.ChatFolder;
using ChatLibrary.MessageFolder;
using ChatLibrary.Exceptions;
using ChatLibrary.Managers;

namespace Client
{
    public class Client : TCPClient
    {
        private ThreadedBindingList<string> chatUsers;
        private ThreadedBindingList<string> messages;
        private ThreadedBindingList<Chat> allChatrooms;
        private ChatManager chatManager;
        private FullUser user;
        private Thread checkConnection;
        private Thread checkDataThread;
        private bool haveCR;
        private FullChat chatroom;
        private List<User> online;

        public delegate void changeUI(bool isJoin);
        public delegate void exception(Exception ex);
        public event changeUI eventChangeUI;
        public event exception eventException;


        public Client(object state)
        {
            Messages = new ThreadedBindingList<string>();
            ChatUsers = new ThreadedBindingList<string>();
            AllChatrooms = new ThreadedBindingList<Chat>();
        }

        public FullUser User { get => user; set => user = value; }
        public ThreadedBindingList<string> Messages { get => messages; set => messages = value; }
        public ThreadedBindingList<string> ChatUsers { get => chatUsers; set => chatUsers = value; }
        public Thread CheckConnection { get => checkConnection; set => checkConnection = value; }
        public Thread CheckDataThread { get => checkDataThread; set => checkDataThread = value; }
        public bool HaveCR { get => haveCR; set => haveCR = value; }
        public FullChat Chatroom { get => chatroom; set => chatroom = value; }
        public ThreadedBindingList<Chat> AllChatrooms { get => allChatrooms; set => allChatrooms = value; }
        public ChatManager ChatManager { get => chatManager; set => chatManager = value; }
        public List<User> Online { get => online; set => online = value; }

        /// <summary>
        /// Запустить потоки для обработки клиентом входящих сообщений
        /// </summary>
        public void run()
        {
            CheckDataThread = new Thread(new ThreadStart(checkData));
            CheckDataThread.Start();

            CheckConnection = new Thread(new ThreadStart(checkConnect));
            CheckConnection.Start();
        }

        /// <summary>
        /// Поток, принимающий сообщения
        /// </summary>
        public void checkData()
        {
            while (Connected)
            {
                try
                {
                    if (tcpClient.GetStream().DataAvailable)
                    {
                        Message message = getMessage();

                        if (message != null)
                        {
                            Thread processData = new Thread(() => this.processData(message));
                            processData.Start();
                        }
                    }
                }
                catch (SocketException ex)
                {
                    eventException?.Invoke(ex);
                }
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Поток, проверяющий подключение к серверу
        /// </summary>
        public void checkConnect()
        {
            while (Connected)
            {
                Socket socket = tcpClient.Client;
                if (socket.Poll(10, SelectMode.SelectRead) && socket.Available >= 0)
                    Connected = false;

                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Поток, обрабатывающий сообщения и добавляющий их в BindingList
        /// </summary>
        /// <param name="message"></param>
        private void processData(Message message)
        {
            switch (message.Head)
            {
                case Message.Header.Registration:
                    acceptRegistration(message);
                    break;
                case Message.Header.Login:
                    acceptLogin(message);
                    break;
                case Message.Header.Disconnect:
                    acceptDisconnect(message.Content as MDisconnect);
                    break;
                case Message.Header.GetListChat:
                    acceptGetListChat(message.Content as List<Chat>);
                    break;
                case Message.Header.CreateChat:
                    acceptCreateChat(message);
                    break;
                case Message.Header.DeleteChat:
                    acceptDeleteChat(message);
                    break;
                case Message.Header.JoinChat:
                    acceptJoinChat(message);
                    break;
                case Message.Header.LeaveChat:
                    acceptLeaveChat(message.Content as MLeaveChat);
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
                    acceptReameUser(message.Content as MRenameUser);
                    break;
            }
        }

        public void acceptRegistration(Message message)
        {
            if (message.Content.GetType() == typeof(FullUser))
                User = user;
            else //если регистрация прошла не успешно
            {
                if (message.Content as string == "Пользователь с таким именем уже существует")
                    eventException?.Invoke(new UserAlreadyExistException("Пользователь с таким именем уже существует"));
                else
                    eventException?.Invoke(new Exception(message.Content as string));
            }
        }

        public void acceptLogin(Message message)
        {
            if (message.Content.GetType() == typeof(FullUser))
                User = user;
            else //если авторизация прошла не успешно
            {
                if (message.Content as string == "Пользователь с таким именем не зарегистрирован")
                    eventException?.Invoke(new WrongLoginException("Пользователь с таким именем не зарегистрирован"));
                else
                    eventException?.Invoke(new WrongPasswordException(message.Content as string));
            }
        }

        public void acceptDisconnect(MDisconnect message)
        {

        }

        public void acceptGetListChat(List<Chat> chats)
        {
            foreach (Chat chat in chats)
                AllChatrooms.Add(chat);
        }

        public void acceptCreateChat(Message message)
        {
            if (message.Content.GetType() == typeof(FullChat)) //Если этот чат создал этот клиент
            {
                FullChat content = message.Content as FullChat;
                ChatManager.add(content.Id, content);
            }
            else
            {
                AllChatrooms.Add(message.Content as Chat);
            }
        }

        public void acceptDeleteChat(Message message)
        {
            if (Chatroom.Equals(message.Content as Chat)) //если активный чат был удален
            {
                ChatManager.remove((message.Content as Chat).Id);
            }
            AllChatrooms.Remove(message.Content as Chat);
        }

        public void acceptJoinChat(Message message)
        {
            if (int.TryParse(message.Content as string, out int result)) //если мы только что вошли в чат
            {
                
            }
            else if (message.Content is MJoinChat) //если в чат зашел другой пользователь
            {
                MJoinChat content = message.Content as MJoinChat;
                FullChat chat = ChatManager.get(content.IdChat); //если мы посещали этот чат
                if (chat != null)
                {
                    if (Chatroom == chat) //если в активный чат вошел пользователь
                    {

                    }
                    ChatManager.userJoin(chat.Id, Online.FirstOrDefault(m => m.Id == content.IdUser));
                }
            }
            
        }

        public void acceptLeaveChat(MLeaveChat message)
        {
            FullChat chat = ChatManager.get(message.IdChat); //если мы посещали этот чат
            if (chat != null)
            {
                if (Chatroom == chat) //если из активного чата вышел пользователь
                {

                }
                ChatManager.userLeave(chat.Id, Online.FirstOrDefault(m => m.Id == message.IdUser));
            }
        }

        public void acceptSendChatMessage(MSendChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.addMessage(message.Message);
        }

        public void acceptEditChatMessage(MEditChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.editMessage(message.Message);
        }

        public void acceptDeleteChatMessage(MDeleteChatMessage message)
        {
            FullChat chat = ChatManager.get(message.IdChat);
            chat.deleteMessage(message.IdMessage);
        }

        public void acceptChangeRights(MChangeRights message)
        {
            ChatManager.changeRights(message.IdChat, message.IdUser, message.Rights);
        }

        public void acceptRenameChat(MRenameChat message)
        {
            ChatManager.rename(message.IdChat, message.Name);
        }

        public void acceptReameUser(MRenameUser message)
        {
            Online.FirstOrDefault(m => m.Id == message.IdUser).Login = message.Name;
        }
    }
}
