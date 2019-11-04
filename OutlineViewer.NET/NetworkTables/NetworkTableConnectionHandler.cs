using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRC.NetworkTables;

namespace OutlineViewer.NET.NetworkTables
{
    public class NetworkTableConnectionHandler : INetworkTableConnectionHandler
    {
        private readonly NetworkTableInstance instance;

        public int NumConnections => instance.GetConnections().Length;

        public NetworkMode NetworkMode => instance.GetNetworkMode();

        public event EventHandler ConnectionChanged;

        public NetworkTableConnectionHandler(NetworkTableInstance instance)
        {
            this.instance = instance;

            instance.AddConnectionListener((in ConnectionNotification notification) =>
            {
                ConnectionChanged?.Invoke(this, EventArgs.Empty);
            }, false);
        }
    }
}
