using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Diagnostics;
using MemoryGameService;
using System.Threading;
using System.Data;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        #region variables
        private Dictionary<int, Button> dictionary;
        private List<Button> randomList;
        private List<Button> buttonList;
        private Random rdm;
        private Button one_pass;
        private int count, scoreVal, numOfColoredObjects = 4, localNumOfAnswers;
        #endregion
        //================================================GameWindow========================================================	
        public GameWindow()
        {
            InitializeComponent();
            dictionary = new Dictionary<int, Button>();
            randomList = new List<Button>();
            buttonList = new List<Button>();
            rdm = new Random();
            dictionary.Add(0, Object_000);
            dictionary.Add(1, Object_001);
            dictionary.Add(2, Object_002);
            dictionary.Add(3, Object_003);
            dictionary.Add(4, Object_004);
            dictionary.Add(5, Object_005);
            dictionary.Add(6, Object_006);
            dictionary.Add(7, Object_007);
            dictionary.Add(8, Object_008);
            dictionary.Add(9, Object_009);
            dictionary.Add(10, Object_010);
            dictionary.Add(11, Object_011);
            dictionary.Add(12, Object_012);
            dictionary.Add(13, Object_013);
            dictionary.Add(14, Object_014);
            dictionary.Add(15, Object_015);
            //set user name label
            userName_lbl.Content += globalVars.userName_mainWindow;

            //Auto scores Updater
            ListPlayersScores();

            //check if online mode
            onlineAutoStart();

            Task.Run(() =>
            {
                Thread.Sleep(new Random().Next(100, 500));
                this.Dispatcher.Invoke(new Action(() => { notify_updateScores(); }), DispatcherPriority.Send);
                while (true)
                {
                    var numOfAns = globalVars.proxy.getNumOfAnswers();
                    if (numOfAns > localNumOfAnswers)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            notify_updateScores();
                            if (globalVars.proxy.checkGameState() == GAME_STATE.Finished)
                                notify_gameFinished();
                            else
                                notify_goToNxtLvl();
                        }), DispatcherPriority.Send);
                        localNumOfAnswers = numOfAns;
                    }
                    Thread.Sleep(new Random().Next(1, 15));
                }
            });
        }
        //========================================================onlineAutoStart================================================	
        private async void onlineAutoStart()
        {
            if (!globalVars.offlineGame) //case onlineGame -> auto-start
            {
                try
                {
                    globalVars.proxy.initializeGame();
                    Start_btn.IsEnabled = false;
                    //stopwatch 10 seconds then auto-start
                    for (int begin = 10; begin > 0; begin--)
                    {
                        Start_btn.Content = begin.ToString();
                        await Task.Delay(1000);
                    }
                    Start_btn.Content = "GO";
                    startGame();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                    //onlineAutoStart();
                }
            }
        }
        //====================================================================================StartGame====================	
        private async void startGame()
        {
            Start_btn.IsEnabled = false;
            Object_000.IsEnabled = true;
            Object_001.IsEnabled = true;
            Object_002.IsEnabled = true;
            Object_003.IsEnabled = true;
            Object_004.IsEnabled = true;
            Object_005.IsEnabled = true;
            Object_006.IsEnabled = true;
            Object_007.IsEnabled = true;
            Object_008.IsEnabled = true;
            Object_009.IsEnabled = true;
            Object_010.IsEnabled = true;
            Object_011.IsEnabled = true;
            Object_012.IsEnabled = true;
            Object_013.IsEnabled = true;
            Object_014.IsEnabled = true;
            Object_015.IsEnabled = true;
            await Task.Delay(1000);
            RendLst();
            await Task.Delay(1500);
            setGame();
        }
        //===============================================RendLst=========================================================	
        public void RendLst()
        {
            randomList.Clear();
            List<int> list = new List<int>();
            list = generateRdmLst();
            if (list != null)
            {
                foreach (int prime in list) // Loop through List with foreach.
                {
                    one_pass = dictionary[prime];
                    randomList.Add(one_pass);
                    one_pass.Background = Brushes.Blue;
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
        //===================================================RendLst=====================================================	
        //2cases
        public List<int> generateRdmLst()
        {
            if (globalVars.offlineGame)
            {
                List<int> rdmLst = new List<int>();
                rdmLst.Clear();
                for (int i = 0; i < numOfColoredObjects; i++)
                {
                    int randomValue = rdm.Next(0, 16);
                    if (!rdmLst.Contains(randomValue))
                    {
                        rdmLst.Add(randomValue);
                    }
                    else
                    {
                        i--;
                        continue;
                    }
                }
                return rdmLst;
            }
            else
            {
                try
                {
                    return globalVars.proxy.getRdmLst();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Connection Lost :(",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }
        //===============================================setGame=========================================================	
        private void setGame()
        {
            //setGame
            for (int x = 0; x < 16; x++)
            {
                one_pass = dictionary[x];
                one_pass.Background = Brushes.Red;
            }
            buttonList.Clear();
            count = 0;
        }
        //===============================================ListPlayersScores===============================================
        private async void ListPlayersScores()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        if (!globalVars.offlineGame)
                        {
                            try
                            {
                                List<playerScore> players_ScoreList = globalVars.proxy.listRoomScores();
                                listPlayers.Items.Clear();
                                foreach (var player_score in players_ScoreList)
                                {
                                    listPlayers.Items.Add(new playerScore { playername = player_score.playername, score = player_score.score });
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                            listPlayers.Items.Add(new playerScore { playername = globalVars.userName_mainWindow, score = scoreVal.ToString() });
                    }
                    catch (Exception) { }
                }));
            });
        }
        //===================================================notify_updateScores=======================
        public void notify_updateScores()
        {
            try { ListPlayersScores(); }
            catch (Exception) { notify_updateScores(); }
        }
        //===================================================notify_goToNxtLvl====================================
        public async void notify_goToNxtLvl()
        {
            //if not answered go next level
            await Continue();
        }
        //=====================================================================================Continue===================	
        private async Task Continue()
        {
            setGame();
            RendLst();
            await Task.Delay(1500);
            setGame();
        }
        //===================================================notify_gameFinished====================================
        public void notify_gameFinished()
        {
            try
            {
                Thread.Sleep(100);
                //stop the game + declare the winner
                stopGame();
                string winnerName = globalVars.proxy.getTheWinner();
                for (int i = 0; i < 200; i++)
                {
                    if (winnerName == "ERROR 404 Winner Not Found xD")
                        winnerName = globalVars.proxy.getTheWinner();
                    else
                        break;
                }
                MessageBox.Show("GameOver. <" + winnerName + "> Wins. :)");
                if (!globalVars.offlineGame)
                {
                    globalVars.proxy.deleteRoom();
                    //((IClientChannel)globalVars.proxy).Close();
                }
                //switch to main window
                MainWindow _MainWindow = new MainWindow();
                App.Current.MainWindow = _MainWindow;
                _MainWindow.Show();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        //====================================================================================stopGame====================	
        private void stopGame()
        {
            Start_btn.IsEnabled = false;
            Object_000.IsEnabled = false;
            Object_001.IsEnabled = false;
            Object_002.IsEnabled = false;
            Object_003.IsEnabled = false;
            Object_004.IsEnabled = false;
            Object_005.IsEnabled = false;
            Object_006.IsEnabled = false;
            Object_007.IsEnabled = false;
            Object_008.IsEnabled = false;
            Object_009.IsEnabled = false;
            Object_010.IsEnabled = false;
            Object_011.IsEnabled = false;
            Object_012.IsEnabled = false;
            Object_013.IsEnabled = false;
            Object_014.IsEnabled = false;
            Object_015.IsEnabled = false;
        }
        //==================================================check======================================================	
        //check for answer
        private bool check()
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                //case offline or online mode (incorrect answers not accepted)
                if (!buttonList.Contains(randomList[i]))
                    return false;
            }
            //case the answer is correct :)
            if (!globalVars.offlineGame)
            {
                try
                {
                    globalVars.proxy.IAnswered(globalVars.player.id);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            return true;
        }
        //====================================================================================
        #region Object Clicks & Brushes
        private void Object_000_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_000);
        }
        private void Object_001_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_001);
        }
        private void Object_002_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_002);
        }
        private void Object_003_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_003);
        }
        private void Object_004_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_004);
        }
        private void Object_005_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_005);
        }
        private void Object_006_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_006);
        }
        private void Object_007_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_007);
        }
        private void Object_008_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_008);
        }
        private void Object_009_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_009);
        }
        private void Object_010_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_010);
        }
        private void Object_011_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_011);
        }
        private void Object_012_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_012);
        }
        private void Object_013_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_013);
        }
        private void Object_014_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_014);
        }
        private void Object_015_Click(object sender, RoutedEventArgs e)
        {
            brush(Object_015);
        }
        private async void brush(Button btn)
        {
            if (!buttonList.Contains(btn))
            {
                buttonList.Add(btn);
                btn.Background = Brushes.Yellow;
                count++;
            }
            else
            {
                buttonList.Remove(btn);
                btn.Background = Brushes.Red;
                count--;
            }
            if (randomList.Count == count)
            {
                if (check())
                {
                    scoreVal++;
                    score_local_lbl.Text = scoreVal.ToString();
                    await Continue();
                }
                else
                {
                    scoreVal--;
                    score_local_lbl.Text = scoreVal.ToString();
                    await Continue();
                }
            }
        }
        #endregion
        //====================================================================================

        //====================================================================================RendLst_Click=========
        private void Start_btn_Click(object sender, RoutedEventArgs e) //Start Btn
        {
            startGame();
        }
        //=====================================================================================end_Click============
        //2cases
        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!globalVars.offlineGame)
            {
                try
                {
                    globalVars.proxy.ISurrender(globalVars.player.id);
                    //((IClientChannel)globalVars.proxy).Close();
                }
                catch (Exception) { }
            }
            //switch to main window
            MainWindow _MainWindow = new MainWindow();
            App.Current.MainWindow = _MainWindow;
            _MainWindow.Show();
            this.Close();
        }
        //===========================================================================================================
        private void Window_Closed(object sender, EventArgs e)
        {
        }
        //===========================================================================================================
    }
}