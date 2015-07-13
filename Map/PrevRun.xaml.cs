using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using Map.Resources;
using System.Windows.Media.Imaging;

namespace Map
{
    public partial class PrevRun : PhoneApplicationPage
    {
        MapPolyline _line;
        public PrevRun()
        {
            InitializeComponent();
            Map.MapElements.Clear();
            _line = new MapPolyline();
            _line.StrokeColor = Colors.Blue;
            _line.StrokeThickness = 10;
            _line.StrokeDashed = true;
            Map.MapElements.Add(_line);
            sldZoomLevel.Value = Map.ZoomLevel = 16;
            sldZoomLevel.ValueChanged += sldZoomLevel_ValueChanged;
            AppName.Text = AppResources.YourRuns;

        }

        void sldZoomLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Map.ZoomLevel = e.NewValue;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("no"))
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)
                {
                    var data = (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
                    ARunData item = data.Single(p => p.No == int.Parse(NavigationContext.QueryString["no"]));
                    DrawRoute(item);
                }
            }
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                //int no = int.Parse(NavigationContext.QueryString["item"]);
                var item = (ARunData)IsolatedStorageSettings.ApplicationSettings["TempData"];
                DrawRoute(item);
            }
        }

        void DrawRoute(ARunData item)
        {
            if (item.geoCollection.Count != 0)
                Map.Center = item.geoCollection[0];
            else
            {
                MessageBox.Show(AppResources.NoGeoFoundMsg);
                NavigationService.GoBack();
                return;
            }
            Map.ZoomLevel = 16;
            Time.Text = item.datetime.ToShortTimeString();
            Date.Text = item.datetime.ToShortDateString();
            foreach (var geoCord in item.geoCollection)
            {
                _line.Path.Add(geoCord);
            }
            MapOverlay myLocationOverlay = new MapOverlay();
            BitmapImage tn = new BitmapImage();
            tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/finishflag.png", UriKind.Relative)).Stream);
            Image img = new Image();
            img.Source = tn;
            img.Height = 80;
            img.Width = 80;
            myLocationOverlay.Content = img;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = item.geoCollection[item.geoCollection.Count - 1];

            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            myLocationOverlay = new MapOverlay();
            tn = new BitmapImage();
            tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/GreenBall.png", UriKind.Relative)).Stream);
            img = new Image();
            img.Source = tn;
            img.Height = 25;
            img.Width = 25;
            myLocationOverlay.Content = img;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = item.geoCollection[0];

            myLocationLayer.Add(myLocationOverlay);
            Map.Layers.Add(myLocationLayer);
            Map.Center = item.geoCollection[item.geoCollection.Count / 2];
        }
        private void btHigher_Click(object sender, RoutedEventArgs e)
        {

            if (Map.Pitch >= 10)
                Map.Pitch -= 10;
        }

        private void btLower_Click(object sender, RoutedEventArgs e)
        {
            if (Map.Pitch <= 65)
                Map.Pitch += 10;
        }

    }
}