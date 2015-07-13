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
using WindowsPhonePostClient;
using Newtonsoft.Json;
using Map.Resources;
using Microsoft.Phone.Tasks;

namespace Map
{
    public partial class Welcome : PhoneApplicationPage
    {
        List<ARunData> TotalData = new List<ARunData>();
        public Welcome()
        {
            InitializeComponent();
            prgBar.IsIndeterminate = true;
            int launchCount = (int)IsolatedStorageSettings.ApplicationSettings["LaunchCount"];
            bool newRelease = (bool)IsolatedStorageSettings.ApplicationSettings["NewRelease"];
            if (newRelease)
            {
                MessageBox.Show(AppResources.NewReleaseMsg, "Change logs", MessageBoxButton.OK);
                IsolatedStorageSettings.ApplicationSettings["NewRelease"] = false;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            if (launchCount == 3)
            {
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
                IsolatedStorageSettings.ApplicationSettings["LaunchCount"] = launchCount + 1;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["LaunchCount"] = launchCount + 1;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            this.Loaded += Welcome_Loaded;

        }
        void NavigateToMainPage()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("FirstRunAndNotRegistered") && !(bool)settings["FirstRunAndNotRegistered"])
            {
                //NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                //Map.App.RootFrame.Navigate(new Uri("/ServerComTest.xaml", UriKind.Relative));
                //Map.App.RootFrame.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                Map.App.RootFrame.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/UserInfo.xaml", UriKind.Relative));
            }
        }
        void Welcome_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                string user = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                string password = (string)IsolatedStorageSettings.ApplicationSettings["Password"];
                if (user.Length > 0 && ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync == true)
                    SyncData(user, password);
                else NavigateToMainPage();
            }
            catch
            {
                NavigateToMainPage();
            }
        }

        private void UploadData(List<ARunData> totalData, string user, string password)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            var uploadData = new List<ARunUploadData>();
            foreach (var item in totalData)
            {
                ARunUploadData a = new ARunUploadData();
                a.ParseFrom(item);
                uploadData.Add(a);
            }

            string jsonString = JsonConvert.SerializeObject(uploadData);

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("User", user);
            parameters.Add("Password", password);
            parameters.Add("RunData", jsonString);
            var userInfo = (User)IsolatedStorageSettings.ApplicationSettings["UserInfo"];
            parameters.Add("Gender", userInfo.gender);
            parameters.Add("Weight", userInfo.weight);
            parameters.Add("Age", userInfo.age);
            parameters.Add("Grade", userInfo.grade * 100);

            PostClient proxy = new PostClient(parameters);
            proxy.DownloadStringCompleted += proxy_UploadDownloadStringCompleted;
            //Newtonsoft.Json.Converters.JavaScriptDateTimeConverter();
            proxy.DownloadStringAsync(new Uri("http://justrun.comlu.com/uploadData.php", UriKind.Absolute));
            //NavigationService.Navigate(new Uri("/DownloadDataTest.xaml",UriKind.Relative));
            if (totalData.Count != 0)
            {
                IsolatedStorageSettings.ApplicationSettings["RunData"] = totalData;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }

        }

        private void proxy_UploadDownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    if (ServerResponse.GetResult(e) == "OK")
                    {
                        //MessageBox.Show(AppResources.SuccessfullyUpdated);
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)
                            foreach (var item in (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"])
                                item.IsSynced = true;
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
                catch
                {
                }

            }


            NavigateToMainPage();

        }
        public void SyncData(string user, string password)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("User", user);
            parameters.Add("Password", password);

            PostClient proxy = new PostClient(parameters);
            proxy.DownloadStringCompleted += proxy_DownloadDownloadStringCompleted;

            proxy.DownloadStringAsync(new Uri("http://justrun.comlu.com/downloadData.php", UriKind.Absolute));
        }

        public void proxy_DownloadDownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                //if (ServerResponse.GetMessage(e) != "")
                //{
                UpdateData(ServerResponse.GetMessage(e));
                //tbARunData.Text = result.Substring(start, length);
                string user = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                string password = (string)IsolatedStorageSettings.ApplicationSettings["Password"];

                if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)
                {

                    List<ARunData> localData = new List<ARunData>();
                    localData = (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
                    foreach (var item in localData)
                        if (!item.IsSynced)
                            TotalData.Add(item);
                }

                if (TotalData.Count > 0)
                    UploadData(TotalData, user, password);
                // }
                //else UploadData(localData, user, password);
            }
            else
                NavigateToMainPage();
        }
        void UpdateData(string JsonString)
        {
            try
            {
                List<ARunUploadData> downloadData = JsonConvert.DeserializeObject<List<ARunUploadData>>(JsonString);
                if (downloadData != null && downloadData.Count > 0)
                    foreach (var item in downloadData)
                        TotalData.Add(item.ToARunData());

            }
            catch
            {
            }
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
        }


    }
}