using Chat.Auth;
using Chat.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Client : TCPClient
    {
        private User user;
        private ThreadedBindingList<string> usersBindingList;
        private ThreadedBindingList<string> messagesBindingList;

        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public ThreadedBindingList<string> UsersBindingList
        {
            get { return usersBindingList; }
            set { usersBindingList = value; }
        }

        public ThreadedBindingList<string> MessagesBindingList
        {
            get { return messagesBindingList; }
            set { messagesBindingList = value; }
        }

        /// <summary>
        /// Метод запуска TCP Клиента
        /// </summary>
        public void run()
        {
            Thread checkConnection = new Thread(new ThreadStart(this.checkData));
            checkConnection.Start();

            Thread checkQuit = new Thread(new ThreadStart(this.checkQuit));
            checkQuit.Start();
        }

        /// <summary>
        /// Поток, проверяющий и считывающий приходящие сообщения от сервера
        /// </summary>
        private void checkData()
        {
            while (!Quit)
            {
                try
                {
                    if (tcpClient.GetStream().DataAvailable)
                    {
                        Thread.Sleep(25);
                        Message message = getMessage();

                        if (message != null)
                        {
                            Thread processData = new Thread(() => this.processData(message));
                            processData.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Поток, проверяющий существует ли соединение с сервером
        /// </summary>
        private void checkQuit()
        {
            while (!Quit)
            {
                try
                {
                    Socket socket = tcpClient.Client;

                    if (socket.Poll(10, SelectMode.SelectRead) && socket.Available == 0)
                    {
                        Quit = true;
                        Console.WriteLine("Сервер отключен");
                    }
                }
                catch { Quit = true; }
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Метод для обработки приходящих серверных сообщений
        /// </summary>
        /// <param name="message"></param>
        private void processData(Message message)
        {
            switch (message.Head)
            {
                case Message.Header.Disconnect:
                    Quit = true;
                    Console.WriteLine("Вы покинули сервер");
                    break;
                case Message.Header.SendMessage:
                    string time = DateTime.Parse(message.MessageList[1]).ToLocalTime().ToShortTimeString();
                    messagesBindingList.Add($"[{time}] {message.MessageList[0]}: {message.MessageList[2]}");
                    break;
                case Message.Header.GetUsers:
                    foreach (string userName in message.MessageList)
                        usersBindingList.Add(userName);
                    break;
                case Message.Header.NewUser:
                    usersBindingList.Add(message.MessageList[0]);
                    break;
            }
        }

        public Client()
        {
            user = new User();
            Quit = false;
        }
    }
}
