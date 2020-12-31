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

namespace Project_WPM
{
    /// <summary>
    /// subwindow1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }
        private void btn_easy_Click(object sender, RoutedEventArgs e)
            //easy 모드 게임 창을 띄운다
        {
            EasyWindow easy = new EasyWindow();
            easy.Show();
            this.Close();
        }

        private void btn_normal_Click(object sender, RoutedEventArgs e)
            //normal 모드 창을 띄운다
        {
            NormalWindow normal = new NormalWindow();
            normal.Show();
            this.Close();
        }

        private void btn_hard_Click(object sender, RoutedEventArgs e)
            //hard 모드 창을 띄운다
        {
            HardWindow hard = new HardWindow();
            hard.Show();
            this.Close();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
            //뒤로가기 버튼. startWindow를 띄운다.
        {
            StartWindow start = new StartWindow();
            start.Show();
            this.Close();
        }


    }
}

