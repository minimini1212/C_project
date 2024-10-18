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
// 게임시간 설정을 위함
using System.Windows.Threading;
using System.Collections;

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
        private int InningNumber = 1;
        private int count;
        private string attackUser;
        private DispatcherTimer timer;
        private DateTime today = DateTime.Today;


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
            FirstAttackAndStartBackground();
            StartGame();
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
            // 테이블 및 유저 상태 변경 메서드 및 화면 초기화
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
            // 게임 시간 측정
            endTime = DateTime.Now;
            difference = endTime - table1.startTime;
            double minutes = Math.Ceiling(difference.TotalMinutes);

            if (score >= billiardScoreUser) 
            {

                MessageBox.Show($"{table1.endGameUserNickname1}님과 {table1.endGameUserNickname2}님의 게임을 종료합니다!");
                MessageBox.Show($"점수: {scoreUser1}, {scoreUser2}");
                MessageBox.Show($"게임시간: {minutes}분");

                try
                {
                    if (billiardScoreUser == User1.BilliardScore)
                    {
                        scoreUser1 = billiardScoreUser;
                        LeftUserScore.Content = scoreUser1;

                        InsertGameRecord(User1.UserId, User2.UserId, minutes, today.ToString("yyyy-MM-dd"), scoreUser1,"win", InningNumber);
                        InsertGameRecord(User2.UserId, User1.UserId, minutes, today.ToString("yyyy-MM-dd"), scoreUser2,"loss", InningNumber);
                    }
                    else if (billiardScoreUser == User2.BilliardScore)
                    {
                        InsertGameRecord(User2.UserId, User1.UserId, minutes, today.ToString("yyyy-MM-dd"), scoreUser2, "win", InningNumber);
                        InsertGameRecord(User1.UserId, User2.UserId, minutes, today.ToString("yyyy-MM-dd"), scoreUser1, "loss", InningNumber);
                        scoreUser2 = billiardScoreUser;
                        RightUserScore.Content = scoreUser2;
                    }
                    MessageBox.Show($"DB에 입력 성공");
                    EndGameMethod();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"DB에 입력 실패 \n {ex.Message}");
                }

                
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


        // 선공한테 흰생으로 점수배경색 변경 및 공격표시 띄워주기 및 게임화면 초기화 메서드
        private void FirstAttackAndStartBackground() 
        {

            Inning.Content = $"{InningNumber} 이닝";

            attackUser = table1.FirstAttackUser;

            if (table1.FirstAttackUser == table1.endGameUserNickname1)
            {
                LeftAttack.Visibility = Visibility.Visible;
                LeftUserScore.Background = new SolidColorBrush(Colors.GreenYellow);
                LeftScoreOneMinus.Visibility = Visibility.Hidden;
                LeftScoreOne.Visibility = Visibility.Hidden;
                LeftScoreTwo.Visibility = Visibility.Hidden;
                
            }
            else if (table1.FirstAttackUser == table1.endGameUserNickname2)
            {
                RightAttack.Visibility = Visibility.Visible;
                RightUserScore.Background = new SolidColorBrush(Colors.GreenYellow);
                RightScoreOneMinus.Visibility = Visibility.Hidden;
                RightScoreOne.Visibility = Visibility.Hidden;
                RightScoreTwo.Visibility = Visibility.Hidden;
            }
        }

        // 턴 넘기기 버튼
        private void TurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (attackUser == table1.endGameUserNickname1)
            {
                LeftAttack.Visibility = Visibility.Hidden;
                LeftUserScore.Background = new SolidColorBrush(Colors.Yellow);
                RightAttack.Visibility = Visibility.Visible;
                RightUserScore.Background = new SolidColorBrush(Colors.GreenYellow);
                LeftScoreOneMinus.Visibility = Visibility.Visible;
                LeftScoreOne.Visibility = Visibility.Visible;
                LeftScoreTwo.Visibility = Visibility.Visible;
                RightScoreOneMinus.Visibility = Visibility.Hidden;
                RightScoreOne.Visibility = Visibility.Hidden;
                RightScoreTwo.Visibility = Visibility.Hidden;
                attackUser = table1.endGameUserNickname2;
            }
            else
            {
                RightAttack.Visibility = Visibility.Hidden;
                RightUserScore.Background = new SolidColorBrush(Colors.Yellow);
                LeftAttack.Visibility = Visibility.Visible;
                LeftUserScore.Background = new SolidColorBrush(Colors.GreenYellow);
                RightScoreOneMinus.Visibility = Visibility.Visible;
                RightScoreOne.Visibility = Visibility.Visible;
                RightScoreTwo.Visibility = Visibility.Visible;
                LeftScoreOneMinus.Visibility = Visibility.Hidden;
                LeftScoreOne.Visibility = Visibility.Hidden;
                LeftScoreTwo.Visibility = Visibility.Hidden;
                attackUser = table1.endGameUserNickname1;
            }

            ++count;
            if (count % 2 == 0) 
            {
                ++InningNumber;
                Inning.Content = $"{InningNumber} 이닝";
            }
        }

        // 게임 시작 시 게임 시간 설정
        public void StartGame()
        {
            // 타이머 초기화 및 설정
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // 1초마다 호출
            timer.Tick += Timer_Tick; // Tick 이벤트 연결
            timer.Start(); // 타이머 시작
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 현재 시간에서 시작 시간을 뺀 값을 구해 경과 시간을 계산
            TimeSpan elapsedTime = DateTime.Now - table1.startTime;

            // 경과 시간을 원하는 형식으로 표시
            GameTime.Content = $"{Math.Ceiling(elapsedTime.TotalMinutes)}분";
        }


        // 이건 굳이 필요없지만 공부를 위해 남겨두자

        //public void StopGame()
        //{
        //    if (timer != null)
        //    {
        //        timer.Stop(); // 타이머 중지
        //    }
        //}



        // 경기기록을 db에 저장하는 메서드 -> 필요한 것 내 아이디, 상대방 아이디, 게임시간, 게임날짜, 게임이닝 추가

        private void InsertGameRecord(
            int playerId, 
            int opponentId, 
            double gameDuration,
            string gameDate, 
            int totalScore,
            string winLoss,
            int inning)
        {
            string query = "" +
                "INSERT INTO minic_db.game_records (player_id, " +
                                                  "opponent_id, " +
                                                  "total_score, " +
                                                  "game_duration, " +
                                                  "game_date, " +
                                                  "win_loss, " +
                                                  "inning) " +
                "VALUES (@playerId, @opponentId, @totalScore, @gameDuration, @gameDate, @winLoss, @inning);";   

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@playerId", playerId);
                cmd.Parameters.AddWithValue("@opponentId", opponentId);
                cmd.Parameters.AddWithValue("@winLoss", winLoss);
                cmd.Parameters.AddWithValue("@gameDuration", gameDuration);
                cmd.Parameters.AddWithValue("@gameDate", gameDate);
                cmd.Parameters.AddWithValue("@totalScore", totalScore);
                cmd.Parameters.AddWithValue("@inning", inning);
                cmd.ExecuteNonQuery();  // 데이터베이스에 명령 실행
            }
        }
    }
}
