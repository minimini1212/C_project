using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace myProjectC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // mysql 연결
        private MySqlConnection _conn = default;

        private void Connection()
        {
            string connetionString = "UID=mini;PWD=mini;Server=127.0.0.1;Port=3306;Database=minic_db";
        
            try {
                _conn = new MySqlConnection(connetionString);
                _conn.Open();
                MessageBox.Show($"Connection 성공");
            }
            catch (Exception ex) {
                MessageBox.Show($"Connection 실패 \n {ex.Message}");
            }
        }

        private LogInUserListWindow _LogInUserListWindow;
        private standbyScreen _standbyScreen;
        private TableInPlay _TableInPlay;
        private Table1 _Table1;

        public MainWindow()
        {
            InitializeComponent();
            Connection();
            
        }

        // 관리자 로그인 시 실행할 것들
        public void rootsMethod()
        {
            _LogInUserListWindow = new LogInUserListWindow(_conn);
            _standbyScreen = new standbyScreen(_conn);
            _TableInPlay = new TableInPlay(_conn);
            _Table1 = new Table1(_conn, _standbyScreen, _LogInUserListWindow);

            _LogInUserListWindow.Show();
            _standbyScreen.Show();
            _TableInPlay.Show();
            _Table1.Show();
        }

        private void signUp(object sender, RoutedEventArgs e)
        {
            signUpWindow signUpWindow = new signUpWindow(_conn);  // _conn을 전달 -> mysql 연결
            signUpWindow.Show();
        }

        private void signIn(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(this, _conn, _LogInUserListWindow, _standbyScreen); 
            loginWindow.Show();
        }
    }
};