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

namespace Map
{
    public partial class Settings : PhoneApplicationPage
    {
        AppSettings newAppSettings = new AppSettings();
        public Settings()
        {
            this.Loaded += Settings_Loaded;
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            AppSettings _appSettings = (AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"];
            newAppSettings._3DObjects = _appSettings._3DObjects;
            newAppSettings._DefaultPitchLevel = _appSettings._DefaultPitchLevel;
            newAppSettings._DefaultZoomLevel = _appSettings._DefaultZoomLevel;
            newAppSettings._language = _appSettings._language;
            newAppSettings._AutoHeading = _appSettings._AutoHeading;
            newAppSettings._Sync = _appSettings._Sync;
            LoadUI();
            sldPitchLevel.ValueChanged += sldPitchLevel_ValueChanged;
            sldZoomLevel.ValueChanged += sldZoomLevel_ValueChanged;
            rdbtLangUS.Checked += rdbtLangUS_Checked;
            rdbtLangVN.Checked += rdbtLangVN_Checked;
            cbx3DObjects.Checked += cbx3DObjects_Checked;
            cbx3DObjects.Unchecked += cbx3DObjects_Unchecked;
            cbxAutoHeading.Checked += cbxAutoHeading_Checked;
            cbxAutoHeading.Unchecked += cbxAutoHeading_Unchecked;
            tbtnBackup.Checked += tbtnBackup_Checked;
            tbtnBackup.Unchecked += tbtnBackup_Unchecked;
            BuildLocalizedApplicationBar();
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("Login"))
                tbAcc.Text = "";
            else
                tbAcc.Text = (string)IsolatedStorageSettings.ApplicationSettings["Login"];
        }

        void tbtnBackup_Unchecked(object sender, RoutedEventArgs e)
        {
            newAppSettings._Sync = false;
        }

        void tbtnBackup_Checked(object sender, RoutedEventArgs e)
        {
           // if (IsolatedStorageSettings.ApplicationSettings.Contains("Login") && IsolatedStorageSettings.ApplicationSettings["Login"] != null)
                newAppSettings._Sync = true;
        }

        void cbxAutoHeading_Unchecked(object sender, RoutedEventArgs e)
        {
            newAppSettings._AutoHeading = false;
        }

        void cbxAutoHeading_Checked(object sender, RoutedEventArgs e)
        {
            newAppSettings._AutoHeading = true;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.Text = AppResources.Save;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn.Text = AppResources.Cancel;
            btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn.Text = AppResources.Default;

        }
        void LoadUI()
        {

            tbtnBackup.IsChecked = newAppSettings._Sync;
            rdbtLangUS.IsChecked = (newAppSettings._language == "en-US");
            rdbtLangVN.IsChecked = (newAppSettings._language == "vi-VN");
            cbx3DObjects.IsChecked = newAppSettings._3DObjects;
            cbxAutoHeading.IsChecked = newAppSettings._AutoHeading;
            sldPitchLevel.Value = newAppSettings._DefaultPitchLevel;
            sldZoomLevel.Value = newAppSettings._DefaultZoomLevel;

        }
        private void rdbtLangVN_Checked(object sender, RoutedEventArgs e)
        {
            newAppSettings._language = "vi-VN";
        }

        private void rdbtLangUS_Checked(object sender, RoutedEventArgs e)
        {
            newAppSettings._language = "en-US";
        }

        private void btSave_AB_Click(object sender, EventArgs e)
        {
            SaveSettings();
            //AppSettings oldSettings = (AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"];
            if ((IsolatedStorageSettings.ApplicationSettings.Contains("Login") && (string)IsolatedStorageSettings.ApplicationSettings["Login"] != "") || newAppSettings._Sync == false)
                NavigationService.GoBack();
            else
            {
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            }
        }

        private void SaveSettings()
        {
            try
            {
                AppSettings oldSettings = (AppSettings)IsolatedStorageSettings.ApplicationSettings["AppSettings"];

                if (oldSettings._language != newAppSettings._language)
                    MessageBox.Show(AppResources.ChangingLanguageWarning);

                IsolatedStorageSettings.ApplicationSettings["AppSettings"] = newAppSettings;

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btCancel_AB_Click(object sender, EventArgs e)
        {

            NavigationService.GoBack();
        }

        private void btDefault_AB_Click(object sender, EventArgs e)
        {

            //settings.Clear();
            newAppSettings = new AppSettings();
            SaveSettings();
            LoadUI();
        }


        private void cbx3DObjects_Checked(object sender, RoutedEventArgs e)
        {
            newAppSettings._3DObjects = true;
        }

        private void cbx3DObjects_Unchecked(object sender, RoutedEventArgs e)
        {
            newAppSettings._3DObjects = false;
        }

        private void sldZoomLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            newAppSettings._DefaultZoomLevel = e.NewValue;
        }

        private void sldPitchLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            newAppSettings._DefaultPitchLevel = e.NewValue;
        }

        private void tbAcc_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/login.xaml", UriKind.Relative));
        }


    }
}