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
        public required string Nickname { get; set; }
        public required string Password { get; set; }

    }

    public partial class LoginWindow : Window
    {
        // standby 화면으로 넘기기위한 작업
        private MainWindow _mainWindow; // 단순히 필드 선언 (아직 객체 없음)
        private MySqlConnection _conn;  // MySQL 연결 변수 -> mainwindow에서 받아온다
        private LogInUserListWindow _LogInUserListWindow;
        private standbyScreen _standbyScreen;

        private int _userId;  // 선택한 사용자 이름을 저장하는 필드
        private string _selectPassword;  // 선택한 비밀번호를 저장하는 필드

        public LoginWindow(MainWindow mainWindow, MySqlConnection conn, LogInUserListWindow LogInUserListWindow, standbyScreen standbyScreen)
        {
            InitializeComponent();
            _mainWindow = mainWindow;  // MainWindow 객체를 받음
            _conn = conn;
            _LogInUserListWindow = LogInUserListWindow;
            _standbyScreen = standbyScreen;
        }

        private SelectUserData _userData = new SelectUserData // UserData 객체 생성
        {
            Nickname = "",  // 기본값 설정
            Password = ""
        };

        private void NicknameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.Nickname = NicknameTextBox.Text;
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
                SelectIdByNickname(_userData.Nickname);
                SelectIdByPassword(_userId);

                if (_userId == 0) 
                {
                    MessageBox.Show($"존재하지 않는 닉네임입니다.");
                    NicknameTextBox.Text = string.Empty;
                    PasswordBox.Password = string.Empty;
                    return;
                }

                if (_userData.Password != _selectPassword) 
                {
                    MessageBox.Show($"비밀번호가 일치하지 않습니다.\n {_userData.Password}, {_selectPassword}");
                    _userData.Password = string.Empty;
                    _selectPassword = string.Empty;
                    NicknameTextBox.Text = string.Empty;
                    PasswordBox.Password = string.Empty;
                    _userId = 0;
                    return;
                }

                if (_userData.Nickname == "roots")
                {
                    _mainWindow.rootsMethod();
                    this.Close();
                }
                else 
                {
                    // 로그인 성공 시
                    MessageBox.Show($"로그인에 성공하였습니다.");
                    if (_LogInUserListWindow == null)
                    {
                        MessageBox.Show($"현재 시스템은 오프상태입니다.");
                        this.Close();
                        return;
                    }
                    UpdateStateByLogIn(_userId);
                    _LogInUserListWindow.InjectOrder(_userId);
                    this.Close();
                }

            } 
            catch (Exception ex)
            {
                MessageBox.Show($"로그인 실패 \n {ex.Message}");
            }
        }

        // 닉네임으로 user_Id 가져오는 메서드
        private void SelectIdByNickname(string nickname)
        {
            string query = "" +
            "SELECT * FROM minic_db.userdata WHERE nick_name = @nick_name";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@nick_name", nickname);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        _userId = (int)dr["user_id"];
                    }
                }
            }
        }

        // user_id로 비밀번호 가져오기
        private void SelectIdByPassword(int userId)
        {
            string query = "" +
            "SELECT * FROM minic_db.userdata WHERE user_id = @user_Id";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_Id", userId);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        _selectPassword = (string)dr["password"];
                    }
                }
            }
        }

        // 로그인 시 입장상태와 게임가능상태 변경
        private void UpdateStateByLogIn(int userId)
        {
            string query = "UPDATE minic_db.userdata SET entry = true, game_possibility = true WHERE user_id = @user_Id";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                // ExecuteNonQuery는 데이터 변경(INSERT, UPDATE, DELETE) 작업에 사용
                cmd.Parameters.AddWithValue("@user_Id", userId);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("상태변경완료.");
                }
                else
                {
                    MessageBox.Show("상태변경실패");
                }
            }
        }
    }
}