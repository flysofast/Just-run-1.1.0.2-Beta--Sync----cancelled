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
using Newtonsoft.Json;
using WindowsPhonePostClient;
using Map.Resources;

namespace Map
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            Rec.Visibility = Visibility.Collapsed;
            tbSyncing.Visibility = Visibility.Collapsed;

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Login") && (string)IsolatedStorageSettings.ApplicationSettings["Login"] != "")
            {
                tbSignedInAcc.Text = string.Format(AppResources.SignedInWith);
                hlSignedInUser.Content = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                grdNotSignedIn.Visibility = Visibility.Collapsed;
                grdSignedIn.Visibility = Visibility.Visible;
            }
            else
            {
                grdSignedIn.Visibility = Visibility.Collapsed;
                grdNotSignedIn.Visibility = Visibility.Visible;

            }
        }
        private void btSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (pwPassword.Password == pwPasswordRetype.Password && pwPasswordRetype.Password == pwPassword.Password)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("User", tbUsername.Text.Trim());
                parameters.Add("Password", pwPassword.Password);
                parameters.Add("Email", tbEmail.Text);

                PostClient proxy = new PostClient(parameters);
                proxy.DownloadStringCompleted += proxy_DownloadStringCompleted;
                proxy.DownloadStringAsync(new Uri("http://justrun.comlu.com/SignUp.php", UriKind.Absolute));
            }
            else
                MessageBox.Show(AppResources.PasswordsNotMatch);

        }

        private void proxy_DownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                MessageBox.Show(ServerResponse.GetMessage(e));
            }
            else
                MessageBox.Show(e.Error.ToString());
        }

        private void btSignIn_Click(object sender, RoutedEventArgs e)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Rec.Visibility = Visibility.Visible;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("User", tbUsername.Text.Trim());
            parameters.Add("Password", pwPassword.Password);

            PostClient signIn = new PostClient(parameters);
            signIn.DownloadStringCompleted += signIn_DownloadStringCompleted;
            signIn.DownloadStringAsync(new Uri("http://justrun.comlu.com/Login.php", UriKind.Absolute));
        }

        void signIn_DownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    if (ServerResponse.GetResult(e) == "OK")
                    {
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("Login"))
                            IsolatedStorageSettings.ApplicationSettings["Login"] = tbUsername.Text.Trim();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("Login", tbUsername.Text.Trim());
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("Password"))
                            IsolatedStorageSettings.ApplicationSettings["Password"] = pwPassword.Password;
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("Password", pwPassword.Password);
                        ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync = true;
                        IsolatedStorageSettings.ApplicationSettings.Save();
                        NavigationService.Navigate(new Uri("/UserAccountManager.xaml?User=" + tbUsername.Text.Trim() + "&Password=" + pwPassword.Password, UriKind.RelativeOrAbsolute));
                    }
                    else
                        MessageBox.Show(ServerResponse.GetMessage(e));

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
            else
                MessageBox.Show(e.Error.ToString());
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            Rec.Visibility = Visibility.Collapsed;
        }

        private void btSignOut_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Login"] = "";
            IsolatedStorageSettings.ApplicationSettings["Password"] = "";
            IsolatedStorageSettings.ApplicationSettings.Save();
            ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync = false;
            IsolatedStorageSettings.ApplicationSettings.Save();

            grdSignedIn.Visibility = Visibility.Collapsed;
            grdNotSignedIn.Visibility = Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
        }

        private void hlSignedInUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                string password = (string)IsolatedStorageSettings.ApplicationSettings["Password"];
                NavigationService.Navigate(new Uri("/UserAccountManager.xaml?User=" + user + "&Password=" + password, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        List<ARunData> TotalData =new List<ARunData>();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                string password = (string)IsolatedStorageSettings.ApplicationSettings["Password"];
                if (((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync != true &&
                    (MessageBox.Show(AppResources.TurnOnSyncingMsg, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK))
                {
                    ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync = true;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
                if (((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync == true)
                    SyncData(user, password);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void UploadData(List<ARunData> totalData, string user, string password)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Rec.Visibility = Visibility.Visible;
            tbSyncing.Visibility = Visibility.Visible;
            var uploadData = new List<ARunUploadData>();
            if (totalData != null && totalData.Count > 0)
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
                        MessageBox.Show(AppResources.SuccessfullyUpdated);
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)

                            foreach (var item in (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"])
                                item.IsSynced = true;
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                    else
                        MessageBox.Show(AppResources.FailedAction);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
            else
                MessageBox.Show(e.Error.ToString());
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            Rec.Visibility = Visibility.Collapsed;
            tbSyncing.Visibility = Visibility.Collapsed;
        }
        public void SyncData(string user, string password)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Rec.Visibility = Visibility.Visible;
            tbSyncing.Visibility = Visibility.Visible;
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

                UpdateData(ServerResponse.GetMessage(e));
                //tbARunData.Text = result.Substring(start, length);
                List<ARunData> localData = new List<ARunData>();

                if (IsolatedStorageSettings.ApplicationSettings.Contains("RunData") && ((List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"]).Count != 0)
                {
                    localData = (List<ARunData>)IsolatedStorageSettings.ApplicationSettings["RunData"];
                    foreach (var item in localData)
                        if (!item.IsSynced)
                            TotalData.Add(item);
                }
               
                string user = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
                string password = (string)IsolatedStorageSettings.ApplicationSettings["Password"];
                UploadData(TotalData, user, password);
            }
            else
                MessageBox.Show(e.Error.ToString());
        }
        void UpdateData(string JsonString)
        {
            try
            {
                List<ARunUploadData> downloadData = JsonConvert.DeserializeObject<List<ARunUploadData>>(JsonString);
                if (downloadData != null)
                    foreach (var item in downloadData)
                        TotalData.Add(item.ToARunData());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            Rec.Visibility = Visibility.Collapsed;
        }




    }
}