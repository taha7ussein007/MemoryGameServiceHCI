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
using System.Runtime.InteropServices;
using System.Security;
namespace MemoryGameServiceClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Back_Main_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow _MainWindow = new MainWindow();
            App.Current.MainWindow = _MainWindow;
            _MainWindow.Show();
            this.Close();
        }

        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {
            string pass = SecureStringToString(User_Pass_txt.SecurePassword);

            if (Email_UserName_txt.Text.ToString() == "" || pass == "")
            {
                MessageBox.Show("Please enter your Username/Email and Password!", "Required Info.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (globalVars.offlineGameController.login(Email_UserName_txt.Text, pass))
            {
                //Get And Save Player Data.
                globalVars.player = new GamePlayer(Email_UserName_txt.Text);
                //Switch Window
                CreateJoinWindow _CreateJoinWindow = new CreateJoinWindow();
                App.Current.MainWindow = _CreateJoinWindow;
                _CreateJoinWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Username/Email or Password!", "Invalid Info.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Forget_Pass_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("phase three");
        }

        private String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
