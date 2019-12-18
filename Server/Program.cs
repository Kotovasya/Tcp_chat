using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            startServer();
        }

        private static void startServer()
        {
            Server server = new Server();
            server.startServer(2020);

            if (server.Running)
                server.run();
            else
                Console.WriteLine("Не удалось запустить сервер.");

            while (server.Running && Console.ReadLine() != "stop")
            {

            }

            server.stopServer();

            Console.WriteLine("Сервер был остановлен, запустить его снова? (Y)");
            if (Console.ReadLine().ToLower() == "y")
                startServer();
        }
    }
}
