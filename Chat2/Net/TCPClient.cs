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
        /// Установить подключаемый TCP сервер
        /// </summary>
        /// <param name="ip">IP сервера</param>
        /// <param name="port">Порт сервера</param>
        public void setServer(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
        }

        /// <summary>
        /// Подключение к установленному TCP серверу
        /// </summary>
        public void connect()
        {
            try
            {
                Console.WriteLine($"Попытка подключения к {IP.ToString()}:{Port}...");
                tcpClient = new TcpClient(IP.ToString(), Port);
                Connected = true;
                Console.WriteLine("Успшено подключено");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("## TCP Server отказал в подключении, исключение:" + ex.Message);
                Connected = false;
            }
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
                    Console.WriteLine("Тип полученного сообщения: " + message.Head);
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
                    Console.WriteLine($"Сообщение типа {message.Head} успешно отправлено");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("## При отправки сообщения произошло исключение: " + ex.Message);
                }
            }
        }
    }
}
