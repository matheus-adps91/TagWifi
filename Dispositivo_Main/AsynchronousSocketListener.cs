using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using System.IO;

namespace Dispositivo_Main
{
    // Modela o socket para receber dados
    class StateObject
    {
        // Socket
        public Socket workSocket = null;
        // Tamanho do buffer
        public const int BufferSize = 1024;
        // Buffer de dados para dados de entrada
        public byte[] buffer = new byte[BufferSize];
        // String para receber os dados enviados pelo cliente
        public StringBuilder sb = new StringBuilder();
    }

    class AsynchronousSocketListener
    {
        private static Form1 tela;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private static string StatusService = string.Empty;

        public const string btnBeg = "10/"; // botão iniciar
        public const string btnEnd = "01/"; // botão finalizar
        public const string btnPause = "11/"; // botão pausar

        public const string msgBegin = "Serviço Iniciado"; // controle de estado
        public const string msgEnd = "Serviço Finalizado"; // controle de estado
        public const string msgPause = "Serviço Pausado"; // controle de estado
        public const string msgResumeBeg = "Serviço não pode ser iniciado";
        public const string msgResumeEnd = "Serviço não pode ser finalizado";
        public const string msgResumePau = "Serviço Reiniciado";
        public const string msgNotBeg = "Serviço não iniciado";


        public AsynchronousSocketListener(Form1 form)
        {
            tela = form;
            // Iniciar a thread com o método que inicia o listener
            new Thread(StartListener).Start();
        }

        static void StartListener()
        {

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 443);

            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            // Vincula o socket com o endpoint local  e o listenning para conexão de entrada
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    // Configura 
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                tela.Log("StartListening.Error: " + e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.EndsWith("$"))
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    ButtonTag(content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                tela.Log("SendCallback.Error: " + e.ToString());
            }
        } // Fim do mÃ©todo SendCallBack()

        public static void ButtonTag(string msg)
        {
            try
            {
                string[] tagInfo = msg.Substring(1).Split(';');

                // tagInfo[0] = IdTag / tagInfo[1] = botÃ£o pressionado (10/01/11)
                string[] tagIdButton = { tagInfo[0], string.Concat(tagInfo[2], tagInfo[3]) };

                string eventDescription = string.Concat(tagIdButton[0], "/", VerifyPressedButton(tagIdButton[1]));

                tela.Log(eventDescription);
            }
            catch (Exception e)
            {
                tela.Log("ButtonTag.Error: " + e.Message);
            }
        } // Fim do mÃ©todo  ButtonTag()

        private static string VerifyPressedButton(string tagIdButton)
        {
            // LÃ³gica para identificar qual evento ocorreu
            switch (tagIdButton)
            {
                case "10":
                    if (StatusService != msgBegin && StatusService != msgPause)
                    {
                        StatusService = msgBegin;
                        return btnBeg + msgBegin;
                    }
                    else
                    {
                        return btnBeg + msgResumeBeg;
                    }
                case "01":
                    if (StatusService == "")
                    {
                        return btnEnd + msgNotBeg;
                    }
                    if (StatusService != msgEnd && StatusService != msgPause)
                    {
                        StatusService = msgEnd;
                        return btnEnd + msgEnd;
                    }
                    else
                    {
                        return btnEnd + msgResumeEnd;
                    }
                case "11":
                    if (StatusService == "")
                    {
                        return btnPause + msgNotBeg;
                    }
                    if (StatusService != msgPause)
                    {
                        if (StatusService != msgEnd)
                        {
                            StatusService = msgPause;
                            return btnPause + msgPause;
                        }
                        else
                        {
                            return btnPause + msgNotBeg;
                        }
                    }
                    else
                    {
                        StatusService = msgBegin;
                        return btnPause + msgResumePau;
                    }
            }
            return " /error";
        }

    }

}

