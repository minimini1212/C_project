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
using System.Windows.Shapes;
using static myProjectC.standbyScreen;

namespace myProjectC
{
    /// <summary>
    /// LogInUserListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogInUserListWindow : Window
    {
        private MySqlConnection _conn; // MySQL 연결을 위한 필드 추가
        public List<LogInUser> _userList = new List<LogInUser>();
        public int order;

        public LogInUserListWindow(MySqlConnection conn)
        {
            InitializeComponent();
            _conn = conn;
            //LogInUserList(); // 로그인 한 유저 명단 조회
            //LoadUserList(_userList);
        }

        //public static class UserList 
        //{
        //    public static List<LogInUser> userList = new List<LogInUser>();
        //}

        public class LogInUser 
        {
            public int _userId { get; set; }
            public string _userName { get; set; }
            public string _nickName { get; set; }
            public int _score { get; set; }
            // 입장순서
            public int _order { get; set; }
            public bool _gamePossibility { get; set; }
        }


        // 로그인하고 게임 가능한 유저 명단 조회하기
        private void LogInUserList(int _userId)
        {
            //_userList.Clear();

            //string query = "" +
            //"SELECT * FROM minic_db.userdata WHERE entry = 1";

            string query = "" +
            "SELECT * FROM minic_db.userdata WHERE entry = 1 and game_possibility = true and user_id = @user_id";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_id", _userId);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        LogInUser user = new LogInUser
                        {
                            _userId = (int)dr["user_id"],
                            _userName = (string)dr["user_name"],
                            _nickName = (string)dr["nick_name"],
                            _score = (int)dr["billiard_score"],
                            _gamePossibility = (bool)dr["game_possibility"]
                        };
                        _userList.Add(user);
                    }
                }
            }
        }

        public void LoadUserList(List<LogInUser> _userListLoad) 
        {
            // 정렬
            _userListLoad.Sort((x, y) => x._order.CompareTo(y._order));

            LogInUserGrid.Children.Clear(); // 화면 초기화

            foreach (var user in _userListLoad) 
            {
                // game_possibility 가 false면 명단에 안뜨게
                if (!user._gamePossibility) 
                {
                    continue;
                }

                // 새로운 테이블을 나타내는 Label을 생성
                MessageBox.Show($"{user._userName}, {user._order}");
                Label LogInUser = new Label
                {
                    Content = $"회원 이름: {user._userName}\n회원 점수: {user._score}\n순서: {user._order}",
                    Background = Brushes.LightBlue,
                    Margin = new Thickness(5),
                    Width = 300, // 너비 설정
                    Height = 100, // 높이 설정
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                // UniformGrid에 추가
                LogInUserGrid.Children.Add(LogInUser);
                
            }
        }

        // 입장 순서대로 유저에게 순서를 부여하는 메서드
        public void InjectOrder(int userId) 
        {
            LogInUserList(userId);

            foreach (var user in _userList)
            {
                if (user._userId == userId) 
                {
                    user._order = ++order;
                }
            }
            LoadUserList(_userList);
            //UserList.userList = _userList;
        }
    }
}
