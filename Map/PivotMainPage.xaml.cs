using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows.Threading;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using System.IO.IsolatedStorage;
using System.Globalization;
using NExtra.Geo;
using System.Windows.Media.Imaging;
using Windows.Devices.Geolocation;
using Map.Resources;
using System.Text.RegularExpressions;

namespace Map
{
    public partial class PivotMainPage : PhoneApplicationPage
    {
        double distance = 0;
        bool running = false;
        MapPolyline _line;
        GeoCoordinateCollection geoCollection = new GeoCoordinateCollection();
        private GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
        GeoCoordinate prevGeoCoordinate = new GeoCoordinate();
        double TimeCount;
        int startTime;
        int pauseTime;
        double AvgSpeed;
        double weight;
        double grade;
        DispatcherTimer timer;
        ApplicationBarIconButton btSelect_AB = new ApplicationBarIconButton();
        public PivotMainPage()
        {

            InitializeComponent();

            prgBar.IsIndeterminate = true;

            BuildLocalizedApplicationBar();
            btSelect_AB.Click += btSelect_Click;
            timer = new DispatcherTimer();
            prevGeoCoordinate = null;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.MovementThreshold = 5;
            watcher.Start();
            BackKeyPress += OnBackKeyPressed;
            tbCalories.Text = AppResources.BurnedCalories + "0.00 cal";
            tbDistance.Text = AppResources.Distance + (distance / 1000).ToString("F") + " km";
            ChangeTile("Total");

        }
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ApplySettings();
            // InitializeComponent();
            BuildLocalizedApplicationBar();
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            if (running && btn.IsEnabled == true)
            {
                btn.Text = AppResources.Pause;
                btn.IconUri = new Uri(@"/Assets/AppBar/pause.png", UriKind.Relative);
            }
            if (!running && btn.IsEnabled == true)
            {
                btn.Text = AppResources.Resume;
                btn.IconUri = new Uri(@"/Assets/AppBar/play.png", UriKind.Relative);
            }
            try
            {
                Geolocator locator = new Geolocator();
                await locator.GetGeopositionAsync();

            }
            catch
            {
                MessageBox.Show(AppResources.GPSErrorMessage, AppResources.Attention, MessageBoxButton.OK);
            }

        }

        void ApplySettings()
        {

            AppSettings appSettings = new AppSettings();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("AppSettings"))
                appSettings = (AppSettings)settings["AppSettings"];

            Thread.CurrentThread.CurrentCulture = new CultureInfo(appSettings._language);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Dispatcher.BeginInvoke(() =>
            {
                MyFirstMap.PedestrianFeaturesEnabled = appSettings._3DObjects;
                MyFirstMap.LandmarksEnabled = appSettings._3DObjects;
            });
            MyFirstMap.ZoomLevel = appSettings._DefaultZoomLevel;
            MyFirstMap.Pitch = appSettings._DefaultPitchLevel;
            User user = new User();
            if (settings.Contains("UserInfo"))
                user = (User)settings["UserInfo"];

            grade = user.grade;
            weight = user.weight;


            if (longListMultiSelector.EnforceIsSelectionEnabled)
                btSelect_Delete_Appearance("Delete");
            else btSelect_Delete_Appearance("Select");
            if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData")&&((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count!=0)
                this.longListMultiSelector.ItemsSource = this.GetRunGroups();

            tbFindingLocation.Text = AppResources.FindingLocation;

        }

        void mniAdd_AB_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddRun.xaml", UriKind.Relative));
        }
        void OnBackKeyPressed(object sender, CancelEventArgs e)
        {
            //while (NavigationService.CanGoBack)
            //    NavigationService.RemoveBackEntry();
            if (!longListMultiSelector.EnforceIsSelectionEnabled)
            {
                var result = MessageBox.Show(AppResources.ExitConfirmMessage, AppResources.Attention, MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    ChangeTile("Total");
                    Map.App.Current.Terminate();
                }
                e.Cancel = true;
            }
            else
            {
                longListMultiSelector.EnforceIsSelectionEnabled = false;
                btSelect_Delete_Appearance("Select");
                e.Cancel = true;
            }
        }
        void btSelect_Delete_Appearance(string btName)
        {
            if (btName == "Delete")
            {
                btSelect_AB.Text = AppResources.Delete;
                btSelect_AB.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative);
            }
            if (btName == "Select")
            {
                btSelect_AB.Text = AppResources.Select;
                btSelect_AB.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative);
            }
        }
        int lastTickCount = System.Environment.TickCount;
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            tbFindingLocation.Visibility = Visibility.Collapsed;
            try
            {


                TimeSpan runTime = TimeSpan.FromMilliseconds(System.Environment.TickCount - lastTickCount);
                var CurrentGeoCord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
                if (prevGeoCoordinate == null) prevGeoCoordinate = CurrentGeoCord;
                MarkCurrentLocation(CurrentGeoCord);
                double temDistance = prevGeoCoordinate.GetDistanceTo(CurrentGeoCord);

                lastTickCount = System.Environment.TickCount;
                if (running)
                {

                    distance += temDistance;
                    tbDistance.Text = AppResources.Distance + (distance / 1000).ToString("F") + " km";
                    double AmountOfBurnedCalories = 0;
                    if (TimeCount >= 0)
                    {
                        AvgSpeed = (distance / TimeCount) * 3.6;
                        AmountOfBurnedCalories = BurnedCalories();
                    }
                    else
                        AmountOfBurnedCalories = AvgSpeed = 0;
                    _line.Path.Add(CurrentGeoCord);
                    geoCollection.Add(CurrentGeoCord);

                    tbCalories.Text = AppResources.BurnedCalories + AmountOfBurnedCalories.ToString("F") + " cal";

                    tbPace.Text = "Pace: " + (runTime.TotalMinutes / (temDistance / 1000)).ToString("F") + " min/km";

                }
                if (((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._AutoHeading)
                {
                    PositionHandler handler = new PositionHandler();
                    var heading = handler.CalculateBearing(new Position(prevGeoCoordinate), new Position(CurrentGeoCord));
                    MyFirstMap.SetView(CurrentGeoCord, MyFirstMap.ZoomLevel, heading, MapAnimationKind.Parabolic);
                }

                tbTemSpeed.Text = AppResources.Speed + ((temDistance / runTime.TotalSeconds) * 3.6).ToString("F") + " km/h";
                if (running)
                    ChangeTile("Running");
                prevGeoCoordinate = CurrentGeoCord;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        double BurnedCalories()
        {
            if (tbDistance.Text == AppResources.Distance + "0.00 km") return 0;
            return ((((3.5 + 16.667 * 0.2 * AvgSpeed + AvgSpeed * grade * 1.8) / 3.5) * weight) * (TimeCount / 3600));
        }
        void timer_Tick(object sender, EventArgs e)
        {
            int tickcount = System.Environment.TickCount;
            if (tickcount - startTime >= 0)
            {
                TimeSpan runTime = TimeSpan.FromMilliseconds(tickcount - startTime);
                TimeCount = runTime.TotalSeconds;
            }
            else TimeCount = 0;

            int _timeCount = (int)TimeCount;
            //tbTimeCount.Text = "Thời gian: " + runTime.ToString(@"hh\:mm\:ss") + " s";
            tbTimeCount.Text = string.Format("{0:00}:{1:00}:{2:00}", _timeCount / 3600, (_timeCount / 60) % 60, _timeCount % 60);
        }


        void MarkCurrentLocation(GeoCoordinate CurrentGeoCord)
        {
            MapOverlay myLocationOverlay = new MapOverlay();
            BitmapImage tn = new BitmapImage();
            if (((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._AutoHeading)
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/triangle-icon.png", UriKind.Relative)).Stream);
            else
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/Appbar/location.png", UriKind.Relative)).Stream);
            Image img = new Image();
            img.Source = tn;
            img.Height = 60;
            img.Width = 30;
            myLocationOverlay.Content = img;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = CurrentGeoCord;

            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);
            MyFirstMap.Layers.Clear();
            MyFirstMap.Layers.Add(myLocationLayer);
            MyFirstMap.Center = CurrentGeoCord;

        }

        private void btZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (MyFirstMap.Pitch >= 10)
                this.MyFirstMap.Pitch -= 10;
        }

        private void btZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (MyFirstMap.Pitch <= 65)
                this.MyFirstMap.Pitch += 10;
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            distance = 0;
            tbCalories.Visibility = Visibility.Visible;
            TimeCount = 0;
            TimeViewbox.Visibility = Visibility.Visible;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn.IsEnabled = true;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn.IsEnabled = true;

            MyFirstMap.MapElements.Clear();
            _line = new MapPolyline();
            _line.StrokeColor = Colors.Blue;
            _line.StrokeThickness = 10;
            _line.StrokeDashed = true;
            MyFirstMap.MapElements.Add(_line);
            watcher.Start();
            timer.Start();
            startTime = System.Environment.TickCount;
            running = true;
            btStart.Visibility = Visibility.Collapsed;
            btFinished.Visibility = Visibility.Visible;
            geoCollection = new GeoCoordinateCollection();
            
        }
        private void btFinished_Click(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn.IsEnabled = false;
            btn.Text = AppResources.Pause;
            btn.IconUri = new Uri(@"/Assets/AppBar/pause.png", UriKind.Relative);
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn.IsEnabled = false;
            timer.Stop();
            //watcher.Stop();
            // MyFirstMap.MapElements.Clear();
            TimeViewbox.Visibility = Visibility.Collapsed;
            //btn.IsEnabled = false;
            running = false;
            btFinished.Visibility = Visibility.Collapsed;
            btStart.Visibility = Visibility.Visible;
            if (MessageBox.Show(AppResources.SaveConfirmMessage, AppResources.Attention, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ARunData _aRunData = new ARunData();
                _aRunData.geoCollection = geoCollection;
                _aRunData.AvgSpeed = AvgSpeed;
                _aRunData.Distance = distance / 1000;

                int _timeCount = (int)TimeCount;
                _aRunData.Duration = string.Format("{0}h {1}m {2}s", _timeCount / 3600, (_timeCount / 60) % 60, _timeCount % 60);
                _aRunData.BurnedCalories = BurnedCalories();
                _aRunData.datetime = DateTime.Now - TimeSpan.FromSeconds(TimeCount);
                _aRunData.Save();
            }
            ChangeTile("Total");

            tbTimeCount.Text = "00:00:00";
            tbDistance.Text = AppResources.Distance + "0.00 km";
            tbCalories.Text = AppResources.BurnedCalories + "0.00 cal";

        }
        void ChangeTile(string status)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("RunData") || ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count == 0) return;
            List<ARunData> data = (List<ARunData>)settings["RunData"];
            data = (List<ARunData>)settings["RunData"];
            if (status == "Total")
                ShellTile.ActiveTiles.First().Update(new IconicTileData()
                {
                    WideContent1 = AppResources.TotalDistance + data.Sum(p => p.Distance).ToString("F") + " km",
                    WideContent2 = AppResources.TotalCalories + data.Sum(p => p.BurnedCalories).ToString("F") + " cal",
                    WideContent3 = AppResources.TotalDuration + getTotalDurationString(data),
                    Title = AppResources.AppName

                });

            if (status == "Running")
                ShellTile.ActiveTiles.First().Update(new IconicTileData()
                {
                    WideContent1 = tbDistance.Text,
                    WideContent2 = tbCalories.Text,
                    WideContent3 = tbPace.Text,
                    Title = AppResources.Running
                });

            if (status == "Paused")
                ShellTile.ActiveTiles.First().Update(new IconicTileData()
                {
                    Title = AppResources.Paused
                });
        }
        private string getTotalDurationString(IEnumerable<ARunData> data)
        {
            int result = 0;
            if (data != null)
                foreach (var run in data)
                {
                    string duration = run.Duration;
                    duration = Regex.Replace(duration, @"[^\d]", " ");
                    duration = Regex.Replace(duration, @"\s+", " ");
                    string[] time = duration.Trim().Split(' ');
                    for (int i = 0; i < 3; i++)
                    {
                        int baseNum = i == 0 ? 3600 : i == 1 ? 60 : 1;
                        result += int.Parse(time[i]) * baseNum;
                    }
                }
            return string.Format("{0}h {1}m {2}s", result / 3600, (result / 60) % 60, result % 60);
        }
        async private void btShowLocation_Click(object sender, EventArgs e)
        {
            try
            {
                prgBar.Visibility = Visibility.Visible;
                prgBar.IsIndeterminate = true;
                tbFindingLocation.Visibility = Visibility.Visible;

                MyFirstMap.ZoomLevel = 17;

                Geolocator locator = new Geolocator();
                locator.DesiredAccuracy = PositionAccuracy.High;
                locator.MovementThreshold = 1;
                Geoposition myLocation;
                myLocation = await locator.GetGeopositionAsync();
                prgBar.Visibility = Visibility.Collapsed;
                tbFindingLocation.Visibility = Visibility.Collapsed;
                prgBar.IsIndeterminate = false;
                GeoCoordinate geoCord = new GeoCoordinate(myLocation.Coordinate.Latitude, myLocation.Coordinate.Longitude);
                MyFirstMap.Center = geoCord;
                MarkCurrentLocation(geoCord);
                prevGeoCoordinate = geoCord;


            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.GPSErrorMessage, AppResources.Attention, MessageBoxButton.OK);
            }
        }

        private void abmnSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void abmnAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/about.xaml", UriKind.Relative));
        }

        private void btReset_AB_Click(object sender, EventArgs e)
        {
            MyFirstMap.MapElements.Clear();
            _line = new MapPolyline();
            _line.StrokeDashed = true;
            _line.StrokeColor = Colors.Blue;
            _line.StrokeThickness = 10;
            MyFirstMap.MapElements.Add(_line);
            TimeCount = 0;
            distance = 0;
            startTime = System.Environment.TickCount;
            geoCollection = new GeoCoordinateCollection();
            tbTimeCount.Text = "00:00:00";
            tbDistance.Text = AppResources.Distance + "0.00 km";
            tbCalories.Text = AppResources.BurnedCalories + "0.00 cal";


        }

        private void btPause_AB_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];

            if (btn.Text == AppResources.Resume)
            {
                timer.Start();
                startTime += System.Environment.TickCount - pauseTime;
                btn.Text = AppResources.Pause;
                btn.IconUri = new Uri(@"/Assets/AppBar/pause.png", UriKind.Relative);
                running = true;
                _line = new MapPolyline();
                _line.StrokeColor = Colors.Blue;
                _line.StrokeThickness = 10;
                _line.StrokeDashed = true;
                MyFirstMap.MapElements.Add(_line);
            }
            else if (btn.Text == AppResources.Pause)
            {
                pauseTime = System.Environment.TickCount;
                timer.Stop();

                btn.Text = AppResources.Resume;
                btn.IconUri = new Uri(@"/Assets/AppBar/play.png", UriKind.Relative);
                running = false;
                ChangeTile("Paused");
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            //ApplicationBar.BackgroundColor = Colors.DarkGray; 
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.Text = AppResources.MyLocation;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn.Text = AppResources.Pause;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn.Text = AppResources.Restart;
            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem mnuitem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            mnuitem.Text = AppResources.Add;

            mnuitem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
            mnuitem.Text = AppResources.UserInfo;

            mnuitem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[2];
            mnuitem.Text = AppResources.Settings;

            mnuitem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[3];
            mnuitem.Text = AppResources.SyncingAcc;

            mnuitem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[4];
            mnuitem.Text = AppResources.About;


        }

        private void abmnUserInfo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/userinfo.xaml", UriKind.Relative));
        }



        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch ((sender as Pivot).SelectedIndex)
                {
                    case 0:
                        // this.ApplicationBar = AppBar1;
                        {
                            if (ApplicationBar.Buttons.Contains(btSelect_AB))
                                ApplicationBar.Buttons.Remove(btSelect_AB);

                        }
                        break;
                    case 1:
                        {

                            if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)
                                this.longListMultiSelector.ItemsSource = this.GetRunGroups();
                            ApplicationBar.Buttons.Add(btSelect_AB);

                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        void btCancel_AB_Click(object sender, EventArgs e)
        {
        }

        void btSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (btSelect_AB.Text == AppResources.Select)
                {
                    btSelect_Delete_Appearance("Delete");
                    longListMultiSelector.EnforceIsSelectionEnabled = true;
                }
                else
                {
                    if (longListMultiSelector.SelectedItems.Count != 0)
                        if (MessageBox.Show(AppResources.DeleteConfirmMsg, AppResources.Attention, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            var data = (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
                            var selectedItems = longListMultiSelector.SelectedItems;
                            foreach (ARunData item in selectedItems)
                                data.Remove(data.First(p => p.No == item.No));

                            IsolatedStorageSettings.ApplicationSettings.Save();
                            this.longListMultiSelector.ItemsSource = this.GetRunGroups();
                        }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<Group<ARunData>> GetRunGroups()
        {
            IEnumerable<ARunData> runList = GetRunDataList();
            return GetItemGroups(runList, c => c.datetime.ToShortDateString());
        }

        private static IEnumerable<ARunData> GetRunDataList()
        {

            return (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
        }



        private static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
        {
            IEnumerable<Group<T>> groupList = from item in itemList
                                              group item by getKeyFunc(item) into g
                                              orderby g.Key descending
                                              select new Group<T>(g.Key, g);

            return groupList.ToList();
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var grid = (Grid)sender;

                TextBlock tb = (TextBlock)grid.FindName("tbItemNo");
                var data = (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
                ARunData item = data.Single(p => p.No == int.Parse(tb.Text));
                NavigationService.Navigate(new Uri("/PrevRun.xaml?no=" + item.No, UriKind.Relative));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void abmnSyncingAcc_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/login.xaml", UriKind.Relative));
        }



    }
}