using Chat.Auth;
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
        private ThreadedBindingList<string> onlineUsers;
        private ThreadedBindingList<string> messages;
        private User user;
        private Thread checkConnection;
        private Thread checkDataThread;

        public Client()
        {
            Messages = new ThreadedBindingList<string>();
            OnlineUsers = new ThreadedBindingList<string>();
        }

        public User User { get => user; set => user = value; }
        public ThreadedBindingList<string> Messages { get => messages; set => messages = value; }
        public ThreadedBindingList<string> OnlineUsers { get => onlineUsers; set => onlineUsers = value; }
        public Thread CheckConnection { get => checkConnection; set => checkConnection = value; }
        public Thread CheckDataThread { get => checkDataThread; set => checkDataThread = value; }

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
            Thread.Sleep(5);
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
                case Message.Header.GetUsers:
                    foreach (string userName in message.MessageList)
                        OnlineUsers.Add(userName);
                    break;
                case Message.Header.NewUser:
                    OnlineUsers.Add(message.MessageList[0]);
                    break;
                case Message.Header.SendMessage:
                    string time = DateTime.Parse(message.MessageList[1]).ToLongTimeString();
                    Messages.Add($"[{time}] {message.MessageList[0]}: {message.MessageList[2]}");
                    break;
            }
        }
    }
}
