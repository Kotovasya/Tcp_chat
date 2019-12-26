/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = ChatLibrary.Net.Message;

namespace Client.Views
{
    public partial class CreateChatroom : Form
    {
        Client client;
        public CreateChatroom(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != string.Empty)
            {
                Message message = new Message(Message.Header.CreateCR);
                message.addData(textBox1.Text);
                client.sendMessage(message);
                this.Close();
            }
            else
            {
                MessageBox.Show("Название комнаты не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
*/