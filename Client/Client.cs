/*using ChatLibrary.Net;
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

namespace Client
{
    public class Client : TCPClient
    {
        private ThreadedBindingList<string> chatUsers;
        private ThreadedBindingList<string> messages;
        private ThreadedBindingList<string> chatrooms;
        private FullUser user;
        private Thread checkConnection;
        private Thread checkDataThread;
        private bool haveCR;
        private FullChat chatroom;
        public delegate void changeUI(bool isJoin);
        public delegate void exception(Exception ex);
        public event changeUI eventChangeUI;
        public event exception eventException;


        public Client(object state)
        {
            Messages = new ThreadedBindingList<string>();
            ChatUsers = new ThreadedBindingList<string>();
            Chatrooms = new ThreadedBindingList<string>();
        }

        public FullUser User { get => user; set => user = value; }
        public ThreadedBindingList<string> Messages { get => messages; set => messages = value; }
        public ThreadedBindingList<string> ChatUsers { get => chatUsers; set => chatUsers = value; }
        public Thread CheckConnection { get => checkConnection; set => checkConnection = value; }
        public Thread CheckDataThread { get => checkDataThread; set => checkDataThread = value; }
        public bool HaveCR { get => haveCR; set => haveCR = value; }
        public FullChat Chatroom { get => chatroom; set => chatroom = value; }
        public ThreadedBindingList<string> Chatrooms { get => chatrooms; set => chatrooms = value; }

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
                    acceptGetListChat(message.Content as FullChat);
                    break;
                case Message.Header.CreateChat:
                    acceptCreateChat(message.Content as MCreateChat);
                    break;
                case Message.Header.DeleteChat:
                    acceptDeleteChat(message);
                    break;
                case Message.Header.JoinChat:
                    acceptJoinChat(message.Content as MJoinChat);
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
                case Message.Header.ChangePassword:
                    acceptChangePassword(message.Content as MChangePassword);
                    break;
            }
        }

        public void acceptRegistration(Message message)
        {
            if (message.Content.GetType() == typeof(FullUser))
                User = user;
            else //если регистрация прошла не успешно
            { }
        }

        public void acceptLogin(Message message)
        {
            if (message.Content.GetType() == typeof(FullUser))
                User = user;
            else //если авторизация прошла не успешно
            { }
        }

        public void acceptDisconnect(MDisconnect message)
        {

        }

        public void acceptGetListChat(FullChat chat)
        {
            List<Chat> chats = new List<Chat>();
            ChatManager.Chats.ForEach(item => chats.Add(item.ToChat()));
            sendMessage(new Message(Message.Header.GetListChat, new MGetListChat(chats)), client.Client);
        }

        public void acceptCreateChat(MCreateChat message)
        {
            FullChat newChat = ChatManager.newChat(message.Name, OnlineManager.getUser(client).UserInfo);
            sendMessage(new Message(Message.Header.CreateChat, newChat), client.Client);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.CreateChat, newChat.ToChat()), user.Key.Client);
            }
        }

        public void acceptDeleteChat(Message message)
        {
            ChatManager.removeChat((message.Content as MDeleteChat).IdChat);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                {
                    sendMessage(message, user.Key.Client);
                }
            }
        }

        public void acceptJoinChat(MJoinChat message)
        {
            ChatManager.userJoin(message.IdChat, OnlineManager.getUser(client).UserInfo);
            sendMessage(new Message(Message.Header.JoinChat, ChatManager.getChat(message.IdChat)), client.Client);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.JoinChat, message), user.Key.Client);
            }
        }

        public void acceptLeaveChat(MLeaveChat message)
        {
            ChatManager.userLeave(message.IdChat, OnlineManager.getUser(client).UserInfo);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    if (user.Key.Client != client.Client)
                        sendMessage(new Message(Message.Header.LeaveChat, message), user.Key.Client);
            }
        }

        public void acceptSendChatMessage(MSendChatMessage message)
        {
            ChatManager.getChat(message.IdChat).addMessage(message.Message);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.SendChatMessage, message), user.Key.Client);
            }
        }

        public void acceptEditChatMessage(MEditChatMessage message)
        {
            ChatManager.getChat(message.IdChat).editMessage(message.Message);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.EditChatMessage, message), user.Key.Client);
            }
        }

        public void acceptDeleteChatMessage(MDeleteChatMessage message)
        {
            ChatManager.getChat(message.IdChat).deleteMessage(message.IdMessage);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.DeleteChatMessage, message), user.Key.Client);
            }
        }

        public void acceptChangeRights(MChangeRights message)
        {
            ChatManager.changeRights(message.IdChat, message.IdUser, message.Rights);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.ChangeRights, message), user.Key.Client);
            }
        }

        public void acceptRenameChat(MRenameChat message)
        {
            ChatManager.rename(message.IdChat, message.Name);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
            {
                if (user.Value != null)
                    sendMessage(new Message(Message.Header.RenameChat, message), user.Key.Client);
            }
        }

        public void acceptReameUser(MRenameUser message)
        {
            BaseManager.renameUser(message.IdUser, message.Name);
            OnlineManager.changeUsername(client, message.Name);
            foreach (KeyValuePair<TcpClient, FullUser> user in OnlineManager.UserList)
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
*/