using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Net
{
    /// <summary>
    /// TCP Server
    /// </summary>

    [Serializable]
    public abstract class TCPServer
    {
        protected volatile TcpClient tcpClient;
        protected volatile TcpListener tcpListener;
        protected volatile Boolean running;
        protected int port;
        protected Thread checkDataThread;
        protected Thread checkListenerThread;

        public bool Running
        {
            get { return running; }
            set { running = value; }
        }

        /// <summary>
        /// Запускает TCP сервер
        /// </summary>
        /// <param name="port">Порт для открытия сервера</param>
        public void startServer(int port)
        {
            this.Running = false;
            this.port = port;

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            try
            {
                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                this.Running = true;
                Console.WriteLine("Сервер запущен");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Сервер не запустился. Исключение: " + ex.Message);
            }
        }

        /// <summary>
        /// Останавливает TCP сервер
        /// </summary>
        public void stopServer()
        {
            this.Running = false;
            tcpListener.Stop();
            Console.WriteLine("Сервер остановлен администратором");
        }

        /// <summary>
        /// Получает сообщение от клиента по сокету
        /// </summary>
        /// <param name="socket">Прослушиваемый клиентский сокет</param>
        /// <returns></returns>
        public Message getMessage(Socket socket)
        {
            Console.WriteLine("TCP Server получил сообщение. Производится обработка...");
            try
            {
                NetworkStream stream = new NetworkStream(socket);
                IFormatter formatter = new BinaryFormatter();
                Message message = (Message)formatter.Deserialize(stream);
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("## При обработке сообщения произошло исключение: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Отправляет сообщение клиенту
        /// </summary>
        /// <param name="message">Отправляемое сообщение</param>
        /// <param name="socket">Клиентский сокет, которому отправляем</param>
        public void sendMessage(Message message, Socket socket)
        {
            try
            {
                NetworkStream stream = new NetworkStream(socket);
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
