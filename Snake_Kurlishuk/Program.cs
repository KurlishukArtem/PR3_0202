using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace Snake_Kurlishuk
{
    internal class Program
    {
        public static List<Leaders> Leaders = new List<Leaders>();
        public static List<ViewModelUserSettings> remoteIPAddress = new List<ViewModelUserSettings>();
        public static List<ViewModelGames> viewModelGames = new List<ViewModelGames>();
        private static int localPort = 5001;
        public static int MaxSpeed = 15;
        static void Main(string[] args)
        {
            try
            {
                //Создаем поток для прослушки сообщений от клиентов
                Thread tRec = new Thread(new ThreadStart(Reciver));
                //Запускаем поток прослушивания
                tRec.Start();
                // Создаем таймер Для управления игрой
                Thread tTime = new Thread(Timer);
                tTime.Start();
            }
            catch (Exception ex)
            { 
                //Если что-то пошло не так, выводим сообщение о том что произошла АШИБКА
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Возникло исключение: " + ex.ToString() + "\n" + ex.Message);
            }
        }

        private static void Send()
        {
            foreach (ViewModelUserSettings User in remoteIPAddress)
            {
                UdpClient sender = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(
                    IPAddress.Parse(User.IPAddress),
                    int.Parse(User.Port));
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelGames.Find(x => x.IdSnake == User.IdSnake)));

                    //отпавляем данные
                    sender.Send(bytes, bytes.Length, endPoint);
                    //выводим данные
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Отправил данные пользователю: {User.IPAddress}:{User.Port}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
                }
                finally
                {
                    //если все выполнилось успешно, уматываем из этого метода
                    sender.Close();
                }
            }
        }

        
        public static void Reciver()
        {
            UdpClient receivingUdpClient = new UdpClient(localPort);
            IPEndPoint RemoteIpEndPoint = null;
            try
            {
                //выводим сообщение
                Console.WriteLine("Команды сервера: ");
                // запускаем бесконечный цикл для прослушки входящих сообщений
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(
                        ref RemoteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("получил команду: " + returnData.ToString());

                    //начало игры
                    if (returnData.ToString().Contains("/start"))
                    {
                        string[] dataMessage = returnData.ToString().Split('|');
                        //конвентируем данные в модель
                        ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Подключился пользователь: {viewModelUserSettings.IPAddress}:{viewModelUserSettings.Port}");
                        // Добавляем данные в коллекцию для того, чтобы отправить пользователю
                        remoteIPAddress.Add(viewModelUserSettings);
                        viewModelUserSettings.IdSnake = AddSnake();
                        //связываем змею и игрокa
                        viewModelGames[viewModelUserSettings.IdSnake].IdSnake = viewModelUserSettings.IdSnake;
                    }
                    else
                    {
                        //если команда не является стартом

                        // управление змеёй
                        string[] dataMessage = returnData.ToString().Split('|');
                        //конвертируем данные в модель
                        ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
                        //Получаем Id игрока
                        int IdPlayer = -1;
                        //в случае если мертвый игрок прописывает команду
                        // находим ID игрока, ища его в списке по IP адрессу и порту
                        IdPlayer = remoteIPAddress.FindIndex(x => x.IPAddress == viewModelUserSettings.IPAddress
                        && x.Port == viewModelUserSettings.Port);
                        if (IdPlayer != -1)
                        {
                            if (dataMessage[0] == "Up" &&
                                viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Down)
                                viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Up;
                        }
                        if (IdPlayer != -1)
                        {
                            if (dataMessage[0] == "Down" &&
                                viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Up)
                                viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Down;
                        }
                        if (IdPlayer != -1)
                        {
                            if (dataMessage[0] == "Left" &&
                                viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Right)
                                viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Left;
                        }
                        if (IdPlayer != -1)
                        {
                            if (dataMessage[0] == "Right" &&
                                viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Left)
                                viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Right;
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Вознило исключение: " + ex.Message);
            }

        }
        public static int AddSnake()
        {
            ViewModelGames viewModelGamesPlayer = new ViewModelGames();
            //указываем стартовые координаты
            viewModelGamesPlayer.SnakesPlayers = new Snakes()
            {
                Points = new List<Snakes.Point>()
                {
                    new Snakes.Point() { X = 30, Y = 10 },
                    new Snakes.Point() { X = 20, Y = 10 },
                    new Snakes.Point() { X = 10, Y = 10 },
                },
                //направление змеи
                direction = Snakes.Direction.Start
            };
            //создаем рандомную точку на карте
            viewModelGamesPlayer.Points = new Snakes.Point(new Random().Next(10, 783), new Random().Next(10, 410));
            //добавляем змею в общий список всех змей
            viewModelGames.Add(viewModelGamesPlayer);
            return viewModelGames.FindIndex(x => x == viewModelGamesPlayer);
        }
        public static void Timer()
        {
            while (true)
            {
                Thread.Sleep(100);

                //получаем змей которых необходимо удалить
                List<ViewModelGames> RemoteSnakes = viewModelGames.FindAll(x => x.SnakesPlayers.GameOver);
                //если количество змей больше 0
                if (RemoteSnakes.Count > 0)
                {
                    foreach (ViewModelGames DeadSnake in RemoteSnakes)
                    {
                        //говорим что отключаем игрока
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Отключил пользователя: {remoteIPAddress.Find(x => x.IdSnake == DeadSnake.IdSnake).IdSnake}" +
                            $": {remoteIPAddress.Find(x => x.IdSnake == DeadSnake.IdSnake).Port}");
                        //Удаляем пользователя
                        remoteIPAddress.RemoveAll(x => x.IdSnake == DeadSnake.IdSnake);

                    }
                    //удаляем змей которых надо удалить
                    viewModelGames.RemoveAll(x => x.SnakesPlayers.GameOver);
                }
                //перебираем подключенных игроков
                foreach (ViewModelUserSettings User in remoteIPAddress)
                {
                    Snakes Snake = viewModelGames.Find(x => x.IdSnake == User.IdSnake).SnakesPlayers;
                    //прогоняем точки змеи через цикл от конца в начало
                    for (int i = Snake.Points.Count - 1; i >= 0; i--)
                    {
                        if (i != 0)
                        {
                            //перемещаем точку на место предыдущей
                            Snake.Points[i] = Snake.Points[i - 1];
                        }
                        else 
                        {
                            //получаем скорость змеи (поскольку радиус точки 10, начальная скорость 10 пунктов)
                            int Speed = 10 + (int)Math.Round(Snake.Points.Count / 20f);
                            //если скорость змеи выше максимальной
                            if (Speed > MaxSpeed) Speed = MaxSpeed;
                            //Если направление змеи влево
                            if (Snake.direction == Snakes.Direction.Right)
                            {
                                //двигаем змею влево
                                Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X + Speed, Y = Snake.Points[i].Y };
                            }
                            else if (Snake.direction == Snakes.Direction.Down)
                            {
                                //двигаем змею влево
                                Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X , Y = Snake.Points[i].Y + Speed };
                            }
                            else if (Snake.direction == Snakes.Direction.Up)
                            {
                                //двигаем змею влево
                                Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X, Y = Snake.Points[i].Y - Speed};
                            }
                            else if (Snake.direction == Snakes.Direction.Left)
                            {
                                //двигаем змею влево
                                Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X - Speed, Y = Snake.Points[i].Y};
                            }
                        }
                    }
                    //Проверим змею на столкновение с препядствием
                    // Если первая точка змеи вышла за координаты экрана по горизонтали
                    if (Snake.Points[0].X <= 0 || Snake.Points[0].X >= 793)
                    {
                        //говорим что игра окончена
                        Snake.GameOver = true;
                    }
                    else if (Snake.Points[0].Y <= 0 || Snake.Points[0].Y >= 420)
                    { 
                        //аналогично верхнему
                        Snake.GameOver = true ;
                    }
                   //проверяем не столкнулись ли змеи между собой 
                   if (Snake.direction != Snakes.Direction.Start)
                   {
                        //прогоняем все точки кроме первой 
                        for (int i = 1; i< Snake.Points.Count; i++)
                        {
                            //Если первая точка находится на координатах последующией по горизонтали 
                            if (Snake.Points[0].X >= Snake.Points[i].X - 1 && Snake.Points[0].X <= Snake.Points[i].X + 1)
                            {
                                if (Snake.Points[0].Y >= Snake.Points[i].Y - 1 && Snake.Points[0].Y <= Snake.Points[i].Y + 1)
                                {
                                    Snake.GameOver = true;
                                    break;
                                }
                            }
                        }
                   }
                   //Проверяем что если первая точка змеи игрока находится в координатах яблока по горизонтали
                   if (Snake.Points[0].X >= viewModelGames.Find(x=>x.IdSnake==User.IdSnake).Points.X - 15 &&
                        Snake.Points[0].X <= viewModelGames.Find(x=>x.IdSnake==User.IdSnake).Points.X + 15)
                   {
                        if (Snake.Points[0].Y >= viewModelGames.Find(x => x.IdSnake == User.IdSnake).Points.Y - 15 &&
                        Snake.Points[0].Y <= viewModelGames.Find(x => x.IdSnake == User.IdSnake).Points.Y + 15)
                        {
                            viewModelGames.Find(x => x.IdSnake == User.IdSnake).Points = new Snakes.Point(
                                new Random().Next(10, 783),
                                new Random().Next(10, 410));

                            //добавляем змее новую точку на координатах последней
                            Snake.Points.Add(new Snakes.Point()
                            {
                                X = Snake.Points[Snake.Points.Count - 1].X,
                                Y = Snake.Points[Snake.Points.Count - 1].Y
                            });

                            // загружаем таблицу
                            LoadLeaders();
                            // добавляем нас в таблицу
                            Leaders.Add(new Leaders()
                            {
                                Name = User.Name,
                                Points = Snake.Points.Count - 3
                            });
                            // сортируем таблицу по двум значениям сначала по ко-ву точек затем  по наименованию
                            Leaders = Leaders.OrderByDescending(x => x.Points).ThenBy(x => x.Name).ToList();
                            //Ищем себя в списке и записываем в модель змеи
                            viewModelGames.Find(x => x.IdSnake == User.IdSnake).Top =
                                Leaders.FindIndex(x => x.Points == Snake.Points.Count - 3 && x.Name == User.Name) + 1;
                        }
                   }
                    //Если игра для змеи закончена 
                    if (Snake.GameOver)
                    {
                        //загружаем таблицу
                        LoadLeaders();
                        //добавляем нас в таблицу 
                        Leaders.Add(new Leaders()
                        {
                            Name = User.Name,
                            Points = Snake.Points.Count - 3
                        });
                        SaveLeaders();
                    }
                }
            }
            Send();
        }
        public static void SaveLeaders()
        {
            string json = JsonConvert.SerializeObject(Leaders);
            //записываем в файл
            StreamWriter SW = new StreamWriter("./leaders.txt");
            SW.WriteLine(json);
            SW.Close();
        }
        public static void LoadLeaders()
        {
            //проверяем есть ли файл
            if (File.Exists("./leaders.txt"))
            {
                // Открываем файл 
                StreamReader SR = new StreamReader("./leaders.txt");
                //читаем первую строку
                string json = SR.ReadLine();
                //закрываем файл
                SR.Close();
                // если есть что читать
                if (!string.IsNullOrEmpty(json))
                    Leaders = JsonConvert.DeserializeObject<List<Leaders>>(json);
                else
                    // возвращаем пустой результат
                    Leaders = new List<Leaders>();
            }
            else
                Leaders = new List<Leaders>();
        }

    }
}
