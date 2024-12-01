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
    }
}
