﻿using FRC.NetworkTables;
using OutlineViewer.NET.NetworkTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OutlineViewer.NET.Views
{
    public sealed partial class NetworkTableTree : UserControl
    {
        public ObservableCollection<NetworkTableTreeEntry> DataSource = new ObservableCollection<NetworkTableTreeEntry>();

        private INetworkTableEntryHandler entryHandler;

        private readonly Dictionary<string, NetworkTableTreeEntry> entryMap = new Dictionary<string, NetworkTableTreeEntry>();

        public NetworkTableTree()
        {
            this.InitializeComponent();

            DataSource.Add(new NetworkTableTreeEntry(null, "Root"));
        }

        public void StartNetworking(INetworkTableEntryHandler entryHandler)
        {
            this.entryHandler = entryHandler;

            entryHandler.NewEntry += (k, v) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    NewEntry(k, v);
                });
            };

            entryHandler.UpdatedEntry += (k, v) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateEntry(k, v);
                });
            };

            entryHandler.DeletedEntry += (k, v) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DeleteEntry(k, v);
                });
            };

            entryHandler.StartListener();
        }

        private void UpdateEntry(string key, object value)
        {
            entryMap[key].SetEntryValue(value);
        }

        private void SortNodes(NetworkTableTreeEntry current)
        {
            current.Children = new ObservableCollection<NetworkTableTreeEntry>(current.Children.OrderByDescending(x => x.EntryType == EntryType.Root).ThenBy(x => x.Name));
            foreach (var child in current.Children)
            {
                SortNodes(child);
            }
        }

        private void AddNewNode(string fullName, string keyName, NetworkTableTreeEntry parentEntry, object value)
        {
            NetworkTableTreeEntry newTreeEntry = null;
            switch (value)
            {
                case string str:
                    newTreeEntry = new NetworkTableTreeEntry(parentEntry, keyName, str);
                    break;
                case double dbl:
                    newTreeEntry = new NetworkTableTreeEntry(parentEntry, keyName, dbl);
                    break;
                case bool bl:
                    newTreeEntry = new NetworkTableTreeEntry(parentEntry, keyName, bl);
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
            parentEntry.Children.Add(newTreeEntry);
            entryMap.Add(fullName, newTreeEntry);
            //SortNodes(DataSource[0]);
        }

        private void NewEntry(string key, object value)
        {
            var normalized = Normalize(key);

            var split = normalized.Split(NetworkTable.PathSeparator).AsSpan();
            var keyName = split[split.Length - 1];
            split = split.Slice(1, split.Length - 2);


            // First entry will always be Root
            var current = DataSource[0];

            // Special case single values
            if (split.Length == 0)
            {
                // Goes in root
                AddNewNode(key, keyName, current, value);
                return;
            }

            foreach (var str in split)
            {
                var tmpCurrent = current;
                // Find children 
                foreach (var child in current.Children)
                {
                    if (child.Name == str)
                    {
                        current = child;
                        break;
                    }
                }
                if (ReferenceEquals(current, tmpCurrent))
                {
                    // Was not found, add a new node
                    var newEmptyNode  = new NetworkTableTreeEntry(current, str);
                    current.Children.Add(newEmptyNode);
                    current = newEmptyNode;
                }
            }

            // Found the node to add.
            AddNewNode(key, keyName, current, value);
        }

        private void DeleteEntry(string key, object value)
        {
            var entry = entryMap[key];
            entryMap.Remove(key);
            // Remove this key
            while (true)
            {
                // Root is only null parent
                if (entry.Parent == null) break;
                entry.Parent.Children.Remove(entry);
                if (entry.Parent.Children.Count == 0)
                {
                    entry.Parent.Children.Remove(entry);
                    entry = entry.Parent;
                }
                else
                {
                    break;
                }

            }
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
