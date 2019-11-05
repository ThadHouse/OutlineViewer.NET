using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public delegate void UpdateEntryNotification(string key, NetworkTableValue value);
    public delegate void NewEntryNotification(string key, NetworkTableEntry entry, NetworkTableValue value);
    public delegate void DeleteEntryNotification(string key);

    public interface INetworkTableEntryHandler
    {
        event NewEntryNotification NewEntry;
        event UpdateEntryNotification UpdatedEntry;
        event DeleteEntryNotification DeletedEntry;

        void StartListener();
        void StopListener();
    }
}
