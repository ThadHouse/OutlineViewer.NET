using FRC.NetworkTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public interface INetworkTableConnectionHandler
    {
        event EventHandler ConnectionChanged;
        int NumConnections { get; }
        NetworkMode NetworkMode { get; }
    }
}
