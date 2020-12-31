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
    public partial class EasyWindow : Window, INotifyPropertyChanged
    {
        Button first;       //첫번째 선택되는 버튼의 정보를 저장
        Button second;      //두번째 선택되는 버튼의 정보를 저장        
        bool isStop = false;    //현재 일시정지 되어 있는지를 나타내는 flag(dafalt는 false, 일시정지 되어있지 않음)

        private string matched;
        public string Matched      // 남은 카드개수를 xaml element와 데이터 바인딩 하기 위한 Property
        {
            get
            {
                return matched;
            }
            set
            {
                matched = value;
                OnPropertyChanged("Matched");       //matched값이 변경되면 알려준다.
            }
        }

        private string count = "10";    //타이머의 남은 시간(기본 10초)
        public string Count             //남은 시간을 xaml의 element와 데이터 바인딩 하기 위한 Property
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged("Count");         //count값이 변경되면 알려준다.
                if (Int32.Parse(count) <= 0)        //count값이 0이 되면(Time out이 되면)
                {
                    tmr.Stop();                     //타이머를 멈추고
                    MessageBoxResult res = MessageBox.Show(     //실패창을 띄우고 다시 할것인지 물어본다.
                         "실패!! 다시 하시겠습니까?", "Failed!!", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)    //다시 한다고 하면
                    {
                        resetRnd();                     //게임을 초기화 한다.(카드를 다시 섞고, 남은 카드 갯수를 초기화하고,
                        boardReset();                   //선택된 버튼들을 null로 바꾸고, 타이머를 다시 작동시킨다.)
                        btnSet();
                        Matched = "16";

                        first = null;
                        second = null;
                        Count = 10.ToString();
                        tmr.Start();
                    }
                    else                                //안한다고 하면 메뉴창을 띄우고 현재 창을 지운다.
                    {
                        MenuWindow menu = new MenuWindow();
                        menu.Show();
                        this.Close();
                    }
                }
            }
        }

        System.Windows.Threading.DispatcherTimer tmr = new System.Windows.Threading.DispatcherTimer();  //타이머 선언

        public EasyWindow()
        {
            InitializeComponent();
            btnSet();   //게임 판 초기화
            tmr.Interval = new TimeSpan(0, 0, 1);   //타이머 간격 설정
            tmr.Tick += new EventHandler(tmr_Tick); //설정해 놓은 간격이(1초) 지날때 마다 이벤트 호출
            this.DataContext = this;    //데이터 바인딩을 위한 DataContext설정
            tmr.Start();    //타이머 시작
        }

        void tmr_Tick(object sender, EventArgs e)   //1초가 지날때마다 Count값을 1감소시킨다.
        {
            Count = (Int32.Parse(Count) - 1).ToString();
        }

        private void btnSet()   //버튼을 랜덤으로 배치
        {
            Matched = "16";     //easy모드는 16(4 *4)개 카드를 맞춰야한다

            for (int i = 0; i < 16; i++)    //16개 버튼을 생성하고 랜덤으로 설정된 tag번호의 이미지를 넣는다
            {
                Button btn = new Button();
                btn.Background = Brushes.White;
                btn.Margin = new Thickness(5);
                btn.Tag = TagSet();                 //버튼마다 랜덤한 tag부여
                btn.Content = MakeImage("/Resources/Images/" + btn.Tag + ".png");
                btn.Click += btn_Click;             //버튼마다 클릭 이벤트를 넣는다
                gameBoard.Children.Add(btn);        //생성된 버튼들을 xaml의 gameBoard(UniformGrid)의 children으로 추가
            }
        }

        int[] randint = new int[16];    // 랜덤 숫자가 중복되는지 체크할 배열선언

        private int TagSet()    // 0~7사이 정수를 만들어 리턴하는 함수. 중복되지 않도록 한쌍에 2개씩 숫자를 랜덤으로 생성한다.
        {                       // 0~15사이의 랜덤값이 중복되지 않게 만들어지고 이를 8로 나눈 나머지값을 리턴
            int i;
            Random rand = new Random();
            while (true)
            {
                i = rand.Next(16);
                if (randint[i] == 0)
                {
                    randint[i] = 1;
                    break;
                }
            }
            return i % 8; // 태그는 0~7까지, 8개의 그림을 표시
        }

        private void btn_Click(object sender, RoutedEventArgs e)    //버튼 클릭 이벤트
        {
            Button btn = sender as Button;

            if (first == null)         // 눌려진 첫번째 버튼이 아직 눌러지지 않았으면 눌린 버튼을첫번째 버튼으로 설정
            {                          // 눌려진 버튼은 흐리게 바꾸고 선택할 수 없게 한다.
                first = btn;
                btn.Opacity = 0.5;
                btn.IsEnabled = false;
                return;
            }
            else if (second == null)   // 첫번째 버튼이 눌렸고 두번째 버튼이 아직 눌려지지 않았을때 두번째 버튼을 누르면 두번째 버튼으로 설정 
            {
                second = btn;
                btn.Opacity = 0.5;
                btn.IsEnabled = false;
            }
            else
                return;

            // 매치가 되었을 때
            if ((int)first.Tag == (int)second.Tag)  // 선택된 두 버튼의 Tag Property가 같다면
            {
                Button btn1 = first;                // 두 버튼을 안보이게 하고
                Button btn2 = second;
                btn1.Opacity = 0.0;
                btn2.Opacity = 0.0;

                first = null;                       // first second를 null로 설정해준다
                second = null;

                Matched = (Int32.Parse(Matched) - 2).ToString();   // 남은 카드를 2개 감소시킨다.

                if (Int32.Parse(Matched) <= 0)      //남은 카드가 0이 되면 게임 Success. 
                {
                    tmr.Stop();                     //타이머를 멈추고 다시 할건지 메시지 박스로 물어본다.
                    MessageBoxResult res = MessageBox.Show(
                         "성공! 다시 하시겠습니까?", "Success", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)    //한다고 하면 위와 동일하게 다시 게임을 시작 아니라고 하면 창을 닫고 메뉴 화면을 띄운다
                    {
                        resetRnd();
                        boardReset();
                        btnSet();
                        Matched = "16";

                        first = null;
                        second = null;
                        Count = 10.ToString();
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
            // 매치가 안되었을 때
            else        // 선택이 되지 않게 해두었던 버튼들을 활성화. first, second 버튼을 null로 설정한다.
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

        // 게임 재시작시 초기화
        private void boardReset()           // 설정되어있던 버튼 초기화
        {
            gameBoard.Children.Clear();
        }

        private void resetRnd()             // randint[] 배열 초기화
        {
            for (int i = 0; i < 16; i++)
                randint[i] = 0;
        }

        private Image MakeImage(string r)   //파일을 불러서 버튼 이미지를 설정
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

        //남은 카드(Matched)변수, 남은 시간(Count)변수를 xaml과 데이터바인딩하기 위한 INotifyPropertyChanged인터페이스 구현
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyScore)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyScore));
            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)   //나가기 버튼. 메뉴 Window를 부르고 현재창을 닫는다.
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Close();
            tmr.Stop();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)   //일시정지 버튼
        {
            if (isStop == false)        //현재 일시정지가 되어있지 않으면 타이머를 멈추고 버튼들이 모두 선택될 수 없도록 하고 일시정지가 되어있다고 설정한다.
            {
                tmr.Stop();
                for (int i = 0; i < 16; i++)
                {
                    Button btn = gameBoard.Children[i] as Button;
                    btn.IsEnabled = false;
                }
                isStop = true;
            }
            else                        //현재 일시정지가 되어있으면 타이머를 다시 시작하고 버튼을 선택될수 있도록 하고 일시정지가 되어있지 않다고 설정한다.
            {
                tmr.Start();
                for (int i = 0; i < 16; i++)
                {
                    Button btn = gameBoard.Children[i] as Button;
                    btn.IsEnabled = true;
                }
                isStop = false;
            }
        }
    }


}
