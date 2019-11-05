using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OutlineViewer.NET.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PreferencesPage : ContentDialog
    {
        public PreferencesPage()
        {
            this.InitializeComponent();
        }

        public ServerProperties GetServerProperties()
        {
            return new ServerProperties()
            {
                Port = int.Parse(ServerPort.Text),
                ServerLocation = ServerLocation.Text,
                ServerMode = ServerMode.IsOn
            };
        }

        private void ServerMode_ToggleChanged(object sender, RoutedEventArgs e)
        {
            if (ServerLocation != null)
            {
                ServerLocation.IsEnabled = !ServerMode.IsOn;
            }
        }

        private void DefaultPort_ToggleChanged(object sender, RoutedEventArgs e)
        {
            if (ServerPort != null)
            {
                ServerPort.IsEnabled = !DefaultPort.IsOn;
            }
        }
    }
}
