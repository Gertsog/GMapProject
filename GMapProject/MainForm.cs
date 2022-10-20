using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GMapProject
{
    public partial class MainForm : Form
    {
        private bool _isLeftButtonDown = false;
        private bool _dbExists = false;
        private DbContext _db = new DbContext();
        private GMapOverlay _markersOverlay;
        private MyMarker _currentMarker = null;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void gMapControl_Load(object sender, EventArgs e)
        {
            var startPosition = new GMap.NET.PointLatLng(54.9739675, 82.9242052);

            //Создание слоя и наполнение маркерами из базы
            _markersOverlay = new GMapOverlay("Markers");
            if (_db.DatabaseExists())
            {
                _dbExists = true;
                var markers = _db.GetMarkers();
                FillMarkersOverlay(_markersOverlay, markers);
                startPosition = markers.Count > 0 
                    ? markers.Last().Position
                    : startPosition;
            }
            else
            {
                MessageBox.Show("Database connection is missing!");
            }
            gMapControl.Overlays.Add(_markersOverlay);

            //Прочие параметры карты
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl.MinZoom = 2;
            gMapControl.MaxZoom = 16;
            gMapControl.Zoom = 10;
            gMapControl.Position = startPosition;
            gMapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl.CanDragMap = true;
            gMapControl.DragButton = MouseButtons.Left;
            gMapControl.ShowCenter = false;
            gMapControl.ShowTileGridLines = false;
        }

        //Установка маркера на карте
        private void gMapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var point = gMapControl.FromLocalToLatLng(e.X, e.Y);
                var marker = new MyMarker(point);
                if (_dbExists)
                {
                    var id = _db.CreateMarker(marker);
                    marker.Id = id;
                }
                _markersOverlay.Markers.Add(marker);
            }
        }

        private void gMapControl_OnMarkerEnter(GMapMarker item)
        {
            if (item is MyMarker && _currentMarker == null)
            {
                _currentMarker = item as MyMarker;
            }
        }

        private void gMapControl_OnMarkerLeave(GMapMarker item)
        {
            _currentMarker = null;
        }

        private void gMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isLeftButtonDown = true;
            }
        }

        private void gMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isLeftButtonDown = false;
                if (_currentMarker != null)
                {
                    if (_dbExists)
                    {
                        _db.UpdateMarker(_currentMarker);
                    }
                    _currentMarker = null;
                }
            }
        }
        private void gMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isLeftButtonDown && _currentMarker != null)
            {
                var point = gMapControl.FromLocalToLatLng(e.X, e.Y);
                _currentMarker.Position = point;
            }
        }

        //Добавление маркеров в список маркеров слоя
        private void FillMarkersOverlay(GMapOverlay overlay, List<MyMarker> markers)
        {
            foreach (var marker in markers)
            {
                overlay.Markers.Add(marker);
            }
        }


    }
}
