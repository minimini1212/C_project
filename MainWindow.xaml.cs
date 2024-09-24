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

namespace myProjectC;

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

    public MainWindow()
    {
        InitializeComponent();
        Connection();
    }

    // 로그인 후 standbyScreen화면으로 넘기기 위한 메서드
    public void LoadStandbyScreen()
    {

        // 초기 화면 숨기기
        InitialGrid.Visibility = Visibility.Collapsed;

        // standbyScreen 페이지를 Frame에 로드
        MainFrame.Navigate(new standbyScreen(_conn));
    }

    private void signUp(object sender, RoutedEventArgs e)
    {
        signUpWindow signUpWindow = new signUpWindow(_conn);  // _conn을 전달 -> mysql 연결
        signUpWindow.Show();
    }


    private void signIn(object sender, RoutedEventArgs e)
    {
        LoginWindow loginWindow = new LoginWindow(this, _conn);
        loginWindow.Show();
    }
}