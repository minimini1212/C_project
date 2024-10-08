using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.X509;
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
using static myProjectC.standbyScreen;

namespace myProjectC
{
    /// <summary>
    /// Table1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Table1 : Window
    {

        public MySqlConnection _conn;
        public List<TableData> tableList;
        public standbyScreen standbyScreen;
        public LogInUserListWindow logInUserListWindow;
        public PlayingGameTable1 _PlayingGameTable1;
        public DateTime startTime;
        public string FirstAttackUser;

        public Table1(MySqlConnection conn, standbyScreen _standbyScreen, LogInUserListWindow _LogInUserListWindow)
        {
            InitializeComponent();
            _conn = conn;
            standbyScreen = _standbyScreen;
            tableList = TableList._tableListData;
            logInUserListWindow = _LogInUserListWindow;

            //// Touch 이벤트 추가
            //Touch.FrameReported += Touch_FrameReported;
        }

        public string endGameUserNickname1;
        public string endGameUserNickname2;
        public bool _entry;

        // 게임 시작 버튼
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            startTime = DateTime.Now;
            string userNickname1 = UserNicknameTextBox1.Text;
            string userNickname2 = UserNicknameTextBox2.Text;
            endGameUserNickname1 = userNickname1;
            endGameUserNickname2 = userNickname2;

            // 로그인 여부
            IsLoginUSer(userNickname1);
            IsLoginUSer(userNickname2);

            if (!string.IsNullOrWhiteSpace(userNickname1) && !string.IsNullOrWhiteSpace(userNickname2))
            { 
                MessageBox.Show($"{userNickname1}님과 {userNickname2}님이 게임을 시작합니다!");
                UpdateStateByPlayGame(userNickname1, false);
                UpdateStateByPlayGame(userNickname2, false);
                UpdateTableStateByPlayGame(tableList[0].tableId, false);
                standbyScreen.RefreshTableDisplay();
                UpdateUserStateByPlayGame(userNickname1, false);
                UpdateUserStateByPlayGame(userNickname2, false);
                logInUserListWindow.LoadUserList(logInUserListWindow._userList);

                // 초기 화면 숨기기
                Table1Grid.Visibility = Visibility.Collapsed;

                _PlayingGameTable1 = new PlayingGameTable1(_conn, this);
                // standbyScreen 페이지를 Frame에 로드
                Table1Frame.Navigate(_PlayingGameTable1);

            }
            else
            {
                MessageBox.Show("이름을 입력해주세요.");
            }
        }


        // 게임 시작 종료 시 유저 상태 변경
        public void UpdateStateByPlayGame(string userNickname, bool status)
        {
            string query = "UPDATE minic_db.userdata SET game_possibility = @game_possibility WHERE nick_name = @nick_name";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                // ExecuteNonQuery는 데이터 변경(INSERT, UPDATE, DELETE) 작업에 사용
                cmd.Parameters.AddWithValue("@nick_name", userNickname);
                cmd.Parameters.AddWithValue("@game_possibility", status);
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

        // 게임 시작 종료 시 테이블 상태 변경
        public void UpdateTableStateByPlayGame(int tableId, bool status)
        {
            string query = "UPDATE minic_db.game_table SET is_available = @is_available WHERE table_id = @table_id";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                // ExecuteNonQuery는 데이터 변경(INSERT, UPDATE, DELETE) 작업에 사용
                cmd.Parameters.AddWithValue("@table_id", tableId);
                cmd.Parameters.AddWithValue("@is_available", status);
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

        // 게임 시작 종료 시 유저 리스트 업데이트
        public void UpdateUserStateByPlayGame(string nickname, bool status)
        {
            foreach (var user in logInUserListWindow._userList)
            {
                // 게임 시작 시
                if (user._nickName == nickname && user._gamePossibility == true)
                {
                    user._gamePossibility = status;
                    return;
                }

                // 게임 종료 시
                if (user._nickName == nickname && user._gamePossibility == false)
                {
                    user._gamePossibility = status;
                    user._order = ++logInUserListWindow.order;
                    return;
                }
            }
        }

        // 게임 시작하려고 할 때 입력한 닉네임이 로그인 된 회원인지 파악하는 메서드
        private void IsLoginUSer(string nickName) 
        {
            MessageBox.Show("여기로 들어옴??");
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
                        _entry = (bool)dr["entry"];
                    }
                }
            }

            if (!_entry) 
            {
                MessageBox.Show("로그인 후 게임을 이용해주세요");
                return;
            }
        }

        private void FirstAttackButtonUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserNicknameTextBox1.Text))
            {
                MessageBox.Show($"닉네임을 입력하세요");
            } 
            else 
            {
                FirstAttackUser = UserNicknameTextBox1.Text;
                MessageBox.Show($"{FirstAttackUser} 님이 선공입니다");
            }
        }

        private void FirstAttackButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserNicknameTextBox2.Text))
            {
                MessageBox.Show($"닉네임을 입력하세요");
            }
            else
            {
                FirstAttackUser = UserNicknameTextBox2.Text;
                MessageBox.Show($"{FirstAttackUser} 님이 선공입니다");
            }
        }



        //private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        //{
        //    // 터치 이벤트 처리 로직 추가
        //    foreach (TouchPoint touchPoint in e.GetTouchPoints(this))
        //    {
        //        // 터치된 포인트에 대한 처리
        //        if (touchPoint.Action == TouchAction.Down)
        //        {
        //            // 터치 시작 시의 로직
        //        }
        //    }
        //}
    }
}
