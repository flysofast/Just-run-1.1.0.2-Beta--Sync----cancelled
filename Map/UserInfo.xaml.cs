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
using Map.Resources;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Map
{
    public partial class UserInfo : PhoneApplicationPage
    {
        User user = new User();
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        public UserInfo()
        {
            InitializeComponent();
            if ((bool)settings["FirstRunAndNotRegistered"])
            {
                MessageBox.Show(AppResources.FirstRegMsg);
                settings["FirstRunAndNotRegistered"] = false;
            }

            user = (User)settings["UserInfo"];

            tbAge.Text = user.age.ToString();

            tbGrade.Text = (user.grade * 100).ToString();

            tbWeight.Text = user.weight.ToString();

            if (user.gender == null)
                lpGender.SelectedIndex = 2;
            else
                lpGender.SelectedIndex = user.gender == true ? 0 : 1;

            BuildApplicationBar();
            if (!settings.Contains("RunData") || ((List<ARunData>)settings["RunData"]).Count == 0)
            {
                tbLastRun.Text = AppResources.YourLastRun + "--";
                tbAvgSpeed.Text = AppResources.AvgSpeed + "--";
                tbTotalCalories.Text = AppResources.TotalCalories + "--";
                tbTotalDistance.Text = AppResources.TotalDistance + "--";
                tbTotalDuration.Text = AppResources.TotalDuration + "--";
                return;
            }
            List<ARunData> data = (List<ARunData>)settings["RunData"];
            data = (List<ARunData>)settings["RunData"];

            Update(data);
        }

        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            try
            {
                ApplicationBar = new ApplicationBar();

                // Create a new button and set the text value to the localized string from AppResources.
                ApplicationBarIconButton btTotal = new ApplicationBarIconButton(new Uri("/Assets/AppBar/all.png", UriKind.Relative));
                btTotal.Text = AppResources.Total;
                btTotal.Click += btTotal_Click;
                ApplicationBar.Buttons.Add(btTotal);

                ApplicationBarIconButton btWeek = new ApplicationBarIconButton(new Uri("/Assets/AppBar/week.png", UriKind.Relative));
                btWeek.Text = AppResources.Week;
                btWeek.Click += btWeek_Click;
                ApplicationBar.Buttons.Add(btWeek);

                ApplicationBarIconButton btMonth = new ApplicationBarIconButton(new Uri("/Assets/AppBar/month.png", UriKind.Relative));
                btMonth.Text = AppResources.Month;
                btMonth.Click += btMonth_Click;
                ApplicationBar.Buttons.Add(btMonth);

                ApplicationBarIconButton btYear = new ApplicationBarIconButton(new Uri("/Assets/AppBar/year.png", UriKind.Relative));
                btYear.Text = AppResources.Year;
                btYear.Click += btYear_Click;
                ApplicationBar.Buttons.Add(btYear);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }



        }

        void btYear_Click(object sender, EventArgs e)
        {
            if (!settings.Contains("RunData")||((List<ARunData>)settings["RunData"]).Count==0) return;
            try
            {
                IEnumerable<ARunData> data = (List<ARunData>)settings["RunData"];
                Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
                int yearNo = cal.GetYear(DateTime.Now);
                data = data.Where(p => cal.GetYear(p.datetime) == yearNo);
                Update(data);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        void btMonth_Click(object sender, EventArgs e)
        {
            if (!settings.Contains("RunData") || ((List<ARunData>)settings["RunData"]).Count == 0) return;
            try
            {
                IEnumerable<ARunData> data = (List<ARunData>)settings["RunData"];
                Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
                int monthNo = cal.GetMonth(DateTime.Now);
                data = data.Where(p => cal.GetMonth(p.datetime) == monthNo);
                Update(data);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        void btWeek_Click(object sender, EventArgs e)
        {
            if (!settings.Contains("RunData") || ((List<ARunData>)settings["RunData"]).Count == 0) return;
            try
            {
                IEnumerable<ARunData> data = (List<ARunData>)settings["RunData"];
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dfi.Calendar;
                int weekNo = cal.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                data = data.Where(p => cal.GetWeekOfYear(p.datetime, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) == weekNo);
                Update(data);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        void btTotal_Click(object sender, EventArgs e)
        {
            if (!settings.Contains("RunData") || ((List<ARunData>)settings["RunData"]).Count == 0) return;
            List<ARunData> data = (List<ARunData>)settings["RunData"];
            data = (List<ARunData>)settings["RunData"];

            Update(data);
        }

        private void Update(IEnumerable<ARunData> data)
        {
            tbTotalCalories.Text = AppResources.TotalCalories + data.Sum(p => p.BurnedCalories).ToString("F") + " cal";
            tbAvgSpeed.Text = AppResources.AvgSpeed + data.Average(p => p.AvgSpeed).ToString("F") + " km/h";
            tbTotalDistance.Text = AppResources.TotalDistance + data.Sum(p => p.Distance).ToString("F") + " km";
            tbTotalDuration.Text = AppResources.TotalDuration + getDurationString(data);
            tbAvgPace.Text = AppResources.AvgPace + data.Average(p => p.AvgPace).ToString("F") + " min/km";

        }
        private string getDurationString(IEnumerable<ARunData> data)
        {
            int result = 0;
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
        private void btSaveUserInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                prgBar.IsIndeterminate = true;
                prgBar.Visibility = Visibility.Visible;
                Rec.Visibility = Visibility.Visible;

                bool? gender;
                if (lpGender.SelectedIndex == 2)
                    gender = null;
                else
                    gender = lpGender.SelectedIndex == 0 ? true : false;

                User user = new User(int.Parse(tbAge.Text), double.Parse(tbWeight.Text), double.Parse(tbGrade.Text) / 100, gender);
                if (!settings.Contains("UserInfo"))
                {
                    settings.Add("UserInfo", user);
                }
                else settings["UserInfo"] = user;
                settings.Save();
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                else
                    NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));

            }
            catch
            {
                MessageBox.Show(AppResources.InvalidUserInfo, "Warning", MessageBoxButton.OK);
                return;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                if (settings.Contains("RunData")&&((List<ARunData>)settings["RunData"]).Count!=0)
                {

                    List<ARunData> data = (List<ARunData>)settings["RunData"];
                    var lastRun = data.Single(p => p.datetime == data.Max(q => q.datetime));
                    tbLastRun.Text = AppResources.YourLastRun + lastRun.datetime.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                            ApplicationBar.IsVisible = false;
                        }
                        break;
                    case 1:
                        {
                            ApplicationBar.IsVisible = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}