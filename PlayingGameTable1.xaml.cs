using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
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
using static myProjectC.PlayingGameTable1;

namespace myProjectC
{
    /// <summary>
    /// PlayingGameTable1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlayingGameTable1 : Page
    {

        private Table1 table1;
        private MySqlConnection _conn;
        public int BilliardScore;
        private PlayGameUserData User1;
        private PlayGameUserData User2;
        private int scoreUser1;
        private int scoreUser2;
        private DateTime endTime;
        private TimeSpan difference;

        public PlayingGameTable1(MySqlConnection conn, Table1 _table1)
        {
            InitializeComponent();
            table1 = _table1;
            _conn = conn;
            LeftNickname.Content = table1.endGameUserNickname1;
            RightNickname.Content = table1.endGameUserNickname2;
            LeftUserScore.Content = scoreUser1;
            RightUserScore.Content = scoreUser2;
            User1 = SelectUserDataByNickname(table1.endGameUserNickname1);
            User2 = SelectUserDataByNickname(table1.endGameUserNickname2);
        }

        public class PlayGameUserData
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string NickName { get; set; }
            public int BilliardScore { get; set; }
        }

        // 상대방 점수 마이너스 해주는 메서드
        private void LeftScoreOneMinusButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser2--;
            RightUserScore.Content = scoreUser2;
        }

        private void RightScoreOneMinusButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser1--;
            LeftUserScore.Content = scoreUser1;
        }

        // 상대바의 점수를 올려주는 메서드
        private void RightScoreOneButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser1++;
            LeftUserScore.Content = scoreUser1;
            scoreFillEndGame(scoreUser1, User1.BilliardScore);
        }

        private void RightScoreTwoButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser1 += 2;
            LeftUserScore.Content = scoreUser1;
            scoreFillEndGame(scoreUser1, User1.BilliardScore);
        }

        private void LeftScoreOneButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser2++;
            RightUserScore.Content = scoreUser2;
            scoreFillEndGame(scoreUser2, User2.BilliardScore);
        }

        private void LeftScoreTwoButton_Click(object sender, RoutedEventArgs e)
        {
            scoreUser2 += 2;
            RightUserScore.Content = scoreUser2;
            scoreFillEndGame(scoreUser2, User2.BilliardScore);
        }


        // 게임 종료 버튼 -> 예기치 않은 상황에서 게임종료를 했을 때 그에 대한 정보를 남길지 말지 정하자.
        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            EndGameMethod();
        }

        // 게임 종료시 메서드
        private void EndGameMethod()
        {
            // 게임 시간 측정
            endTime = DateTime.Now;
            difference = endTime - table1.startTime;
            double minutes = Math.Ceiling(difference.TotalMinutes);
            

            MessageBox.Show($"{table1.endGameUserNickname1}님과 {table1.endGameUserNickname2}님의 게임을 종료합니다!");
            MessageBox.Show($"점수: {scoreUser1}, {scoreUser2}");
            MessageBox.Show($"게임시간: {minutes}분");
            table1.UpdateStateByPlayGame(table1.endGameUserNickname1, true);
            table1.UpdateStateByPlayGame(table1.endGameUserNickname2, true);
            table1.UpdateTableStateByPlayGame(table1.tableList[0].tableId, true);
            table1.standbyScreen.RefreshTableDisplay();
            table1.UpdateUserStateByPlayGame(table1.endGameUserNickname1, true);
            table1.UpdateUserStateByPlayGame(table1.endGameUserNickname2, true);
            table1.logInUserListWindow.LoadUserList(table1.logInUserListWindow._userList);

            // 화면 전환
            table1.Table1Grid.Visibility = Visibility.Visible;

            // Frame에서 PlayingGameTable1을 제거하고, Table1로 돌아갑니다.
            table1.Table1Frame.Navigate(typeof(PlayingGameTable1));
            table1.UserNicknameTextBox1.Text = string.Empty;
            table1.UserNicknameTextBox2.Text = string.Empty;
        }

        // 점수가 자기 수지에 맞거나 넘치면 그 수지를 기준으로 게임 끝내는 메서드
        private void scoreFillEndGame (int score, int billiardScoreUser) 
        {
            if (score >= billiardScoreUser) 
            {
                if (billiardScoreUser == User1.BilliardScore)
                {
                    scoreUser1 = billiardScoreUser;
                    LeftUserScore.Content = scoreUser1;
                }
                else if (billiardScoreUser == User2.BilliardScore)
                {
                    scoreUser2 = billiardScoreUser;
                    RightUserScore.Content = scoreUser2;
                }
                EndGameMethod();
                return;
            }
        }

        // 닉네임으로 유저 정보 가져오는 메서드
        private PlayGameUserData SelectUserDataByNickname(string nickname)
        {
            PlayGameUserData playGameUserData = new PlayGameUserData();

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
                        playGameUserData.UserId = (int)dr["user_id"];
                        playGameUserData.UserName = (string)dr["user_name"];
                        playGameUserData.NickName = (string)dr["nick_name"];
                        playGameUserData.BilliardScore = (int)dr["billiard_score"];
                    }
                }
            }
            return playGameUserData;
        }

        



        // 경기기록을 db에 저장하는 메서드 -> 필요한 것 내 아이디, 상대방 아이디, 게임시간, 게임날짜, 게임이닝 추가

        //private void InsertGameRecord(int playerId, int opponentId, string winLoss, gameDuration) 
        //{

        //}
    }
}
