using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Snakes
    {
        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            //система координат очевидно
            public Point(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            } 
            public Point() { }
        }
        //направление движения змеи
        public enum Direction 
        {
            Left,
            Right, 
            Up,
            Down,
            Start  
        }

        //точки из которых состоит Змея
        public List<Point> Points = new List<Point>();
        //направление движения в котором двигается змея
        public List<Direction> direction = Direction.Start;
        public bool GameOver = false;   
    }
}
