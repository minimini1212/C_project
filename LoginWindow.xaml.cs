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
    public partial class LoginWindow : Window
    {
        // standby 화면으로 넘기기위한 작업
        private MainWindow _mainWindow; // 단순히 필드 선언 (아직 객체 없음)

        public LoginWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;  // MainWindow 객체를 받음
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("signup Button Clicked!");
            // 로그인 성공 시 MainWindow에서 standbyScreen 페이지로 이동
            _mainWindow.LoadStandbyScreen();
            this.Close();
        }
    }
}