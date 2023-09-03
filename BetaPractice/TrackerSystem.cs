using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BetaPractice
{
    public class TrackerSystem
    {
        List<GMarkerGoogle> gMarkers = new List<GMarkerGoogle>();//Список маркеров
        List<Tracker> trackerList = new List<Tracker>();//Список трекеров
        string name;//Имя группы
        /// <summary>
        /// Создаёт новую группу трекеров
        /// </summary>
        /// <param name="name">Имя группы</param>
        public TrackerSystem (string name)
        {
            this.name = name;
        }
        /// <summary>
        /// Создаёт новую группу трекеров
        /// </summary>
        /// <param name="group">Список трекеров</param>
        /// <param name="name">Имя группы</param>
        public TrackerSystem (List<Tracker> group, string name)
        {
            this.trackerList = group;
            this.name = name;
            foreach (Tracker tracker in group)
            {
                GMarkerGoogle marker = GetMarker(tracker);
                this.gMarkers.Add(marker);
            }
        }
        /// <summary>
        /// Список трекеров
        /// </summary>
        public List<Tracker> TrackerList { get { return trackerList; } }
        /// <summary>
        /// Имя группы
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Список маркеров
        /// </summary>
        public List<GMarkerGoogle> GMarkers { get { return gMarkers; } set { gMarkers = value; } }

        /// <summary>
        /// Добавляет трекер в систему
        /// </summary>
        /// <param name="tracker">Трекер</param>
        public void AddTracker(Tracker tracker)
        {
            trackerList.Add(tracker);
            GMarkerGoogle marker = GetMarker(tracker);
            gMarkers.Add(marker);
        }

        /// <summary>
        /// Добавляет трекер в систему
        /// </summary>
        /// <param name="Id">Id трекера</param>
        public void AddTracker(int Id)
        {
            Tracker tracker = new Tracker(Id);
            trackerList.Add(tracker);
        }

        /// <summary>
        /// Добавляет трекер в систему
        /// </summary>
        /// <param name="Name">Имя трекера</param>
        /// <param name="Id">ID трекера</param>
        public void AddTracker(string Name, int Id)
        {
            Tracker tracker = new Tracker(Name, Id);
            trackerList.Add(tracker);
        }

        /// <summary>
        /// Удаляет трекер из системы
        /// </summary>
        /// <param name="trackerId">ID трекера</param>
        public void DeleteTracker(int trackerId)
        {
            Tracker tracker = new Tracker();
            bool find = false;
            int i = 0;
            while (i < trackerList.Count && !find)
            {
                if (trackerList[i].Id == trackerId)
                {
                    tracker = trackerList[i];
                    find = true;
                }
                i++;
            }
            if (find)
            {
                trackerList.Remove(tracker);
                gMarkers.RemoveAt(i - 1);
            }
            else
            {
                MessageBox.Show("Ошибка!", "Объект не найден!");
            }
        }

        /// <summary>
        /// Удаляет трекер из системы
        /// </summary>
        /// <param name="tracker">Трекер</param>
        public void DeleteTracker(Tracker tracker)
        {
            trackerList.Remove(tracker);
        }

        /// <summary>
        /// Обновляет значения координат и времени на трекере
        /// </summary>
        /// <param name="TrackerId">ID трекера</param>
        /// <param name="time">Время отправки пакета</param>
        /// <param name="Lat">Широта</param>
        /// <param name="Lng">Долгота</param>
        public void UpdateTracker(int TrackerId, DateTime time, string name = "", double Lat = 361, double Lng = 361)
        {
            int i = Search(TrackerId);
            if (name != "")
            {
                trackerList[i].Name = name;
                gMarkers[i].ToolTipText = trackerList[i].Id + ":" + name;
            }
            trackerList[i].LastTime = time;
            if (Lat != 361 && Lng != 361)
            {
                trackerList[i].Lat = Lat;
                trackerList[i].Lng = Lng;
                PointLatLng point = new PointLatLng(Lat, Lng);
                gMarkers[i].Position = point;
            }
        }
        /// <summary>
        /// Ищет в системе трекер с заданным ID
        /// </summary>
        /// <param name="TrackerId">ID трекера</param>
        /// <returns>Порядковый номер трекера в системе если он существует и -1 иначе</returns>
        public int Search(int TrackerId)
        {
            int i = 0;
            bool find = false;
            while (i < trackerList.Count && !find)
            {
                if (trackerList[i].Id != TrackerId)
                {
                    i++;
                }
                else
                {
                    find = true;
                }
            }
            if (find) return i;
            else return -1;
        }
        /// <summary>
        /// Создаёт маркер трекера
        /// </summary>
        /// <param name="tracker">Трекер</param>
        /// <param name="gMarkerGoogleType">Тип маркера</param>
        /// <returns>Маркер с широтой, долготой, именем и Id  трекера</returns>
        private GMarkerGoogle GetMarker(Tracker tracker, GMarkerGoogleType gMarkerGoogleType = GMarkerGoogleType.green_dot)
        {
            PointLatLng point = new PointLatLng(tracker.Lat, tracker.Lng);
            GMarkerGoogle mapMarker = new GMarkerGoogle(point, gMarkerGoogleType);//широта, долгота, тип маркера
            mapMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(mapMarker);//всплывающее окно с инфой к маркеру
            mapMarker.ToolTipText = tracker.Id + ":" + tracker.Name; // текст внутри всплывающего окна
            mapMarker.ToolTipMode = MarkerTooltipMode.Always; //отображение всплывающего окна (при наведении)
            return mapMarker;
        }

    }
}
