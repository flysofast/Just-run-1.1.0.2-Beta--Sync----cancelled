using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Map.Resources;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using WindowsPhonePostClient;

namespace Map
{
    public partial class UserAccountManager : PhoneApplicationPage
    {
        public UserAccountManager()
        {
            InitializeComponent();
        }
        string user = "";
        string password = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                user = NavigationContext.QueryString["User"];
                password = NavigationContext.QueryString["Password"];
                tbHelloUser.Text = string.Format(AppResources.HelloUser, user);
            }
            catch
            {
                NavigationService.Navigate(new Uri("/login.xaml", UriKind.Relative));
                return;
            }
            DownloadData();
            BuildLocalizedApplicationBar();
        }

        public void DownloadData()
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Rec.Visibility = Visibility.Visible;
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
                string result = e.Result.ToString();
                string startString = "<JustRunServerResponse>";
                int start = result.IndexOf(startString) + startString.Length;
                int length = result.IndexOf(("</JustRunServerResponse>")) - start;
                if (length >= 0)
                    UpdateData(result.Substring(start, length));
                //tbARunData.Text = result.Substring(start, length);
            }
            else
                MessageBox.Show(e.Error.ToString());
        }
        void UpdateData(string JsonString)
        {
            try
            {
                List<ARunUploadData> downloadData = JsonConvert.DeserializeObject<List<ARunUploadData>>(JsonString);
                data = new List<ARunData>();
                if (downloadData != null && downloadData.Count!=0)
                {
                    foreach (var item in downloadData)
                        data.Add(item.ToARunData());

                    this.longListMultiSelector.ItemsSource = this.GetRunGroups();
                }
                else
                    MessageBox.Show(AppResources.EmptyData);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            prgBar.IsIndeterminate = false;
            prgBar.Visibility = Visibility.Collapsed;
            Rec.Visibility = Visibility.Collapsed;
        }


        List<ARunData> data = null;

        private void BuildLocalizedApplicationBar()
        {
            try
            {
                ApplicationBar = new ApplicationBar();
                
                ApplicationBarIconButton btSelect = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative));
                btSelect.Text = AppResources.Select;
                btSelect.Click += btSelect_Click;
                ApplicationBar.Buttons.Add(btSelect);
                ApplicationBarIconButton btDelete = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative));
                btDelete.Text = AppResources.Delete;
                btDelete.Click += btDelete_Click;
                ApplicationBar.Buttons.Add(btDelete);
                ApplicationBarIconButton btSignOut = new ApplicationBarIconButton(new Uri("/Assets/AppBar/SignOut.png", UriKind.Relative));
                btSignOut.Text = AppResources.SignOut;
                btSignOut.Click += btSignOut_Click;
                ApplicationBar.Buttons.Add(btSignOut);
                ApplicationBarIconButton btDelAcc = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
                btDelAcc.Text = AppResources.DeleteAccount;
                btDelAcc.Click += btDelAcc_Click;
                ApplicationBar.Buttons.Add(btDelAcc);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btDelAcc_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(AppResources.DelAccMsg, AppResources.Attention, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    prgBar.IsIndeterminate = true;
                    prgBar.Visibility = Visibility.Visible;
                    Rec.Visibility = Visibility.Visible;


                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("User", user);
                    parameters.Add("Password", password);


                    PostClient delProxy = new PostClient(parameters);
                    delProxy.DownloadStringCompleted += delProxy_DownloadStringCompleted;
                    //Newtonsoft.Json.Converters.JavaScriptDateTimeConverter();
                    delProxy.DownloadStringAsync(new Uri("http://justrun.comlu.com/DelAcc.php", UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void delProxy_DownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    string result = e.Result.ToString();
                    string startString = "<JustRunServerResponse>";
                    int start = result.IndexOf(startString) + startString.Length;
                    int length = result.IndexOf(("</JustRunServerResponse>")) - start;
                    MessageBox.Show(result.Substring(start, length));
                    IsolatedStorageSettings.ApplicationSettings["Login"] = "";
                    IsolatedStorageSettings.ApplicationSettings["Password"] = "";
                    ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync = false;
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                    else NavigationService.Navigate(new Uri("/pivotMainpage.xaml", UriKind.Relative));

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

        void btDelete_Click(object sender, EventArgs e)
        {
            if (longListMultiSelector.SelectedItems.Count != 0)
                if (MessageBox.Show(AppResources.DeleteConfirmMsg, AppResources.Attention, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var newData = new List<ARunData>();
                    for (int i = 0; i < data.Count; i++)
                        newData.Add(data[i]);
                    var selectedItems = longListMultiSelector.SelectedItems;
                    foreach (ARunData item in selectedItems)
                        newData.Remove(data.First(p => p.No == item.No));
                    UploadData(newData);
                }
        }

        private void UploadData(List<ARunData> LocalData)
        {
            prgBar.IsIndeterminate = true;
            prgBar.Visibility = Visibility.Visible;
            Rec.Visibility = Visibility.Visible;

            var uploadData = new List<ARunUploadData>();
            if (LocalData != null)
                foreach (var item in LocalData)
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

        }

        private void proxy_UploadDownloadStringCompleted(object sender, WindowsPhonePostClient.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    string result = e.Result.ToString();
                    string startString = "<JustRunServerResponse>";
                    int start = result.IndexOf(startString) + startString.Length;
                    int length = result.IndexOf(("</JustRunServerResponse>")) - start;
                    if (result.Substring(start, length) == "Successfully updated your run data!")
                    {
                        MessageBox.Show(AppResources.SuccessfullyUpdated);
                        DownloadData();
                        this.longListMultiSelector.ItemsSource = this.GetRunGroups();
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
        }
        void btSelect_Click(object sender, EventArgs e)
        {
            longListMultiSelector.EnforceIsSelectionEnabled = !longListMultiSelector.EnforceIsSelectionEnabled;
        }

        void btSignOut_Click(object sender, EventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Login"] = "";
            IsolatedStorageSettings.ApplicationSettings["Password"] = "";
            IsolatedStorageSettings.ApplicationSettings.Save();
            ((AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"])._Sync = false;
            IsolatedStorageSettings.ApplicationSettings.Save();
            NavigationService.Navigate(new Uri("/login.xaml", UriKind.Relative));
        }
        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var grid = (Grid)sender;

                TextBlock tb = (TextBlock)grid.FindName("tbItemNo");
                ARunData item = data.Single(p => p.No == int.Parse(tb.Text));

                if (!IsolatedStorageSettings.ApplicationSettings.Contains("TempData"))
                    IsolatedStorageSettings.ApplicationSettings.Add("TempData", item);
                else
                    IsolatedStorageSettings.ApplicationSettings["TempData"] = item;

                NavigationService.Navigate(new Uri("/PrevRun.xaml?item=true", UriKind.Relative));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private List<Group<ARunData>> GetRunGroups()
        {
            IEnumerable<ARunData> runList = data;
            return GetItemGroups(runList, c => c.datetime.ToShortDateString());
        }

        private static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
        {
            IEnumerable<Group<T>> groupList = from item in itemList
                                              group item by getKeyFunc(item) into g
                                              orderby g.Key descending
                                              select new Group<T>(g.Key, g);

            return groupList.ToList();
        }

    }
}