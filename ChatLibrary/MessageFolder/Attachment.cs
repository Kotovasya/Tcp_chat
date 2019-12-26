using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary.MessageFolder
{
    public class Attachment
    {
        private object content;

        public Attachment(FileInfo file)
        {
            this.Content = file;
        }

        public Attachment(Image image)
        {
            this.Content = image;
        }

        public object Content { get => content; set => content = value; }
    }
}
