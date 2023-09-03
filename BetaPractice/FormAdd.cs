using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaPractice
{
    public partial class FormAdd : Form
    {
        public FormAdd(FormMain formMain)
        {
            parent = formMain;
            copy = formMain.system;
            InitializeComponent();
            SetDataGrid(copy);
        }
        FormMain parent;//Родителькая форма
        TrackerSystem copy;//Группа трекеров

        const double MapTop = 53.176766;
        const double MapBottom = 53.171947;
        const double MapLeft = 50.151372;
        const double MapRight = 50.164168;
        const double centerLat = (MapTop + MapBottom) / 2;//Центр карты (широта)
        const double centerLng = (MapLeft + MapRight) / 2;//Центр карты (долгота)
        /// <summary>
        /// Задаёт значения в DataGridView
        /// </summary>
        /// <param name="trackerSystem">Группа трекеров</param>
        public void SetDataGrid(TrackerSystem trackerSystem)
        {
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();
            dataGridView.Columns.Add("ID","ID");//0
            dataGridView.Columns[0].ReadOnly = true;
            dataGridView.Columns.Add("Имя", "Имя");//1
            dataGridView.Columns.Add("Время", "Время");//2
            dataGridView.Columns[2].ReadOnly = true;
            dataGridView.Columns.Add("Широта", "Широта");//3
            dataGridView.Columns[3].ValueType = typeof(double);
            dataGridView.Columns.Add("Долгота", "Долгота");//4
            dataGridView.Columns[4].ValueType = typeof(double);
            if (trackerSystem.TrackerList.Count > 0)
            {
                for (int i = 0; i < trackerSystem.TrackerList.Count; i++)
                {
                    dataGridView.Rows.Add();
                    dataGridView[0,i].Value = trackerSystem.TrackerList[i].Id;
                    if (trackerSystem.TrackerList[i].Name != null) dataGridView[1, i].Value = trackerSystem.TrackerList[i].Name;
                    if (trackerSystem.TrackerList[i].LastTime != null) dataGridView[2, i].Value = trackerSystem.TrackerList[i].LastTime;
                    if (trackerSystem.TrackerList[i].Lat != 0) dataGridView[3, i].Value = trackerSystem.TrackerList[i].Lat;
                    if (trackerSystem.TrackerList[i].Lng != 0) dataGridView[4, i].Value = trackerSystem.TrackerList[i].Lng;
                }
            }
            parent.SetDataGridValues(copy);
            parent.UpdateOverlay(copy);
        }
        //----------------------------------------------- Взаимодйствие с элементами управления ------------------------------------
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int id;
            string name = string.Empty;
            if (int.TryParse(textBoxID.Text, out id))//Если ID в нужном формате
            {
                if (copy.Search(id) == -1)//Если трекер с данным ID ещё не добавлен
                {
                    if (textBoxName.Text != null) name = textBoxName.Text;//Если имя задано
                    double lng;
                    double lat;
                    if (double.TryParse(textBoxLan.Text, out lat) && double.TryParse(textBoxLng.Text, out lng))//Если координаты заданы
                    {
                        if (name != string.Empty)//Если имя не пустое
                        {
                            Tracker tracker = new Tracker(name, id, DateTime.Now, lat, lng);
                            copy.AddTracker(tracker);
                        }
                        else
                        {
                            Tracker tracker = new Tracker(id.ToString(), id, DateTime.Now, lat, lng);
                            copy.AddTracker(tracker);
                        }
                    }
                    else
                    {
                        if (name != string.Empty)//Если имя не пустое
                        {
                            Tracker tracker = new Tracker(name, id, DateTime.Now, centerLat, centerLng);
                            copy.AddTracker(tracker);
                        }
                        else
                        {
                            Tracker tracker = new Tracker(id.ToString(), id, DateTime.Now, centerLat, centerLng);
                            copy.AddTracker(tracker);
                        }
                    }
                    textBoxID.Clear();
                    textBoxName.Clear();
                    textBoxLan.Clear();
                    textBoxLng.Clear();
                    textBoxID.Focus();
                }
                else
                {
                    MessageBox.Show("Трекер с данным ID уже добавлен", "Ошибка!");
                }
            }
            else MessageBox.Show("Введите корректный ID!", "Ошибка!");
            SetDataGrid(copy);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0)
            {
                List<int> indexes = new List<int>();
                for (int i = 0; i < dataGridView.SelectedCells.Count; i++)
                {
                    
                    if (!indexes.Contains(dataGridView.SelectedCells[i].RowIndex))
                    {
                        indexes.Add(dataGridView.SelectedCells[i].RowIndex);
                    }
                }
                List<int> ids = new List<int>();
                for (int i = 0; i < indexes.Count; i++)
                {
                    ids.Add((int)dataGridView[0, indexes[i]].Value);
                }
                foreach (int id in ids)
                {
                    copy.DeleteTracker(id);
                }
            }
            SetDataGrid(copy);
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                int row = e.RowIndex;
            int id = (int)dataGridView[0, row].Value;
            int column = e.ColumnIndex;
            switch (column)
            {
                case 1:
                    copy.UpdateTracker(id, DateTime.Now, dataGridView[1, row].Value.ToString());
                    break;
                case 3:
                    copy.UpdateTracker(id, DateTime.Now, Lat: double.Parse(dataGridView[3, row].Value.ToString()));
                    break;
                case 4:
                    copy.UpdateTracker(id, DateTime.Now, Lng: double.Parse(dataGridView[4, row].Value.ToString()));
                    break;
                default:
                    MessageBox.Show("Ошибка!");
                    break;
            }
            SetDataGrid(copy);
        }));
        }
    }
}
