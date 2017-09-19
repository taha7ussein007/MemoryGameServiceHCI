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
using MemoryGameService;
using System.Threading;

namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for ChatReadyWindow.xaml
    /// </summary>
    public partial class ChatReadyWindow : Window
    {
        public ChatReadyWindow()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5);
                    updateRoomPlayers();
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5);
                    updateChatBox();
                }
            });
        }
        private async void Ready_btn_Click(object sender, RoutedEventArgs e)
        {
            Ready_btn.IsEnabled = false;
            // change state
            await Task.Run(() => {
                bool notReady = true;
                while(notReady)
                {
                    try 
                    {
                        globalVars.proxy.IReady(globalVars.player.id);
                        notReady = false;
                    }
                    catch(Exception)
                    {
                        notReady = true;
                    }
                }
            });
            //check if all is ready
            await Task.Run(() =>
            {
                try
                {
                    while (!globalVars.proxy.canIStart())
                    {
                        Thread.Sleep(5);
                    }
                }
                catch (Exception) { }
            });
            //switch to the online game window
            GameWindow _GameWindow = new GameWindow();
            App.Current.MainWindow = _GameWindow;
            _GameWindow.Show();
            this.Close();
        }
        private void updateRoomPlayers()
        {
            List<playerScore> Players_ScoresList =
                globalVars.offlineGameController.listRoomScores(globalVars.roomId);
            if (Players_ScoresList != null)
            {
                try
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        userNameState_listView.Items.Clear();
                        foreach (var playerscore in Players_ScoresList)
                        {
                            userNameState_listView.Items.Add(new playerScore
                            {
                                playername = playerscore.playername,
                                is_ready = playerscore.is_ready
                            });
                        }
                    }));
                }
                catch (Exception) { }
            }
        }
        private async void sendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            sendMsgBtn.IsEnabled = false;
            ChatMessage msg = new ChatMessage();
            msg.senderName = globalVars.player.username;
            msg.MsgContent = chatMsg_txtBox.Text;

            bool isSent = false;

            await Task.Run(() => 
            {
                if (msg.MsgContent.Length > 0)
                {
                    if (globalVars.onlineChatClientController.Send(msg))
                    {
                        isSent = true;
                    }
                    else
                    {
                        isSent = false;
                        MessageBox.Show("Invalid Msg Content Or Internal Error!", "Message Cannot Be Sent :(.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });

            if(isSent)
                chatMsg_txtBox.Text = "";

            sendMsgBtn.IsEnabled = true;
        }
        private void updateChatBox()
        {
            List<ChatMessage> msgs = 
                globalVars.onlineChatClientController.ReceiveAll(chatBox_listView.Items.Count - 1);
            if(msgs == null) {
                //pass (No New Msgs To Receive)
            }
            else
            {
                if(msgs.Count == 0){
                    //pass (No New Msgs To Receive)
                }
                else
                {
                    this.Dispatcher.Invoke((Action)(() => 
                    {
                        //Update ChatBox (There is new msgs)
                        foreach (var msg in msgs)
                        {
                            chatBox_listView.Items.Add(new ChatMessage
                            {
                                senderName = msg.senderName,
                                MsgContent = msg.MsgContent
                            });
                            chatBox_listView.SelectedIndex = chatBox_listView.Items.Count - 1;
                            chatBox_listView.ScrollIntoView(chatBox_listView.SelectedItem);
                            System.Media.SoundPlayer player = 
                                new System.Media.SoundPlayer(@"notifyMsg.wav");
                            player.Play();
                        }
                    }));
                }
            }
        }
        private void chatMsg_txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                sendMsgBtn_Click(sender, e);
            }
        }
    }
}
