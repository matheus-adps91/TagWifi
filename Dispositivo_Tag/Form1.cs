using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dispositivo_Tag
{  
    public partial class Form1 : Form
    {
        public Tag tag =  new Tag();
        public Network senderSocket;

        public Form1()
        {
            InitializeComponent();            
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            tag.button = "1;0;";
            string tagData = tag.BuildInfo(tag);
            senderSocket = new Network();
            
            senderSocket.ConnectToServer();
            senderSocket.ConvertAndSend(tagData);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            tag.button = "0;1;";
            string tagData = tag.BuildInfo(tag);
            senderSocket = new Network();

            senderSocket.ConnectToServer();
            senderSocket.ConvertAndSend(tagData);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            tag.button = "1;1;";
            string tagData = tag.BuildInfo(tag);
            senderSocket = new Network();

            senderSocket.ConnectToServer();
            senderSocket.ConvertAndSend(tagData);
        }
    }
}
