using MySql.Data.MySqlClient;
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

namespace myProjectC
{
    /// <summary>
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>

    // 유저 데이터 클래스
    public class SelectUserData
    {
        // required null값을 허용 X
        public required string UserName { get; set; }
        public required string Password { get; set; }

    }

    public partial class LoginWindow : Window
    {
        // standby 화면으로 넘기기위한 작업
        private MainWindow _mainWindow; // 단순히 필드 선언 (아직 객체 없음)
        private MySqlConnection _conn;  // MySQL 연결 변수 -> mainwindow에서 받아온다

        private string _selectName;  // 선택한 사용자 이름을 저장하는 필드
        private string _selectPassword;  // 선택한 비밀번호를 저장하는 필드

        public LoginWindow(MainWindow mainWindow, MySqlConnection conn)
        {
            InitializeComponent();
            _mainWindow = mainWindow;  // MainWindow 객체를 받음
            _conn = conn;
        }

        private SelectUserData _userData = new SelectUserData // UserData 객체 생성
        {
            UserName = "",  // 기본값 설정
            Password = ""
        };

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.UserName = UsernameTextBox.Text;
        }

        private void PasswordBox_TextChanged(object sender, RoutedEventArgs e)
        {
            _userData.Password = PasswordBox.Password;
        }

        // 로그인 버튼 클릭
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Select(_userData.UserName, _userData.Password);
                MessageBox.Show($"{_selectName}, {_selectPassword}");
                // 로그인 성공 시 MainWindow에서 standbyScreen 페이지로 이동
                _mainWindow.LoadStandbyScreen();
                this.Close();
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"로그인 실패 \n {ex.Message}");
            }
        }

        private void Select(string userName, string password)
        {
            string query = "" +
            "SELECT * FROM minic_db.userdata WHERE user_name = @user_name and password = @password";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_name", userName);
                cmd.Parameters.AddWithValue("@password", password);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        _selectName = (string)dr["user_name"];
                        _selectPassword = (string)dr["password"];
                        //var phone_number = (string)dr["phone_number"];
                        //var billiard_score = (int)dr["billiard_score"];
                        // ---start
                        // data1 => true
                        // data2 => true
                        // data3 => true
                        // ---end => false
                    }
                }
            }
        }  
    }
}