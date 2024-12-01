using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            //Если есть соединение
            if (MainWindow.mainWindow.receivingUdpClient != null) 
                MainWindow.mainWindow.receivingUdpClient.Close();
            //если есть поток
            if (MainWindow.mainWindow.tRec != null)
                MainWindow.mainWindow.tRec.Abort();
            //IP address
            IPAddress UserIPAddress;
            //Если IP не преобразуется
            if (!IPAddress.TryParse(ip.Text, out UserIPAddress))
            {
                MessageBox.Show("Please use the IP addres in the format X.X.X.X.");
                return;
            }
            int UserPort;
            //если не преобразуется
            if (!int.TryParse(port.Text, out UserPort))
            {
                MessageBox.Show("Please use the port as a number");
                return;
            }
            //запускаем потоки на прослушку
            MainWindow.mainWindow.StartReciver();
            //заполняем ИП игрока в модель
            MainWindow.mainWindow.ViewModelUserSettings.IPAddress = ip.Text;
            // заполянем порт игрока в модель
            MainWindow.mainWindow.ViewModelUserSettings.Port = port.Text;
            //заполянем ник игрока
            MainWindow.mainWindow.ViewModelUserSettings.Name = name.Text;
            //отправляем команду /start
            MainWindow.Send("/start|" + JsonConvert.SerializeObject(MainWindow.mainWindow.ViewModelUserSettings));

        }
    }
}
