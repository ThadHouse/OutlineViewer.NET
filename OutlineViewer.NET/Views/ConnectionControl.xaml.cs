using FRC.NetworkTables;
using OutlineViewer.NET.NetworkTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ConnectionControl : UserControl
    {
        public string ServerLocation { get; set; }

        private DispatcherTimer dispatcherTimer;
        private INetworkTableConnectionHandler connectionHandler;

        public ConnectionControl()
        {
            this.InitializeComponent();
        }


        public void StartNetworking(INetworkTableConnectionHandler connectionHandler)
        {
            this.connectionHandler = connectionHandler;

            connectionHandler.ConnectionChanged += (o, e) =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                  {
                      UpdateConnectionLabel();
                  });
            };

            UpdateConnectionLabel();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (o, e) => UpdateConnectionLabel();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        public void UpdateConnectionLabel()
        {
            var mode = connectionHandler.NetworkMode;
            if (mode == NetworkMode.None)
            {
                GeneralFailure();
                return;
            } 

            if (mode.HasFlag(NetworkMode.Server))
            {
                if (mode.HasFlag(NetworkMode.Failure))
                {
                    ServerFail();
                } 
                else if (mode.HasFlag(NetworkMode.Starting))
                {
                    ServerStarting();
                }
                else
                {
                    ServerSuccess();
                }
            }
            else if (mode.HasFlag(NetworkMode.Client))
            {
                if (mode.HasFlag(NetworkMode.Failure))
                {
                    ClientFail();
                }
                else if (mode.HasFlag(NetworkMode.Starting))
                {
                    ClientStarting();
                }
                else
                {
                    ClientSuccess();
                }
            }
            else
            {
                GeneralFailure();
            }
        }

        private void ClientStarting()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ConnectionLabel.Text = string.Format(resourceLoader.GetString("ConnectingTo/Text"), ServerLocation);
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["ClientBrush"];
        }

        private void ClientFail()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (ServerLocation == null)
            {
                ConnectionLabel.Text = resourceLoader.GetString("NoConnection/Text");
            }
            else
            {
                ConnectionLabel.Text = string.Format(resourceLoader.GetString("NoConnectionTo/Text"), ServerLocation);
            }
            
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["FailedBrush"];
        }

        private void ClientSuccess()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ConnectionLabel.Text = string.Format(resourceLoader.GetString("ConnectedToServer/Text"), ServerLocation);
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["ClientBrush"];
        }

        private void ServerStarting()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ConnectionLabel.Text = resourceLoader.GetString("StartingServer/Text");
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["ServerBrush"];
        }

        private void ServerFail()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ConnectionLabel.Text = resourceLoader.GetString("CouldNotRunServer/Text");
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["FailedBrush"];
        }

        private void ServerSuccess()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var numClients = connectionHandler.NumConnections;
            if (numClients == 0)
            {
                ConnectionLabel.Text = resourceLoader.GetString("RunningServerNoClients/Text");
            }
            else if (numClients == 1)
            {
                ConnectionLabel.Text = resourceLoader.GetString("RunningServer1Client/Text");
            }
            else
            {
                ConnectionLabel.Text = string.Format(resourceLoader.GetString("RuningServerClients/Text"), numClients);
            }
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["ServerBrush"];
        }

        private void GeneralFailure()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ConnectionLabel.Text = resourceLoader.GetString("GeneralFailure/Text");
            ConnectionBlock.Fill = (Brush)ControlGrid.Resources["FailedBrush"];
        }
    }
}
