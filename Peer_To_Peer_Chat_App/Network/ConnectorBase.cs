using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peer_To_Peer_Chat_App.Network
{
    internal abstract class ConnectorBase : IDisposable
    {
        protected IPEndPoint localEndPoint;
        protected IPEndPoint remoteEndPoint;

        public string LocalEndPointStr { get { return localEndPoint.ToString(); } }
        public string RemoteEndPointStr { get {return remoteEndPoint.ToString(); } }

        public abstract ICommunicator Connect();

        public abstract ICommunicator WaitConnection();

        public abstract void Disconnect();

        public abstract void Dispose();
    }
}
