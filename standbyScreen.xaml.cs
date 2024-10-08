using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// standbyScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class standbyScreen : Window
    {

        private MySqlConnection _conn; // MySQL 연결을 위한 필드 추가
        private string tableStatus; // 테이블 상태

        public standbyScreen(MySqlConnection conn)
        {
            InitializeComponent();
            _conn = conn; // 먼저 _conn을 초기화
            SelectTable(); // 그 후에 SelectTable 호출 -> 이렇게 해야 _conn기 초기화 되기 때문에 예외상황이 발생X
        }

        // 지금 당장 사용하지는 않지만 좋은 공부가 된 부분
        private List<TableData> _tableList = new List<TableData>(); // 사용자 데이터를 저장할 리스트
        
        // 각각의 당구테이블에서 사용할 테이블정보
        public static class TableList
        {
            public static List<TableData> _tableListData = new List<TableData>();
        }

        public class TableData 
        {
            public int tableId { get; set; }
            public bool isAvailable { get; set; }
        }

        // 테이블 추가 버튼
        private void AddTable_Button_Click(object sender, RoutedEventArgs e)
        {
            InsertTable();
            RefreshTableDisplay(); // 화면을 새로 고침
        }

        // 당구테이블 추가
        private void InsertTable()
        {
            string query = "" +
                "INSERT INTO minic_db.game_table (is_available) " +
                "VALUES (@is_available);";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                // 이렇게 입력하면 타입을 찾아가기 때문에 시간이 좀 더 걸린다고 함.
                cmd.Parameters.AddWithValue("@is_available", true);


                cmd.ExecuteNonQuery();  // 데이터베이스에 명령 실행
            }
        }

        // 테이블 화면 새로 고침
        public void RefreshTableDisplay()
        {
            TableGrid.Children.Clear(); // 기존의 테이블 레이블을 모두 제거
            _tableList.Clear(); // 테이블 리스트 초기화
            SelectTable(); // 새로운 데이터 가져오기
        }

        // 당구 테이블 정보 조회
        private void SelectTable()
        {
            string query = "" +
            "SELECT * FROM minic_db.game_table";

            using (MySqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = query;
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TableData table = new TableData
                        {
                            tableId = (int)dr["table_id"],
                            isAvailable = (bool)dr["is_available"]
                        };

                        _tableList.Add(table);
                        TableList._tableListData.Add(table);

                        if (table.isAvailable) {
                            tableStatus = "게임 가능";
                        } 
                        else if (!table.isAvailable) 
                        {
                            tableStatus = "게임 중";
                        }

                        // 새로운 테이블을 나타내는 Label을 생성
                        Label newTable = new Label
                        {   //이렇게 하니까 id가 자동생성이라 다 지우고 생성하면 1이 아닌 상태로 시작해버림..
                            //Content = $"테이블 번호: {table.tableId}, 테이블 상태: {table.isAvailable}",
                            Content = $"테이블 번호: {_tableList.IndexOf(table)+1}, 테이블 상태: {tableStatus}",
                            Background = Brushes.LightBlue,
                            Margin = new Thickness(5),
                            Width = 330, // 너비 설정
                            Height = 100, // 높이 설정
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };

                        // UniformGrid에 추가
                        TableGrid.Children.Add(newTable);
                    }
                }
            }
        }

        // 테이블 삭제 버튼
        private void DeleteTable_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_tableList.Count > 0) // 리스트에 테이블이 있는지 확인
            {
                // 마지막 테이블의 ID를 가져옴
                int lastTableId = _tableList[_tableList.Count - 1].tableId;

                // 테이블 삭제
                Delete(lastTableId);

                // 화면 새로 고침
                RefreshTableDisplay();
            }
            else
            {
                MessageBox.Show("삭제할 테이블이 없습니다.");
            }
        }

        // 테이블 삭제
        private void Delete(int tableId)
        {
            string query = "" +
            "DELETE FROM minic_db.game_table WHERE table_id = @table_id";
                using (MySqlCommand cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@table_id", tableId);
                    cmd.ExecuteNonQuery();
                }
        }

    }
}
