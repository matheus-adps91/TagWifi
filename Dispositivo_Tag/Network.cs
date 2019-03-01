using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace Dispositivo_Tag
{
    public class Network
    {
        public const string strEnderecoIp = "127.0.0.1";
        public const int port = 443;
        public TcpClient tcpClient;
        public NetworkStream stream;
        public Socket socket;

        public void ConnectToServer()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(strEnderecoIp, port);
        }

        public void ConvertAndSend(string tagData)
        {
            int byteCount = Encoding.ASCII.GetByteCount(tagData);
            byte[] sendData = new byte[byteCount];
            sendData = Encoding.ASCII.GetBytes(tagData);
            stream = tcpClient.GetStream();
            stream.Write(sendData, 0, sendData.Length);
            stream.Close();
            tcpClient.Close();
        }

    }
}
