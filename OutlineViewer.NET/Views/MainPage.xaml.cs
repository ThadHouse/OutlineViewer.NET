using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
using OutlineViewer.NET.NetworkTables;
using OutlineViewer.NET.Views.NewEntry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OutlineViewer.NET.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private INetworkTableServerClientManager serverClientManager;
        private INetworkTableEntryHandler entryHandler;

        public MainPage()
        {
            this.InitializeComponent();

            var defaultInst = NetworkTableInstance.Default;
            ConnectionBlock.StartNetworking(new NetworkTableConnectionHandler(defaultInst));
            serverClientManager = new NetworkTableServerClientManager(defaultInst);

            defaultInst.GetEntry("Hello").SetString("42");
            defaultInst.GetEntry("Inner/S1").SetString("56"); 
            defaultInst.GetEntry("Inner/S2").SetString("56hh");

            defaultInst.GetEntry("Inner/N2/v2").SetString("5asdasd6");
            defaultInst.GetEntry("Inner/N2/v1").SetString("5hdfgh6");

            entryHandler = new NetworkTableEntryHandler(defaultInst);
            TableTree.StartNetworking(entryHandler);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ShowPreferencesInternal(true);
        }

        private async void ShowPreferences(object source, RoutedEventArgs e)
        {
            await ShowPreferencesInternal(false);
        }

        private async Task ShowPreferencesInternal(bool isStart)
        {
            PreferencesPage preferencesDialog = new PreferencesPage();
            if (isStart)
            {
                preferencesDialog.PrimaryButtonText = "Start";
                preferencesDialog.CloseButtonText = "Quit";
            } 
            else
            {
                preferencesDialog.PrimaryButtonText = "Update";
                preferencesDialog.CloseButtonText = "Cancel";
            }

            var result = await preferencesDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var serverProperties = preferencesDialog.GetServerProperties();
                if (serverProperties.ServerMode)
                {
                    serverClientManager.StartServer(serverProperties.Port);
                }
                else
                {
                    ConnectionBlock.ServerLocation = serverProperties.ServerLocation;
                    serverClientManager.StartClient(serverProperties.ServerLocation, serverProperties.Port);
                }

                ConnectionBlock.UpdateConnectionLabel();
                ConnectionBlock.Visibility = Visibility.Visible;
            }
            else if (isStart)
            {
                Application.Current.Exit();
            }
        }

        private async void NewString_Click(object sender, RoutedEventArgs e)
        {
            NewStringDialog dialog = new NewStringDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newData = dialog.GetNewEntryData();
                entryHandler.NewValue(newData.Key, newData.Value);
            }
        }
    }
}
