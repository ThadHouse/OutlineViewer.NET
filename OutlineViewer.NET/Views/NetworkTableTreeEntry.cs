using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OutlineViewer.NET.Views
{
    public enum EntryType
    {
        Root,
        String,
        Bool,
        Double
    }

    public class NetworkTableTreeEntry : INotifyPropertyChanged
    {
        private string stringValue;
        private double doubleValue;
        private bool boolValue;
        private NetworkTableEntry entry;
        private bool deleted = false;

        public EntryType EntryType { get; set; }

        public string Name { get; set; }
        public string StringValue
        {
            get => stringValue;
            set
            {
                stringValue = value;
                if (!deleted) entry.SetValue(value);
                OnPropertyChanged();
                
            }
        }
        public double DoubleValue
        {
            get => doubleValue;
            set
            {
                doubleValue = value;
                if (!deleted) entry.SetValue(value);
                OnPropertyChanged();
            }
        }
        public bool BoolValue
        {
            get => boolValue;
            set
            {
                boolValue = value;
                if (!deleted) entry.SetValue(value);
                OnPropertyChanged();
            }
        }

        public void SetEntryValue(in NetworkTableValue value)
        {
            switch (value.Type)
            {
                case NtType.Boolean:
                    EntryType = EntryType.Bool;
                    BoolValue = value.GetBoolean();
                    break;
                case NtType.Double:
                    EntryType = EntryType.Double;
                    DoubleValue = value.GetDouble();
                    break;
                case NtType.String:
                    EntryType = EntryType.String;
                    StringValue = value.GetString();
                    break;
                //case NtType.Raw:
                //    break;
                //case NtType.BooleanArray:
                //    break;
                //case NtType.DoubleArray:
                //    break;
                //case NtType.StringArray:
                //    break;
                //case NtType.Rpc:
                //    break;
                default:
                    break;
            }
        }

        public ObservableCollection<NetworkTableTreeEntry> Children { get; set; } = new ObservableCollection<NetworkTableTreeEntry>();

        public NetworkTableTreeEntry Parent { get; }

        public NetworkTableTreeEntry(NetworkTableTreeEntry parent, string name)
        {
            Parent = parent;
            Name = name;
            EntryType = EntryType.Root;
        }

        public NetworkTableTreeEntry(NetworkTableTreeEntry parent, string name, in NetworkTableEntry entry, in NetworkTableValue value)
        {
            Parent = parent;
            Name = name;
            this.entry = entry;
            SetEntryValue(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string PersistentText => !entry.IsPersistent() ? "Set Persistent" : "Set Transient";

        public void UpdatePersistText()
        {
            OnPropertyChanged(nameof(PersistentText));
        }

        public void ChangePersistent_Click()
        {
            if (entry.IsPersistent())
            {
                entry.ClearPersistent();
            }
            else
            {
                entry.SetPersistent();
            }
        }

        public void Delete_Click()
        {
            deleted = true;
            entry.Delete();
        }
    }
}
