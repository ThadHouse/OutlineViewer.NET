using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OutlineViewer.NET.Views
{
    public class NetworkTableTreeItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate RootTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            NetworkTableTreeEntry entry = (NetworkTableTreeEntry)item;

            switch (entry.EntryType)
            {
                case EntryType.Root:
                    return RootTemplate;
                case EntryType.String:
                    return StringTemplate;
                case EntryType.Bool:
                    return BoolTemplate;
                case EntryType.Double:
                    return DoubleTemplate;
                default:
                    throw new InvalidOperationException("Unknown template");
            }
        }
    }
}
