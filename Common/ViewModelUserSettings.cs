using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    //отвечающий за данные пользователя, а именно IP-адрес, прослушиваемый порт, имя игрока, ID змеи:
    public class ViewModelUserSettings
    {
        public string IPAddress { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public int IdSnake = -1;
    }
}
