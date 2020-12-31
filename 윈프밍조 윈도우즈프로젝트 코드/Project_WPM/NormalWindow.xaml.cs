using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Project_WPM
{
    /// <summary>
    /// subwindow2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NormalWindow : Window, INotifyPropertyChanged  //Normal 모드. 전체적으로 Easy모드와 동일하게 작동한다.
    {
        Button first;
        Button second;
        bool isStop = false;

        private string matched;
        public string Matched
        {
            get
            {
                return matched;
            }
            set
            {
                matched = value;
                OnPropertyChanged("Matched");
            }
        }

        private string count = "20";        //Normal모드 시간 제한은 20초
        public string Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged("Count");
                if (Int32.Parse(count) <= 0)
                {
                    tmr.Stop();
                    MessageBoxResult res = MessageBox.Show(
                         "실패!! 다시 하시겠습니까?", "Failed!!", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        resetRnd();
                        boardReset();
                        btnSet();
                        Matched = "36";

                        first = null;
                        second = null;
                        count = 20.ToString();
                        tmr.Start();
                    }
                    else
                    {
                        MenuWindow menu = new MenuWindow();
                        menu.Show();
                        this.Close();
                    }

                }
            }
        }

        System.Windows.Threading.DispatcherTimer tmr = new System.Windows.Threading.DispatcherTimer();

        public NormalWindow()
        {
            InitializeComponent();
            btnSet();
            tmr.Interval = new TimeSpan(0, 0, 1);
            tmr.Tick += new EventHandler(tmr_Tick);
            this.DataContext = this;
            tmr.Start();
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            Count = (Int32.Parse(Count) - 1).ToString();
            label1.Content = Count;
        }

        private void btnSet()
        {
            Matched = "36";         //Normal모드는 맞춰야 하는 카드 갯수가 36(6*6)개이다.

            for (int i = 0; i < 36; i++)    //36개의 버튼을 세팅
            {
                Button btn = new Button();
                btn.Background = Brushes.White;
                btn.Margin = new Thickness(5);
                btn.Tag = TagSet();       
                btn.Content = MakeImage("/Resources/Images/" + btn.Tag + ".png");
                btn.Click += btn_Click;
                gameBoard.Children.Add(btn);
            }
        }

        int[] randint = new int[36];

        private int TagSet()    // 0~17사이 정수를 만들어 리턴하는 함수. 중복되지 않도록 한쌍에 2개씩 숫자를 랜덤으로 생성한다.
        {                       // 0~35사이의 랜덤값이 중복되지 않게 만들어지고 이를 18로 나눈 나머지값을 리턴
            int i;
            Random rand = new Random();
            while (true)
            {
                i = rand.Next(36); 
                if (randint[i] == 0)
                {
                    randint[i] = 1;
                    break;
                }
            }
            return i % 18; // 태그는 0~17까지, 18개의 그림을 표시
        }

        private void btn_Click(object sender, RoutedEventArgs e) 
        {
            Button btn = sender as Button;

            if (first == null) 
            {
                first = btn;
                btn.Opacity = 0.5;
                btn.IsEnabled = false;
                return;
            }
            else if (second == null)  
            {
                second = btn;
                btn.Opacity = 0.5;
                btn.IsEnabled = false;
            }
            else 
                return;

            if ((int)first.Tag == (int)second.Tag) 
            {
                Button btn1 = first;
                Button btn2 = second;
                btn1.Opacity = 0.0;
                btn2.Opacity = 0.0;

                first = null;
                second = null;

                Matched = (Int32.Parse(Matched) - 2).ToString(); 

                if (Int32.Parse(Matched) <= 0)
                {
                    tmr.Stop();
                    MessageBoxResult res = MessageBox.Show(
                         "성공! 다시 하시겠습니까?", "Success", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        resetRnd();
                        boardReset();
                        btnSet();
                        Matched = "36";

                        first = null;
                        second = null;
                        Count = 20.ToString();
                        tmr.Start();
                    }
                    else
                    {
                        MenuWindow menu = new MenuWindow();
                        menu.Show();
                        this.Close();
                    }
                }
            }

            else
            {
                Button btn1 = first;
                Button btn2 = second;
                btn1.Opacity = 1.0;
                btn1.IsEnabled = true;
                btn2.Opacity = 1.0;
                btn2.IsEnabled = true;

                first = null;
                second = null;
            }
        }

        private void boardReset()    
        {
            gameBoard.Children.Clear();
        }

        private void resetRnd()       
        {
            for (int i = 0; i < 36; i++)
                randint[i] = 0;
        }

        private Image MakeImage(string r) 
        {
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(r, UriKind.Relative);
            bit.EndInit();

            Image myImage = new Image();
            myImage.Margin = new Thickness(5);
            myImage.Stretch = Stretch.Fill;
            myImage.Source = bit;

            return myImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyScore)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyScore));
            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Close();
            tmr.Stop();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            if (isStop == false)
            {
                tmr.Stop();
                for (int i = 0; i < 36; i++)
                {
                    Button btn = gameBoard.Children[i] as Button;
                    btn.IsEnabled = false;
                }
                isStop = true;
            }
            else
            {
                tmr.Start();
                for (int i = 0; i < 36; i++)
                {
                    Button btn = gameBoard.Children[i] as Button;
                    btn.IsEnabled = true;
                }
                isStop = false;
            }
        }
    }
}
