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


        private readonly NetworkTableInstance instance;
        private NtEntryListener entryListener;
        private NtEntryListener updateListener;

        public NetworkTableEntryHandler(NetworkTableInstance instance)
        {
            this.instance = instance;
        }

        public event NewEntryNotification NewEntry;
        public event UpdateEntryNotification UpdatedEntry;
        public event DeleteEntryNotification DeletedEntry;

        public void StartListener()
        {
            entryListener = instance.AddEntryListener("", (in RefEntryNotification notification) =>
            {
                if (notification.Flags.HasFlag(NotifyFlags.New))
                {
                    NewEntry?.Invoke(notification.Name, notification.Entry, notification.Value.ToValue());
                }
                else if (notification.Flags.HasFlag(NotifyFlags.Delete))
                {
                    DeletedEntry?.Invoke(notification.Name);
                }
            }, NotifyFlags.New | NotifyFlags.Delete | NotifyFlags.Immediate | NotifyFlags.Local);

            updateListener = instance.AddEntryListener("", (in RefEntryNotification notification) =>
            {
                UpdatedEntry?.Invoke(notification.Name, notification.Value.ToValue());
            }, NotifyFlags.Update | NotifyFlags.Immediate);
        }

        public void StopListener()
        {
            if (entryListener.Get() != 0)
            {
                instance.RemoveEntryListener(entryListener);
            }

            if (updateListener.Get() != 0)
            {
                instance.RemoveEntryListener(updateListener);
            }
        }
    }
}
