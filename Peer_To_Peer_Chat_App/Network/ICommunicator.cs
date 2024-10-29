using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peer_To_Peer_Chat_App.Network
{
    public interface ICommunicator : IDisposable
    {
        void Send(string message);

        string Receive();
    }
}
