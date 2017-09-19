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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MemoryGameService;
using System.Text.RegularExpressions;

namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async Task<bool> GetUserName()
        {
            string nameValue = this.UserName_txt.Text;
            bool isValid = false;
            Regex regex = new Regex 
            (
            "^[a-zA-Z][a-zA-Z\\s]|[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]+$",
            RegexOptions.IgnoreCase
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );
            await Task.Run(() => 
            {
                if (regex.IsMatch(nameValue))
                {
                    globalVars.userName_mainWindow = nameValue;
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            });
            return isValid;
        }
        private async void StartSingleGame_btn_Click(object sender, RoutedEventArgs e)
        {
            string holdContent = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                StartSingleGame_btn.IsEnabled = false;
                StartMultGame_btn.IsEnabled = false;
                holdContent = StartSingleGame_btn.Content.ToString();
                StartSingleGame_btn.Content = "Please Wait...";
            }));
            if (await GetUserName())
            {
                globalVars.offlineGame = true; //offline mode
                GameWindow _GameWindow = new GameWindow();
                App.Current.MainWindow = _GameWindow;
                _GameWindow.Show();
                this.Close();
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    StartSingleGame_btn.IsEnabled = true;
                    StartMultGame_btn.IsEnabled = true;
                    StartSingleGame_btn.Content = holdContent;
                }));
                MessageBox.Show("Please Enter A Valid Name First!","Required Info.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void StartMultGame_btn_Click(object sender, RoutedEventArgs e)
        {
            string holdContent = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                StartSingleGame_btn.IsEnabled = false;
                StartMultGame_btn.IsEnabled = false;
                holdContent = StartMultGame_btn.Content.ToString();
                StartMultGame_btn.Content = "Please Wait...";
            }));
            if (await GetUserName())
            {
                globalVars.offlineGame = false; //online mode
                LoginWindow _LoginWindow = new LoginWindow();
                App.Current.MainWindow = _LoginWindow;
                _LoginWindow.Show();
                this.Close();
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    StartSingleGame_btn.IsEnabled = true;
                    StartMultGame_btn.IsEnabled = true;
                    StartMultGame_btn.Content = holdContent;
                }));
                MessageBox.Show("Please Enter A Valid Name First!", "Required Info.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
