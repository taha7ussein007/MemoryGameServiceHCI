using MemoryGameService;
using NATUPNPLib;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for CreateJoinWindow.xaml
    /// </summary>
    public partial class CreateJoinWindow : Window
    {
        public CreateJoinWindow()
        {
            InitializeComponent();
        }
        private async void CreateHostOnline_btn_Click(object sender, RoutedEventArgs e)
        {
            disableGraphicsAndWait();
            await Task.Run(() => 
            {
                if (!checkForInternetConnection())
                {
                    MessageBox.Show("Please Check Your Internet Connection!\n" +
                        ">Try to turn off any firewall program.\n" +
                        ">Or just try to [Create Host Via Local Network].", "Internet Connection Missing.",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    resetGraphics();
                    return;
                }
                createHost(getPublicIp());
            });
        }
        private async void CreateHostNetwork_btn_Click(object sender, RoutedEventArgs e)
        {
            disableGraphicsAndWait();
            await Task.Run(() => { createHost(getLocalIp()); });
        }
        private async void createHost(string hostIP)
        {
            var testNetIp = "";
            try { testNetIp = getLocalIp(); } catch (Exception) { }
            //Check Network Connection First Of All
            if (isNetworkAvailable() || (testNetIp != "127.0.0.1" && testNetIp != "") )
            {
                #region Try To Open Port 8888
                try
                {
                    bool isPortOpened = false;
                    try
                    {
                        #region Create Port Mapping Via UPnP Native Lib
                        //create port mapping via UPnP
                        UPnPNATClass upnpnat = new UPnPNATClass();
                        IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
                        foreach (IStaticPortMapping portMapping in mappings)
                        {
                            // remove any old mappings
                            if (portMapping.ExternalPort == 8888)
                            {
                                // Remove TCP forwarding for Port 8888
                                mappings.Remove(8888, "TCP");
                                // Remove UDP forwarding for Port 8888
                                mappings.Remove(8888, "UDP");
                            }
                        }
                        // Add TCP forwarding for Port 8888
                        mappings.Add(8888, "TCP", 8888, getLocalIp(), true, "MGS");
                        // Add UDP forwarding for Port 8888
                        mappings.Add(8888, "UDP", 8888, getLocalIp(), true, "MGS");
                        isPortOpened = true;
                        #endregion
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        isPortOpened = false;
                    }
                    if(!isPortOpened)
                    {
                        #region Create Port Mapping Via UPnP Open.NAT Lib
                        var discoverer = new NatDiscoverer();
                        var cts = new CancellationTokenSource(10000);
                        var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                        // remove any old mappings
                        foreach (var mapping in await device.GetAllMappingsAsync())
                        {
                            if (mapping.Description.Contains("MGS"))
                            {
                                await device.DeletePortMapAsync(mapping);
                            }
                        }
                        // Add TCP forwarding for Port 8888
                        await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, IPAddress.Parse(getLocalIp()), 8888, 8888, 0, "MGS"));
                        // Add UDP forwarding for Port 8888
                        await device.CreatePortMapAsync(new Mapping(Protocol.Udp, IPAddress.Parse(getLocalIp()), 8888, 8888, 0, "MGS"));
                        isPortOpened = true;
                        #endregion
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("It seems to be that your router does not support UPnP!\n"
                    +"Please open the port < 8888 (TCP/UDP) > manually then press ok", "Cannot Open <8888> Port Automatically :(",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                #endregion

                //Open Host And Join Self
                try
                {
                    //get host.exe path
                    string hostPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    hostPath = hostPath.Replace("Client", "Host");
                    //set Host IP
                    globalVars.userIp = hostIP;
                    //start server
                    System.Diagnostics.Process.Start(hostPath, globalVars.userIp);
                    //join self
                    await Task.Run(() => { joinSelf(); });
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                    resetGraphics();
                    return;
                }
                //Open Room In Database And Join It then Switch Window
                try
                {
                    //Initialize And Create A GameRoom
                    this.Dispatcher.Invoke((Action)(() => { initAndCreateGameRoom(); }));
                    //Join The Game Room
                    if (globalVars.roomId != 0)
                    {
                        bool isJoined = globalVars.proxy.joinRoom(globalVars.roomId,
                            globalVars.player.id, globalVars.userIp);
                        if(isJoined)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                switchToNextWindow();
                            });
                        }
                        else
                        {
                            //Try To Delete The Room
                            try
                            {
                                globalVars.proxy.deleteRoom();
                            }
                            catch(Exception) //If The Server Failed To Delete The Room (Connection Lost)
                            {
                                globalVars.offlineGameController.deleteRoom(globalVars.roomId);
                            }
                            MessageBox.Show("Room Created But Cannot Join The Room!", "Internal Error Occured :(",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            resetGraphics();
                            return;
                        }
                    }
                    else
                    {
                        resetGraphics();
                        return;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    resetGraphics();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please Check Your Network Connection!", "Network Connection Missing.", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void disableGraphicsAndWait()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.numOfPlayer_txt.IsEnabled = false;
                this.numOfPlays_txt.IsEnabled = false;
                this.LevelBox.IsEnabled = false;
                this.CreateHostOnline_btn.IsEnabled = false;
                this.CreateHostNetwork_btn.IsEnabled = false;
                this.Join_btn.IsEnabled = false;

                CreateHostOnline_btn.FontSize = 40;
                CreateHostOnline_btn.Content = "Please Wait... :)";
            }));
        }
        private void resetGraphics()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.numOfPlayer_txt.IsEnabled = true;
                this.numOfPlays_txt.IsEnabled = true;
                this.LevelBox.IsEnabled = true;
                this.CreateHostOnline_btn.IsEnabled = true;
                this.CreateHostNetwork_btn.IsEnabled = true;
                this.Join_btn.IsEnabled = true;

                CreateHostOnline_btn.FontSize = 60;
                CreateHostOnline_btn.Content = "Create Host (Online)";
            }));
        }
        private void initAndCreateGameRoom()
        {
            gameRoom gr = new gameRoom();
            //Initialize "7" Required Game Room Attributes.
            gr.gameState = GAME_STATE.Wait;
            gr.host_ID = globalVars.player.id;
            gr.host_IP = globalVars.userIp;
            if (LevelBox.SelectedIndex == -1) gr.level = 1; //default = 1
            else gr.level = LevelBox.SelectedIndex + 1;
            if ((numOfPlayer_txt.Text).Length == 0) gr.maxPlayers = 2; //default = 2
            else gr.maxPlayers = int.Parse(numOfPlayer_txt.Text);
            if ((RoomName_txt.Text).Length == 0) gr.name = "Unnamed"; //default = "Unnamed"
            else gr.name = RoomName_txt.Text;
            if ((numOfPlays_txt.Text).Length == 0) gr.numOfPlays = 3; //default
            else gr.numOfPlays = int.Parse(numOfPlays_txt.Text);
            //Add The Room To DB
            try
            {
                int roomId_or_Zero = globalVars.proxy.openRoom(gr);
                if (roomId_or_Zero != 0)
                    globalVars.roomId = roomId_or_Zero;
                else
                {
                    throw new Exception("Cannot Open The Game Room :(");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void joinSelf()
        {
            try
            {
                string MemoryGameServiceHostUri = "http://" + globalVars.userIp + ":8888/MemoryGameService/MemoryGameService/";
                EndpointAddress MemoryGameServiceEndpointAddress = new EndpointAddress(MemoryGameServiceHostUri);
                //make a connection to self:
                BasicHttpBinding _BasicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IMemoryGameService> _ChannelFactory =
                    new ChannelFactory<IMemoryGameService>(_BasicHttpBinding, MemoryGameServiceEndpointAddress);
                globalVars.proxy = _ChannelFactory.CreateChannel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void Join_btn_Click(object sender, RoutedEventArgs e)
        {
            disableGraphicsAndWait();
            await Task.Run(() => 
            {
                if (isNetworkAvailable())
                {
                    if (checkForInternetConnection())
                        globalVars.userIp = getPublicIp();
                    else
                        globalVars.userIp = getLocalIp();
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        JoinWindow _JoinWindow = new JoinWindow();
                        App.Current.MainWindow = _JoinWindow;
                        _JoinWindow.Show();
                        this.Close();
                    });
                }
                else
                {
                    MessageBox.Show("Please Check You Network Connection!", "Network Connection Missing.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    resetGraphics();
                }
            });
        }
        private void switchToNextWindow()
        {
            ChatReadyWindow _ChatReadyWindow = new ChatReadyWindow();
            App.Current.MainWindow = _ChatReadyWindow;
            _ChatReadyWindow.Show();
            this.Close();
        }
        //check for Internet connectivity
        private bool checkForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        //check for a network connection
        private static bool isNetworkAvailable()
        {
            bool Result = isNetworkAvailable(0);
            //return Result;
            return true;
        }
        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        private static bool isNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
        private string getLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        private string getPublicIp()
        {
            try
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch(Exception) //Keep Trying Till Getting The IP :P
            {
                return getPublicIp();
            }
        }
    }
}
