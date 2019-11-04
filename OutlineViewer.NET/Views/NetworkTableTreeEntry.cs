using FRC.NetworkTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        private string name;
        private string stringValue;
        private double doubleValue;
        private bool boolValue;

        public EntryType EntryType { get; set; }

        public string Name { get => name; set => name = value; }
        public string StringValue
        {
            get => stringValue;
            set
            {
                stringValue = value;
                OnPropertyChanged();
            }
        }
        public double DoubleValue
        {
            get => doubleValue;
            set
            {
                doubleValue = value;
                OnPropertyChanged();
            }
        }
        public bool BoolValue
        {
            get => boolValue;
            set
            {
                boolValue = value;
                OnPropertyChanged();
            }
        }

        public void SetEntryValue(object value)
        {
            switch (value)
            {
                case string str:
                    EntryType = EntryType.String;
                    StringValue = str;
                    break;
                case double dbl:
                    EntryType = EntryType.Double;
                    DoubleValue = dbl;
                    break;
                case bool bl:
                    EntryType = EntryType.Bool;
                    BoolValue = bl;
                    break;
                case byte[] raw:
                    break;
                case bool[] blArr:
                    break;
                case double[] dblArr:
                    break;
                case string[] strArr:
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Value?");
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

        public NetworkTableTreeEntry(NetworkTableTreeEntry parent, string name, string value)
        {
            Parent = parent;
            Name = name;
            EntryType = EntryType.String;
            StringValue = value;
        }

        public NetworkTableTreeEntry(NetworkTableTreeEntry parent, string name, double value)
        {
            Parent = parent;
            Name = name;
            EntryType = EntryType.Double;
            DoubleValue = value;
        }

        public NetworkTableTreeEntry(NetworkTableTreeEntry parent, string name, bool value)
        {
            Parent = parent;
            Name = name;
            EntryType = EntryType.Bool;
            BoolValue = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
