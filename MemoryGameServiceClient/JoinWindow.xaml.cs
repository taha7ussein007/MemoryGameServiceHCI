using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.ServiceModel;
using MemoryGameService;
using System.Threading;

namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for JoinWindow.xaml
    /// </summary>
    public partial class JoinWindow : Window
    {
        private System.Windows.Forms.Timer updateRoomsListTimer;
        public JoinWindow()
        {
            InitializeComponent();
            updateRoomsList();
            //updateRoomsListTimer init.
            updateRoomsListTimer = new System.Windows.Forms.Timer();
            // Hook up the Elapsed event for the timer.
            updateRoomsListTimer.Tick += new EventHandler(updateRoomsListTimer_EventProcessor);
            updateRoomsListTimer.Interval = 2500; //ms
            updateRoomsListTimer.Start();
        }
        private void updateRoomsListTimer_EventProcessor(Object myObject, EventArgs myEventArgs)
        {
            updateRoomsList();
        }
        private async void updateRoomsList()
        {
            await Task.Run(() => {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    List<gameRoom> roomslist =
                        globalVars.offlineGameController.listAllRooms();
                    listRooms.Items.Clear();
                    foreach (var gr in roomslist)
                    {
                        listRooms.Items.Add(new Room
                        {
                            ID = gr.id.ToString(),
                            RoomName = gr.name,
                            HostUserName = gr.hostName,
                            NumberOfPlayer = gr.numOfPlayers.ToString(),
                            State = gr.gameState.ToString(),
                            Level = gr.level.ToString(),
                            maxPlayers = gr.maxPlayers.ToString()
                        });
                    }
                }));
            });
        }
        private async void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => 
            {
                disableGraphicsAndWait();
                bool invalidRoomID = false;
                //Validiate Room ID
                this.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        globalVars.roomId = Convert.ToInt32(RoomId_txt.Text);
                    }
                    catch(Exception)
                    {
                        invalidRoomID = true;
                        globalVars.roomId = 0;
                        MessageBox.Show("Invalid Room ID!", "Invalid Info.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        resetGraphics();
                    }
                }));
                if (invalidRoomID) return; //Don't Make Progress just return
                //Get The Room Data By ID
                gameRoom gr = new gameRoom(globalVars.roomId);
                if (gr.id == 0) //Room Not Found!
                {
                    globalVars.roomId = 0;
                    MessageBox.Show("Room Not Found!", "Invalid Info.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    resetGraphics();
                    return;
                }
                else
                {
                    //Trying To Establish a connection to the host:
                    try
                    {
                        string MemoryGameServiceHostUri = "http://" + gr.host_IP + ":8888/MemoryGameService/MemoryGameService/";
                        EndpointAddress MemoryGameServiceEndpointAddress = new EndpointAddress(MemoryGameServiceHostUri);
                        BasicHttpBinding _BasicHttpBinding = new BasicHttpBinding();
                        ChannelFactory<IMemoryGameService> _ChannelFactory =
                            new ChannelFactory<IMemoryGameService>(_BasicHttpBinding, MemoryGameServiceEndpointAddress);
                        globalVars.proxy = _ChannelFactory.CreateChannel();
                    }
                    catch (Exception ex)
                    {
                        globalVars.roomId = 0;
                        MessageBox.Show(ex.Message, "Cannot Establish a Connection.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        resetGraphics();
                        return;
                    }
                    //Trying To Join The Game Room:
                    bool isJoined = false;
                    try
                    {
                        isJoined = globalVars.proxy.joinRoom(globalVars.roomId,
                            globalVars.player.id, globalVars.userIp);
                    }
                    catch (Exception ex)
                    {
                        globalVars.roomId = 0;
                        MessageBox.Show(ex.Message, "Connection Lost :(",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        resetGraphics();
                        return;
                    }
                    //If Joined Successfully -> Switch To The Next Window:
                    if (isJoined)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            //switch to next window
                            ChatReadyWindow _ChatReadyWindow = new ChatReadyWindow();
                            App.Current.MainWindow = _ChatReadyWindow;
                            _ChatReadyWindow.Show();
                            this.Close();
                        });
                    }
                    else
                    {
                        globalVars.roomId = 0;
                        MessageBox.Show("Sorry, No Late Join :(", "Cannot Join This Room.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        resetGraphics();
                        return;
                    }
                }
            });
        }
        private void listRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Room _room = (Room)listRooms.SelectedItems[0];
                RoomId_txt.Text = _room.ID.ToString();
            }
            catch (Exception) { }
        }
        private void disableGraphicsAndWait()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Connect_btn.IsEnabled = false;
                this.Connect_btn.Content = "Please Wait... :)";
            }));
        }
        private void resetGraphics()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Connect_btn.IsEnabled = true;
                this.Connect_btn.Content = "Connect";
            }));
        }
    }
}
public class Room
{
    public string ID { get; set; }
    public string RoomName { get; set; }
    public string HostUserName { get; set; }
    public string NumberOfPlayer { get; set; }
    public string State { get; set; }
    public string Level { get; set; }
    public string maxPlayers { get; set; }
}