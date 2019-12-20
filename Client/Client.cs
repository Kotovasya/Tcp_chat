using Chat.Auth;
using Chat.ChatRoom;
using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Client : TCPClient
    {
        private ThreadedBindingList<string> chatUsers;
        private ThreadedBindingList<string> messages;
        private ThreadedBindingList<string> chatrooms;
        private User user;
        private Thread checkConnection;
        private Thread checkDataThread;
        private bool haveCR;
        private Chatroom chatroom;
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

        public User User { get => user; set => user = value; }
        public ThreadedBindingList<string> Messages { get => messages; set => messages = value; }
        public ThreadedBindingList<string> ChatUsers { get => chatUsers; set => chatUsers = value; }
        public Thread CheckConnection { get => checkConnection; set => checkConnection = value; }
        public Thread CheckDataThread { get => checkDataThread; set => checkDataThread = value; }
        public bool HaveCR { get => haveCR; set => haveCR = value; }
        public Chatroom Chatroom { get => chatroom; set => chatroom = value; }
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
                catch(SocketException ex)
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
                if (socket.Poll(10, SelectMode.SelectRead) && socket.Available == 0)
                    Connected = false;

                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Поток, обрабатывающий сообщения и добавляющий их в BindingList
        /// </summary>
        /// <param name="message"></param>
        public void processData(Message message)
        {
            switch (message.Head)
            {
                case Message.Header.GetCR:
                    foreach (string nameCR in message.MessageList)
                        Chatrooms.Add(nameCR);
                    break;
                case Message.Header.JoinCR:
                    if (Chatroom != null)
                    {
                        Messages.Add($"Пользователь {message.MessageList[1]} присоединился к чату");
                        ChatUsers.Add(message.MessageList[1]);
                    }
                    else
                    {
                        Chatroom = new Chatroom(int.Parse(message.MessageList[0]), message.MessageList[2]);
                        Message reply = new Message(Message.Header.GetUsers);
                        reply.addData(Chatroom.Id.ToString());
                        sendMessage(reply);
                        eventChangeUI?.Invoke(true);
                    }
                    break;
                case Message.Header.CreateCR:
                    if (message.MessageList[0] == "success")
                    {
                        Chatroom = new Chatroom(int.Parse(message.MessageList[1]), message.MessageList[2]);
                        Message reply = new Message(Message.Header.GetUsers);
                        reply.addData(Chatroom.Id.ToString());
                        sendMessage(reply);
                        Chatrooms.Add(message.MessageList[2]);
                        eventChangeUI?.Invoke(true);
                    }
                    else
                        Chatrooms.Add(message.MessageList[0]);
                    break;
                case Message.Header.LeaveCR:
                    if (message.MessageList[0] == "success")
                    {
                        Chatroom = null;
                        eventChangeUI?.Invoke(false);
                        Messages.Clear();
                        ChatUsers.Clear();
                    }
                    else
                    {
                        ChatUsers.Remove(message.MessageList[1]);
                        Messages.Add($"Пользователь {message.MessageList[1]} покинул чат");
                    }
                    break;
                case Message.Header.GetUsers:
                    foreach (string userName in message.MessageList)
                        ChatUsers.Add(userName);
                    break;
                case Message.Header.NewUser:
                    ChatUsers.Add(message.MessageList[0]);
                    break;
                case Message.Header.SendMessage:
                    string time = DateTime.Parse(message.MessageList[1]).ToLongTimeString();
                    Messages.Add($"[{time}] {message.MessageList[0]}: {message.MessageList[2]}");
                    break;
            }
        }
    }
}
