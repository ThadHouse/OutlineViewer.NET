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

        public void NewValue(string key, string value)
        {
            key = Normalize(key);
            var entry = instance.GetEntry(key);
            entry.SetString(value);
        }

        private string Normalize(string key)
        {
            string tmp = NetworkTable.PathSeparator + key;
            var sep = NetworkTable.PathSeparator;
            StringBuilder newString = new StringBuilder();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] == sep)
                {
                    // Add it
                    newString.Append(tmp[i]);
                    i++;
                    // Advance to first not sep character
                    while (true)
                    {
                        if (i >= tmp.Length) goto end;
                        if (tmp[i] == sep) i++;
                        else
                        {
                            i--;
                            break;
                        }
                    }
                }
                else
                {
                    newString.Append(tmp[i]);
                }
            }
        end:
            return newString.ToString();
        }
    }
}
