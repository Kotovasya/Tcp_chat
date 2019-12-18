using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat.Auth;
using Chat.Net;
using Message = Chat.Net.Message;

namespace Client
{
    public partial class LoginForm : Form
    {
        Client client;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            setConnect(sender);
            if (client.Connected)
            {
                Message message = new Message(Message.Header.Login);
                message.addData(userNameTextBox.Text);
                message.addData(passwordTextBox.Text);
                client.sendMessage(message);

                Message reply = client.getMessage();

                if (reply == null)
                {
                    showError(sender);
                }
                else if (reply.MessageList[0] == "success")
                {
                    showChat();
                }
                else
                {
                    MessageBox.Show(reply.MessageList[0], "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            setConnect(sender);
            if (client.Connected)
            {
                Message message = new Message(Message.Header.Registration);
                message.addData(userNameTextBox.Text);
                message.addData(passwordTextBox.Text);
                client.sendMessage(message);

                Message reply = client.getMessage();

                if (reply == null)
                {
                    showError(sender);
                }
                else if (reply.MessageList[0] == "success")
                {
                    showChat();
                }
                else
                {
                    MessageBox.Show(reply.MessageList[0], "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Установить подключение к серверу
        /// </summary>
        protected void setConnect(object sender)
        {
            if (client == null)
            {
                try
                {
                    client = new Client();
                    client.IP = IPAddress.Parse("127.0.0.1");
                    client.Port = 2020;
                    client.connect();
                }
                catch { showError(sender); }
            }
        }

        protected void showChat()
        {
            client.User = new User(userNameTextBox.Text, passwordTextBox.Text);
            var frm = new Views.Chat(client);
            frm.Location = this.Location;
            frm.StartPosition = FormStartPosition.Manual;
            frm.FormClosing += delegate {
                client = null;
                this.Show();
            };
            frm.Show();
            this.Hide();
        }

        protected void showError(object sender)
        {
            DialogResult result = MessageBox.Show("Ошибка подключения", "Не удалось соединиться с сервером, повторить попытку?", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (result == DialogResult.Retry)
            {
                if (sender.Equals(LoginButton))
                {
                    LoginButton_Click(sender, new EventArgs());
                }
                else
                {
                    RegisterButton_Click(sender, new EventArgs());
                }
            }
        }
    }
}
