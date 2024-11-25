using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    //отвечающий за данные, передаваемые пользователю,
    //а именно координаты точек и которых состоит змея игрока,
    //точка на карте, место в рейтинге лидеров, код змеи.
    public class ViewModelGames
    {
        public Snakes SnakesPlayers = new Snakes();
        public Snakes.Point Points = new Snakes.Point();
        public int Top = 0;
        public int IdSnake {  get; set; }
    }
}
