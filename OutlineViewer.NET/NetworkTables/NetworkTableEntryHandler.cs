using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineViewer.NET.NetworkTables
{
    public class NetworkTableEntryHandler : INetworkTableEntryHandler
    {
        public event EntryNotification NewEntry;
        public event EntryNotification UpdatedEntry;
        public event EntryNotification DeletedEntry;


        private readonly NetworkTableInstance instance;
        private NtEntryListener entryListener;

        public NetworkTableEntryHandler(NetworkTableInstance instance)
        {
            this.instance = instance;
        }

        public void StartListener()
        {
            entryListener = instance.AddEntryListener("", (in RefEntryNotification notification) =>
            {
                if (notification.Flags.HasFlag(NotifyFlags.New))
                {
                    NewEntry?.Invoke(notification.Name, notification.Value.Value.GetValue());
                }
                else if (notification.Flags.HasFlag(NotifyFlags.Update))
                {
                    UpdatedEntry?.Invoke(notification.Name, notification.Value.Value.GetValue());
                }
                else if (notification.Flags.HasFlag(NotifyFlags.Delete))
                {
                    DeletedEntry?.Invoke(notification.Name, notification.Value.Value.GetValue());
                }
            }, NotifyFlags.New | NotifyFlags.Delete | NotifyFlags.Update | NotifyFlags.Immediate | NotifyFlags.Local);
        }

        public void StopListener()
        {
            if (entryListener.Get() != 0)
            {
                instance.RemoveEntryListener(entryListener);
            }
        }
    }
}
