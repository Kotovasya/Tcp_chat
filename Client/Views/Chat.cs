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
using Message = ChatLibrary.Net.Message;

namespace Client.Views
{
    public partial class Chat : Form
    {
        Client client;
        Action<bool> actionListChanged;

        public Chat(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            nameList.DataSource = client.AllChatrooms;
            messages.DataSource = client.Messages;

            client.run();

            actionListChanged += changeList;
            client.eventChangeUI += invokeIfNeeded;
            client.eventException += clientException;

            Message message = new Message(Message.Header.GetCR);
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

        //Нужно сделать доступ в этом потоке к элементам формы
        private void invokeIfNeeded(bool isJoin)
        {
            if (nameList.InvokeRequired)
                nameList.Invoke(actionListChanged, isJoin);
            else
                actionListChanged(isJoin);
        }

        private void changeList(bool isJoin)
        {
            if (isJoin)
            {
                nameList.DataSource = client.ChatUsers;
                nameList.SelectionMode = SelectionMode.None;
                crButton.Click -= CreateCR_Click;
                crButton.Click += LeaveCR_Click;
                crButton.Text = "Покинуть комнату";
                messages.Enabled = true;
                messageTextBox.Enabled = true;
                sendButton.Enabled = true;
            }
            else
            {
                nameList.DataSource = client.AllChatrooms;
                nameList.SelectionMode = SelectionMode.One;
                nameList.SelectedIndex = -1;
                crButton.Click -= LeaveCR_Click;
                crButton.Click += CreateCR_Click;
                crButton.Text = "Создать комнату";
                messages.Enabled = false;
                messageTextBox.Enabled = false;
                sendButton.Enabled = false;
            }
        }

        private void CreateCR_Click(object sender, EventArgs e)
        {
            var frm = new CreateChatroom(client);
            frm.Location = this.Location;
            frm.StartPosition = FormStartPosition.Manual;
            frm.FormClosing += delegate { this.Show(); };
            frm.Show();
        }

        private void LeaveCR_Click(object sender, EventArgs e)
        {
            Message message = new Message(Message.Header.LeaveCR);
            message.addData(client.Chatroom.Id.ToString());
            message.addData(client.User.Login);
            client.sendMessage(message);
        }

        private void JoinCRButton_Click(object sender, EventArgs e)
        {
            if (nameList.SelectedIndex != -1)
            {
                Message message = new Message(Message.Header.JoinCR);
                message.addData(nameList.SelectedItem.ToString());
                client.sendMessage(message);
            }
        }

        private void NameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nameList.SelectedIndex != -1)
                joinCRButton.Visible = true;
            else
                joinCRButton.Visible = false;
        }

        private void clientException(Exception ex)
        {
            MessageBox.Show("Соединение разорвано. Сервер не дал ответа", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
