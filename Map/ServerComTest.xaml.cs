using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using WindowsPhonePostClient;
using System.IO.IsolatedStorage;


namespace Map
{
    public partial class ServerComTest : PhoneApplicationPage
    {
        public ServerComTest()
        {
            InitializeComponent();
        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            //IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            //var LocalData = ((List<ARunData>)settings["RunData"]);
            //var uploadData = new List<ARunUploadData>();
            //if (LocalData != null)
            //    foreach (var item in LocalData)
            //    {
            //        ARunUploadData a = new ARunUploadData();
            //        a.ParseFrom(item);
            //        uploadData.Add(a);
            //    }

            string jsonString = JsonConvert.SerializeObject(DateTime.Now.ToUniversalTime());

            //tbARunData.Text = jsonString;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("User", "admin");
            //parameters.Add("Password", "admin");
            //parameters.Add("RunData", jsonString);
            //var userInfo = (User)IsolatedStorageSettings.ApplicationSettings["UserInfo"];
            //parameters.Add("Gender", userInfo.gender);
            //parameters.Add("Weight", userInfo.weight);
            //parameters.Add("Age", userInfo.age);
            //parameters.Add("Grade", userInfo.grade*100);
            parameters.Add("name", "aaaanasmd");
            PostClient proxy = new PostClient(parameters);
            proxy.DownloadStringCompleted += proxy_UploadDownloadStringCompleted;
            //Newtonsoft.Json.Converters.JavaScriptDateTimeConverter();
            proxy.DownloadStringAsync(new Uri("http://justrun.somee.com/login.aspx", UriKind.Absolute));
            //NavigationService.Navigate(new Uri("/DownloadDataTest.xaml",UriKind.Relative));
        }
        private void proxy_UploadDownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {

                    //MessageBox.Show(ServerResponse.GetMessage(e));
                    MessageBox.Show(e.Result.ToString());

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
            else
                MessageBox.Show(e.Error.ToString());
        }

        private void btDownload_Click(object sender, RoutedEventArgs e)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("User", "ac");
            parameters.Add("Password", "ac");

            PostClient proxy = new PostClient(parameters);
            proxy.DownloadStringCompleted += proxy_DownloadDownloadStringCompleted;

            proxy.DownloadStringAsync(new Uri("http://justrun.comlu.com/downloadData.php", UriKind.Absolute));

        }

        private void proxy_DownloadDownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string result = e.Result.ToString();
                string startString = "<JustRunServerResponse>";
                int start = result.IndexOf(startString) + startString.Length;
                int length = result.IndexOf(("</JustRunServerResponse>")) - start;
                UpdateData(result.Substring(start, length));
                tbARunData.Text = result.Substring(start, length);


            }
            else
                MessageBox.Show(e.Error.ToString());
        }
        void UpdateData(string JsonString)
        {
            try
            {
                List<ARunUploadData> downloadData = JsonConvert.DeserializeObject<List<ARunUploadData>>(JsonString);
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                var data = new List<ARunData>();
                if (downloadData != null)
                    foreach (var item in downloadData)
                    {
                        data.Add(item.ToARunData());
                    }
                settings["RunData"] = data;
                settings.Save();
                MessageBox.Show("OK: " + data[3].datetime.ToString());

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btMainpage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));

        }

    }
}