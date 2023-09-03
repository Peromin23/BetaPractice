using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BetaPractice
{
    public class Tracker
    {
        string name; //Имя трекера
        int id; //Идентификационный номер трекера
        DateTime lastTime; //Время отправки последнего пакета
        double lat; // Координаты широты
        double lng; // Координаты долготы

        public Tracker() { }
        /// <summary>
        /// Создаёт новый объект типа Traker с заданными Id и Name
        /// </summary>
        /// <param name="Name">Имя трекера</param>
        /// <param name="Id">ID трекера</param>
        public Tracker(string Name, int Id)
        {
            name = Name;
            this.id = Id;
        }
        /// <summary>
        ///  Создаёт новый объект типа Traker с заданным Id
        /// </summary>
        /// <param name="Id">ID трекера</param>
        public Tracker(int Id)
        {
            name = Id.ToString();
            this.id = Id;
        }
        /// <summary>
        /// Создаёт новый объект типа Traker с заданными Id, Name, LastTime, Lat, Lng
        /// </summary>
        /// <param name="Name">Имя трекера</param>
        /// <param name="Id">ID трекера</param>
        /// <param name="LastTime">Последнее время обновления позиции трекера</param>
        /// <param name="Lat">Широта трекера</param>
        /// <param name="Lng">Долгота трекера</param>
        public Tracker (string Name, int Id, DateTime LastTime, double Lat, double Lng)
        {
            this.name = Name;
            this.id = Id;
            this.lastTime = LastTime;
            this.lat = Lat;
            this.lng = Lng;
        }
        /// <summary>
        /// Имя трекера
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// ID трекера
        /// </summary>
        public int Id { get { return id; } }
        /// <summary>
        /// Последнее время обновления координат трекера
        /// </summary>
        public DateTime LastTime { get { return lastTime; } set { lastTime = value; } }
        /// <summary>
        /// Широта трекера
        /// </summary>
        public double Lat { get { return lat; } set { lat = value; } }
        /// <summary>
        /// Долгота трекера
        /// </summary>
        public double Lng { get { return lng; } set { lng = value; } }
    }
}
