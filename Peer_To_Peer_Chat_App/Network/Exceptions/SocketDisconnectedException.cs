using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peer_To_Peer_Chat_App.Network.Exceptions
{
    internal class SocketDisconnectedException : ApplicationException
    {
        public SocketDisconnectedException() : base("remote socket was disconnected") { }
    }
}
