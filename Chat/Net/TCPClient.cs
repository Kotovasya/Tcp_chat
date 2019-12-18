using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net
{
    public abstract class TCPClient
    {
        protected int port;
        protected Boolean connected;
        protected TcpClient tcpClient;
        protected IPAddress ip;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public Boolean Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        public IPAddress IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public TCPClient()
        {
            ip = null;
            tcpClient = null;
        }

        /// <summary>
        /// Подключение к установленному TCP серверу
        /// </summary>
        public void connect()
        {
            Console.WriteLine($"Попытка подключения к {IP.ToString()}:{Port}...");
            tcpClient = new TcpClient(IP.ToString(), Port);
            Connected = true;
        }

        /// <summary>
        /// Отправляет серверу уведомление об отключении клиента от него
        /// </summary>
        public void disconnect()
        {
            sendMessage(new Message(Message.Header.Disconnect));
            connected = false;
        }

        /// <summary>
        /// Получение сообщения от TCP сервера
        /// </summary>
        /// <returns></returns>
        public Message getMessage()
        {
            if (Connected)
            {
                try
                {
                    Console.WriteLine("TCP Client получил сообщение, производится обработка...");
                    NetworkStream stream = tcpClient.GetStream();
                    IFormatter formatter = new BinaryFormatter();
                    Message message = (Message)formatter.Deserialize(stream);
                    return message;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("## При обработке сообщения произошло исключение: " + ex.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Отправление сообщения на TCP сервер
        /// </summary>
        /// <param name="message">Отправляемое сообщение</param>
        public void sendMessage(Message message)
        {
            if (Connected)
            {
                try
                {
                    Console.WriteLine("Отправка сообщения на TCP Server...");
                    NetworkStream stream = tcpClient.GetStream();
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("## При отправки сообщения произошло исключение: " + ex.Message);
                }
            }
        }
    }
}
