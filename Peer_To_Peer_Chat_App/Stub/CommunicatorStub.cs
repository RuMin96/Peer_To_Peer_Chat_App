using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Peer_To_Peer_Chat_App.Network;

namespace Peer_To_Peer_Chat_App.Stub
{
    internal class CommunicatorStub : ICommunicator
    {
        private Random random = new Random();

        public string Reseive()
        {
            Thread.Sleep(random.Next(5000, 15000));
            return $"сообщение от CommunicatorStub, время {DateTime.UtcNow}";
        }

        public void Send(string message)
        {

        }

        public void Dispose()
        {

        }

        void ICommunicator.Send(string message)
        {
            throw new NotImplementedException();
        }

        string ICommunicator.Receive()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
