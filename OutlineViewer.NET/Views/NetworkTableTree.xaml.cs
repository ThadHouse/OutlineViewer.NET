using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
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

            entryHandler.NewEntry += (k, e, v) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    NewEntry(k, e, v);
                });
            };

            entryHandler.UpdatedEntry += (k, v) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateEntry(k, v);
                });
            };

            entryHandler.DeletedEntry += (k) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DeleteEntry(k);
                });
            };

            entryHandler.StartListener();
        }

        private void UpdateEntry(string key, in NetworkTableValue value)
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

        private void AddNewNode(string fullName, string keyName, in NetworkTableEntry entry, NetworkTableTreeEntry parentEntry, in NetworkTableValue value)
        {
            NetworkTableTreeEntry newTreeEntry = new NetworkTableTreeEntry(parentEntry, keyName, entry, value);
            parentEntry.Children.Add(newTreeEntry);
            entryMap.Add(fullName, newTreeEntry);
            //SortNodes(DataSource[0]);
        }

        private void NewEntry(string key, in NetworkTableEntry entry, in NetworkTableValue value)
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
                AddNewNode(key, keyName, entry, current, value);
                return;
            }

            foreach (var str in split)
            {
                var tmpCurrent = current;
                // Find children 
                foreach (var child in current.Children)
                {
                    if (child.Name == str && child.EntryType == EntryType.Root)
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
            AddNewNode(key, keyName, entry, current, value);
        }

        private void DeleteEntry(string key)
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

        private void ChangePersistent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem mfi)
            {
                if (mfi.DataContext is NetworkTableTreeEntry treeEntry)
                {
                    treeEntry.ChangePersistent_Click();
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem mfi)
            {
                if (mfi.DataContext is NetworkTableTreeEntry treeEntry)
                {
                    treeEntry.Delete_Click();
                }
            }
            ;
            //entry.Delete();
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            if (sender is MenuFlyout mf)
            {
                if (mf.Target.DataContext is NetworkTableTreeEntry treeEntry)
                {
                    treeEntry.UpdatePersistText();
                }
                //if (mf. is NetworkTableTreeEntry treeEntry)
                //{
                //    
                //}
            }
        }

        private void TreeView_ItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            var toExpand = sender.ContainerFromItem(args.InvokedItem);
            var node = sender.NodeFromContainer(toExpand);
            if (node.IsExpanded)
            {
                sender.Collapse(node);
            }
            else
            {
                sender.Expand(node);
            }
        }
    }
}
