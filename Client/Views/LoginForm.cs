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
        private Client client;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                Message message = new Message(Message.Header.Registration);
                message.addData(userNameTextBox.Text);
                message.addData(passwordTextBox.Text);
                client.sendMessage(message);

                Message reply = client.getMessage();

                if (reply == null)
                    MessageBox.Show("Сервер не дал ответа", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (reply.MessageList.First() == "success")
                    ShowChat();
                else
                    MessageBox.Show(reply.MessageList.First(), "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                Message message = new Message(Message.Header.Login);
                message.addData(userNameTextBox.Text);
                message.addData(passwordTextBox.Text);
                client.sendMessage(message);

                Message reply = client.getMessage();

                if (reply == null)
                    MessageBox.Show("Сервер не дал ответа", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (reply.MessageList.First() == "success")
                    ShowChat();
                else
                    MessageBox.Show(reply.MessageList.First(), "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowChat()
        {
            client.User = new User(userNameTextBox.Text, passwordTextBox.Text);
            var frm = new Views.Chat(client);
            frm.Location = this.Location;
            frm.StartPosition = FormStartPosition.Manual;
            frm.FormClosing += delegate { this.Show(); };
            frm.Show();
            this.Hide();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new Client();
                client.setServer(IPAddress.Parse("127.0.0.1"), 2020);
                client.connect();
                client.run();
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(
                    ex.Message,
                    "Ошибка соединения",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                    ConnectToServer();
                else
                    Application.Exit();
            }
        }
    }
}
