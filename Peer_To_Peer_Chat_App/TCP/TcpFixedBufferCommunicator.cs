using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Peer_To_Peer_Chat_App.Network;
using Peer_To_Peer_Chat_App.Network.Exceptions;

namespace Peer_To_Peer_Chat_App.TCP
{
    internal class TcpFixedBufferCommunicator : ICommunicator
    {
        private Socket socket;
        private byte[] buffer;

        public TcpFixedBufferCommunicator(Socket socket,int bufferSize = 1024,int reseiveTimeout = 100)
        {
            this.socket = socket;
            this.socket.ReceiveTimeout = reseiveTimeout;
            buffer = new byte[bufferSize];
        }

        public void Send(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            if (messageBytes.Length > buffer.Length)
            {
                throw new InvalidOperationException($"too large message, only {buffer.Length} bytes allowed");
            }
            socket.Send(messageBytes);
        }

        public string Receive()
        {
            try
            {
                int bytesRead = socket.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (message == "\0DISCONNECT")
                {
                    throw new SocketDisconnectedException();
                }
                return message;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    return null;
                }
                throw ex;
            }
        }

        public void Dispose()
        {
            socket.Close();
        }



    }
}
