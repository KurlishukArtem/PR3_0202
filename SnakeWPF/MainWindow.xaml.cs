using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
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
using Common;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace SnakeWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Главное окно, используется для для общения между страницами
        public static MainWindow mainWindow;
        /// <summary> Модель данных для передачи IP адреса устройства, порта и никнейма
        public ViewModelUserSettings ViewModelUserSettings = new ViewModelUserSettings();
        /// <summary> Модель игрока в который передаются координаты змеи, яблок и точки
        public ViewModelGames ViewModelGames = null;
        /// <summary> Удалённый IP адрес для подключения к серверу
        public static IPAddress remoteIPAddress = IPAddress.Parse("127.0.0.1");
        /// <summary> Удалённый порт для подключения к серверу
        public static int remotePort = 5001;
        /// <summary> Основной поток для получения данных о игре
        public Thread tRec;
        /// <summary> UDP клиент для получения данных
        public UdpClient receivingUdpClient;
        /// <summary> Страница НОМЕ
        public Pages.Home Home = new Pages.Home();
        /// <summary> Страница GAME
        public Pages.Game Game = new Pages.Game();

        public MainWindow()
        {
            InitializeComponent();
        }
        public void StartReciver()
        {
            //создаем поток для прослушивания канала
            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
        }

        public void OpenPages(Page PageOpen)
        {
            DoubleAnimation startAnimation = new DoubleAnimation();
            //задаем начальное положение анимации
            startAnimation.From = 1;
            startAnimation.To = 0;
            //задаем время анимации
            startAnimation.Duration = TimeSpan.FromSeconds(0.6);
            //подписываемся на выполенение анимации
            startAnimation.Completed += delegate
            {
                //переключаем страницу
                frame.Navigate(PageOpen);
                //создаем конечную анимацию
                DoubleAnimation endAnimation = new DoubleAnimation();
                //задаем начальное и конечное положение
                endAnimation.From = 0;
                endAnimation.To = 1;
                endAnimation.Duration = TimeSpan.FromSeconds(0.6);
                //воспроизводим анимацию на frame, анимацияч прозрачности
                frame.BeginAnimation(OpacityProperty, endAnimation);
            };
            //воспроизводим анимацию на frame, анимацияч прозрачности
            frame.BeginAnimation(OpacityProperty, startAnimation);
        }
        public void Receiver()
        {
            //создаем клиент для обслуживания
            receivingUdpClient = new UdpClient(int.Parse(ViewModelUserSettings.Port));
            IPEndPoint RemoteIpPoint = null;

            try
            {
                //слушаем постоянно
                while (true)
                {
                    //Ожидание дейтаграммы
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    //преобразуем и отображаем данные
                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    //если у нас не существует данных от сервера
                    if (ViewModelGames == null)
                    {
                        //говорим что выполняем вне потока
                        Dispatcher.Invoke(() =>
                        {
                            OpenPages(Game);
                        });
                    }
                    ViewModelGames = JsonConvert.DeserializeObject<ViewModelGames>(returnData.ToString());
                    //если игрок проиграл
                    if (ViewModelGames.SnakesPlayers.GameOver)
                    {
                        //выполняем вне потока
                        Dispatcher.Invoke(() =>
                        {
                            //открываем окно с окончанием игры
                            OpenPages(new Pages.EndGame());
                        });
                    }
                    else
                    {
                        //Вызываем создание UI
                        Game.CreateUI();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
        }
    }
}
