using FRC.NetworkTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public class NetworkTableServerClientManager : INetworkTableServerClientManager
    {
        private readonly NetworkTableInstance instance;

        public void StartClient(string server, int port)
        {
            Stop();
            instance.SetNetworkIdentity("OutlineViewer.NET");

            if (int.TryParse(server, out int result))
            {
                instance.StartClientTeam(result, port);
            }
            else
            {
                instance.StartClient(server, port);
            }
        }

        public void StartServer(int port)
        {
            Stop();
            instance.SetNetworkIdentity("OutlineViewer.NET");
            instance.StartServer(port: port);
        }

        public void Stop()
        {
            instance.StopClient();
            instance.StopServer();
            instance.StopDSClient();
            instance.DeleteAllEntries();
        }

        public NetworkTableServerClientManager(NetworkTableInstance instance)
        {
            this.instance = instance;
        }
    }
}
