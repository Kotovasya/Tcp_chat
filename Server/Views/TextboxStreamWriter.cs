using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    class TextboxStreamWriter : TextWriter
    {
        TextBox output = new TextBox();

        public TextboxStreamWriter(TextBox output)
        {
            this.output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            try
            {
                output.Invoke((MethodInvoker)(() =>
                {
                   output.AppendText(value.ToString());
               }));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
