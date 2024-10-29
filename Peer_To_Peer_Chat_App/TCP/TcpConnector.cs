using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Peer_To_Peer_Chat_App.Network;

namespace Peer_To_Peer_Chat_App.TCP
{
    internal class TcpConnector : ConnectorBase
    {
        private Socket socket;
        private bool isDisconnected = false;

        public TcpConnector( string localIPStr, int localPort,
            string remoteIpStr,int remotePort )
        {
            socket = new Socket( AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp );
            localEndPoint = new IPEndPoint(IPAddress.Parse(localIPStr)
                ,localPort);
            socket.Bind( localEndPoint );

            socket.Listen(1);
        }

        public override ICommunicator WaitConnection()
        {
            Socket clientSocket = socket.Accept();
            byte[] buffer = new byte[1024];
            int bytesRead = clientSocket.Receive(buffer);
            string connectMessage = Encoding.UTF8.GetString(buffer,0,
                bytesRead);
            string[] connectMessageTokens = connectMessage.Split(':');

            if (connectMessageTokens[0] != "CONNECT")
            {
                throw new InvalidOperationException("no connect message");
            }

            remoteEndPoint = new IPEndPoint(IPAddress.Parse(connectMessageTokens[1]),
                Convert.ToInt32(connectMessageTokens[2]));

            string acceptConnectMessage = "ACCEPT_CONNECT";
            clientSocket.Send(Encoding.UTF8.GetBytes(acceptConnectMessage));
            return new TcpFixedBufferCommunicator(clientSocket);
        }

        public override ICommunicator Connect()
        {
            socket.Connect(remoteEndPoint);

            string connectMessage = $"CONNECT:{LocalEndPointStr}";
            socket.Send(Encoding.UTF8.GetBytes(connectMessage));

            byte[] buffer = new byte[1024];
            int bytesRead = socket.Receive(buffer);
            string acceptConnectMessage = Encoding.UTF8.GetString(buffer,0,
                bytesRead);

            if (acceptConnectMessage != "ACCEPT_CONNECT")
            {
                throw new InvalidOperationException("no accept connect message");
            }
            return new TcpFixedBufferCommunicator(socket);
        }

        public override void Disconnect()
        {
            if (isDisconnected )
            {
                return;
            }

            string disconnectMessage = "\0DISCONNECT";
            socket .Send(Encoding.UTF8.GetBytes(disconnectMessage));

            socket.Close();
            isDisconnected = true;
        }

        public override void Dispose()
        {
            Disconnect();
        }
    }
}
