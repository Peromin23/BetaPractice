using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static GMap.NET.Entity.OpenStreetMapGraphHopperGeocodeEntity;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;

namespace BetaPractice
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        public TrackerSystem system = new TrackerSystem("Группа 1");//Группа трекеров
        private GMap.NET.WindowsForms.GMapMarker selectedMarker;//Выбранный маркер (для перетаскивания мышкой)
        private double deadDistance = 6.5;//Дистанция "мёртвой зоны"
        private bool endImitate = true;//Для имитации движения
        private bool imitation = false;//Для имитации движения
        private List<Thread> threads = new List<Thread>();//Для имитации движения
        FormAdd formAdd;//Форма для добавления маркеров

        const double MapTop = 53.176766;//Верхний край карты
        const double MapBottom = 53.171947;//нижний край карты
        const double MapLeft = 50.151372;//Левый край карты
        const double MapRight = 50.164168;//Правый край карты
        const double centerLat = (MapTop + MapBottom) / 2;//Центр карты (широта)
        const double centerLng = (MapLeft + MapRight) / 2;//Центр карты (долгота)
        //Рабочая зона
        static GMap.NET.PointLatLng TopLeft = new GMap.NET.PointLatLng(53.176604584893646, 50.1511743664742);//Левый верхний угол
        static GMap.NET.PointLatLng TopRight = new GMap.NET.PointLatLng(53.176950213918943, 50.151592791080475);//Правый верхний угол
        static GMap.NET.PointLatLng BottomLeft = new GMap.NET.PointLatLng(53.172197972660605, 50.1643306016922);//Нижний левый угол
        static GMap.NET.PointLatLng BottomRight = new GMap.NET.PointLatLng(53.1718715984811, 50.1639711856842);//Нижний правый угол
        static List<GMap.NET.PointLatLng> WorkArea = new List<GMap.NET.PointLatLng>() { TopLeft, TopRight, BottomLeft, BottomRight };//Полигон с рабочей зоной
        //Путь 1
        static GMap.NET.PointLatLng way1Point1 = new GMap.NET.PointLatLng(53.1766897867259, 50.1512538269162);//Точка 1
        static GMap.NET.PointLatLng way1Point2 = new GMap.NET.PointLatLng(53.1755708963539, 50.1542615890503);//Точка 2
        static GMap.NET.PointLatLng way1Point3 = new GMap.NET.PointLatLng(53.1746842836189, 50.15664473176);//Точка 3
        static GMap.NET.PointLatLng way1Point4 = new GMap.NET.PointLatLng(53.1719516849859, 50.1640053838491);//Точка 4
        static List<GMap.NET.PointLatLng> Way1 = new List<GMap.NET.PointLatLng>() { way1Point1, way1Point2, way1Point3, way1Point4 };
        //Путь 2
        static GMap.NET.PointLatLng way2Point1 = new GMap.NET.PointLatLng(53.1767376122086, 50.1513212174177);//Точка 1
        static GMap.NET.PointLatLng way2Point2 = new GMap.NET.PointLatLng(53.176582078721, 50.151678621769);//Точка 2
        static GMap.NET.PointLatLng way2Point3 = new GMap.NET.PointLatLng(53.1754436930688, 50.154748074710369);//Точка 3
        static GMap.NET.PointLatLng way2Point4 = new GMap.NET.PointLatLng(53.173032787929095, 50.161244049668312);//Точка 4
        static GMap.NET.PointLatLng way2Point5 = new GMap.NET.PointLatLng(53.172848201503022, 50.1618498936296);//Точка 5
        static GMap.NET.PointLatLng way2Point6 = new GMap.NET.PointLatLng(53.172648139869004, 50.162479877471924);//Точка 6
        static GMap.NET.PointLatLng way2Point7 = new GMap.NET.PointLatLng(53.172471891930812, 50.162965022027493);//Точка 7
        static GMap.NET.PointLatLng way2Point8 = new GMap.NET.PointLatLng(53.172015693791273, 50.1640821620822);//Точка 8
        static List<GMap.NET.PointLatLng> Way2 = new List<GMap.NET.PointLatLng>() 
        { way2Point1, way2Point2, way2Point3, way2Point4, way2Point5, way2Point6, way2Point7, way2Point8 };
        //Путь 3
        static GMap.NET.PointLatLng way3Point1 = new GMap.NET.PointLatLng(53.176817187263268, 50.1514278352261);//Точка 1
        static GMap.NET.PointLatLng way3Point2 = new GMap.NET.PointLatLng(53.176401225119406, 50.152466520667076);//Точка 2
        static GMap.NET.PointLatLng way3Point3 = new GMap.NET.PointLatLng(53.1763513897708, 50.152583867311478);//Точка 3
        static GMap.NET.PointLatLng way3Point4 = new GMap.NET.PointLatLng(53.175759188676068, 50.154198557138443);//Точка 4
        static GMap.NET.PointLatLng way3Point5 = new GMap.NET.PointLatLng(53.175579738274408, 50.1547054946423);//Точка 5
        static GMap.NET.PointLatLng way3Point6 = new GMap.NET.PointLatLng(53.1750805688195, 50.156144499778748);//Точка 6
        static GMap.NET.PointLatLng way3Point7 = new GMap.NET.PointLatLng(53.174850675619524, 50.156807005405426);//Точка 7
        static GMap.NET.PointLatLng way3Point8 = new GMap.NET.PointLatLng(53.17475984335136, 50.1570537686348);//Точка 8
        static GMap.NET.PointLatLng way3Point9 = new GMap.NET.PointLatLng(53.174007455399973, 50.1590922474861);//Точка 9
        static GMap.NET.PointLatLng way3Point10 = new GMap.NET.PointLatLng(53.173515502297626, 50.1604263111949);//Точка 10
        static GMap.NET.PointLatLng way3Point11 = new GMap.NET.PointLatLng(53.1734120065436, 50.16070157289505);//Точка 11
        static GMap.NET.PointLatLng way3Point12 = new GMap.NET.PointLatLng(53.172973503317039, 50.161805972456932);//Точка 12
        static GMap.NET.PointLatLng way3Point13 = new GMap.NET.PointLatLng(53.172477920948644, 50.1629485934973);//Точка 13
        static List<GMap.NET.PointLatLng> Way3 = new List<GMap.NET.PointLatLng>()
        { way3Point1, way3Point2, way3Point3, way3Point4, way3Point5, way3Point6, way3Point7, way3Point8, way3Point9, way3Point10, way3Point11,
          way3Point12, way3Point13};
        //Путь 4
        static GMap.NET.PointLatLng way4Point1 = new GMap.NET.PointLatLng(53.176825627033132, 50.151445269584656);//Точка 1
        static GMap.NET.PointLatLng way4Point2 = new GMap.NET.PointLatLng(53.1765185790989, 50.152276754379272);//Точка 2
        static GMap.NET.PointLatLng way4Point3 = new GMap.NET.PointLatLng(53.175936427865636, 50.153854228556156);//Точка 3
        static GMap.NET.PointLatLng way4Point4 = new GMap.NET.PointLatLng(53.1757421077631, 50.154387652873993);//Точка 4
        static GMap.NET.PointLatLng way4Point5 = new GMap.NET.PointLatLng(53.175268662339491, 50.155748873949051);//Точка 5
        static GMap.NET.PointLatLng way4Point6 = new GMap.NET.PointLatLng(53.1749077471233, 50.156798958778381);//Точка 6
        static GMap.NET.PointLatLng way4Point7 = new GMap.NET.PointLatLng(53.1748281685268, 50.1570162177086);//Точка 7
        static GMap.NET.PointLatLng way4Point8 = new GMap.NET.PointLatLng(53.174278348735911, 50.158505514264107);//Точка 8
        static GMap.NET.PointLatLng way4Point9 = new GMap.NET.PointLatLng(53.173710837292909, 50.160045437514782);//Точка 9
        static GMap.NET.PointLatLng way4Point10 = new GMap.NET.PointLatLng(53.173462247232081, 50.160717330873013);//Точка 10
        static GMap.NET.PointLatLng way4Point11 = new GMap.NET.PointLatLng(53.17329665370027, 50.161133743822575);//Точка 11
        static GMap.NET.PointLatLng way4Point12 = new GMap.NET.PointLatLng(53.173064942260694, 50.1617161184549);//Точка 12
        static GMap.NET.PointLatLng way4Point13 = new GMap.NET.PointLatLng(53.173012289530121, 50.161849558353424);//Точка 13
        static GMap.NET.PointLatLng way4Point14 = new GMap.NET.PointLatLng(53.1729841544568, 50.161912590265274);//Точка 14
        static GMap.NET.PointLatLng way4Point15 = new GMap.NET.PointLatLng(53.172486562539383, 50.1630424708128);//Точка 15
        static GMap.NET.PointLatLng way4Point16 = new GMap.NET.PointLatLng(53.172394117524895, 50.1632510125637);//Точка 16
        static GMap.NET.PointLatLng way4Point17 = new GMap.NET.PointLatLng(53.172279565817576, 50.163529627025127);//Точка 17
        static GMap.NET.PointLatLng way4Point18 = new GMap.NET.PointLatLng(53.172029962617145, 50.164145193994);//Точка 18

        static List<GMap.NET.PointLatLng> Way4 = new List<GMap.NET.PointLatLng>()
        { way4Point1, way4Point2, way4Point3, way4Point4, way4Point5, way4Point6, way4Point7, way4Point8, way4Point9, 
          way4Point10, way4Point11, way4Point12, way4Point13, way4Point14, way4Point15, way4Point16, way4Point17, way4Point18};
        //Путь 5
        static GMap.NET.PointLatLng way5Point1 = new GMap.NET.PointLatLng(53.176866218284054, 50.151490867137909);//Точка 1
        static GMap.NET.PointLatLng way5Pointx1 = new GMap.NET.PointLatLng(53.176000330250986, 50.153830423951149);//Точка x1
        static GMap.NET.PointLatLng way5Point2 = new GMap.NET.PointLatLng(53.17572040494624, 50.15462301671505);//Точка 2
        static GMap.NET.PointLatLng way5Point3 = new GMap.NET.PointLatLng(53.1754688122308, 50.155406892299652);//Точка 3
        static GMap.NET.PointLatLng way5Point4 = new GMap.NET.PointLatLng(53.175138644791325, 50.156347677111626);//Точка 4
        static GMap.NET.PointLatLng way5Point5 = new GMap.NET.PointLatLng(53.174704379305425, 50.157517790794373);//Точка 5
        static GMap.NET.PointLatLng way5Point6 = new GMap.NET.PointLatLng(53.172828004464279, 50.162576101720333);//Точка 6
        static GMap.NET.PointLatLng way5Point7 = new GMap.NET.PointLatLng(53.1725581068052, 50.163252353668213);//Точка 7
        static GMap.NET.PointLatLng way5Pointx2 = new GMap.NET.PointLatLng(53.172282580340109, 50.163856521248817);//Точка x2
        static GMap.NET.PointLatLng way5Point8 = new GMap.NET.PointLatLng(53.172120901149377, 50.164245273917913);//Точка 8

        static List<GMap.NET.PointLatLng> Way5 = new List<GMap.NET.PointLatLng>()
        { way5Point1, way5Pointx1, way5Point2, way5Point3, way5Point4, way5Point5, way5Point6, way5Point7, way5Pointx2, way5Point8 };
        //Путь 6
        static GMap.NET.PointLatLng way6Point1 = new GMap.NET.PointLatLng(53.176761123035568, 50.151364132761955);//Точка 1
        static GMap.NET.PointLatLng way6Point2 = new GMap.NET.PointLatLng(53.1767127952106, 50.151442922651768);//Точка 2
        static GMap.NET.PointLatLng way6Point3 = new GMap.NET.PointLatLng(53.1731335715816, 50.1611077599227);//Точка 3
        static List<GMap.NET.PointLatLng> Way6 = new List<GMap.NET.PointLatLng>() { way6Point1, way6Point2, way6Point3};
        //Подпуть 1
        static GMap.NET.PointLatLng subWay1Point1 = new GMap.NET.PointLatLng(53.176564395291372, 50.1517242193222);//Точка 1
        static GMap.NET.PointLatLng subWay1Point2 = new GMap.NET.PointLatLng(53.176220772654787, 50.152514800429344);//Точка 2
        static List<GMap.NET.PointLatLng> SubWay1 = new List<GMap.NET.PointLatLng>() { subWay1Point1, subWay1Point2};

        //Подпуть 2
        static GMap.NET.PointLatLng subWay2Point1 = new GMap.NET.PointLatLng(53.1767127952106, 50.151443090289831);//Точка 1
        static GMap.NET.PointLatLng subWay2Point2 = new GMap.NET.PointLatLng(53.176405445045461, 50.152454786002636);//Точка 2
        static List<GMap.NET.PointLatLng> SubWay2 = new List<GMap.NET.PointLatLng>() { subWay2Point1, subWay2Point2 };

        //Подпуть 3
        static GMap.NET.PointLatLng subWay3Point1 = new GMap.NET.PointLatLng(53.176338328077044, 50.152620412409306);//Точка 1
        static GMap.NET.PointLatLng subWay3Point2 = new GMap.NET.PointLatLng(53.176048156502262, 50.153552144765854);//Точка 2
        static List<GMap.NET.PointLatLng> SubWay3 = new List<GMap.NET.PointLatLng>() { subWay3Point1, subWay3Point2 };

        //Подпуть 4
        static GMap.NET.PointLatLng subWay4Point1 = new GMap.NET.PointLatLng(53.175972398087964, 50.153756663203239);//Точка 1
        static GMap.NET.PointLatLng subWay4Point2 = new GMap.NET.PointLatLng(53.1757200030421, 50.15462402254343);//Точка 2
        static List<GMap.NET.PointLatLng> SubWay4 = new List<GMap.NET.PointLatLng>() { subWay4Point1, subWay4Point2 };

        //Подпуть 5
        static GMap.NET.PointLatLng subWay5Point1 = new GMap.NET.PointLatLng(53.172368594627116, 50.163035094738);//Точка 1
        static GMap.NET.PointLatLng subWay5Point2 = new GMap.NET.PointLatLng(53.173030778282573, 50.161250755190849);//Точка 2
        static List<GMap.NET.PointLatLng> SubWay5 = new List<GMap.NET.PointLatLng>() { subWay5Point1, subWay5Point2 };
        /// <summary>
        /// Возвращает список отмеченных путей
        /// </summary>
        /// <returns>Список размеченных путей</returns>
        private List<GMapRoute> DeadZone()
        {
            GMapRoute way1 = new GMapRoute(Way1, "Путь 1");
            way1.Stroke = new Pen(Color.Red);
            GMapRoute way2 = new GMapRoute(Way2, "Путь 2");
            way2.Stroke = new Pen(Color.Red);
            GMapRoute way3 = new GMapRoute(Way3, "Путь 3");
            way3.Stroke = new Pen(Color.Red);
            GMapRoute way4 = new GMapRoute(Way4, "Путь 4");
            way4.Stroke = new Pen(Color.Red);
            GMapRoute way5 = new GMapRoute(Way5, "Путь 5");
            way5.Stroke = new Pen(Color.Red);
            GMapRoute way6 = new GMapRoute(Way6, "Путь 6");
            way6.Stroke = new Pen(Color.Red);

            GMapRoute subWay1 = new GMapRoute(SubWay1, "Подпуть 1");
            subWay1.Stroke = new Pen(Color.Red);
            GMapRoute subWay2 = new GMapRoute(SubWay2, "Подпуть 2");
            subWay2.Stroke = new Pen(Color.Red);
            GMapRoute subWay3 = new GMapRoute(SubWay3, "Подпуть 3");
            subWay3.Stroke = new Pen(Color.Red);
            GMapRoute subWay4 = new GMapRoute(SubWay4, "Подпуть 4");
            subWay4.Stroke = new Pen(Color.Red);
            GMapRoute subWay5 = new GMapRoute(SubWay5, "Подпуть 5");
            subWay5.Stroke = new Pen(Color.Red);
            List<GMapRoute> gMapRoutes = new List<GMapRoute>() { way1, way2, way3, way4, way5, way6, subWay1, subWay2, subWay3, subWay4, subWay5 };

            return gMapRoutes;
        }
        /// <summary>
        /// Добавляет слой с путями и выделенной зоной работы
        /// </summary>
        private void WorkZone()
        {
            GMap.NET.WindowsForms.GMapPolygon polygonWork = new GMap.NET.WindowsForms.GMapPolygon(WorkArea, "Рабочая зона");
            polygonWork.Fill = new SolidBrush(Color.FromArgb(10, Color.Blue));
            GMapOverlay overlayWork = new GMapOverlay();
            overlayWork.Polygons.Add(polygonWork);
            List<GMapRoute> ways = DeadZone();
            foreach (GMapRoute way in ways)
            {
                overlayWork.Routes.Add(way);
            }
            gMapControl.Overlays.Add(overlayWork);
        }

        /// <summary>
        /// Загрузка карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.CacheOnly; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl.CacheLocation = "Cache";
            gMapControl.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance; //какой провайдер карт используется
            gMapControl.MinZoom = 2; //минимальный зум 15
            gMapControl.MaxZoom = 50; //максимальный зум
            gMapControl.Zoom = 16; // какой используется зум при открытии
            gMapControl.Position = new GMap.NET.PointLatLng(53.1743565, 50.15777);// точка в центре карты при открытии 
            gMapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает 
            gMapControl.CanDragMap = true; // перетаскивание карты мышью
            gMapControl.DragButton = MouseButtons.Left; // какой кнопкой осуществляется перетаскивание
            gMapControl.ShowCenter = true; //показывать или скрывать красный крестик в центре
            gMapControl.ShowTileGridLines = false; //показывать или скрывать тайлы
            gMapControl.MarkersEnabled = true;
            gMapControl.PolygonsEnabled = true;
            gMapControl.VirtualSizeEnabled = true;
            gMapControl.DisableFocusOnMouseEnter = true;
            gMapControl.MapScaleInfoEnabled = true;
            gMapControl.MapScaleInfoPosition = MapScaleInfoPosition.Bottom;
            WorkZone();
        }
        /// <summary>
        /// Функция, возвращающая дистанцию от точки до дороги
        /// </summary>
        /// <param name="point">Точка</param>
        /// <param name="route">Дорога</param>
        /// <returns></returns>
        private double GetDistance(PointLatLng point, GMapRoute route)
        {
            double minDistance = double.MaxValue;
            double distance;

            double x = point.Lat;
            double y = point.Lng;

            for (int i = 1; i < route.Points.Count; i++)
            {
                double x1 = route.Points[i - 1].Lat;
                double y1 = route.Points[i - 1].Lng;

                double x2 = route.Points[i].Lat;
                double y2 = route.Points[i].Lng;

                double dx = x2 - x1;
                double dy = y2 - y1;
                double sqrLength = dx * dx + dy * dy;

                double t = ((x - x1) * dx + (y - y1) * dy) / sqrLength;

                if (t < 0)
                {
                    distance = GMapProviders.EmptyProvider.Projection.GetDistance(route.Points[i - 1], point);
                }
                else if (t > 1)
                {
                    distance = GMapProviders.EmptyProvider.Projection.GetDistance(route.Points[i], point);
                }
                else
                {
                    double projectionX = x1 + t * dx;
                    double projectionY = y1 + t * dy;
                    PointLatLng projectionPoint = new PointLatLng(projectionX, projectionY);
                    distance = GMapProviders.EmptyProvider.Projection.GetDistance(projectionPoint, point);
                }

                distance *= 1000;
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            return minDistance;
        }
        /// <summary>
        /// Проверяет вошёл (вышел) ли трекер в "мёрвую зону" и перекрашивает его в случае истины. Добавляет дистанцию до пути в DataGridView
        /// </summary>
        /// <param name="trackerSystem">Группа трекеров</param>
        /// <param name="mapOverlay">Слой карты</param>
        private void IsDeadZone(TrackerSystem trackerSystem, GMapOverlay mapOverlay)
        {
            mapOverlay.Markers.Clear();
            for (int i = 0; i < trackerSystem.GMarkers.Count; i++)
            {
                //Считаем расстояние до ближайшего пути
                double distance = double.MaxValue;
                String minRoute = string.Empty;
                foreach (GMapRoute route in gMapControl.Overlays[0].Routes)
                {
                    if (GetDistance(trackerSystem.GMarkers[i].Position, route) < distance)
                    {
                        //route.Stroke.Color = Color.DeepPink;
                        distance = GetDistance(trackerSystem.GMarkers[i].Position, route);
                        minRoute = route.Name;
                    }
                }
                distance = Math.Round(distance, 1);
                //Запоминаем ID
                int lenth = trackerSystem.GMarkers[i].ToolTipText.IndexOf(":");
                string id = trackerSystem.GMarkers[i].ToolTipText.Substring(0, lenth);
                //Если дистанция меньше 6,5 метров
                if (distance <= deadDistance)
                {
                    GMarkerGoogle gMarkerGoogle = new GMarkerGoogle(trackerSystem.GMarkers[i].Position, GMarkerGoogleType.red_dot);
                    gMarkerGoogle.ToolTipText = trackerSystem.GMarkers[i].ToolTipText;
                    gMarkerGoogle.ToolTipMode = MarkerTooltipMode.Always;
                    trackerSystem.GMarkers[i] = gMarkerGoogle; //Красим в красный
                }
                else
                {
                    if (trackerSystem.GMarkers[i].Type == GMarkerGoogleType.red_dot)
                    {
                        GMarkerGoogle gMarkerGoogle = new GMarkerGoogle(trackerSystem.GMarkers[i].Position, GMarkerGoogleType.green_dot);
                        gMarkerGoogle.ToolTipText = trackerSystem.GMarkers[i].ToolTipText;
                        gMarkerGoogle.ToolTipMode = MarkerTooltipMode.Always;
                        trackerSystem.GMarkers[i] = gMarkerGoogle;
                    }
                }
                //Заиписываем в DataGrid
                for (int j = 0; j < trackerSystem.GMarkers.Count; j++)
                {
                    if (dataGridViewMain[0, j].Value.ToString() == id)
                    {
                        dataGridViewMain[2, j].Value = distance.ToString();
                        if (distance < deadDistance)
                        {
                            dataGridViewMain.Rows[j].Cells[2].Style.BackColor = Color.Red;
                        }
                        else
                        {
                            dataGridViewMain.Rows[j].Cells[2].Style.BackColor = Color.White;
                        }
                    }
                }

                mapOverlay.Markers.Add(trackerSystem.GMarkers[i]);
            }
        }
        /// <summary>
        /// Отрисовывает слой с маркерами трекеров
        /// </summary>
        /// <param name="trackerSystem">Группа трекеров</param>
        public void UpdateOverlay(TrackerSystem trackerSystem)
        {
            if (gMapControl.Overlays.Count < 2)
            {
                GMapOverlay gMapOverlay = new GMapOverlay();
                foreach(GMarkerGoogle marker in trackerSystem.GMarkers)
                {
                    gMapOverlay.Markers.Add(marker);
                }
                gMapControl.Overlays.Add(gMapOverlay);
            }
            else
            {
                if (gMapControl.Overlays[1].Markers.Count < trackerSystem.GMarkers.Count)
                {
                    for (int i = gMapControl.Overlays[1].Markers.Count; i < trackerSystem.GMarkers.Count; i++)
                    {
                        gMapControl.Overlays[1].Markers.Add(trackerSystem.GMarkers[i]);
                        if (endImitate)
                        {
                            Thread thread = new Thread(new ParameterizedThreadStart(ImitateMovement));
                            thread.Start(system.TrackerList[i]);
                            threads.Add(thread);
                        }
                    }
                }
            }
            IsDeadZone(trackerSystem, gMapControl.Overlays[1]);
            gMapControl.Overlays[1].IsVisibile = false;
            gMapControl.Overlays[1].IsVisibile = true;
        }
        /// <summary>
        /// Имитирует движение трекера
        /// </summary>
        /// <param name="obj">Трекер</param>
        public void ImitateMovement(Object obj)
        {
            if (obj is Tracker tracker)
            {
                try
                {
                    int startWait = 800 - tracker.Id % 200;
                    int endWait = 1200 + tracker.Id % 200;
                    double scale = 10000;
                    Random random = new Random(((int)DateTime.Now.Ticks) + tracker.Id);
                    while (!endImitate)
                    {
                        if (system.Search(tracker.Id) >= 0)
                        {
                            int sleepTime = random.Next(startWait, endWait);
                            Invoke((MethodInvoker)delegate
                            {
                                // Running on the UI thread
                                system.UpdateTracker(tracker.Id, DateTime.Now,
                                                     Lat: tracker.Lat += (random.NextDouble() - 0.5) / scale,
                                                     Lng: tracker.Lng += (random.NextDouble() - 0.5) / scale);
                                UpdateOverlay(system);
                            });
                            Thread.Sleep(sleepTime);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            Thread.Yield();
        }
        /// <summary>
        /// Задаёт значения в DataGridView
        /// </summary>
        /// <param name="trackerSystem">Группа трекеров</param>
        public void SetDataGridValues(TrackerSystem trackerSystem)
        {
            dataGridViewMain.Columns.Clear();
            dataGridViewMain.Rows.Clear();
            dataGridViewMain.Columns.Add("ID", "ID");//0
            dataGridViewMain.Columns.Add("Имя", "Имя");//1
            dataGridViewMain.Columns.Add("Дистанция до пути", "Дистанция до пути");//2
            if (trackerSystem.TrackerList.Count > 0)
            {
                for (int i = 0; i < trackerSystem.TrackerList.Count; i++)
                {
                    dataGridViewMain.Rows.Add();
                    dataGridViewMain[0, i].Value = trackerSystem.TrackerList[i].Id;
                    dataGridViewMain[1, i].Value = trackerSystem.TrackerList[i].Name;
                }
            }
        }
        /// <summary>
        /// Сохраняет выбранный фрагмент в кэш
        /// </summary>
        private void SaveToCache()
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            RectLatLng area = gMapControl.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)gMapControl.Zoom; i <= gMapControl.MaxZoom; i++)
                {
                    DialogResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        using (TilePrefetcher obj = new TilePrefetcher())
                        {

                            obj.Shuffle = gMapControl.Manager.Mode != AccessMode.CacheOnly;

                            obj.Owner = this;
                            obj.ShowCompleteMessage = true;
                            obj.Start(area, i, gMapControl.MapProvider, gMapControl.Manager.Mode == AccessMode.CacheOnly ? 0 : 100, gMapControl.Manager.Mode == AccessMode.CacheOnly ? 0 : 1);
                        }
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.CacheOnly;
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    SaveToCache();
        //}
        //--------------------------------------------------------- Взаимодействие с элементами управления ---------------------------------------
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            gMapControl.Bearing = trackBar1.Value;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (formAdd == null || formAdd.IsDisposed)
            {
                formAdd = new FormAdd(this);
            }
            formAdd.Show();
        }

        private void gMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (endImitate == true)
            {
                //находим тот маркер над которым нажали клавишу мыши
                selectedMarker = gMapControl.Overlays.SelectMany(o => o.Markers).FirstOrDefault(m => m.IsMouseOver == true);
            }
        }

        private void gMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedMarker != null)
            {
                //переводим координаты курсора мыши в долготу и широту на карте
                var latlng = gMapControl.FromLocalToLatLng(e.X, e.Y);
                //присваиваем новую позицию для маркера
                selectedMarker.Position = latlng;
                int lenth = selectedMarker.ToolTipText.IndexOf(":");
                int id = int.Parse(selectedMarker.ToolTipText.Substring(0, lenth));
                system.TrackerList[system.Search(id)].Lng = latlng.Lng;
                system.TrackerList[system.Search(id)].Lat = latlng.Lat;
                selectedMarker = null;
                SetDataGridValues(system);
                UpdateOverlay(system);
                if (!(formAdd == null || formAdd.IsDisposed))
                {
                    formAdd.SetDataGrid(system);
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            endImitate = true;
        }

        private void buttonImitate_Click(object sender, EventArgs e)
        {
            if (imitation)
            {
                buttonImitate.Text = "Начать имитацию движения";
                endImitate = true;
                imitation = false;
                buttonAdd.Enabled = true;
            }
            else
            {
                endImitate = false;
                buttonImitate.Text = "Закончить имитацию движения";
                foreach (Tracker tracker in system.TrackerList)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ImitateMovement));
                    thread.Start(tracker);
                    threads.Add(thread);
                }
                imitation = true;
                buttonAdd.Enabled = false;
                if (!(formAdd == null || formAdd.IsDisposed))
                {
                    formAdd.Close();
                }
            }
        }

        private void gMapControl_OnMapDrag()
        {
            //PointLatLng center = new PointLatLng(gMapControl.ViewArea.LocationMiddle.Lat, gMapControl.ViewArea.LocationMiddle.Lng);
            //if (center.Lat > MapTop)
            //{
            //    gMapControl.Position = new PointLatLng(MapTop, center.Lng);
            //}
            //if (center.Lat < MapBottom)
            //{
            //    gMapControl.Position = new PointLatLng(MapBottom, center.Lng);
            //}
            //if (center.Lng < MapLeft)
            //{
            //    gMapControl.Position = new PointLatLng(center.Lat, MapLeft);
            //}
            //if (center.Lng > MapRight)
            //{
            //    gMapControl.Position = new PointLatLng(center.Lat, MapRight);
            //}
        }
    }
}
