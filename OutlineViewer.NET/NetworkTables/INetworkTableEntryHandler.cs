using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public delegate void EntryNotification(string key, object value);

    public interface INetworkTableEntryHandler
    {
        event EntryNotification NewEntry;
        event EntryNotification UpdatedEntry;
        event EntryNotification DeletedEntry;

        void StartListener();
        void StopListener();
    }
}
