using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public interface INetworkTableServerClientManager
    {
        void StartServer(int port);
        void StartClient(string server, int port);
        void Stop();
    }
}
