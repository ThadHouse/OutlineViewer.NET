using FRC.NetworkTables;
using FRC.NetworkTables.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

namespace OutlineViewer.NET
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is StartProperties startProperties)
            {
                NtCoreLoader.ExtractOnStaticLoad = false;
                NtCore.ForceLoad();

                NetworkTableInstance inst = NetworkTableInstance.Default;
                inst.SetNetworkIdentity("OutlineViewer.NET");
                if (startProperties.ServerMode)
                {
                    inst.StartServer(port: startProperties.Port);
                }
                else
                {
                    if (int.TryParse(startProperties.ServerLocation, out int result))
                    {
                        inst.StartClientTeam(result, startProperties.Port);
                    } 
                    else
                    {
                        inst.StartClient(startProperties.ServerLocation, startProperties.Port);
                    }
                }

                TableTree.StartNetworking(inst);

                ConnectionBlock.StartNetworking(startProperties, inst);
                
                ;
            }

            // parameters.Name
            // parameters.Text
            // ...
        }
    }
}
