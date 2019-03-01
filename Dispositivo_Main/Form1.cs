using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dispositivo_Main
{
    public partial class Form1 : Form
    {
        // declaraÃ§Ã£o do delegado
        private delegate void addReport(string text);
        // variÃ¡vel do tipo do delegado
        private addReport report;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            report = new addReport(sendLog);
            new AsynchronousSocketListener(this);
        }

        // mÃ©todo "encapsulado" pelo delegate
        private void sendLog(string text)
        {
            string[] msg = text.Split('/');
            StringBuilder description = new StringBuilder(DateTime.Now.ToLongTimeString() + " | Posto ");

            switch (msg[1])
            {
                case "10":
                    if (msg[2].Contains(AsynchronousSocketListener.msgBegin))
                    {
                        lblP1.BackColor = Color.Blue;
                        lblP2.BackColor = Color.White;
                    }
                    description.Append(msg[0] + " - " + msg[2]);
                    break;
                case "01":
                    if (msg[2].Contains(AsynchronousSocketListener.msgEnd))
                    {
                        lblP1.BackColor = Color.White;
                        lblP2.BackColor = Color.Yellow;
                    }
                    description.Append(msg[0] + " - " + msg[2]);
                    break;
                case "11":
                    if (msg[2].Contains(AsynchronousSocketListener.msgPause))
                    {
                        lblP2.BackColor = Color.Yellow;
                    }
                    else if (msg[2].Contains(AsynchronousSocketListener.msgResumePau))
                    {
                        lblP2.BackColor = Color.White;
                    }
                    description.Append(msg[0] + " - " + msg[2]);
                    break;
                case " ":
                    description.Append("Erro");
                    break;
            }

            // Menmsagem que serÃ¡ adicionado no list box do form
            listBox1.Items.Add(description);
        }

        public void Log(string msg)
        {
            Invoke(report, msg);
        }
    }

}
