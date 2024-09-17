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

    // required를 사용하지 않을 때
    //public class UserData
    //{
    //    public string UserName { get; set; } = string.Empty; // 기본값 설정
    //    public string Password { get; set; } = string.Empty;
    //    public string RePassword { get; set; } = string.Empty;
    //    public string PhoneNumber { get; set; } = string.Empty;
    //    public string BillibordScore { get; set; } = string.Empty;
    //}

    // 유저 데이터 클래스
    public class UserData
    {
        // required null값을 허용 X
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string RePassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required int BilliardScore { get; set; }
    }


    public partial class signUpWindow : Window
    {
        private MySqlConnection _conn;  // MySQL 연결 변수 -> mainwindow에서 받아온다

        // 생성자에서 MySQL 연결을 받아옴
        public signUpWindow(MySqlConnection conn)
        {
            InitializeComponent();
            _conn = conn;
        }

        private UserData _userData = new UserData // UserData 객체 생성
        {
            UserName = "",  // 기본값 설정
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
        // UserData 클래스의 Password 속성에 TextBox에서 입력된 값을 저장
        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.Password = PasswordBox.Text;
        }
        // UserData 클래스의 RePassword 속성에 TextBox에서 입력된 값을 저장
        private void RePasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _userData.RePassword = RePasswordBox.Text;
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
            MessageBox.Show($"Username: {_userData.UserName}\nPassword: {_userData.Password}\nRePassword: {_userData.RePassword}\nPhoneNumber: {_userData.PhoneNumber}\nBilliardScore: {_userData.BilliardScore}");

            // 문자열이 비어 있는지 확인 메서드 -> string.IsNullOrEmpty() 또는 string.IsNullOrWhiteSpace() -> boolean타입 반환
            if (string.IsNullOrWhiteSpace(_userData.UserName) ||
                string.IsNullOrWhiteSpace(_userData.Password) ||
                string.IsNullOrWhiteSpace(_userData.RePassword) ||
                string.IsNullOrWhiteSpace(_userData.PhoneNumber) ||
                _userData.BilliardScore == 0)
            {
                MessageBox.Show("항목에 알맞게 입력해주세요");
            }
            else
            {
                // 모든 항목이 올바르게 입력된 경우 처리
                MessageBox.Show("회원가입이 완료되었습니다.");
                try
                {
                    Insert(_userData.UserName, _userData.Password, _userData.PhoneNumber, _userData.BilliardScore);
                    MessageBox.Show($"DB에 입력 성공");
                    this.Close();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DB에 입력 실패 \n {ex.Message}");
                }
                
            }

   
        }

        private void Insert(string userName, string password, string phoneNumber, int billiardScore)
        {



            string query = "" +
                "INSERT INTO minic_db.userdata (user_name, password, phone_number, billiard_score) " +
                "VALUES (@userName, @password, @phoneNumber, @billiardScore);";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add("@userName", MySqlDbType.VarChar);
                cmd.Parameters.Add("@password", MySqlDbType.VarChar);
                cmd.Parameters.Add("@phoneNumber", MySqlDbType.VarChar);
                cmd.Parameters.Add("@billiardScore", MySqlDbType.Int32);

                cmd.Parameters["@userName"].Value = userName;
                cmd.Parameters["@password"].Value = password;
                cmd.Parameters["@phoneNumber"].Value = phoneNumber;
                cmd.Parameters["@billiardScore"].Value = billiardScore;

                cmd.ExecuteNonQuery();  // 데이터베이스에 명령 실행
            }
        }

    }
}
