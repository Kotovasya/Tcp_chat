using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class ChatMessage
    {
        private int id;
        private int idUser;
        private string text;
        private List<Attachment> attachments;
        private DateTime dateTime;

        public ChatMessage(int id, List<Attachment> attachments, string text, DateTime dateTime, int idUser)
        {
            Id = id;
            Attachments = attachments;
            Text = text;
            DateTime = dateTime;
            IdUser = idUser;
        }

        public delegate void delegateEdit(string text);
        public event delegateEdit messageEdited;

        public delegate void delegateDelete();
        public event delegateDelete messageDeleted;

        public int Id { get => id; set => id = value; }
        public List<Attachment> Attachments { get => attachments; set => attachments = value; }
        public string Text { get => text; set => text = value; }
        public DateTime DateTime { get => dateTime; set => dateTime = value; }
        public int IdUser { get => idUser; set => idUser = value; }

        public void edit(string text)
        {
            this.Text = text;
            messageEdited?.Invoke(text);
        }

        public void delete()
        {
            messageDeleted?.Invoke();
        }
    }
}
