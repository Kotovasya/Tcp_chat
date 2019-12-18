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
        Client client;

        public Chat(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            userList.DataSource = client.OnlineUsers;
            messages.DataSource = client.Messages;

            client.run();

            Message message = new Message(Message.Header.GetUsers);
            client.sendMessage(message);
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Message message = new Message(Message.Header.Disconnect);
            client.sendMessage(message);
            client.CheckDataThread.Abort();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != null && messageTextBox.Text != string.Empty)
            {
                Message message = new Message(Message.Header.SendMessage);
                message.addData(client.User.Login);
                message.addData(DateTime.UtcNow.ToString());
                message.addData(messageTextBox.Text);
                client.sendMessage(message);
            }
        }

        private void MessageTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendButton_Click(sender, e);
                messageTextBox.Text = string.Empty;
                messageTextBox.Focus();
            }
        }
    }
}
