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
using System.Windows.Shapes;

namespace myProjectC
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>

    // 유저 데이터 클래스
    public class UserData
    {
        // required null값을 허용 X
        public required string UserName { get; set; }
        public required string NickName { get; set; }
        public required string Password { get; set; }
        public required string RePassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required int BilliardScore { get; set; }
    }


    public partial class signUpWindow : Window
    {
        private MySqlConnection _conn;  // MySQL 연결 변수 -> mainwindow에서 받아온다
        private string _existedNickname; // 닉네임 존재여부확인을 위한 변수 

        // 생성자에서 MySQL 연결을 받아옴
        public signUpWindow(MySqlConnection conn)
        {
            InitializeComponent();
            _conn = conn;
        }

        private UserData _userData = new UserData // UserData 객체 생성
        {
            UserName = "",  // 기본값 설정
            NickName = "",
            Password = "",
            RePassword = "",
            PhoneNumber = "",
            BilliardScore = 0
        };

        // UserData 클래스의 Username 속성에 TextBox에서 입력된 값을 저장
        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.UserName = UsernameTextBox.Text;
        }
        // UserData 클래스의 Nickname 속성에 TextBox에서 입력된 값을 저장
        private void NicknameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.NickName = NicknameTextBox.Text;
        }

        // UserData 클래스의 Password 속성에 TextBox에서 입력된 값을 저장
        private void PasswordBox_TextChanged(object sender, RoutedEventArgs e)
        {
            _userData.Password = PasswordBox.Password;
        }
        // UserData 클래스의 RePassword 속성에 TextBox에서 입력된 값을 저장
        private void RePasswordBox_TextChanged(object sender, RoutedEventArgs e)
        {
            _userData.RePassword = RePasswordBox.Password;
        }
        // UserData 클래스의 PhoneNumber 속성에 TextBox에서 입력된 값을 저장
        private void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.PhoneNumber = PhoneNumber.Text;
        }
        // UserData 클래스의 BilliardScore 속성에 TextBox에서 입력된 값을 저장
        private void BilliardScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.BilliardScore = int.Parse(BilliardScore.Text);
        }

        // 회원가입 클릭 버튼
        private void signUp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Username: {_userData.UserName}\nNickname: {_userData.NickName}\nPassword: {_userData.Password}\nRePassword: {_userData.RePassword}\nPhoneNumber: {_userData.PhoneNumber}\nBilliardScore: {_userData.BilliardScore}");

            // 문자열이 비어 있는지 확인 메서드 -> string.IsNullOrEmpty() 또는 string.IsNullOrWhiteSpace() -> boolean타입 반환
            if (string.IsNullOrWhiteSpace(_userData.UserName) ||
                string.IsNullOrWhiteSpace(_userData.NickName) ||
                string.IsNullOrWhiteSpace(_userData.Password) ||
                string.IsNullOrWhiteSpace(_userData.RePassword) ||
                string.IsNullOrWhiteSpace(_userData.PhoneNumber) ||
                _userData.BilliardScore == 0)
            {
                MessageBox.Show("항목에 알맞게 입력해주세요");
                return;
            }

            IsNickName(_userData.NickName);
            if (!string.IsNullOrWhiteSpace(_existedNickname)) 
            {
                MessageBox.Show("이미 존재하는 닉네임입니다.");
                NicknameTextBox.Text = string.Empty;
                // 존재하는 닉네임을 초기화하지 않으면 그대로 처음 중복 닉네임이
                // 그대로 남아있기 때문에 닉네임 바꿔도
                // 이미 존재하는 닉네임입니다. 메시지가 계속 뜸.
                _existedNickname = string.Empty;
                return;
            }

            if (_userData.Password != _userData.RePassword)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
                PasswordBox.Password = string.Empty;  // 비밀번호 입력 필드를 초기화
                RePasswordBox.Password = string.Empty; // 확인용 비밀번호 필드를 초기화
                return;
            }
            
            // 모든 항목이 올바르게 입력된 경우 처리
            MessageBox.Show("회원가입이 완료되었습니다.");
            try
            {
                Insert(_userData.UserName, _userData.Password, _userData.PhoneNumber, _userData.BilliardScore, _userData.NickName);
                MessageBox.Show($"DB에 입력 성공");
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"DB에 입력 실패 \n {ex.Message}");
            }
        }

        private void Insert(string userName, string password, string phoneNumber, int billiardScore, string nickName)
        {
            string query = "" +
                "INSERT INTO minic_db.userdata (user_name, password, phone_number, billiard_score, nick_name) " +
                "VALUES (@userName, @password, @phoneNumber, @billiardScore, @nickName);";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                // 이렇게 입력하면 타입을 찾아가기 때문에 시간이 좀 더 걸린다고 함.
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@billiardScore", billiardScore);
                cmd.Parameters.AddWithValue("@nickName", nickName);

                cmd.ExecuteNonQuery();  // 데이터베이스에 명령 실행
            }
        }

        private void IsNickName(string nickName) 
        {
            string query = "" +
            "SELECT * FROM minic_db.userdata WHERE nick_name = @nick_name";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@nick_name", nickName);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        _existedNickname = (string)dr["nick_name"];
                    }
                }
            }
        }
    }
}
