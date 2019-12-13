using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class ServerGUI : Form
    {
        TextWriter writer = null;
        private Server server;

        public ServerGUI()
        {
            InitializeComponent();
        }

        private void ServerGUI_Load(object sender, EventArgs e)
        {
            //Устанавливаем textbox как консоль
            writer = new TextboxStreamWriter(txtConsole);
            Console.SetOut(writer);
        }

        /// <summary>
        /// Кнопка включения/отключения сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text == "Start server")
            {
                startButton.Text = "Stop server";
                server = new Server();
                server.startServer(Convert.ToInt32(portBox.Text));

                if (server.Running)
                    server.run();
                else
                    MessageBox.Show("Сбой сервера",
                     "Сервер уже запущен",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
            }
            else
            {
                startButton.Text = "Start server";
                server.stopServer();
            }
        }
    }
}
