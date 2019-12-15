using Chat.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = Chat.Net.Message;

namespace Client.Views
{
    public partial class Chat : Form
    {
        protected Client client;
        private Thread checkServer;
        private ThreadedBindingList<string> usersList;
        private ThreadedBindingList<string> messagesList;

        public Chat(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            //Когда мы закрываем форму, нужно закрыть и все подключения
            this.FormClosing += new FormClosingEventHandler(Chat_Closing);

            usersList = new ThreadedBindingList<string>();
            client.UsersBindingList = usersList;
            userList.DataSource = usersList;

            messagesList = new ThreadedBindingList<string>();
            client.MessagesBindingList = messagesList;
            messages.DataSource = messagesList;

            checkServer = new Thread(new ThreadStart(this.getServer));
            checkServer.Start();

            Message message = new Message(Message.Header.GetUsers);
            client.sendMessage(message);
        }
        
        /// <summary>
        /// Поток, проверяющий есть ли подключение к серверу
        /// </summary>
        private void getServer()
        {
            while (!client.Quit)
            {
                Thread.Sleep(2000);
            }

            if (client.Quit)
            {
                Console.WriteLine("Сервер оборвал подключение");
                    MessageBox.Show(
                    "Сервер оборвал подключение",
                    "Связь с сервером потеряна",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning
                    );
            }
        }

        private void Chat_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Client exiting");
            checkServer.Abort();
            client.Quit = true;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Message message = new Message(Message.Header.SendMessage);
            message.addData(client.User.Login);
            message.addData(DateTime.UtcNow.ToString());
            message.addData(messageTextBox.Text);
            client.sendMessage(message);
        }

        private void MessageTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendButton_Click(sender, e);
            }
        }
    }
}
