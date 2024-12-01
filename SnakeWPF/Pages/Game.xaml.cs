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
using Common;
using DocumentFormat.OpenXml.Vml;


namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        public int StepCadr = 0;
        public Game()
        {
            InitializeComponent();
        }
        public void CreateUI()
        {
            //выполняем вне потока
            Dispatcher.Invoke(() =>
            {
                //если кадр 0 то кадр 1
                if (StepCadr == 0) StepCadr = 1;
                else StepCadr = 0;
                //чистим интерфейс
                canvas.Children.Clear();
                //перебираем точки змеи
                for (int iPoint = MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Points.Count - 1; iPoint >= 0; iPoint--)
                {
                    Snakes.Point SnakePoint = MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Points[iPoint];

                    // Смещение точек змеи
                    if (iPoint != 0)
                    {
                        //получаем следующую точку змеи
                        Snakes.Point NextSnakePoint = MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Points[iPoint - 1];
                        // Если точка находится по горизонтали
                        if (SnakePoint.X > NextSnakePoint.X || SnakePoint.X < NextSnakePoint.X)
                        {
                            if (iPoint % 2 == 0)
                            {
                                //если кадр четный
                                if (iPoint % 2 == 0)
                                    SnakePoint.Y -= 1;
                                else
                                    SnakePoint.Y += 1;


                            }
                            else {
                                //если кадр четный
                                if (iPoint % 2 == 0)
                                    SnakePoint.Y += 1;
                                else
                                    SnakePoint.Y -= 1;
                            }
                        }
                        if (SnakePoint.Y > NextSnakePoint.Y || SnakePoint.Y < NextSnakePoint.Y)
                        {
                            if (iPoint % 2 == 0)
                            {
                                //если кадр четный
                                if (iPoint % 2 == 0)
                                    SnakePoint.X -= 1;
                                else
                                    SnakePoint.X += 1;


                            }
                            else
                            {
                                //если кадр четный
                                if (iPoint % 2 == 0)
                                    SnakePoint.X += 1;
                                else
                                    SnakePoint.X -= 1;
                            }
                        }
                    }
                    Brush Color;
                    if (iPoint == 0)
                        Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 127, 14));
                    else
                        Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 198, 19));
                    //рисуем точку
                    Ellipse ellipse = new Ellipse()
                    {
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(SnakePoint.X - 10, SnakePoint.Y - 10, 0, 0),
                        //Цвет точки
                        Fill = Color,
                        // обводка
                        Stroke = Brushes.Black
                    };
                    canvas.Children.Add(ellipse);
                }
                //отрисовка яблока
                //Изображение яблока
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Image/Apple.png"));
                //яблоко на UI
                Ellipse point = new Ellipse()
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(MainWindow.mainWindow.ViewModelGames.Points.X - 20,
                        MainWindow.mainWindow.ViewModelGames.Points.Y - 20,0,0),
                    Fill = myBrush
                };
                canvas.Children.Add(point);
            });
        }
    }
}
