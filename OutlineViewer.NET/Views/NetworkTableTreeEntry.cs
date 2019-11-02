using FRC.NetworkTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.Views
{
    public class NetworkTableTreeEntry
    {
        public string Name { get; set; }
        public NetworkTableEntry NetworkTableEntry;
        public ObservableCollection<NetworkTableTreeEntry> Children { get; set; } = new ObservableCollection<NetworkTableTreeEntry>();
    }
}
